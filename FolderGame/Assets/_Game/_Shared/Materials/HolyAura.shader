Shader "Custom/HolyAura"
{
    Properties
    {
        [HDR] _Color ("Aura Color", Color) = (1, 0.78, 0.3, 1)
        _Opacity ("Opacity", Range(0, 1)) = 0.8

        [Header(Radial Slices)]
        _Slices ("Slice Count", Range(4, 64)) = 16
        _InnerRadius ("Inner Radius", Range(0, 2)) = 0.25
        _OuterRadius ("Outer Radius", Range(0.1, 2)) = 0.95
        _EdgeSoftness ("Radial Edge Blur", Range(0.001, 0.3)) = 0.08
        _SliceSoftness ("Slice Edge Blur", Range(0.001, 1)) = 0.04
        _Rotation ("Initial Rotation", Range(0, 360)) = 0

        [Header(Rotation Animation)]
        _RotationSpeed ("Rotation Speed (Degrees/sec)", Float) = 30

        [Header(Outer Radius Pulse)]
        _PulseAmount ("Pulse Amount", Range(0, 1)) = 0.08
        _PulseSpeed ("Pulse Speed (Cycles/sec)", Range(0, 5)) = 0.5

        [Header(Center Circle)]
        _CenterRadius ("Center Circle Radius", Range(0.01, 2)) = 0.12
        _CenterSoftness ("Center Edge Blur", Range(0.001, 1)) = 0.06
        _CenterOpacity ("Center Opacity", Range(0, 1)) = 0.8

        [Header(Glow)]
        _Glow ("Glow Strength", Range(0, 5)) = 1.5
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
        }

        Blend SrcAlpha One
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            fixed4 _Color;

            float _Opacity;
            float _Slices;
            float _InnerRadius;
            float _OuterRadius;
            float _EdgeSoftness;
            float _SliceSoftness;
            float _Rotation;
            float _RotationSpeed;

            float _PulseAmount;
            float _PulseSpeed;

            float _CenterRadius;
            float _CenterSoftness;
            float _CenterOpacity;

            float _Glow;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv * 2.0 - 1.0;
                float radius = length(uv);

                // ------------------------------------
                // AUTOMATIC OUTER RADIUS PULSE
                // ------------------------------------

                // PulseSpeed = number of complete cycles per second.
                float pulse = sin(
                    _Time.y * _PulseSpeed * 6.28318530718
                );

                // Radius oscillates around OuterRadius.
                float animatedOuterRadius = max(
                    0.001,
                    _OuterRadius + pulse * _PulseAmount
                );

                // ------------------------------------
                // OUTER RING
                // ------------------------------------

                float outerMask = 1.0 - smoothstep(
                    max(0.0, animatedOuterRadius - _EdgeSoftness),
                    animatedOuterRadius,
                    radius
                );

                float innerMask = smoothstep(
                    _InnerRadius,
                    _InnerRadius + _EdgeSoftness,
                    radius
                );

                float radialMask = outerMask * innerMask;

                // ------------------------------------
                // AUTOMATIC CLOCKWISE ROTATION
                // ------------------------------------

                float angle = atan2(uv.y, uv.x);

                float animatedRotation =
                    _Rotation + _Time.y * _RotationSpeed;

                angle -= animatedRotation * UNITY_PI / 180.0;

                // ------------------------------------
                // ALTERNATING PIE SLICES
                // ------------------------------------

                float normalizedAngle = frac(
                    angle / (2.0 * UNITY_PI) + 1.0
                );

                float slicePosition = normalizedAngle * _Slices;
                float sliceIndex = floor(slicePosition);
                float sliceFraction = frac(slicePosition);

                float isVisibleSlice =
                    1.0 - fmod(sliceIndex, 2.0);

                float edgeDistance = min(
                    sliceFraction,
                    1.0 - sliceFraction
                );

                float angularMask = smoothstep(
                    0.0,
                    max(_SliceSoftness, 0.001),
                    edgeDistance
                );

                float slicesAlpha = radialMask
                                  * angularMask
                                  * isVisibleSlice;

                // ------------------------------------
                // STATIC CENTER CIRCLE
                // ------------------------------------

                float centerMask = 1.0 - smoothstep(
                    max(0.0, _CenterRadius - _CenterSoftness),
                    _CenterRadius,
                    radius
                );

                float centerAlpha = centerMask * _CenterOpacity;

                // ------------------------------------
                // COMBINE EFFECTS
                // ------------------------------------

                float alpha = max(
                    slicesAlpha * _Opacity,
                    centerAlpha * _Opacity
                );

                alpha *= _Color.a;

                fixed3 emission = _Color.rgb * _Glow;

                return fixed4(emission, saturate(alpha));
            }

            ENDCG
        }
    }

    Fallback Off
}