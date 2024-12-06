//====================== Copyright 2017-2024, HTC.Corporation. All rights reserved. ======================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace HTC.UnityPlugin.Vive3DSP
{
    [CustomEditor(typeof(Vive3DSPAudioRoom))]
    [CanEditMultipleObjects]
    public class Vive3DSPAudioRoomEditor : Editor
    {
        private SerializedProperty roomEffect = null;
        private SerializedProperty reverbPreset = null;
        private SerializedProperty Ceiling = null;
        private SerializedProperty FrontWall = null;
        private SerializedProperty BackWall = null;
        private SerializedProperty RightWall = null;
        private SerializedProperty LeftWall = null;
        private SerializedProperty Floor = null;
        private SerializedProperty ceilingReflectionRate = null;
        private SerializedProperty frontWallReflectionRate = null;
        private SerializedProperty backWallReflectionRate = null;
        private SerializedProperty rightWallReflectionRate = null;
        private SerializedProperty leftWallReflectionRate = null;
        private SerializedProperty floorReflectionRate = null;
        private SerializedProperty reflectionLevel = null;
        private SerializedProperty reverbLevel = null;
        private SerializedProperty backgroundType = null;
        private SerializedProperty userDefineClip = null;
        private SerializedProperty sourceVolume = null;
        private SerializedProperty size = null;

        // for artificial reverb
        private GUIContent roomEffectLabel = new GUIContent("Room Effect",
            "Reverb effect enable/disable");
        private GUIContent roomPresetLabel = new GUIContent("Room Reverb Preset",
            "Room Reverb Preset");
        private GUIContent SurfaceMaterialsLabel = new GUIContent("Room Surface Material",
            "Set room surface materials for reverb effect");
        private GUIContent surfaceMaterialLabel = new GUIContent("Room Surface Material",
            "Set room surface materials for reverb effect");
        private GUIContent ceilingReflectionRateLabel = new GUIContent("Reflection Rate",
            "Set reflection rate of the ceiling");
        private GUIContent frontWallReflectionRateLabel = new GUIContent("Reflection Rate",
            "Set reflection rate of the front wall");
        private GUIContent backWallReflectionRateLabel = new GUIContent("Reflection Rate",
            "Set reflection rate of the back wall");
        private GUIContent rightWallReflectionRateLabel = new GUIContent("Reflection Rate",
            "Set reflection rate of the right wall");
        private GUIContent leftWallReflectionRateLabel = new GUIContent("Reflection Rate",
            "Set reflection rate of the left wall");
        private GUIContent floorReflectionRateLabel = new GUIContent("Reflection Rate",
            "Set reflection rate of the floor");
        private GUIContent reflectionLevelLabel = new GUIContent("Reflection Level (dB)",
            "Set reflection level for reverb effect");
        private GUIContent reverbLevelLable = new GUIContent("Reverb Level (dB)",
            "Set reverb level for reverb effect");
        private GUIContent backgroundTypeLabel = new GUIContent("Background Audio",
            "Set background audio type in the room");
        private GUIContent backgroundVolumeLabel = new GUIContent("Background Volume",
            "Set background audio volume in the room in dB scale");
        private GUIContent backgroundAudioClipLabel = new GUIContent("Background Audio Clip",
            "Set background audio clip");
        private GUIContent sizeLabel = new GUIContent("Room size",
            "Set the room size");

        void OnEnable()
        {
            roomEffect = serializedObject.FindProperty("roomEffect");
            reverbPreset = serializedObject.FindProperty("reverbPreset");
            Ceiling = serializedObject.FindProperty("ceiling");
            FrontWall = serializedObject.FindProperty("frontWall");
            BackWall = serializedObject.FindProperty("backWall");
            RightWall = serializedObject.FindProperty("rightWall");
            LeftWall = serializedObject.FindProperty("leftWall");
            Floor = serializedObject.FindProperty("floor");
            ceilingReflectionRate = serializedObject.FindProperty("ceilingReflectionRate");
            frontWallReflectionRate = serializedObject.FindProperty("frontWallReflectionRate");
            backWallReflectionRate = serializedObject.FindProperty("backWallReflectionRate");
            rightWallReflectionRate = serializedObject.FindProperty("rightWallReflectionRate");
            leftWallReflectionRate = serializedObject.FindProperty("leftWallReflectionRate");
            floorReflectionRate = serializedObject.FindProperty("floorReflectionRate");
            reflectionLevel = serializedObject.FindProperty("reflectionLevel");
            reverbLevel = serializedObject.FindProperty("reverbLevel");
            backgroundType = serializedObject.FindProperty("backgroundType");
            userDefineClip = serializedObject.FindProperty("userDefineClip");
            sourceVolume = serializedObject.FindProperty("sourceVolume");
            size = serializedObject.FindProperty("size");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(roomEffect, roomEffectLabel);

                if (Application.isPlaying)
                    GUI.enabled = false;
                else
                    GUI.enabled = true;

                GUI.enabled = true;
                EditorGUILayout.Separator();

                    EditorGUILayout.PropertyField(reverbPreset, roomPresetLabel);

                    if ((RoomReverbPreset)reverbPreset.enumValueIndex == RoomReverbPreset.UserDefine)
                    {
                        EditorGUILayout.LabelField(SurfaceMaterialsLabel);
                        ++EditorGUI.indentLevel;
                        EditorGUILayout.PropertyField(Ceiling);
                        if (Ceiling.enumValueIndex == (int)RoomPlateMaterial.UserDefine)
                        {
                            ++EditorGUI.indentLevel;
                            EditorGUILayout.Slider(ceilingReflectionRate, 0.0f, 1.0f, ceilingReflectionRateLabel);
                            --EditorGUI.indentLevel;
                        }
                        EditorGUILayout.PropertyField(FrontWall);
                        if (FrontWall.enumValueIndex == (int)RoomPlateMaterial.UserDefine)
                        {
                            ++EditorGUI.indentLevel;
                            EditorGUILayout.Slider(frontWallReflectionRate, 0.0f, 1.0f, frontWallReflectionRateLabel);
                            --EditorGUI.indentLevel;
                        }
                        EditorGUILayout.PropertyField(BackWall);
                        if (BackWall.enumValueIndex == (int)RoomPlateMaterial.UserDefine)
                        {
                            ++EditorGUI.indentLevel;
                            EditorGUILayout.Slider(backWallReflectionRate, 0.0f, 1.0f, backWallReflectionRateLabel);
                            --EditorGUI.indentLevel;
                        }
                        EditorGUILayout.PropertyField(RightWall);
                        if (RightWall.enumValueIndex == (int)RoomPlateMaterial.UserDefine)
                        {
                            ++EditorGUI.indentLevel;
                            EditorGUILayout.Slider(rightWallReflectionRate, 0.0f, 1.0f, rightWallReflectionRateLabel);
                            --EditorGUI.indentLevel;
                        }
                        EditorGUILayout.PropertyField(LeftWall);
                        if (LeftWall.enumValueIndex == (int)RoomPlateMaterial.UserDefine)
                        {
                            ++EditorGUI.indentLevel;
                            EditorGUILayout.Slider(leftWallReflectionRate, 0.0f, 1.0f, leftWallReflectionRateLabel);
                            --EditorGUI.indentLevel;
                        }
                        EditorGUILayout.PropertyField(Floor);
                        if (Floor.enumValueIndex == (int)RoomPlateMaterial.UserDefine)
                        {
                            ++EditorGUI.indentLevel;
                            EditorGUILayout.Slider(floorReflectionRate, 0.0f, 1.0f, floorReflectionRateLabel);
                            --EditorGUI.indentLevel;
                        }
                        --EditorGUI.indentLevel;
                        EditorGUILayout.Separator();
                        EditorGUILayout.Slider(reflectionLevel, -30.0f, 10.0f, reflectionLevelLabel);
                        EditorGUILayout.Slider(reverbLevel, -30.0f, 10.0f, reverbLevelLable);
                        EditorGUILayout.Separator();
                    }

                    EditorGUILayout.PropertyField(backgroundType, backgroundTypeLabel);

                    if ((RoomBackgroundAudioType)backgroundType.enumValueIndex == RoomBackgroundAudioType.UserDefine)
                    {
                        ++EditorGUI.indentLevel;
                        EditorGUILayout.PropertyField(userDefineClip, backgroundAudioClipLabel);
                        --EditorGUI.indentLevel;
                    }

                    EditorGUILayout.Slider(sourceVolume, -96.0f, 0.0f, backgroundVolumeLabel);

            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(size, sizeLabel);

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
                if (!Application.isPlaying)
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            }
        }

        private void DrawSurfaceMaterial(SerializedProperty surfaceMaterial)
        {
            surfaceMaterialLabel.text = surfaceMaterial.displayName;
            EditorGUILayout.PropertyField(surfaceMaterial, surfaceMaterialLabel);
        }
    }
}
