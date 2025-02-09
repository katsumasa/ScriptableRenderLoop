using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.Experimental.Rendering.HDPipeline;
using System.IO;
using UnityEngine.Experimental.Rendering;

namespace UnityEditor.Experimental.Rendering.HDPipeline
{
    public class HDRenderPipelineMenuItems
    {
        [UnityEditor.MenuItem("HDRenderPipeline/Add \"Additional Light-shadow Data\" (if not present)")]
        static void AddAdditionalLightData()
        {
            Light[] lights = GameObject.FindObjectsOfType(typeof(Light)) as Light[];

            foreach (Light light in lights)
            {
                // Do not add a component if there already is one.
                if (light.GetComponent<HDAdditionalLightData>() == null)
                {
                    light.gameObject.AddComponent<HDAdditionalLightData>();
                }

                if (light.GetComponent<AdditionalShadowData>() == null)
                {
                    light.gameObject.AddComponent<AdditionalShadowData>();
                }
            }
        }

        [UnityEditor.MenuItem("HDRenderPipeline/Add \"Additional Camera Data\" (if not present)")]
        static void AddAdditionalCameraData()
        {
            Camera[] cameras = GameObject.FindObjectsOfType(typeof(Camera)) as Camera[];

            foreach (Camera camera in cameras)
            {
                // Do not add a component if there already is one.
                if (camera.GetComponent<HDAdditionalCameraData>() == null)
                {
                    camera.gameObject.AddComponent<HDAdditionalCameraData>();
                }
            }
        }

        // This script is a helper for the artists to re-synchronize all layered materials
        [MenuItem("HDRenderPipeline/Synchronize all Layered materials")]
        static void SynchronizeAllLayeredMaterial()
        {
            Object[] materials = Resources.FindObjectsOfTypeAll<Material>();
            foreach (Object obj in materials)
            {
                Material mat = obj as Material;
                if (mat.shader.name == "HDRenderPipeline/InfluenceLayeredLit" || mat.shader.name == "HDRenderPipeline/InfluenceLayeredLitTessellation")
                {
                    InfluenceLayeredLitGUI.SynchronizeAllLayers(mat);
                    EditorUtility.SetDirty(mat);
                }
            }
        }

        static void RemoveMaterialKeywords(Material material)
        {
            string[] keywordsToRemove = material.shaderKeywords;
            foreach (var keyword in keywordsToRemove)
            {
                material.DisableKeyword(keyword);
            }
        }

        // The goal of this script is to help maintenance of data that have already been produced but need to update to the latest shader code change.
        // In case the shader code have change and the inspector have been update with new kind of keywords we need to regenerate the set of keywords use by the material.
        // This script will remove all keyword of a material and trigger the inspector that will re-setup all the used keywords.
        // It require that the inspector of the material have a static function call that update all keyword based on material properties.
        [MenuItem("HDRenderPipeline/Test/Reset all materials keywords")]
        static void ResetAllMaterialKeywords()
        {
            try
            {
                Object[] materials = Resources.FindObjectsOfTypeAll<Material>();
                for (int i = 0, length = materials.Length; i < length; i++)
                {
                    Material mat = materials[i] as Material;

                    EditorUtility.DisplayProgressBar(
                        "Setup materials Keywords...",
                        string.Format("{0} / {1} materials cleaned.", i, length),
                        i / (float)(length - 1));

                    if (mat.shader.name == "HDRenderPipeline/InfluenceLayeredLit" || mat.shader.name == "HDRenderPipeline/InfluenceLayeredLitTessellation")
                    {
                        // We remove all keyword already present
                        RemoveMaterialKeywords(mat);
                        InfluenceLayeredLitGUI.SetupMaterialKeywordsAndPass(mat);
                        EditorUtility.SetDirty(mat);
                    }
                    else if (mat.shader.name == "HDRenderPipeline/Lit" || mat.shader.name == "HDRenderPipeline/LitTessellation")
                    {
                        // We remove all keyword already present
                        RemoveMaterialKeywords(mat);
                        LitGUI.SetupMaterialKeywordsAndPass(mat);
                        EditorUtility.SetDirty(mat);
                    }
                    else if (mat.shader.name == "HDRenderPipeline/Unlit")
                    {
                        // We remove all keyword already present
                        RemoveMaterialKeywords(mat);
                        UnlitGUI.SetupMaterialKeywordsAndPass(mat);
                        EditorUtility.SetDirty(mat);
                    }
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        // Function used only to check performance of data with and without tessellation
        [MenuItem("HDRenderPipeline/Test/Remove tessellation materials (not reversible)")]
        static void RemoveTessellationMaterials()
        {
            Object[] materials = Resources.FindObjectsOfTypeAll<Material>();

            Shader litShader = Shader.Find("HDRenderPipeline/Lit");
            Shader layeredLitShader = Shader.Find("HDRenderPipeline/InfluenceLayeredLit");

            foreach (Object obj in materials)
            {
                Material mat = obj as Material;
                if (mat.shader.name == "HDRenderPipeline/LitTessellation")
                {
                    mat.shader = litShader;
                    // We remove all keyword already present
                    RemoveMaterialKeywords(mat);
                    LitGUI.SetupMaterialKeywordsAndPass(mat);
                    EditorUtility.SetDirty(mat);
                }
                else if (mat.shader.name == "HDRenderPipeline/InfluenceLayeredLitTessellation")
                {
                    mat.shader = layeredLitShader;
                    // We remove all keyword already present
                    RemoveMaterialKeywords(mat);
                    InfluenceLayeredLitGUI.SetupMaterialKeywordsAndPass(mat);
                    EditorUtility.SetDirty(mat);
                }
            }
        }

        [MenuItem("HDRenderPipeline/Export Sky to Image")]
        static void ExportSkyToImage()
        {
            HDRenderPipeline renderpipeline = UnityEngine.Experimental.Rendering.RenderPipelineManager.currentPipeline as HDRenderPipeline;
            if(renderpipeline == null)
            {
                Debug.LogError("HDRenderPipeline is not instantiated.");
                return;
            }

            Texture2D result = renderpipeline.ExportSkyToTexture();
            if(result == null)
            {
                return;
            }

            // Encode texture into PNG
            byte[] bytes = null;
            bytes = result.EncodeToEXR(Texture2D.EXRFlags.CompressZIP);
            Object.DestroyImmediate(result);

            string assetPath = EditorUtility.SaveFilePanel("Export Sky", "Assets", "SkyExport", "exr");
            if (!string.IsNullOrEmpty(assetPath))
            {
                File.WriteAllBytes(assetPath, bytes);
                AssetDatabase.Refresh();
            }
        }
    }
}
