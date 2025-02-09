﻿namespace UnityEngine.Experimental.Rendering
{
    public enum LightArchetype { Punctual, Area };

    public enum SpotLightShape { Cone, Pyramid, Box };

    //@TODO: We should continuously move these values
    // into the engine when we can see them being generally useful
    [RequireComponent(typeof(Light))]
    public class HDAdditionalLightData : MonoBehaviour
    {
        [Range(0.0F, 100.0F)]
        public float m_innerSpotPercent = 0.0f; // To display this field in the UI this need to be public

        public float GetInnerSpotPercent01()
        {
            return Mathf.Clamp(m_innerSpotPercent, 0.0f, 100.0f) / 100.0f;
        }

        [Range(0.0F, 1.0F)]
        public float lightDimmer = 1.0f;

        // Not used for directional lights.
        public float fadeDistance = 10000.0f;

        public bool affectDiffuse = true;
        public bool affectSpecular = true;

        public LightArchetype archetype = LightArchetype.Punctual;
        public SpotLightShape spotLightShape = SpotLightShape.Cone; // Note: Only for Spotlight, should be hide for other light

        [Range(0.0f, 20.0f)]
        public float lightLength = 0.0f; // Area & projector lights

        [Range(0.0f, 20.0f)]
        public float lightWidth  = 0.0f; // Area & projector lights

        [Range(0.0f, 1.0f)]
        public float maxSmoothness = 1.0f; // this is use with punctual light to fake an area lights

        public bool applyRangeAttenuation = true; // If true, we apply the smooth attenuation factor on the range attenuation to get 0 value, else the attenuation is juste inverse square and never reach 0
    }
}
