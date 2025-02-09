Shader "Hidden/HDRenderPipeline/DrawTransmittanceGraph"
{
    SubShader
    {
        Pass
        {
            Cull   Off
            ZTest  Off
            ZWrite Off
            Blend  Off

            HLSLPROGRAM
            #pragma target 4.5
            #pragma only_renderers d3d11 ps4 metal // TEMP: until we go further in dev

            #pragma vertex Vert
            #pragma fragment Frag

            //-------------------------------------------------------------------------------------
            // Include
            //-------------------------------------------------------------------------------------

            #include "../../../ShaderLibrary/CommonMaterial.hlsl"
            #include "../../../ShaderLibrary/Common.hlsl"
            #define USE_LEGACY_UNITY_MATRIX_VARIABLES
            #include "../../ShaderVariables.hlsl"

            //-------------------------------------------------------------------------------------
            // Inputs & outputs
            //-------------------------------------------------------------------------------------

            uint   _UseDisneySSS;
            float4 _HalfRcpVarianceAndWeight1, _HalfRcpVarianceAndWeight2;
            float4 _ShapeParam, _TransmissionTint, _ThicknessRemap;

            //-------------------------------------------------------------------------------------
            // Implementation
            //-------------------------------------------------------------------------------------

            struct Attributes
            {
                float3 vertex   : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct Varyings
            {
                float4 vertex   : SV_POSITION;
                float2 texcoord : TEXCOORD0;
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;
                output.vertex   = TransformWorldToHClip(input.vertex);
                output.texcoord = input.texcoord.xy;
                return output;
            }

            float4 Frag(Varyings input) : SV_Target
            {
                float  d = (_ThicknessRemap.x + input.texcoord.x * (_ThicknessRemap.y - _ThicknessRemap.x));
                float3 T;

                if (_UseDisneySSS)
                {
                    T = ComputeTransmittance(_ShapeParam.rgb, float3(0.25, 0.25, 0.25), d, 1);
                }
                else
                {
                    T = ComputeTransmittanceJimenez(_HalfRcpVarianceAndWeight1.rgb,
                                                    _HalfRcpVarianceAndWeight1.a,
                                                    _HalfRcpVarianceAndWeight2.rgb,
                                                    _HalfRcpVarianceAndWeight2.a,
                                                    float3(0.25, 0.25, 0.25), d, 1);
                }

                // Apply gamma for visualization only. Do not apply gamma to the color.
                return float4(sqrt(T) * _TransmissionTint.rgb, 1);
            }
            ENDHLSL
        }
    }
    Fallback Off
}
