// Copyright HTC Corporation All Rights Reserved.

using System.Text;
using UnityEngine;
using UnityEngine.UI;

using VIVE.OpenXR.Toolkits.BodyTracking.RuntimeDependency;

namespace VIVE.OpenXR.Toolkits.BodyTracking.Demo
{
	public class BodyIKMenu : MonoBehaviour
	{
		const string LOG_TAG = "VIVE.OpenXR.Toolkits.BodyTracking.Demo.BodyIKMenu";
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

		public BodyIKSample bodytrackingIK = null;
		public Button beginTrackingButton = null;
		public Button endTrackingButton = null;

		public void BeginTracking()
		{
			if (bodytrackingIK != null)
			{
				sb.Clear().Append("BeginTracking()"); DEBUG(sb);
				bodytrackingIK.BeginTracking();
			}
		}
		public void EndTracking()
		{
			if (bodytrackingIK != null)
			{
				sb.Clear().Append("EndTracking()"); DEBUG(sb);
				bodytrackingIK.StopTracking();
			}
		}

		private void Update()
		{
			if (bodytrackingIK != null)
			{
				var status = bodytrackingIK.GetTrackingStatus();
				if (beginTrackingButton != null)
				{
					beginTrackingButton.interactable = (
						status == BodyIKSample.TrackingStatus.NotStart ||
						status == BodyIKSample.TrackingStatus.StartFailure
					);
				}
				if (endTrackingButton != null)
				{
					endTrackingButton.interactable = (status == BodyIKSample.TrackingStatus.Available);
				}
			}
		}
	}
}
