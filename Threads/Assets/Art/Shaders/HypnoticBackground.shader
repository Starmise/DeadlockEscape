Shader "Custom/HypnoticBackground"
{
    Properties
    {
        _ColorA ("Color A", Color) = (0.05,0.1,0.2,1)
        _ColorB ("Color B", Color) = (0.4,0.25,0.6,1)

        _Speed ("Global Speed", Float) = 0.2
        _WaveStrength ("Wave Strength", Float) = 0.4
        _WaveFrequency ("Wave Frequency", Float) = 3.0
        _WaveSpeed ("Wave Speed", Float) = 1.0

        _NoiseScale ("Noise Scale", Float) = 2.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Background" }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 _ColorA;
            float4 _ColorB;

            float _Speed;
            float _WaveStrength;
            float _WaveFrequency;
            float _WaveSpeed;
            float _NoiseScale;

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453);
            }

            float noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);

                float a = hash(i);
                float b = hash(i + float2(1, 0));
                float c = hash(i + float2(0, 1));
                float d = hash(i + float2(1, 1));

                float2 u = f * f * (3.0 - 2.0 * f);
                return lerp(a, b, u.x) +
                       (c - a) * u.y * (1.0 - u.x) +
                       (d - b) * u.x * u.y;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                float t = _Time.y * _Speed;

                float2 uv = IN.uv * _NoiseScale;

                // MULTI-WAVE DISTORTION
                float wave1 = sin((uv.x + t * _WaveSpeed) * _WaveFrequency);
                float wave2 = sin((uv.y - t * _WaveSpeed * 0.8) * (_WaveFrequency * 1.3));
                float wave3 = sin((uv.x + uv.y + t * _WaveSpeed * 0.5) * (_WaveFrequency * 0.7));

                float wave = (wave1 + wave2 + wave3) / 3.0;

                uv += wave * _WaveStrength;

                float n = noise(uv + t);
                float blend = smoothstep(0.25, 0.75, n);

                float4 color = lerp(_ColorA, _ColorB, blend);
                return color;
            }
            ENDHLSL
        }
    }
}
