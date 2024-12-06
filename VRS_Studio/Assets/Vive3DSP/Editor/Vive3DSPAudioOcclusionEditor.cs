//====================== Copyright 2017-2024, HTC.Corporation. All rights reserved. ======================
using UnityEngine;
using UnityEditor;

namespace HTC.UnityPlugin.Vive3DSP
{
    [CustomEditor(typeof(Vive3DSPAudioOcclusion))]
    [CanEditMultipleObjects]
    public class Vive3DSPAudioOcclusionEditor : Editor
    {
        private SerializedProperty occlusionEffect = null;
        private SerializedProperty occlusionMaterial = null;
        private SerializedProperty occlusionIntensity = null;
        private SerializedProperty highFreqAttenuation = null;
        private SerializedProperty lowFreqAttenuationRatio = null;
        private SerializedProperty occlusionGeometry = null;
        private SerializedProperty occlusionRadius = null;
        private SerializedProperty occlusionSize = null;
        private SerializedProperty occlusionCenter = null;
        private SerializedProperty occlusionComputeMode = null;
        private SerializedProperty occlusionHeight = null;
        private SerializedProperty occlusionRotation = null;
        private SerializedProperty occlusionAngle = null;

        private GUIContent occlusionEffectLabel = new GUIContent("Occlusion Effect",
            "ON or OFF occlusion effect");
        private GUIContent occlusionMaterialLabel = new GUIContent("Occlusion Material",
            "Set material for occlusion object");
        private GUIContent occlusionIntensityLabel = new GUIContent("Occlusion Intensity",
            "Set occlusion intensity");
        private GUIContent highFreqAttenuationTapLabel = new GUIContent("High Freq. Attenuation (dB)",
            "Set high frequency attenuation level, default cut-off frequency is 5kHz");
        private GUIContent lowFreqAttenuationRatioTapLabel = new GUIContent("Low Freq. Attenuation Ratio",
            "Set low frequency attenuation ratio");
        private GUIContent highFreqAttenuationLabel = new GUIContent(" ",
            "Set high frequency attenuation level, default cut-off frequency is 5kHz");
        private GUIContent lowFreqAttenuationRatioLabel = new GUIContent(" ",
            "Set low frequency attenuation ratio");
        private GUIContent surfaceMaterialLabel = new GUIContent("Room Surface Material",
            "Set room surface materials for reverb effect");
        private GUIContent occlusionRadiusLabel = new GUIContent("Occlusion Radius",
            "Set sphere occlusion radius");
        private GUIContent occlusionSizeLabel = new GUIContent("Occlusion Size",
            "Set box occlusion size");
        private GUIContent occlusionCenterLabel = new GUIContent("Occlusion Center",
            "Set occlusion center");
        private GUIContent occlusionComputeModeLabel = new GUIContent("Occlusion Compute Mode",
            "Set compute mode for occlusion object");
        private GUIContent occlusionHeightLabel = new GUIContent("Occlusion Height",
            "Set cylineder occlusion height");
        private GUIContent occlusionRotationLabel = new GUIContent("Occlusion Rotation",
            "Set cylineder occlusion rotation");
        private GUIContent occlusionAngleLabel = new GUIContent("Occlusion Angle (degree)",
            "Set occlusion cover angle in degree");

        void OnEnable()
        {
            occlusionEffect = serializedObject.FindProperty("occlusionEffect");
            occlusionMaterial = serializedObject.FindProperty("occlusionMaterial");
            occlusionIntensity = serializedObject.FindProperty("occlusionIntensity");
            highFreqAttenuation = serializedObject.FindProperty("highFreqAttenuation");
            lowFreqAttenuationRatio = serializedObject.FindProperty("lowFreqAttenuationRatio");
            occlusionGeometry = serializedObject.FindProperty("occlusionGeometry");
            occlusionRadius = serializedObject.FindProperty("occlusionRadius");
            occlusionSize = serializedObject.FindProperty("occlusionSize");
            occlusionCenter = serializedObject.FindProperty("occlusionCenter");
            occlusionComputeMode = serializedObject.FindProperty("occlusionComputeMode");
            occlusionHeight = serializedObject.FindProperty("occlusionHeight");
            occlusionRotation = serializedObject.FindProperty("occlusionRotation");
            occlusionAngle = serializedObject.FindProperty("occlusionAngle");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(occlusionEffect, occlusionEffectLabel);
            EditorGUILayout.PropertyField(occlusionMaterial, occlusionMaterialLabel);
            if (occlusionMaterial.enumValueIndex == (int)OccMaterial.UserDefine)
            {
                ++EditorGUI.indentLevel;
                EditorGUILayout.LabelField(highFreqAttenuationTapLabel);
                ++EditorGUI.indentLevel;
                EditorGUILayout.Slider(highFreqAttenuation, -50.0f, 0.0f, highFreqAttenuationLabel);
                --EditorGUI.indentLevel;
                EditorGUILayout.LabelField(lowFreqAttenuationRatioTapLabel);
                ++EditorGUI.indentLevel;
                EditorGUILayout.Slider(lowFreqAttenuationRatio, 0.0f, 1.0f, lowFreqAttenuationRatioLabel);
                --EditorGUI.indentLevel;
                --EditorGUI.indentLevel;
            }
            EditorGUILayout.Slider(occlusionIntensity, 1.0f, 2.0f, occlusionIntensityLabel);
            EditorGUILayout.Slider(occlusionAngle, 15.0f, 45.0f, occlusionAngleLabel);
                        
            occlusionGeometry.enumValueIndex = EditorGUILayout.Popup("Occlusion Geometry", occlusionGeometry.enumValueIndex, new string[] { "Sphere", "Box", "Cylinder"});
            
            if (occlusionGeometry.enumValueIndex == (int)OccGeometryMode.Box)
            {
                EditorGUILayout.PropertyField(occlusionComputeMode, occlusionComputeModeLabel);
            }
            
            EditorGUILayout.PropertyField(occlusionCenter, occlusionCenterLabel);

            if ((occlusionGeometry.enumValueIndex == (int)OccGeometryMode.Sphere) ||
                (occlusionGeometry.enumValueIndex == (int)OccGeometryMode.Cylinder))
            {
                EditorGUILayout.PropertyField(occlusionRadius, occlusionRadiusLabel);
                if (occlusionRadius.floatValue < 0) occlusionRadius.floatValue = 0;
            }

            if (occlusionGeometry.enumValueIndex == (int)OccGeometryMode.Box)
            {
                EditorGUILayout.PropertyField(occlusionSize, occlusionSizeLabel);   
            }

            if (occlusionGeometry.enumValueIndex == (int)OccGeometryMode.Cylinder)
            {
                EditorGUILayout.PropertyField(occlusionHeight, occlusionHeightLabel);
                EditorGUILayout.PropertyField(occlusionRotation, occlusionRotationLabel);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawSurfaceMaterial(SerializedProperty surfaceMaterial)
        {
            surfaceMaterialLabel.text = surfaceMaterial.displayName;
            EditorGUILayout.PropertyField(surfaceMaterial, surfaceMaterialLabel);
        }
    }
}

