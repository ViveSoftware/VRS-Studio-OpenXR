//====================== Copyright 2017-2024, HTC.Corporation. All rights reserved. ======================
/**
*   release version:    1.3.8.0
*   script version:     1.3.8.0
*/

using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace HTC.UnityPlugin.Vive3DSP
{
    public class Vive3DSPWindow : EditorWindow
    {
        [MenuItem("Window/VIVE 3DSP Audio SDK/Visit VIVE 3DSP Forum")]
        public static void OpenVive3DSPForumWebSite()
        {
            Application.OpenURL(Vive3DSPAudio.VIVE_3DSP_FORUM_URL);
        }
    }
}