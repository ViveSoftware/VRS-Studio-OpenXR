//====================== Copyright 2017-2024, HTC.Corporation. All rights reserved. ======================
using UnityEngine;

namespace HTC.UnityPlugin.Vive3DSP
{
    public partial class Vive3DSPSettings : ScriptableObject
    {
        public const string DEFAULT_RESOURCE_PATH = "Vive3DSPSettings";
        public const bool AUTO_CHECK_NEW_VIVE_3DSP_VERSION_DEFAULT_VALUE = true;

        [SerializeField]
        private bool m_autoCheckNewVive3DSPVersion = AUTO_CHECK_NEW_VIVE_3DSP_VERSION_DEFAULT_VALUE;

        public static bool autoCheckNewVive3DSPVersion { get { return Instance == null ? AUTO_CHECK_NEW_VIVE_3DSP_VERSION_DEFAULT_VALUE : s_instance.m_autoCheckNewVive3DSPVersion; } set { if (Instance != null) { Instance.m_autoCheckNewVive3DSPVersion = value; } } }

        private static Vive3DSPSettings s_instance = null;

        public static Vive3DSPSettings Instance
        {
            get
            {
                if (s_instance == null)
                {
                    LoadFromResource();
                }

                return s_instance;
            }
        }

        public static void LoadFromResource(string path = null)
        {
            if (path == null)
            {
                path = DEFAULT_RESOURCE_PATH;
            }

            if ((s_instance = Resources.Load<Vive3DSPSettings>(path)) == null)
            {
                s_instance = CreateInstance<Vive3DSPSettings>();
            }
        }

        private void OnDestroy()
        {
            if (s_instance == this)
            {
                s_instance = null;
            }
        }
    }
}