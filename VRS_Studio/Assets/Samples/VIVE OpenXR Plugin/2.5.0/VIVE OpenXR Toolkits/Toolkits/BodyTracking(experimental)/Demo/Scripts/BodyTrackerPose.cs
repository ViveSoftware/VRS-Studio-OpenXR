// Copyright HTC Corporation All Rights Reserved.

using System.Text;
using UnityEngine;
using VIVE.OpenXR.Toolkits.BodyTracking.RuntimeDependency;

#if TMPExist
using TMPro;
#endif

namespace VIVE.OpenXR.Toolkits.BodyTracking.Demo
{
	public class BodyTrackerPose : MonoBehaviour
	{
		const string LOG_TAG = "VIVE.OpenXR.Toolkits.BodyTracking.Demo.BodyTrackerPose";
		StringBuilder m_sb = null;
		StringBuilder sb
		{
			get
			{
				if (m_sb == null) { m_sb = new StringBuilder(); }
				return m_sb;
			}
		}
		private void DEBUG(StringBuilder msg) { Rdp.d(LOG_TAG, msg, true); }

		public TrackerLocation m_Role = TrackerLocation.Undefined;
		public TrackerType m_Type = TrackerType.ViveSelfTracker;
		public GameObject m_Model = null;
#if TMPExist
		public TextMeshProUGUI canvasText1 = null;
		public TextMeshProUGUI canvasText2 = null;
#endif

		#region Connection
		private Rdp.Tracker.Id m_Id = Rdp.Tracker.Id.Tracker0;
		private bool m_Connected = false;
		private void OnTrackerConnectionChange(Rdp.Tracker.Id id, bool connected, TrackerLocation location)
		{
			if (connected)
			{
				if (m_Role == location)
				{
					m_Id = id;
					m_Connected = true;

					sb.Clear().Append("OnTrackerConnectionChange() ").Append(gameObject.name).Append(", connected: ").Append(m_Id.Name()).Append(" ").Append(m_Role.Name()).Append(" ").Append(m_Type.Name());
					DEBUG(sb);
				}
			}
			else
			{
				if (m_Id == id)
				{
					m_Connected = false;

					sb.Clear().Append("OnTrackerConnectionChange() ").Append(gameObject.name).Append(", disconnected: ").Append(m_Id.Name()).Append(" ").Append(m_Role.Name()).Append(" ").Append(m_Type.Name());
					DEBUG(sb);
				}
			}
		}
		private void CheckConnection()
		{
			if (m_Connected) { return; }

			for (int i = 0; i < Rdp.Tracker.s_TrackerIds.Length; i++)
			{
				var role = Rdp.Tracker.GetTrackerRole(Rdp.Tracker.s_TrackerIds[i]);

				if (m_Role == role)
				{
					m_Id = Rdp.Tracker.s_TrackerIds[i];
					m_Connected = true;

					sb.Clear().Append("CheckConnection() ").Append(gameObject.name).Append(", connected: ").Append(m_Id.Name()).Append(" ").Append(m_Role.Name()).Append(" ").Append(m_Type.Name());
					DEBUG(sb);

					break;
				}
			}
		}
		#endregion

		private void Start()
		{
			CheckConnection();
		}
		private void OnEnable()
		{
			//Srdp.TrackerRoleCb += OnTrackerConnectionChange;
		}
		private void OnDisable()
		{
			//Srdp.TrackerRoleCb -= OnTrackerConnectionChange;
		}

		Vector3 trackerPos = Vector3.zero;
		Quaternion trackerRot = Quaternion.identity;
		void Update()
		{
#if TMPExist
			if (canvasText1 != null) { canvasText1.text = m_Id.Name(); }
			if (canvasText2 != null) { canvasText2.text = m_Id.Name(); }
#endif
			CheckConnection();
			if (m_Model != null && (m_Model.activeSelf != m_Connected)) { m_Model.SetActive(m_Connected); }
			if (m_Connected)
			{
				if (m_Type != TrackerType.ViveSelfTrackerIM && Rdp.Tracker.GetTrackerPosition(m_Id, out trackerPos)) { transform.localPosition = trackerPos; }
				if (Rdp.Tracker.GetTrackerRotation(m_Id, out trackerRot)) { transform.localRotation = trackerRot; }
			}
		}
	}
}
