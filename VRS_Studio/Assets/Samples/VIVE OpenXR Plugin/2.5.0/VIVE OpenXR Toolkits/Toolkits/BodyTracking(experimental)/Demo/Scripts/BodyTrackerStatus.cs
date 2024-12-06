// Copyright HTC Corporation All Rights Reserved.

using UnityEngine;
using UnityEngine.UI;
using VIVE.OpenXR.Toolkits.BodyTracking.RuntimeDependency;

namespace VIVE.OpenXR.Toolkits.BodyTracking.Demo
{
	[RequireComponent(typeof(Text))]
	public class BodyTrackerStatus : MonoBehaviour
	{
		public TrackerLocation m_Role = TrackerLocation.Waist;

		private Text m_Text = null;
		private void Awake()
		{
			m_Text = GetComponent<Text>();
		}
		private void Update()
		{
			for (int i = 0; i < Rdp.Tracker.s_TrackerIds.Length; i++)
			{
				Rdp.Tracker.Id id = Rdp.Tracker.s_TrackerIds[i];
				var role = Rdp.Tracker.GetTrackerRole(id);
				if (role == m_Role)
				{
					m_Text.text = m_Role + ": " + id.Name();
					break;
				}
				else
				{
					m_Text.text = m_Role + ": Disconnected";
				}
			}
		}
	}
}
