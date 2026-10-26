
Shader "Custom/Preview_Mapping"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}

        [HDR] _ColorA ("Stripe Color A", Color) = (0, 0.8, 1, 0.5)
        [HDR] _ColorB ("Stripe Color B", Color) = (0.2, 0.3, 1, 0.5)

        _StripeWidth ("Stripe Width", Range(0.01, 2)) = 0.15
        _GapWidth ("Gap Width", Range(0.01, 2)) = 0.1

        _Speed ("Animation Speed", Float) = 1
        _Direction ("Animation Direction", Range(0, 360)) = 45

        _Alpha ("Overall Opacity", Range(0, 1)) = 0.5

        [HDR] _BorderColor ("Inner Border Color", Color) = (1, 0, 0, 1)
        _BorderWidth ("Inner Border Width", Range(0.001, 0.2)) = 0.03

        [HideInInspector] _RendererColor ("Renderer Color", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [HideInInspector] _AlphaTex ("External Alpha", 2D) = "white" {}
        [HideInInspector] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        ZWrite On
        ZTest LEqual
        Blend One OneMinusSrcAlpha

        Pass
        {
            Tags { "LightMode" = "Universal2D" }

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                float2 worldPos : TEXCOORD1;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;

                float4 _ColorA;
                float4 _ColorB;

                float _StripeWidth;
                float _GapWidth;

                float _Speed;
                float _Direction;

                float _Alpha;

                float4 _BorderColor;
                float _BorderWidth;
            CBUFFER_END

            Varyings vert(Attributes v)
            {
                Varyings o;

                float3 worldPos = TransformObjectToWorld(v.positionOS.xyz);

                o.positionCS = TransformWorldToHClip(worldPos);
                o.worldPos = worldPos.xy;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;

                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                half4 sprite = SAMPLE_TEXTURE2D(
                    _MainTex,
                    sampler_MainTex,
                    i.uv
                );

                // Preserve the sprite's existing transparency.
                half alpha = sprite.a * i.color.a;

                // Static inner border.
                // Assumes the sprite UV range is 0..1.
                float2 borderDist = min(i.uv, 1.0 - i.uv);
                float distanceToEdge = min(
                    borderDist.x,
                    borderDist.y
                );

                float aa = max(
                    fwidth(distanceToEdge),
                    0.0001
                );

                float borderMask = 1.0 - smoothstep(
                    _BorderWidth - aa,
                    _BorderWidth + aa,
                    distanceToEdge
                );

                // Draw the border only where the sprite is visible.
                if (borderMask > 0.5 && alpha > 0.001)
                {
                    half4 result;

                    result.a = alpha * _BorderColor.a;
                    result.rgb = _BorderColor.rgb
                        * i.color.rgb
                        * result.a;

                    return result;
                }

                // Calculate the animated pattern in world space.
                float angle = radians(_Direction);

                float2 direction = float2(
                    cos(angle),
                    sin(angle)
                );

                float distanceAlongPattern =
                    dot(i.worldPos, direction) - _Time.y * _Speed;

                // Each stripe is followed by a transparent gap.
                float period = max(
                    _StripeWidth + _GapWidth,
                    0.001
                );

                float positionInPeriod =
                    frac(distanceAlongPattern / period) * period;

                // Anti-alias the stripe edges.
                float stripeEdge = min(
                    positionInPeriod,
                    _StripeWidth - positionInPeriod
                );

                float stripeAA = max(
                    fwidth(distanceAlongPattern),
                    0.0001
                );

                float stripeMask = smoothstep(
                    -stripeAA,
                    stripeAA,
                    stripeEdge
                );

                // Alternate between the two stripe colors.
                float stripeIndex = floor(
                    distanceAlongPattern / period
                );

                float colorSelector = fmod(
                    abs(stripeIndex),
                    2.0
                );

                half4 stripeColor = lerp(
                    _ColorA,
                    _ColorB,
                    colorSelector
                );

                // Transparent gaps; only the stripes are visible.
                half4 finalColor;

                finalColor.a =
                    alpha * stripeMask * stripeColor.a * _Alpha;

                finalColor.rgb =
                    stripeColor.rgb * i.color.rgb * finalColor.a;

                return finalColor;
            }

            ENDHLSL
        }
    }

    Fallback "Sprites/Default"
}
