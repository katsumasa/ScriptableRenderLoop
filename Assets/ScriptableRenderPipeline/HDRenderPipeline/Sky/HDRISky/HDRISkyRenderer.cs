using UnityEngine.Rendering;

namespace UnityEngine.Experimental.Rendering.HDPipeline
{
    public class HDRISkyRenderer : SkyRenderer
    {
        Material m_SkyHDRIMaterial; // Renders a cubemap into a render texture (can be cube or 2D)
        private HDRISkySettings m_HdriSkyParams;

        public HDRISkyRenderer(HDRISkySettings hdriSkyParams)
        {
            m_HdriSkyParams = hdriSkyParams;
        }

        public override void Build()
        {
            m_SkyHDRIMaterial = Utilities.CreateEngineMaterial("Hidden/HDRenderPipeline/Sky/SkyHDRI");
        }

        public override void Cleanup()
        {
            Utilities.Destroy(m_SkyHDRIMaterial);
        }

        public override void SetRenderTargets(BuiltinSkyParameters builtinParams)
        {
            if (builtinParams.depthBuffer == BuiltinSkyParameters.nullRT)
            {
                Utilities.SetRenderTarget(builtinParams.commandBuffer, builtinParams.colorBuffer);
            }
            else
            {
                Utilities.SetRenderTarget(builtinParams.commandBuffer, builtinParams.colorBuffer, builtinParams.depthBuffer);
            }
        }

        public override void RenderSky(BuiltinSkyParameters builtinParams, SkySettings skyParameters, bool renderForCubemap)
        {
            m_SkyHDRIMaterial.SetTexture(HDShaderIDs._Cubemap, m_HdriSkyParams.skyHDRI);
            m_SkyHDRIMaterial.SetVector(HDShaderIDs._SkyParam, new Vector4(m_HdriSkyParams.exposure, m_HdriSkyParams.multiplier, m_HdriSkyParams.rotation, 0.0f));

            builtinParams.commandBuffer.DrawMesh(builtinParams.skyMesh, Matrix4x4.identity, m_SkyHDRIMaterial, 0, renderForCubemap ? 0 : 1);
        }

        public override bool IsSkyValid()
        {
            return m_HdriSkyParams != null && m_SkyHDRIMaterial != null;
        }
    }
}
