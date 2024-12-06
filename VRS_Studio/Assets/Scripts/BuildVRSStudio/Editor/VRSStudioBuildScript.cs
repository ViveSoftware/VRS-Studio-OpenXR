using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;


namespace VRSStudio.Build.Editor
{
    public class VRSStudioBuildScript
    {
        private const string VRSStudio_Play = "VRS Studio/Play";
        private const string VRSStudio_PlayMode = "VRS Studio/Play Mode";
        private const string VRSStudio_PlayMode_Default = "VRS Studio/Play Mode/Default";
        
        private const string VRSStudio_Build = "VRS Studio/Build(Old)";
        private const string VRSStudio_Build6432 = "VRS Studio/Build6432";
        private const string VRSStudio_Build32 = "VRS Studio/Build Config/32-bit";
        private const string VRSStudio_Build64 = "VRS Studio/Build Config/64-bit";
        private const string VRSStudio_AutoRunAfterBuild = "VRS Studio/Build Config/Auto Run";
        private const string VRSStudio_DevBuild = "VRS Studio/Build Config/Development build";
        private const string VRSStudio_NeedSymbol = "VRS Studio/Build Config/Need symbol zip";
        private const string VRSStudio_DevName = "VRS Studio/Build Config/Use Dev Name";

        private static string originalCompanyName, originalProductName, originalPackageName, originalVersion;
        private static AndroidArchitecture originalAndroidArchitecture;
        private static ScriptingImplementation originalScriptingBackEnd;
        private static ManagedStrippingLevel originalManagedStrippingLevel;
        private static bool originalStripEngineCode;
        private static int originalBundleVersionCode;
        private static AndroidCreateSymbols originalAndroidCcreateSymbols = AndroidCreateSymbols.Disabled;

        private static string buildCompanyName = "HTC Corp.";
        private static string buildProductName = "VRS Studio";
        private static string buildPackageName = "com.htc.vrs.vrsstudio";
        private static string buildPackageNameDev = "com.htc.vrssdev";
        private static string buildVersion = "0.0.3a";
        private static AndroidArchitecture buildAndroidArchitecture = AndroidArchitecture.None;
        private static ScriptingImplementation buildScriptingBackEnd = ScriptingImplementation.IL2CPP;
        private static ManagedStrippingLevel buildManagedStrippingLevel = ManagedStrippingLevel.Disabled;

        private static bool buildStripEngineCode = false;
        private static int buildBundleVersionCode = 1;

        private static string apkName = "VRSStudio.apk";
        private static string apkBuildDestination = null;

        private static bool autoRun = false;
        private static bool devBuild = false;
        private static bool needSymbol = false;
        private static bool devName = false;
        private static bool build6432 = false;  // 64 = false
        private static int playMode = 0;

        private static void PlayInEditor(List<string> pathList)
        {
            if (pathList == null || pathList.Count <= 0)
                return;
            if (!EditorApplication.isPlaying)
            {
                List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>();
                foreach (var path in pathList)
                {
                    scenes.Add(new EditorBuildSettingsScene(path, true));
                }
                EditorBuildSettings.scenes = scenes.ToArray();

                EditorPrefs.SetString("VRSRunInEditor_OriginalScenePath", EditorSceneManager.GetActiveScene().path);

                EditorSceneManager.OpenScene(pathList[0]);
                EditorApplication.isPlaying = true;
                EditorApplication.playModeStateChanged += RestoreOriginalScene;
            }
        }

        private static void RestoreOriginalScene(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredEditMode)
            {
                var original = EditorPrefs.GetString("VRSRunInEditor_OriginalScenePath", "");
                if (!string.IsNullOrEmpty(original))
                    EditorSceneManager.OpenScene(original);
                EditorApplication.playModeStateChanged -= RestoreOriginalScene;
            }
        }

        [MenuItem(VRSStudio_Play, false, 101)]
        private static void PlayInEditor()
        {
            playMode = EditorPrefs.GetInt(VRSStudio_PlayMode, 0);
            switch (playMode)
            {
                case 0:
                    PlayInEditor(VRSStudioScenes.Instance.pathList);
                    break;
            }
        }

        private static void UpdatePlayModeMenu()
        {
            Menu.SetChecked(VRSStudio_PlayMode_Default, playMode == 0);
        }

        [MenuItem(VRSStudio_PlayMode_Default, true)]
        private static bool ValidatePlayModeDefault()
        {
            playMode = EditorPrefs.GetInt(VRSStudio_PlayMode, 0);
            UpdatePlayModeMenu();
            return true;
        }

        [MenuItem(VRSStudio_PlayMode_Default, false, 301)]
        private static void TogglePlayModeDefault()
        {
            playMode = 0;
            EditorPrefs.SetInt(VRSStudio_PlayMode, 0);
            UpdatePlayModeMenu();
        }

        [MenuItem(VRSStudio_AutoRunAfterBuild, true)]
        private static bool ValidateAutoRunAfterBuild()
        {
            autoRun = EditorPrefs.GetBool(VRSStudio_AutoRunAfterBuild, false);
            Menu.SetChecked(VRSStudio_AutoRunAfterBuild, autoRun);
            return true;
        }

        [MenuItem(VRSStudio_AutoRunAfterBuild, false, 230)]
        private static void ToggleAutoRunAfterBuild()
        {
            autoRun = !autoRun;
            EditorPrefs.SetBool(VRSStudio_AutoRunAfterBuild, autoRun);
            Menu.SetChecked(VRSStudio_AutoRunAfterBuild, autoRun);
        }

        [MenuItem(VRSStudio_DevBuild, true)]
        private static bool ValidateDevBuild()
        {
            devBuild = EditorPrefs.GetBool(VRSStudio_DevBuild, true);
            Menu.SetChecked(VRSStudio_DevBuild, devBuild);
            return true;
        }

        [MenuItem(VRSStudio_DevBuild, false, 231)]
        private static void ToggleDevBuild()
        {
            devBuild = !devBuild;
            EditorPrefs.SetBool(VRSStudio_DevBuild, devBuild);
            Menu.SetChecked(VRSStudio_DevBuild, devBuild);
        }

        [MenuItem(VRSStudio_NeedSymbol, true)]
        private static bool ValidateNeedSymbol()
        {
            needSymbol = EditorPrefs.GetBool(VRSStudio_NeedSymbol, false);
            Menu.SetChecked(VRSStudio_NeedSymbol, needSymbol);
            return true;
        }

        [MenuItem(VRSStudio_NeedSymbol, false, 231)]
        private static void ToggleNeedSymbol()
        {
            needSymbol = !needSymbol;
            EditorPrefs.SetBool(VRSStudio_NeedSymbol, needSymbol);
            Menu.SetChecked(VRSStudio_NeedSymbol, needSymbol);
        }

        [MenuItem(VRSStudio_DevName, true)]
        private static bool ValidateDevName()
        {
            devName = EditorPrefs.GetBool(VRSStudio_DevName, true);
            Menu.SetChecked(VRSStudio_DevName, devName);
            return true;
        }

        [MenuItem(VRSStudio_DevName, false, 232)]
        private static void ToggleDevName()
        {
            devName = !devName;
            EditorPrefs.SetBool(VRSStudio_DevName, devName);
            Menu.SetChecked(VRSStudio_DevName, devName);
        }

        [MenuItem(VRSStudio_Build, false, 100)]
        private static void Build()
        {
            if (string.IsNullOrEmpty(apkBuildDestination))
            {
                apkBuildDestination = Path.GetDirectoryName(Application.dataPath);
            }

            string[] scenePathArray = VRSStudioScenes.Instance.pathList.ToArray();

            var path = Path.Combine(new string[] { apkBuildDestination, "VRSStudio_Builds", build6432 ? "armv7" : "arm64" });
            BuildApk(path, scenePathArray);
        }

        private static void UpdateMenuBuild6432()
        {
            Menu.SetChecked(VRSStudio_Build32, build6432);
            Menu.SetChecked(VRSStudio_Build64, !build6432);
        }

        [MenuItem(VRSStudio_Build32, true)]
        private static bool ValidateBuild32()
        {
            build6432 = EditorPrefs.GetBool(VRSStudio_Build6432, false);
            UpdateMenuBuild6432();
            return true;
        }

        [MenuItem(VRSStudio_Build32, false, 201)]
        private static void ToggleBuild32()
        {
            build6432 = true;
            EditorPrefs.SetBool(VRSStudio_Build6432, build6432);
            UpdateMenuBuild6432();
        }

        [MenuItem(VRSStudio_Build64, true)]
        private static bool ValidateBuild64()
        {
            build6432 = EditorPrefs.GetBool(VRSStudio_Build6432, false);
            UpdateMenuBuild6432();
            return true;
        }

        [MenuItem(VRSStudio_Build64, false, 201)]
        private static void ToggleBuild64()
        {
            build6432 = false;
            EditorPrefs.SetBool(VRSStudio_Build6432, build6432);
            UpdateMenuBuild6432();
        }

        private static void BuildApk(string buildDestinationPath, string[] scenes)
        {
            devName = EditorPrefs.GetBool(VRSStudio_DevName, true);
            autoRun = EditorPrefs.GetBool(VRSStudio_AutoRunAfterBuild, false);
            devBuild = EditorPrefs.GetBool(VRSStudio_DevBuild, true);
            needSymbol = EditorPrefs.GetBool(VRSStudio_NeedSymbol, false);
            build6432 = EditorPrefs.GetBool(VRSStudio_Build6432, false);

            BackupAndChangePlayerSettings();

            BuildOptions extraFlags = BuildOptions.None;
            BuildOptions buildOptions = (autoRun ? BuildOptions.AutoRunPlayer : BuildOptions.None) | extraFlags;
            if (devBuild)
                buildOptions |= BuildOptions.Development;


            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions()
            {
                options = buildOptions,
                target = BuildTarget.Android,
                scenes = scenes,
                targetGroup = BuildTargetGroup.Android,
                locationPathName = Path.Combine(buildDestinationPath, apkName)
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("Time=").Append(DateTime.Now).AppendLine();
            sb.Append("AppId=").Append(PlayerSettings.applicationIdentifier).AppendLine();
            sb.Append("Arch=").Append(PlayerSettings.Android.targetArchitectures).AppendLine();
            sb.Append("Development Build=").Append(devBuild).AppendLine();
            sb.Append("Create Debug Symbol=").Append(EditorUserBuildSettings.androidCreateSymbols).AppendLine();
            sb.Append("Build and Run=").Append(autoRun).AppendLine();
            sb.Append("Path=").Append(buildPlayerOptions.locationPathName).AppendLine();
            var msg = sb.ToString();
            Debug.Log(msg);
            //if (EditorUtility.DisplayDialog("Build VRS Studio", msg, "Build", "Cancel"))
            {
                BuildPipeline.BuildPlayer(buildPlayerOptions);
            }

            //RestorePlayerSettings();
        }

        //Backup Player Settings of original project and change the to VRS Studio APK settings
        private static void BackupAndChangePlayerSettings()
        {
            originalCompanyName = PlayerSettings.companyName;
            originalProductName = PlayerSettings.productName;
            originalPackageName = PlayerSettings.applicationIdentifier;
            originalVersion = PlayerSettings.bundleVersion;
            originalBundleVersionCode = PlayerSettings.Android.bundleVersionCode;
            originalScriptingBackEnd = PlayerSettings.GetScriptingBackend(BuildTargetGroup.Android);
            originalAndroidArchitecture = PlayerSettings.Android.targetArchitectures;
            originalAndroidCcreateSymbols = EditorUserBuildSettings.androidCreateSymbols;

            PlayerSettings.companyName = buildCompanyName;
            PlayerSettings.productName = buildProductName;
            PlayerSettings.applicationIdentifier = devName ? buildPackageNameDev : buildPackageName;
            PlayerSettings.bundleVersion = buildVersion;
            PlayerSettings.Android.bundleVersionCode = buildBundleVersionCode;
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, buildScriptingBackEnd);
            PlayerSettings.Android.targetArchitectures = build6432 ? AndroidArchitecture.ARMv7 : AndroidArchitecture.ARM64;
            EditorUserBuildSettings.androidCreateSymbols = needSymbol ? AndroidCreateSymbols.Debugging : AndroidCreateSymbols.Disabled;

            originalManagedStrippingLevel = PlayerSettings.GetManagedStrippingLevel(BuildTargetGroup.Android);
            PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.Android, buildManagedStrippingLevel);

            originalStripEngineCode = PlayerSettings.stripEngineCode;
            PlayerSettings.stripEngineCode = buildStripEngineCode;
        }

        //Restore Player Settings of original project
        private static void RestorePlayerSettings()
        {
            PlayerSettings.companyName = originalCompanyName;
            PlayerSettings.productName = originalProductName;
            PlayerSettings.applicationIdentifier = originalPackageName;
            PlayerSettings.bundleVersion = originalVersion;
            PlayerSettings.Android.bundleVersionCode = originalBundleVersionCode;
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, originalScriptingBackEnd);
            PlayerSettings.Android.targetArchitectures = originalAndroidArchitecture;
            EditorUserBuildSettings.androidCreateSymbols = originalAndroidCcreateSymbols;

            PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.Android, originalManagedStrippingLevel);

            PlayerSettings.stripEngineCode = originalStripEngineCode;
        }
    }
}
