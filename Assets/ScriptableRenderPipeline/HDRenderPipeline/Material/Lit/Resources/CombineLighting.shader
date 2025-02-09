Shader "Hidden/HDRenderPipeline/CombineLighting"
{
    SubShader
    {
        Pass
        {
            Stencil
            {
                Ref  1 // StencilLightingUsage.SplitLighting
                Comp Equal
                Pass Keep
            }

            Cull   Off
            ZTest  Always
            ZWrite Off
            Blend  One One // Additive

            HLSLPROGRAM
            #pragma target 4.5
            #pragma only_renderers d3d11 ps4 metal // TEMP: until we go further in dev
            // #pragma enable_d3d11_debug_symbols

            #pragma vertex Vert
            #pragma fragment Frag

            #include "../../../../ShaderLibrary/Common.hlsl"

            TEXTURE2D(_IrradianceSource);

            struct Attributes
            {
                uint vertexID : SV_VertexID;
            };

            struct Varyings
            {
                float4 positionCS : SV_Position;
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;
                output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
                return output;
            }

            float4 Frag(Varyings input) : SV_Target
            {
                return LOAD_TEXTURE2D(_IrradianceSource, input.positionCS.xy);
            }
            ENDHLSL
        }
    }
    Fallback Off
}
