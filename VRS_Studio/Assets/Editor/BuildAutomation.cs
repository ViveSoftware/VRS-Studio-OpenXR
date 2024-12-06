using UnityEngine;
using System.Linq;
using System;
using System.IO;
using UnityEditor.SceneManagement;
using JetBrains.Annotations;
using UnityEditor.Build.Reporting;

#if UNITY_EDITOR
using UnityEditor;
#endif

static class BuildAutomation
{
    const string kVendorName = "HTC";
    const string kProductName = "VRS Studio";
    const string kGuiBuildExecutableName = "VRS_Studio";
    const string kBuildPath = "./builds/";

    // Android Application Identifier
    public static string GuiBuildAndroidApplicationIdentifier => $"com.{kVendorName}.xr.{kProductName}".ToLower();
    // Android Executable
    public static string GuiBuildAndroidExecutableName => kGuiBuildExecutableName.ToLower() + ".apk";

    public class VRSBuildOptions
    {
        public bool AutoProfile;
        public bool Il2Cpp;
        public BuildTarget Target;
        public string Location;
        public string Stamp;
        public BuildOptions UnityOptions;
        public string Description;
    }

    [Serializable()]
    public class BuildFailedException : System.Exception
    {
        // The << >> markers help the build script parse the message
        public BuildFailedException(string message)
            : base(string.Format("<<{0}>>", message))
        {
        }
    }

    const string kMenuPlatformPref = "VRS Studio/Build/Platform";
    const string kMenuPlatformWindows = "VRS Studio/Build/Platform: Windows";
    const string kMenuPlatformAndroid = "VRS Studio/Build/Platform: Android";
    const string kMenuMono = "VRS Studio/Build/Runtime: Mono";
    const string kMenuIl2cpp = "VRS Studio/Build/Runtime: IL2CPP";
    const string kMenuAutoRun = "VRS Studio/Build/Auto Run";
    const string kMenuDebug = "VRS Studio/Build/Debug";
    const string kMenuTimestamp = "VRS Studio/Build/Timestamp";
    const string kMenuDevelopment = "VRS Studio/Build/Development";
    const string kMenuAutoProfile = "VRS Studio/Build/Auto Profile";

    private static string[] scenes = {
        "Assets/Scenes/Main.unity",
        "Assets/Scenes/VRSS_Environment.unity",
        "Assets/Scenes/Spectator.unity",
        "Assets/Scenes/JelbeeAvatar.unity",
        "Assets/Scenes/ThrowBottles.unity",
        "Assets/Scenes/RobotAssistant.unity",
        "Assets/Scenes/Keyboard.unity",
        "Assets/Scenes/3DObjectManipulation.unity",
        "Assets/Scenes/FaceTracking_Bubble.unity",
        "Assets/Scenes/Tracker.unity",
    };

    private static string m_buildStatus = "-";
    public static string BuildStatus => m_buildStatus; // info about current status

    public static BuildTarget GuiSelectedBuildTarget
    {
        get
        {
            return BuildTarget.Android;
            //return AsEnum(EditorPrefs.GetString(kMenuPlatformPref, "StandaloneWindows64"),
            //    BuildTarget.StandaloneWindows64);
        }
        //set
        //{
        //    EditorPrefs.SetString(kMenuPlatformPref, value.ToString());
        //    Menu.SetChecked(kMenuPlatformWindows, value == BuildTarget.StandaloneWindows64);
        //    Menu.SetChecked(kMenuPlatformAndroid, value == BuildTarget.Android);
        //}
    }

    public static bool GuiAutoRun
    {
        get => EditorPrefs.GetBool(kMenuAutoRun, false);
        set
        {
            EditorPrefs.SetBool(kMenuAutoRun, value);
            Menu.SetChecked(kMenuAutoRun, value);
        }
    }

    public static bool GuiDebug
    {
        get => EditorPrefs.GetBool(kMenuDebug, false);
        set
        {
            EditorPrefs.SetBool(kMenuDebug, value);
            Menu.SetChecked(kMenuDebug, value);
        }
    }

    public static bool GuiTimestamp
    {
        get => EditorPrefs.GetBool(kMenuTimestamp, false);
        set
        {
            EditorPrefs.SetBool(kMenuTimestamp, value);
            Menu.SetChecked(kMenuTimestamp, value);
        }
    }

    // Gui setting for "Development" checkbox
    public static bool GuiDevelopment
    {
        get => EditorPrefs.GetBool(kMenuDevelopment, false);
        set
        {
            EditorPrefs.SetBool(kMenuDevelopment, value);
            Menu.SetChecked(kMenuDevelopment, value);
        }
    }

    // Gui setting for "Auto Profile" checkbox
    public static bool GuiAutoProfile
    {
        get => EditorPrefs.GetBool(kMenuAutoProfile, false);
        set
        {
            EditorPrefs.SetBool(kMenuAutoProfile, value);
            Menu.SetChecked(kMenuAutoProfile, value);
        }
    }

    public static bool GuiRuntimeIl2cpp
    {
        get => EditorPrefs.GetBool(kMenuIl2cpp, false);
        set
        {
            EditorPrefs.SetBool(kMenuIl2cpp, value);
            Menu.SetChecked(kMenuIl2cpp, value);
            Menu.SetChecked(kMenuMono, !value);
        }
    }

    public static bool GuiRuntimeMono
    {
        get { return !GuiRuntimeIl2cpp; }
        set { GuiRuntimeIl2cpp = !value; }
    }

    public static VRSBuildOptions GetGuiOptions()
    {
        return new VRSBuildOptions
        {
            AutoProfile = GuiAutoProfile,
            Il2Cpp = GuiRuntimeIl2cpp,
            Target = GuiSelectedBuildTarget,
            Location = GetAppPathForGuiBuild(),
            Stamp = "(menuitem)",
            UnityOptions = GuiDevelopment
                ? (BuildOptions.AllowDebugging | BuildOptions.Development | BuildOptions.CleanBuildCache)
                : BuildOptions.None,
            //Description = "(unity editor)",
        };
    }

#if UNITY_EDITOR
    // Menu items
    [MenuItem("VRS Studio/Build/Do Build... #&b", false, 2)]
    public static void MenuItem_Build()
    {
        VRSBuildOptions vrsOptions = GetGuiOptions();

        using (var unused = new RestoreCurrentScene())
        {
            DoBuild(vrsOptions);
        }
    }

    public static string GetAppPathForGuiBuild()
    {
        BuildTarget buildTarget = GuiSelectedBuildTarget;

        var directoryName = string.Format(
            "{0}_{1}_{2}{3}",
            GuiDevelopment ? "Debug" : "Release",
            GuiRuntimeIl2cpp ? "_Il2cpp" : "",
            GuiAutoProfile ? "_AutoProfile" : "",
            kGuiBuildExecutableName);
        var location = kBuildPath;//Path.GetDirectoryName(Path.GetDirectoryName(Application.dataPath));

        //location = Path.Combine(Path.Combine(location, "Builds"), directoryName);
        switch (buildTarget)
        {
            case BuildTarget.Android:
                if (GuiTimestamp)
                {
                    location += "/" + kGuiBuildExecutableName.ToLower() + "_" + GetTimeStamp() + ".apk";
                }
                else
                {
                    location += "/" + kGuiBuildExecutableName.ToLower() + ".apk";
                }
                break;
            default:
                throw new BuildFailedException("Unsupported BuildTarget: " + buildTarget.ToString());
        }

        return location;
    }

    public static string GetTimeStamp()
    {
        int bundleVersionCode;

        if (!int.TryParse(DateTime.Now.ToString("yyMMddHHmm"), out bundleVersionCode))
        {
            DateTime LastMinOf2021 = new DateTime(2021, 12, 31, 11, 59, 00);
            TimeSpan TimeDiff = DateTime.Now - LastMinOf2021;
            int TimDiffMins = (int)TimeDiff.TotalMinutes;
            bundleVersionCode = 2112311159 + Mathf.Min(TimDiffMins, 35172487);
        }

        return bundleVersionCode.ToString();
    }

    //=======  Platforms =======

    //[MenuItem(kMenuPlatformWindows, isValidateFunction: false, priority: 200)]
    //static void MenuItem_Platform_Windows()
    //{
    //    GuiSelectedBuildTarget = BuildTarget.StandaloneWindows64;
    //}

    //[MenuItem(kMenuPlatformAndroid, isValidateFunction: false, priority: 210)]
    //static void MenuItem_Platform_Android()
    //{
    //    GuiSelectedBuildTarget = BuildTarget.Android;
    //}

    //=======  Runtimes =======

    [MenuItem(kMenuMono, isValidateFunction: false, priority: 300)]
    static void MenuItem_Runtime_Mono()
    {
        GuiRuntimeMono = !GuiRuntimeMono;
    }

    [MenuItem(kMenuMono, isValidateFunction: true)]
    static bool MenuItem_Runtime_Mono_Validate()
    {
        Menu.SetChecked(kMenuMono, GuiRuntimeMono);
        return true;
    }

    [MenuItem(kMenuIl2cpp, isValidateFunction: false, priority: 305)]
    static void MenuItem_Runtime_Il2cpp()
    {
        GuiRuntimeIl2cpp = !GuiRuntimeIl2cpp;
    }

    [MenuItem(kMenuIl2cpp, isValidateFunction: true)]
    static bool MenuItem_Runtime_Il2cpp_Validate()
    {
        Menu.SetChecked(kMenuIl2cpp, GuiRuntimeIl2cpp);
        return true;
    }

    //=======  Options =======
    [MenuItem(kMenuAutoRun, isValidateFunction: false, priority: 400)]
    static void MenuItem_AutoRun()
    {
        GuiAutoRun = !GuiAutoRun;
    }

    [MenuItem(kMenuAutoRun, isValidateFunction: true)]
    static bool MenuItem_AutoRun_Validate()
    {
        Menu.SetChecked(kMenuAutoRun, GuiAutoRun);
        return true;
    }

    [MenuItem(kMenuDebug, isValidateFunction: false, priority: 405)]
    static void MenuItem_Debug()
    {
        GuiDebug = !GuiDebug;
    }

    [MenuItem(kMenuDebug, isValidateFunction: true)]
    static bool MenuItem_Debug_Validate()
    {
        Menu.SetChecked(kMenuDebug, GuiDebug);
        return true;
    }

    [MenuItem(kMenuTimestamp, isValidateFunction: false, priority: 410)]
    static void MenuItem_Timestamp()
    {
        GuiTimestamp = !GuiTimestamp;
    }

    [MenuItem(kMenuTimestamp, isValidateFunction: true)]
    static bool MenuItem_Timestamp_Validate()
    {
        Menu.SetChecked(kMenuTimestamp, GuiTimestamp);
        return true;
    }

    [MenuItem(kMenuDevelopment, isValidateFunction: false, priority: 415)]
    static void MenuItem_Development()
    {
        GuiDevelopment = !GuiDevelopment;
        if (!GuiDevelopment)
        {
            GuiAutoProfile = false;
        }
    }

    [MenuItem(kMenuDevelopment, isValidateFunction: true)]
    static bool MenuItem_Development_Validate()
    {
        Menu.SetChecked(kMenuDevelopment, GuiDevelopment);
        return true;
    }

    [MenuItem(kMenuAutoProfile, isValidateFunction: false, priority: 420)]
    static void MenuItem_AutoProfile()
    {
        GuiAutoProfile = !GuiAutoProfile;
        if (GuiAutoProfile)
        {
            GuiDevelopment = true;
        }
    }

    [MenuItem(kMenuAutoProfile, isValidateFunction: true)]
    static bool MenuItem_AutoProfile_Validate()
    {
        Menu.SetChecked(kMenuAutoProfile, GuiAutoProfile);
        return true;
    }
#endif

    static T AsEnum<T>(string s, T defaultValue)
    {
        try
        {
            return (T)Enum.Parse(typeof(T), s, true);
        }
        catch (ArgumentException)
        {
            Debug.LogErrorFormat("_btb_ Unknown value for {0}: {1}", typeof(T).FullName, s);
            return defaultValue;
        }
    }

    static T? AsEnum<T>(string s) where T : struct
    {
        try
        {
            return (T)Enum.Parse(typeof(T), s, true);
        }
        catch (ArgumentException)
        {
            Debug.LogErrorFormat("_btb_ Unknown value for {0}: {1}", typeof(T).FullName, s);
            return null;
        }
    }

    static public BuildTargetGroup TargetToGroup(BuildTarget buildTarget)
    {
        switch (buildTarget)
        {
            case BuildTarget.Android:
                return BuildTargetGroup.Android;
            default:
                throw new ArgumentException("buildTarget");
        }
    }

    public class RestoreCurrentScene : System.IDisposable
    {
        SceneSetup[] m_scene;
        public RestoreCurrentScene()
        {
            m_scene = EditorSceneManager.GetSceneManagerSetup();
        }
        public void Dispose()
        {
            if (m_scene != null)
            {
                // force reload
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                EditorSceneManager.RestoreSceneManagerSetup(m_scene);
            }
        }
    }

    // Make changes to PlayerSettings that either can't be or shouldn't be serialized
    class TempSetCommandLineOnlyPlayerSettings : IDisposable
    {
        string m_oldKeystoreName, m_oldKeystorePass;
        string m_oldKeyaliasName, m_oldKeyaliasPass;
        public TempSetCommandLineOnlyPlayerSettings(
            string keystoreName, string keystorePass,
            string keyaliasName, string keyaliasPass)
        {
            m_oldKeystoreName = CheckUnset(PlayerSettings.Android.keystoreName, "keystoreName");
            m_oldKeystorePass = CheckUnset(PlayerSettings.Android.keystorePass, "keystorePass");
            m_oldKeyaliasName = CheckUnset(PlayerSettings.Android.keyaliasName, "keyaliasName");
            m_oldKeyaliasPass = CheckUnset(PlayerSettings.Android.keyaliasPass, "keyaliasPass");

            if (keystoreName != null) { PlayerSettings.Android.keystoreName = keystoreName; }
            if (keystorePass != null) { PlayerSettings.Android.keystorePass = keystorePass; }
            if (keyaliasName != null) { PlayerSettings.Android.keyaliasName = keyaliasName; }
            if (keyaliasPass != null) { PlayerSettings.Android.keyaliasPass = keyaliasPass; }
        }

        public void Dispose()
        {
            PlayerSettings.Android.keystoreName = m_oldKeystoreName;
            PlayerSettings.Android.keystorePass = m_oldKeystorePass;
            PlayerSettings.Android.keyaliasName = m_oldKeyaliasName;
            PlayerSettings.Android.keyaliasPass = m_oldKeyaliasPass;
            AssetDatabase.SaveAssets();
        }

        private static string CheckUnset(string value, string name)
        {
            if (!string.IsNullOrEmpty(value))
            {
                Debug.LogWarningFormat("Expected: {0} is unset", name);
            }
            return value;
        }
    }

    [PublicAPI]
    static void Build()
    {
        BuildTarget? target = null;
        VRSBuildOptions vrsOptions = new VRSBuildOptions()
        {
            Stamp = "",
            UnityOptions = BuildOptions.None,
        };
        string keystoreName = null;
        string keyaliasName = null;
        string keystorePass = Environment.GetEnvironmentVariable("BTB_KEYSTORE_PASS");
        string keyaliasPass = Environment.GetEnvironmentVariable("BTB_KEYALIAS_PASS");

        {
            string[] args = Environment.GetCommandLineArgs();
            int i = 0;
            for (; i < args.Length; ++i)
            {
                if (args[i] == "BuildAutomation.Build")
                {
                    break;
                }
            }
            if (i == args.Length)
            {
                Die(2, "Could not find command line arguments");
            }

            for (i = i + 1; i < args.Length; ++i)
            {
                if (args[i] == "-customBuildPath")
                {
                    vrsOptions.Location = args[++i];
                }
                else if (args[i] == "-androidVersionCode")
                {
                    PlayerSettings.Android.bundleVersionCode = Int32.Parse(args[++i]);
                }
                else if (args[i] == "-androidBundleVersion")
                {
                    string version = args[++i];
                    vrsOptions.Stamp = string.IsNullOrEmpty(version) ? $"_{DateTime.Now:yyMMddHHmm}" : "_" + version + '.' + GetTimeStamp();
                }
                else
                {
                    Die(3, "Unknown argument {0}", args[i]);
                    EditorApplication.Exit(3);
                }
            }
        }

        if (target == null)
        {
            target = BuildTarget.StandaloneWindows64;
        }

        if (target == BuildTarget.Android)
        {
            EditorUserBuildSettings.androidCreateSymbols = AndroidCreateSymbols.Debugging;
        }

        vrsOptions.Target = target.Value;
        using (var unused = new TempSetCommandLineOnlyPlayerSettings(
            keystoreName, keystorePass,
            keyaliasName, keyaliasPass))
        {
            DoBuild(vrsOptions);
        }
    }

    class TempDefineSymbols : System.IDisposable
    {
        string m_prevSymbols;
        BuildTargetGroup m_group;

        // For convenience, the extra symbols can be "" or null
        public TempDefineSymbols(BuildTarget target, params string[] symbols)
        {
            m_group = BuildTargetGroup.Android; //BuildAutomation.TargetToGroup(target);
            m_prevSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(m_group);
            var newSymbols = m_prevSymbols.Split(';') // might be [""]
                .Concat(symbols.Where(elt => elt != null))
                .Select(elt => elt.Trim())
                .Where(elt => elt != "")
                .ToArray();
            var newDefs = string.Join(";", newSymbols);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(m_group, newDefs);
            Debug.Log($"Build defines for {m_group.ToString()}: {newDefs}");
        }

        public void Dispose()
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(m_group, m_prevSymbols);
        }
    }

    class TempSetScriptingBackend : IDisposable
    {
        private ScriptingImplementation m_prevbackend;
        private BuildTargetGroup m_group;

        public TempSetScriptingBackend(BuildTarget target, bool useIl2cpp)
        {
            m_group = BuildTargetGroup.Android; //BuildAutomation.TargetToGroup(target);
            m_prevbackend = PlayerSettings.GetScriptingBackend(m_group);

            // Build script assumes there are only 2 possibilities. It's been true so far,
            // but detect if that assumption ever becomes dangerous and we need to generalize
            var desired = useIl2cpp ? ScriptingImplementation.IL2CPP : ScriptingImplementation.Mono2x;
            if (m_prevbackend != ScriptingImplementation.IL2CPP &&
                m_prevbackend != ScriptingImplementation.Mono2x)
            {
                throw new BuildFailedException(string.Format(
                    "Internal error: trying to switch away from {0}", m_prevbackend));
            }
            PlayerSettings.SetScriptingBackend(m_group, desired);
        }

        public void Dispose()
        {
            PlayerSettings.SetScriptingBackend(m_group, m_prevbackend);
        }
    }

    // Must come after TempHookUpSingletons
    class TempSetBundleVersion : IDisposable
    {
        string m_prevBundleVersion;
        public TempSetBundleVersion(string stamp)
        {
            m_prevBundleVersion = PlayerSettings.bundleVersion;
            if (!string.IsNullOrEmpty(stamp))
            {
                PlayerSettings.bundleVersion = stamp;
            }


        }
        public void Dispose()
        {
            PlayerSettings.bundleVersion = m_prevBundleVersion;
        }
    }

    class TempSetAppNames : IDisposable
    {
        private string m_identifier;
        private string m_name;
        private string m_company;
        private bool m_isAndroid;
        public TempSetAppNames(bool isAndroid, string Description)
        {
            m_isAndroid = isAndroid;
            m_identifier = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);
            PlayerSettings.productName = GuiDebug ? kProductName + " (Beta)" : kProductName; //ensure product name is set correctly
            m_name = PlayerSettings.productName;
            m_company = PlayerSettings.companyName;
            string new_identifier = GuiDebug ? GuiBuildAndroidApplicationIdentifier + "dev" : GuiBuildAndroidApplicationIdentifier;

            if (!String.IsNullOrEmpty(Description))
            {
                new_identifier += Description.Replace("_", "").Replace("#", "").Replace("-", "");
            }
            if (m_isAndroid)
            {
                PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, new_identifier);
            }

            PlayerSettings.companyName = kVendorName;
        }

        public void Dispose()
        {
            if (m_isAndroid)
            {
                PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, m_identifier);
            }
            PlayerSettings.productName = m_name;
            PlayerSettings.companyName = m_company;
        }
    }

    class TempSetGraphicsApis : IDisposable
    {
        UnityEngine.Rendering.GraphicsDeviceType[] m_graphicsApis;

        BuildTarget m_Target;

        public TempSetGraphicsApis(VRSBuildOptions vrsOptions)
        {
            m_Target = vrsOptions.Target;
            m_graphicsApis = PlayerSettings.GetGraphicsAPIs(vrsOptions.Target);
            UnityEngine.Rendering.GraphicsDeviceType[] targetGraphicsApisRequired;

            targetGraphicsApisRequired = new UnityEngine.Rendering.GraphicsDeviceType[] { UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3 };

            PlayerSettings.SetGraphicsAPIs(m_Target, targetGraphicsApisRequired);
        }

        public void Dispose()
        {
            PlayerSettings.SetGraphicsAPIs(m_Target, m_graphicsApis);
        }
    }

    // Load a scene so it can be temporarily modified in-place.
    // Upon Dispose(), scene is restored to its old state.
    // Note: does not try and restore current scene.
    class TempModifyScene : System.IDisposable
    {
        string m_scene, m_backup;
        public TempModifyScene(string sceneName)
        {
            m_scene = sceneName;
            if (!string.IsNullOrEmpty(m_scene))
            {
                m_backup = Path.Combine(Path.GetDirectoryName(sceneName),
                    "Temp_" + Path.GetFileName(sceneName));
                FileUtil.DeleteFileOrDirectory(m_backup);
                FileUtil.CopyFileOrDirectory(m_scene, m_backup);
                // force reload
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                EditorSceneManager.OpenScene(m_scene);
            }
            else
            {
                m_backup = null;
            }
        }
        public void Dispose()
        {
            if (m_backup != null)
            {
                FileUtil.DeleteFileOrDirectory(m_scene);
                FileUtil.MoveFileOrDirectory(m_backup, m_scene);
            }
        }
    }

    class TempSetStereoRenderPath : IDisposable
    {
        private StereoRenderingPath m_Path;

        public TempSetStereoRenderPath(StereoRenderingPath path)
        {
            m_Path = PlayerSettings.stereoRenderingPath;
            PlayerSettings.stereoRenderingPath = path;
        }

        public void Dispose()
        {
            PlayerSettings.stereoRenderingPath = m_Path;
            AssetDatabase.SaveAssets();
        }
    }

    public static void DoBuild(VRSBuildOptions vrsOptions)
    {
        BuildTarget target = vrsOptions.Target;
        string location = vrsOptions.Location;
        string stamp = vrsOptions.Stamp;
        BuildOptions options = vrsOptions.UnityOptions;
        if (GuiAutoRun) options |= BuildOptions.AutoRunPlayer;

        if (GuiRuntimeIl2cpp) PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        else PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.Mono2x);

        PlayerSettings.Android.targetArchitectures = GuiRuntimeIl2cpp ? AndroidArchitecture.ARM64 : AndroidArchitecture.ARMv7;

        m_buildStatus = "Started build";

        Note("BuildVRSStudio: Start target:{0} profile:{1} options:{2}",
            target, vrsOptions.AutoProfile,
            // For some reason, "None" comes through as "CompressTextures"
            options == BuildOptions.None ? "None" : options.ToString());

        using (var unused11 = new TempSetStereoRenderPath(target == BuildTarget.Android
            ? StereoRenderingPath.SinglePass : StereoRenderingPath.MultiPass))
        using (var unused3 = new TempDefineSymbols(
            target,
            vrsOptions.Il2Cpp ? "DISABLE_AUDIO_CAPTURE" : null,
            vrsOptions.AutoProfile ? "AUTOPROFILE_ENABLED" : null))
        //using (var unused5 = new TempSetScriptingBackend(target, vrsOptions.Il2Cpp))
        using (var unused14 = new TempSetGraphicsApis(vrsOptions))
        using (var unused6 = new TempSetBundleVersion(stamp))
            // Save our changes and notify the editor that there have been changes.
            //m_buildStatus = "Saving scene";
            //EditorSceneManager.SaveOpenScenes();

            // If we're building android, we need to copy Support files into streaming assets
            m_buildStatus = "Copying platform support files";
        using (var unused10 = new TempSetAppNames(target == BuildTarget.Android, vrsOptions.Description))
        {
            string buildDirectory = Path.GetDirectoryName(location);
            Directory.CreateDirectory(buildDirectory);

            // Some information on what we are building
            var buildDesc = $"Building player: {target}";
            if (target == BuildTarget.Android)
            {
                buildDesc += $", {PlayerSettings.Android.targetArchitectures}";
            }
            m_buildStatus = buildDesc;

            // Start building
            var thing = BuildPipeline.BuildPlayer(scenes, location, target, options);
            string error = FormatBuildReport(thing);
            if (!string.IsNullOrEmpty(error))
            {
                string message = $"BuildPipeline.BuildPlayer() returned: \"{error}\"";
                Note(message);
                m_buildStatus = $"Build player failed: {error}";
                throw new BuildFailedException(message);
            }
            else
            {
                Note("BuildVRSStudio: End");
                EditorPrefs.SetString("LastVRSStudioBuildLocation", location);
            }
        }

        // At the end of a GUI build, the in-memory value of VR::enabledDevices
        // is correct, but the on-disk value is not. "File -> Save Project"
        // flushes the change to disk, but I can't find a way to do that
        // programmatically. Either AssetDatabase.SaveAssets() doesn't also
        // save ProjectSettings.asset; or doing it here isn't late enough.
        // AssetDatabase.SaveAssets();

        m_buildStatus = "Finished";
    }

    // Returns null if no errors; otherwise a string with what went wrong.
    private static string FormatBuildReport(BuildReport report)
    {
        if (report.summary.result == BuildResult.Succeeded)
        {
            return null;
        }

        return "Errors:\n";
    }

    static void Note(string msg, params System.Object[] args)
    {
        // TODO: Is there a way to get this to stdout somehow?
        if (args != null && args.Length > 0)
        {
            msg = string.Format(msg, args);
        }
        Debug.LogFormat("_btb_ {0}", msg);
    }

    static void Die(int exitCode, string msg = null, params System.Object[] args)
    {
        // TODO: Is there a way to get this to stdout somehow?
        if (msg != null)
        {
            Debug.LogErrorFormat("_btb_ Abort <<{0}>>", string.Format(msg, args));
        }
        EditorApplication.Exit(exitCode);
    }
}
