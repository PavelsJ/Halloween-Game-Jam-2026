
Shader "Custom/Outline_Mapping"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}

        [HDR] _LineColor ("Line Color", Color) = (1, 0, 0, 1)
        _LineWidth ("Line Width", Range(0.001, 0.2)) = 0.025
        _LineBlur ("Inner Blur", Range(0.001, 0.1)) = 0.025
        _DashLength ("Dash Length", Range(0.01, 0.99)) = 0.5
        _DashSpacing ("Dash Spacing", Range(0.01, 0.5)) = 0.08
        _DashCount ("Dash Count", Range(4, 100)) = 24
        _Speed ("Animation Speed", Range(-5, 5)) = 1
        _Alpha ("Opacity", Range(0, 1)) = 1

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

        ZWrite Off
        ZTest LEqual
        Blend One OneMinusSrcAlpha
        Cull Off

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
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _LineColor;
                float _LineWidth;
                float _LineBlur;
                float _DashLength;
                float _DashSpacing;
                float _DashCount;
                float _Speed;
                float _Alpha;
            CBUFFER_END

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                half4 sprite = SAMPLE_TEXTURE2D(
                    _MainTex, sampler_MainTex, i.uv
                );

                if (sprite.a <= 0.001)
                    discard;

                float2 uv = saturate(i.uv);

                // Distance to each edge.
                float topDist = 1.0 - uv.y;
                float rightDist = 1.0 - uv.x;
                float bottomDist = uv.y;
                float leftDist = uv.x;

                // Clockwise perimeter coordinate, from 0 to 1.
                // Top: left -> right
                // Right: top -> bottom
                // Bottom: right -> left
                // Left: bottom -> top
                float perimeterPosition;

                if (topDist <= rightDist &&
                    topDist <= bottomDist &&
                    topDist <= leftDist)
                {
                    perimeterPosition = uv.x * 0.25;
                }
                else if (rightDist <= bottomDist &&
                         rightDist <= leftDist)
                {
                    perimeterPosition = 0.25 + (1.0 - uv.y) * 0.25;
                }
                else if (bottomDist <= leftDist)
                {
                    perimeterPosition = 0.5 + (1.0 - uv.x) * 0.25;
                }
                else
                {
                    perimeterPosition = 0.75 + uv.y * 0.25;
                }

                // Fixed dash count distributed around the perimeter.
                float dashPosition = frac(
                    perimeterPosition * _DashCount
                    - _Time.y * _Speed
                );

                float dashAA = max(fwidth(dashPosition), 0.0001);

                float dashMask = 1.0 - smoothstep(
                    _DashLength - dashAA,
                    _DashLength + dashAA,
                    dashPosition
                );

                // Draw the line inside the sprite edges.
                float edgeDistance = min(
                    min(uv.x, 1.0 - uv.x),
                    min(uv.y, 1.0 - uv.y)
                );

               float aa = max(fwidth(edgeDistance), 0.0001);

                // Keep the outer edge antialiased.
                float outerMask = smoothstep(
                    0.0,
                    aa,
                    edgeDistance
                );

                // Fade the line gradually towards the sprite center.
                float innerMask = 1.0 - smoothstep(
                    max(0.0, _LineWidth - _LineBlur),
                    _LineWidth + aa,
                    edgeDistance
                );

                float lineMask = outerMask * innerMask;
                float mask = lineMask * dashMask;

                half alpha = mask
                    * sprite.a
                    * i.color.a
                    * _LineColor.a
                    * _Alpha;

                half4 result;
                result.rgb = _LineColor.rgb * i.color.rgb * alpha;
                result.a = alpha;

                return result;
            }

            ENDHLSL
        }
    }

    Fallback "Sprites/Default"
}
