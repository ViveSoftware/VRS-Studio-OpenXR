using UnityEditor;

namespace HTC.UnityPlugin.Vive3DSP
{
    [InitializeOnLoad]
    public static class Vive3DSPRecommendedSettings
    {
        static Vive3DSPRecommendedSettings()
        {
            Vive3DSPVersionCheck.AddRecommendedSetting(new Vive3DSPVersionCheck.RecommendedSetting<string>()
            {
                settingTitle = "Audio Spatializer Plugin",
                currentValueFunc = () =>
                {
                    var audioSettings = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/AudioManager.asset")[0]);
                    var spatializerProp = audioSettings.FindProperty("m_SpatializerPlugin");
                    var v = spatializerProp.stringValue;
                    return string.IsNullOrEmpty(v) ? "None" : v;
                },
                setValueFunc = (v) =>
                {
                    var audioSettings = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/AudioManager.asset")[0]);
                    var spatializerProp = audioSettings.FindProperty("m_SpatializerPlugin");
                    spatializerProp.stringValue = v;
                    audioSettings.ApplyModifiedProperties();
                },
                recommendedValue = "VIVE 3DSP Audio",
            });

#if UNITY_2017_1_OR_NEWER
            Vive3DSPVersionCheck.AddRecommendedSetting(new Vive3DSPVersionCheck.RecommendedSetting<string>()
            {
                settingTitle = "Ambisonic Decoder Plugin",
                currentValueFunc = () =>
                {
                    var audioSettings = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/AudioManager.asset")[0]);
                    var spatializerProp = audioSettings.FindProperty("m_AmbisonicDecoderPlugin");
                    var v = spatializerProp.stringValue;
                    return string.IsNullOrEmpty(v) ? "None" : v;
                },
                setValueFunc = (v) =>
                {
                    var audioSettings = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/AudioManager.asset")[0]);
                    var spatializerProp = audioSettings.FindProperty("m_AmbisonicDecoderPlugin");
                    spatializerProp.stringValue = v;
                    audioSettings.ApplyModifiedProperties();
                },
                recommendedValue = "VIVE 3DSP Audio",
            });
#endif
        }
    }
}