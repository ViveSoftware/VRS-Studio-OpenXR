// Copyright HTC Corporation All Rights Reserved.

using System.Text;
using UnityEngine;
using UnityEngine.UI;

using VIVE.OpenXR.Toolkits.BodyTracking.RuntimeDependency;

namespace VIVE.OpenXR.Toolkits.BodyTracking.Demo
{
	public class HumanoidIKMenu : MonoBehaviour
	{
		const string LOG_TAG = "VIVE.OpenXR.Toolkits.BodyTracking.Demo.HumanoidIKMenu";
		private StringBuilder m_sb = null;
		private StringBuilder sb
		{
			get
			{
				if (m_sb == null) { m_sb = new StringBuilder(); }
				return m_sb;
			}
		}
		void DEBUG(StringBuilder msg) { Rdp.d(LOG_TAG, msg, true); }

		public HumanoidIKSample humanoidIK = null;
		public Button beginTrackingButton = null;
		public Button endTrackingButton = null;

		public void BeginTracking()
		{
			if (humanoidIK != null)
			{
				sb.Clear().Append("BeginTracking()"); DEBUG(sb);
				humanoidIK.BeginTracking();
			}
		}
		public void EndTracking()
		{
			if (humanoidIK != null)
			{
				sb.Clear().Append("EndTracking()"); DEBUG(sb);
				humanoidIK.StopTracking();
			}
		}

		private void Update()
		{
			if (humanoidIK != null)
			{
				var status = humanoidIK.GetTrackingStatus();
				if (beginTrackingButton != null)
				{
					beginTrackingButton.interactable = (
						status == HumanoidIKSample.TrackingStatus.NotStart ||
						status == HumanoidIKSample.TrackingStatus.StartFailure
					);
				}
				if (endTrackingButton != null)
				{
					endTrackingButton.interactable = (status == HumanoidIKSample.TrackingStatus.Available);
				}
			}
		}
	}
}
