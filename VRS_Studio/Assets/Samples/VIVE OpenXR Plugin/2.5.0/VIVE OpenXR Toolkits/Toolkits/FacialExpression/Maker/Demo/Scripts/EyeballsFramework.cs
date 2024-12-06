using System;
using UnityEngine;
using UnityEngine.XR.OpenXR;
using VIVE.OpenXR.FacialTracking;

namespace VIVE.OpenXR.Toolkits.FacialExpression.Maker.Demo
{
    public class EyeballsFramework : MonoBehaviour
    {
        public enum FrameworkStatus { STOP, START, WORKING, ERROR, NOT_SUPPORT }
        /// <summary>
        /// The status of the anipal engine.
        /// </summary>
        public static FrameworkStatus Status { get; protected set; }

        /// <summary>
        /// Currently supported lip motion prediction engine's version.
        /// </summary>
        public enum SupportedEyeVersion { version1, version2 }

        /// <summary>
        /// Whether to enable anipal's Eye module.
        /// </summary>
        public bool EnableEye = true;

        /// <summary>
        /// Which version of eye prediction engine will be used, default is version 1.
        /// </summary>
        public SupportedEyeVersion EnableEyeVersion = SupportedEyeVersion.version2;
        private static EyeballsFramework Mgr = null;
        public static EyeballsFramework Instance
        {
            get
            {
                if (Mgr == null)
                {
                    Mgr = FindObjectOfType<EyeballsFramework>();
                }
                if (Mgr == null)
                {
                    Debug.LogError("EyeballsFramework not found");
                }
                return Mgr;
            }
        }

        void Start()
        {
            StartFramework();
        }

        void OnDestroy()
        {
            StopFramework();
        }

        ViveFacialTracking feature;
        public void StartFramework()
        {
            if (!EnableEye) return;
            if (Status == FrameworkStatus.WORKING || Status == FrameworkStatus.NOT_SUPPORT) return;
            if (EnableEyeVersion == SupportedEyeVersion.version1)
            {
                Debug.LogError("[EyeballsFramework] Initial Version 1 Eye not supported now : ");
            }
            else
            {
                /*var*/ feature = OpenXRSettings.Instance.GetFeature<ViveFacialTracking>();
                if (feature && feature.CreateFacialTracker(XrFacialTrackingTypeHTC.XR_FACIAL_TRACKING_TYPE_EYE_DEFAULT_HTC))
                {
                    Debug.Log("[EyeballsFramework] Initial Eye v2 success!");
                    Status = FrameworkStatus.WORKING;
                }
                else
                {
                    Debug.LogError("[EyeballsFramework] Initial Eye v2 failed!");
                    Status = FrameworkStatus.ERROR;
                }

            }
            Debug.Log("[EyeballsFramework] Initial Eye v2 success! " + Status);
        }

        public void StopFramework()
        {
            if (Status != FrameworkStatus.NOT_SUPPORT)
            {
                if (Status != FrameworkStatus.STOP)
                {
                    if (EnableEyeVersion == SupportedEyeVersion.version1)
                    {
                        Debug.LogError("[EyeballsFramework] Initial Version 1 Eye not supported now : ");
                    }
                    else
                    {
                        var feature = OpenXRSettings.Instance.GetFeature<ViveFacialTracking>();
                        if (feature && feature.DestroyFacialTracker(XrFacialTrackingTypeHTC.XR_FACIAL_TRACKING_TYPE_EYE_DEFAULT_HTC))
                        {
                            Debug.Log("[EyeballsFramework] Release Version 2 Eye success!");

                        }
                        else
                        {
                            Debug.LogError("[EyeballsFramework] Release Version 2 Eye failed!");
                        }
                    }
                }
                else
                {
                    Debug.Log("[EyeballsFramework] Stop Framework : module not on");
                }
            }
            Status = FrameworkStatus.STOP;
        }
    }
}
