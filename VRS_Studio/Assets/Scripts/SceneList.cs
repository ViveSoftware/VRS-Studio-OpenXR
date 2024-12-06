using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "SceneList.asset", menuName = "SceneList")]
public class SceneList : ScriptableObject
{
    public const string AssetPath = "Assets/Resources/SceneList.asset";
    public const string AssetPathShort = "SceneList";
    public VRSStudioScenes Scenes;

#if UNITY_EDITOR
    [MenuItem("Tools/Scene List")]
    static void LoadDeveloperSettings()
    {
        Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(AssetPath);
    }

    public void UpdateSceneNames()
    {
        Scenes.PMainSceneName = Scenes.MainSceneName;
        Scenes.PEnvironmentSceneName = Scenes.EnvironmentSceneName;
        Scenes.PSpectatorSceneName = Scenes.SpectatorSceneName;
        Scenes.PAvatarSceneName = Scenes.AvatarSceneName;
        Scenes.PBottleSceneName = Scenes.BottleSceneName;
        Scenes.PRobotAssistantSceneName = Scenes.RobotAssistantSceneName;
        Scenes.PKeyboardSceneName = Scenes.KeyboardSceneName;
        Scenes.PObject3DSceneName = Scenes.Object3DSceneName;
        Scenes.PFacialTrackingSceneName = Scenes.FacialTrackingSceneName;
        Scenes.PTrackerSceneName = Scenes.TrackerSceneName;
    }
#endif
}

[System.Serializable]
public class VRSStudioScenes
{
    public Object MainScene;
    public string MainSceneName { get { return (MainScene != null) ? MainScene.name : PMainSceneName; } }

    public Object EnvironmentScene;
    public string EnvironmentSceneName { get { return (EnvironmentScene != null) ? EnvironmentScene.name : PEnvironmentSceneName; } }
    public Object SpectatorScene;
    public string SpectatorSceneName { get { return (SpectatorScene != null) ? SpectatorScene.name : PSpectatorSceneName; } }
    public Object AvatarScene;
    public string AvatarSceneName { get { return (AvatarScene != null) ? AvatarScene.name : PAvatarSceneName; } }
    public Object BottleScene;
    public string BottleSceneName { get { return (BottleScene != null) ? BottleScene.name : PBottleSceneName; } }
    public Object RobotAssistantScene;
    public string RobotAssistantSceneName { get { return (RobotAssistantScene != null) ? RobotAssistantScene.name : PRobotAssistantSceneName; } }
    public Object KeyboardScene;
    public string KeyboardSceneName { get { return (KeyboardScene != null) ? KeyboardScene.name : PKeyboardSceneName; } }
    public Object Object3DScene;
    public string Object3DSceneName { get { return (Object3DScene != null) ? Object3DScene.name : PObject3DSceneName; } }
    public Object FacialTrackingScene;
    public string FacialTrackingSceneName { get { return (FacialTrackingScene != null) ? FacialTrackingScene.name : PFacialTrackingSceneName; } }
    public Object TrackerScene;
    public string TrackerSceneName { get { return (TrackerScene != null) ? TrackerScene.name : PTrackerSceneName; } }

    [HideInInspector] public string PMainSceneName = string.Empty;
    [HideInInspector] public string PEnvironmentSceneName = string.Empty;
    [HideInInspector] public string PSpectatorSceneName = string.Empty;
    [HideInInspector] public string PAvatarSceneName = string.Empty;
    [HideInInspector] public string PBottleSceneName = string.Empty;
    [HideInInspector] public string PRobotAssistantSceneName = string.Empty;
    [HideInInspector] public string PKeyboardSceneName = string.Empty;
    [HideInInspector] public string PObject3DSceneName = string.Empty;
    [HideInInspector] public string PFacialTrackingSceneName = string.Empty;
    [HideInInspector] public string PTrackerSceneName = string.Empty;
    [HideInInspector] public string PDemo3DSPSceneName = string.Empty;
}
