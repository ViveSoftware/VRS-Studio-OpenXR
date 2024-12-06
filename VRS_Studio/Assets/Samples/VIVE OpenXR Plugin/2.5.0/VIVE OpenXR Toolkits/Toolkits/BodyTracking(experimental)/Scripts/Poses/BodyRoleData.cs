// Copyright HTC Corporation All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VIVE.OpenXR.Toolkits.BodyTracking.RuntimeDependency;
using VIVE.OpenXR.Toolkits.Common;

namespace VIVE.OpenXR.Toolkits.BodyTracking
{
	[DisallowMultipleComponent]
	public sealed class BodyRoleData : MonoBehaviour
	{
		#region Rdp
		const string LOG_TAG = "VIVE.OpenXR.Toolkits.BodyTracking.BodyRoleData";
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

		public enum TrackerBase
		{
			IndexBase,
		}

		[Serializable]
		public class TrackerIndexBase
		{
			public TrackerLocation location = TrackerLocation.Undefined;
			[Range(0, 15)]
			public UInt32 trackerId = 0;

			public TrackerIndexBase(TrackerLocation in_loc, UInt32 in_id)
			{
				location = in_loc;
				trackerId = in_id;
			}
		}
		[Serializable]
		public class TrackerTypeBase
		{
			public TrackerLocation location = TrackerLocation.Undefined;
			public TrackerType type = TrackerType.Undefined;

			public TrackerTypeBase(TrackerLocation in_loc, TrackerType in_type)
			{
				location = in_loc;
				type = in_type;
			}
			public void Update(TrackerLocation in_loc, TrackerType in_type)
			{
				location = in_loc;
				type = in_type;
			}
		}

		#region Inspector
		[SerializeField]
		private TrackerBase m_TrackerPose = TrackerBase.IndexBase;
		public TrackerBase TrackerPose => m_TrackerPose;
		// Pose base inputs.
		[SerializeField]
		private TrackerIndexBase[] m_TrackerIndexInputs = new TrackerIndexBase[]
		{
			new TrackerIndexBase(TrackerLocation.WristLeft, 1),
			new TrackerIndexBase(TrackerLocation.WristRight, 0),
			new TrackerIndexBase(TrackerLocation.Waist, 2),
			new TrackerIndexBase(TrackerLocation.KneeLeft, 3),
			new TrackerIndexBase(TrackerLocation.KneeRight, 4),
			new TrackerIndexBase(TrackerLocation.AnkleLeft, 5),
			new TrackerIndexBase(TrackerLocation.AnkleRight, 6),
		};
		public TrackerIndexBase[] TrackerIndexInputs => m_TrackerIndexInputs;
		public void SetTrackerIndex(TrackerLocation location, int index)
		{
			if (m_TrackerIndexInputs == null) { return; }
			bool[] usedIds = new bool[16];
			uint newId = (UInt32)(index & 0x7FFFFFFF);
			for (int i = 0; i < m_TrackerIndexInputs.Length; i++)
			{
				if (m_TrackerIndexInputs[i].location == location)
				{
					m_TrackerIndexInputs[i].trackerId = newId;
					Rdp.Tracker.SetTrackerRole(location, (Rdp.Tracker.Id)m_TrackerIndexInputs[i].trackerId);
					sb.Clear().Append("SetTrackerIndex() ").Append(location).Append(" trackerId: ").Append(m_TrackerIndexInputs[i].trackerId); DEBUG(sb);
				}
				usedIds[m_TrackerIndexInputs[i].trackerId] = true;
			}

			for (int i = 0; i < m_TrackerIndexInputs.Length; i++)
			{
				if (m_TrackerIndexInputs[i].location != location &&
					m_TrackerIndexInputs[i].trackerId == newId)
				{
					int id = Array.FindIndex(usedIds, value => !value);
					if (id != -1)
					{
						m_TrackerIndexInputs[i].trackerId = (uint)id;
						Rdp.Tracker.SetTrackerRole(m_TrackerIndexInputs[i].location, (Rdp.Tracker.Id)m_TrackerIndexInputs[i].trackerId);
						sb.Clear().Append("SetTrackerIndex() to avoid duplicate trackerId ").Append(m_TrackerIndexInputs[i].location).Append(" trackerId: ").Append(m_TrackerIndexInputs[i].trackerId); DEBUG(sb);
						usedIds[id] = true;
					}
				}
			}
		}

		// Type base inputs. (Wave only)
		[SerializeField]
		private TrackerTypeBase[] m_TrackerTypeInputs = new TrackerTypeBase[]
		{
			new TrackerTypeBase(TrackerLocation.WristLeft, TrackerType.ViveSelfTracker),
			new TrackerTypeBase(TrackerLocation.WristRight, TrackerType.ViveSelfTracker),
			new TrackerTypeBase(TrackerLocation.Waist, TrackerType.ViveSelfTracker),
			new TrackerTypeBase(TrackerLocation.KneeLeft, TrackerType.ViveSelfTrackerIM),
			new TrackerTypeBase(TrackerLocation.KneeRight, TrackerType.ViveSelfTrackerIM),
			new TrackerTypeBase(TrackerLocation.AnkleLeft, TrackerType.ViveSelfTrackerIM),
			new TrackerTypeBase(TrackerLocation.AnkleRight, TrackerType.ViveSelfTrackerIM),
		};
		public TrackerTypeBase[] TrackerTypeInputs => m_TrackerTypeInputs;
		public void SetTrackerType(TrackerLocation location, TrackerType type)
		{
			if (m_TrackerTypeInputs == null) { return; }
			for (int i = 0; i < m_TrackerTypeInputs.Length; i++)
			{
				if (m_TrackerTypeInputs[i].location == location)
				{
					m_TrackerTypeInputs[i].type = type;
					sb.Clear().Append("SetTrackerType() ").Append(location).Append(" type: ").Append(m_TrackerTypeInputs[i].type); DEBUG(sb);
				}
			}
		}
		#endregion

		#region Role Data
		[Flags]
		public enum VelocityFlag { ANGULAR = 1, LINEAR = 2 }
		public struct RoleData
		{
			public VelocityFlag velocityState;
			public TrackedDevicePose devicePose;

			public RoleData(VelocityFlag in_state, TrackedDevicePose in_pose)
			{
				velocityState = in_state;
				devicePose = in_pose;
			}
			public static RoleData identity
			{
				get { return new RoleData(0, TrackedDevicePose.identity); }
			}
		}

		private static RoleData[] s_RoleData = new RoleData[(Int32)TrackedDeviceRole.NUMS_OF_ROLE]
		{
			new RoleData(0, new TrackedDevicePose(TrackedDeviceRole.ROLE_HIP, PoseState.NODATA, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Quaternion.identity)),
			new RoleData(0, new TrackedDevicePose(TrackedDeviceRole.ROLE_CHEST, PoseState.NODATA, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Quaternion.identity)),
			new RoleData(0, new TrackedDevicePose(TrackedDeviceRole.ROLE_HEAD, PoseState.NODATA, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Quaternion.identity)),

			new RoleData(0, new TrackedDevicePose(TrackedDeviceRole.ROLE_LEFTELBOW, PoseState.NODATA, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Quaternion.identity)),
			new RoleData(0, new TrackedDevicePose(TrackedDeviceRole.ROLE_LEFTWRIST, PoseState.NODATA, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Quaternion.identity)),
			new RoleData(0, new TrackedDevicePose(TrackedDeviceRole.ROLE_LEFTHAND, PoseState.NODATA, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Quaternion.identity)),
			new RoleData(0, new TrackedDevicePose(TrackedDeviceRole.ROLE_LEFTHANDHELD, PoseState.NODATA, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Quaternion.identity)),

			new RoleData(0, new TrackedDevicePose(TrackedDeviceRole.ROLE_RIGHTELBOW, PoseState.NODATA, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Quaternion.identity)),
			new RoleData(0, new TrackedDevicePose(TrackedDeviceRole.ROLE_RIGHTWRIST, PoseState.NODATA, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Quaternion.identity)),
			new RoleData(0, new TrackedDevicePose(TrackedDeviceRole.ROLE_RIGHTHAND, PoseState.NODATA, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Quaternion.identity)),
			new RoleData(0, new TrackedDevicePose(TrackedDeviceRole.ROLE_RIGHTHANDHELD, PoseState.NODATA, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Quaternion.identity)),

			new RoleData(0, new TrackedDevicePose(TrackedDeviceRole.ROLE_LEFTKNEE, PoseState.NODATA, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Quaternion.identity)),
			new RoleData(0, new TrackedDevicePose(TrackedDeviceRole.ROLE_LEFTANKLE, PoseState.NODATA, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Quaternion.identity)),
			new RoleData(0, new TrackedDevicePose(TrackedDeviceRole.ROLE_LEFTFOOT, PoseState.NODATA, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Quaternion.identity)),

			new RoleData(0, new TrackedDevicePose(TrackedDeviceRole.ROLE_RIGHTKNEE, PoseState.NODATA, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Quaternion.identity)),
			new RoleData(0, new TrackedDevicePose(TrackedDeviceRole.ROLE_RIGHTANKLE, PoseState.NODATA, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Quaternion.identity)),
			new RoleData(0, new TrackedDevicePose(TrackedDeviceRole.ROLE_RIGHTFOOT, PoseState.NODATA, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Quaternion.identity)),
		};
		public static RoleData GetRoleData(TrackedDeviceRole role)
		{
			if (role > TrackedDeviceRole.ROLE_UNDEFINED && role < TrackedDeviceRole.NUMS_OF_ROLE)
				return s_RoleData[(Int32)role];

			return RoleData.identity;
		}
		private void UpdateRoleData(RolePoseType type, ref RoleData data)
		{
			RolePose pose = RolePoseProvider.GetRolePose(type);
			if (pose == null) { return; }

			data.velocityState = 0;
			data.devicePose.poseState = 0;
			if (!pose.IsTracked()) { return; }
			if (pose.GetRotation(out data.devicePose.rotation)) { data.devicePose.poseState |= PoseState.ROTATION; }
			if (pose.GetPosition(out data.devicePose.translation)) { data.devicePose.poseState |= PoseState.TRANSLATION; }
			if (pose.GetAngularVelocity(out data.devicePose.angularVelocity)) { data.velocityState |= VelocityFlag.ANGULAR; }
			if (pose.GetLinearVelocity(out data.devicePose.velocity)) { data.velocityState |= VelocityFlag.LINEAR; }
			pose.GetAcceleration(out data.devicePose.acceleration);
		}
		private void UpdateRoleData(UInt32 trackerId, ref RoleData data)
		{
			if (trackerId == 0) { UpdateRoleData(RolePoseType.TRACKER_0, ref data); }
			if (trackerId == 1) { UpdateRoleData(RolePoseType.TRACKER_1, ref data); }
			if (trackerId == 2) { UpdateRoleData(RolePoseType.TRACKER_2, ref data); }
			if (trackerId == 3) { UpdateRoleData(RolePoseType.TRACKER_3, ref data); }
			if (trackerId == 4) { UpdateRoleData(RolePoseType.TRACKER_4, ref data); }
			if (trackerId == 5) { UpdateRoleData(RolePoseType.TRACKER_5, ref data); }
			if (trackerId == 6) { UpdateRoleData(RolePoseType.TRACKER_6, ref data); }
			if (trackerId == 7) { UpdateRoleData(RolePoseType.TRACKER_7, ref data); }
			if (trackerId == 8) { UpdateRoleData(RolePoseType.TRACKER_8, ref data); }
			if (trackerId == 9) { UpdateRoleData(RolePoseType.TRACKER_9, ref data); }
			if (trackerId == 10) { UpdateRoleData(RolePoseType.TRACKER_10, ref data); }
			if (trackerId == 11) { UpdateRoleData(RolePoseType.TRACKER_11, ref data); }
			if (trackerId == 12) { UpdateRoleData(RolePoseType.TRACKER_12, ref data); }
			if (trackerId == 13) { UpdateRoleData(RolePoseType.TRACKER_13, ref data); }
			if (trackerId == 14) { UpdateRoleData(RolePoseType.TRACKER_14, ref data); }
			if (trackerId == 15) { UpdateRoleData(RolePoseType.TRACKER_15, ref data); }
		}
		private void UpdateRoleData(TrackerType type, ref RoleData data)
		{
			GetTrackerData(type, ref data);
		}
		#endregion

		#region Monobehaviour
		private RolePoseHead m_RoleHead = null;
		private RolePoseController m_RoleControllerL = null, m_RoleControllerR = null;
		private RolePoseHand m_RoleHandL = null, m_RoleHandR = null;
		private List<RolePoseTracker> s_RoleTracker = new List<RolePoseTracker>();
		private void Awake()
		{
			var hmd = new GameObject("HeadPose");
			hmd.transform.SetParent(transform);
			hmd.SetActive(false);
			m_RoleHead = hmd.AddComponent<RolePoseHead>();
			hmd.SetActive(true);

			var ctrlL = new GameObject("LeftControllerPose");
			ctrlL.transform.SetParent(transform);
			ctrlL.SetActive(false);
			m_RoleControllerL = ctrlL.AddComponent<RolePoseController>();
			m_RoleControllerL.isLeft = true;
			ctrlL.SetActive(true);

			var ctrlR = new GameObject("RightControllerPose");
			ctrlR.transform.SetParent(transform);
			ctrlR.SetActive(false);
			m_RoleControllerR = ctrlR.AddComponent<RolePoseController>();
			m_RoleControllerR.isLeft = false;
			ctrlR.SetActive(true);

			var handL = new GameObject("LeftHandPose");
			handL.transform.SetParent(transform);
			handL.SetActive(false);
			m_RoleHandL = handL.AddComponent<RolePoseHand>();
			m_RoleHandL.isLeft = true;
			handL.SetActive(true);

			var handR = new GameObject("RightHandPose");
			handR.transform.SetParent(transform);
			handR.SetActive(false);
			m_RoleHandR = handR.AddComponent<RolePoseHand>();
			m_RoleHandR.isLeft = false;
			handR.SetActive(true);

			if (m_TrackerPose == TrackerBase.IndexBase && m_TrackerIndexInputs != null)
			{
				for (int i = 0; i < m_TrackerIndexInputs.Length; i++)
				{
					string name = "TrackerIndexPose" + m_TrackerIndexInputs[i].trackerId;
					var tracker = new GameObject(name);
					tracker.transform.SetParent(transform);
					tracker.SetActive(false);
					var pose = tracker.AddComponent<RolePoseTracker>();
					if (m_TrackerIndexInputs[i].trackerId == 0) { pose.trackerId = Rdp.Tracker.Id.Tracker0; }
					if (m_TrackerIndexInputs[i].trackerId == 1) { pose.trackerId = Rdp.Tracker.Id.Tracker1; }
					if (m_TrackerIndexInputs[i].trackerId == 2) { pose.trackerId = Rdp.Tracker.Id.Tracker2; }
					if (m_TrackerIndexInputs[i].trackerId == 3) { pose.trackerId = Rdp.Tracker.Id.Tracker3; }
					if (m_TrackerIndexInputs[i].trackerId == 4) { pose.trackerId = Rdp.Tracker.Id.Tracker4; }
					if (m_TrackerIndexInputs[i].trackerId == 5) { pose.trackerId = Rdp.Tracker.Id.Tracker5; }
					if (m_TrackerIndexInputs[i].trackerId == 6) { pose.trackerId = Rdp.Tracker.Id.Tracker6; }
					if (m_TrackerIndexInputs[i].trackerId == 7) { pose.trackerId = Rdp.Tracker.Id.Tracker7; }
					if (m_TrackerIndexInputs[i].trackerId == 8) { pose.trackerId = Rdp.Tracker.Id.Tracker8; }
					if (m_TrackerIndexInputs[i].trackerId == 9) { pose.trackerId = Rdp.Tracker.Id.Tracker9; }
					if (m_TrackerIndexInputs[i].trackerId == 10) { pose.trackerId = Rdp.Tracker.Id.Tracker10; }
					if (m_TrackerIndexInputs[i].trackerId == 11) { pose.trackerId = Rdp.Tracker.Id.Tracker11; }
					if (m_TrackerIndexInputs[i].trackerId == 12) { pose.trackerId = Rdp.Tracker.Id.Tracker12; }
					if (m_TrackerIndexInputs[i].trackerId == 13) { pose.trackerId = Rdp.Tracker.Id.Tracker13; }
					if (m_TrackerIndexInputs[i].trackerId == 14) { pose.trackerId = Rdp.Tracker.Id.Tracker14; }
					if (m_TrackerIndexInputs[i].trackerId == 15) { pose.trackerId = Rdp.Tracker.Id.Tracker15; }

					if (s_RoleTracker == null) { s_RoleTracker = new List<RolePoseTracker>(); }
					s_RoleTracker.Add(pose);
					tracker.SetActive(true);
				}
			}
		}
		private void Start()
		{
			if (m_TrackerPose == TrackerBase.IndexBase && m_TrackerIndexInputs != null)
			{
				for (int i = 0; i < m_TrackerIndexInputs.Length; i++)
				{
					sb.Clear().Append("Start() Use id ").Append(m_TrackerIndexInputs[i].trackerId).Append(" for role ").Append(m_TrackerIndexInputs[i].location.Name());
					DEBUG(sb);
				}
			}
		}
		private void Update()
		{
			for (int i = 0; i < s_RoleData.Length; i++)
			{
				// Head
				if (s_RoleData[i].devicePose.trackedDeviceRole == TrackedDeviceRole.ROLE_HEAD) { UpdateRoleData(RolePoseType.HMD, ref s_RoleData[i]); }

				// Controller
				if (s_RoleData[i].devicePose.trackedDeviceRole == TrackedDeviceRole.ROLE_LEFTHANDHELD) { UpdateRoleData(RolePoseType.CONTROLLER_LEFT, ref s_RoleData[i]); }
				if (s_RoleData[i].devicePose.trackedDeviceRole == TrackedDeviceRole.ROLE_RIGHTHANDHELD) { UpdateRoleData(RolePoseType.CONTROLLER_RIGHT, ref s_RoleData[i]); }

				// Hand
				if (s_RoleData[i].devicePose.trackedDeviceRole == TrackedDeviceRole.ROLE_LEFTHAND) { UpdateRoleData(RolePoseType.HAND_LEFT, ref s_RoleData[i]); }
				if (s_RoleData[i].devicePose.trackedDeviceRole == TrackedDeviceRole.ROLE_RIGHTHAND) { UpdateRoleData(RolePoseType.HAND_RIGHT, ref s_RoleData[i]); }
			}

			if (m_TrackerPose == TrackerBase.IndexBase && m_TrackerIndexInputs != null)
			{
				for (int i = 0; i < m_TrackerIndexInputs.Length; i++)
					UpdateRoleData(m_TrackerIndexInputs[i].trackerId, ref s_RoleData[(Int32)m_TrackerIndexInputs[i].location]);
			}
		}
		#endregion

		#region Device Data
		private void GetHeadData(ref RoleData data)
		{
			data.velocityState = 0;
			data.devicePose.poseState = 0;
			if (!Rdp.Head.IsTracked()) { return; }
			if (Rdp.Head.GetRotation(ref data.devicePose.rotation)) { data.devicePose.poseState |= PoseState.ROTATION; }
			if (Rdp.Head.GetPosition(ref data.devicePose.translation)) { data.devicePose.poseState |= PoseState.TRANSLATION; }
			if (Rdp.Head.GetAngularVelocity(ref data.devicePose.angularVelocity)) { data.velocityState |= VelocityFlag.ANGULAR; }
			if (Rdp.Head.GetVelocity(ref data.devicePose.velocity)) { data.velocityState |= VelocityFlag.LINEAR; }
			Rdp.Head.GetAcceleration(ref data.devicePose.acceleration);
		}
		private void GetControllerData(ref RoleData data, bool isLeft)
		{
			data.velocityState = 0;
			data.devicePose.poseState = 0;
			if (!Rdp.Controller.IsTracked(isLeft)) { return; }
			if (Rdp.Controller.GetRotation(isLeft, ref data.devicePose.rotation)) { data.devicePose.poseState |= PoseState.ROTATION; }
			if (Rdp.Controller.GetPosition(isLeft, ref data.devicePose.translation)) { data.devicePose.poseState |= PoseState.TRANSLATION; }
			if (Rdp.Controller.GetAngularVelocity(isLeft, ref data.devicePose.angularVelocity)) { data.velocityState |= VelocityFlag.ANGULAR; }
			if (Rdp.Controller.GetVelocity(isLeft, ref data.devicePose.velocity)) { data.velocityState |= VelocityFlag.LINEAR; }
			Rdp.Controller.GetAcceleration(isLeft, ref data.devicePose.acceleration);
		}
		private void GetHandData(ref RoleData data, bool isLeft)
		{
			data.velocityState = 0;
			data.devicePose.poseState = 0;
			if (Rdp.Hand.GetJointRotation(HandJointType.Palm, ref data.devicePose.rotation, isLeft)) { data.devicePose.poseState |= PoseState.ROTATION; }
			if (Rdp.Hand.GetJointPosition(HandJointType.Palm, ref data.devicePose.translation, isLeft)) { data.devicePose.poseState |= PoseState.TRANSLATION; }
			if (Rdp.Hand.GetWristAngularVelocity(ref data.devicePose.angularVelocity, isLeft)) { data.velocityState |= VelocityFlag.ANGULAR; }
			if (Rdp.Hand.GetWristLinearVelocity(ref data.devicePose.velocity, isLeft)) { data.velocityState |= VelocityFlag.LINEAR; }
		}

		private int trackerFrame = -1;
		private TrackerLocation[] s_TrackerRole = new TrackerLocation[16] { // TrackerUtils.s_TrackerIds.Length
			TrackerLocation.Undefined,TrackerLocation.Undefined,TrackerLocation.Undefined,TrackerLocation.Undefined,TrackerLocation.Undefined,
			TrackerLocation.Undefined,TrackerLocation.Undefined,TrackerLocation.Undefined,TrackerLocation.Undefined,TrackerLocation.Undefined,
			TrackerLocation.Undefined,TrackerLocation.Undefined,TrackerLocation.Undefined,TrackerLocation.Undefined,TrackerLocation.Undefined,
			TrackerLocation.Undefined,
		};
		private void UpdateTrackerRolesAndTypes()
		{
			if (trackerFrame == Time.frameCount) { return; }
			trackerFrame = Time.frameCount;

			for (int i = 0; i < Rdp.Tracker.s_TrackerIds.Length; i++)
			{
				var id = Rdp.Tracker.s_TrackerIds[i];
				if (!Rdp.Tracker.IsTracked(id))
				{
					s_TrackerRole[i] = TrackerLocation.Undefined;
				}
				else
				{
					s_TrackerRole[i] = Rdp.Tracker.GetTrackerRole(id);
				}
			}
		}
		private void GetTrackerData(TrackerType type, ref RoleData data)
		{
			//sb.Clear().Append("GetTrackerData() ").Append(data.devicePose.trackedDeviceRole.Name()).Append(", type: ").Append(type.Name()); DEBUG(sb);

			data.velocityState = 0;
			data.devicePose.poseState = 0;

			UpdateTrackerRolesAndTypes();
			for (int i = 0; i < Rdp.Tracker.s_TrackerIds.Length; i++)
			{
				var id = Rdp.Tracker.s_TrackerIds[i];
				// Role should be the same.
				if ((TrackedDeviceRole)s_TrackerRole[i] != data.devicePose.trackedDeviceRole) { continue; }
				// Tracker should have a valid pose.
				if (!Rdp.Tracker.IsTracked(id)) { return; }

				if (Rdp.Tracker.GetTrackerRotation(id, out data.devicePose.rotation)) { data.devicePose.poseState |= PoseState.ROTATION; }
				if (type != TrackerType.ViveSelfTrackerIM && Rdp.Tracker.GetTrackerPosition(id, out data.devicePose.translation)) { data.devicePose.poseState |= PoseState.TRANSLATION; }
				if (Rdp.Tracker.GetTrackerAngularVelocity(id, out data.devicePose.angularVelocity)) { data.velocityState |= VelocityFlag.ANGULAR; }
				if (Rdp.Tracker.GetTrackerVelocity(id, out data.devicePose.velocity)) { data.velocityState |= VelocityFlag.LINEAR; }

				break; // prevent different IDs have the same role.
			}
		}
		#endregion
	}
}
