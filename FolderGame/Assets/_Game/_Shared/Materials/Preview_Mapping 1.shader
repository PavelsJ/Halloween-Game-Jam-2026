
Shader "UI/GlitchMask"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        [Header(Overlay)]
        _OverlayColor ("Overlay Color", Color) = (1,0,0,0.5)
        _Intensity ("Intensity", Range(0,1)) = 1
        _Invert ("Color Inversion", Range(0,1)) = 0

        [Header(Glitch)]
        _GlitchStrength ("Glitch Strength", Range(0,0.1)) = 0.02
        _GlitchSpeed ("Glitch Speed", Range(0,30)) = 10
        _GlitchAmount ("Glitch Amount", Range(0,1)) = 0.5

        [Header(RGB Split)]
        _RGBSplit ("RGB Split", Range(0,0.05)) = 0.01

        [Header(Pixelation)]
        _PixelSize ("Pixel Size", Range(100,2000)) = 500

        [Header(Scanlines)]
        _ScanlineStrength ("Scanline Strength", Range(0,1)) = 0.2
        _ScanlineCount ("Scanline Count", Range(100,2000)) = 600

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
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "CanUseSpriteAtlas"="True"
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
            Name "UI Glitch Mask"

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;

            fixed4 _Color;
            fixed4 _OverlayColor;

            float4 _ClipRect;

            float _Intensity;
            float _Invert;

            float _GlitchStrength;
            float _GlitchSpeed;
            float _GlitchAmount;

            float _RGBSplit;

            float _PixelSize;

            float _ScanlineStrength;
            float _ScanlineCount;

            struct appdata
            {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
            };

            // Random noise
            float GlitchHash(float2 p)
            {
                return frac(
                    sin(dot(p, float2(127.1, 311.7)))
                    * 43758.5453
                );
            }

            v2f vert(appdata v)
            {
                v2f o;

                o.worldPosition = v.vertex;
                o.vertex = UnityObjectToClipPos(v.vertex);

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color * _Color;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float time = _Time.y * _GlitchSpeed;

                // Pixelation
                float pixelSize = max(_PixelSize, 1.0);
                float2 pixelUV = floor(uv * pixelSize) / pixelSize;

                // Random horizontal glitch blocks
                float row = floor(uv.y * 35.0);
                float block = floor(time * 8.0);

                float randomValue = GlitchHash(float2(row, block));
                float randomValue2 = GlitchHash(float2(row + 17.0, block + 5.0));

                // Large horizontal displacement
                float glitchActive = step(1.0 - _GlitchAmount, randomValue);

                float displacement = glitchActive *
                    (randomValue2 - 0.5) *
                    _GlitchStrength * 8.0;

                pixelUV.x += displacement;

                // Occasional full horizontal tearing
                float tearRandom = GlitchHash(float2(floor(time * 3.0), 91.0));
                float tear = step(0.94, tearRandom);

                pixelUV.x += tear * (randomValue2 - 0.5) * 0.15;

                // RGB Split
                float split = _RGBSplit * (1.0 + glitchActive * 3.0);

                float red = tex2D(
                    _MainTex,
                    pixelUV + float2(split, 0)
                ).r;

                float green = tex2D(
                    _MainTex,
                    pixelUV
                ).g;

                float blue = tex2D(
                    _MainTex,
                    pixelUV - float2(split, 0)
                ).b;

                float4 original = tex2D(_MainTex, uv);

                float3 rgb = float3(red, green, blue);

                // Color inversion
                rgb = lerp(rgb, 1.0 - rgb, _Invert);

                // Random black and white noise
                float noise = GlitchHash(
                    floor(uv * float2(180.0, 120.0)) +
                    floor(time * 25.0)
                );

                float noiseMask = step(0.97, noise) * _GlitchAmount;

                rgb = lerp(
                    rgb,
                    float3(noise, noise, noise),
                    noiseMask * 0.8
                );

                // Horizontal white glitch flashes
                float flash = step(
                    0.985,
                    GlitchHash(float2(row, floor(time * 12.0)))
                );

                rgb = lerp(
                    rgb,
                    float3(1.0, 1.0, 1.0),
                    flash * _GlitchAmount * 0.8
                );

                // Red overlay
                rgb = lerp(
                    rgb,
                    _OverlayColor.rgb,
                    _OverlayColor.a
                );

                // Scanlines
                float scanline = sin(uv.y * _ScanlineCount) * 0.5 + 0.5;
                rgb *= 1.0 - scanline * _ScanlineStrength;

                // Final intensity
                float3 finalColor = lerp(
                    original.rgb,
                    rgb,
                    _Intensity
                );

                fixed4 col = fixed4(
                    finalColor,
                    original.a
                );

                col *= i.color;

                #ifdef UNITY_UI_CLIP_RECT

                float2 inside = step(
                    _ClipRect.xy,
                    i.worldPosition.xy
                ) * step(
                    i.worldPosition.xy,
                    _ClipRect.zw
                );

                col.a *= inside.x * inside.y;

                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip(col.a - 0.001);
                #endif

                return col;
            }

            ENDCG
        }
    }
}
