Shader "Custom/FilmGrain"
{
    Properties
    {
        _FilmGrain ("Film Grain", Range(0, 1)) = 0.15
        _FilmGrainScale ("Grain Size", Range(0.5, 4)) = 1.5

        _FilmDust ("Film Dust", Range(0, 1)) = 0.25
        _FilmScratches ("Film Scratches", Range(0, 1)) = 0.2

        _FilmSpeed ("Film Animation Speed", Range(1, 30)) = 12
        _FilmFlicker ("Film Flicker", Range(0, 0.2)) = 0.025
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
        }

        ZWrite Off
        ZTest Always
        Cull Off
        Blend Off

        Pass
        {
            Name "FilmGrain"

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float _FilmGrain;
                float _FilmGrainScale;
                float _FilmDust;
                float _FilmScratches;
                float _FilmSpeed;
                float _FilmFlicker;
            CBUFFER_END

            float Hash21(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);

                return frac(p.x * p.y);
            }

            float FilmNoise(float2 p, float frame)
            {
                return Hash21(p + frame * 17.13);
            }

            half4 Frag(Varyings input) : SV_Target
            {
                float2 uv = input.texcoord;

                half4 source = SAMPLE_TEXTURE2D_X(
                    _BlitTexture,
                    sampler_LinearClamp,
                    uv
                );

                float frame = floor(_Time.y * _FilmSpeed);

                // -------------------------------------
                // 1. FILM GRAIN
                // -------------------------------------

                float2 grainUV = floor(
                    uv * _ScreenParams.xy /
                    max(_FilmGrainScale, 0.1)
                );

                float grain = FilmNoise(grainUV, frame);

                grain = (grain - 0.5) * 2.0;

                float grainFactor =
                    1.0 + grain * _FilmGrain;

                // -------------------------------------
                // 2. FILM DUST
                // -------------------------------------

                float2 dustUV = uv * float2(85.0, 110.0);

                float2 dustCell = floor(dustUV);
                float2 dustLocal = frac(dustUV) - 0.5;

                float dustRandom = Hash21(
                    dustCell + frame * 3.71
                );

                float dustRadius = lerp(
                    0.035,
                    0.16,
                    Hash21(dustCell + 8.17)
                );

                float dustSpot = 1.0 - smoothstep(
                    dustRadius * 0.55,
                    dustRadius,
                    length(dustLocal)
                );

                dustSpot *= step(0.975, dustRandom);
                dustSpot *= _FilmDust;

                // -------------------------------------
                // 3. VERTICAL SCRATCHES
                // -------------------------------------

                float scratchX = uv.x * 100.0;
                float scratchColumn = floor(scratchX);

                float scratchRandom = Hash21(
                    float2(scratchColumn, frame * 0.37)
                );

                float scratchWidth = lerp(
                    0.015,
                    0.055,
                    Hash21(float2(scratchColumn, 4.71))
                );

                float scratchLine = 1.0 - smoothstep(
                    0.0,
                    scratchWidth,
                    abs(frac(scratchX) - 0.5)
                );

                float scratchSegment = step(
                    0.72,
                    Hash21(
                        float2(
                            scratchColumn * 7.13,
                            floor(uv.y * 18.0) + frame
                        )
                    )
                );

                float scratch = scratchLine *
                    scratchSegment *
                    step(0.94, scratchRandom) *
                    _FilmScratches;

                // -------------------------------------
                // 4. FILM FLICKER
                // -------------------------------------

                float flickerNoise = Hash21(
                    float2(frame, 19.73)
                );

                float flicker = 1.0 +
                    (flickerNoise - 0.5) *
                    _FilmFlicker;

                // -------------------------------------
                // 5. COMBINE EFFECTS
                // -------------------------------------

                float3 color = source.rgb;

                color *= grainFactor * flicker;

                float artifacts = saturate(
                    dustSpot + scratch
                );

                color = lerp(
                    color,
                    float3(1.0, 0.92, 0.78),
                    artifacts
                );

                return half4(color, source.a);
            }

            ENDHLSL
        }
    }
}