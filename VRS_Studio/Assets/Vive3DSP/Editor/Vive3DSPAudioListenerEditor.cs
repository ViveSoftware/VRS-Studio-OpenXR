//====================== Copyright 2017-2024, HTC.Corporation. All rights reserved. ======================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

namespace HTC.UnityPlugin.Vive3DSP
{
    [CustomEditor(typeof(Vive3DSPAudioListener))]
    public class Vive3DSPAudioListenerEditor : Editor
    {
        private SerializedProperty globalGain = null;
        private SerializedProperty hrtfModel = null;
        private SerializedProperty headsetConfig = null;
        private SerializedProperty isRecordToFile = null;
        private SerializedProperty fname = null;
        private SerializedProperty isAddWatermark = null;
        private SerializedProperty watermarkString = null;


        private GUIContent globalGainLabel = new GUIContent("Global Gain (dB)",
            "Set the global gain of the system");
        private GUIContent hrtfModelLabel = new GUIContent("Spatial Model",
            "Set the hrtf model for spatial effect");
        private GUIContent headsetConfigLabel = new GUIContent("Headset Config",
            "Set the headset to compensate");
        private GUIContent isRecordToFileLabel = new GUIContent(
            "Record Audio Listener",
            "Check the box to record");
        private GUIContent isAddWatermarkLabel = new GUIContent(
            "Add watermark",
            "Check the box to add watermark");
        private GUIContent watermarkLabel = new GUIContent(
            "Watermark",
            "Set the watermark you would like to add");


        private string export_filepath = "";

        void OnEnable()
        {
            globalGain = serializedObject.FindProperty("globalGain");
            hrtfModel = serializedObject.FindProperty("hrtfModel");
            headsetConfig = serializedObject.FindProperty("headsetConfig");
            isRecordToFile = serializedObject.FindProperty("isRecordToFile");
            fname = serializedObject.FindProperty("fname");
            isAddWatermark = serializedObject.FindProperty("isAddWatermark");
            watermarkString = serializedObject.FindProperty("watermarkString");
        }

        public override void OnInspectorGUI()
        {
            Vive3DSPAudioListener model = target as Vive3DSPAudioListener;
            if(model == null) {return; }
            serializedObject.Update();

            EditorGUILayout.Slider(globalGain, -24.0f, 24.0f, globalGainLabel);
            EditorGUILayout.Separator();

            if (Application.isPlaying)
                GUI.enabled = false;
            else
                GUI.enabled = true;
            EditorGUILayout.PropertyField(hrtfModel, hrtfModelLabel);
            EditorGUILayout.PropertyField(headsetConfig, headsetConfigLabel);

            GUI.enabled = true;
            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(isRecordToFile, isRecordToFileLabel);
            if (GUILayout.Button(
                "Set path", GUILayout.Width(120), GUILayout.Height(20)))
            {
                export_filepath = EditorUtility.SaveFilePanel(
                    "Record to wav file", Application.dataPath, "", "wav");
                model.SetRecordWavFileName(export_filepath);
            }
            if (fname.stringValue == "")
                GUILayout.Label("Path: << Please set the file path. >>");
            else
                GUILayout.Label("Path: " + fname.stringValue);

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Separator();
            if (Application.isPlaying)
                GUI.enabled = false;
            else
                GUI.enabled = true;

            EditorGUILayout.PropertyField(isAddWatermark, isAddWatermarkLabel);
            if (isAddWatermark.boolValue)
            {
++EditorGUI.indentLevel;
                GUIStyle myStyle = GUI.skin.GetStyle("HelpBox");
                myStyle.richText = true;
           
                EditorGUILayout.PropertyField(watermarkString, watermarkLabel);
                Regex RgxUrl = new Regex("[^A-Za-z0-9]");
                if (RgxUrl.IsMatch(watermarkString.stringValue))
                    EditorGUILayout.HelpBox("<b>ERROR: Invalid watermark format.</b>\nPlease follow the requirement of the watermark, or watermark will not be encoded.", MessageType.Error);
                Regex RgxlLowerUrl = new Regex("[a-z]");
                if (RgxlLowerUrl.IsMatch(watermarkString.stringValue))
                {
                    EditorGUILayout.HelpBox("The lowercase letters have been automatically converted to uppercase letters.", MessageType.Warning);
                    watermarkString.stringValue = watermarkString.stringValue.ToUpper();
                }
                if (watermarkString.stringValue == "")
                    EditorGUILayout.HelpBox("<b>ERROR: Invalid watermark format.</b>\nPlease enter at least 1 character.", MessageType.Error);
                if (watermarkString.stringValue.Length > 8)
                {
                    EditorGUILayout.HelpBox("<b>ERROR: Invalid watermark format.</b>\nPlease do not exceed 8 characters.", MessageType.Error);
                    watermarkString.stringValue = watermarkString.stringValue.Substring(0, 8);
                }
                EditorGUILayout.HelpBox("Requirement of the watermark:\n - The length should between 1-8 charactor.\n - Numeric character 0-9 and uppercase letter A-Z. \n  (Any lowercase letter will be automatically converted to uppercase letter.)", MessageType.Info);

                --EditorGUI.indentLevel;
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}