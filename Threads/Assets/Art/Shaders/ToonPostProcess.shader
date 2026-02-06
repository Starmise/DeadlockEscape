Shader "Hidden/ToonPostProcess"
{
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" }
        Pass
        {
            Name "ToonPostProcess"
            ZTest Always
            ZWrite Off
            Cull Off

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

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float _Steps;
            float _Contrast;

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                float3 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv).rgb;

                // luminancia
                float luma = dot(col, float3(0.299, 0.587, 0.114));

                // toon quantization
                luma = floor(luma * _Steps) / _Steps;

                col = col * luma * _Contrast;

                return half4(col, 1);
            }
            ENDHLSL
        }
    }
}