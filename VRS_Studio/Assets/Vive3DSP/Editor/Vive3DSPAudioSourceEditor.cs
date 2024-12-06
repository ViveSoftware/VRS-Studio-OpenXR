//====================== Copyright 2017-2024, HTC.Corporation. All rights reserved. ======================
using UnityEngine;
using UnityEditor;

namespace HTC.UnityPlugin.Vive3DSP
{
    [CustomEditor(typeof(Vive3DSPAudioSource))]
    [CanEditMultipleObjects]
    public class Vive3DSPAudioSourceEditor : Editor
    {
        private SerializedProperty gain = null;
        private SerializedProperty soundDecayEffectSwitch = null;
        private SerializedProperty soundDecayMode = null;
        private SerializedProperty spatializer = null;
        private SerializedProperty reverb = null;
        private SerializedProperty occlusion = null;
        private SerializedProperty occlusionEffectEngine = null;
        private SerializedProperty nearfield = null;
        private SerializedProperty nearInten = null;
        private SerializedProperty minimumDecayVolumeDb = null;
        private SerializedProperty minDistance = null;
        private SerializedProperty maxDistance = null;
        private SerializedProperty sourceDirectivity = null;
        private SerializedProperty shape = null;
        private SerializedProperty focus = null;
        private Texture2D sourceDirectivityPreview = null;

        private GUIContent gainLabel = new GUIContent(
            "Gain (dB)",
            "Set the gain of the sound source");
        private GUIContent soundDecayEffectSwitchLabel = new GUIContent(
            "Overwrite Volume Rolloff",
            "Enable 3DSP sound decay effect to overwrite unity audio source volume rolloff");
        private GUIContent soundDecayModeLabel = new GUIContent(
            "Sound Decay Effect",
            "Set sound decay mode");
        private GUIContent spatializerLabel = new GUIContent(
            "3D Sound Effect",
            "Set the 3D sound effect feature");
        private GUIContent reverbLabel = new GUIContent(
            "Room Effect",
            "Set the reverb effect feature");
        private GUIContent occlusionLabel = new GUIContent(
            "Occlusion Effect",
            "Set the occlusion effect feature");
        private GUIContent occlusionEffectEngineLabel = new GUIContent(
            "Occlusion Effect Engine",
            "Set effect engine");
        private GUIContent nearfieldLabel = new GUIContent(
            "Nearfield Effect",
            "Set the nearfield effect feature");
        private GUIContent nearIntenLable = new GUIContent(
            "Nearfield Effect Intensity",
            "Set the intensity of nearfield effect");
        private GUIContent minimumDecayVolumeTapDbLabel = new GUIContent(
            "Minimum Decay Volume (dB)",
            "Set minimum decay volume");
        private GUIContent minimumDecayVolumeDbLabel = new GUIContent(
            " ",
            "Set minimum decay volume");
        private GUIContent minDistanceLabel = new GUIContent(
            "Minimum Distance (M)",
            "Set minimum distance");
        private GUIContent maxDistanceLabel = new GUIContent(
            "Maximum Distance (M)",
            "Set maximum distance");
        private GUIContent sourceDirectivityLabel = new GUIContent(
            "Source Directivity",
            "Set the source directivity feature and show the directivity field between audio source and listener");
        private GUIContent shapeLabel = new GUIContent(
            "Shape",
            "Set the directivity shape");
        private GUIContent focusLabel = new GUIContent(
            "Focus",
            "Set the focus of directivity");

        void OnEnable()
        {
            Vive3DSPAudioSource model = target as Vive3DSPAudioSource;
            gain = serializedObject.FindProperty("gain");
            soundDecayMode = serializedObject.FindProperty("soundDecayMode");
            soundDecayEffectSwitch = serializedObject.FindProperty("soundDecayEffectSwitch");
            spatializer = serializedObject.FindProperty("spatializer_3d");
            reverb = serializedObject.FindProperty("reverb");
            occlusion = serializedObject.FindProperty("occlusion");
            occlusionEffectEngine = serializedObject.FindProperty("occlusionEffectEngine");
            nearfield = serializedObject.FindProperty("nearfield");
            nearInten = serializedObject.FindProperty("nearInten");
            minimumDecayVolumeDb = serializedObject.FindProperty("minimumDecayVolumeDb");
            minDistance = serializedObject.FindProperty("minDistance");
            maxDistance = serializedObject.FindProperty("maxDistance");
            sourceDirectivity = serializedObject.FindProperty("sourceDirectivity");
            sourceDirectivityPreview = Texture2D.blackTexture;
            shape = serializedObject.FindProperty("shape");
            focus = serializedObject.FindProperty("focus");
        }

        public override void OnInspectorGUI()
        {
            checkSpatializerPlugin();
            checkAudioFileType();
            serializedObject.Update();

            EditorGUILayout.Slider(gain, -24.0f, 24.0f, gainLabel);

            EditorGUILayout.PropertyField(spatializer, spatializerLabel);
            EditorGUILayout.PropertyField(reverb, reverbLabel);

            EditorGUILayout.PropertyField(occlusion, occlusionLabel);
            if (occlusion.boolValue == true)
            {
                ++EditorGUI.indentLevel;
                EditorGUILayout.PropertyField(occlusionEffectEngine, occlusionEffectEngineLabel);
                --EditorGUI.indentLevel;
            }
            
            EditorGUILayout.PropertyField(nearfield, nearfieldLabel);
            if (nearfield.boolValue == true){
                ++EditorGUI.indentLevel;
                EditorGUILayout.Slider(nearInten, 1.0f, 9.0f, nearIntenLable);
                --EditorGUI.indentLevel;
            }

            drawSourceDirectivityInspector();
            drawSoundDecayEffectInspector();
            EditorGUILayout.Separator();
            
            markSceneDirty();
        }
        private void checkSpatializerPlugin()
        {
            if(AudioSettings.GetSpatializerPluginName() != "VIVE 3DSP Audio")
            {
                EditorGUILayout.HelpBox("VIVE 3DSP Audio Spatializer Plugin not found.\nPlease go to the following path and select VIVE 3DSP Audio as Spatializer Plugin:\nEdit>Project Settings>Audio>Spatializer Plugin", MessageType.Warning); 
            }
        }

        private void checkAudioFileType()
        {
            Vive3DSPAudioSource model = target as Vive3DSPAudioSource;
            if (model == null) { return; }

            AudioClip clip = model.audioSource.clip;
            if (clip != null)
            {
                var clipProperty = clip.GetType().GetProperty("ambisonic");
                if (clipProperty != null)
                {
                    if ((bool)clipProperty.GetValue(clip, null))
                    {
                        EditorGUILayout.HelpBox("The audio clip is ambisonic file. Please remove the Vive 3DSP Audio Source and disable spatialize checkbox in audio source.", MessageType.Error);
                        Debug.LogError("The audio clip is ambisonic file. Please remove the Vive 3DSP Audio Source and disable spatialize checkbox in audio source.");
                    }
                }
            }
        }

        private void drawSoundDecayEffectInspector()
        {
            EditorGUILayout.PropertyField(soundDecayEffectSwitch, soundDecayEffectSwitchLabel);
            
            if (soundDecayEffectSwitch.boolValue == true)
            {
                ++EditorGUI.indentLevel;
                EditorGUILayout.PropertyField(soundDecayMode, soundDecayModeLabel);
                if (soundDecayMode.enumValueIndex == (int)SoundDecayMode.PointSourceDecay
                    || soundDecayMode.enumValueIndex == (int)SoundDecayMode.LineSourceDecay
                    || soundDecayMode.enumValueIndex == (int)SoundDecayMode.LinearDecay)
                {
                    ++EditorGUI.indentLevel;
                    EditorGUILayout.LabelField(minimumDecayVolumeTapDbLabel);
                    ++EditorGUI.indentLevel;
                    EditorGUILayout.Slider(minimumDecayVolumeDb, -96.0f, 0.0f, minimumDecayVolumeDbLabel);
                    --EditorGUI.indentLevel;
                    --EditorGUI.indentLevel;
                }

                if (soundDecayMode.enumValueIndex == (int)SoundDecayMode.LinearDecay)
                {
                    ++EditorGUI.indentLevel;

                    // Minimum Effective Distance Input Box
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(minDistanceLabel);
                    ++EditorGUI.indentLevel;
                    float min_distance = 0.0f;
                    string input_str = EditorGUILayout.TextField(minDistance.floatValue.ToString("F3"));
                    bool parsed = float.TryParse(input_str, out min_distance);
                    if (!parsed)
                    {
                        minDistance.floatValue = 0.0f;
                        Debug.LogWarning("The input " + input_str + " may not be a float number.");
                    }
                    minDistance.floatValue = min_distance;
                    EditorGUILayout.EndHorizontal();
                    --EditorGUI.indentLevel;

                    // Maximum Effective Distance Input Box
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(maxDistanceLabel);
                    ++EditorGUI.indentLevel;
                    input_str = EditorGUILayout.TextField(maxDistance.floatValue.ToString("F3"));
                    float max_distance = 1000.0f;
                    parsed = float.TryParse(input_str, out max_distance);
                    if (!parsed)
                    {
                        max_distance = 1000.0f;
                        Debug.LogWarning("The input " + input_str + " may not be a float number.");
                    }
                    else if (max_distance < min_distance)
                    {
                        max_distance = min_distance;
                    }
                    maxDistance.floatValue = max_distance;
                    EditorGUILayout.EndHorizontal();
                    --EditorGUI.indentLevel;
                    --EditorGUI.indentLevel;
                }

                switch (soundDecayMode.enumValueIndex)
                {
                    case (int)SoundDecayMode.PointSourceDecay:
                        EditorGUILayout.HelpBox("The sound source behaves like point source, which the volume decay rate is the inverse of the square of distance.", MessageType.Info);
                        break;
                    case (int)SoundDecayMode.LineSourceDecay:
                        EditorGUILayout.HelpBox("The sound source behaves like line source, which the volume decay rate is the inverse of the distance.", MessageType.Info);
                        break;
                    case (int)SoundDecayMode.LinearDecay:
                        EditorGUILayout.HelpBox("The volume decay rate is 1.0 when the distance is below the minimum effective distance, and it is 0.0 when the distance is above the maximum effective distance. Between them the decay rate is linear decreased.", MessageType.Info);
                        break;
                    default:
                        break;
                }
                --EditorGUI.indentLevel;
            }
            else
            {
                EditorGUILayout.HelpBox("To overwirte Audio Source volume rolloff will enable 3DSP Sound Decay Effect", MessageType.Info);
            }
        }

        private void markSceneDirty()
        {
            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
                if (!Application.isPlaying)
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            }
        }

        private void drawSourceDirectivityInspector()
        {
            EditorGUILayout.PropertyField(sourceDirectivity, sourceDirectivityLabel);
            
            if (sourceDirectivity.boolValue == true)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginVertical();
                ++EditorGUI.indentLevel;
                EditorGUILayout.Slider(shape, 0.0f, 1.0f, shapeLabel);
                EditorGUILayout.Slider(focus, 1.0f, 10.0f, focusLabel);
                EditorGUILayout.EndVertical();
                drawSourceDirectivity(shape.floatValue, focus.floatValue);
                EditorGUILayout.EndHorizontal();
                --EditorGUI.indentLevel;
            }
        }

        private void drawSourceDirectivity(float alpha, float sharpness) {
            int size = (int) (2.0f * EditorGUIUtility.singleLineHeight);
#if UNITY_2021_2_OR_NEWER
            sourceDirectivityPreview.Reinitialize(size, size);
#else
            sourceDirectivityPreview.Resize(size, size);
#endif
            for (int i = 0; i < size; ++i) {
                // Draw x-axis from (0, size/2) to (size, size/2).
                sourceDirectivityPreview.SetPixel(i, size / 2, Color.gray);
                // Draw y-axis from (size/2, 0) to (size/2, size).
                sourceDirectivityPreview.SetPixel(size / 2, i, Color.gray);
            }
            
            Vector2[] sourceDirectivityPoints = Vive3DSPAudio.SetSourceDirectivityPoints(alpha, sharpness, 180);
            for (int i = 0; i < sourceDirectivityPoints.Length; ++i) {
                sourceDirectivityPreview.SetPixel((int) (0.5f * size + 0.45f * size * sourceDirectivityPoints[i].x),
                                                  (int) (0.5f * size + 0.45f * size * sourceDirectivityPoints[i].y), Color.black);
            }
            sourceDirectivityPreview.Apply();
            GUILayout.Box(sourceDirectivityPreview);
        }
    }
}


