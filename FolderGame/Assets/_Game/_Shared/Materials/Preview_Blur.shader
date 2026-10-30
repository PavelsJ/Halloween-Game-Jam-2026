
Shader "UI/PixelateBlur"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        [Header(Pixelation)]
        _PixelSize ("Pixel Size", Range(0.001, 0.1)) = 0.025
        _BlurStrength ("Blur Strength", Range(0, 20)) = 8

        [Header(Circle Mask)]
        _CirclePosition ("Circle Position", Vector) = (0.5, 0.5, 0, 0)
        _CircleRadius ("Circle Radius", Range(0, 1)) = 0.3
        _Feather ("Circle Feather", Range(0, 0.5)) = 0.02
        _InvertMask ("Invert Mask", Float) = 0

        [Header(UI Mask)]
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)]
        _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "UI Pixelate Blur"

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;

            fixed4 _Color;
            float4 _ClipRect;

            float _PixelSize;
            float _BlurStrength;

            float4 _CirclePosition;
            float _CircleRadius;
            float _Feather;
            float _InvertMask;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;

                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert(appdata_t v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.worldPosition = v.vertex;
                o.vertex = UnityObjectToClipPos(v.vertex);

                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color * _Color;

                return o;
            }

            // RectMask2D clipping
            float UnityGet2DClippingCustom(
                float2 position,
                float4 clipRect
            )
            {
                float2 inside = step(clipRect.xy, position)
                              * step(position, clipRect.zw);

                return inside.x * inside.y;
            }

            // Pixelation + Blur
            float4 SampleBlur(float2 uv)
            {
                float pixelSize = max(_PixelSize, 0.0001);

                float2 pixelUV =
                    (floor(uv / pixelSize) + 0.5) * pixelSize;

                float2 offset =
                    _MainTex_TexelSize.xy * _BlurStrength;

                float4 col = 0;

                col += tex2D(_MainTex, pixelUV) * 0.20;

                col += tex2D(
                    _MainTex,
                    pixelUV + float2(offset.x, 0)
                ) * 0.10;

                col += tex2D(
                    _MainTex,
                    pixelUV - float2(offset.x, 0)
                ) * 0.10;

                col += tex2D(
                    _MainTex,
                    pixelUV + float2(0, offset.y)
                ) * 0.10;

                col += tex2D(
                    _MainTex,
                    pixelUV - float2(0, offset.y)
                ) * 0.10;

                col += tex2D(
                    _MainTex,
                    pixelUV + offset
                ) * 0.10;

                col += tex2D(
                    _MainTex,
                    pixelUV - offset
                ) * 0.10;

                col += tex2D(
                    _MainTex,
                    pixelUV + float2(offset.x, -offset.y)
                ) * 0.10;

                col += tex2D(
                    _MainTex,
                    pixelUV + float2(-offset.x, offset.y)
                ) * 0.10;

                return col;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

                float2 uv = i.uv;

                // Original texture
                fixed4 original = tex2D(_MainTex, uv);

                // Pixelated and blurred texture
                fixed4 blurred = SampleBlur(uv);

                // Circular mask
                float dist = distance(
                    uv,
                    _CirclePosition.xy
                );

                float mask = 1.0 - smoothstep(
                    _CircleRadius,
                    _CircleRadius + max(_Feather, 0.0001),
                    dist
                );

                // Invert circle
                mask = lerp(
                    mask,
                    1.0 - mask,
                    _InvertMask
                );

                // Combine original and effect
                fixed4 col = lerp(
                    original,
                    blurred,
                    mask
                );

                // UI Tint
                col *= i.color;

                // RectMask2D support
                #ifdef UNITY_UI_CLIP_RECT

                col.a *= UnityGet2DClippingCustom(
                    i.worldPosition.xy,
                    _ClipRect
                );

                #endif

                // Alpha clipping
                #ifdef UNITY_UI_ALPHACLIP

                clip(col.a - 0.001);

                #endif

                return col;
            }

            ENDHLSL
        }
    }
}
