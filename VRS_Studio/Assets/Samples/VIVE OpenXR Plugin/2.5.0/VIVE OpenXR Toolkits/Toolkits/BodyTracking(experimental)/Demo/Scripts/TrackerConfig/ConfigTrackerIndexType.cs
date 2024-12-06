// Copyright HTC Corporation All Rights Reserved.

using System.Text;
using UnityEngine;
using UnityEngine.UI;

using VIVE.OpenXR.Toolkits.BodyTracking.RuntimeDependency;

namespace VIVE.OpenXR.Toolkits.BodyTracking.Demo
{
	public class ConfigTrackerIndexType : MonoBehaviour
	{
		#region log
		const string LOG_TAG = "VIVE.OpenXR.Toolkits.BodyTracking.Demo.ConfigTrackerIndexType";
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
		#endregion

		public BodyRoleData bodyData = null;
		public GameObject menuIndex = null;

		public GameObject menuType = null;
		private int GetTypeDropdownValue(TrackerType type)
		{
			if (type == TrackerType.ViveWristTracker) { return 1; }
			if (type == TrackerType.ViveSelfTracker) { return 2; }
			if (type == TrackerType.ViveSelfTrackerIM) { return 3; }

			return 0;
		}
		private TrackerType GetDropdownValueType(int value)
		{
			if (value == 1) { return TrackerType.ViveWristTracker; }
			if (value == 2) { return TrackerType.ViveSelfTracker; }
			if (value == 3) { return TrackerType.ViveSelfTrackerIM; }

			return TrackerType.Undefined;
		}

		private void Start()
		{
			if (bodyData == null) { return; }

			if (menuIndex != null && bodyData.TrackerIndexInputs != null)
			{
				DefineTrackerLocation[] types = menuIndex.GetComponentsInChildren<DefineTrackerLocation>();
				if (types != null)
				{
					for (int location_index = 0; location_index < types.Length; location_index++)
					{
						for (int i = 0; i < bodyData.TrackerIndexInputs.Length; i++)
						{
							if (types[location_index].location == bodyData.TrackerIndexInputs[i].location)
							{
								int value = (int)bodyData.TrackerIndexInputs[i].trackerId;
								sb.Clear().Append("Start() Set ").Append(types[location_index].gameObject.name).Append(" index to ").Append(value); DEBUG(sb);
								types[location_index].gameObject.GetComponentInChildren<Dropdown>().value = value;
							}
						}
					}
				}
			}

			if (menuType != null && bodyData.TrackerTypeInputs != null)
			{
				DefineTrackerLocation[] types = menuType.GetComponentsInChildren<DefineTrackerLocation>();
				if (types != null)
				{
					for (int location_index = 0; location_index < types.Length; location_index++)
					{
						for (int i = 0; i < bodyData.TrackerTypeInputs.Length; i++)
						{
							if (types[location_index].location == bodyData.TrackerTypeInputs[i].location)
							{
								int value = GetTypeDropdownValue(bodyData.TrackerTypeInputs[i].type);
								sb.Clear().Append("Start() Set ").Append(types[location_index].gameObject.name).Append(" type to ").Append(value); DEBUG(sb);
								types[location_index].gameObject.GetComponentInChildren<Dropdown>().value = value;
							}
						}
					}
				}
			}
		}
		private void Update()
		{
			if (bodyData == null)
			{
				if (menuIndex != null && menuIndex.activeSelf) { menuIndex.SetActive(false); }
				if (menuType != null && menuType.activeSelf) { menuType.SetActive(false); }
				return;
			}

			if (bodyData.TrackerPose == BodyRoleData.TrackerBase.IndexBase)
			{
				if (menuIndex != null && !menuIndex.activeSelf) { menuIndex.SetActive(true); }
				if (menuType != null && menuType.activeSelf) { menuType.SetActive(false); }
			}
			else
			{
				if (menuIndex != null && menuIndex.activeSelf) { menuIndex.SetActive(false); }
				if (menuType != null && !menuType.activeSelf) { menuType.SetActive(true); }
			}
		}

		public void OnLeftWristIndexChanged(int value)
		{
			if (bodyData == null || value < 0 || value > 6) { return; }

			sb.Clear().Append("OnLeftWristIndexChanged() ").Append(value); DEBUG(sb);
			bodyData.SetTrackerIndex(TrackerLocation.WristLeft, value);
		}
		public void OnRightWristIndexChanged(int value)
		{
			if (bodyData == null || value < 0 || value > 6) { return; }

			sb.Clear().Append("OnRightWristIndexChanged() ").Append(value); DEBUG(sb);
			bodyData.SetTrackerIndex(TrackerLocation.WristRight, value);
		}
		public void OnWaistIndexChanged(int value)
		{
			if (bodyData == null || value < 0 || value > 6) { return; }

			sb.Clear().Append("OnWaistIndexChanged() ").Append(value); DEBUG(sb);
			bodyData.SetTrackerIndex(TrackerLocation.Waist, value);
		}
		public void OnLeftKneeIndexChanged(int value)
		{
			if (bodyData == null || value < 0 || value > 6) { return; }

			sb.Clear().Append("OnLeftKneeIndexChanged() ").Append(value); DEBUG(sb);
			bodyData.SetTrackerIndex(TrackerLocation.KneeLeft, value);
		}
		public void OnRightKneeIndexChanged(int value)
		{
			if (bodyData == null || value < 0 || value > 6) { return; }

			sb.Clear().Append("OnRightKneeIndexChanged() ").Append(value); DEBUG(sb);
			bodyData.SetTrackerIndex(TrackerLocation.KneeRight, value);
		}
		public void OnLeftAnkleIndexChanged(int value)
		{
			if (bodyData == null || value < 0 || value > 6) { return; }

			sb.Clear().Append("OnLeftAnkleIndexChanged() ").Append(value); DEBUG(sb);
			bodyData.SetTrackerIndex(TrackerLocation.AnkleLeft, value);
		}
		public void OnRightAnkleIndexChanged(int value)
		{
			if (bodyData == null || value < 0 || value > 6) { return; }

			sb.Clear().Append("OnRightAnkleIndexChanged() ").Append(value); DEBUG(sb);
			bodyData.SetTrackerIndex(TrackerLocation.AnkleRight, value);
		}

		private const int kTypeCount = 3; // count of BodyRoleData.TrackerType
		public void OnLeftWristTypeChanged(int value)
		{
			if (bodyData == null || value < 0 || value > kTypeCount) { return; }

			TrackerType type = GetDropdownValueType(value);
			sb.Clear().Append("OnLeftWristTypeChanged() ").Append(type.Name()); DEBUG(sb);
			bodyData.SetTrackerType(TrackerLocation.WristLeft, type);
		}
		public void OnRightWristTypeChanged(int value)
		{
			if (bodyData == null || value < 0 || value > kTypeCount) { return; }

			TrackerType type = GetDropdownValueType(value);
			sb.Clear().Append("OnRightWristTypeChanged() ").Append(type.Name()); DEBUG(sb);
			bodyData.SetTrackerType(TrackerLocation.WristRight, type);
		}
		public void OnWaistTypeChanged(int value)
		{
			if (bodyData == null || value < 0 || value > kTypeCount) { return; }

			TrackerType type = GetDropdownValueType(value);
			sb.Clear().Append("OnWaistTypeChanged() ").Append(type.Name()); DEBUG(sb);
			bodyData.SetTrackerType(TrackerLocation.Waist, type);
		}
		public void OnLeftKneeTypeChanged(int value)
		{
			if (bodyData == null || value < 0 || value > kTypeCount) { return; }

			TrackerType type = GetDropdownValueType(value);
			sb.Clear().Append("OnLeftKneeTypeChanged() ").Append(type.Name()); DEBUG(sb);
			bodyData.SetTrackerType(TrackerLocation.KneeLeft, type);
		}
		public void OnRightKneeTypeChanged(int value)
		{
			if (bodyData == null || value < 0 || value > kTypeCount) { return; }

			TrackerType type = GetDropdownValueType(value);
			sb.Clear().Append("OnRightKneeTypeChanged() ").Append(type.Name()); DEBUG(sb);
			bodyData.SetTrackerType(TrackerLocation.KneeRight, type);
		}
		public void OnLeftAnkleTypeChanged(int value)
		{
			if (bodyData == null || value < 0 || value > kTypeCount) { return; }

			TrackerType type = GetDropdownValueType(value);
			sb.Clear().Append("OnLeftAnkleTypeChanged() ").Append(type.Name()); DEBUG(sb);
			bodyData.SetTrackerType(TrackerLocation.AnkleLeft, type);
		}
		public void OnRightAnkleTypeChanged(int value)
		{
			if (bodyData == null || value < 0 || value > kTypeCount) { return; }

			TrackerType type = GetDropdownValueType(value);
			sb.Clear().Append("OnRightAnkleTypeChanged() ").Append(type.Name()); DEBUG(sb);
			bodyData.SetTrackerType(TrackerLocation.AnkleRight, type);
		}
	}
}
