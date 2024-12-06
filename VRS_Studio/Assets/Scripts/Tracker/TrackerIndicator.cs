using HTC.UnityPlugin.ColliderEvent;
using TMPro;
using UnityEngine;
using UnityEngine.XR;
using VIVE.OpenXR;
using VRSStudio.Common;
using VRSStudio.Common.Input;
using VRSStudio.Spectator;

namespace VRSStudio.Tracker
{
    public class TrackerIndicator : MonoBehaviour, IColliderEventHoverEnterHandler, IColliderEventHoverExitHandler
    {
        VRSStudio.Common.Input.Tracker tracker = new Common.Input.Tracker(Common.Input.Tracker.TrackerID.Tracker0, InputUsage.Position | InputUsage.Rotation);
        public GameObject indicator;
        public TextMeshPro text;
        [SerializeField]
        private Material commonMat;
        [SerializeField]
        private Material invalidMat;
        [SerializeField]
        private Material usingMat;
        private MeshRenderer meshRenderer = null;

        /// <summary>
        /// if tracker is in front of Camera within the showFOV angle, show the indicator.
        /// </summary>
        [Range(30, 179)]
        public float showFOV = 40;

        /// <summary>
        /// if tracker is not in front of Camera within the hideFOV angle, hide the indicator.  Could be wider than showFOV.  Let it hard to hide.
        /// </summary>
        [Range(30, 179)]
        public float hideFOV = 70;

        /// <summary>
        /// A timer for show tracker.  If look at the tracker, how long will the tracker show.  
        /// </summary>
        [Range(0.01f, 3)]
        public float timeShow = 0.5f;

        /// <summary>
        /// A timer for hide tracker.  Could be longer than timeShow.  Let it hard to hide.
        /// </summary>
        [Range(0.01f, 3)]
        public float timeHide = 1;

        Timer timerShow;
        Timer timerHide;

        public bool IsShow { get; private set; }

        private void Awake()
        {
            timerShow = new Timer(timeShow);
            timerHide = new Timer(timeHide);
            if (indicator)
            {
                meshRenderer = indicator.GetComponent<MeshRenderer>();
            }
        }

        private Transform GetRig()
        {
            if (VIVERig.Instance != null) return VIVERig.Instance.transform;
            else if (Camera.main != null) return Camera.main.transform.parent;
            else return null;
        }

        public void SetDevice(int trackerId, InputDevice dev, string name)
        {
            tracker.trackerId = (Common.Input.Tracker.TrackerID)trackerId;
            tracker.dev = dev;
            if (text)
                text.text = name;
        }

        public int GetTrackerId()
        {
            return (int)tracker.trackerId;
        }

        public InputDevice GetInputDevice()
        {
            return tracker.dev;
        }

        // Update is called once per frame
        void Update()
        {
            //Log.d("TrackerIndicator", "n=" + name + " timerGone=" + timerHide + ", timerShow=" + timerShow);

            if (timerHide.Check())
            {
                indicator.SetActive(false);
                IsShow = false;
            }
            if (timerShow.Check())
            {
                indicator.SetActive(true);

                if (IsStandalone())
                {
                    int ctid = VRSSpectatorManager.Instance.GetCurrentTracker();
                    int tid = (int)tracker.trackerId;
                    if (tid == ctid)
                    {
                        SetIndicatorMat(usingMat);
                    }
                    else
                    {
                        SetIndicatorMat(commonMat);
                        //if ((int)ctid == -1)
                        //{
                        //    VRSSpectatorManager.Instance.SetTracker((uint)tid);
                        //    SetIndicatorMat(usingMat);
                        //}
                        //else
                        //{
                        //    SetIndicatorMat(commonMat);
                        //}
                    }
                }
                IsShow = true;
            }

            var rig = GetRig();
            if (!rig) return;
            if (!tracker.dev.isValid)
            {
                ShowIndicator(false, true);
                return;
            }
            InputDeviceTools.UpdateXRTrackingDevice(tracker, InputUsage.Position | InputUsage.Rotation);

            // If already show, the hideFOV will be wider.
            ShowIndicator(IsInFov(IsShow ? hideFOV : showFOV));
        }

        public void Show()
        {
            ShowIndicator(true, true);
        }

        public void Hide()
        {
            ShowIndicator(false, true);
        }

        bool IsInFov(float fov)
        {
            var rig = GetRig();
            if (!rig) return true;

            // Use rig, make tracker pose to world space
            var pos = rig.TransformPoint(tracker.position);
            transform.position = pos;
            transform.rotation = rig.rotation * tracker.rotation;

            var cam = Camera.main;
            if (cam == null) return true;
            var dir = (pos - cam.transform.position).normalized;
            var threshold = fov / 2 * Mathf.Deg2Rad;
            var angle = Mathf.Acos(Vector3.Dot(cam.transform.forward, dir));
            //Log.d("TrackerIndicator", "n=" + name + "angle = " + angle * Mathf.Rad2Deg);
            return angle < threshold;
        }


        public void ShowIndicator(bool show, bool force = false)
        {
            if (show && (!indicator.activeInHierarchy))
            {
                if (!timerShow.IsSet)
                    timerShow.Set();
                if (force)
                    timerShow.Timeout();

                // Cancel timer hide
                timerHide.Reset();
            }
            if (!show && (indicator.activeInHierarchy || force))
            {
                if (!timerHide.IsSet)
                    timerHide.Set();
                if (force)
                    timerHide.Timeout();

                // Cancel timer show
                timerShow.Reset();
            }
        }

        public void OnColliderEventHoverEnter(ColliderHoverEventData eventData)
        {
            if (IsStandalone())
            {
                VRSSpectatorManager.Instance.SetTracker((uint)tracker.trackerId);
            }
            else
            {
                SetIndicatorMat(invalidMat);
            }
        }

        public void OnColliderEventHoverExit(ColliderHoverEventData eventData)
        {
            SetIndicatorMat(commonMat);
        }

        private void SetIndicatorMat(Material mat)
        {
            if (meshRenderer)
            {
                meshRenderer.material = mat;
            }
        }

        private bool IsStandalone()
        {
            return text != null && text.text.Contains("Tracker") && VRSSpectatorManager.Instance;
        }
    }
}