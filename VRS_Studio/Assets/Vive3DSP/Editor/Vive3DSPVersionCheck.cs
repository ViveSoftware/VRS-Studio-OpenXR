//========= Copyright 2017-2024, HTC Corporation. All rights reserved. ===========
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using System.Reflection;

#if UNITY_5_4_OR_NEWER
using UnityEditor.Rendering;
using UnityEngine.Networking;
#endif

namespace HTC.UnityPlugin.Vive3DSP
{
    [InitializeOnLoad]
    public class Vive3DSPVersionCheck : EditorWindow
    {
        [Serializable]
        private struct RepoInfo
        {
            public string tag_name;
            public string body;
        }

        public interface IPropSetting
        {
            bool SkipCheck();
            void UpdateCurrentValue();
            bool IsIgnored();
            bool IsUsingRecommendedValue();
            bool DoDrawRecommend(); // return true if setting accepted
            void AcceptRecommendValue();
            void DoIgnore();
            void DeleteIgnore();
        }

        public class RecommendedSetting<T> : IPropSetting
        {
            private const string fmtTitle = "{0} (current = {1})";
            private const string fmtRecommendBtn = "Use recommended ({0})";
            private const string fmtRecommendBtnWithPosefix = "Use recommended ({0}) - {1}";

            private string m_settingTitle;
            private string m_settingTrimedTitle;
            private string ignoreKey { get { return m_settingTrimedTitle; } }

            public string settingTitle { get { return m_settingTitle; } set { m_settingTitle = value; m_settingTrimedTitle = value.Replace(" ", ""); } }
            public string recommendBtnPostfix = string.Empty;
            public string toolTip = string.Empty;
            public Func<bool> skipCheckFunc = null;
            public Func<T> recommendedValueFunc = null;
            public Func<T> currentValueFunc = null;
            public Action<T> setValueFunc = null;
            public T currentValue = default(T);
            public T recommendedValue = default(T);

            public T GetRecommended() { return recommendedValueFunc == null ? recommendedValue : recommendedValueFunc(); }

            public bool SkipCheck() { return skipCheckFunc == null ? false : skipCheckFunc(); }

            public bool IsIgnored() { return Vive3DSPProjectSettings.HasIgnoreKey(ignoreKey); }

            public bool IsUsingRecommendedValue() { return EqualityComparer<T>.Default.Equals(currentValue, GetRecommended()); }

            public void UpdateCurrentValue() { currentValue = currentValueFunc(); }

            public bool DoDrawRecommend()
            {
                GUILayout.Label(new GUIContent(string.Format(fmtTitle, settingTitle, currentValue), toolTip));

                GUILayout.BeginHorizontal();

                bool recommendBtnClicked;
                if (string.IsNullOrEmpty(recommendBtnPostfix))
                {
                    recommendBtnClicked = GUILayout.Button(new GUIContent(string.Format(fmtRecommendBtn, GetRecommended()), toolTip));
                }
                else
                {
                    recommendBtnClicked = GUILayout.Button(new GUIContent(string.Format(fmtRecommendBtnWithPosefix, GetRecommended(), recommendBtnPostfix), toolTip));
                }

                if (recommendBtnClicked)
                {
                    AcceptRecommendValue();
                }

                GUILayout.FlexibleSpace();

                if (GUILayout.Button(new GUIContent("Ignore", toolTip)))
                {
                    DoIgnore();
                }

                GUILayout.EndHorizontal();

                return recommendBtnClicked;
            }

            public void AcceptRecommendValue()
            {
                setValueFunc(GetRecommended());
            }

            public void DoIgnore()
            {
                Vive3DSPProjectSettings.AddIgnoreKey(ignoreKey);
            }

            public void DeleteIgnore()
            {
                Vive3DSPProjectSettings.RemoveIgnoreKey(ignoreKey);
            }
        }

        public abstract class RecommendedSettingCollection : List<IPropSetting> { }

        public const string lastestVersionUrl = "https://api.github.com/repos/Professor3DSound/Vive3DSP/releases/latest";
                public const string pluginUrl = "https://github.com/Professor3DSound/Vive3DSP/releases";
        public const double versionCheckIntervalMinutes = 30.0;

        private const string nextVersionCheckTimeKey = "Vive3DSPAudio.LastVersionCheckTime";
        private const string fmtIgnoreUpdateKey = "DoNotShowUpdate.v{0}";
        private static string ignoreThisVersionKey;

        private static bool completeCheckVersionFlow = false;
        private static UnityWebRequest webReq;
        private static RepoInfo latestRepoInfo;
        private static System.Version latestVersion;
        private static Vector2 releaseNoteScrollPosition;
        private static Vector2 settingScrollPosition;
        private static bool showNewVersion;
        private static bool toggleSkipThisVersion = false;
        private static Vive3DSPVersionCheck windowInstance;
        private static List<IPropSetting> s_settings;
        private static bool editorUpdateRegistered;
        private Texture2D dspLogo;

        /// <summary>
        /// Count of settings that are ignored
        /// </summary>
        public static int ignoredSettingsCount { get; private set; }
        /// <summary>
        /// Count of settings that are not using recommended value
        /// </summary>
        public static int shouldNotifiedSettingsCount { get; private set; }
        /// <summary>
        /// Count of settings that are not ignored and not using recommended value
        /// </summary>
        public static int notifiedSettingsCount { get; private set; }

        public static bool recommendedWindowOpened { get { return windowInstance != null; } }

        static Vive3DSPVersionCheck()
        {
            editorUpdateRegistered = true;
            EditorApplication.update += CheckVersionAndSettings;

#if UNITY_2017_2_OR_NEWER
            EditorApplication.playModeStateChanged += (mode) =>
            {
                if (mode == PlayModeStateChange.EnteredEditMode && !editorUpdateRegistered)
                {
#else
            EditorApplication.playmodeStateChanged += () =>
            {
                if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode && !editorUpdateRegistered)
                {
#endif
                    editorUpdateRegistered = true;
                    EditorApplication.update += CheckVersionAndSettings;
                }
            };
        }

        public static void AddRecommendedSetting<T>(RecommendedSetting<T> setting)
        {
            InitializeSettins();
            s_settings.Add(setting);
        }

        private static void InitializeSettins()
        {
            if (s_settings != null) { return; }

            s_settings = new List<IPropSetting>();

            foreach (var type in Assembly.GetAssembly(typeof(RecommendedSettingCollection)).GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(RecommendedSettingCollection))))
            {
                s_settings.AddRange((RecommendedSettingCollection)Activator.CreateInstance(type));
            }
        }

        private static void VersionCheckLog(string msg)
        {
#if VIVE_3DSP_PRINT_FETCH_VERSION_LOG
            using (var outputFile = new StreamWriter("Vive3DSPVersionCheck.log", true))
            {
                outputFile.WriteLine(DateTime.Now.ToString() + " - " + msg + ". Stop fetching until " + UtcDateTimeFromStr(EditorPrefs.GetString(nextVersionCheckTimeKey)).ToLocalTime().ToString());
            }
#endif
        }

        // check vive 3dsp audio version on github
        private static void CheckVersionAndSettings()
        {
            if (Application.isPlaying)
            {
                EditorApplication.update -= CheckVersionAndSettings;
                editorUpdateRegistered = false;
                return;
            }

            InitializeSettins();

            EditorPrefs.SetString(nextVersionCheckTimeKey, UtcDateTimeToStr(DateTime.UtcNow));

            // fetch new version info from github release site
            if (!completeCheckVersionFlow && Vive3DSPSettings.autoCheckNewVive3DSPVersion)
            {
                if (webReq == null) // web request not running
                {
                    if (EditorPrefs.HasKey(nextVersionCheckTimeKey) && DateTime.UtcNow < UtcDateTimeFromStr(EditorPrefs.GetString(nextVersionCheckTimeKey)))
                    {
                        VersionCheckLog("Skipped");
                        completeCheckVersionFlow = true;
                        return;
                    }

                    webReq = GetUnityWebRequestAndSend(lastestVersionUrl);
                }

                if (!webReq.isDone)
                {
                    return;
                }

                EditorPrefs.SetString(nextVersionCheckTimeKey, UtcDateTimeToStr(DateTime.UtcNow.AddMinutes(versionCheckIntervalMinutes)));

                if (UrlSuccess(webReq))
                {
                    var json = GetWebText(webReq);
                    if (!string.IsNullOrEmpty(json))
                    {
                        latestRepoInfo = JsonUtility.FromJson<RepoInfo>(json);
                        VersionCheckLog("Fetched");
                    }
                    else{
                        Debug.Log("empty");
                    }
                }

                // parse latestVersion and ignoreThisVersionKey
                if (!string.IsNullOrEmpty(latestRepoInfo.tag_name))
                {
                    try
                    {
                        latestVersion = new System.Version(Regex.Replace(latestRepoInfo.tag_name, "[^0-9\\.]", string.Empty));
                        ignoreThisVersionKey = string.Format(fmtIgnoreUpdateKey, latestVersion.ToString());
                    }
                    catch
                    {
                        latestVersion = default(System.Version);
                        ignoreThisVersionKey = string.Empty;
                    }
                }

                webReq.Dispose();
                webReq = null;

                completeCheckVersionFlow = true;
            }

            showNewVersion = !string.IsNullOrEmpty(ignoreThisVersionKey) && !Vive3DSPProjectSettings.HasIgnoreKey(ignoreThisVersionKey) && latestVersion > Vive3DSPAudio.Vive3DSPVersion.Current;

            UpdateIgnoredNotifiedSettingsCount(false);

            if (showNewVersion || notifiedSettingsCount > 0)
            {
                TryOpenRecommendedSettingWindow();
            }

            EditorApplication.update -= CheckVersionAndSettings;
            editorUpdateRegistered = false;
        }

        public static bool UpdateIgnoredNotifiedSettingsCount(bool drawNotifiedPrompt)
        {
            InitializeSettins();

            ignoredSettingsCount = 0;
            shouldNotifiedSettingsCount = 0;
            notifiedSettingsCount = 0;
            var hasSettingsAccepted = false;

            foreach (var setting in s_settings)
            {
                if (setting.SkipCheck()) { continue; }

                setting.UpdateCurrentValue();

                var isIgnored = setting.IsIgnored();
                if (isIgnored) { ++ignoredSettingsCount; }

                if (setting.IsUsingRecommendedValue()) { continue; }
                else { ++shouldNotifiedSettingsCount; }

                if (!isIgnored)
                {
                    ++notifiedSettingsCount;

                    if (drawNotifiedPrompt)
                    {
                        if (notifiedSettingsCount == 1)
                        {
                            EditorGUILayout.HelpBox("Recommended project settings:", MessageType.Warning);

                            settingScrollPosition = GUILayout.BeginScrollView(settingScrollPosition, GUILayout.ExpandHeight(true));
                        }

                        hasSettingsAccepted |= setting.DoDrawRecommend();
                    }

                }
            }

            return hasSettingsAccepted;
        }

        // Open recommended setting window (with possible new version prompt)
        // won't do any thing if the window is already opened
        public static void TryOpenRecommendedSettingWindow()
        {
            if (recommendedWindowOpened) { return; }

            windowInstance = GetWindow<Vive3DSPVersionCheck>(true, "Vive 3DSP Audio");
            windowInstance.minSize = new Vector2(240f, 750f);
            var rect = windowInstance.position;
            windowInstance.position = new Rect(Mathf.Max(rect.x, 50f), Mathf.Max(rect.y, 50f), 350f, 400f);
        }

        private static DateTime UtcDateTimeFromStr(string str)
        {
            var utcTicks = default(long);
            if (string.IsNullOrEmpty(str) || !long.TryParse(str, out utcTicks)) { return DateTime.MinValue; }
            return new DateTime(utcTicks, DateTimeKind.Utc);
        }

        private static string UtcDateTimeToStr(DateTime utcDateTime)
        {
            return utcDateTime.Ticks.ToString();
        }

        private static UnityWebRequest GetUnityWebRequestAndSend(string url)
        {
            var webReq = new UnityWebRequest(url);

            if (webReq.downloadHandler == null)
            {
                webReq.downloadHandler = new DownloadHandlerBuffer();
            }

#if UNITY_2017_2_OR_NEWER
            webReq.SendWebRequest();
#elif UNITY_5_4_OR_NEWER
            webReq.Send();
#endif
            return webReq;
        }

        private static string GetWebText(UnityWebRequest wr)
        {
#if UNITY_5_4_OR_NEWER
            return wr != null && wr.downloadHandler != null ? wr.downloadHandler.text : string.Empty;
#else
            return wr != null ? wr.text : string.Empty;
#endif
        }

        private static bool TryGetWebHeaderValue(UnityWebRequest wr, string headerKey, out string headerValue)
        {
#if UNITY_5_4_OR_NEWER
            headerValue = wr.GetResponseHeader(headerKey);
            return string.IsNullOrEmpty(headerValue);
#else
            if (wr.responseHeaders == null) { headerValue = string.Empty; return false; }
            return wr.responseHeaders.TryGetValue(headerKey, out headerValue);
#endif
        }

        private static bool UrlSuccess(UnityWebRequest wr)
        {
            try
            {
                if (wr == null) { return false; }

                if (!string.IsNullOrEmpty(wr.error))
                {
                    // API rate limit exceeded, see https://developer.github.com/v3/#rate-limiting
                    Debug.Log("url:" + wr.url);
                    Debug.Log("error:" + wr.error);
                    Debug.Log(GetWebText(wr));

                    string responseHeader;
                    if (TryGetWebHeaderValue(wr, "X-RateLimit-Limit", out responseHeader))
                    {
                        Debug.Log("X-RateLimit-Limit:" + responseHeader);
                    }
                    if (TryGetWebHeaderValue(wr, "X-RateLimit-Remaining", out responseHeader))
                    {
                        Debug.Log("X-RateLimit-Remaining:" + responseHeader);
                    }
                    if (TryGetWebHeaderValue(wr, "X-RateLimit-Reset", out responseHeader))
                    {
                        Debug.Log("X-RateLimit-Reset:" + TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(double.Parse(responseHeader))).ToString());
                    }
                    VersionCheckLog("Failed. Rate limit exceeded");
                    return false;
                }

                if (Regex.IsMatch(GetWebText(wr), "404 not found", RegexOptions.IgnoreCase))
                {
                    Debug.Log("url:" + wr.url);
                    Debug.Log("error:" + wr.error);
                    Debug.Log(GetWebText(wr));
                    VersionCheckLog("Failed. 404 not found");
                    return false;
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
                VersionCheckLog("Failed. " + e.ToString());
                return false;
            }

            return true;
        }

        private string GetResourcePath()
        {
            var ms = MonoScript.FromScriptableObject(this);
            var path = AssetDatabase.GetAssetPath(ms);
            path = Path.GetDirectoryName(path);
            return path.Substring(0, path.Length - "Scripts/Editor".Length) + "Textures/";
        }

        public void OnGUI()
        {
#if UNITY_2017_1_OR_NEWER
            if (EditorApplication.isCompiling)
            {
                EditorGUILayout.LabelField("Compiling...");
                return;
            }
#endif
            if (dspLogo == null)
            {
                var currentDir = Path.GetDirectoryName(AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this)));
                var texturePath = currentDir.Substring(0, currentDir.Length - "Editor".Length) + "Textures/3DSP_logo.png";
                dspLogo = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
            }

            if (dspLogo != null)
            {
                GUI.DrawTexture(GUILayoutUtility.GetRect(0, 200), dspLogo, ScaleMode.StretchToFill);
            }

            if (showNewVersion)
            {
                EditorGUILayout.HelpBox("New version available:", MessageType.Warning);

                GUILayout.Label("Current version: " + Vive3DSPAudio.Vive3DSPVersion.Current);
                GUILayout.Label("New version: " + latestVersion);

                if (!string.IsNullOrEmpty(latestRepoInfo.body))
                {
                    GUILayout.Label("Release notes:");
                    releaseNoteScrollPosition = GUILayout.BeginScrollView(releaseNoteScrollPosition, GUILayout.Height(250f));
                    EditorGUILayout.HelpBox(latestRepoInfo.body, MessageType.None);
                    GUILayout.EndScrollView();
                }

                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button(new GUIContent("Get Latest Version", "Goto " + pluginUrl)))
                    {
                        Application.OpenURL(pluginUrl);
                    }

                    GUILayout.FlexibleSpace();

                    toggleSkipThisVersion = GUILayout.Toggle(toggleSkipThisVersion, "Do not prompt for this version again.");
                }
                GUILayout.EndHorizontal();
            }

            var hasSettingsAccepted = UpdateIgnoredNotifiedSettingsCount(true);

            if (notifiedSettingsCount > 0)
            {
                GUILayout.EndScrollView();

                if (ignoredSettingsCount > 0)
                {
                    if (GUILayout.Button("Clear All Ignores(" + ignoredSettingsCount + ")"))
                    {
                        foreach (var setting in s_settings) { setting.DeleteIgnore(); }
                    }
                }

                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Accept All(" + notifiedSettingsCount + ")"))
                    {
                        for (int i = 10; i >= 0 && notifiedSettingsCount > 0; --i)
                        {
                            foreach (var setting in s_settings) { if (!setting.SkipCheck() && !setting.IsIgnored() && !setting.IsUsingRecommendedValue()) { setting.AcceptRecommendValue(); } }

                            UpdateIgnoredNotifiedSettingsCount(false);
                        }

                        hasSettingsAccepted = true;
                    }

                    if (GUILayout.Button("Ignore All(" + notifiedSettingsCount + ")"))
                    {
                        foreach (var setting in s_settings) { if (!setting.SkipCheck() && !setting.IsIgnored() && !setting.IsUsingRecommendedValue()) { setting.DoIgnore(); } }
                    }
                }
                GUILayout.EndHorizontal();
            }
            else if (shouldNotifiedSettingsCount > 0)
            {
                EditorGUILayout.HelpBox("Some recommended settings ignored.", MessageType.Warning);

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Clear All Ignores(" + ignoredSettingsCount + ")"))
                {
                    foreach (var setting in s_settings) { setting.DeleteIgnore(); }
                }
            }
            else
            {
                EditorGUILayout.HelpBox("All recommended settings applied.", MessageType.Info);

                GUILayout.FlexibleSpace();
            }

            if (Vive3DSPProjectSettings.hasChanged)
            {
                // save ignore keys
                Vive3DSPProjectSettings.Save();
            }

            if (GUILayout.Button("Close"))
            {
                Close();
            }
        }

        private void OnDestroy()
        {
            if (dspLogo != null)
            {
                dspLogo = null;
            }

            if (showNewVersion && toggleSkipThisVersion && !string.IsNullOrEmpty(ignoreThisVersionKey))
            {
                showNewVersion = false;
                Vive3DSPProjectSettings.AddIgnoreKey(ignoreThisVersionKey);
                Vive3DSPProjectSettings.Save();
            }

            if (windowInstance == this)
            {
                windowInstance = null;
            }
        }
    }
}