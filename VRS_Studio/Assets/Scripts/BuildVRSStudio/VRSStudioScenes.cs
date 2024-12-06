using System;
using System.Collections.Generic;
using UnityEngine;

namespace VRSStudio.Build
{
    [Serializable]
    public struct SceneData
    {
        public SceneData(string inName, string inPath)
        {
            name = inName;
            path = inPath;
        }

        public string name;
        public string path;
    }

    [Serializable]
    public class VRSStudioScenes : ScriptableObject
    {
        private static VRSStudioScenes instance = null;
        public static VRSStudioScenes Instance
        {
            get
            {
#if UNITY_EDITOR
                if (instance == null)
                {
                    instance = new VRSStudioScenes();
                    instance.sceneDataList = GetSceneDataList();
                    instance.pathList = GetScenePathList();
                }
#endif
                return instance;
            }
        }

        public List<string> pathList = new List<string>();
        public List<SceneData> sceneDataList = new List<SceneData>();

        public static List<SceneData> GetSceneDataList()
        {
            return new List<SceneData>()
            {
                new SceneData("Main", "Assets/Scenes/Main.unity"),
                new SceneData("Environment", "Assets/Scenes/VRSS_Environment.unity"),
                new SceneData("Spectator", "Assets/Scenes/Spectator.unity"),
                new SceneData("JelbeeAvatar", "Assets/Scenes/JelbeeAvatar.unity"),
                new SceneData("Bottle", "Assets/Scenes/ThrowBottles.unity"),
                new SceneData("RobotAssistant", "Assets/Scenes/RobotAssistant.unity"),
                new SceneData("Keyboard", "Assets/Scenes/Keyboard.unity"),
                new SceneData("3DObjectManipulation", "Assets/Scenes/3DObjectManipulation.unity"),
                new SceneData("FaceTracking", "Assets/Scenes/FaceTracking_Bubble.unity"),
                new SceneData("Tracker", "Assets/Scenes/Tracker.unity"),
            };
        }

        private static List<string> GetScenePathList()
        {
            List<string> scenePaths = new List<string>();

            foreach (SceneData sceneData in instance.sceneDataList)
            {
                scenePaths.Add(sceneData.path);
            }

            return scenePaths;
        }

        private void Awake()
        {
            Debug.Log("VRSStudioScenes Awake");
            instance = this;
        }
    }
}
