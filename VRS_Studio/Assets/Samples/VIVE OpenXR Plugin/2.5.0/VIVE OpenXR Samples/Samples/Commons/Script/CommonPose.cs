// Copyright HTC Corporation All Rights Reserved.

using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

namespace VIVE.OpenXR.Samples
{
    public class CommonPose : MonoBehaviour
    {
		#region LOG
		const string LOG_TAG = "VIVE.OpenXR.Samples.CommonPose";
        StringBuilder m_sb = null;
        StringBuilder sb {
            get {
                if (m_sb == null) { m_sb = new StringBuilder(); }
                return m_sb;
			}
		}
        void DEBUG(StringBuilder msg) { Debug.LogFormat("{0} {1} {2}", LOG_TAG, PoseName, msg); }
        void INTERVAL(StringBuilder msg) { if (printIntervalLog) { DEBUG(msg); } }
        int printFrame = 0;
        bool printIntervalLog = false;
        void ERROR(StringBuilder msg)
        {
            if (!Application.isEditor)
                Debug.LogErrorFormat("{0} {1} {2}", LOG_TAG, PoseName, msg);
        }
        #endregion

        #region Inspector
        public string PoseName = "";
        public GameObject VisibleElements = null;

        [SerializeField]
        private InputActionReference m_DevicePose = null;
        public InputActionReference DevicePose { get { return m_DevicePose; } set { m_DevicePose = value; } }
		#endregion

		private void Update()
		{
			if (m_DevicePose == null) { return; }

            printFrame++;
            printFrame %= 300;
            printIntervalLog = (printFrame == 0);

            if (CommonHelper.GetPoseIsTracked(m_DevicePose, out bool isTracked, out string msg))
			{
                if (isTracked)
                {
                    if (VisibleElements != null && !VisibleElements.activeSelf) { VisibleElements.SetActive(true); }

                    if (CommonHelper.GetPoseTrackingState(m_DevicePose, out InputTrackingState trackingState, out msg))
                    {
                        if (trackingState.HasFlag(InputTrackingState.Rotation))
						{
                            if (CommonHelper.GetPoseRotation(m_DevicePose, out Quaternion rotation, out msg))
							{
                                transform.localRotation = rotation;
							}
							else
							{
                                if (printIntervalLog)
                                {
                                    sb.Clear().Append("Update() rotation: ").Append(msg);
                                    ERROR(sb);
                                }
                            }
                        }

                        if (trackingState.HasFlag(InputTrackingState.Position))
                        {
                            if (CommonHelper.GetPosePosition(m_DevicePose, out Vector3 position, out msg))
                            {
                                transform.localPosition = position;
                            }
                            else
                            {
                                if (printIntervalLog)
                                {
                                    sb.Clear().Append("Update() position: ").Append(msg);
                                    ERROR(sb);
                                }
                            }
                        }

                        if (printIntervalLog)
                        {
                            sb.Clear().Append("Update() trackingState: ").Append(trackingState);
                            ERROR(sb);
                        }
                    }
                    else
					{
                        if (printIntervalLog)
                        {
                            sb.Clear().Append("Update() trackingState: ").Append(msg);
                            ERROR(sb);
                        }
                    }
                }
				else
				{
                    if (VisibleElements != null && VisibleElements.activeSelf) { VisibleElements.SetActive(false); }

                    if (printIntervalLog)
                    {
                        sb.Clear().Append("Update() isTracked: ").Append(isTracked);
                        ERROR(sb);
                    }
                }
            }
			else
			{
                if (VisibleElements != null && VisibleElements.activeSelf) { VisibleElements.SetActive(false); }

                if (printIntervalLog)
                {
                    sb.Clear().Append("Update() isTracked: ").Append(msg);
                    ERROR(sb);
                }
			}
		}
	}
}