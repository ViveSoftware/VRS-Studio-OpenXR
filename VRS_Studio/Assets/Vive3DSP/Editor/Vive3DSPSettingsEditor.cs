//========= Copyright 2017-2024, HTC Corporation. All rights reserved. ===========

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace HTC.UnityPlugin.Vive3DSP
{
    public static class Vive3DSPSettingsEditor
    {
        public interface ISupportedSDK
        {
            string name { get; }
            bool enabled { get; set; }
        }

        public class Foldouter
        {
            private static bool s_initialized;
            private static GUIStyle s_styleFoleded;
            private static GUIStyle s_styleExpended;

            public bool isExpended { get; private set; }

            public static void Initialize()
            {
                if (s_initialized) { return; }
                s_initialized = true;

                s_styleFoleded = new GUIStyle(EditorStyles.foldout);
                s_styleExpended = new GUIStyle(EditorStyles.foldout);
                s_styleExpended.normal = s_styleFoleded.onNormal;
                s_styleExpended.active = s_styleFoleded.onActive;
            }

            public static void ShowFoldoutBlank()
            {
                GUILayout.Space(20f);
            }

            public void ShowFoldoutButton()
            {
                var style = isExpended ? s_styleExpended : s_styleFoleded;
                if (GUILayout.Button(string.Empty, style, GUILayout.Width(12f)))
                {
                    isExpended = !isExpended;
                }
            }

            public bool ShowFoldoutButtonOnToggleEnabled(GUIContent content, bool toggleValue)
            {
                GUILayout.BeginHorizontal();
                if (toggleValue)
                {
                    ShowFoldoutButton();
                }
                else
                {
                    ShowFoldoutBlank();
                }
                var toggleResult = EditorGUILayout.ToggleLeft(content, toggleValue, s_labelStyle);
                if (toggleResult != toggleValue) { s_guiChanged = true; }
                GUILayout.EndHorizontal();
                return toggleResult;
            }

            public bool ShowFoldoutButtonWithEnabledToggle(GUIContent content, bool toggleValue)
            {
                GUILayout.BeginHorizontal();
                ShowFoldoutButton();
                var toggleResult = EditorGUILayout.ToggleLeft(content, toggleValue, s_labelStyle);
                if (toggleResult != toggleValue) { s_guiChanged = true; }
                GUILayout.EndHorizontal();
                return toggleResult;
            }

            public void ShowFoldoutButtonWithDisbledToggle(GUIContent content)
            {
                GUILayout.BeginHorizontal();
                ShowFoldoutButton();
                GUI.enabled = false;
                EditorGUILayout.ToggleLeft(content, false, s_labelStyle);
                GUI.enabled = true;
                GUILayout.EndHorizontal();
            }

            public static void ShowFoldoutBlankWithDisbledToggle(GUIContent content)
            {
                GUILayout.BeginHorizontal();
                ShowFoldoutBlank();
                GUI.enabled = false;
                EditorGUILayout.ToggleLeft(content, false, s_labelStyle);
                GUI.enabled = true;
                GUILayout.EndHorizontal();
            }

            public static bool ShowFoldoutBlankWithEnabledToggle(GUIContent content, bool toggleValue)
            {
                GUILayout.BeginHorizontal();
                ShowFoldoutBlank();
                var toggleResult = EditorGUILayout.ToggleLeft(content, toggleValue, s_labelStyle);
                if (toggleResult != toggleValue) { s_guiChanged = true; }
                GUILayout.EndHorizontal();
                return toggleResult;
            }
        }

        public const string URL_VIVE_3DSP_GITHUB_RELEASE_PAGE = "https://github.com/Professor3DSound/Vive3DSP/releases";

        private const string DEFAULT_ASSET_PATH = "Assets/Vive3DSPSettings/Resources/Vive3DSPSettings.asset";

        private static Vector2 s_scrollValue = Vector2.zero;
        private static float s_warningHeight;
        private static GUIStyle s_labelStyle;
        private static bool s_guiChanged;
        private static string s_defaultAssetPath;
        
        private static Foldouter s_autoBindFoldouter = new Foldouter();
        private static Foldouter s_bindingUIFoldouter = new Foldouter();

        public static string defaultAssetPath
        {
            get
            {
                if (s_defaultAssetPath == null)
                {
                    s_defaultAssetPath = DEFAULT_ASSET_PATH;
                }

                return s_defaultAssetPath;
            }
        }

#pragma warning disable 0618
        [PreferenceItem("VIVE 3DSP Settings")]
#pragma warning restore 0618
        private static void OnVIUPreferenceGUI()
        {
#if UNITY_2017_1_OR_NEWER
            if (EditorApplication.isCompiling)
            {
                EditorGUILayout.LabelField("Compiling...");
                return;
            }
#endif

            if (s_labelStyle == null)
            {
                s_labelStyle = new GUIStyle(EditorStyles.label);
                s_labelStyle.richText = true;
            }

            Foldouter.Initialize();

            s_guiChanged = false;

            s_scrollValue = EditorGUILayout.BeginScrollView(s_scrollValue);

            EditorGUILayout.LabelField("<b>VIVE 3DSP Audio v" + Vive3DSPAudio.Vive3DSPVersion.Current + "</b>", s_labelStyle);
            EditorGUI.BeginChangeCheck();
            Vive3DSPSettings.autoCheckNewVive3DSPVersion = EditorGUILayout.ToggleLeft("Auto Check Latest Version", Vive3DSPSettings.autoCheckNewVive3DSPVersion);
            s_guiChanged |= EditorGUI.EndChangeCheck();

            GUILayout.BeginHorizontal();
            ShowUrlLinkButton(URL_VIVE_3DSP_GITHUB_RELEASE_PAGE, "Get Latest Release");
            ShowCheckRecommendedSettingsButton();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            var assetPath = AssetDatabase.GetAssetPath(Vive3DSPSettings.Instance);
            
            if (s_guiChanged)
            {
                if (string.IsNullOrEmpty(assetPath))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(defaultAssetPath));
                    AssetDatabase.CreateAsset(Vive3DSPSettings.Instance, defaultAssetPath);
                }

                EditorUtility.SetDirty(Vive3DSPSettings.Instance);

                Vive3DSPVersionCheck.UpdateIgnoredNotifiedSettingsCount(false);
            }

            if (!string.IsNullOrEmpty(assetPath))
            {
                GUILayout.Space(10);

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Use Default Settings"))
                {
                    AssetDatabase.DeleteAsset(assetPath);
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            EditorGUILayout.EndScrollView();
        }

        private static bool ShowToggle(GUIContent label, bool value, params GUILayoutOption[] options)
        {
            var result = EditorGUILayout.ToggleLeft(label, value, s_labelStyle, options);
            if (result != value) { s_guiChanged = true; }
            return result;
        }

        private static void ShowCheckRecommendedSettingsButton()
        {
            if (Vive3DSPVersionCheck.notifiedSettingsCount <= 0) { return; }

            if (GUILayout.Button("View Recommended Settings", GUILayout.ExpandWidth(false)))
            {
                Vive3DSPVersionCheck.TryOpenRecommendedSettingWindow();
            }
        }

        private static void ShowUrlLinkButton(string url, string label = "Get Plugin")
        {
            if (GUILayout.Button(new GUIContent(label, url), GUILayout.ExpandWidth(false)))
            {
                Application.OpenURL(url);
            }
        }
    }
}