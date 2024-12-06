// Copyright HTC Corporation All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using VIVE.OpenXR.Toolkits.Common;

namespace VIVE.OpenXR.Toolkits.BodyTracking.RuntimeDependency
{
	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_Vector3f_t
	{
		public float v0;  // float[3]
		public float v1;
		public float v2;

		public override bool Equals(object rhs)
		{
			if (rhs is WVR_Vector3f_t)
				return this == (WVR_Vector3f_t)rhs;
			return false;
		}

		public override int GetHashCode()
		{
			return v0.GetHashCode() ^ v1.GetHashCode() ^ v2.GetHashCode();
		}

		public static bool operator ==(WVR_Vector3f_t lhs, WVR_Vector3f_t rhs)
		{
			return lhs.v0 == rhs.v0 && lhs.v1 == rhs.v1 && lhs.v2 == rhs.v2;
		}

		public static bool operator !=(WVR_Vector3f_t lhs, WVR_Vector3f_t rhs)
		{
			return lhs.v0 != rhs.v0 || lhs.v1 != rhs.v1 || lhs.v2 != rhs.v2;
		}

		public WVR_Vector3f_t(float x, float y, float z)
		{
			v0 = x;
			v1 = y;
			v2 = z;
		}
		public static WVR_Vector3f_t Zero
		{
			get
			{
				return new WVR_Vector3f_t(0, 0, 0);
			}
		}

		// No coordination conversion
		public Vector3 ToVector3()
		{
			return new Vector3(v0, v1, v2);
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WVR_Quatf_t
	{
		public float w;
		public float x;
		public float y;
		public float z;

		public override bool Equals(object rhs)
		{
			if (rhs is WVR_Quatf_t)
				return this == (WVR_Quatf_t)rhs;
			return false;
		}

		public override int GetHashCode()
		{
			return w.GetHashCode() ^ x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
		}

		public static bool operator ==(WVR_Quatf_t lhs, WVR_Quatf_t rhs)
		{
			return lhs.w == rhs.w && lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z;
		}

		public static bool operator !=(WVR_Quatf_t lhs, WVR_Quatf_t rhs)
		{
			return lhs.w != rhs.w || lhs.x != rhs.x || lhs.y != rhs.y || lhs.z != rhs.z;
		}

		public WVR_Quatf_t(float in_x, float in_y, float in_z, float in_w)
		{
			x = in_x;
			y = in_y;
			z = in_z;
			w = in_w;
		}
		public static WVR_Quatf_t Identity
		{
			get
			{
				return new WVR_Quatf_t(0, 0, 0, 1);
			}
		}

		// No coordination conversion
		public Quaternion ToQuaternion()
		{
			return new Quaternion(x, y, z, w);
		}
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct WVR_Pose_t    // [FieldOffset(28)]
	{
		[FieldOffset(0)] public WVR_Vector3f_t position;
		[FieldOffset(12)] public WVR_Quatf_t rotation;

		public static bool operator ==(WVR_Pose_t lhs, WVR_Pose_t rhs)
		{
			return lhs.position == rhs.position && lhs.rotation == rhs.rotation;
		}

		public static bool operator !=(WVR_Pose_t lhs, WVR_Pose_t rhs)
		{
			return lhs.position != rhs.position || lhs.rotation != rhs.rotation;
		}

		public override bool Equals(object rhs)
		{
			if (rhs is WVR_Pose_t)
				return this == (WVR_Pose_t)rhs;
			return false;
		}

		public override int GetHashCode()
		{
			return position.GetHashCode() ^ rotation.GetHashCode();
		}

		public WVR_Pose_t(WVR_Vector3f_t in_pos, WVR_Quatf_t in_rot)
		{
			position = in_pos;
			rotation = in_rot;
		}

		public static WVR_Pose_t Identity
		{
			get
			{
				return new WVR_Pose_t(WVR_Vector3f_t.Zero, WVR_Quatf_t.Identity);
			}
		}

		// No coordination conversion
		public Pose ToPose()
		{
			return new Pose(position.ToVector3(), rotation.ToQuaternion());
		}
	}

	public enum TrackingOrigin
	{
		Local = 0, // WVR_PoseOriginModel.WVR_PoseOriginModel_OriginOnHead,
		Global = 1, // WVR_PoseOriginModel.WVR_PoseOriginModel_OriginOnGround
	}
	public enum TrackerType : UInt32
	{
		Undefined = TrackedDeviceType.Invalid,
		ViveWristTracker = TrackedDeviceType.ViveWristTracker,
		ViveSelfTracker = TrackedDeviceType.ViveSelfTracker,
		ViveSelfTrackerIM = TrackedDeviceType.ViveSelfTrackerIM,
	}
	public enum TrackerLocation : Int32 // Legal wearing location of Tracker.
	{
		Undefined = TrackedDeviceRole.ROLE_UNDEFINED,

		WristLeft = TrackedDeviceRole.ROLE_LEFTWRIST,
		WristRight = TrackedDeviceRole.ROLE_RIGHTWRIST,

		Waist = TrackedDeviceRole.ROLE_HIP,

		KneeLeft = TrackedDeviceRole.ROLE_LEFTKNEE,
		KneeRight = TrackedDeviceRole.ROLE_RIGHTKNEE,

		AnkleLeft = TrackedDeviceRole.ROLE_LEFTANKLE,
		AnkleRight = TrackedDeviceRole.ROLE_RIGHTANKLE,

		FootLeft = TrackedDeviceRole.ROLE_LEFTFOOT,
		FootRight = TrackedDeviceRole.ROLE_RIGHTFOOT,
	}
	public static class Rdp
	{
		#region Log
		public static void d(string tag, StringBuilder sb, bool logInEditor = false)
		{
			Debug.Log($"{tag}, {sb.ToString()}");
		}
		public static void w(string tag, StringBuilder sb, bool logInEditor = false)
		{
			Debug.LogWarning($"{tag}, {sb.ToString()}");
		}
		public static void e(string tag, StringBuilder sb, bool logInEditor = false)
		{
			Debug.LogError($"{tag}, {sb.ToString()}");
		}

		const string LOG_TAG = "VIVE.OpenXR.Toolkits.BodyTracking.RuntimeDependency.Rdp";
		static StringBuilder m_sb = null;
		static StringBuilder sb
		{
			get
			{
				if (m_sb == null) { m_sb = new StringBuilder(); }
				return m_sb;
			}
		}
		static void DEBUG(StringBuilder msg) { d(LOG_TAG, msg, true); }
		#endregion

		public static string Name(this TrackingOrigin origin)
		{
			if (origin == TrackingOrigin.Local) { return "Local"; }
			return "Global";
		}

		#region Coordinate
		// Validates the rotation, including WVR_Quatf_t, Quaternion and Vector4
		public static bool isZero(this WVR_Quatf_t qua)
		{
			if (qua.x == 0 &&
				qua.y == 0 &&
				qua.z == 0 &&
				qua.w == 0)
			{
				return true;
			}

			return false;
		}
		public static bool isMinusHomogeneous(this WVR_Quatf_t qua)
		{
			if (qua.x == 0 &&
				qua.y == 0 &&
				qua.z == 0 &&
				qua.w == -1)
			{
				return true;
			}

			return false;
		}
		public static void Validate(ref WVR_Quatf_t qua)
		{
			if (qua.isZero() || qua.isMinusHomogeneous()) { qua = WVR_Quatf_t.Identity; }
		}
		public static bool isZero(this Quaternion qua)
		{
			if (qua.x == 0 &&
				qua.y == 0 &&
				qua.z == 0 &&
				qua.w == 0)
			{
				return true;
			}

			return false;
		}
		public static bool isMinusHomogeneous(this Quaternion qua)
		{
			if (qua.x == 0 &&
				qua.y == 0 &&
				qua.z == 0 &&
				qua.w == -1)
			{
				return true;
			}

			return false;
		}
		public static void Validate(ref Quaternion qua)
		{
			if (qua.isZero() || qua.isMinusHomogeneous()) { qua = Quaternion.identity; }
		}
		public static bool isZero(this Vector4 qua)
		{
			if (qua.x == 0 &&
				qua.y == 0 &&
				qua.z == 0 &&
				qua.w == 0)
			{
				return true;
			}

			return false;
		}
		public static bool isMinusHomogeneous(this Vector4 qua)
		{
			if (qua.x == 0 &&
				qua.y == 0 &&
				qua.z == 0 &&
				qua.w == -1)
			{
				return true;
			}

			return false;
		}
		public static void Validate(ref Vector4 qua)
		{
			if (qua.isZero() || qua.isMinusHomogeneous()) { qua = new Vector4(0, 0, 0, 1); }
		}

		// Transforms and validates the rotation, including WVR_Quatf_t, Quaternion and Vector4
		public static Quaternion GetQuaternionFromGL(WVR_Quatf_t glQuat)
		{
			Quaternion quat = Quaternion.identity;
			quat.x = glQuat.x;
			quat.y = glQuat.y;
			quat.z = -glQuat.z;
			quat.w = -glQuat.w;
			Validate(ref quat);
			return quat;
		}
		public static void GetQuaternionFromGL(WVR_Quatf_t glQuat, out Quaternion unityQuat)
		{
			unityQuat.x = glQuat.x;
			unityQuat.y = glQuat.y;
			unityQuat.z = -glQuat.z;
			unityQuat.w = -glQuat.w;
			Validate(ref unityQuat);
		}

		public static Vector4 GetVector4FromGL(WVR_Quatf_t glQuat)
		{
			Vector4 quat = Vector4.zero;
			quat.x = glQuat.x;
			quat.y = glQuat.y;
			quat.z = -glQuat.z;
			quat.w = -glQuat.w;
			Validate(ref quat);
			return quat;
		}
		public static void GetVector4FromGL(WVR_Quatf_t glQuat, out Vector4 unityQuat)
		{
			unityQuat.x = glQuat.x;
			unityQuat.y = glQuat.y;
			unityQuat.z = -glQuat.z;
			unityQuat.w = -glQuat.w;
			Validate(ref unityQuat);
		}
		public static void GetVector4FromGL(Quaternion glQuat, out Vector4 unityQuat)
		{
			unityQuat.x = glQuat.x;
			unityQuat.y = glQuat.y;
			unityQuat.z = -glQuat.z;
			unityQuat.w = -glQuat.w;
			Validate(ref unityQuat);
		}

		public static WVR_Quatf_t GetQuatfFromUnity(Quaternion unityQuat)
		{
			WVR_Quatf_t qua = WVR_Quatf_t.Identity;
			qua.x = unityQuat.x;
			qua.y = unityQuat.y;
			qua.z = -unityQuat.z;
			qua.w = -unityQuat.w;
			Validate(ref qua);
			return qua;
		}
		public static void GetQuatfFromUnity(Quaternion unityQuat, out WVR_Quatf_t glQuat)
		{
			glQuat.x = unityQuat.x;
			glQuat.y = unityQuat.y;
			glQuat.z = -unityQuat.z;
			glQuat.w = -unityQuat.w;
			Validate(ref glQuat);
		}

		// Transforms the translation, including WVR_Vector3f_t and Vector3.
		public static void GetVector3fFromUnity(Vector3 unityVec, out WVR_Vector3f_t glVec)
		{
			glVec.v0 = unityVec.x;
			glVec.v1 = unityVec.y;
			glVec.v2 = -unityVec.z;
		}
		public static void GetVector3FromGL(WVR_Vector3f_t glVec, out Vector3 unityVec)
		{
			unityVec.x = glVec.v0;
			unityVec.y = glVec.v1;
			unityVec.z = -glVec.v2;
		}

		// Transform the angular translation, including WVR_Vector3f_t and Vector3.
		public static void GetAngularVector3FromGL(WVR_Vector3f_t glVec, out Vector3 unityVec)
		{
			unityVec.x = -glVec.v0;
			unityVec.y = -glVec.v1;
			unityVec.z = glVec.v2;
		}
		#endregion

		public static void GetQuaternion(Vector4 right, out Quaternion left)
		{
			left.x = right.x;
			left.y = right.y;
			left.z = right.z;
			left.w = right.w;
			Validate(ref left);
		}
		public static void GetVector4(Quaternion right, out Vector4 left)
		{
			left.x = right.x;
			left.y = right.y;
			left.z = right.z;
			left.w = right.w;
			Validate(ref left);
		}

		public static class Head
		{
			public static bool IsTracked()
			{
				VIVEInput.GetPoseState(DeviceCategory.HMD, Common.PoseState.IsTracked, out bool isTracked);
				return isTracked;
			}
			public static bool GetRotation(ref Quaternion rotation)
			{
				return VIVEInput.GetPoseState(DeviceCategory.HMD, Common.PoseState.Rotation, out rotation);
			}
			public static bool GetPosition(ref Vector3 position)
			{
				return VIVEInput.GetPoseState(DeviceCategory.HMD, Common.PoseState.Position, out position);
			}
			public static bool GetVelocity(ref Vector3 velocity)
			{
				return VIVEInput.GetPoseState(DeviceCategory.HMD, Common.PoseState.Velocity, out velocity);
			}
			public static bool GetAngularVelocity(ref Vector3 angularVelocity)
			{
				return VIVEInput.GetPoseState(DeviceCategory.HMD, Common.PoseState.AngularVelocity, out angularVelocity);
			}
			public static bool GetAcceleration(ref Vector3 acceleration)
			{
				return VIVEInput.GetPoseState(DeviceCategory.HMD, Common.PoseState.Acceleration, out acceleration);
			}
		}
		public static class Controller
		{
			public static bool IsTracked(bool isLeft)
			{
				if (Srdp.Instance != null)
				{
					InputActionProperty actionReference = isLeft ? Srdp.Instance.leftControllerPose : Srdp.Instance.rightControllerPose;
					if (Srdp.Instance.GetPose(actionReference, out var pose))
					{
						return pose.isTracked || pose.trackingState.HasFlag(InputTrackingState.Rotation);
					}
				}
				VIVEInput.GetPoseState(isLeft ? DeviceCategory.LeftController : DeviceCategory.RightController, Common.PoseState.IsTracked, out bool isTracked);
				return isTracked;
			}
#if USE_INPUT_SYSTEM_POSE_CONTROL
			public static bool IsTracked(bool isLeft, out UnityEngine.InputSystem.XR.PoseState pose)
#else
			public static bool IsTracked(bool isLeft, out UnityEngine.XR.OpenXR.Input.Pose pose)
#endif
			{
				pose = default;
				if (Srdp.Instance != null)
				{
					InputActionProperty inputActionProperty = isLeft ? Srdp.Instance.leftControllerPose : Srdp.Instance.rightControllerPose;
					if (Srdp.Instance.GetPose(inputActionProperty, out pose))
					{
						return pose.isTracked || pose.trackingState.HasFlag(InputTrackingState.Rotation);
					}
				}
				return false;
			}

			public static bool GetRotation(bool isLeft, ref Quaternion rotation)
			{
				if (IsTracked(isLeft, out var pose))
				{
					if (pose.trackingState.HasFlag(InputTrackingState.Rotation))
					{
						rotation = pose.rotation;
						return true;
					}
				}
				else if (IsTracked(isLeft))
				{
					return VIVEInput.GetPoseState(isLeft ? DeviceCategory.LeftController : DeviceCategory.RightController, Common.PoseState.Rotation, out rotation);
				}
				return false;

			}
			public static bool GetPosition(bool isLeft, ref Vector3 position)
			{
				if (IsTracked(isLeft, out var pose))
				{
					if (pose.trackingState.HasFlag(InputTrackingState.Position))
					{
						position = pose.position;
						return true;
					}
				}
				else if (IsTracked(isLeft))
				{
					return VIVEInput.GetPoseState(isLeft ? DeviceCategory.LeftController : DeviceCategory.RightController, Common.PoseState.Position, out position);
				}
				return false;
			}
			public static bool GetVelocity(bool isLeft, ref Vector3 velocity)
			{
				if (IsTracked(isLeft, out var pose))
				{
					if (pose.trackingState.HasFlag(InputTrackingState.Velocity))
					{
						velocity = pose.velocity;
						return true;
					}
				}
				else if (IsTracked(isLeft))
				{
					return VIVEInput.GetPoseState(isLeft ? DeviceCategory.LeftController : DeviceCategory.RightController, Common.PoseState.Velocity, out velocity);
				}
				return false;
			}
			public static bool GetAngularVelocity(bool isLeft, ref Vector3 angularVelocity)
			{
				if (IsTracked(isLeft, out var pose))
				{
					if (pose.trackingState.HasFlag(InputTrackingState.AngularVelocity))
					{
						angularVelocity = pose.angularVelocity;
						return true;
					}
				}
				else if (IsTracked(isLeft))
				{
					return VIVEInput.GetPoseState(isLeft ? DeviceCategory.LeftController : DeviceCategory.RightController, Common.PoseState.AngularVelocity, out angularVelocity);
				}
				return false;
			}
			public static bool GetAcceleration(bool isLeft, ref Vector3 acceleration)
			{
				return VIVEInput.GetPoseState(isLeft ? DeviceCategory.LeftController : DeviceCategory.RightController,
				Common.PoseState.Acceleration, out acceleration);
			}
		}
		public static class Hand
		{
			public static bool IsTracked(bool isLeft)
			{
				VIVEInput.GetPoseState(isLeft ? DeviceCategory.LeftHand : DeviceCategory.RightHand, Common.PoseState.IsTracked, out bool isTracked);
				return isTracked;
			}
			public static bool GetJointRotation(HandJointType joint, ref Quaternion rotation, bool isLeft)
			{
				if (IsTracked(isLeft))
				{
					VIVEInput.GetJointPose(isLeft ? Handedness.Left : Handedness.Right, joint, out Pose jointPose);
					rotation = jointPose.rotation;
					return true;
				}
				return false;
			}
			public static bool GetJointPosition(HandJointType joint, ref Vector3 position, bool isLeft)
			{
				if (IsTracked(isLeft))
				{
					VIVEInput.GetJointPose(isLeft ? Handedness.Left : Handedness.Right, joint, out Pose jointPose);
					position = jointPose.position;
					return true;
				}
				return false;
			}
			public static bool GetWristAngularVelocity(ref Vector3 velocity, bool isLeft)
			{
				return false;
			}
			public static bool GetWristLinearVelocity(ref Vector3 velocity, bool isLeft)
			{
				return false;
			}
			public static bool IsGestureOK(bool isLeft)
			{
				return false;
			}
			public static bool IsGestureLike(bool isLeft)
			{
				return false;
			}
		}
		public static class Tracker
		{
			public enum Id : UInt32
			{
				Tracker0 = 0,
				Tracker1 = 1,
				Tracker2 = 2,
				Tracker3 = 3,
				Tracker4 = 4,
				Tracker5 = 5,
				Tracker6 = 6,
				Tracker7 = 7,
				Tracker8 = 8,
				Tracker9 = 9,
				Tracker10 = 10,
				Tracker11 = 11,
				Tracker12 = 12,
				Tracker13 = 13,
				Tracker14 = 14,
				Tracker15 = 15,
			}
			public static Id[] s_TrackerIds = new Id[]
			{
				Id.Tracker0,
				Id.Tracker1,
				Id.Tracker2,
				Id.Tracker3,
				Id.Tracker4,
				Id.Tracker5,
				Id.Tracker6,
				Id.Tracker7,
				Id.Tracker8,
				Id.Tracker9,
				Id.Tracker10,
				Id.Tracker11,
				Id.Tracker12,
				Id.Tracker13,
				Id.Tracker14,
				Id.Tracker15,
			};
			private static DeviceCategory GetTrackerDeviceCategory(Id id)
			{
				DeviceCategory deviceCategory = DeviceCategory.None;
				switch (id)
				{
					case Id.Tracker0: deviceCategory = DeviceCategory.Tracker0; break;
					case Id.Tracker1: deviceCategory = DeviceCategory.Tracker1; break;
					case Id.Tracker2: deviceCategory = DeviceCategory.Tracker2; break;
					case Id.Tracker3: deviceCategory = DeviceCategory.Tracker3; break;
					case Id.Tracker4: deviceCategory = DeviceCategory.Tracker4; break;
					default: break;
				}
				return deviceCategory;
			}
			public static bool IsTracked(Id trackerId)
			{
				DeviceCategory tracker = GetTrackerDeviceCategory(trackerId);
				if (tracker == DeviceCategory.None) { return false; }
				VIVEInput.GetPoseState(tracker, Common.PoseState.IsTracked, out bool isTracked);
				return isTracked;
			}
#if USE_INPUT_SYSTEM_POSE_CONTROL
			public static bool IsTracked(Id trackerId, out UnityEngine.InputSystem.XR.PoseState pose)
#else
			public static bool IsTracked(Id trackerId, out UnityEngine.XR.OpenXR.Input.Pose pose)
#endif
			{
				pose = default;
				if (Srdp.Instance != null &&
					GetTrackerInputAction(trackerId, out InputActionProperty inputActionProperty) &&
					Srdp.Instance.GetPose(inputActionProperty, out pose))
				{
					return pose.isTracked;
				}
				return false;
			}
			public static bool GetTrackerRotation(Id trackerId, out Quaternion rotation)
			{
				rotation = Quaternion.identity;
				if (IsTracked(trackerId, out var pose))
				{
					if (pose.trackingState.HasFlag(InputTrackingState.Rotation))
					{
						rotation = pose.rotation;
						return true;
					}
				}
				else if (IsTracked(trackerId))
				{
					DeviceCategory tracker = GetTrackerDeviceCategory(trackerId);
					return VIVEInput.GetPoseState(tracker, Common.PoseState.Rotation, out rotation);
				}
				return false;
			}
			public static bool GetTrackerPosition(Id trackerId, out Vector3 position)
			{
				position = Vector3.zero;
				if (IsTracked(trackerId, out var pose))
				{
					if (pose.trackingState.HasFlag(InputTrackingState.Position))
					{
						position = pose.position;
						return true;
					}
				}
				else if (IsTracked(trackerId))
				{
					DeviceCategory tracker = GetTrackerDeviceCategory(trackerId);
					return VIVEInput.GetPoseState(tracker, Common.PoseState.Position, out position);
				}
				return false;
			}
			public static bool GetTrackerVelocity(Id trackerId, out Vector3 velocity)
			{
				velocity = Vector3.zero;
				if (IsTracked(trackerId, out var pose))
				{
					if (pose.trackingState.HasFlag(InputTrackingState.Velocity))
					{
						velocity = pose.velocity;
						return true;
					}
				}
				else if (IsTracked(trackerId))
				{
					DeviceCategory tracker = GetTrackerDeviceCategory(trackerId);
					return VIVEInput.GetPoseState(tracker, Common.PoseState.Velocity, out velocity);
				}
				return false;
			}
			public static bool GetTrackerAngularVelocity(Id trackerId, out Vector3 angularVelocity)
			{
				angularVelocity = Vector3.zero;
				if (IsTracked(trackerId, out var pose))
				{
					if (pose.trackingState.HasFlag(InputTrackingState.AngularVelocity))
					{
						angularVelocity = pose.angularVelocity;
						return true;
					}
				}
				else if (IsTracked(trackerId))
				{
					DeviceCategory tracker = GetTrackerDeviceCategory(trackerId);
					return VIVEInput.GetPoseState(tracker, Common.PoseState.AngularVelocity, out angularVelocity);
				}
				return false;
			}

			private static Dictionary<TrackerLocation, Id> s_ContentRoleMapping = new Dictionary<TrackerLocation, Id>();
			private static object trackerLock = new object();
			public static void SetTrackerRole(TrackerLocation location, Id trackerId)
			{
				lock (trackerLock)
				{
					if (!s_ContentRoleMapping.ContainsKey(location))
					{
						s_ContentRoleMapping.Add(location, trackerId);
					}
					else
					{
						s_ContentRoleMapping[location] = trackerId;
					}
				}
			}
			public static TrackerLocation GetTrackerRole(Id trackerId)
			{
				if (IsTracked(trackerId))
				{
					lock (trackerLock)
					{
						for (int i = 0; i < s_ContentRoleMapping.Count; i++)
						{
							var keyValuePair = s_ContentRoleMapping.ElementAt(i);
							if (keyValuePair.Value == trackerId)
							{
								return keyValuePair.Key;
							}
						}
					}
				}
				return TrackerLocation.Undefined;
			}
			private static bool GetTrackerInputAction(Id id, out InputActionProperty inputActionProperty)
			{
				inputActionProperty = default;
				int intId = (int)id;
				if (Srdp.Instance != null &&
					intId < Srdp.Instance.trackersInputAction.Count)
				{
					InputActionProperty trackerAction = Srdp.Instance.trackersInputAction[intId];
					if (trackerAction != null && trackerAction.action != null)
					{
						inputActionProperty = trackerAction;
						return true;
					}
				}
				return false;
			}
		}

		public static string Name(this TrackerType type)
		{
			if (type == TrackerType.ViveWristTracker) { return "ViveWristTracker"; }
			if (type == TrackerType.ViveSelfTracker) { return "ViveSelfTracker"; }
			if (type == TrackerType.ViveSelfTrackerIM) { return "ViveSelfTrackerIM"; }

			return "Undefined";
		}
		public static string Name(this TrackerLocation role)
		{
			if (role == TrackerLocation.WristLeft) { return "WristLeft"; }
			if (role == TrackerLocation.WristRight) { return "WristRight"; }

			if (role == TrackerLocation.Waist) { return "Waist"; }

			if (role == TrackerLocation.KneeLeft) { return "KneeLeft"; }
			if (role == TrackerLocation.KneeRight) { return "KneeRight"; }

			if (role == TrackerLocation.AnkleLeft) { return "AnkleLeft"; }
			if (role == TrackerLocation.AnkleRight) { return "AnkleRight"; }

			if (role == TrackerLocation.FootLeft) { return "FootLeft"; }
			if (role == TrackerLocation.FootRight) { return "FootRight"; }

			return "Undefined";
		}
		public static string Name(this Tracker.Id id)
		{
			if (id == Tracker.Id.Tracker0) { return "Tracker 0"; }
			if (id == Tracker.Id.Tracker1) { return "Tracker 1"; }
			if (id == Tracker.Id.Tracker2) { return "Tracker 2"; }
			if (id == Tracker.Id.Tracker3) { return "Tracker 3"; }
			if (id == Tracker.Id.Tracker4) { return "Tracker 4"; }
			if (id == Tracker.Id.Tracker5) { return "Tracker 5"; }
			if (id == Tracker.Id.Tracker6) { return "Tracker 6"; }
			if (id == Tracker.Id.Tracker7) { return "Tracker 7"; }
			if (id == Tracker.Id.Tracker8) { return "Tracker 8"; }
			if (id == Tracker.Id.Tracker9) { return "Tracker 9"; }
			if (id == Tracker.Id.Tracker10) { return "Tracker 10"; }
			if (id == Tracker.Id.Tracker11) { return "Tracker 11"; }
			if (id == Tracker.Id.Tracker12) { return "Tracker 12"; }
			if (id == Tracker.Id.Tracker13) { return "Tracker 13"; }
			if (id == Tracker.Id.Tracker14) { return "Tracker 14"; }
			if (id == Tracker.Id.Tracker15) { return "Tracker 15"; }

			return "";
		}

		public static TrackerType ToRdpTracker(this TrackedDeviceType type)
		{
			if (type == TrackedDeviceType.ViveWristTracker) { return TrackerType.ViveWristTracker; }
			if (type == TrackedDeviceType.ViveSelfTracker) { return TrackerType.ViveSelfTracker; }
			if (type == TrackedDeviceType.ViveSelfTrackerIM) { return TrackerType.ViveSelfTrackerIM; }
			return TrackerType.Undefined;
		}
		public static TrackerLocation ToRdpTracker(this TrackedDeviceRole role)
		{
			if (role == TrackedDeviceRole.ROLE_LEFTWRIST) { return TrackerLocation.WristLeft; }
			if (role == TrackedDeviceRole.ROLE_RIGHTWRIST) { return TrackerLocation.WristRight; }
			if (role == TrackedDeviceRole.ROLE_HIP) { return TrackerLocation.Waist; }
			if (role == TrackedDeviceRole.ROLE_LEFTANKLE) { return TrackerLocation.AnkleLeft; }
			if (role == TrackedDeviceRole.ROLE_RIGHTANKLE) { return TrackerLocation.AnkleRight; }
			if (role == TrackedDeviceRole.ROLE_LEFTFOOT) { return TrackerLocation.FootLeft; }
			if (role == TrackedDeviceRole.ROLE_RIGHTFOOT) { return TrackerLocation.FootRight; }

			return TrackerLocation.Undefined;
		}

		public static void Update(ref ExtrinsicVector4_t ext, WVR_Pose_t in_ext)
		{
			GetVector3FromGL(in_ext.position, out ext.translation);
			ext.rotation = GetVector4FromGL(in_ext.rotation);
		}
		public static void Update(ref ExtrinsicInfo_t ext, WVR_Pose_t in_ext)
		{
			ext.isTracking = true;
			Update(ref ext.extrinsic, in_ext);
		}
		public static void Update(ref Extrinsic ext, WVR_Pose_t in_ext)
		{
			GetVector3FromGL(in_ext.position, out ext.translation);
			GetQuaternionFromGL(in_ext.rotation, out ext.rotation);
		}

		public static WVR_Pose_t GetExtrinsicWVR(Extrinsic ext)
		{
			WVR_Pose_t pose;
			GetVector3fFromUnity(ext.translation, out pose.position);
			GetQuatfFromUnity(ext.rotation, out pose.rotation);
			return pose;
		}
		public static WVR_Pose_t GetExtrinsicWVR(ExtrinsicVector4_t ext)
		{
			return GetExtrinsicWVR(ext.GetExtrinsic());
		}
		public static WVR_Pose_t GetExtrinsicWVR(ExtrinsicInfo_t info)
		{
			return GetExtrinsicWVR(info.extrinsic.GetExtrinsic());
		}

#if WAVE_BODY_CALIBRATION
		public static void SetOETIMPoseMode()
		{
			string key = "PLAYER04POSECALIB_START";
			sb.Clear().Append("SetOETIMPoseMode() with key ").Append(key); DEBUG(sb);
			Interop.WVR_SetParameters(WVR_DeviceType.WVR_DeviceType_HMD, Marshal.StringToHGlobalAnsi(key));
		}
		public static void SetOETIMComputeCalibrationResult()
		{
			string key = "PLAYER04POSECALIB_COMPUTE";
			sb.Clear().Append("SetOETIMComputeCalibrationResult() with key ").Append(key); DEBUG(sb);
			Interop.WVR_SetParameters(WVR_DeviceType.WVR_DeviceType_HMD, Marshal.StringToHGlobalAnsi(key));
		}
		public static void SetOETIMTrackingMode()
		{
			string key = "PLAYER04POSECALIB_END";
			sb.Clear().Append("SetOETIMTrackingMode() with key ").Append(key); DEBUG(sb);
			Interop.WVR_SetParameters(WVR_DeviceType.WVR_DeviceType_HMD, Marshal.StringToHGlobalAnsi(key));
		}

		public static UInt32 GetBodyTrackingRedirectExtrinsicCount()
		{
			return Interop.WVR_GetBodyTrackingRedirectExtrinsicCount();
		}
		public static UInt32 GetBodyTrackingDeviceCount()
		{
			return Interop.WVR_GetBodyTrackingDeviceCount();
		}
		public static WVR_Result GetBodyTrackingDeviceInfo(
			ref float userHeight,
			ref WVR_BodyTrackingCalibrationMode mode,
			[In, Out] WVR_BodyTracking_DeviceInfo_t[] devices, ref UInt32 deviceCount,
			[In, Out] WVR_BodyTracking_RedirectExtrinsic_t[] redirectExtrinsics, ref UInt32 redirectCount)
		{
			return Interop.WVR_GetBodyTrackingDeviceInfo(ref userHeight, ref mode, devices, ref deviceCount, redirectExtrinsics, ref redirectCount);
		}
		public static UInt32 GetActiveDeviceExtrinsicCount()
		{
			return Interop.WVR_GetActiveDeviceExtrinsicCount();
		}
		public static WVR_Result GetActiveDeviceExtrinsics([In, Out] WVR_BodyTracking_DeviceInfo_t[] devices, ref UInt32 count)
		{
			return Interop.WVR_GetActiveDeviceExtrinsics(devices, ref count);
		}
		public static WVR_Result ApplyCalibrationData(
			float userHeight,
			[In] WVR_BodyTracking_DeviceInfo_t[] devices, UInt32 deviceCount,
			WVR_BodyTrackingCalibrationMode mode,
			[In] WVR_BodyTracking_RedirectExtrinsic_t[] redirectExtrinsics, UInt32 redirectCount)
		{
			return Interop.WVR_ApplyCalibrationData(userHeight, devices, deviceCount, mode, redirectExtrinsics, redirectCount);
		}

		public static void Update(ref ExtrinsicVector4_t ext, WVR_BodyTracking_Extrinsic_t in_ext)
		{
			GetVector3FromGL(in_ext.position, out ext.translation);
			ext.rotation = GetVector4FromGL(in_ext.rotation);
		}
		public static void Update(ref ExtrinsicInfo_t ext, WVR_BodyTracking_Extrinsic_t in_ext)
		{
			ext.isTracking = true;
			Update(ref ext.extrinsic, in_ext);
		}
		public static void Update(ref Extrinsic ext, WVR_BodyTracking_Extrinsic_t in_ext)
		{
			GetVector3FromGL(in_ext.position, out ext.translation);
			GetQuaternionFromGL(in_ext.rotation, out ext.rotation);
		}
		public static void Update(ref TrackedDevicePose pose, WVR_BodyTrackingDeviceRole in_role, WVR_BodyTracking_Pose_t in_pose)
		{
			pose.trackedDeviceRole = in_role.FromRdp();
			pose.poseState = PoseState.NODATA;
			if (in_pose.poseState.HasFlag(WVR_BodyTrackingPoseState.WVR_BodyTrackingPoseState_Rotation)) { pose.poseState |= PoseState.ROTATION; }
			if (in_pose.poseState.HasFlag(WVR_BodyTrackingPoseState.WVR_BodyTrackingPoseState_Position)) { pose.poseState |= PoseState.TRANSLATION; }
			GetVector3FromGL(in_pose.position, out pose.translation);
			GetVector3FromGL(in_pose.velocity, out pose.velocity);
			GetVector3FromGL(in_pose.acceleration, out pose.acceleration);
			GetQuaternionFromGL(in_pose.rotation, out pose.rotation);
			GetAngularVector3FromGL(in_pose.angularVelocity, out pose.angularVelocity);
		}
		public static void Update(ref TrackedDeviceExtrinsic ext, WVR_BodyTrackingDeviceRole in_role, WVR_BodyTracking_Extrinsic_t in_ext)
		{
			ext.trackedDeviceRole = in_role.FromRdp();
			Update(ref ext.extrinsic, in_ext);
		}
		public static void Update(ref TrackedDeviceRedirectExtrinsic ext, WVR_BodyTracking_RedirectExtrinsic_t in_ext)
		{
			ext.trackedDeviceRole = in_ext.role.FromRdp();
			GetQuaternionFromGL(in_ext.rotation, out ext.rotation);
		}

		public static TrackedDeviceRole FromRdp(this WVR_BodyTrackingDeviceRole role)
		{
			if (role == WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_Hip) { return TrackedDeviceRole.ROLE_HIP; }
			if (role == WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_Chest) { return TrackedDeviceRole.ROLE_CHEST; }
			if (role == WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_Head) { return TrackedDeviceRole.ROLE_HEAD; }

			if (role == WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_LeftElbow) { return TrackedDeviceRole.ROLE_LEFTELBOW; }
			if (role == WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_LeftWrist) { return TrackedDeviceRole.ROLE_LEFTWRIST; }
			if (role == WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_LeftHand) { return TrackedDeviceRole.ROLE_LEFTHAND; }
			if (role == WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_LeftHandheld) { return TrackedDeviceRole.ROLE_LEFTHANDHELD; }

			if (role == WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_RightElbow) { return TrackedDeviceRole.ROLE_RIGHTELBOW; }
			if (role == WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_RightWrist) { return TrackedDeviceRole.ROLE_RIGHTWRIST; }
			if (role == WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_RightHand) { return TrackedDeviceRole.ROLE_RIGHTHAND; }
			if (role == WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_RightHandheld) { return TrackedDeviceRole.ROLE_RIGHTHANDHELD; }

			if (role == WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_LeftKnee) { return TrackedDeviceRole.ROLE_LEFTKNEE; }
			if (role == WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_LeftAnkle) { return TrackedDeviceRole.ROLE_LEFTANKLE; }
			if (role == WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_LeftFoot) { return TrackedDeviceRole.ROLE_LEFTFOOT; }

			if (role == WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_RightKnee) { return TrackedDeviceRole.ROLE_RIGHTKNEE; }
			if (role == WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_RightAnkle) { return TrackedDeviceRole.ROLE_RIGHTANKLE; }
			if (role == WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_RightFoot) { return TrackedDeviceRole.ROLE_RIGHTFOOT; }

			return TrackedDeviceRole.ROLE_UNDEFINED;
		}
		public static WVR_BodyTrackingDeviceRole ToRdp(this TrackerRole role)
		{
			// Use Tracker Waist as Hip role.
			if (role == TrackerRole.Waist) { return WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_Hip; }
			if (role == TrackerRole.Chest) { return WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_Chest; }
			// Tracker has no Head role.

			if (role == TrackerRole.Elbow_Left) { return WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_LeftElbow; }
			if (role == TrackerRole.Wrist_Left) { return WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_LeftWrist; }
			if (role == TrackerRole.Hand_Left) { return WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_LeftHand; }
			// Tracker has no Left Handheld role.

			if (role == TrackerRole.Elbow_Right) { return WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_RightElbow; }
			if (role == TrackerRole.Wrist_Right) { return WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_RightWrist; }
			if (role == TrackerRole.Hand_Right) { return WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_RightHand; }
			// Tracker has no Right Handheld role.

			if (role == TrackerRole.Knee_Left) { return WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_LeftKnee; }
			if (role == TrackerRole.Ankle_Left) { return WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_LeftAnkle; }
			if (role == TrackerRole.Foot_Left) { return WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_LeftFoot; }

			if (role == TrackerRole.Knee_Right) { return WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_RightKnee; }
			if (role == TrackerRole.Ankle_Right) { return WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_RightAnkle; }
			if (role == TrackerRole.Foot_Right) { return WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_RightFoot; }

			return WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_Invalid;
		}
		public static WVR_BodyTrackingDeviceRole ToRdp(this TrackedDeviceRole role)
		{
			// Use Tracker Waist as Hip role.
			if (role == TrackedDeviceRole.ROLE_HIP) { return WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_Hip; }
			if (role == TrackedDeviceRole.ROLE_CHEST) { return WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_Chest; }
			if (role == TrackedDeviceRole.ROLE_HEAD) { return WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_Head; }

			if (role == TrackedDeviceRole.ROLE_LEFTELBOW) { return WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_LeftElbow; }
			if (role == TrackedDeviceRole.ROLE_LEFTWRIST) { return WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_LeftWrist; }
			if (role == TrackedDeviceRole.ROLE_LEFTHAND) { return WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_LeftHand; }
			if (role == TrackedDeviceRole.ROLE_LEFTHANDHELD) { return WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_LeftHandheld; }

			if (role == TrackedDeviceRole.ROLE_RIGHTELBOW) { return WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_RightElbow; }
			if (role == TrackedDeviceRole.ROLE_RIGHTWRIST) { return WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_RightWrist; }
			if (role == TrackedDeviceRole.ROLE_RIGHTHAND) { return WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_RightHand; }
			if (role == TrackedDeviceRole.ROLE_RIGHTHANDHELD) { return WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_RightHandheld; }

			if (role == TrackedDeviceRole.ROLE_LEFTKNEE) { return WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_LeftKnee; }
			if (role == TrackedDeviceRole.ROLE_LEFTANKLE) { return WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_LeftAnkle; }
			if (role == TrackedDeviceRole.ROLE_LEFTFOOT) { return WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_LeftFoot; }

			if (role == TrackedDeviceRole.ROLE_RIGHTKNEE) { return WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_RightKnee; }
			if (role == TrackedDeviceRole.ROLE_RIGHTANKLE) { return WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_RightAnkle; }
			if (role == TrackedDeviceRole.ROLE_RIGHTFOOT) { return WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_RightFoot; }

			return WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_Invalid;
		}
		public static bool Contains(this WVR_BodyTrackingCalibrationMode wvrMode, BodyTrackingMode mode)
		{
			if (wvrMode == WVR_BodyTrackingCalibrationMode.WVR_BodyTrackingCalibrationMode_UpperIKAndLegFK)
			{
				if (mode == BodyTrackingMode.ARMIK || mode == BodyTrackingMode.UPPERBODYIK || mode == BodyTrackingMode.UPPERIKANDLEGFK)
					return true;
			}
			if (wvrMode == WVR_BodyTrackingCalibrationMode.WVR_BodyTrackingCalibrationMode_FullBodyIK)
			{
				if (mode == BodyTrackingMode.ARMIK || mode == BodyTrackingMode.UPPERBODYIK || mode == BodyTrackingMode.FULLBODYIK)
					return true;
			}
			if (wvrMode == WVR_BodyTrackingCalibrationMode.WVR_BodyTrackingCalibrationMode_UpperBodyIK)
			{
				if (mode == BodyTrackingMode.ARMIK || mode == BodyTrackingMode.UPPERBODYIK)
					return true;
			}
			if (wvrMode == WVR_BodyTrackingCalibrationMode.WVR_BodyTrackingCalibrationMode_ArmIK)
			{
				if (mode == BodyTrackingMode.ARMIK)
					return true;
			}

			return false;
		}
#endif

#if WAVE_BODY_IK
		public class BodyTrackingIK
		{
			public bool useDefaultTracking = false;
			public WVR_BodyCreateInfo_t customAvatar = WVR_BodyCreateInfo_t.identity;
			public WVR_BodyTracker ikTracker = 0;
			public WVR_BodyJointData_t ikJoint = WVR_BodyJointData_t.identity;
			public WVR_BodyProperties_t ikInfo = WVR_BodyProperties_t.identity;

			public BodyTrackingIK(bool in_default)
			{
				useDefaultTracking = in_default;

				customAvatar = WVR_BodyCreateInfo_t.identity;
				ikTracker = 0;
				ikJoint = WVR_BodyJointData_t.identity;
				ikInfo = WVR_BodyProperties_t.identity;
			}
		}

		public static WVR_Result CreateBodyTracker([In] ref WVR_BodyCreateInfo_t info, ref WVR_BodyTracker bodyTracker)
		{
			return Interop.WVR_CreateBodyTracker(ref info, ref bodyTracker);
		}
		public static WVR_Result DestroyBodyTracker(WVR_BodyTracker bodyTracker)
		{
			return Interop.WVR_DestroyBodyTracker(bodyTracker);
		}
		public static WVR_Result GetBodyJointData(WVR_BodyTracker bodyTracker, TrackingOrigin originModel, ref WVR_BodyJointData_t data)
		{
			return Interop.WVR_GetBodyJointData(bodyTracker, originModel, ref data);
		}
		public static WVR_Result GetBodyProperties(WVR_BodyTracker bodyTracker, ref WVR_BodyProperties_t properties)
		{
			return Interop.WVR_GetBodyProperties(bodyTracker, ref properties);
		}

		public static void Update(ref WVR_BodyLocationPose_t pose, Transform trans)
		{
			if (trans == null) { return; }
			pose.locationFlags = 3;
			GetVector3fFromUnity(trans.position, out pose.position);
			GetQuatfFromUnity(trans.rotation, out pose.orientation);
		}
		public static void Update(ref WVR_AvatarData_t avatar, Body body)
		{
			avatar.originType = GetOriginFromRdp();

			if (body == null) { return; }

			avatar.height = body.height;

			Update(ref avatar.joints[(UInt32)WVR_BodyJoint.WVR_BodyJoint_Hip], body.root);

			Update(ref avatar.joints[(UInt32)WVR_BodyJoint.WVR_BodyJoint_Left_Thigh], body.leftThigh);
			Update(ref avatar.joints[(UInt32)WVR_BodyJoint.WVR_BodyJoint_Left_Leg], body.leftLeg);
			Update(ref avatar.joints[(UInt32)WVR_BodyJoint.WVR_BodyJoint_Left_Ankle], body.leftAnkle);
			Update(ref avatar.joints[(UInt32)WVR_BodyJoint.WVR_BodyJoint_Left_Foot], body.leftFoot);

			Update(ref avatar.joints[(UInt32)WVR_BodyJoint.WVR_BodyJoint_Right_Thigh], body.rightThigh);
			Update(ref avatar.joints[(UInt32)WVR_BodyJoint.WVR_BodyJoint_Right_Leg], body.rightLeg);
			Update(ref avatar.joints[(UInt32)WVR_BodyJoint.WVR_BodyJoint_Right_Ankle], body.rightAnkle);
			Update(ref avatar.joints[(UInt32)WVR_BodyJoint.WVR_BodyJoint_Right_Foot], body.rightFoot);

			Update(ref avatar.joints[(UInt32)WVR_BodyJoint.WVR_BodyJoint_Waist], body.waist);
			Update(ref avatar.joints[(UInt32)WVR_BodyJoint.WVR_BodyJoint_Spine_Lower], body.spineLower);
			Update(ref avatar.joints[(UInt32)WVR_BodyJoint.WVR_BodyJoint_Spine_Middle], body.spineMiddle);
			Update(ref avatar.joints[(UInt32)WVR_BodyJoint.WVR_BodyJoint_Spine_High], body.spineHigh);

			Update(ref avatar.joints[(UInt32)WVR_BodyJoint.WVR_BodyJoint_Chest], body.chest);
			Update(ref avatar.joints[(UInt32)WVR_BodyJoint.WVR_BodyJoint_Head], body.head);
			Update(ref avatar.joints[(UInt32)WVR_BodyJoint.WVR_BodyJoint_Neck], body.neck);

			Update(ref avatar.joints[(UInt32)WVR_BodyJoint.WVR_BodyJoint_Left_Clavicle], body.leftClavicle);
			Update(ref avatar.joints[(UInt32)WVR_BodyJoint.WVR_BodyJoint_Left_Scapula], body.leftScapula);
			Update(ref avatar.joints[(UInt32)WVR_BodyJoint.WVR_BodyJoint_Left_Upper_Arm], body.leftUpperarm);
			Update(ref avatar.joints[(UInt32)WVR_BodyJoint.WVR_BodyJoint_Left_Forearm], body.leftForearm);
			Update(ref avatar.joints[(UInt32)WVR_BodyJoint.WVR_BodyJoint_Left_Hand], body.leftHand);

			Update(ref avatar.joints[(UInt32)WVR_BodyJoint.WVR_BodyJoint_Right_Clavicle], body.rightClavicle);
			Update(ref avatar.joints[(UInt32)WVR_BodyJoint.WVR_BodyJoint_Right_Scapula], body.rightScapula);
			Update(ref avatar.joints[(UInt32)WVR_BodyJoint.WVR_BodyJoint_Right_Upper_Arm], body.rightUpperarm);
			Update(ref avatar.joints[(UInt32)WVR_BodyJoint.WVR_BodyJoint_Right_Forearm], body.rightForearm);
			Update(ref avatar.joints[(UInt32)WVR_BodyJoint.WVR_BodyJoint_Right_Hand], body.rightHand);
		}
#endif
	}

	// Defines the OpenGL coordinate extrinsics and Unity coordinate extrinsics (with T).
	public class wvr
	{
		#region head
		public static readonly WVR_Pose_t extHead = new WVR_Pose_t(
			new WVR_Vector3f_t(0.0f, -0.08f, 0.1f),
			new WVR_Quatf_t(0, 0, 0, -1)
		);
		public static readonly ExtrinsicVector4_t extHeadT = new ExtrinsicVector4_t(
			new Vector3(0, -0.08f, -0.1f),
			new Vector4(0, 0, 0, 1)
		);
		#endregion

		#region Wrist - Self Tracker
		public static readonly WVR_Pose_t extSelfTracker_Wrist_Left = new WVR_Pose_t(
			new WVR_Vector3f_t(0.0f, -0.035f, -0.043f),
			new WVR_Quatf_t(0.0f, 0.707f, 0.0f, -0.707f)
		);
		public static readonly ExtrinsicVector4_t extSelfTracker_Wrist_LeftT = new ExtrinsicVector4_t(
			new Vector3(0.0f, -0.035f, 0.043f),
			new Vector4(0.0f, 0.707f, 0.0f, 0.707f)
		);

		public static readonly WVR_Pose_t extSelfTracker_Wrist_Right = new WVR_Pose_t(
			new WVR_Vector3f_t(0.0f, -0.035f, -0.043f),
			new WVR_Quatf_t(0.0f, -0.707f, 0.0f, -0.707f)
		);
		public static readonly ExtrinsicVector4_t extSelfTracker_Wrist_RightT = new ExtrinsicVector4_t(
			new Vector3(0.0f, -0.035f, 0.043f),
			new Vector4(0.0f, -0.707f, 0.0f, 0.707f)
		);
		#endregion

		#region Wrist - Wrist Tracker
		public static readonly WVR_Pose_t extWristTracker_Wrist_Left = new WVR_Pose_t(
					new WVR_Vector3f_t(-0.03f, 0.005f, -0.056f),
					new WVR_Quatf_t(0, 0, 0, -1)
				);
		public static readonly WVR_Pose_t extWristTracker_Wrist_Right = new WVR_Pose_t(
					new WVR_Vector3f_t(0.03f, 0.005f, -0.056f),
					new WVR_Quatf_t(0, 0, 0, -1)
				);
		#endregion

		#region Handheld
		public static readonly WVR_Pose_t extController_Handheld_Left = new WVR_Pose_t(
#if UNITY_EDITOR
			new WVR_Vector3f_t(-0.0197f, -0.0085f, 0.1345f),
			new WVR_Quatf_t(-0.214565f, 0.510735f, -0.601313f, -0.57579f)
#else
			new WVR_Vector3f_t(-0.03f, -0.035f, 0.13f),
			new WVR_Quatf_t(-0.345273f, 0.639022f, -0.462686f, -0.508290f)
#endif
		);
		public static readonly ExtrinsicVector4_t extController_Handheld_LeftT = new ExtrinsicVector4_t(
#if UNITY_EDITOR
			new Vector3(-0.0197f, -0.0085f, -0.1345f),
			new Vector4(-0.214565f, 0.510735f, 0.601313f, 0.57579f)
#else
			new Vector3(-0.03f, -0.035f, -0.13f),
			new Vector4(-0.345273f, 0.639022f, 0.462686f, 0.508290f)
#endif
		);

		public static readonly WVR_Pose_t extController_Handheld_Right = new WVR_Pose_t(
#if UNITY_EDITOR
			new WVR_Vector3f_t(0.0197f, -0.0085f, 0.1345f),
			new WVR_Quatf_t(-0.214565f, -0.510735f, 0.601313f, -0.57579f)
#else
			new WVR_Vector3f_t(0.03f, -0.035f, 0.13f),
			new WVR_Quatf_t(-0.345273f, -0.639022f, 0.462686f, -0.508290f)
#endif
		);
		public static readonly ExtrinsicVector4_t extController_Handheld_RightT = new ExtrinsicVector4_t(
#if UNITY_EDITOR
			new Vector3(0.0197f, -0.0085f, -0.1345f),
			new Vector4(-0.214565f, -0.510735f, -0.601313f, 0.57579f)
#else
			new Vector3(0.03f, -0.035f, -0.13f),
			new Vector4(-0.345273f, -0.639022f, -0.462686f, 0.508290f)
#endif
		);
		#endregion

		#region Hand
		public static readonly WVR_Pose_t extHand_Hand_Left = new WVR_Pose_t(
			new WVR_Vector3f_t(0, 0, 0),
			new WVR_Quatf_t(0.094802f, 0.641923f, 0.071626f, -0.757508f)
		);
		public static readonly ExtrinsicVector4_t extHand_Hand_LeftT = new ExtrinsicVector4_t(
			Vector3.zero,
			new Vector4(0.094802f, 0.641923f, -0.071626f, 0.757508f)
		);

		public static readonly WVR_Pose_t extHand_Hand_Right = new WVR_Pose_t(
			new WVR_Vector3f_t(0, 0, 0),
			new WVR_Quatf_t(0.094802f, -0.641923f, 0.071626f, -0.757508f)
		);
		public static readonly ExtrinsicVector4_t extHand_Hand_RightT = new ExtrinsicVector4_t(
			Vector3.zero,
			new Vector4(0.094802f, -0.641923f, -0.071626f, 0.757508f)
		);
		#endregion

		#region Hip
		public static readonly WVR_Pose_t extSelfTracker_Hip = new WVR_Pose_t(
					new WVR_Vector3f_t(0, 0, 0),
					new WVR_Quatf_t(0, 0, 0, -1)
				);
		public static readonly ExtrinsicVector4_t extSelfTracker_HipT = ExtrinsicVector4_t.identity;
		#endregion

		#region Knee - SelfTracker IM
		public static readonly WVR_Pose_t extSelfTrackerIM_Knee_Left = new WVR_Pose_t(
					new WVR_Vector3f_t(0, 0, 0),
					new WVR_Quatf_t(0, 0, 0, -1)
				);
		public static readonly ExtrinsicVector4_t extSelfTrackerIM_Knee_LeftT = ExtrinsicVector4_t.identity;

		public static readonly WVR_Pose_t extSelfTrackerIM_Knee_Right = new WVR_Pose_t(
					new WVR_Vector3f_t(0, 0, 0),
					new WVR_Quatf_t(0, 0, 0, -1)
				);
		public static readonly ExtrinsicVector4_t extSelfTrackerIM_Knee_RightT = ExtrinsicVector4_t.identity;
		#endregion

		#region Ankle - SelfTracker
		public static readonly WVR_Pose_t extSelfTracker_Ankle_Left = new WVR_Pose_t(
			new WVR_Vector3f_t(0.0f, -0.05f, 0.0f),
			new WVR_Quatf_t(-0.5f, 0.5f, -0.5f, 0.5f)
		);
		public static readonly ExtrinsicVector4_t extSelfTracker_Ankle_LeftT = new ExtrinsicVector4_t(
			new Vector3(0.0f, -0.05f, 0.0f),
			new Vector4(-0.5f, 0.5f, 0.5f, -0.5f)
		);

		public static readonly WVR_Pose_t extSelfTracker_Ankle_Right = new WVR_Pose_t(
			new WVR_Vector3f_t(0.0f, -0.05f, 0.0f),
			new WVR_Quatf_t(0.5f, 0.5f, -0.5f, -0.5f)
		);
		public static readonly ExtrinsicVector4_t extSelfTracker_Ankle_RightT = new ExtrinsicVector4_t(
			new Vector3(0.0f, -0.05f, 0.0f),
			new Vector4(0.5f, 0.5f, 0.5f, 0.5f)
		);
		#endregion

		#region Ankle - SelfTracker IM
		public static readonly WVR_Pose_t extSelfTrackerIM_Ankle_Left = new WVR_Pose_t(
			new WVR_Vector3f_t(0, 0, 0),
			new WVR_Quatf_t(0, 0, 0, -1)
		);
		public static readonly ExtrinsicVector4_t extSelfTrackerIM_Ankle_LeftT = ExtrinsicVector4_t.identity;

		public static readonly WVR_Pose_t extSelfTrackerIM_Ankle_Right = new WVR_Pose_t(
			new WVR_Vector3f_t(0, 0, 0),
			new WVR_Quatf_t(0, 0, 0, -1)
		);
		public static readonly ExtrinsicVector4_t extSelfTrackerIM_Ankle_RightT = ExtrinsicVector4_t.identity;
		#endregion

		#region Foot - SelfTracker
		public static readonly WVR_Pose_t extSelfTracker_Foot_Left = new WVR_Pose_t(
			new WVR_Vector3f_t(0, 0, 0.13f),
			new WVR_Quatf_t(0, 0, 0, -1)
		);
		public static readonly ExtrinsicVector4_t extSelfTracker_Foot_LeftT = new ExtrinsicVector4_t(
			new Vector3(0, 0, -0.13f),
			new Vector4(0, 0, 0, 1)
		);

		public static readonly WVR_Pose_t extSelfTracker_Foot_Right = new WVR_Pose_t(
			new WVR_Vector3f_t(0, 0, 0.13f),
			new WVR_Quatf_t(0, 0, 0, -1)
		);
		public static readonly ExtrinsicVector4_t extSelfTracker_Foot_RightT = new ExtrinsicVector4_t(
			new Vector3(0, 0, -0.13f),
			new Vector4(0, 0, 0, 1)
		);
		#endregion
	}

	[DisallowMultipleComponent]
	[RequireComponent(typeof(BodyManager))]
	public sealed class Srdp : MonoBehaviour
	{
		const string LOG_TAG = "VIVE.OpenXR.Toolkits.BodyTracking.RuntimeDependency.Srdp";
		static StringBuilder m_sb = null;
		static StringBuilder sb
		{
			get
			{
				if (m_sb == null) { m_sb = new StringBuilder(); }
				return m_sb;
			}
		}
		static void DEBUG(StringBuilder msg) { Rdp.d(LOG_TAG, msg, true); }

		private static Srdp m_Instance = null;
		public static Srdp Instance => m_Instance;

		[SerializeField]
		private InputActionProperty m_LeftControllerPose;
		public InputActionProperty leftControllerPose => m_LeftControllerPose;
		[SerializeField]
		private InputActionProperty m_RightControllerPose;
		public InputActionProperty rightControllerPose => m_RightControllerPose;

		[SerializeField]
		private List<InputActionProperty> m_TrackersInputAction = new List<InputActionProperty>();
		public IReadOnlyList<InputActionProperty> trackersInputAction => m_TrackersInputAction;

		private void Awake()
		{
			if (m_Instance == null)
			{
				m_Instance = this;
			}
			else if (m_Instance != this)
			{
				Destroy(this);
			}
		}

#if USE_INPUT_SYSTEM_POSE_CONTROL
		public bool GetPose(InputActionProperty actionReference, out UnityEngine.InputSystem.XR.PoseState pose)
		{
			pose = default;
			if (actionReference != null && actionReference.action != null)
			{
				try
				{
					pose = actionReference.action.ReadValue<UnityEngine.InputSystem.XR.PoseState>();
					return true;
				}
				catch (InvalidOperationException)
				{
					return false;
				}
			}
			return false;
		}
#else
		public bool GetPose(InputActionProperty actionReference, out UnityEngine.XR.OpenXR.Input.Pose pose)
		{
			pose = default;
			if (actionReference != null && actionReference.action != null)
			{
				try
				{
					pose = actionReference.action.ReadValue<UnityEngine.XR.OpenXR.Input.Pose>();
					return true;
				}
				catch (InvalidOperationException)
				{
					return false;
				}
			}
			return false;
		}
#endif
	}
}
