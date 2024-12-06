// Copyright HTC Corporation All Rights Reserved.

#define CoordinateOpenGL

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using VIVE.OpenXR.Toolkits.BodyTracking.AvatarCoordinate;
using VIVE.OpenXR.Toolkits.BodyTracking.RuntimeDependency;

#if WAVE_BODY_CALIBRATION || WAVE_BODY_IK
using Wave.Native;
#endif

namespace VIVE.OpenXR.Toolkits.BodyTracking
{
	public enum BodyTrackingResult : Byte
	{
		SUCCESS = 0,
		ERROR_IK_NOT_UPDATED = 1,
		ERROR_INVALID_ARGUMENT = 2,
		ERROR_IK_NOT_DESTROYED = 3,

		ERROR_BODYTRACKINGMODE_NOT_FOUND = 100,
		ERROR_TRACKER_AMOUNT_FAILED = 101,
		ERROR_SKELETONID_NOT_FOUND = 102,
		ERROR_INPUTPOSE_NOT_VALID = 103,
		ERROR_NOT_CALIBRATED = 104,
		ERROR_BODYTRACKINGMODE_NOT_ALIGNED = 105,
		ERROR_AVATAR_INIT_FAILED = 106,
		ERROR_CALIBRATE_FAILED = 107,
		ERROR_COMPUTE_FAILED = 108,
		ERROR_TABLE_STATIC = 109,
		ERROR_SOLVER_NOT_FOUND = 110,
		ERROR_NOT_INITIALIZATION = 111,
		ERROR_JOINT_NOT_FOUND = 112,

		ERROR_FATAL_ERROR = 255,
	}
	public enum DeviceExtRole : UInt64
	{
		Unknown = 0,

		Arm_Wrist = (UInt64)(1 << (Int32)TrackedDeviceRole.ROLE_HEAD
			| 1 << (Int32)TrackedDeviceRole.ROLE_LEFTWRIST | 1 << (Int32)TrackedDeviceRole.ROLE_RIGHTWRIST),
		UpperBody_Wrist = (UInt64)(Arm_Wrist | 1 << (Int32)TrackedDeviceRole.ROLE_HIP),
		FullBody_Wrist_Ankle = (UInt64)(UpperBody_Wrist | 1 << (Int32)TrackedDeviceRole.ROLE_LEFTANKLE | 1 << (Int32)TrackedDeviceRole.ROLE_RIGHTANKLE),
		FullBody_Wrist_Foot = (UInt64)(UpperBody_Wrist | 1 << (Int32)TrackedDeviceRole.ROLE_LEFTFOOT | 1 << (Int32)TrackedDeviceRole.ROLE_RIGHTFOOT),

		Arm_Handheld_Hand = (UInt64)(1 << (Int32)TrackedDeviceRole.ROLE_HEAD
			| 1 << (Int32)TrackedDeviceRole.ROLE_LEFTHAND | 1 << (Int32)TrackedDeviceRole.ROLE_RIGHTHAND
			| 1 << (Int32)TrackedDeviceRole.ROLE_LEFTHANDHELD | 1 << (Int32)TrackedDeviceRole.ROLE_RIGHTHANDHELD),
		UpperBody_Handheld_Hand = (UInt64)(Arm_Handheld_Hand | 1 << (Int32)TrackedDeviceRole.ROLE_HIP),
		FullBody_Handheld_Hand_Ankle = (UInt64)(UpperBody_Handheld_Hand | 1 << (Int32)TrackedDeviceRole.ROLE_LEFTANKLE | 1 << (Int32)TrackedDeviceRole.ROLE_RIGHTANKLE),
		FullBody_Handheld_Hand_Foot = (UInt64)(UpperBody_Handheld_Hand | 1 << (Int32)TrackedDeviceRole.ROLE_LEFTFOOT | 1 << (Int32)TrackedDeviceRole.ROLE_RIGHTFOOT),

		UpperBody_Handheld_Hand_Knee_Ankle = (UInt64)(UpperBody_Handheld_Hand
			| 1 << (Int32)TrackedDeviceRole.ROLE_LEFTKNEE | 1 << (Int32)TrackedDeviceRole.ROLE_RIGHTKNEE
			| 1 << (Int32)TrackedDeviceRole.ROLE_LEFTANKLE | 1 << (Int32)TrackedDeviceRole.ROLE_RIGHTANKLE),

		// Total 9 Device Extrinsic Roles.
	}
	public enum BodyPoseRole : UInt64
	{
		Unknown = 0,

		// Using Tracker
		Arm_Wrist = (UInt64)(1 << (Int32)TrackedDeviceRole.ROLE_HEAD | 1 << (Int32)TrackedDeviceRole.ROLE_LEFTWRIST | 1 << (Int32)TrackedDeviceRole.ROLE_RIGHTWRIST),
		UpperBody_Wrist = (UInt64)(Arm_Wrist | 1 << (Int32)TrackedDeviceRole.ROLE_HIP),
		FullBody_Wrist_Ankle = (UInt64)(UpperBody_Wrist | 1 << (Int32)TrackedDeviceRole.ROLE_LEFTANKLE | 1 << (Int32)TrackedDeviceRole.ROLE_RIGHTANKLE),
		FullBody_Wrist_Foot = (UInt64)(UpperBody_Wrist | 1 << (Int32)TrackedDeviceRole.ROLE_LEFTFOOT | 1 << (Int32)TrackedDeviceRole.ROLE_RIGHTFOOT),

		// Using Controller
		Arm_Handheld = (UInt64)(1 << (Int32)TrackedDeviceRole.ROLE_HEAD | 1 << (Int32)TrackedDeviceRole.ROLE_LEFTHANDHELD | 1 << (Int32)TrackedDeviceRole.ROLE_RIGHTHANDHELD),
		UpperBody_Handheld = (UInt64)(Arm_Handheld | 1 << (Int32)TrackedDeviceRole.ROLE_HIP),
		FullBody_Handheld_Ankle = (UInt64)(UpperBody_Handheld | 1 << (Int32)TrackedDeviceRole.ROLE_LEFTANKLE | 1 << (Int32)TrackedDeviceRole.ROLE_RIGHTANKLE),
		FullBody_Handheld_Foot = (UInt64)(UpperBody_Handheld | 1 << (Int32)TrackedDeviceRole.ROLE_LEFTFOOT | 1 << (Int32)TrackedDeviceRole.ROLE_RIGHTFOOT),

		// Using Natural Hand
		Arm_Hand = (UInt64)(1 << (Int32)TrackedDeviceRole.ROLE_HEAD | 1 << (Int32)TrackedDeviceRole.ROLE_LEFTHAND | 1 << (Int32)TrackedDeviceRole.ROLE_RIGHTHAND),
		UpperBody_Hand = (UInt64)(Arm_Hand | 1 << (Int32)TrackedDeviceRole.ROLE_HIP),
		FullBody_Hand_Ankle = (UInt64)(UpperBody_Hand | 1 << (Int32)TrackedDeviceRole.ROLE_LEFTANKLE | 1 << (Int32)TrackedDeviceRole.ROLE_RIGHTANKLE),
		FullBody_Hand_Foot = (UInt64)(UpperBody_Hand | 1 << (Int32)TrackedDeviceRole.ROLE_LEFTFOOT | 1 << (Int32)TrackedDeviceRole.ROLE_RIGHTFOOT),

		// Head + Controller/Hand + Hip + Knee + Ankle
		UpperBody_Handheld_Knee_Ankle = (UInt64)(UpperBody_Handheld
			| 1 << ((Int32)TrackedDeviceRole.ROLE_LEFTKNEE) | 1 << ((Int32)TrackedDeviceRole.ROLE_RIGHTKNEE)
			| 1 << (Int32)TrackedDeviceRole.ROLE_LEFTANKLE | 1 << (Int32)TrackedDeviceRole.ROLE_RIGHTANKLE),
		UpperBody_Hand_Knee_Ankle = (UInt64)(UpperBody_Hand
			| 1 << ((Int32)TrackedDeviceRole.ROLE_LEFTKNEE) | 1 << ((Int32)TrackedDeviceRole.ROLE_RIGHTKNEE)
			| 1 << (Int32)TrackedDeviceRole.ROLE_LEFTANKLE | 1 << (Int32)TrackedDeviceRole.ROLE_RIGHTANKLE),

		// Total 14 Body Pose Roles.
	}
	public enum TrackedDeviceType : UInt32
	{
		Invalid = 0,
		HMD = 1,
		Controller = 2,
		Hand = 3,
		ViveWristTracker = 4,
		ViveSelfTracker = 5,
		ViveSelfTrackerIM = 6,
	}

	public struct TransformData
	{
		public Vector3 position;
		public Vector3 localPosition;
		public Quaternion rotation;
		public Quaternion localRotation;
		public Vector3 localScale;

		public TransformData(Vector3 in_pos, Vector3 in_localPos, Quaternion in_rot, Quaternion in_localRot, Vector3 in_scale)
		{
			position = in_pos;
			localPosition = in_localPos;

			rotation = in_rot;
			Rdp.Validate(ref rotation);
			localRotation = in_localRot;
			Rdp.Validate(ref localRotation);

			localScale = in_scale;
		}
		public TransformData(Transform trans)
		{
			position = trans.position;
			localPosition = trans.localPosition;

			rotation = trans.rotation;
			Rdp.Validate(ref rotation);
			localRotation = trans.localRotation;
			Rdp.Validate(ref localRotation);

			localScale = trans.localScale;
		}
		public static TransformData identity
		{
			get
			{
				return new TransformData(Vector3.zero, Vector3.zero, Quaternion.identity, Quaternion.identity, Vector3.zero);
			}
		}
		public void Update(Transform trans)
		{
			if (trans == null) { return; }
			position = trans.position;
			localPosition = trans.localPosition;
			rotation = trans.rotation;
			Rdp.Validate(ref rotation);
			localRotation = trans.localRotation;
			Rdp.Validate(ref localRotation);
			localScale = trans.localScale;
		}
		public void Update(ref Transform trans)
		{
			if (trans == null) { return; }
			trans.position = position;
			trans.localPosition = localPosition;
			trans.rotation = rotation;
			trans.localRotation = localRotation;
			trans.localScale = localScale;
		}
	}
	public class BodyAvatar
	{
		const string LOG_TAG = "VIVE.OpenXR.Toolkits.BodyTracking.BodyAvatar";
		StringBuilder m_sb = null;
		StringBuilder sb
		{
			get
			{
				if (m_sb == null) { m_sb = new StringBuilder(); }
				return m_sb;
			}
		}
		void DEBUG(StringBuilder msg) { Rdp.d(LOG_TAG, msg, true); }

		public float height = 0;

		public Joint hip = Joint.identity;

		public Joint leftThigh = Joint.identity;
		public Joint leftLeg = Joint.identity;
		public Joint leftAnkle = Joint.identity;
		public Joint leftFoot = Joint.identity;

		public Joint rightThigh = Joint.identity;
		public Joint rightLeg = Joint.identity;
		public Joint rightAnkle = Joint.identity;
		public Joint rightFoot = Joint.identity;

		public Joint waist = Joint.identity;

		public Joint spineLower = Joint.identity;
		public Joint spineMiddle = Joint.identity;
		public Joint spineHigh = Joint.identity;

		public Joint chest = Joint.identity;
		public Joint neck = Joint.identity;
		public Joint head = Joint.identity;

		public Joint leftClavicle = Joint.identity;
		public Joint leftScapula = Joint.identity;
		public Joint leftUpperarm = Joint.identity;
		public Joint leftForearm = Joint.identity;
		public Joint leftHand = Joint.identity;

		public Joint rightClavicle = Joint.identity;
		public Joint rightScapula = Joint.identity;
		public Joint rightUpperarm = Joint.identity;
		public Joint rightForearm = Joint.identity;
		public Joint rightHand = Joint.identity;

		public float scale = 1;
		public float confidence = 1;
		public const float kAvatarHeight = 1.5f;

		private Joint[] s_AvatarJoints = null;
		private void UpdateJoints()
		{
			if (s_AvatarJoints == null || s_AvatarJoints.Length <= 0) { return; }

			int jointCount = 0;
			s_AvatarJoints[jointCount++].Update(hip);

			s_AvatarJoints[jointCount++].Update(leftThigh);
			s_AvatarJoints[jointCount++].Update(leftLeg);
			s_AvatarJoints[jointCount++].Update(leftAnkle);
			s_AvatarJoints[jointCount++].Update(leftFoot);

			s_AvatarJoints[jointCount++].Update(rightThigh);
			s_AvatarJoints[jointCount++].Update(rightLeg);
			s_AvatarJoints[jointCount++].Update(rightAnkle);
			s_AvatarJoints[jointCount++].Update(rightFoot);

			s_AvatarJoints[jointCount++].Update(waist);

			s_AvatarJoints[jointCount++].Update(spineLower);
			s_AvatarJoints[jointCount++].Update(spineMiddle);
			s_AvatarJoints[jointCount++].Update(spineHigh);

			s_AvatarJoints[jointCount++].Update(chest);
			s_AvatarJoints[jointCount++].Update(neck);
			s_AvatarJoints[jointCount++].Update(head);

			s_AvatarJoints[jointCount++].Update(leftClavicle);
			s_AvatarJoints[jointCount++].Update(leftScapula);
			s_AvatarJoints[jointCount++].Update(leftUpperarm);
			s_AvatarJoints[jointCount++].Update(leftForearm);
			s_AvatarJoints[jointCount++].Update(leftHand);

			s_AvatarJoints[jointCount++].Update(rightClavicle);
			s_AvatarJoints[jointCount++].Update(rightScapula);
			s_AvatarJoints[jointCount++].Update(rightUpperarm);
			s_AvatarJoints[jointCount++].Update(rightForearm);
			s_AvatarJoints[jointCount++].Update(rightHand);
		}
		public BodyAvatar()
		{
			int jointCount = 0;

			height = 0;
			// Joint initialization
			{
				hip.jointType = JointType.HIP; jointCount++;

				leftThigh.jointType = JointType.LEFTTHIGH; jointCount++;
				leftLeg.jointType = JointType.LEFTLEG; jointCount++;
				leftAnkle.jointType = JointType.LEFTANKLE; jointCount++;
				leftFoot.jointType = JointType.LEFTFOOT; jointCount++; // 5

				rightThigh.jointType = JointType.RIGHTTHIGH; jointCount++;
				rightLeg.jointType = JointType.RIGHTLEG; jointCount++;
				rightAnkle.jointType = JointType.RIGHTANKLE; jointCount++;
				rightFoot.jointType = JointType.RIGHTFOOT; jointCount++;

				waist.jointType = JointType.WAIST; jointCount++; // 10

				spineLower.jointType = JointType.SPINELOWER; jointCount++;
				spineMiddle.jointType = JointType.SPINEMIDDLE; jointCount++;
				spineHigh.jointType = JointType.SPINEHIGH; jointCount++;

				chest.jointType = JointType.CHEST; jointCount++;
				neck.jointType = JointType.NECK; jointCount++; // 15
				head.jointType = JointType.HEAD; jointCount++;

				leftClavicle.jointType = JointType.LEFTCLAVICLE; jointCount++;
				leftScapula.jointType = JointType.LEFTSCAPULA; jointCount++;
				leftUpperarm.jointType = JointType.LEFTUPPERARM; jointCount++;
				leftForearm.jointType = JointType.LEFTFOREARM; jointCount++; // 20
				leftHand.jointType = JointType.LEFTHAND; jointCount++;

				rightClavicle.jointType = JointType.RIGHTCLAVICLE; jointCount++;
				rightScapula.jointType = JointType.RIGHTSCAPULA; jointCount++;
				rightUpperarm.jointType = JointType.RIGHTUPPERARM; jointCount++;
				rightForearm.jointType = JointType.RIGHTFOREARM; jointCount++; // 25
				rightHand.jointType = JointType.RIGHTHAND; jointCount++;
			}
			scale = 1;
			confidence = 1;

			s_AvatarJoints = new Joint[jointCount];
		}
		public void Update(BodyAvatar in_avatar)
		{
			if (in_avatar == null) { return; }

			height = in_avatar.height;

			head.Update(in_avatar.head);
			neck.Update(in_avatar.neck);
			chest.Update(in_avatar.chest);
			waist.Update(in_avatar.waist);
			hip.Update(in_avatar.hip);

			leftClavicle.Update(in_avatar.leftClavicle);
			leftScapula.Update(in_avatar.leftScapula);
			leftUpperarm.Update(in_avatar.leftUpperarm);
			leftForearm.Update(in_avatar.leftForearm);
			leftHand.Update(in_avatar.leftHand);

			leftThigh.Update(in_avatar.leftThigh);
			leftLeg.Update(in_avatar.leftLeg);
			leftAnkle.Update(in_avatar.leftAnkle);
			leftFoot.Update(in_avatar.leftFoot);

			rightClavicle.Update(in_avatar.rightClavicle);
			rightScapula.Update(in_avatar.rightScapula);
			rightUpperarm.Update(in_avatar.rightUpperarm);
			rightForearm.Update(in_avatar.rightForearm);
			rightHand.Update(in_avatar.rightHand);

			rightThigh.Update(in_avatar.rightThigh);
			rightLeg.Update(in_avatar.rightLeg);
			rightAnkle.Update(in_avatar.rightAnkle);
			rightFoot.Update(in_avatar.rightFoot);

			scale = in_avatar.scale;
			confidence = in_avatar.confidence;
		}
		public void Update(Joint joint)
		{
			if (joint.jointType == JointType.HIP) { hip.Update(joint); }

			if (joint.jointType == JointType.LEFTTHIGH) { leftThigh.Update(joint); }
			if (joint.jointType == JointType.LEFTLEG) { leftLeg.Update(joint); }
			if (joint.jointType == JointType.LEFTANKLE) { leftAnkle.Update(joint); }
			if (joint.jointType == JointType.LEFTFOOT) { leftFoot.Update(joint); } // 5

			if (joint.jointType == JointType.RIGHTTHIGH) { rightThigh.Update(joint); }
			if (joint.jointType == JointType.RIGHTLEG) { rightLeg.Update(joint); }
			if (joint.jointType == JointType.RIGHTANKLE) { rightAnkle.Update(joint); }
			if (joint.jointType == JointType.RIGHTFOOT) { rightFoot.Update(joint); }

			if (joint.jointType == JointType.WAIST) { waist.Update(joint); } // 10

			if (joint.jointType == JointType.SPINELOWER) { spineLower.Update(joint); }
			if (joint.jointType == JointType.SPINEMIDDLE) { spineMiddle.Update(joint); }
			if (joint.jointType == JointType.SPINEHIGH) { spineHigh.Update(joint); }

			if (joint.jointType == JointType.CHEST) { chest.Update(joint); }
			if (joint.jointType == JointType.NECK) { neck.Update(joint); } // 15
			if (joint.jointType == JointType.HEAD) { head.Update(joint); }

			if (joint.jointType == JointType.LEFTCLAVICLE) { leftClavicle.Update(joint); }
			if (joint.jointType == JointType.LEFTSCAPULA) { leftScapula.Update(joint); }
			if (joint.jointType == JointType.LEFTUPPERARM) { leftUpperarm.Update(joint); }
			if (joint.jointType == JointType.LEFTFOREARM) { leftForearm.Update(joint); } // 20
			if (joint.jointType == JointType.LEFTHAND) { leftHand.Update(joint); }

			if (joint.jointType == JointType.RIGHTCLAVICLE) { rightClavicle.Update(joint); }
			if (joint.jointType == JointType.RIGHTSCAPULA) { rightScapula.Update(joint); }
			if (joint.jointType == JointType.RIGHTUPPERARM) { rightUpperarm.Update(joint); }
			if (joint.jointType == JointType.RIGHTFOREARM) { rightForearm.Update(joint); } // 25
			if (joint.jointType == JointType.RIGHTHAND) { rightHand.Update(joint); }
		}
		public void Update(JointType jointType, PoseState poseState, Vector3 translation, Vector3 velocity, Vector3 angularVelocity, Quaternion rotation, bool init = false)
		{
			string func = "Update() ";
			if (jointType == JointType.HEAD)
			{
				head.Update(poseState, translation, velocity, angularVelocity, rotation);
				if (init)
				{
					sb.Clear().Append(func).Append("head poseState: ").Append(head.poseState)
						.Append(", position (").Append(head.translation.x.ToString("N3")).Append(", ").Append(head.translation.y.ToString("N3")).Append(", ").Append(head.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(head.rotation.x.ToString("N3")).Append(", ").Append(head.rotation.y.ToString("N3")).Append(", ").Append(head.rotation.z.ToString("N3")).Append(", ").Append(head.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
			}
			if (jointType == JointType.NECK)
			{
				neck.Update(poseState, translation, velocity, angularVelocity, rotation);
				if (init)
				{
					sb.Clear().Append(func).Append("neck poseState: ").Append(neck.poseState)
						.Append(", position (").Append(neck.translation.x.ToString("N3")).Append(", ").Append(neck.translation.y.ToString("N3")).Append(", ").Append(neck.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(neck.rotation.x.ToString("N3")).Append(", ").Append(neck.rotation.y.ToString("N3")).Append(", ").Append(neck.rotation.z.ToString("N3")).Append(", ").Append(neck.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
			}
			if (jointType == JointType.CHEST)
			{
				chest.Update(poseState, translation, velocity, angularVelocity, rotation);
				if (init)
				{
					sb.Clear().Append(func).Append("chest poseState: ").Append(chest.poseState)
						.Append(", position (").Append(chest.translation.x.ToString("N3")).Append(", ").Append(chest.translation.y.ToString("N3")).Append(", ").Append(chest.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(chest.rotation.x.ToString("N3")).Append(", ").Append(chest.rotation.y.ToString("N3")).Append(", ").Append(chest.rotation.z.ToString("N3")).Append(", ").Append(chest.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
			}
			if (jointType == JointType.WAIST)
			{
				waist.Update(poseState, translation, velocity, angularVelocity, rotation);
				if (init)
				{
					sb.Clear().Append(func).Append("waist poseState: ").Append(waist.poseState)
						.Append(", position (").Append(waist.translation.x.ToString("N3")).Append(", ").Append(waist.translation.y.ToString("N3")).Append(", ").Append(waist.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(waist.rotation.x.ToString("N3")).Append(", ").Append(waist.rotation.y.ToString("N3")).Append(", ").Append(waist.rotation.z.ToString("N3")).Append(", ").Append(waist.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
			}
			if (jointType == JointType.HIP)
			{
				hip.Update(poseState, translation, velocity, angularVelocity, rotation);
				if (init)
				{
					sb.Clear().Append(func).Append("hip poseState: ").Append(hip.poseState)
						.Append(", position (").Append(hip.translation.x.ToString("N3")).Append(", ").Append(hip.translation.y.ToString("N3")).Append(", ").Append(hip.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(hip.rotation.x.ToString("N3")).Append(", ").Append(hip.rotation.y.ToString("N3")).Append(", ").Append(hip.rotation.z.ToString("N3")).Append(", ").Append(hip.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
			}

			if (jointType == JointType.LEFTCLAVICLE)
			{
				leftClavicle.Update(poseState, translation, velocity, angularVelocity, rotation);
				if (init)
				{
					sb.Clear().Append(func).Append("leftClavicle poseState: ").Append(leftClavicle.poseState)
						.Append(", position (").Append(leftClavicle.translation.x.ToString("N3")).Append(", ").Append(leftClavicle.translation.y.ToString("N3")).Append(", ").Append(leftClavicle.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(leftClavicle.rotation.x.ToString("N3")).Append(", ").Append(leftClavicle.rotation.y.ToString("N3")).Append(", ").Append(leftClavicle.rotation.z.ToString("N3")).Append(", ").Append(leftClavicle.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
			}
			if (jointType == JointType.LEFTSCAPULA)
			{
				leftScapula.Update(poseState, translation, velocity, angularVelocity, rotation);
				if (init)
				{
					sb.Clear().Append(func).Append("leftScapula poseState: ").Append(leftScapula.poseState)
						.Append(", position (").Append(leftScapula.translation.x.ToString("N3")).Append(", ").Append(leftScapula.translation.y.ToString("N3")).Append(", ").Append(leftScapula.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(leftScapula.rotation.x.ToString("N3")).Append(", ").Append(leftScapula.rotation.y.ToString("N3")).Append(", ").Append(leftScapula.rotation.z.ToString("N3")).Append(", ").Append(leftScapula.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
			}
			if (jointType == JointType.LEFTUPPERARM)
			{
				leftUpperarm.Update(poseState, translation, velocity, angularVelocity, rotation);
				if (init)
				{
					sb.Clear().Append(func).Append("leftUpperarm poseState: ").Append(leftUpperarm.poseState)
						.Append(", position (").Append(leftUpperarm.translation.x.ToString("N3")).Append(", ").Append(leftUpperarm.translation.y.ToString("N3")).Append(", ").Append(leftUpperarm.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(leftUpperarm.rotation.x.ToString("N3")).Append(", ").Append(leftUpperarm.rotation.y.ToString("N3")).Append(", ").Append(leftUpperarm.rotation.z.ToString("N3")).Append(", ").Append(leftUpperarm.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
			}
			if (jointType == JointType.LEFTFOREARM)
			{
				leftForearm.Update(poseState, translation, velocity, angularVelocity, rotation);
				if (init)
				{
					sb.Clear().Append(func).Append("leftForearm poseState: ").Append(leftForearm.poseState)
						.Append(", position (").Append(leftForearm.translation.x.ToString("N3")).Append(", ").Append(leftForearm.translation.y.ToString("N3")).Append(", ").Append(leftForearm.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(leftForearm.rotation.x.ToString("N3")).Append(", ").Append(leftForearm.rotation.y.ToString("N3")).Append(", ").Append(leftForearm.rotation.z.ToString("N3")).Append(", ").Append(leftForearm.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
			}
			if (jointType == JointType.LEFTHAND)
			{
				leftHand.Update(poseState, translation, velocity, angularVelocity, rotation);
				if (init)
				{
					sb.Clear().Append(func).Append("leftHand poseState: ").Append(leftHand.poseState)
						.Append(", position (").Append(leftHand.translation.x.ToString("N3")).Append(", ").Append(leftHand.translation.y.ToString("N3")).Append(", ").Append(leftHand.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(leftHand.rotation.x.ToString("N3")).Append(", ").Append(leftHand.rotation.y.ToString("N3")).Append(", ").Append(leftHand.rotation.z.ToString("N3")).Append(", ").Append(leftHand.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
			}

			if (jointType == JointType.LEFTTHIGH)
			{
				leftThigh.Update(poseState, translation, velocity, angularVelocity, rotation);
				if (init)
				{
					sb.Clear().Append(func).Append("leftThigh poseState: ").Append(leftThigh.poseState)
						.Append(", position (").Append(leftThigh.translation.x.ToString("N3")).Append(", ").Append(leftThigh.translation.y.ToString("N3")).Append(", ").Append(leftThigh.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(leftThigh.rotation.x.ToString("N3")).Append(", ").Append(leftThigh.rotation.y.ToString("N3")).Append(", ").Append(leftThigh.rotation.z.ToString("N3")).Append(", ").Append(leftThigh.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
			}
			if (jointType == JointType.LEFTLEG)
			{
				leftLeg.Update(poseState, translation, velocity, angularVelocity, rotation);
				if (init)
				{
					sb.Clear().Append(func).Append("leftLeg poseState: ").Append(leftLeg.poseState)
						.Append(", position (").Append(leftLeg.translation.x.ToString("N3")).Append(", ").Append(leftLeg.translation.y.ToString("N3")).Append(", ").Append(leftLeg.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(leftLeg.rotation.x.ToString("N3")).Append(", ").Append(leftLeg.rotation.y.ToString("N3")).Append(", ").Append(leftLeg.rotation.z.ToString("N3")).Append(", ").Append(leftLeg.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
			}
			if (jointType == JointType.LEFTANKLE)
			{
				leftAnkle.Update(poseState, translation, velocity, angularVelocity, rotation);
				if (init)
				{
					sb.Clear().Append(func).Append("leftAnkle poseState: ").Append(leftAnkle.poseState)
						.Append(", position (").Append(leftAnkle.translation.x.ToString("N3")).Append(", ").Append(leftAnkle.translation.y.ToString("N3")).Append(", ").Append(leftAnkle.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(leftAnkle.rotation.x.ToString("N3")).Append(", ").Append(leftAnkle.rotation.y.ToString("N3")).Append(", ").Append(leftAnkle.rotation.z.ToString("N3")).Append(", ").Append(leftAnkle.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
			}
			if (jointType == JointType.LEFTFOOT)
			{
				leftFoot.Update(poseState, translation, velocity, angularVelocity, rotation);
				if (init)
				{
					sb.Clear().Append(func).Append("leftFoot poseState: ").Append(leftFoot.poseState)
						.Append(", position (").Append(leftFoot.translation.x.ToString("N3")).Append(", ").Append(leftFoot.translation.y.ToString("N3")).Append(", ").Append(leftFoot.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(leftFoot.rotation.x.ToString("N3")).Append(", ").Append(leftFoot.rotation.y.ToString("N3")).Append(", ").Append(leftFoot.rotation.z.ToString("N3")).Append(", ").Append(leftFoot.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
			}

			if (jointType == JointType.RIGHTCLAVICLE)
			{
				rightClavicle.Update(poseState, translation, velocity, angularVelocity, rotation);
				if (init)
				{
					sb.Clear().Append(func).Append("rightClavicle poseState: ").Append(rightClavicle.poseState)
						.Append(", position (").Append(rightClavicle.translation.x.ToString("N3")).Append(", ").Append(rightClavicle.translation.y.ToString("N3")).Append(", ").Append(rightClavicle.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(rightClavicle.rotation.x.ToString("N3")).Append(", ").Append(rightClavicle.rotation.y.ToString("N3")).Append(", ").Append(rightClavicle.rotation.z.ToString("N3")).Append(", ").Append(rightClavicle.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
			}
			if (jointType == JointType.RIGHTSCAPULA)
			{
				rightScapula.Update(poseState, translation, velocity, angularVelocity, rotation);
				if (init)
				{
					sb.Clear().Append(func).Append("rightScapula poseState: ").Append(rightScapula.poseState)
						.Append(", position (").Append(rightScapula.translation.x.ToString("N3")).Append(", ").Append(rightScapula.translation.y.ToString("N3")).Append(", ").Append(rightScapula.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(rightScapula.rotation.x.ToString("N3")).Append(", ").Append(rightScapula.rotation.y.ToString("N3")).Append(", ").Append(rightScapula.rotation.z.ToString("N3")).Append(", ").Append(rightScapula.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
			}
			if (jointType == JointType.RIGHTUPPERARM)
			{
				rightUpperarm.Update(poseState, translation, velocity, angularVelocity, rotation);
				if (init)
				{
					sb.Clear().Append(func).Append("rightUpperarm poseState: ").Append(rightUpperarm.poseState)
						.Append(", position (").Append(rightUpperarm.translation.x.ToString("N3")).Append(", ").Append(rightUpperarm.translation.y.ToString("N3")).Append(", ").Append(rightUpperarm.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(rightUpperarm.rotation.x.ToString("N3")).Append(", ").Append(rightUpperarm.rotation.y.ToString("N3")).Append(", ").Append(rightUpperarm.rotation.z.ToString("N3")).Append(", ").Append(rightUpperarm.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
			}
			if (jointType == JointType.RIGHTFOREARM)
			{
				rightForearm.Update(poseState, translation, velocity, angularVelocity, rotation);
				if (init)
				{
					sb.Clear().Append(func).Append("rightForearm poseState: ").Append(rightForearm.poseState)
						.Append(", position (").Append(rightForearm.translation.x.ToString("N3")).Append(", ").Append(rightForearm.translation.y.ToString("N3")).Append(", ").Append(rightForearm.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(rightForearm.rotation.x.ToString("N3")).Append(", ").Append(rightForearm.rotation.y.ToString("N3")).Append(", ").Append(rightForearm.rotation.z.ToString("N3")).Append(", ").Append(rightForearm.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
			}
			if (jointType == JointType.RIGHTHAND)
			{
				rightHand.Update(poseState, translation, velocity, angularVelocity, rotation);
				if (init)
				{
					sb.Clear().Append(func).Append("rightHand poseState: ").Append(rightHand.poseState)
						.Append(", position (").Append(rightHand.translation.x.ToString("N3")).Append(", ").Append(rightHand.translation.y.ToString("N3")).Append(", ").Append(rightHand.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(rightHand.rotation.x.ToString("N3")).Append(", ").Append(rightHand.rotation.y.ToString("N3")).Append(", ").Append(rightHand.rotation.z.ToString("N3")).Append(", ").Append(rightHand.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
			}

			if (jointType == JointType.RIGHTTHIGH)
			{
				rightThigh.Update(poseState, translation, velocity, angularVelocity, rotation);
				if (init)
				{
					sb.Clear().Append(func).Append("rightThigh poseState: ").Append(rightThigh.poseState)
						.Append(", position (").Append(rightThigh.translation.x.ToString("N3")).Append(", ").Append(rightThigh.translation.y.ToString("N3")).Append(", ").Append(rightThigh.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(rightThigh.rotation.x.ToString("N3")).Append(", ").Append(rightThigh.rotation.y.ToString("N3")).Append(", ").Append(rightThigh.rotation.z.ToString("N3")).Append(", ").Append(rightThigh.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
			}
			if (jointType == JointType.RIGHTLEG)
			{
				rightLeg.Update(poseState, translation, velocity, angularVelocity, rotation);
				if (init)
				{
					sb.Clear().Append(func).Append("rightLeg poseState: ").Append(rightLeg.poseState)
						.Append(", position (").Append(rightLeg.translation.x.ToString("N3")).Append(", ").Append(rightLeg.translation.y.ToString("N3")).Append(", ").Append(rightLeg.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(rightLeg.rotation.x.ToString("N3")).Append(", ").Append(rightLeg.rotation.y.ToString("N3")).Append(", ").Append(rightLeg.rotation.z.ToString("N3")).Append(", ").Append(rightLeg.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
			}
			if (jointType == JointType.RIGHTANKLE)
			{
				rightAnkle.Update(poseState, translation, velocity, angularVelocity, rotation);
				if (init)
				{
					sb.Clear().Append(func).Append("rightAnkle poseState: ").Append(rightAnkle.poseState)
						.Append(", position (").Append(rightAnkle.translation.x.ToString("N3")).Append(", ").Append(rightAnkle.translation.y.ToString("N3")).Append(", ").Append(rightAnkle.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(rightAnkle.rotation.x.ToString("N3")).Append(", ").Append(rightAnkle.rotation.y.ToString("N3")).Append(", ").Append(rightAnkle.rotation.z.ToString("N3")).Append(", ").Append(rightAnkle.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
			}
			if (jointType == JointType.RIGHTFOOT)
			{
				rightFoot.Update(poseState, translation, velocity, angularVelocity, rotation);
				if (init)
				{
					sb.Clear().Append(func).Append("rightFoot poseState: ").Append(rightFoot.poseState)
						.Append(", position (").Append(rightFoot.translation.x.ToString("N3")).Append(", ").Append(rightFoot.translation.y.ToString("N3")).Append(", ").Append(rightFoot.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(rightFoot.rotation.x.ToString("N3")).Append(", ").Append(rightFoot.rotation.y.ToString("N3")).Append(", ").Append(rightFoot.rotation.z.ToString("N3")).Append(", ").Append(rightFoot.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
			}

			if (jointType == JointType.SPINELOWER)
			{
				spineLower.Update(poseState, translation, velocity, angularVelocity, rotation);
				if (init)
				{
					sb.Clear().Append(func).Append("spineLower poseState: ").Append(spineLower.poseState)
						.Append(", position (").Append(spineLower.translation.x.ToString("N3")).Append(", ").Append(spineLower.translation.y.ToString("N3")).Append(", ").Append(spineLower.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(spineLower.rotation.x.ToString("N3")).Append(", ").Append(spineLower.rotation.y.ToString("N3")).Append(", ").Append(spineLower.rotation.z.ToString("N3")).Append(", ").Append(spineLower.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
			}
			if (jointType == JointType.SPINEMIDDLE)
			{
				spineMiddle.Update(poseState, translation, velocity, angularVelocity, rotation);
				if (init)
				{
					sb.Clear().Append(func).Append("spineMiddle poseState: ").Append(spineMiddle.poseState)
						.Append(", position (").Append(spineMiddle.translation.x.ToString("N3")).Append(", ").Append(spineMiddle.translation.y.ToString("N3")).Append(", ").Append(spineMiddle.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(spineMiddle.rotation.x.ToString("N3")).Append(", ").Append(spineMiddle.rotation.y.ToString("N3")).Append(", ").Append(spineMiddle.rotation.z.ToString("N3")).Append(", ").Append(spineMiddle.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
			}
			if (jointType == JointType.SPINEHIGH)
			{
				spineHigh.Update(poseState, translation, velocity, angularVelocity, rotation);
				if (init)
				{
					sb.Clear().Append(func).Append("spineHigh poseState: ").Append(spineHigh.poseState)
						.Append(", position (").Append(spineHigh.translation.x.ToString("N3")).Append(", ").Append(spineHigh.translation.y.ToString("N3")).Append(", ").Append(spineHigh.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(spineHigh.rotation.x.ToString("N3")).Append(", ").Append(spineHigh.rotation.y.ToString("N3")).Append(", ").Append(spineHigh.rotation.z.ToString("N3")).Append(", ").Append(spineHigh.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
			}
		}

		private void Update([In] Transform trans, [In] Vector3 velocity, [In] Vector3 angularVelocity, ref Joint joint)
		{
			if (trans == null) { return; }
			joint.poseState = (PoseState.ROTATION | PoseState.TRANSLATION);
			joint.translation = trans.position;
			joint.velocity = velocity;
			joint.rotation = trans.rotation;
			Rdp.Validate(ref joint.rotation);
			joint.angularVelocity = angularVelocity;
		}
		public void Update(JointType jointType, Transform trans, Vector3 velocity, Vector3 angularVelocity)
		{
			if (trans == null) { return; }
			if (jointType == JointType.HEAD) { Update(trans, velocity, angularVelocity, ref head); }
			if (jointType == JointType.NECK) { Update(trans, velocity, angularVelocity, ref neck); }
			if (jointType == JointType.CHEST) { Update(trans, velocity, angularVelocity, ref chest); }
			if (jointType == JointType.WAIST) { Update(trans, velocity, angularVelocity, ref waist); }
			if (jointType == JointType.HIP) { Update(trans, velocity, angularVelocity, ref hip); }

			if (jointType == JointType.LEFTCLAVICLE) { Update(trans, velocity, angularVelocity, ref leftClavicle); }
			if (jointType == JointType.LEFTSCAPULA) { Update(trans, velocity, angularVelocity, ref leftScapula); }
			if (jointType == JointType.LEFTUPPERARM) { Update(trans, velocity, angularVelocity, ref leftUpperarm); }
			if (jointType == JointType.LEFTFOREARM) { Update(trans, velocity, angularVelocity, ref leftForearm); }
			if (jointType == JointType.LEFTHAND) { Update(trans, velocity, angularVelocity, ref leftHand); }

			if (jointType == JointType.LEFTTHIGH) { Update(trans, velocity, angularVelocity, ref leftThigh); }
			if (jointType == JointType.LEFTLEG) { Update(trans, velocity, angularVelocity, ref leftLeg); }
			if (jointType == JointType.LEFTANKLE) { Update(trans, velocity, angularVelocity, ref leftAnkle); }
			if (jointType == JointType.LEFTFOOT) { Update(trans, velocity, angularVelocity, ref leftFoot); }

			if (jointType == JointType.RIGHTCLAVICLE) { Update(trans, velocity, angularVelocity, ref rightClavicle); }
			if (jointType == JointType.RIGHTSCAPULA) { Update(trans, velocity, angularVelocity, ref rightScapula); }
			if (jointType == JointType.RIGHTUPPERARM) { Update(trans, velocity, angularVelocity, ref rightUpperarm); }
			if (jointType == JointType.RIGHTFOREARM) { Update(trans, velocity, angularVelocity, ref rightForearm); }
			if (jointType == JointType.RIGHTHAND) { Update(trans, velocity, angularVelocity, ref rightHand); }

			if (jointType == JointType.RIGHTTHIGH) { Update(trans, velocity, angularVelocity, ref rightThigh); }
			if (jointType == JointType.RIGHTLEG) { Update(trans, velocity, angularVelocity, ref rightLeg); }
			if (jointType == JointType.RIGHTANKLE) { Update(trans, velocity, angularVelocity, ref rightAnkle); }
			if (jointType == JointType.RIGHTFOOT) { Update(trans, velocity, angularVelocity, ref rightFoot); }

			if (jointType == JointType.SPINELOWER) { Update(trans, velocity, angularVelocity, ref spineLower); }
			if (jointType == JointType.SPINEMIDDLE) { Update(trans, velocity, angularVelocity, ref spineMiddle); }
			if (jointType == JointType.SPINEHIGH) { Update(trans, velocity, angularVelocity, ref spineHigh); }
		}
		private void Update([In] Transform trans, [In] Vector3 velocity, ref Joint joint)
		{
			if (trans == null) { return; }
			joint.poseState = (PoseState.ROTATION | PoseState.TRANSLATION);
			joint.translation = trans.transform.position;
			joint.velocity = velocity;
			joint.rotation = trans.transform.rotation;
			Rdp.Validate(ref joint.rotation);
		}
		public void Update(JointType jointType, Transform trans, Vector3 velocity)
		{
			if (trans == null) { return; }
			if (jointType == JointType.HEAD) { Update(trans, velocity, ref head); }
			if (jointType == JointType.NECK) { Update(trans, velocity, ref neck); }
			if (jointType == JointType.CHEST) { Update(trans, velocity, ref chest); }
			if (jointType == JointType.WAIST) { Update(trans, velocity, ref waist); }
			if (jointType == JointType.HIP) { Update(trans, velocity, ref hip); }

			if (jointType == JointType.LEFTCLAVICLE) { Update(trans, velocity, ref leftClavicle); }
			if (jointType == JointType.LEFTSCAPULA) { Update(trans, velocity, ref leftScapula); }
			if (jointType == JointType.LEFTUPPERARM) { Update(trans, velocity, ref leftUpperarm); }
			if (jointType == JointType.LEFTFOREARM) { Update(trans, velocity, ref leftForearm); }
			if (jointType == JointType.LEFTHAND) { Update(trans, velocity, ref leftHand); }

			if (jointType == JointType.LEFTTHIGH) { Update(trans, velocity, ref leftThigh); }
			if (jointType == JointType.LEFTLEG) { Update(trans, velocity, ref leftLeg); }
			if (jointType == JointType.LEFTANKLE) { Update(trans, velocity, ref leftAnkle); }
			if (jointType == JointType.LEFTFOOT) { Update(trans, velocity, ref leftFoot); }

			if (jointType == JointType.RIGHTCLAVICLE) { Update(trans, velocity, ref rightClavicle); }
			if (jointType == JointType.RIGHTSCAPULA) { Update(trans, velocity, ref rightScapula); }
			if (jointType == JointType.RIGHTUPPERARM) { Update(trans, velocity, ref rightUpperarm); }
			if (jointType == JointType.RIGHTFOREARM) { Update(trans, velocity, ref rightForearm); }
			if (jointType == JointType.RIGHTHAND) { Update(trans, velocity, ref rightHand); }

			if (jointType == JointType.RIGHTTHIGH) { Update(trans, velocity, ref rightThigh); }
			if (jointType == JointType.RIGHTLEG) { Update(trans, velocity, ref rightLeg); }
			if (jointType == JointType.RIGHTANKLE) { Update(trans, velocity, ref rightAnkle); }
			if (jointType == JointType.RIGHTFOOT) { Update(trans, velocity, ref rightFoot); }

			if (jointType == JointType.SPINELOWER) { Update(trans, velocity, ref spineLower); }
			if (jointType == JointType.SPINEMIDDLE) { Update(trans, velocity, ref spineMiddle); }
			if (jointType == JointType.SPINEHIGH) { Update(trans, velocity, ref spineHigh); }
		}
		private void Update([In] Transform trans, ref Joint joint)
		{
			if (trans == null) { return; }
			joint.translation = trans.position;
			joint.rotation = trans.rotation;
			Rdp.Validate(ref joint.rotation);
			joint.poseState = (PoseState.ROTATION | PoseState.TRANSLATION);
			//sb.Clear().Append("Update() ").Append(joint.Rdp()); DEBUG(sb);
		}
		public void Update(JointType jointType, Transform trans)
		{
			if (trans == null) { return; }
			if (jointType == JointType.HEAD) { Update(trans, ref head); }
			if (jointType == JointType.NECK) { Update(trans, ref neck); }
			if (jointType == JointType.CHEST) { Update(trans, ref chest); }
			if (jointType == JointType.WAIST) { Update(trans, ref waist); }
			if (jointType == JointType.HIP) { Update(trans, ref hip); }

			if (jointType == JointType.LEFTCLAVICLE) { Update(trans, ref leftClavicle); }
			if (jointType == JointType.LEFTSCAPULA) { Update(trans, ref leftScapula); }
			if (jointType == JointType.LEFTUPPERARM) { Update(trans, ref leftUpperarm); }
			if (jointType == JointType.LEFTFOREARM) { Update(trans, ref leftForearm); }
			if (jointType == JointType.LEFTHAND) { Update(trans, ref leftHand); }

			if (jointType == JointType.LEFTTHIGH) { Update(trans, ref leftThigh); }
			if (jointType == JointType.LEFTLEG) { Update(trans, ref leftLeg); }
			if (jointType == JointType.LEFTANKLE) { Update(trans, ref leftAnkle); }
			if (jointType == JointType.LEFTFOOT) { Update(trans, ref leftFoot); }

			if (jointType == JointType.RIGHTCLAVICLE) { Update(trans, ref rightClavicle); }
			if (jointType == JointType.RIGHTSCAPULA) { Update(trans, ref rightScapula); }
			if (jointType == JointType.RIGHTUPPERARM) { Update(trans, ref rightUpperarm); }
			if (jointType == JointType.RIGHTFOREARM) { Update(trans, ref rightForearm); }
			if (jointType == JointType.RIGHTHAND) { Update(trans, ref rightHand); }

			if (jointType == JointType.RIGHTTHIGH) { Update(trans, ref rightThigh); }
			if (jointType == JointType.RIGHTLEG) { Update(trans, ref rightLeg); }
			if (jointType == JointType.RIGHTANKLE) { Update(trans, ref rightAnkle); }
			if (jointType == JointType.RIGHTFOOT) { Update(trans, ref rightFoot); }

			if (jointType == JointType.SPINELOWER) { Update(trans, ref spineLower); }
			if (jointType == JointType.SPINEMIDDLE) { Update(trans, ref spineMiddle); }
			if (jointType == JointType.SPINEHIGH) { Update(trans, ref spineHigh); }
		}
		private void Update([In] Joint joint, ref Transform trans, float scale = 1)
		{
			if (trans == null) { return; }
			if (joint.poseState.HasFlag(PoseState.TRANSLATION)) { trans.position = joint.translation * scale; }
			if (joint.poseState.HasFlag(PoseState.ROTATION))
			{
				Rdp.Validate(ref joint.rotation);
				trans.rotation = joint.rotation;
			}
		}
		public void Update([In] JointType jointType, ref Transform trans, float scale = 1)
		{
			if (trans == null) { return; }
			if (jointType == JointType.HEAD) { Update(head, ref trans, scale); }
			if (jointType == JointType.NECK) { Update(neck, ref trans, scale); }
			if (jointType == JointType.CHEST) { Update(chest, ref trans, scale); }
			if (jointType == JointType.WAIST) { Update(waist, ref trans, scale); }
			if (jointType == JointType.HIP) { Update(hip, ref trans, scale); }

			if (jointType == JointType.LEFTCLAVICLE) { Update(leftClavicle, ref trans, scale); }
			if (jointType == JointType.LEFTSCAPULA) { Update(leftScapula, ref trans, scale); }
			if (jointType == JointType.LEFTUPPERARM) { Update(leftUpperarm, ref trans, scale); }
			if (jointType == JointType.LEFTFOREARM) { Update(leftForearm, ref trans, scale); }
			if (jointType == JointType.LEFTHAND) { Update(leftHand, ref trans, scale); }

			if (jointType == JointType.LEFTTHIGH) { Update(leftThigh, ref trans, scale); }
			if (jointType == JointType.LEFTLEG) { Update(leftLeg, ref trans, scale); }
			if (jointType == JointType.LEFTANKLE) { Update(leftAnkle, ref trans, scale); }
			if (jointType == JointType.LEFTFOOT) { Update(leftFoot, ref trans, scale); }

			if (jointType == JointType.RIGHTCLAVICLE) { Update(rightClavicle, ref trans, scale); }
			if (jointType == JointType.RIGHTSCAPULA) { Update(rightScapula, ref trans, scale); }
			if (jointType == JointType.RIGHTUPPERARM) { Update(rightUpperarm, ref trans, scale); }
			if (jointType == JointType.RIGHTFOREARM) { Update(rightForearm, ref trans, scale); }
			if (jointType == JointType.RIGHTHAND) { Update(rightHand, ref trans, scale); }

			if (jointType == JointType.RIGHTTHIGH) { Update(rightThigh, ref trans, scale); }
			if (jointType == JointType.RIGHTLEG) { Update(rightLeg, ref trans, scale); }
			if (jointType == JointType.RIGHTANKLE) { Update(rightAnkle, ref trans, scale); }
			if (jointType == JointType.RIGHTFOOT) { Update(rightFoot, ref trans, scale); }

			if (jointType == JointType.SPINELOWER) { Update(spineLower, ref trans, scale); }
			if (jointType == JointType.SPINEMIDDLE) { Update(spineMiddle, ref trans, scale); }
			if (jointType == JointType.SPINEHIGH) { Update(spineHigh, ref trans, scale); }
		}

#if WAVE_BODY_IK
		public void Update(ref WVR_AvatarData_t avatar)
		{
			avatar.height = height;
			avatar.originType = Rdp.GetOriginFromRdp();

			hip.Update(ref avatar.joints[(UInt32)WVR_BodyJoint.WVR_BodyJoint_Hip]);

			leftThigh.Update(ref avatar.joints[(UInt32)WVR_BodyJoint.WVR_BodyJoint_Left_Thigh]);
			leftLeg.Update(ref avatar.joints[(UInt32)WVR_BodyJoint.WVR_BodyJoint_Left_Leg]);
			leftAnkle.Update(ref avatar.joints[(UInt32)WVR_BodyJoint.WVR_BodyJoint_Left_Ankle]);
			leftFoot.Update(ref avatar.joints[(UInt32)WVR_BodyJoint.WVR_BodyJoint_Left_Foot]);

			rightThigh.Update(ref avatar.joints[(UInt32)WVR_BodyJoint.WVR_BodyJoint_Right_Thigh]);
			rightLeg.Update(ref avatar.joints[(UInt32)WVR_BodyJoint.WVR_BodyJoint_Right_Leg]);
			rightAnkle.Update(ref avatar.joints[(UInt32)WVR_BodyJoint.WVR_BodyJoint_Right_Ankle]);
			rightFoot.Update(ref avatar.joints[(UInt32)WVR_BodyJoint.WVR_BodyJoint_Right_Foot]);

			waist.Update(ref avatar.joints[(UInt32)WVR_BodyJoint.WVR_BodyJoint_Waist]);
			spineLower.Update(ref avatar.joints[(UInt32)WVR_BodyJoint.WVR_BodyJoint_Spine_Lower]);
			spineMiddle.Update(ref avatar.joints[(UInt32)WVR_BodyJoint.WVR_BodyJoint_Spine_Middle]);
			spineHigh.Update(ref avatar.joints[(UInt32)WVR_BodyJoint.WVR_BodyJoint_Spine_High]);

			chest.Update(ref avatar.joints[(UInt32)WVR_BodyJoint.WVR_BodyJoint_Chest]);
			head.Update(ref avatar.joints[(UInt32)WVR_BodyJoint.WVR_BodyJoint_Head]);
			neck.Update(ref avatar.joints[(UInt32)WVR_BodyJoint.WVR_BodyJoint_Neck]);

			leftClavicle.Update(ref avatar.joints[(UInt32)WVR_BodyJoint.WVR_BodyJoint_Left_Clavicle]);
			leftScapula.Update(ref avatar.joints[(UInt32)WVR_BodyJoint.WVR_BodyJoint_Left_Scapula]);
			leftUpperarm.Update(ref avatar.joints[(UInt32)WVR_BodyJoint.WVR_BodyJoint_Left_Upper_Arm]);
			leftForearm.Update(ref avatar.joints[(UInt32)WVR_BodyJoint.WVR_BodyJoint_Left_Forearm]);
			leftHand.Update(ref avatar.joints[(UInt32)WVR_BodyJoint.WVR_BodyJoint_Left_Hand]);

			rightClavicle.Update(ref avatar.joints[(UInt32)WVR_BodyJoint.WVR_BodyJoint_Right_Clavicle]);
			rightScapula.Update(ref avatar.joints[(UInt32)WVR_BodyJoint.WVR_BodyJoint_Right_Scapula]);
			rightUpperarm.Update(ref avatar.joints[(UInt32)WVR_BodyJoint.WVR_BodyJoint_Right_Upper_Arm]);
			rightForearm.Update(ref avatar.joints[(UInt32)WVR_BodyJoint.WVR_BodyJoint_Right_Forearm]);
			rightHand.Update(ref avatar.joints[(UInt32)WVR_BodyJoint.WVR_BodyJoint_Right_Hand]);
		}
#endif
		public void Update([In] Body body)
		{
			if (body == null) { return; }

			Update(JointType.HIP, body.root); // 0

			Update(JointType.LEFTTHIGH, body.leftThigh);
			Update(JointType.LEFTLEG, body.leftLeg);
			Update(JointType.LEFTANKLE, body.leftAnkle);
			Update(JointType.LEFTFOOT, body.leftFoot);

			Update(JointType.RIGHTTHIGH, body.rightThigh); // 5
			Update(JointType.RIGHTLEG, body.rightLeg);
			Update(JointType.RIGHTANKLE, body.rightAnkle);
			Update(JointType.RIGHTFOOT, body.rightFoot);

			Update(JointType.WAIST, body.waist);

			Update(JointType.SPINELOWER, body.spineLower); // 10
			Update(JointType.SPINEMIDDLE, body.spineMiddle);
			Update(JointType.SPINEHIGH, body.spineHigh);

			Update(JointType.CHEST, body.chest);
			Update(JointType.NECK, body.neck);
			Update(JointType.HEAD, body.head); // 15

			Update(JointType.LEFTCLAVICLE, body.leftClavicle);
			Update(JointType.LEFTSCAPULA, body.leftScapula);
			Update(JointType.LEFTUPPERARM, body.leftUpperarm);
			Update(JointType.LEFTFOREARM, body.leftForearm);
			Update(JointType.LEFTHAND, body.leftHand); // 20

			Update(JointType.RIGHTCLAVICLE, body.rightClavicle);
			Update(JointType.RIGHTSCAPULA, body.rightScapula);
			Update(JointType.RIGHTUPPERARM, body.rightUpperarm);
			Update(JointType.RIGHTFOREARM, body.rightForearm);
			Update(JointType.RIGHTHAND, body.rightHand);

			height = body.height;
		}
		/// <summary>
		/// Update full body poses. Note that your avatar should have joints in specified order.
		/// E.g. You avatar's toe should be the child of foot and the foot should be the child of leg.
		/// </summary>
		/// <param name="body">Reference to the avatar body.</param>
		public void Update(ref Body body)
		{
			if (body == null) { return; }

			body.height = height;

			if (body.root != null) Update(JointType.HIP, ref body.root); // 0

			if (body.leftThigh != null) Update(JointType.LEFTTHIGH, ref body.leftThigh);
			if (body.leftLeg != null) Update(JointType.LEFTLEG, ref body.leftLeg);
			if (body.leftAnkle != null) Update(JointType.LEFTANKLE, ref body.leftAnkle);
			if (body.leftFoot != null) Update(JointType.LEFTFOOT, ref body.leftFoot);

			if (body.rightThigh != null) Update(JointType.RIGHTTHIGH, ref body.rightThigh); // 5
			if (body.rightLeg != null) Update(JointType.RIGHTLEG, ref body.rightLeg);
			if (body.rightAnkle != null) Update(JointType.RIGHTANKLE, ref body.rightAnkle);
			if (body.rightFoot != null) Update(JointType.RIGHTFOOT, ref body.rightFoot);

			if (body.waist != null) Update(JointType.WAIST, ref body.waist);

			if (body.spineLower != null) Update(JointType.SPINELOWER, ref body.spineLower); // 10
			if (body.spineMiddle != null) Update(JointType.SPINEMIDDLE, ref body.spineMiddle);
			if (body.spineHigh != null) Update(JointType.SPINEHIGH, ref body.spineHigh);

			if (body.chest != null) Update(JointType.CHEST, ref body.chest);
			if (body.neck != null) Update(JointType.NECK, ref body.neck);
			if (body.head != null) Update(JointType.HEAD, ref body.head); // 15

			if (body.leftClavicle != null) Update(JointType.LEFTCLAVICLE, ref body.leftClavicle);
			if (body.leftScapula != null) Update(JointType.LEFTSCAPULA, ref body.leftScapula);
			if (body.leftUpperarm != null) Update(JointType.LEFTUPPERARM, ref body.leftUpperarm);
			if (body.leftForearm != null) Update(JointType.LEFTFOREARM, ref body.leftForearm);
			if (body.leftHand != null) Update(JointType.LEFTHAND, ref body.leftHand); // 20

			if (body.rightClavicle != null) Update(JointType.RIGHTCLAVICLE, ref body.rightClavicle);
			if (body.rightScapula != null) Update(JointType.RIGHTSCAPULA, ref body.rightScapula);
			if (body.rightUpperarm != null) Update(JointType.RIGHTUPPERARM, ref body.rightUpperarm);
			if (body.rightForearm != null) Update(JointType.RIGHTFOREARM, ref body.rightForearm);
			if (body.rightHand != null) Update(JointType.RIGHTHAND, ref body.rightHand); // 25
		}

		private List<Joint> joints = new List<Joint>();
		public bool GetJoints(out Joint[] avatarJoints, out UInt32 avatarJointCount, bool is6DoF = false)
		{
			if (!is6DoF) // including NODATA joints.
			{
				UpdateJoints();
				avatarJoints = s_AvatarJoints;
				avatarJointCount = (UInt32)(avatarJoints.Length & 0x7FFFFFFF);
				return true;
			}

			avatarJoints = null;
			avatarJointCount = 0;

			joints.Clear();
			if (hip.poseState == (PoseState.ROTATION | PoseState.TRANSLATION)) { joints.Add(hip); }

			if (leftThigh.poseState == (PoseState.ROTATION | PoseState.TRANSLATION)) { joints.Add(leftThigh); }
			if (leftLeg.poseState == (PoseState.ROTATION | PoseState.TRANSLATION)) { joints.Add(leftLeg); }
			if (leftAnkle.poseState == (PoseState.ROTATION | PoseState.TRANSLATION)) { joints.Add(leftAnkle); }
			if (leftFoot.poseState == (PoseState.ROTATION | PoseState.TRANSLATION)) { joints.Add(leftFoot); }

			if (rightThigh.poseState == (PoseState.ROTATION | PoseState.TRANSLATION)) { joints.Add(rightThigh); }
			if (rightLeg.poseState == (PoseState.ROTATION | PoseState.TRANSLATION)) { joints.Add(rightLeg); }
			if (rightAnkle.poseState == (PoseState.ROTATION | PoseState.TRANSLATION)) { joints.Add(rightAnkle); }
			if (rightFoot.poseState == (PoseState.ROTATION | PoseState.TRANSLATION)) { joints.Add(rightFoot); }

			if (waist.poseState == (PoseState.ROTATION | PoseState.TRANSLATION)) { joints.Add(waist); }

			if (spineLower.poseState == (PoseState.ROTATION | PoseState.TRANSLATION)) { joints.Add(spineLower); }
			if (spineMiddle.poseState == (PoseState.ROTATION | PoseState.TRANSLATION)) { joints.Add(spineMiddle); }
			if (spineHigh.poseState == (PoseState.ROTATION | PoseState.TRANSLATION)) { joints.Add(spineHigh); }

			if (chest.poseState == (PoseState.ROTATION | PoseState.TRANSLATION)) { joints.Add(chest); }
			if (neck.poseState == (PoseState.ROTATION | PoseState.TRANSLATION)) { joints.Add(neck); }
			if (head.poseState == (PoseState.ROTATION | PoseState.TRANSLATION)) { joints.Add(head); }

			if (leftClavicle.poseState == (PoseState.ROTATION | PoseState.TRANSLATION)) { joints.Add(leftClavicle); }
			if (leftScapula.poseState == (PoseState.ROTATION | PoseState.TRANSLATION)) { joints.Add(leftScapula); }
			if (leftUpperarm.poseState == (PoseState.ROTATION | PoseState.TRANSLATION)) { joints.Add(leftUpperarm); }
			if (leftForearm.poseState == (PoseState.ROTATION | PoseState.TRANSLATION)) { joints.Add(leftForearm); }
			if (leftHand.poseState == (PoseState.ROTATION | PoseState.TRANSLATION)) { joints.Add(leftHand); }

			if (rightClavicle.poseState == (PoseState.ROTATION | PoseState.TRANSLATION)) { joints.Add(rightClavicle); }
			if (rightScapula.poseState == (PoseState.ROTATION | PoseState.TRANSLATION)) { joints.Add(rightScapula); }
			if (rightUpperarm.poseState == (PoseState.ROTATION | PoseState.TRANSLATION)) { joints.Add(rightUpperarm); }
			if (rightForearm.poseState == (PoseState.ROTATION | PoseState.TRANSLATION)) { joints.Add(rightForearm); }
			if (rightHand.poseState == (PoseState.ROTATION | PoseState.TRANSLATION)) { joints.Add(rightHand); }

			if (joints.Count > 0)
			{
				avatarJoints = joints.ToArray();
				avatarJointCount = (UInt32)(joints.Count & 0x7FFFFFFF);
				return true;
			}

			return false;
		}
		public void Set6DoFJoints(Joint[] avatarJoints, UInt32 avatarJointCount)
		{
			for (UInt32 i = 0; i < avatarJointCount; i++)
			{
				Update(avatarJoints[i]);
			}
		}

		bool print = true;
		public void ChangeJointCoordinate(AvatarCoordinateProducer coordinate)
		{
			if (coordinate == null) { return; }

			if (print)
			{
				print = false;

				sb.Clear().Append("ChangeJointCoordinate()")
					.Append("\nhip (").Append(coordinate.hip.x).Append(", ").Append(coordinate.hip.y).Append(", ").Append(coordinate.hip.z).Append(")")
					.Append("\nleftThigh (").Append(coordinate.leftThigh.x).Append(", ").Append(coordinate.leftThigh.y).Append(", ").Append(coordinate.leftThigh.z).Append(")")
					.Append("\nleftLeg (").Append(coordinate.leftLeg.x).Append(", ").Append(coordinate.leftLeg.y).Append(", ").Append(coordinate.leftLeg.z).Append(")")
					.Append("\nleftAnkle (").Append(coordinate.leftAnkle.x).Append(", ").Append(coordinate.leftAnkle.y).Append(", ").Append(coordinate.leftAnkle.z).Append(")")
					.Append("\nleftFoot (").Append(coordinate.leftFoot.x).Append(", ").Append(coordinate.leftFoot.y).Append(", ").Append(coordinate.leftFoot.z).Append(")")
					.Append("\nrightThigh (").Append(coordinate.rightThigh.x).Append(", ").Append(coordinate.rightThigh.y).Append(", ").Append(coordinate.rightThigh.z).Append(")")
					.Append("\nrightLeg (").Append(coordinate.rightLeg.x).Append(", ").Append(coordinate.rightLeg.y).Append(", ").Append(coordinate.rightLeg.z).Append(")")
					.Append("\nrightAnkle (").Append(coordinate.rightAnkle.x).Append(", ").Append(coordinate.rightAnkle.y).Append(", ").Append(coordinate.rightAnkle.z).Append(")")
					.Append("\nrightFoot (").Append(coordinate.rightFoot.x).Append(", ").Append(coordinate.rightFoot.y).Append(", ").Append(coordinate.rightFoot.z).Append(")")
					.Append("\nwaist (").Append(coordinate.waist.x).Append(", ").Append(coordinate.waist.y).Append(", ").Append(coordinate.waist.z).Append(")")
					.Append("\nspineLower (").Append(coordinate.spineLower.x).Append(", ").Append(coordinate.spineLower.y).Append(", ").Append(coordinate.spineLower.z).Append(")")
					.Append("\nspineMiddle (").Append(coordinate.spineMiddle.x).Append(", ").Append(coordinate.spineMiddle.y).Append(", ").Append(coordinate.spineMiddle.z).Append(")")
					.Append("\nspineHigh (").Append(coordinate.spineHigh.x).Append(", ").Append(coordinate.spineHigh.y).Append(", ").Append(coordinate.spineHigh.z).Append(")")
					.Append("\nchest (").Append(coordinate.chest.x).Append(", ").Append(coordinate.chest.y).Append(", ").Append(coordinate.chest.z).Append(")")
					.Append("\nneck (").Append(coordinate.neck.x).Append(", ").Append(coordinate.neck.y).Append(", ").Append(coordinate.neck.z).Append(")")
					.Append("\nhead (").Append(coordinate.head.x).Append(", ").Append(coordinate.head.y).Append(", ").Append(coordinate.head.z).Append(")")
					.Append("\nleftClavicle (").Append(coordinate.leftClavicle.x).Append(", ").Append(coordinate.leftClavicle.y).Append(", ").Append(coordinate.leftClavicle.z).Append(")")
					.Append("\nleftScapula (").Append(coordinate.leftScapula.x).Append(", ").Append(coordinate.leftScapula.y).Append(", ").Append(coordinate.leftScapula.z).Append(")")
					.Append("\nleftUpperarm (").Append(coordinate.leftUpperarm.x).Append(", ").Append(coordinate.leftUpperarm.y).Append(", ").Append(coordinate.leftUpperarm.z).Append(")")
					.Append("\nleftForearm (").Append(coordinate.leftForearm.x).Append(", ").Append(coordinate.leftForearm.y).Append(", ").Append(coordinate.leftForearm.z).Append(")")
					.Append("\nleftHand (").Append(coordinate.leftHand.x).Append(", ").Append(coordinate.leftHand.y).Append(", ").Append(coordinate.leftHand.z).Append(")")
					.Append("\nrightClavicle (").Append(coordinate.rightClavicle.x).Append(", ").Append(coordinate.rightClavicle.y).Append(", ").Append(coordinate.rightClavicle.z).Append(")")
					.Append("\nrightScapula (").Append(coordinate.rightScapula.x).Append(", ").Append(coordinate.rightScapula.y).Append(", ").Append(coordinate.rightScapula.z).Append(")")
					.Append("\nrightUpperarm (").Append(coordinate.rightUpperarm.x).Append(", ").Append(coordinate.rightUpperarm.y).Append(", ").Append(coordinate.rightUpperarm.z).Append(")")
					.Append("\nrightForearm (").Append(coordinate.rightForearm.x).Append(", ").Append(coordinate.rightForearm.y).Append(", ").Append(coordinate.rightForearm.z).Append(")")
					.Append("\nrightHand (").Append(coordinate.rightHand.x).Append(", ").Append(coordinate.rightHand.y).Append(", ").Append(coordinate.rightHand.z).Append(")");
				DEBUG(sb);
			}

			BodyTrackingUtils.TransformCoordinate(ref hip, coordinate.hip);

			BodyTrackingUtils.TransformCoordinate(ref leftThigh, coordinate.leftThigh);
			BodyTrackingUtils.TransformCoordinate(ref leftLeg, coordinate.leftLeg);
			BodyTrackingUtils.TransformCoordinate(ref leftAnkle, coordinate.leftAnkle);
			BodyTrackingUtils.TransformCoordinate(ref leftFoot, coordinate.leftFoot);

			BodyTrackingUtils.TransformCoordinate(ref rightThigh, coordinate.rightThigh);
			BodyTrackingUtils.TransformCoordinate(ref rightLeg, coordinate.rightLeg);
			BodyTrackingUtils.TransformCoordinate(ref rightAnkle, coordinate.rightAnkle);
			BodyTrackingUtils.TransformCoordinate(ref rightFoot, coordinate.rightFoot);

			BodyTrackingUtils.TransformCoordinate(ref waist, coordinate.waist);

			BodyTrackingUtils.TransformCoordinate(ref spineLower, coordinate.spineLower);
			BodyTrackingUtils.TransformCoordinate(ref spineMiddle, coordinate.spineMiddle);
			BodyTrackingUtils.TransformCoordinate(ref spineHigh, coordinate.spineHigh);

			BodyTrackingUtils.TransformCoordinate(ref chest, coordinate.chest);
			BodyTrackingUtils.TransformCoordinate(ref neck, coordinate.neck);
			BodyTrackingUtils.TransformCoordinate(ref head, coordinate.head);

			BodyTrackingUtils.TransformCoordinate(ref leftClavicle, coordinate.leftClavicle);
			BodyTrackingUtils.TransformCoordinate(ref leftScapula, coordinate.leftScapula);
			BodyTrackingUtils.TransformCoordinate(ref leftUpperarm, coordinate.leftUpperarm);
			BodyTrackingUtils.TransformCoordinate(ref leftForearm, coordinate.leftForearm);
			BodyTrackingUtils.TransformCoordinate(ref leftHand, coordinate.leftHand);

			BodyTrackingUtils.TransformCoordinate(ref rightClavicle, coordinate.rightClavicle);
			BodyTrackingUtils.TransformCoordinate(ref rightScapula, coordinate.rightScapula);
			BodyTrackingUtils.TransformCoordinate(ref rightUpperarm, coordinate.rightUpperarm);
			BodyTrackingUtils.TransformCoordinate(ref rightForearm, coordinate.rightForearm);
			BodyTrackingUtils.TransformCoordinate(ref rightHand, coordinate.rightHand);
		}
	}

	[Serializable]
	public struct ExtrinsicVector4_t
	{
		public Vector3 translation;
		[SerializeField]
		private Vector4 m_rotation;
		public Vector4 rotation
		{
			get
			{
				Rdp.Validate(ref m_rotation);
				return m_rotation;
			}
			set { m_rotation = value; }
		}

		private Extrinsic ext;
		private void UpdateExtrinsic()
		{
			ext.translation = translation;
			Rdp.GetQuaternion(m_rotation, out ext.rotation);
		}
		public Extrinsic GetExtrinsic()
		{
			UpdateExtrinsic();
			return ext;
		}

		public ExtrinsicVector4_t(Vector3 in_tra, Vector4 in_rot)
		{
			translation = in_tra;
			m_rotation = in_rot;

			ext = Extrinsic.identity;
			UpdateExtrinsic();
		}
		public static ExtrinsicVector4_t identity
		{
			get
			{
				return new ExtrinsicVector4_t(Vector3.zero, new Vector4(0, 0, 0, 1));
			}
		}

		public void Update(ExtrinsicVector4_t in_ext)
		{
			translation = in_ext.translation;
			m_rotation = in_ext.rotation;
		}
		public void Update(Vector3 in_tra, Vector4 in_rot)
		{
			translation = in_tra;
			m_rotation = in_rot;

			ext = Extrinsic.identity;
			UpdateExtrinsic();
		}
		public void Update(Extrinsic in_ext)
		{
			translation = in_ext.translation;
			Rdp.GetVector4(in_ext.rotation, out m_rotation);
		}
		public void Update(Quaternion in_rot)
		{
			Rdp.GetVector4(in_rot, out m_rotation);
		}

		public void Reset()
		{
			translation.x = 0;
			translation.y = 0;
			translation.z = 0;
			m_rotation.x = 0;
			m_rotation.y = 0;
			m_rotation.z = 0;
			m_rotation.w = 1;
		}
#if WAVE_BODY_CALIBRATION
		public void Update(WVR_BodyTracking_Extrinsic_t in_ext)
		{
			Rdp.GetVector3FromGL(in_ext.position, out translation);
			Rdp.GetVector4FromGL(in_ext.rotation, out m_rotation);
		}
#endif
	}
	[Serializable]
	public struct ExtrinsicInfo_t
	{
		const string LOG_TAG = "VIVE.OpenXR.Toolkits.BodyTracking.ExtrinsicInfo_t";
		static StringBuilder m_sb = null;
		static StringBuilder sb
		{
			get
			{
				if (m_sb == null) { m_sb = new StringBuilder(); }
				return m_sb;
			}
		}
		void DEBUG(StringBuilder msg) { Rdp.d(LOG_TAG, msg, true); }

		public bool isTracking;
		public ExtrinsicVector4_t extrinsic;
		public ExtrinsicInfo_t(bool in_isTracking, ExtrinsicVector4_t in_extrinsic)
		{
			isTracking = in_isTracking;
			extrinsic = in_extrinsic;
		}
		public static ExtrinsicInfo_t identity
		{
			get
			{
				return new ExtrinsicInfo_t(false, ExtrinsicVector4_t.identity);
			}
		}
		public void Reset()
		{
			isTracking = false;
			extrinsic.Reset();
		}
		public void Update(ExtrinsicInfo_t in_info)
		{
			isTracking = in_info.isTracking;
			extrinsic.Update(in_info.extrinsic);
		}
		public void Update(ExtrinsicVector4_t in_ext)
		{
			isTracking = true;
			extrinsic.Update(in_ext);
		}
		public void Update(Extrinsic in_ext)
		{
			isTracking = true;
			extrinsic.Update(in_ext);
		}
		public void Update(TrackedDeviceExtrinsic in_ext)
		{
			isTracking = true;
			extrinsic.Update(in_ext.extrinsic);
		}
#if WAVE_BODY_CALIBRATION
		public void Update(WVR_BodyTracking_Extrinsic_t in_ext)
		{
			isTracking = true;
			extrinsic.Update(in_ext);
		}
#endif

		public void printLog(string prefix)
		{
			sb.Clear().Append(prefix)
				.Append(", position(").Append(extrinsic.translation.x).Append(", ").Append(extrinsic.translation.y).Append(", ").Append(extrinsic.translation.z).Append(")")
				.Append(", rotation(").Append(extrinsic.rotation.x).Append(", ").Append(extrinsic.rotation.y).Append(", ").Append(extrinsic.rotation.z).Append(", ").Append(extrinsic.rotation.w).Append(")");
			DEBUG(sb);
		}
	}

	public struct TrackedDeviceExtrinsicState_t
	{
		const string LOG_TAG = "VIVE.OpenXR.Toolkits.BodyTracking.TrackedDeviceExtrinsicState_t";
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

		public bool isTracking;
		public TrackedDeviceExtrinsic deviceExtrinsic;

		public TrackedDeviceExtrinsicState_t(bool in_isTracking, TrackedDeviceExtrinsic in_deviceExtrinsic)
		{
			isTracking = in_isTracking;
			deviceExtrinsic = in_deviceExtrinsic;
		}
		public static TrackedDeviceExtrinsicState_t identity
		{
			get
			{
				return new TrackedDeviceExtrinsicState_t(false, TrackedDeviceExtrinsic.identity);
			}
		}
		public static TrackedDeviceExtrinsicState_t init(TrackedDeviceRole role)
		{
			return new TrackedDeviceExtrinsicState_t(false, TrackedDeviceExtrinsic.init(role));
		}
		public void Update(TrackedDeviceExtrinsicState_t in_info)
		{
			isTracking = in_info.isTracking;
			deviceExtrinsic.Update(in_info.deviceExtrinsic);
		}
		public void Update(ExtrinsicInfo_t extInfo)
		{
			isTracking = extInfo.isTracking;
			deviceExtrinsic.extrinsic.Update(extInfo.extrinsic.GetExtrinsic());

			sb.Clear().Append(deviceExtrinsic.trackedDeviceRole.Name())
				.Append(", isTracking: ").Append(isTracking)
				.Append(", position(")
					.Append(deviceExtrinsic.extrinsic.translation.x).Append(", ")
					.Append(deviceExtrinsic.extrinsic.translation.y).Append(", ")
					.Append(deviceExtrinsic.extrinsic.translation.z)
				.Append(")")
				.Append(", rotation(")
					.Append(deviceExtrinsic.extrinsic.rotation.x).Append(", ")
					.Append(deviceExtrinsic.extrinsic.rotation.y).Append(", ")
					.Append(deviceExtrinsic.extrinsic.rotation.z).Append(", ")
					.Append(deviceExtrinsic.extrinsic.rotation.w)
				.Append(")");
			DEBUG(sb);
		}
		public void Update(TrackedDeviceExtrinsic in_ext)
		{
			isTracking = true;
			deviceExtrinsic.Update(in_ext);
		}
	}
	/// <summary>
	/// A class records the developer's choices of tracking devices.
	/// The developer selects which devices to be tracked in the Inspector of BodyManager.Body.
	/// The selections will be imported as a BodyTrackedDevice instance.
	/// </summary>
	public class BodyTrackedDevice
	{
		const string LOG_TAG = "VIVE.OpenXR.Toolkits.BodyTracking.BodyTrackedDevice";
		static StringBuilder m_sb = null;
		static StringBuilder sb
		{
			get
			{
				if (m_sb == null) { m_sb = new StringBuilder(); }
				return m_sb;
			}
		}
		void DEBUG(StringBuilder msg) { Rdp.d(LOG_TAG, msg, true); }
		static int logFrame = 0;
		bool printIntervalLog = false;
		void ERROR(StringBuilder msg) { Rdp.e(LOG_TAG, msg, true); }

		public TrackedDeviceExtrinsicState_t hip = TrackedDeviceExtrinsicState_t.init(TrackedDeviceRole.ROLE_HIP);
		public TrackedDeviceExtrinsicState_t chest = TrackedDeviceExtrinsicState_t.init(TrackedDeviceRole.ROLE_CHEST);
		public TrackedDeviceExtrinsicState_t head = TrackedDeviceExtrinsicState_t.init(TrackedDeviceRole.ROLE_HEAD);

		public TrackedDeviceExtrinsicState_t leftElbow = TrackedDeviceExtrinsicState_t.init(TrackedDeviceRole.ROLE_LEFTELBOW);
		public TrackedDeviceExtrinsicState_t leftWrist = TrackedDeviceExtrinsicState_t.init(TrackedDeviceRole.ROLE_LEFTWRIST);
		public TrackedDeviceExtrinsicState_t leftHand = TrackedDeviceExtrinsicState_t.init(TrackedDeviceRole.ROLE_LEFTHAND);
		public TrackedDeviceExtrinsicState_t leftHandheld = TrackedDeviceExtrinsicState_t.init(TrackedDeviceRole.ROLE_LEFTHANDHELD);

		public TrackedDeviceExtrinsicState_t rightElbow = TrackedDeviceExtrinsicState_t.init(TrackedDeviceRole.ROLE_RIGHTELBOW);
		public TrackedDeviceExtrinsicState_t rightWrist = TrackedDeviceExtrinsicState_t.init(TrackedDeviceRole.ROLE_RIGHTWRIST);
		public TrackedDeviceExtrinsicState_t rightHand = TrackedDeviceExtrinsicState_t.init(TrackedDeviceRole.ROLE_RIGHTHAND);
		public TrackedDeviceExtrinsicState_t rightHandheld = TrackedDeviceExtrinsicState_t.init(TrackedDeviceRole.ROLE_RIGHTHANDHELD);

		public TrackedDeviceExtrinsicState_t leftKnee = TrackedDeviceExtrinsicState_t.init(TrackedDeviceRole.ROLE_LEFTKNEE);
		public TrackedDeviceExtrinsicState_t leftAnkle = TrackedDeviceExtrinsicState_t.init(TrackedDeviceRole.ROLE_LEFTANKLE);
		public TrackedDeviceExtrinsicState_t leftFoot = TrackedDeviceExtrinsicState_t.init(TrackedDeviceRole.ROLE_LEFTFOOT);

		public TrackedDeviceExtrinsicState_t rightKnee = TrackedDeviceExtrinsicState_t.init(TrackedDeviceRole.ROLE_RIGHTKNEE);
		public TrackedDeviceExtrinsicState_t rightAnkle = TrackedDeviceExtrinsicState_t.init(TrackedDeviceRole.ROLE_RIGHTANKLE);
		public TrackedDeviceExtrinsicState_t rightFoot = TrackedDeviceExtrinsicState_t.init(TrackedDeviceRole.ROLE_RIGHTFOOT);

		private Dictionary<DeviceExtRole, TrackedDeviceExtrinsic[]> s_TrackedDeviceExtrinsics = new Dictionary<DeviceExtRole, TrackedDeviceExtrinsic[]>();
		private bool getDeviceExtrinsicsFirstTime = true;
		private void UpdateTrackedDevicesArray()
		{
			Dictionary<DeviceExtRole, List<TrackedDeviceExtrinsic>> dev_exts = new Dictionary<DeviceExtRole, List<TrackedDeviceExtrinsic>>();

			dev_exts.Add(DeviceExtRole.Arm_Wrist, new List<TrackedDeviceExtrinsic>());
			dev_exts.Add(DeviceExtRole.UpperBody_Wrist, new List<TrackedDeviceExtrinsic>());
			dev_exts.Add(DeviceExtRole.FullBody_Wrist_Ankle, new List<TrackedDeviceExtrinsic>());
			dev_exts.Add(DeviceExtRole.FullBody_Wrist_Foot, new List<TrackedDeviceExtrinsic>());

			dev_exts.Add(DeviceExtRole.Arm_Handheld_Hand, new List<TrackedDeviceExtrinsic>());
			dev_exts.Add(DeviceExtRole.UpperBody_Handheld_Hand, new List<TrackedDeviceExtrinsic>());
			dev_exts.Add(DeviceExtRole.FullBody_Handheld_Hand_Ankle, new List<TrackedDeviceExtrinsic>());
			dev_exts.Add(DeviceExtRole.FullBody_Handheld_Hand_Foot, new List<TrackedDeviceExtrinsic>());

			dev_exts.Add(DeviceExtRole.UpperBody_Handheld_Hand_Knee_Ankle, new List<TrackedDeviceExtrinsic>());

			// 7 roles use hip.
			if (hip.isTracking)
			{
				sb.Clear().Append("UpdateTrackedDevicesArray() Uses extrinsic of ").Append(hip.deviceExtrinsic.trackedDeviceRole.Name()); DEBUG(sb);

				dev_exts[DeviceExtRole.UpperBody_Wrist].Add(hip.deviceExtrinsic);
				dev_exts[DeviceExtRole.FullBody_Wrist_Ankle].Add(hip.deviceExtrinsic);
				dev_exts[DeviceExtRole.FullBody_Wrist_Foot].Add(hip.deviceExtrinsic);

				dev_exts[DeviceExtRole.UpperBody_Handheld_Hand].Add(hip.deviceExtrinsic);
				dev_exts[DeviceExtRole.FullBody_Handheld_Hand_Ankle].Add(hip.deviceExtrinsic);
				dev_exts[DeviceExtRole.FullBody_Handheld_Hand_Foot].Add(hip.deviceExtrinsic);

				dev_exts[DeviceExtRole.UpperBody_Handheld_Hand_Knee_Ankle].Add(hip.deviceExtrinsic);
			}
			if (chest.isTracking)
			{
				sb.Clear().Append("UpdateTrackedDevicesArray() Uses extrinsic of ").Append(chest.deviceExtrinsic.trackedDeviceRole.Name()); DEBUG(sb);
			}
			// 9 roles use head.
			if (head.isTracking)
			{
				sb.Clear().Append("UpdateTrackedDevicesArray() Uses extrinsic of ").Append(head.deviceExtrinsic.trackedDeviceRole.Name()); DEBUG(sb);

				dev_exts[DeviceExtRole.Arm_Wrist].Add(head.deviceExtrinsic);
				dev_exts[DeviceExtRole.UpperBody_Wrist].Add(head.deviceExtrinsic);
				dev_exts[DeviceExtRole.FullBody_Wrist_Ankle].Add(head.deviceExtrinsic);
				dev_exts[DeviceExtRole.FullBody_Wrist_Foot].Add(head.deviceExtrinsic);

				dev_exts[DeviceExtRole.Arm_Handheld_Hand].Add(head.deviceExtrinsic);
				dev_exts[DeviceExtRole.UpperBody_Handheld_Hand].Add(head.deviceExtrinsic);
				dev_exts[DeviceExtRole.FullBody_Handheld_Hand_Ankle].Add(head.deviceExtrinsic);
				dev_exts[DeviceExtRole.FullBody_Handheld_Hand_Foot].Add(head.deviceExtrinsic);

				dev_exts[DeviceExtRole.UpperBody_Handheld_Hand_Knee_Ankle].Add(head.deviceExtrinsic);
			}

			if (leftElbow.isTracking)
			{
				sb.Clear().Append("UpdateTrackedDevicesArray() Uses extrinsic of ").Append(leftElbow.deviceExtrinsic.trackedDeviceRole.Name()); DEBUG(sb);
			}
			// 4 roles use leftWrist.
			if (leftWrist.isTracking)
			{
				sb.Clear().Append("UpdateTrackedDevicesArray() Uses extrinsic of ").Append(leftWrist.deviceExtrinsic.trackedDeviceRole.Name()); DEBUG(sb);

				dev_exts[DeviceExtRole.Arm_Wrist].Add(leftWrist.deviceExtrinsic);
				dev_exts[DeviceExtRole.UpperBody_Wrist].Add(leftWrist.deviceExtrinsic);
				dev_exts[DeviceExtRole.FullBody_Wrist_Ankle].Add(leftWrist.deviceExtrinsic);
				dev_exts[DeviceExtRole.FullBody_Wrist_Foot].Add(leftWrist.deviceExtrinsic);
			}
			// 5 roles use leftHand
			if (leftHand.isTracking)
			{
				sb.Clear().Append("UpdateTrackedDevicesArray() Uses extrinsic of ").Append(leftHand.deviceExtrinsic.trackedDeviceRole.Name()); DEBUG(sb);

				dev_exts[DeviceExtRole.Arm_Handheld_Hand].Add(leftHand.deviceExtrinsic);
				dev_exts[DeviceExtRole.UpperBody_Handheld_Hand].Add(leftHand.deviceExtrinsic);
				dev_exts[DeviceExtRole.FullBody_Handheld_Hand_Ankle].Add(leftHand.deviceExtrinsic);
				dev_exts[DeviceExtRole.FullBody_Handheld_Hand_Foot].Add(leftHand.deviceExtrinsic);

				dev_exts[DeviceExtRole.UpperBody_Handheld_Hand_Knee_Ankle].Add(leftHand.deviceExtrinsic);
			}
			// 5 roles use leftHandheld
			if (leftHandheld.isTracking)
			{
				sb.Clear().Append("UpdateTrackedDevicesArray() Uses extrinsic of ").Append(leftHandheld.deviceExtrinsic.trackedDeviceRole.Name()); DEBUG(sb);

				dev_exts[DeviceExtRole.Arm_Handheld_Hand].Add(leftHandheld.deviceExtrinsic);
				dev_exts[DeviceExtRole.UpperBody_Handheld_Hand].Add(leftHandheld.deviceExtrinsic);
				dev_exts[DeviceExtRole.FullBody_Handheld_Hand_Ankle].Add(leftHandheld.deviceExtrinsic);
				dev_exts[DeviceExtRole.FullBody_Handheld_Hand_Foot].Add(leftHandheld.deviceExtrinsic);

				dev_exts[DeviceExtRole.UpperBody_Handheld_Hand_Knee_Ankle].Add(leftHandheld.deviceExtrinsic);
			}

			if (rightElbow.isTracking)
			{
				sb.Clear().Append("UpdateTrackedDevicesArray() Uses extrinsic of ").Append(rightElbow.deviceExtrinsic.trackedDeviceRole.Name()); DEBUG(sb);
			}
			// 4 roles use rightWrist.
			if (rightWrist.isTracking)
			{
				sb.Clear().Append("UpdateTrackedDevicesArray() Uses extrinsic of ").Append(rightWrist.deviceExtrinsic.trackedDeviceRole.Name()); DEBUG(sb);

				dev_exts[DeviceExtRole.Arm_Wrist].Add(rightWrist.deviceExtrinsic);
				dev_exts[DeviceExtRole.UpperBody_Wrist].Add(rightWrist.deviceExtrinsic);
				dev_exts[DeviceExtRole.FullBody_Wrist_Ankle].Add(rightWrist.deviceExtrinsic);
				dev_exts[DeviceExtRole.FullBody_Wrist_Foot].Add(rightWrist.deviceExtrinsic);
			}
			// 5 roles use rightHand
			if (rightHand.isTracking)
			{
				sb.Clear().Append("UpdateTrackedDevicesArray() Uses extrinsic of ").Append(rightHand.deviceExtrinsic.trackedDeviceRole.Name()); DEBUG(sb);

				dev_exts[DeviceExtRole.Arm_Handheld_Hand].Add(rightHand.deviceExtrinsic);
				dev_exts[DeviceExtRole.UpperBody_Handheld_Hand].Add(rightHand.deviceExtrinsic);
				dev_exts[DeviceExtRole.FullBody_Handheld_Hand_Ankle].Add(rightHand.deviceExtrinsic);
				dev_exts[DeviceExtRole.FullBody_Handheld_Hand_Foot].Add(rightHand.deviceExtrinsic);

				dev_exts[DeviceExtRole.UpperBody_Handheld_Hand_Knee_Ankle].Add(rightHand.deviceExtrinsic);
			}
			// 5 roles use rightHandheld
			if (rightHandheld.isTracking)
			{
				sb.Clear().Append("UpdateTrackedDevicesArray() Uses extrinsic of ").Append(rightHandheld.deviceExtrinsic.trackedDeviceRole.Name()); DEBUG(sb);

				dev_exts[DeviceExtRole.Arm_Handheld_Hand].Add(rightHandheld.deviceExtrinsic);
				dev_exts[DeviceExtRole.UpperBody_Handheld_Hand].Add(rightHandheld.deviceExtrinsic);
				dev_exts[DeviceExtRole.FullBody_Handheld_Hand_Ankle].Add(rightHandheld.deviceExtrinsic);
				dev_exts[DeviceExtRole.FullBody_Handheld_Hand_Foot].Add(rightHandheld.deviceExtrinsic);

				dev_exts[DeviceExtRole.UpperBody_Handheld_Hand_Knee_Ankle].Add(rightHandheld.deviceExtrinsic);
			}

			// Only 1 role uses leftKnee.
			if (leftKnee.isTracking)
			{
				sb.Clear().Append("UpdateTrackedDevicesArray() Uses extrinsic of ").Append(leftKnee.deviceExtrinsic.trackedDeviceRole.Name()); DEBUG(sb);

				dev_exts[DeviceExtRole.UpperBody_Handheld_Hand_Knee_Ankle].Add(leftKnee.deviceExtrinsic);
			}
			// 3 roles use leftAnkle
			if (leftAnkle.isTracking)
			{
				sb.Clear().Append("UpdateTrackedDevicesArray() Uses extrinsic of ").Append(leftAnkle.deviceExtrinsic.trackedDeviceRole.Name()); DEBUG(sb);

				dev_exts[DeviceExtRole.FullBody_Wrist_Ankle].Add(leftAnkle.deviceExtrinsic);
				dev_exts[DeviceExtRole.FullBody_Handheld_Hand_Ankle].Add(leftAnkle.deviceExtrinsic);

				dev_exts[DeviceExtRole.UpperBody_Handheld_Hand_Knee_Ankle].Add(leftAnkle.deviceExtrinsic);
			}
			// 2 roles use leftFoot
			if (leftFoot.isTracking)
			{
				sb.Clear().Append("UpdateTrackedDevicesArray() Uses extrinsic of ").Append(leftFoot.deviceExtrinsic.trackedDeviceRole.Name()); DEBUG(sb);

				dev_exts[DeviceExtRole.FullBody_Wrist_Foot].Add(leftFoot.deviceExtrinsic);
				dev_exts[DeviceExtRole.FullBody_Handheld_Hand_Foot].Add(leftFoot.deviceExtrinsic);
			}

			// Only 1 role uses rightKnee.
			if (rightKnee.isTracking)
			{
				sb.Clear().Append("UpdateTrackedDevicesArray() Uses extrinsic of ").Append(rightKnee.deviceExtrinsic.trackedDeviceRole.Name()); DEBUG(sb);

				dev_exts[DeviceExtRole.UpperBody_Handheld_Hand_Knee_Ankle].Add(rightKnee.deviceExtrinsic);
			}
			// 3 roles use rightAnkle
			if (rightAnkle.isTracking)
			{
				sb.Clear().Append("UpdateTrackedDevicesArray() Uses extrinsic of ").Append(rightAnkle.deviceExtrinsic.trackedDeviceRole.Name()); DEBUG(sb);

				dev_exts[DeviceExtRole.FullBody_Wrist_Ankle].Add(rightAnkle.deviceExtrinsic);
				dev_exts[DeviceExtRole.FullBody_Handheld_Hand_Ankle].Add(rightAnkle.deviceExtrinsic);

				dev_exts[DeviceExtRole.UpperBody_Handheld_Hand_Knee_Ankle].Add(rightAnkle.deviceExtrinsic);
			}
			// 2 roles use rightFoot
			if (rightFoot.isTracking)
			{
				sb.Clear().Append("UpdateTrackedDevicesArray() Uses extrinsic of ").Append(rightFoot.deviceExtrinsic.trackedDeviceRole.Name()); DEBUG(sb);

				dev_exts[DeviceExtRole.FullBody_Wrist_Foot].Add(rightFoot.deviceExtrinsic);
				dev_exts[DeviceExtRole.FullBody_Handheld_Hand_Foot].Add(rightFoot.deviceExtrinsic);
			}

			if (s_TrackedDeviceExtrinsics == null) { s_TrackedDeviceExtrinsics = new Dictionary<DeviceExtRole, TrackedDeviceExtrinsic[]>(); }
			s_TrackedDeviceExtrinsics.Clear();

			s_TrackedDeviceExtrinsics.Add(DeviceExtRole.Arm_Wrist, dev_exts[DeviceExtRole.Arm_Wrist].ToArray());
			s_TrackedDeviceExtrinsics.Add(DeviceExtRole.UpperBody_Wrist, dev_exts[DeviceExtRole.UpperBody_Wrist].ToArray());
			s_TrackedDeviceExtrinsics.Add(DeviceExtRole.FullBody_Wrist_Ankle, dev_exts[DeviceExtRole.FullBody_Wrist_Ankle].ToArray());
			s_TrackedDeviceExtrinsics.Add(DeviceExtRole.FullBody_Wrist_Foot, dev_exts[DeviceExtRole.FullBody_Wrist_Foot].ToArray());

			s_TrackedDeviceExtrinsics.Add(DeviceExtRole.Arm_Handheld_Hand, dev_exts[DeviceExtRole.Arm_Handheld_Hand].ToArray());
			s_TrackedDeviceExtrinsics.Add(DeviceExtRole.UpperBody_Handheld_Hand, dev_exts[DeviceExtRole.UpperBody_Handheld_Hand].ToArray());
			s_TrackedDeviceExtrinsics.Add(DeviceExtRole.FullBody_Handheld_Hand_Ankle, dev_exts[DeviceExtRole.FullBody_Handheld_Hand_Ankle].ToArray());
			s_TrackedDeviceExtrinsics.Add(DeviceExtRole.FullBody_Handheld_Hand_Foot, dev_exts[DeviceExtRole.FullBody_Handheld_Hand_Foot].ToArray());

			s_TrackedDeviceExtrinsics.Add(DeviceExtRole.UpperBody_Handheld_Hand_Knee_Ankle, dev_exts[DeviceExtRole.UpperBody_Handheld_Hand_Knee_Ankle].ToArray());

			getDeviceExtrinsicsFirstTime = true;
		}

		public void Update(BodyTrackedDevice in_device)
		{
			if (in_device == null) { return; }

			hip.Update(in_device.hip);
			chest.Update(in_device.chest);
			head.Update(in_device.head);

			leftElbow.Update(in_device.leftElbow);
			leftWrist.Update(in_device.leftWrist);
			leftHand.Update(in_device.leftHand);
			leftHandheld.Update(in_device.leftHandheld);

			rightElbow.Update(in_device.rightElbow);
			rightWrist.Update(in_device.rightWrist);
			rightHand.Update(in_device.rightHand);
			rightHandheld.Update(in_device.rightHandheld);

			leftKnee.Update(in_device.leftKnee);
			leftAnkle.Update(in_device.leftAnkle);
			leftFoot.Update(in_device.leftFoot);

			rightKnee.Update(in_device.rightKnee);
			rightAnkle.Update(in_device.rightAnkle);
			rightFoot.Update(in_device.rightFoot);

			UpdateTrackedDevicesArray();
		}
		/// <summary> Should only be called in CreateBodyTracking() </summary>
		public void Update([In] TrackerExtrinsic in_ext)
		{
			if (in_ext == null) { return; }
			sb.Clear().Append("Update() TrackerExtrinsic of each device."); DEBUG(sb);

			hip.Update(in_ext.hip); // 0
			chest.Update(in_ext.chest);
			head.Update(in_ext.head);

			leftElbow.Update(in_ext.leftElbow);
			leftWrist.Update(in_ext.leftWrist);
			leftHand.Update(in_ext.leftHand); // 5
			leftHandheld.Update(in_ext.leftHandheld);

			rightElbow.Update(in_ext.rightElbow);
			rightWrist.Update(in_ext.rightWrist);
			rightHand.Update(in_ext.rightHand);
			rightHandheld.Update(in_ext.rightHandheld); // 10

			leftKnee.Update(in_ext.leftKnee);
			leftAnkle.Update(in_ext.leftAnkle);
			leftFoot.Update(in_ext.leftFoot);

			rightKnee.Update(in_ext.rightKnee);
			rightAnkle.Update(in_ext.rightAnkle); // 15
			rightFoot.Update(in_ext.rightFoot);

			UpdateTrackedDevicesArray();
		}
		public void Update([In] TrackedDeviceExtrinsic[] bodyTrackedDevices)
		{
			if (bodyTrackedDevices == null) { return; }

			for (int dev = 0; dev < bodyTrackedDevices.Length; dev++)
			{
				if (bodyTrackedDevices[dev].trackedDeviceRole == TrackedDeviceRole.ROLE_HIP) { hip.Update(bodyTrackedDevices[dev]); }
				if (bodyTrackedDevices[dev].trackedDeviceRole == TrackedDeviceRole.ROLE_CHEST) { chest.Update(bodyTrackedDevices[dev]); }
				if (bodyTrackedDevices[dev].trackedDeviceRole == TrackedDeviceRole.ROLE_CHEST) { head.Update(bodyTrackedDevices[dev]); }

				if (bodyTrackedDevices[dev].trackedDeviceRole == TrackedDeviceRole.ROLE_LEFTELBOW) { leftElbow.Update(bodyTrackedDevices[dev]); }
				if (bodyTrackedDevices[dev].trackedDeviceRole == TrackedDeviceRole.ROLE_LEFTWRIST) { leftWrist.Update(bodyTrackedDevices[dev]); }
				if (bodyTrackedDevices[dev].trackedDeviceRole == TrackedDeviceRole.ROLE_LEFTHAND) { leftHand.Update(bodyTrackedDevices[dev]); }
				if (bodyTrackedDevices[dev].trackedDeviceRole == TrackedDeviceRole.ROLE_LEFTHANDHELD) { leftHandheld.Update(bodyTrackedDevices[dev]); }

				if (bodyTrackedDevices[dev].trackedDeviceRole == TrackedDeviceRole.ROLE_RIGHTELBOW) { rightElbow.Update(bodyTrackedDevices[dev]); }
				if (bodyTrackedDevices[dev].trackedDeviceRole == TrackedDeviceRole.ROLE_RIGHTWRIST) { rightWrist.Update(bodyTrackedDevices[dev]); }
				if (bodyTrackedDevices[dev].trackedDeviceRole == TrackedDeviceRole.ROLE_RIGHTHAND) { rightHand.Update(bodyTrackedDevices[dev]); }
				if (bodyTrackedDevices[dev].trackedDeviceRole == TrackedDeviceRole.ROLE_RIGHTHANDHELD) { rightHandheld.Update(bodyTrackedDevices[dev]); }

				if (bodyTrackedDevices[dev].trackedDeviceRole == TrackedDeviceRole.ROLE_LEFTKNEE) { leftKnee.Update(bodyTrackedDevices[dev]); }
				if (bodyTrackedDevices[dev].trackedDeviceRole == TrackedDeviceRole.ROLE_LEFTANKLE) { leftAnkle.Update(bodyTrackedDevices[dev]); }
				if (bodyTrackedDevices[dev].trackedDeviceRole == TrackedDeviceRole.ROLE_LEFTFOOT) { leftFoot.Update(bodyTrackedDevices[dev]); }

				if (bodyTrackedDevices[dev].trackedDeviceRole == TrackedDeviceRole.ROLE_RIGHTKNEE) { rightKnee.Update(bodyTrackedDevices[dev]); }
				if (bodyTrackedDevices[dev].trackedDeviceRole == TrackedDeviceRole.ROLE_RIGHTANKLE) { rightAnkle.Update(bodyTrackedDevices[dev]); }
				if (bodyTrackedDevices[dev].trackedDeviceRole == TrackedDeviceRole.ROLE_RIGHTFOOT) { rightFoot.Update(bodyTrackedDevices[dev]); }
			}

			if (s_TrackedDeviceExtrinsics == null) { UpdateTrackedDevicesArray(); }

			for (int dev = 0; dev < bodyTrackedDevices.Length; dev++)
			{
				for (int i = 0; i < s_TrackedDeviceExtrinsics.Count; i++)
				{
					DeviceExtRole role = s_TrackedDeviceExtrinsics.ElementAt(i).Key;
					for (int ext = 0; ext < s_TrackedDeviceExtrinsics[role].Length; ext++)
					{
						if (s_TrackedDeviceExtrinsics[role][ext].trackedDeviceRole == bodyTrackedDevices[dev].trackedDeviceRole)
						{
							sb.Clear().Append("Update() ").Append(s_TrackedDeviceExtrinsics[role][ext].trackedDeviceRole.Name()).Append(" extrinsic")
								.Append(" at s_TrackedDeviceExtrinsics[").Append(role.Name()).Append("][").Append(ext).Append("]");
							DEBUG(sb);

							s_TrackedDeviceExtrinsics[role][ext].extrinsic.Update(bodyTrackedDevices[dev].extrinsic);
						}
					}
				}
			}
		}

		/// <summary> The device extrinsics for use depends on the calibration pose role. </summary>
		public bool GetDevicesExtrinsics(BodyPoseRole calibRole, out TrackedDeviceExtrinsic[] bodyTrackedDevices, out UInt32 bodyTrackedDeviceCount)
		{
			logFrame++;
			logFrame %= 500;
			printIntervalLog = (logFrame == 0);

			// Upper Body + Leg FK
			if (calibRole == BodyPoseRole.UpperBody_Handheld_Knee_Ankle || calibRole == BodyPoseRole.UpperBody_Hand_Knee_Ankle)
			{
				bodyTrackedDevices = s_TrackedDeviceExtrinsics[DeviceExtRole.UpperBody_Handheld_Hand_Knee_Ankle];
				bodyTrackedDeviceCount = (UInt32)(bodyTrackedDevices.Length & 0x7FFFFFFF);

				if (printIntervalLog || getDeviceExtrinsicsFirstTime)
				{
					sb.Clear().Append("GetDevicesExtrinsics() of extrinsic role ").Append(DeviceExtRole.UpperBody_Handheld_Hand_Knee_Ankle.Name()); DEBUG(sb);
					sb.Clear();
					for (int i = 0; i < bodyTrackedDeviceCount; i++)
					{
						sb.Append("GetDevicesExtrinsics() Add extrinsic[").Append(i).Append("] ")
							.Append(bodyTrackedDevices[i].trackedDeviceRole.Name()).Append("\n");
					}
					DEBUG(sb);
				}
				return bodyTrackedDeviceCount > 0;
			}

			// Full Body
			if (calibRole == BodyPoseRole.FullBody_Wrist_Ankle)
			{
				bodyTrackedDevices = s_TrackedDeviceExtrinsics[DeviceExtRole.FullBody_Wrist_Ankle];
				bodyTrackedDeviceCount = (UInt32)(bodyTrackedDevices.Length & 0x7FFFFFFF);

				if (printIntervalLog || getDeviceExtrinsicsFirstTime)
				{
					sb.Clear().Append("GetDevicesExtrinsics() of extrinsic role ").Append(DeviceExtRole.FullBody_Wrist_Ankle.Name()); DEBUG(sb);
					sb.Clear();
					for (int i = 0; i < bodyTrackedDeviceCount; i++)
					{
						sb.Append("GetDevicesExtrinsics() Add extrinsic[").Append(i).Append("] ")
							.Append(bodyTrackedDevices[i].trackedDeviceRole.Name()).Append("\n");
					}
					DEBUG(sb);
				}
				return bodyTrackedDeviceCount > 0;
			}
			if (calibRole == BodyPoseRole.FullBody_Wrist_Foot)
			{
				bodyTrackedDevices = s_TrackedDeviceExtrinsics[DeviceExtRole.FullBody_Wrist_Foot];
				bodyTrackedDeviceCount = (UInt32)(bodyTrackedDevices.Length & 0x7FFFFFFF);

				if (printIntervalLog || getDeviceExtrinsicsFirstTime)
				{
					sb.Clear().Append("GetDevicesExtrinsics() of extrinsic role ").Append(DeviceExtRole.FullBody_Wrist_Foot.Name()); DEBUG(sb);
					sb.Clear();
					for (int i = 0; i < bodyTrackedDeviceCount; i++)
					{
						sb.Append("GetDevicesExtrinsics() Add extrinsic[").Append(i).Append("] ")
							.Append(bodyTrackedDevices[i].trackedDeviceRole.Name()).Append("\n");
					}
					DEBUG(sb);
				}
				return bodyTrackedDeviceCount > 0;
			}
			if (calibRole == BodyPoseRole.FullBody_Handheld_Ankle || calibRole == BodyPoseRole.FullBody_Hand_Ankle)
			{
				bodyTrackedDevices = s_TrackedDeviceExtrinsics[DeviceExtRole.FullBody_Handheld_Hand_Ankle];
				bodyTrackedDeviceCount = (UInt32)(bodyTrackedDevices.Length & 0x7FFFFFFF);

				if (printIntervalLog || getDeviceExtrinsicsFirstTime)
				{
					sb.Clear().Append("GetDevicesExtrinsics() of extrinsic role ").Append(DeviceExtRole.FullBody_Handheld_Hand_Ankle.Name()); DEBUG(sb);
					sb.Clear();
					for (int i = 0; i < bodyTrackedDeviceCount; i++)
					{
						sb.Append("GetDevicesExtrinsics() Add extrinsic[").Append(i).Append("] ")
							.Append(bodyTrackedDevices[i].trackedDeviceRole.Name()).Append("\n");
					}
					DEBUG(sb);
				}
				return bodyTrackedDeviceCount > 0;
			}
			if (calibRole == BodyPoseRole.FullBody_Handheld_Foot || calibRole == BodyPoseRole.FullBody_Hand_Foot)
			{
				bodyTrackedDevices = s_TrackedDeviceExtrinsics[DeviceExtRole.FullBody_Handheld_Hand_Foot];
				bodyTrackedDeviceCount = (UInt32)(bodyTrackedDevices.Length & 0x7FFFFFFF);

				if (printIntervalLog || getDeviceExtrinsicsFirstTime)
				{
					sb.Clear().Append("GetDevicesExtrinsics() of extrinsic role ").Append(DeviceExtRole.FullBody_Handheld_Hand_Foot.Name()); DEBUG(sb);
					sb.Clear();
					for (int i = 0; i < bodyTrackedDeviceCount; i++)
					{
						sb.Append("GetDevicesExtrinsics() Add extrinsic[").Append(i).Append("] ")
							.Append(bodyTrackedDevices[i].trackedDeviceRole.Name()).Append("\n");
					}
					DEBUG(sb);
				}
				return bodyTrackedDeviceCount > 0;
			}

			// Upper Body
			if (calibRole == BodyPoseRole.UpperBody_Wrist)
			{
				bodyTrackedDevices = s_TrackedDeviceExtrinsics[DeviceExtRole.UpperBody_Wrist];
				bodyTrackedDeviceCount = (UInt32)(bodyTrackedDevices.Length & 0x7FFFFFFF);

				if (printIntervalLog || getDeviceExtrinsicsFirstTime)
				{
					sb.Clear().Append("GetDevicesExtrinsics() of extrinsic role ").Append(DeviceExtRole.UpperBody_Wrist.Name()); DEBUG(sb);
					sb.Clear();
					for (int i = 0; i < bodyTrackedDeviceCount; i++)
					{
						sb.Append("GetDevicesExtrinsics() Add extrinsic[").Append(i).Append("] ")
							.Append(bodyTrackedDevices[i].trackedDeviceRole.Name()).Append("\n");
					}
					DEBUG(sb);
				}
				return bodyTrackedDeviceCount > 0;
			}
			if (calibRole == BodyPoseRole.UpperBody_Handheld || calibRole == BodyPoseRole.UpperBody_Hand)
			{
				bodyTrackedDevices = s_TrackedDeviceExtrinsics[DeviceExtRole.UpperBody_Handheld_Hand];
				bodyTrackedDeviceCount = (UInt32)(bodyTrackedDevices.Length & 0x7FFFFFFF);

				if (printIntervalLog || getDeviceExtrinsicsFirstTime)
				{
					sb.Clear().Append("GetDevicesExtrinsics() of extrinsic role ").Append(DeviceExtRole.UpperBody_Handheld_Hand.Name()); DEBUG(sb);
					sb.Clear();
					for (int i = 0; i < bodyTrackedDeviceCount; i++)
					{
						sb.Append("GetDevicesExtrinsics() Add extrinsic[").Append(i).Append("] ")
							.Append(bodyTrackedDevices[i].trackedDeviceRole.Name()).Append("\n");
					}
					DEBUG(sb);
				}
				return bodyTrackedDeviceCount > 0;
			}

			// Arm
			if (calibRole == BodyPoseRole.Arm_Wrist)
			{
				bodyTrackedDevices = s_TrackedDeviceExtrinsics[DeviceExtRole.Arm_Wrist];
				bodyTrackedDeviceCount = (UInt32)(bodyTrackedDevices.Length & 0x7FFFFFFF);

				if (printIntervalLog || getDeviceExtrinsicsFirstTime)
				{
					sb.Clear().Append("GetDevicesExtrinsics() of extrinsic role ").Append(DeviceExtRole.Arm_Wrist.Name()); DEBUG(sb);
					sb.Clear();
					for (int i = 0; i < bodyTrackedDeviceCount; i++)
					{
						sb.Append("GetDevicesExtrinsics() Add extrinsic[").Append(i).Append("] ")
							.Append(bodyTrackedDevices[i].trackedDeviceRole.Name()).Append("\n");
					}
					DEBUG(sb);
				}
				return bodyTrackedDeviceCount > 0;
			}
			if (calibRole == BodyPoseRole.Arm_Handheld || calibRole == BodyPoseRole.Arm_Hand)
			{
				bodyTrackedDevices = s_TrackedDeviceExtrinsics[DeviceExtRole.Arm_Handheld_Hand];
				bodyTrackedDeviceCount = (UInt32)(bodyTrackedDevices.Length & 0x7FFFFFFF);

				if (printIntervalLog || getDeviceExtrinsicsFirstTime)
				{
					sb.Clear().Append("GetDevicesExtrinsics() of extrinsic role ").Append(DeviceExtRole.Arm_Handheld_Hand.Name()); DEBUG(sb);
					sb.Clear();
					for (int i = 0; i < bodyTrackedDeviceCount; i++)
					{
						sb.Append("GetDevicesExtrinsics() Add extrinsic[").Append(i).Append("] ")
							.Append(bodyTrackedDevices[i].trackedDeviceRole.Name()).Append("\n");
					}
					DEBUG(sb);
				}
				return bodyTrackedDeviceCount > 0;
			}

			bodyTrackedDevices = null;
			bodyTrackedDeviceCount = 0;
			return false;
		}

		private int ikFrame = -1;
		private DeviceExtRole m_IKRoles = DeviceExtRole.Unknown;
		public DeviceExtRole GetIKRoles(BodyPoseRole calibRole)
		{
			if (printIntervalLog || getDeviceExtrinsicsFirstTime) { sb.Clear().Append("GetIKRoles()"); DEBUG(sb); }

			if (ikFrame == Time.frameCount) { return m_IKRoles; }
			else { m_IKRoles = DeviceExtRole.Unknown; ikFrame = Time.frameCount; }

			if (GetDevicesExtrinsics(calibRole, out TrackedDeviceExtrinsic[] bodyTrackedDevices, out UInt32 bodyTrackedDeviceCount))
				m_IKRoles = BodyTrackingUtils.GetDeviceExtRole(calibRole, bodyTrackedDevices, bodyTrackedDeviceCount);

			return m_IKRoles;
		}
	}

	internal struct TrackingInfos_t
	{
		internal struct TrackingInfo_t
		{
			public TrackedDeviceType type;
			public UInt32[] ids;
			public UInt32 count;

			public TrackingInfo_t(TrackedDeviceType in_type, UInt32[] in_ids, UInt32 in_count)
			{
				type = in_type;
				ids = in_ids;
				count = in_count;
			}
			public void Update(TrackingInfo_t in_info)
			{
				type = in_info.type;
				if (count != in_info.count && in_info.count > 0)
				{
					count = in_info.count;
					ids = new UInt32[count];
				}
				for (UInt32 i = 0; i < count; i++)
					ids[i] = in_info.ids[i];
			}
		}

		public TrackingInfo_t[] s_info;
		public UInt32 size;
		public TrackingInfos_t(TrackingInfo_t[] in_info, UInt32 in_size)
		{
			s_info = in_info;
			size = in_size;
		}
		public void Update(TrackingInfos_t in_infos)
		{
			if (size != in_infos.size && in_infos.size > 0)
			{
				size = in_infos.size;
				s_info = new TrackingInfo_t[size];
				for (UInt32 i = 0; i < size; i++)
					s_info[i].Update(in_infos.s_info[i]);
			}
		}
		public void Update(TrackingInfo_t in_info)
		{
			for (int i = 0; i < s_info.Length; i++)
			{
				if (s_info[i].type == in_info.type)
					s_info[i].Update(in_info);
			}
		}
	}
	public class BodyPose
	{
		const string LOG_TAG = "VIVE.OpenXR.Toolkits.BodyTracking.BodyPose";
		StringBuilder m_sb = null;
		StringBuilder sb
		{
			get
			{
				if (m_sb == null) { m_sb = new StringBuilder(); }
				return m_sb;
			}
		}
		void DEBUG(StringBuilder msg) { Rdp.d(LOG_TAG, msg, true); }

		public BodyTrackingMode trackingMode = BodyTrackingMode.UPPERIKANDLEGFK;

		public TrackedDevicePose hip = TrackedDevicePose.identity;
		public TrackedDevicePose chest = TrackedDevicePose.identity;
		public TrackedDevicePose head = TrackedDevicePose.identity;

		public TrackedDevicePose leftElbow = TrackedDevicePose.identity;
		public TrackedDevicePose leftWrist = TrackedDevicePose.identity;
		public TrackedDevicePose leftHand = TrackedDevicePose.identity;
		public TrackedDevicePose leftHandheld = TrackedDevicePose.identity;

		public TrackedDevicePose rightElbow = TrackedDevicePose.identity;
		public TrackedDevicePose rightWrist = TrackedDevicePose.identity;
		public TrackedDevicePose rightHand = TrackedDevicePose.identity;
		public TrackedDevicePose rightHandheld = TrackedDevicePose.identity;

		public TrackedDevicePose leftKnee = TrackedDevicePose.identity;
		public TrackedDevicePose leftAnkle = TrackedDevicePose.identity;
		public TrackedDevicePose leftFoot = TrackedDevicePose.identity;

		public TrackedDevicePose rightKnee = TrackedDevicePose.identity;
		public TrackedDevicePose rightAnkle = TrackedDevicePose.identity;
		public TrackedDevicePose rightFoot = TrackedDevicePose.identity;

		public const float kUserHeight = 1.6f;
		public float userCalibrationHeight = kUserHeight;

		internal TrackedDeviceType[] s_TrackedDeviceTypes = new TrackedDeviceType[(int)TrackedDeviceRole.NUMS_OF_ROLE]
		{
			TrackedDeviceType.Invalid, TrackedDeviceType.Invalid, TrackedDeviceType.Invalid, TrackedDeviceType.Invalid, TrackedDeviceType.Invalid,
			TrackedDeviceType.Invalid, TrackedDeviceType.Invalid, TrackedDeviceType.Invalid, TrackedDeviceType.Invalid, TrackedDeviceType.Invalid,
			TrackedDeviceType.Invalid, TrackedDeviceType.Invalid, TrackedDeviceType.Invalid, TrackedDeviceType.Invalid, TrackedDeviceType.Invalid,
			TrackedDeviceType.Invalid, TrackedDeviceType.Invalid
		};
		private void InitTrackedDeviceTypes()
		{
			s_TrackedDeviceTypes = new TrackedDeviceType[(int)TrackedDeviceRole.NUMS_OF_ROLE]
			{
				TrackedDeviceType.Invalid, TrackedDeviceType.Invalid, TrackedDeviceType.Invalid, TrackedDeviceType.Invalid, TrackedDeviceType.Invalid,
				TrackedDeviceType.Invalid, TrackedDeviceType.Invalid, TrackedDeviceType.Invalid, TrackedDeviceType.Invalid, TrackedDeviceType.Invalid,
				TrackedDeviceType.Invalid, TrackedDeviceType.Invalid, TrackedDeviceType.Invalid, TrackedDeviceType.Invalid, TrackedDeviceType.Invalid,
				TrackedDeviceType.Invalid, TrackedDeviceType.Invalid
			};
		}
		private void ResetTrackedDeviceTypes()
		{
			if (s_TrackedDeviceTypes == null) { InitTrackedDeviceTypes(); return; }
			for (int i = 0; i < s_TrackedDeviceTypes.Length; i++)
				s_TrackedDeviceTypes[i] = TrackedDeviceType.Invalid;
		}

		[Obsolete("This function is deprecated, please use BodyPose(BodyTrackingMode) instead.")]
		public BodyPose()
		{
			hip.trackedDeviceRole = TrackedDeviceRole.ROLE_HIP;
			chest.trackedDeviceRole = TrackedDeviceRole.ROLE_CHEST;
			head.trackedDeviceRole = TrackedDeviceRole.ROLE_HEAD;

			leftElbow.trackedDeviceRole = TrackedDeviceRole.ROLE_LEFTELBOW;
			leftWrist.trackedDeviceRole = TrackedDeviceRole.ROLE_LEFTWRIST;
			leftHand.trackedDeviceRole = TrackedDeviceRole.ROLE_LEFTHAND;
			leftHandheld.trackedDeviceRole = TrackedDeviceRole.ROLE_LEFTHANDHELD;

			rightElbow.trackedDeviceRole = TrackedDeviceRole.ROLE_RIGHTELBOW;
			rightWrist.trackedDeviceRole = TrackedDeviceRole.ROLE_RIGHTWRIST;
			rightHand.trackedDeviceRole = TrackedDeviceRole.ROLE_RIGHTHAND;
			rightHandheld.trackedDeviceRole = TrackedDeviceRole.ROLE_RIGHTHANDHELD;

			leftKnee.trackedDeviceRole = TrackedDeviceRole.ROLE_LEFTKNEE;
			leftAnkle.trackedDeviceRole = TrackedDeviceRole.ROLE_LEFTANKLE;
			leftFoot.trackedDeviceRole = TrackedDeviceRole.ROLE_LEFTFOOT;

			rightKnee.trackedDeviceRole = TrackedDeviceRole.ROLE_RIGHTKNEE;
			rightAnkle.trackedDeviceRole = TrackedDeviceRole.ROLE_RIGHTANKLE;
			rightFoot.trackedDeviceRole = TrackedDeviceRole.ROLE_RIGHTFOOT;
		}
		public BodyPose(BodyTrackingMode mode)
		{
			trackingMode = mode;

			hip.trackedDeviceRole = TrackedDeviceRole.ROLE_HIP;
			chest.trackedDeviceRole = TrackedDeviceRole.ROLE_CHEST;
			head.trackedDeviceRole = TrackedDeviceRole.ROLE_HEAD;

			leftElbow.trackedDeviceRole = TrackedDeviceRole.ROLE_LEFTELBOW;
			leftWrist.trackedDeviceRole = TrackedDeviceRole.ROLE_LEFTWRIST;
			leftHand.trackedDeviceRole = TrackedDeviceRole.ROLE_LEFTHAND;
			leftHandheld.trackedDeviceRole = TrackedDeviceRole.ROLE_LEFTHANDHELD;

			rightElbow.trackedDeviceRole = TrackedDeviceRole.ROLE_RIGHTELBOW;
			rightWrist.trackedDeviceRole = TrackedDeviceRole.ROLE_RIGHTWRIST;
			rightHand.trackedDeviceRole = TrackedDeviceRole.ROLE_RIGHTHAND;
			rightHandheld.trackedDeviceRole = TrackedDeviceRole.ROLE_RIGHTHANDHELD;

			leftKnee.trackedDeviceRole = TrackedDeviceRole.ROLE_LEFTKNEE;
			leftAnkle.trackedDeviceRole = TrackedDeviceRole.ROLE_LEFTANKLE;
			leftFoot.trackedDeviceRole = TrackedDeviceRole.ROLE_LEFTFOOT;

			rightKnee.trackedDeviceRole = TrackedDeviceRole.ROLE_RIGHTKNEE;
			rightAnkle.trackedDeviceRole = TrackedDeviceRole.ROLE_RIGHTANKLE;
			rightFoot.trackedDeviceRole = TrackedDeviceRole.ROLE_RIGHTFOOT;
		}
		public void Update([In] BodyPose in_body)
		{
			trackingMode = in_body.trackingMode;

			hip.Update(in_body.hip);
			chest.Update(in_body.chest);
			head.Update(in_body.head);

			leftElbow.Update(in_body.leftElbow);
			leftWrist.Update(in_body.leftWrist);
			leftHand.Update(in_body.leftHand);
			leftHandheld.Update(in_body.leftHandheld);

			rightElbow.Update(in_body.rightElbow);
			rightWrist.Update(in_body.rightWrist);
			rightHand.Update(in_body.rightHand);
			rightHandheld.Update(in_body.rightHandheld);

			leftKnee.Update(in_body.leftKnee);
			leftAnkle.Update(in_body.leftAnkle);
			leftFoot.Update(in_body.leftFoot);

			rightKnee.Update(in_body.rightKnee);
			rightAnkle.Update(in_body.rightAnkle);
			rightFoot.Update(in_body.rightFoot);

			userCalibrationHeight = in_body.userCalibrationHeight;
			s_TrackedDeviceTypes = in_body.s_TrackedDeviceTypes;
		}
		public void Update(TrackedDevicePose[] trackedDevicePoses)
		{
			if (trackedDevicePoses == null) { return; }
			string func = "Update() ";

			for (int i = 0; i < trackedDevicePoses.Length; i++)
			{
				if (trackedDevicePoses[i].trackedDeviceRole == TrackedDeviceRole.ROLE_HIP)
				{
					hip.Update(trackedDevicePoses[i]);
					sb.Clear().Append(func).Append(hip.trackedDeviceRole.Name()).Append(", poseState: ").Append(hip.poseState)
						.Append(", position (").Append(hip.translation.x.ToString("N3")).Append(", ").Append(hip.translation.y.ToString("N3")).Append(", ").Append(hip.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(hip.rotation.x.ToString("N3")).Append(", ").Append(hip.rotation.y.ToString("N3")).Append(", ").Append(hip.rotation.z.ToString("N3")).Append(", ").Append(hip.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
				if (trackedDevicePoses[i].trackedDeviceRole == TrackedDeviceRole.ROLE_CHEST)
				{
					chest.Update(trackedDevicePoses[i]);
					sb.Clear().Append(func).Append(chest.trackedDeviceRole.Name()).Append(", poseState: ").Append(chest.poseState)
						.Append(", position (").Append(chest.translation.x.ToString("N3")).Append(", ").Append(chest.translation.y.ToString("N3")).Append(", ").Append(chest.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(chest.rotation.x.ToString("N3")).Append(", ").Append(chest.rotation.y.ToString("N3")).Append(", ").Append(chest.rotation.z.ToString("N3")).Append(", ").Append(chest.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
				if (trackedDevicePoses[i].trackedDeviceRole == TrackedDeviceRole.ROLE_HEAD)
				{
					head.Update(trackedDevicePoses[i]);
					sb.Clear().Append(func).Append(head.trackedDeviceRole.Name()).Append(", poseState: ").Append(head.poseState)
						.Append(", position (").Append(head.translation.x.ToString("N3")).Append(", ").Append(head.translation.y.ToString("N3")).Append(", ").Append(head.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(head.rotation.x.ToString("N3")).Append(", ").Append(head.rotation.y.ToString("N3")).Append(", ").Append(head.rotation.z.ToString("N3")).Append(", ").Append(head.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}

				if (trackedDevicePoses[i].trackedDeviceRole == TrackedDeviceRole.ROLE_LEFTELBOW)
				{
					leftElbow.Update(trackedDevicePoses[i]);
					sb.Clear().Append(func).Append(leftElbow.trackedDeviceRole.Name()).Append(", poseState: ").Append(leftElbow.poseState)
						.Append(", position (").Append(leftElbow.translation.x.ToString("N3")).Append(", ").Append(leftElbow.translation.y.ToString("N3")).Append(", ").Append(leftElbow.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(leftElbow.rotation.x.ToString("N3")).Append(", ").Append(leftElbow.rotation.y.ToString("N3")).Append(", ").Append(leftElbow.rotation.z.ToString("N3")).Append(", ").Append(leftElbow.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
				if (trackedDevicePoses[i].trackedDeviceRole == TrackedDeviceRole.ROLE_LEFTWRIST)
				{
					leftWrist.Update(trackedDevicePoses[i]);
					sb.Clear().Append(func).Append(leftWrist.trackedDeviceRole.Name()).Append(", poseState: ").Append(leftWrist.poseState)
						.Append(", position (").Append(leftWrist.translation.x.ToString("N3")).Append(", ").Append(leftWrist.translation.y.ToString("N3")).Append(", ").Append(leftWrist.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(leftWrist.rotation.x.ToString("N3")).Append(", ").Append(leftWrist.rotation.y.ToString("N3")).Append(", ").Append(leftWrist.rotation.z.ToString("N3")).Append(", ").Append(leftWrist.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
				if (trackedDevicePoses[i].trackedDeviceRole == TrackedDeviceRole.ROLE_LEFTHAND)
				{
					leftHand.Update(trackedDevicePoses[i]);
					sb.Clear().Append(func).Append(leftHand.trackedDeviceRole.Name()).Append(", poseState: ").Append(leftHand.poseState)
						.Append(", position (").Append(leftHand.translation.x.ToString("N3")).Append(", ").Append(leftHand.translation.y.ToString("N3")).Append(", ").Append(leftHand.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(leftHand.rotation.x.ToString("N3")).Append(", ").Append(leftHand.rotation.y.ToString("N3")).Append(", ").Append(leftHand.rotation.z.ToString("N3")).Append(", ").Append(leftHand.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
				if (trackedDevicePoses[i].trackedDeviceRole == TrackedDeviceRole.ROLE_LEFTHANDHELD)
				{
					leftHandheld.Update(trackedDevicePoses[i]);
					sb.Clear().Append(func).Append(leftHandheld.trackedDeviceRole.Name()).Append(", poseState: ").Append(leftHandheld.poseState)
						.Append(", position (").Append(leftHandheld.translation.x.ToString("N3")).Append(", ").Append(leftHandheld.translation.y.ToString("N3")).Append(", ").Append(leftHandheld.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(leftHandheld.rotation.x.ToString("N3")).Append(", ").Append(leftHandheld.rotation.y.ToString("N3")).Append(", ").Append(leftHandheld.rotation.z.ToString("N3")).Append(", ").Append(leftHandheld.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}

				if (trackedDevicePoses[i].trackedDeviceRole == TrackedDeviceRole.ROLE_RIGHTELBOW)
				{
					rightElbow.Update(trackedDevicePoses[i]);
					sb.Clear().Append(func).Append(rightElbow.trackedDeviceRole.Name()).Append(", poseState: ").Append(rightElbow.poseState)
						.Append(", position (").Append(rightElbow.translation.x.ToString("N3")).Append(", ").Append(rightElbow.translation.y.ToString("N3")).Append(", ").Append(rightElbow.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(rightElbow.rotation.x.ToString("N3")).Append(", ").Append(rightElbow.rotation.y.ToString("N3")).Append(", ").Append(rightElbow.rotation.z.ToString("N3")).Append(", ").Append(rightElbow.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
				if (trackedDevicePoses[i].trackedDeviceRole == TrackedDeviceRole.ROLE_RIGHTWRIST)
				{
					rightWrist.Update(trackedDevicePoses[i]);
					sb.Clear().Append(func).Append(rightWrist.trackedDeviceRole.Name()).Append(", poseState: ").Append(rightWrist.poseState)
						.Append(", position (").Append(rightWrist.translation.x.ToString("N3")).Append(", ").Append(rightWrist.translation.y.ToString("N3")).Append(", ").Append(rightWrist.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(rightWrist.rotation.x.ToString("N3")).Append(", ").Append(rightWrist.rotation.y.ToString("N3")).Append(", ").Append(rightWrist.rotation.z.ToString("N3")).Append(", ").Append(rightWrist.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
				if (trackedDevicePoses[i].trackedDeviceRole == TrackedDeviceRole.ROLE_RIGHTHAND)
				{
					rightHand.Update(trackedDevicePoses[i]);
					sb.Clear().Append(func).Append(rightHand.trackedDeviceRole.Name()).Append(", poseState: ").Append(rightHand.poseState)
						.Append(", position (").Append(rightHand.translation.x.ToString("N3")).Append(", ").Append(rightHand.translation.y.ToString("N3")).Append(", ").Append(rightHand.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(rightHand.rotation.x.ToString("N3")).Append(", ").Append(rightHand.rotation.y.ToString("N3")).Append(", ").Append(rightHand.rotation.z.ToString("N3")).Append(", ").Append(rightHand.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
				if (trackedDevicePoses[i].trackedDeviceRole == TrackedDeviceRole.ROLE_RIGHTHANDHELD)
				{
					rightHandheld.Update(trackedDevicePoses[i]);
					sb.Clear().Append(func).Append(rightHandheld.trackedDeviceRole.Name()).Append(", poseState: ").Append(rightHandheld.poseState)
						.Append(", position (").Append(rightHandheld.translation.x.ToString("N3")).Append(", ").Append(rightHandheld.translation.y.ToString("N3")).Append(", ").Append(rightHandheld.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(rightHandheld.rotation.x.ToString("N3")).Append(", ").Append(rightHandheld.rotation.y.ToString("N3")).Append(", ").Append(rightHandheld.rotation.z.ToString("N3")).Append(", ").Append(rightHandheld.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}

				if (trackedDevicePoses[i].trackedDeviceRole == TrackedDeviceRole.ROLE_LEFTKNEE)
				{
					leftKnee.Update(trackedDevicePoses[i]);
					sb.Clear().Append(func).Append(leftKnee.trackedDeviceRole.Name()).Append(", poseState: ").Append(leftKnee.poseState)
						.Append(", position (").Append(leftKnee.translation.x.ToString("N3")).Append(", ").Append(leftKnee.translation.y.ToString("N3")).Append(", ").Append(leftKnee.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(leftKnee.rotation.x.ToString("N3")).Append(", ").Append(leftKnee.rotation.y.ToString("N3")).Append(", ").Append(leftKnee.rotation.z.ToString("N3")).Append(", ").Append(leftKnee.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
				if (trackedDevicePoses[i].trackedDeviceRole == TrackedDeviceRole.ROLE_LEFTANKLE)
				{
					leftAnkle.Update(trackedDevicePoses[i]);
					sb.Clear().Append(func).Append(leftAnkle.trackedDeviceRole.Name()).Append(", poseState: ").Append(leftAnkle.poseState)
						.Append(", position (").Append(leftAnkle.translation.x.ToString("N3")).Append(", ").Append(leftAnkle.translation.y.ToString("N3")).Append(", ").Append(leftAnkle.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(leftAnkle.rotation.x.ToString("N3")).Append(", ").Append(leftAnkle.rotation.y.ToString("N3")).Append(", ").Append(leftAnkle.rotation.z.ToString("N3")).Append(", ").Append(leftAnkle.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
				if (trackedDevicePoses[i].trackedDeviceRole == TrackedDeviceRole.ROLE_LEFTFOOT)
				{
					leftFoot.Update(trackedDevicePoses[i]);
					sb.Clear().Append(func).Append(leftFoot.trackedDeviceRole.Name()).Append(", poseState: ").Append(leftFoot.poseState)
						.Append(", position (").Append(leftFoot.translation.x.ToString("N3")).Append(", ").Append(leftFoot.translation.y.ToString("N3")).Append(", ").Append(leftFoot.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(leftFoot.rotation.x.ToString("N3")).Append(", ").Append(leftFoot.rotation.y.ToString("N3")).Append(", ").Append(leftFoot.rotation.z.ToString("N3")).Append(", ").Append(leftFoot.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}

				if (trackedDevicePoses[i].trackedDeviceRole == TrackedDeviceRole.ROLE_RIGHTKNEE)
				{
					rightKnee.Update(trackedDevicePoses[i]);
					sb.Clear().Append(func).Append(rightKnee.trackedDeviceRole.Name()).Append(", poseState: ").Append(rightKnee.poseState)
						.Append(", position (").Append(rightKnee.translation.x.ToString("N3")).Append(", ").Append(rightKnee.translation.y.ToString("N3")).Append(", ").Append(rightKnee.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(rightKnee.rotation.x.ToString("N3")).Append(", ").Append(rightKnee.rotation.y.ToString("N3")).Append(", ").Append(rightKnee.rotation.z.ToString("N3")).Append(", ").Append(rightKnee.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
				if (trackedDevicePoses[i].trackedDeviceRole == TrackedDeviceRole.ROLE_RIGHTANKLE)
				{
					rightAnkle.Update(trackedDevicePoses[i]);
					sb.Clear().Append(func).Append(rightAnkle.trackedDeviceRole.Name()).Append(", poseState: ").Append(rightAnkle.poseState)
						.Append(", position (").Append(rightAnkle.translation.x.ToString("N3")).Append(", ").Append(rightAnkle.translation.y.ToString("N3")).Append(", ").Append(rightAnkle.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(rightAnkle.rotation.x.ToString("N3")).Append(", ").Append(rightAnkle.rotation.y.ToString("N3")).Append(", ").Append(rightAnkle.rotation.z.ToString("N3")).Append(", ").Append(rightAnkle.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
				if (trackedDevicePoses[i].trackedDeviceRole == TrackedDeviceRole.ROLE_RIGHTFOOT)
				{
					rightFoot.Update(trackedDevicePoses[i]);
					sb.Clear().Append(func).Append(rightFoot.trackedDeviceRole.Name()).Append(", poseState: ").Append(rightFoot.poseState)
						.Append(", position (").Append(rightFoot.translation.x.ToString("N3")).Append(", ").Append(rightFoot.translation.y.ToString("N3")).Append(", ").Append(rightFoot.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(rightFoot.rotation.x.ToString("N3")).Append(", ").Append(rightFoot.rotation.y.ToString("N3")).Append(", ").Append(rightFoot.rotation.z.ToString("N3")).Append(", ").Append(rightFoot.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
			}
		}

		private void ResetPose()
		{
			hip.poseState = PoseState.NODATA;
			chest.poseState = PoseState.NODATA;
			head.poseState = PoseState.NODATA;

			leftElbow.poseState = PoseState.NODATA;
			leftWrist.poseState = PoseState.NODATA;
			leftHand.poseState = PoseState.NODATA;
			leftHandheld.poseState = PoseState.NODATA;

			rightElbow.poseState = PoseState.NODATA;
			rightWrist.poseState = PoseState.NODATA;
			rightHand.poseState = PoseState.NODATA;
			rightHandheld.poseState = PoseState.NODATA;

			leftKnee.poseState = PoseState.NODATA;
			leftAnkle.poseState = PoseState.NODATA;
			leftFoot.poseState = PoseState.NODATA;

			rightKnee.poseState = PoseState.NODATA;
			rightAnkle.poseState = PoseState.NODATA;
			rightFoot.poseState = PoseState.NODATA;
		}
		[Obsolete("This function is deprecated, please use Clear(BodyTrackingMode) instead.")]
		public void Clear()
		{
			ResetPose();
		}
		public void Clear(BodyTrackingMode mode)
		{
			trackingMode = mode;
			ResetPose();
		}

		private int ikFrame = -1;
		private BodyPoseRole m_IKRoles = BodyPoseRole.Unknown;

		private Dictionary<BodyTrackingMode, TrackedDevicePose[]> s_TrackedDevicePose = new Dictionary<BodyTrackingMode, TrackedDevicePose[]>
		{
			{ BodyTrackingMode.ARMIK, new TrackedDevicePose[3] },
			{ BodyTrackingMode.UPPERBODYIK, new TrackedDevicePose[4] },
			{ BodyTrackingMode.FULLBODYIK, new TrackedDevicePose[6] },
			{ BodyTrackingMode.UPPERIKANDLEGFK, new TrackedDevicePose[8] },
		};
		public bool GetTrackedDevicePoses(bool init, out TrackedDevicePose[] trackedDevicePoses, out UInt32 trackedDevicePoseCount)
		{
			string func = "GetTrackedDevicePoses() ";
			if (init)
			{
				sb.Clear().Append(func).Append(trackingMode.Name());
				DEBUG(sb);
			}

			trackedDevicePoses = null;
			trackedDevicePoseCount = 0;
			if (!s_TrackedDevicePose.ContainsKey(trackingMode)) { return false; }

			// head
			if (head.poseState == PoseState.NODATA) { return false; }
			s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].Update(head);
			if (init)
			{
				sb.Clear().Append(func)
					.Append("Add ").Append(trackedDevicePoseCount).Append(".head with poseState ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].poseState)
					.Append(", position (").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].translation.x.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].translation.y.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].translation.z.ToString("N3")).Append(")")
					.Append(", rotation (").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.x.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.y.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.z.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.w.ToString("N3")).Append(")");
				DEBUG(sb);
			}

			// left wrist or handheld or hand
			trackedDevicePoseCount++;
			if (leftWrist.poseState != PoseState.NODATA)
			{
				s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].Update(leftWrist);
				if (init)
				{
					sb.Clear().Append(func)
						.Append("Add ").Append(trackedDevicePoseCount).Append(".leftWrist with poseState ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].poseState)
						.Append(", position (").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].translation.x.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].translation.y.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.x.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.y.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.z.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
			}
			else if (leftHandheld.poseState != PoseState.NODATA)
			{
				s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].Update(leftHandheld);
				if (init)
				{
					sb.Clear().Append(func)
						.Append("Add ").Append(trackedDevicePoseCount).Append(".leftHandheld with poseState ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].poseState)
						.Append(", position (").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].translation.x.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].translation.y.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.x.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.y.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.z.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
			}
			else
			{
				// allows hand to be NODATA.
				s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].Update(leftHand);
				if (init)
				{
					sb.Clear().Append(func)
						.Append("Add ").Append(trackedDevicePoseCount).Append(".leftHand with poseState ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].poseState)
						.Append(", position (").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].translation.x.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].translation.y.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.x.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.y.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.z.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
			}
			// right wrist or handheld or hand
			trackedDevicePoseCount++;
			if (rightWrist.poseState != PoseState.NODATA)
			{
				s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].Update(rightWrist);
				if (init)
				{
					sb.Clear().Append(func)
						.Append("Add ").Append(trackedDevicePoseCount).Append(".rightWrist with poseState ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].poseState)
						.Append(", position (").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].translation.x.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].translation.y.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.x.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.y.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.z.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
			}
			else if (rightHandheld.poseState != PoseState.NODATA)
			{
				s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].Update(rightHandheld);
				if (init)
				{
					sb.Clear().Append(func)
						.Append("Add ").Append(trackedDevicePoseCount).Append(".rightHandheld with poseState ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].poseState)
						.Append(", position (").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].translation.x.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].translation.y.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.x.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.y.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.z.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
			}
			else
			{
				// allow hand to be NODATA.
				s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].Update(rightHand);
				if (init)
				{
					sb.Clear().Append(func)
						.Append("Add ").Append(trackedDevicePoseCount).Append(".rightHand with poseState ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].poseState)
						.Append(", position (").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].translation.x.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].translation.y.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.x.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.y.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.z.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
			}

			// hip
			if (trackingMode == BodyTrackingMode.UPPERBODYIK || trackingMode == BodyTrackingMode.FULLBODYIK || trackingMode == BodyTrackingMode.UPPERIKANDLEGFK)
			{
				trackedDevicePoseCount++;
				if (hip.poseState == PoseState.NODATA) { return false; }
				s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].Update(hip);
				if (init)
				{
					sb.Clear().Append(func)
						.Append("Add ").Append(trackedDevicePoseCount).Append(".hip with poseState ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].poseState)
						.Append(", position (").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].translation.x.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].translation.y.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.x.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.y.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.z.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
			}

			// ankle or foot
			if (trackingMode == BodyTrackingMode.FULLBODYIK)
			{
				if (leftAnkle.poseState != PoseState.NODATA)
				{
					trackedDevicePoseCount++;
					s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].Update(leftAnkle);
					if (init)
					{
						sb.Clear().Append(func)
							.Append("Add ").Append(trackedDevicePoseCount).Append(".leftAnkle with poseState ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].poseState)
							.Append(", position (").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].translation.x.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].translation.y.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].translation.z.ToString("N3")).Append(")")
							.Append(", rotation (").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.x.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.y.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.z.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.w.ToString("N3")).Append(")");
						DEBUG(sb);
					}
				}
				else
				{
					if (leftFoot.poseState == PoseState.NODATA) { return false; }
					trackedDevicePoseCount++;
					s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].Update(leftFoot);
					if (init)
					{
						sb.Clear().Append(func)
							.Append("Add ").Append(trackedDevicePoseCount).Append(".leftFoot with poseState ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].poseState)
							.Append(", position (").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].translation.x.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].translation.y.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].translation.z.ToString("N3")).Append(")")
							.Append(", rotation (").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.x.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.y.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.z.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.w.ToString("N3")).Append(")");
						DEBUG(sb);
					}
				}

				if (rightAnkle.poseState != PoseState.NODATA)
				{
					trackedDevicePoseCount++;
					s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].Update(rightAnkle);
					if (init)
					{
						sb.Clear().Append(func)
							.Append("Add ").Append(trackedDevicePoseCount).Append(".rightAnkle with poseState ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].poseState)
							.Append(", position (").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].translation.x.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].translation.y.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].translation.z.ToString("N3")).Append(")")
							.Append(", rotation (").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.x.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.y.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.z.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.w.ToString("N3")).Append(")");
						DEBUG(sb);
					}
				}
				else
				{
					if (rightFoot.poseState == PoseState.NODATA) { return false; }
					trackedDevicePoseCount++;
					s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].Update(rightFoot);
					if (init)
					{
						sb.Clear().Append(func)
							.Append("Add ").Append(trackedDevicePoseCount).Append(".rightFoot with poseState ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].poseState)
							.Append(", position (").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].translation.x.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].translation.y.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].translation.z.ToString("N3")).Append(")")
							.Append(", rotation (").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.x.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.y.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.z.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.w.ToString("N3")).Append(")");
						DEBUG(sb);
					}
				}
			}

			// knee and ankle
			if (trackingMode == BodyTrackingMode.UPPERIKANDLEGFK)
			{
				if (leftKnee.poseState == PoseState.NODATA) { return false; }
				trackedDevicePoseCount++;
				s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].Update(leftKnee);
				if (init)
				{
					sb.Clear().Append(func).Append("Add ").Append(trackedDevicePoseCount).Append(".leftKnee with poseState ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].poseState)
						.Append(", position (").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].translation.x.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].translation.y.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.x.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.y.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.z.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}

				if (rightKnee.poseState == PoseState.NODATA) { return false; }
				trackedDevicePoseCount++;
				s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].Update(rightKnee);
				if (init)
				{
					sb.Clear().Append(func).Append("Add ").Append(trackedDevicePoseCount).Append(".rightKnee with poseState ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].poseState)
						.Append(", position (").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].translation.x.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].translation.y.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.x.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.y.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.z.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}

				if (leftAnkle.poseState == PoseState.NODATA) { return false; }
				trackedDevicePoseCount++;
				s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].Update(leftAnkle);
				if (init)
				{
					sb.Clear().Append(func).Append("Add ").Append(trackedDevicePoseCount).Append(".leftAnkle with poseState ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].poseState)
						.Append(", position (").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].translation.x.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].translation.y.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.x.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.y.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.z.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}

				if (rightAnkle.poseState == PoseState.NODATA) { return false; }
				trackedDevicePoseCount++;
				s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].Update(rightAnkle);
				if (init)
				{
					sb.Clear().Append(func).Append("Add ").Append(trackedDevicePoseCount).Append(".rightAnkle with poseState ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].poseState)
						.Append(", position (").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].translation.x.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].translation.y.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.x.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.y.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.z.ToString("N3")).Append(", ").Append(s_TrackedDevicePose[trackingMode][trackedDevicePoseCount].rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
			}

			trackedDevicePoses = s_TrackedDevicePose[trackingMode];
			trackedDevicePoseCount = (UInt32)(s_TrackedDevicePose[trackingMode].Length & 0x7FFFFFFF);
			return true;
		}
		public BodyPoseRole GetIKRoles(bool init)
		{
			if (init)
			{
				sb.Clear().Append("GetIKRoles() ").Append(trackingMode.Name()).Append(", origin: ").Append(m_IKRoles.Name());
				DEBUG(sb);
			}

			if (ikFrame == Time.frameCount) { return m_IKRoles; }
			else { m_IKRoles = BodyPoseRole.Unknown; ikFrame = Time.frameCount; }

			if (GetTrackedDevicePoses(init, out TrackedDevicePose[] trackedDevicePoses, out UInt32 trackedDevicePoseCount))
				m_IKRoles = BodyTrackingUtils.GetBodyPoseRole(trackedDevicePoses, trackedDevicePoseCount);

			if (init)
			{
				sb.Clear().Append("GetIKRoles() ").Append(trackingMode.Name()).Append(", new: ").Append(m_IKRoles.Name());
				DEBUG(sb);
			}

			return m_IKRoles;
		}

		// The Self Trackers used in Body Tracking Mode.
		private readonly Dictionary<BodyTrackingMode, List<TrackedDeviceRole>> s_SelfTrackerRoles = new Dictionary<BodyTrackingMode, List<TrackedDeviceRole>>()
		{
			// Arm uses tracker roles: wrist
			{ BodyTrackingMode.ARMIK, new List<TrackedDeviceRole>() {
				TrackedDeviceRole.ROLE_LEFTWRIST, TrackedDeviceRole.ROLE_RIGHTWRIST }
			},
			// Upper Body uses self tracker roles: wrist, waist
			{ BodyTrackingMode.UPPERBODYIK, new List<TrackedDeviceRole>() {
				TrackedDeviceRole.ROLE_LEFTWRIST, TrackedDeviceRole.ROLE_RIGHTWRIST, TrackedDeviceRole.ROLE_HIP }
			},
			// Full Body uses self tracker roles: wrist, waist, ankle, foot
			{ BodyTrackingMode.FULLBODYIK, new List<TrackedDeviceRole>() {
				TrackedDeviceRole.ROLE_LEFTWRIST, TrackedDeviceRole.ROLE_RIGHTWRIST, TrackedDeviceRole.ROLE_HIP,
				TrackedDeviceRole.ROLE_LEFTANKLE, TrackedDeviceRole.ROLE_RIGHTANKLE, TrackedDeviceRole.ROLE_LEFTFOOT, TrackedDeviceRole.ROLE_RIGHTFOOT }
			},
			// Upper Body + Leg FK uses self tracker roles: waist
			{ BodyTrackingMode.UPPERIKANDLEGFK, new List<TrackedDeviceRole>() {
				TrackedDeviceRole.ROLE_HIP }
			},
		};
		// The Wrist Trackers used in Body Tracking Mode.
		private readonly Dictionary<BodyTrackingMode, List<TrackedDeviceRole>> s_WristTrackerRoles = new Dictionary<BodyTrackingMode, List<TrackedDeviceRole>>()
		{
			// Arm uses wrist tracker roles: wrist
			{ BodyTrackingMode.ARMIK, new List<TrackedDeviceRole>() {
				TrackedDeviceRole.ROLE_LEFTWRIST, TrackedDeviceRole.ROLE_RIGHTWRIST }
			},
			// Upper Body uses wrist tracker roles: wrist
			{ BodyTrackingMode.UPPERBODYIK, new List<TrackedDeviceRole>() {
				TrackedDeviceRole.ROLE_LEFTWRIST, TrackedDeviceRole.ROLE_RIGHTWRIST }
			},
			// Full Body uses wrist tracker roles: wrist
			{ BodyTrackingMode.FULLBODYIK, new List<TrackedDeviceRole>() {
				TrackedDeviceRole.ROLE_LEFTWRIST, TrackedDeviceRole.ROLE_RIGHTWRIST }
			},
			// Upper Body + Leg FK uses self tracker roles: waist
			{ BodyTrackingMode.UPPERIKANDLEGFK, new List<TrackedDeviceRole>() {
				TrackedDeviceRole.ROLE_LEFTWRIST, TrackedDeviceRole.ROLE_RIGHTWRIST }
			},
		};
		// The Self IM Trackers used in Body Tracking Mode.
		private readonly Dictionary<BodyTrackingMode, List<TrackedDeviceRole>> s_SelfIMTrackerRoles = new Dictionary<BodyTrackingMode, List<TrackedDeviceRole>>()
		{
			// Only Upper Body + Leg FK uses self im trackers: knee, ankle
			{ BodyTrackingMode.UPPERIKANDLEGFK, new List<TrackedDeviceRole>() {
				TrackedDeviceRole.ROLE_LEFTKNEE, TrackedDeviceRole.ROLE_RIGHTKNEE, TrackedDeviceRole.ROLE_LEFTANKLE, TrackedDeviceRole.ROLE_RIGHTANKLE }
			}
		};

		/// Initialize the Standard Pose (w/o Redirective Poses)
		internal BodyTrackingResult UpdatePoseInContent(bool init = false)
		{
			if (s_TrackedDeviceTypes == null) { return BodyTrackingResult.ERROR_NOT_CALIBRATED; }
			string func = "UpdatePoseInContent() ";

			ResetPose();
			if (s_TrackedDeviceTypes[(Int32)TrackedDeviceRole.ROLE_HEAD] != TrackedDeviceType.Invalid)
			{
				head.Update(BodyRoleData.GetRoleData(TrackedDeviceRole.ROLE_HEAD).devicePose);
				if (init)
				{
					sb.Clear().Append(func).Append("head poseState: ").Append(head.poseState)
						.Append(", position (").Append(head.translation.x.ToString("N3")).Append(", ").Append(head.translation.y.ToString("N3")).Append(", ").Append(head.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(head.rotation.x.ToString("N3")).Append(", ").Append(head.rotation.y.ToString("N3")).Append(", ").Append(head.rotation.z.ToString("N3")).Append(", ").Append(head.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
			}

			if (s_TrackedDeviceTypes[(Int32)TrackedDeviceRole.ROLE_LEFTHANDHELD] != TrackedDeviceType.Invalid ||
				s_TrackedDeviceTypes[(Int32)TrackedDeviceRole.ROLE_LEFTHAND] != TrackedDeviceType.Invalid)
			{
				leftHandheld.Update(BodyRoleData.GetRoleData(TrackedDeviceRole.ROLE_LEFTHANDHELD).devicePose);
				if (init)
				{
					sb.Clear().Append(func).Append("leftHandheld poseState: ").Append(leftHandheld.poseState)
						.Append(", position (").Append(leftHandheld.translation.x.ToString("N3")).Append(", ").Append(leftHandheld.translation.y.ToString("N3")).Append(", ").Append(leftHandheld.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(leftHandheld.rotation.x.ToString("N3")).Append(", ").Append(leftHandheld.rotation.y.ToString("N3")).Append(", ").Append(leftHandheld.rotation.z.ToString("N3")).Append(", ").Append(leftHandheld.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
			}
			if (leftHandheld.poseState == PoseState.NODATA)
			{
				leftHand.Update(BodyRoleData.GetRoleData(TrackedDeviceRole.ROLE_LEFTHAND).devicePose);
				if (init)
				{
					sb.Clear().Append(func).Append("leftHand poseState: ").Append(leftHand.poseState)
						.Append(", position (").Append(leftHand.translation.x.ToString("N3")).Append(", ").Append(leftHand.translation.y.ToString("N3")).Append(", ").Append(leftHand.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(leftHand.rotation.x.ToString("N3")).Append(", ").Append(leftHand.rotation.y.ToString("N3")).Append(", ").Append(leftHand.rotation.z.ToString("N3")).Append(", ").Append(leftHand.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
			}

			if (s_TrackedDeviceTypes[(Int32)TrackedDeviceRole.ROLE_RIGHTHANDHELD] != TrackedDeviceType.Invalid ||
				s_TrackedDeviceTypes[(Int32)TrackedDeviceRole.ROLE_RIGHTHAND] != TrackedDeviceType.Invalid)
			{
				rightHandheld.Update(BodyRoleData.GetRoleData(TrackedDeviceRole.ROLE_RIGHTHANDHELD).devicePose);
				if (init)
				{
					sb.Clear().Append(func).Append("rightHandheld poseState: ").Append(rightHandheld.poseState)
						.Append(", position (").Append(rightHandheld.translation.x.ToString("N3")).Append(", ").Append(rightHandheld.translation.y.ToString("N3")).Append(", ").Append(rightHandheld.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(rightHandheld.rotation.x.ToString("N3")).Append(", ").Append(rightHandheld.rotation.y.ToString("N3")).Append(", ").Append(rightHandheld.rotation.z.ToString("N3")).Append(", ").Append(rightHandheld.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
			}
			if (rightHandheld.poseState == PoseState.NODATA)
			{
				rightHand.Update(BodyRoleData.GetRoleData(TrackedDeviceRole.ROLE_RIGHTHAND).devicePose);
				if (init)
				{
					sb.Clear().Append(func).Append("rightHand poseState: ").Append(rightHand.poseState)
						.Append(", position (").Append(rightHand.translation.x.ToString("N3")).Append(", ").Append(rightHand.translation.y.ToString("N3")).Append(", ").Append(rightHand.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(rightHand.rotation.x.ToString("N3")).Append(", ").Append(rightHand.rotation.y.ToString("N3")).Append(", ").Append(rightHand.rotation.z.ToString("N3")).Append(", ").Append(rightHand.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
			}

			if (s_TrackedDeviceTypes[(Int32)TrackedDeviceRole.ROLE_LEFTWRIST] != TrackedDeviceType.Invalid)
			{
				leftWrist.Update(BodyRoleData.GetRoleData(TrackedDeviceRole.ROLE_LEFTWRIST).devicePose);
				if (init)
				{
					sb.Clear().Append(func).Append("leftWrist poseState: ").Append(leftWrist.poseState)
						.Append(", position (").Append(leftWrist.translation.x.ToString("N3")).Append(", ").Append(leftWrist.translation.y.ToString("N3")).Append(", ").Append(leftWrist.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(leftWrist.rotation.x.ToString("N3")).Append(", ").Append(leftWrist.rotation.y.ToString("N3")).Append(", ").Append(leftWrist.rotation.z.ToString("N3")).Append(", ").Append(leftWrist.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
			}
			if (s_TrackedDeviceTypes[(Int32)TrackedDeviceRole.ROLE_RIGHTWRIST] != TrackedDeviceType.Invalid)
			{
				rightWrist.Update(BodyRoleData.GetRoleData(TrackedDeviceRole.ROLE_RIGHTWRIST).devicePose);
				if (init)
				{
					sb.Clear().Append(func).Append("rightWrist poseState: ").Append(rightWrist.poseState)
						.Append(", position (").Append(rightWrist.translation.x.ToString("N3")).Append(", ").Append(rightWrist.translation.y.ToString("N3")).Append(", ").Append(rightWrist.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(rightWrist.rotation.x.ToString("N3")).Append(", ").Append(rightWrist.rotation.y.ToString("N3")).Append(", ").Append(rightWrist.rotation.z.ToString("N3")).Append(", ").Append(rightWrist.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
			}
			if (s_TrackedDeviceTypes[(Int32)TrackedDeviceRole.ROLE_HIP] != TrackedDeviceType.Invalid)
			{
				hip.Update(BodyRoleData.GetRoleData(TrackedDeviceRole.ROLE_HIP).devicePose);
				if (init)
				{
					sb.Clear().Append(func).Append("hip poseState: ").Append(hip.poseState)
						.Append(", position (").Append(hip.translation.x.ToString("N3")).Append(", ").Append(hip.translation.y.ToString("N3")).Append(", ").Append(hip.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(hip.rotation.x.ToString("N3")).Append(", ").Append(hip.rotation.y.ToString("N3")).Append(", ").Append(hip.rotation.z.ToString("N3")).Append(", ").Append(hip.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
			}
			if (s_TrackedDeviceTypes[(Int32)TrackedDeviceRole.ROLE_LEFTANKLE] != TrackedDeviceType.Invalid)
			{
				leftAnkle.Update(BodyRoleData.GetRoleData(TrackedDeviceRole.ROLE_LEFTANKLE).devicePose);
				if (init)
				{
					sb.Clear().Append(func).Append("leftAnkle poseState: ").Append(leftAnkle.poseState)
						.Append(", position (").Append(leftAnkle.translation.x.ToString("N3")).Append(", ").Append(leftAnkle.translation.y.ToString("N3")).Append(", ").Append(leftAnkle.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(leftAnkle.rotation.x.ToString("N3")).Append(", ").Append(leftAnkle.rotation.y.ToString("N3")).Append(", ").Append(leftAnkle.rotation.z.ToString("N3")).Append(", ").Append(leftAnkle.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
			}
			if (s_TrackedDeviceTypes[(Int32)TrackedDeviceRole.ROLE_RIGHTANKLE] != TrackedDeviceType.Invalid)
			{
				rightAnkle.Update(BodyRoleData.GetRoleData(TrackedDeviceRole.ROLE_RIGHTANKLE).devicePose);
				if (init)
				{
					sb.Clear().Append(func).Append("rightAnkle poseState: ").Append(rightAnkle.poseState)
						.Append(", position (").Append(rightAnkle.translation.x.ToString("N3")).Append(", ").Append(rightAnkle.translation.y.ToString("N3")).Append(", ").Append(rightAnkle.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(rightAnkle.rotation.x.ToString("N3")).Append(", ").Append(rightAnkle.rotation.y.ToString("N3")).Append(", ").Append(rightAnkle.rotation.z.ToString("N3")).Append(", ").Append(rightAnkle.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
			}
			if (s_TrackedDeviceTypes[(Int32)TrackedDeviceRole.ROLE_LEFTFOOT] != TrackedDeviceType.Invalid)
			{
				leftFoot.Update(BodyRoleData.GetRoleData(TrackedDeviceRole.ROLE_LEFTFOOT).devicePose);
				if (init)
				{
					sb.Clear().Append(func).Append("leftFoot poseState: ").Append(leftFoot.poseState)
						.Append(", position (").Append(leftFoot.translation.x.ToString("N3")).Append(", ").Append(leftFoot.translation.y.ToString("N3")).Append(", ").Append(leftFoot.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(leftFoot.rotation.x.ToString("N3")).Append(", ").Append(leftFoot.rotation.y.ToString("N3")).Append(", ").Append(leftFoot.rotation.z.ToString("N3")).Append(", ").Append(leftFoot.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
			}
			if (s_TrackedDeviceTypes[(Int32)TrackedDeviceRole.ROLE_RIGHTFOOT] != TrackedDeviceType.Invalid)
			{
				rightFoot.Update(BodyRoleData.GetRoleData(TrackedDeviceRole.ROLE_RIGHTFOOT).devicePose);
				if (init)
				{
					sb.Clear().Append(func).Append("rightFoot poseState: ").Append(rightFoot.poseState)
						.Append(", position (").Append(rightFoot.translation.x.ToString("N3")).Append(", ").Append(rightFoot.translation.y.ToString("N3")).Append(", ").Append(rightFoot.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(rightFoot.rotation.x.ToString("N3")).Append(", ").Append(rightFoot.rotation.y.ToString("N3")).Append(", ").Append(rightFoot.rotation.z.ToString("N3")).Append(", ").Append(rightFoot.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
			}
			if (s_TrackedDeviceTypes[(Int32)TrackedDeviceRole.ROLE_LEFTKNEE] != TrackedDeviceType.Invalid)
			{
				leftKnee.Update(BodyRoleData.GetRoleData(TrackedDeviceRole.ROLE_LEFTKNEE).devicePose);
				if (init)
				{
					sb.Clear().Append(func).Append("leftKnee poseState: ").Append(leftKnee.poseState)
						.Append(", position (").Append(leftKnee.translation.x.ToString("N3")).Append(", ").Append(leftKnee.translation.y.ToString("N3")).Append(", ").Append(leftKnee.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(leftKnee.rotation.x.ToString("N3")).Append(", ").Append(leftKnee.rotation.y.ToString("N3")).Append(", ").Append(leftKnee.rotation.z.ToString("N3")).Append(", ").Append(leftKnee.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
			}
			if (s_TrackedDeviceTypes[(Int32)TrackedDeviceRole.ROLE_RIGHTKNEE] != TrackedDeviceType.Invalid)
			{
				rightKnee.Update(BodyRoleData.GetRoleData(TrackedDeviceRole.ROLE_RIGHTKNEE).devicePose);
				if (init)
				{
					sb.Clear().Append(func).Append("rightKnee poseState: ").Append(rightKnee.poseState)
						.Append(", position (").Append(rightKnee.translation.x.ToString("N3")).Append(", ").Append(rightKnee.translation.y.ToString("N3")).Append(", ").Append(rightKnee.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(rightKnee.rotation.x.ToString("N3")).Append(", ").Append(rightKnee.rotation.y.ToString("N3")).Append(", ").Append(rightKnee.rotation.z.ToString("N3")).Append(", ").Append(rightKnee.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
			}

			return BodyTrackingResult.SUCCESS;
		}
		internal BodyTrackingResult InitPoseInContent(BodyTrackingMode mode)
		{
			string func = "InitPoseInContent() ";
			ResetTrackedDeviceTypes();

			// Checks Head
			Vector3 position = Vector3.zero;
			if (Rdp.Head.IsTracked())
			{
				s_TrackedDeviceTypes[(Int32)TrackedDeviceRole.ROLE_HEAD] = TrackedDeviceType.HMD;
				if (Rdp.Head.GetPosition(ref position)) { userCalibrationHeight = position.y; }
				sb.Clear().Append(func).Append("add HMD").Append(", userCalibrationHeight: ").Append(userCalibrationHeight); DEBUG(sb);
			}

			bool hasLeftHand = false, hasRightHand = false;
			bool hasLeftAnkle = false, hasRightAnkle = false;

			// Checks Tracker first.
			for (int i = 0; i < Rdp.Tracker.s_TrackerIds.Length; i++)
			{
				Rdp.Tracker.Id id = Rdp.Tracker.s_TrackerIds[i];
				if (!Rdp.Tracker.IsTracked(id)) { continue; }

				TrackedDeviceRole role = (TrackedDeviceRole)Rdp.Tracker.GetTrackerRole(id);
				TrackedDeviceType btType = TrackedDeviceType.ViveSelfTracker;
				if (role == TrackedDeviceRole.ROLE_UNDEFINED) { continue; }

				if (role == TrackedDeviceRole.ROLE_LEFTWRIST)
				{
					if (hasLeftHand) { continue; }
					hasLeftHand = true;
				}
				if (role == TrackedDeviceRole.ROLE_RIGHTWRIST)
				{
					if (hasRightHand) { continue; }
					hasRightHand = true;
				}
				if (role == TrackedDeviceRole.ROLE_LEFTANKLE)
				{
					if (hasLeftAnkle) { continue; }
					hasLeftAnkle = true;
				}
				if (role == TrackedDeviceRole.ROLE_RIGHTANKLE)
				{
					if (hasRightAnkle) { continue; }
					hasRightAnkle = true;
				}

				s_TrackedDeviceTypes[(Int32)role] = btType;
				sb.Clear().Append(func).Append("add ").Append(role.Name()).Append(" ").Append(btType.Name()).Append(" ").Append(id.Name()); DEBUG(sb);
			}
			if (!hasLeftHand || !hasRightHand)
			{
				hasLeftHand = false;
				hasRightHand = false;
			}

			// Checks Controller next.
			if (!hasLeftHand && Rdp.Controller.IsTracked(true))
			{
				hasLeftHand = true;

				s_TrackedDeviceTypes[(Int32)TrackedDeviceRole.ROLE_LEFTHANDHELD] = TrackedDeviceType.Controller;
				sb.Clear().Append(func).Append("add Left Controller"); DEBUG(sb);
			}
			if (!hasRightHand && Rdp.Controller.IsTracked(false))
			{
				hasRightHand = true;

				s_TrackedDeviceTypes[(Int32)TrackedDeviceRole.ROLE_RIGHTHANDHELD] = TrackedDeviceType.Controller;
				sb.Clear().Append(func).Append("add Right Controller"); DEBUG(sb);
			}

			// Force using Hand if no Tracker & Controller.
			if (!hasLeftHand)
			{
				s_TrackedDeviceTypes[(Int32)TrackedDeviceRole.ROLE_LEFTHAND] = TrackedDeviceType.Hand;
				sb.Clear().Append(func).Append("FORCE add Left Hand"); DEBUG(sb);
			}
			if (!hasRightHand)
			{
				s_TrackedDeviceTypes[(Int32)TrackedDeviceRole.ROLE_RIGHTHAND] = TrackedDeviceType.Hand;
				sb.Clear().Append(func).Append("FORCE add Right Hand"); DEBUG(sb);
			}

			BodyTrackingResult result = UpdatePoseInContent(true);
			if (result != BodyTrackingResult.SUCCESS)
			{
				sb.Clear().Append(func).Append("UpdatePoseInContent failed, result: ").Append(result.Name());
				return result;
			}

			BodyPoseRole ikRoles = GetIKRoles(true);
			sb.Clear().Append(func).Append("ikRoles: ").Append(ikRoles.Name()); DEBUG(sb);

			return (ikRoles == BodyPoseRole.Unknown ? BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID : BodyTrackingResult.SUCCESS);
		}

#if WAVE_BODY_CALIBRATION
		internal BodyTrackingResult InitPoseFromRuntime(float height, [In] WVR_BodyTracking_DeviceInfo_t[] info, UInt32 count)
		{
			string func = "InitPoseFromRuntime() ";
			if (info == null || info.Length != count)
			{
				sb.Clear().Append(func).Append("Incorrect WVR_BodyTracking_DeviceInfo_t array size "); ERROR(sb);
				return BodyTrackingResult.ERROR_INVALID_ARGUMENT;
			}

			userCalibrationHeight = height;
			sb.Clear().Append(func).Append("height: ").Append(userCalibrationHeight).Append(", count: ").Append(count); DEBUG(sb);
			for (int i = 0; i < count; i++)
			{
				TrackedDeviceRole role = info[i].role.FromRdp();
				s_TrackedDeviceTypes[(Int32)role] = info[i].type.FromRdp(info[i].trackerId);

				if (role == TrackedDeviceRole.ROLE_HIP)
				{
					Rdp.Update(ref hip, info[i].role, info[i].standardPose);
					sb.Clear().Append(func).Append("hip poseState: ").Append(hip.poseState)
						.Append(", position (").Append(hip.translation.x.ToString("N3")).Append(", ").Append(hip.translation.y.ToString("N3")).Append(", ").Append(hip.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(hip.rotation.x.ToString("N3")).Append(", ").Append(hip.rotation.y.ToString("N3")).Append(", ").Append(hip.rotation.z.ToString("N3")).Append(", ").Append(hip.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
				if (role == TrackedDeviceRole.ROLE_CHEST)
				{
					Rdp.Update(ref chest, info[i].role, info[i].standardPose);
					sb.Clear().Append(func).Append("chest poseState: ").Append(chest.poseState)
						.Append(", position (").Append(chest.translation.x.ToString("N3")).Append(", ").Append(chest.translation.y.ToString("N3")).Append(", ").Append(chest.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(chest.rotation.x.ToString("N3")).Append(", ").Append(chest.rotation.y.ToString("N3")).Append(", ").Append(chest.rotation.z.ToString("N3")).Append(", ").Append(chest.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
				if (role == TrackedDeviceRole.ROLE_HEAD)
				{
					Rdp.Update(ref head, info[i].role, info[i].standardPose);
					sb.Clear().Append(func).Append("head poseState: ").Append(head.poseState)
						.Append(", position (").Append(head.translation.x.ToString("N3")).Append(", ").Append(head.translation.y.ToString("N3")).Append(", ").Append(head.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(head.rotation.x.ToString("N3")).Append(", ").Append(head.rotation.y.ToString("N3")).Append(", ").Append(head.rotation.z.ToString("N3")).Append(", ").Append(head.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}

				if (role == TrackedDeviceRole.ROLE_LEFTELBOW)
				{
					Rdp.Update(ref leftElbow, info[i].role, info[i].standardPose);
					sb.Clear().Append(func).Append("leftElbow poseState: ").Append(leftElbow.poseState)
						.Append(", position (").Append(leftElbow.translation.x.ToString("N3")).Append(", ").Append(leftElbow.translation.y.ToString("N3")).Append(", ").Append(leftElbow.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(leftElbow.rotation.x.ToString("N3")).Append(", ").Append(leftElbow.rotation.y.ToString("N3")).Append(", ").Append(leftElbow.rotation.z.ToString("N3")).Append(", ").Append(leftElbow.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
				if (role == TrackedDeviceRole.ROLE_LEFTWRIST)
				{
					Rdp.Update(ref leftWrist, info[i].role, info[i].standardPose);
					sb.Clear().Append(func).Append("leftWrist poseState: ").Append(leftWrist.poseState)
						.Append(", position (").Append(leftWrist.translation.x.ToString("N3")).Append(", ").Append(leftWrist.translation.y.ToString("N3")).Append(", ").Append(leftWrist.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(leftWrist.rotation.x.ToString("N3")).Append(", ").Append(leftWrist.rotation.y.ToString("N3")).Append(", ").Append(leftWrist.rotation.z.ToString("N3")).Append(", ").Append(leftWrist.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
				if (role == TrackedDeviceRole.ROLE_LEFTHAND)
				{
					Rdp.Update(ref leftHand, info[i].role, info[i].standardPose);
					sb.Clear().Append(func).Append("leftHand poseState: ").Append(leftHand.poseState)
						.Append(", position (").Append(leftHand.translation.x.ToString("N3")).Append(", ").Append(leftHand.translation.y.ToString("N3")).Append(", ").Append(leftHand.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(leftHand.rotation.x.ToString("N3")).Append(", ").Append(leftHand.rotation.y.ToString("N3")).Append(", ").Append(leftHand.rotation.z.ToString("N3")).Append(", ").Append(leftHand.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
				if (role == TrackedDeviceRole.ROLE_LEFTHANDHELD)
				{
					Rdp.Update(ref leftHandheld, info[i].role, info[i].standardPose);
					sb.Clear().Append(func).Append("leftHandheld poseState: ").Append(leftHandheld.poseState)
						.Append(", position (").Append(leftHandheld.translation.x.ToString("N3")).Append(", ").Append(leftHandheld.translation.y.ToString("N3")).Append(", ").Append(leftHandheld.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(leftHandheld.rotation.x.ToString("N3")).Append(", ").Append(leftHandheld.rotation.y.ToString("N3")).Append(", ").Append(leftHandheld.rotation.z.ToString("N3")).Append(", ").Append(leftHandheld.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}

				if (role == TrackedDeviceRole.ROLE_RIGHTELBOW)
				{
					Rdp.Update(ref rightElbow, info[i].role, info[i].standardPose);
					sb.Clear().Append(func).Append("rightElbow poseState: ").Append(rightElbow.poseState)
						.Append(", position (").Append(rightElbow.translation.x.ToString("N3")).Append(", ").Append(rightElbow.translation.y.ToString("N3")).Append(", ").Append(rightElbow.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(rightElbow.rotation.x.ToString("N3")).Append(", ").Append(rightElbow.rotation.y.ToString("N3")).Append(", ").Append(rightElbow.rotation.z.ToString("N3")).Append(", ").Append(rightElbow.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
				if (role == TrackedDeviceRole.ROLE_RIGHTWRIST)
				{
					Rdp.Update(ref rightWrist, info[i].role, info[i].standardPose);
					sb.Clear().Append(func).Append("rightWrist poseState: ").Append(rightWrist.poseState)
						.Append(", position (").Append(rightWrist.translation.x.ToString("N3")).Append(", ").Append(rightWrist.translation.y.ToString("N3")).Append(", ").Append(rightWrist.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(rightWrist.rotation.x.ToString("N3")).Append(", ").Append(rightWrist.rotation.y.ToString("N3")).Append(", ").Append(rightWrist.rotation.z.ToString("N3")).Append(", ").Append(rightWrist.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
				if (role == TrackedDeviceRole.ROLE_RIGHTHAND)
				{
					Rdp.Update(ref rightHand, info[i].role, info[i].standardPose);
					sb.Clear().Append(func).Append("rightHand poseState: ").Append(rightHand.poseState)
						.Append(", position (").Append(rightHand.translation.x.ToString("N3")).Append(", ").Append(rightHand.translation.y.ToString("N3")).Append(", ").Append(rightHand.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(rightHand.rotation.x.ToString("N3")).Append(", ").Append(rightHand.rotation.y.ToString("N3")).Append(", ").Append(rightHand.rotation.z.ToString("N3")).Append(", ").Append(rightHand.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
				if (role == TrackedDeviceRole.ROLE_RIGHTHANDHELD)
				{
					Rdp.Update(ref rightHandheld, info[i].role, info[i].standardPose);
					sb.Clear().Append(func).Append("rightHandheld poseState: ").Append(rightHandheld.poseState)
						.Append(", position (").Append(rightHandheld.translation.x.ToString("N3")).Append(", ").Append(rightHandheld.translation.y.ToString("N3")).Append(", ").Append(rightHandheld.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(rightHandheld.rotation.x.ToString("N3")).Append(", ").Append(rightHandheld.rotation.y.ToString("N3")).Append(", ").Append(rightHandheld.rotation.z.ToString("N3")).Append(", ").Append(rightHandheld.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}

				if (role == TrackedDeviceRole.ROLE_LEFTKNEE)
				{
					Rdp.Update(ref leftKnee, info[i].role, info[i].standardPose);
					sb.Clear().Append(func).Append("leftKnee poseState: ").Append(leftKnee.poseState)
						.Append(", position (").Append(leftKnee.translation.x.ToString("N3")).Append(", ").Append(leftKnee.translation.y.ToString("N3")).Append(", ").Append(leftKnee.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(leftKnee.rotation.x.ToString("N3")).Append(", ").Append(leftKnee.rotation.y.ToString("N3")).Append(", ").Append(leftKnee.rotation.z.ToString("N3")).Append(", ").Append(leftKnee.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
				if (role == TrackedDeviceRole.ROLE_LEFTANKLE)
				{
					Rdp.Update(ref leftAnkle, info[i].role, info[i].standardPose);
					sb.Clear().Append(func).Append("leftAnkle poseState: ").Append(leftAnkle.poseState)
						.Append(", position (").Append(leftAnkle.translation.x.ToString("N3")).Append(", ").Append(leftAnkle.translation.y.ToString("N3")).Append(", ").Append(leftAnkle.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(leftAnkle.rotation.x.ToString("N3")).Append(", ").Append(leftAnkle.rotation.y.ToString("N3")).Append(", ").Append(leftAnkle.rotation.z.ToString("N3")).Append(", ").Append(leftAnkle.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
				if (role == TrackedDeviceRole.ROLE_LEFTFOOT)
				{
					Rdp.Update(ref leftFoot, info[i].role, info[i].standardPose);
					sb.Clear().Append(func).Append("leftFoot poseState: ").Append(leftFoot.poseState)
						.Append(", position (").Append(leftFoot.translation.x.ToString("N3")).Append(", ").Append(leftFoot.translation.y.ToString("N3")).Append(", ").Append(leftFoot.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(leftFoot.rotation.x.ToString("N3")).Append(", ").Append(leftFoot.rotation.y.ToString("N3")).Append(", ").Append(leftFoot.rotation.z.ToString("N3")).Append(", ").Append(leftFoot.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}

				if (role == TrackedDeviceRole.ROLE_RIGHTKNEE)
				{
					Rdp.Update(ref rightKnee, info[i].role, info[i].standardPose);
					sb.Clear().Append(func).Append("rightKnee poseState: ").Append(rightKnee.poseState)
						.Append(", position (").Append(rightKnee.translation.x.ToString("N3")).Append(", ").Append(rightKnee.translation.y.ToString("N3")).Append(", ").Append(rightKnee.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(rightKnee.rotation.x.ToString("N3")).Append(", ").Append(rightKnee.rotation.y.ToString("N3")).Append(", ").Append(rightKnee.rotation.z.ToString("N3")).Append(", ").Append(rightKnee.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
				if (role == TrackedDeviceRole.ROLE_RIGHTANKLE)
				{
					Rdp.Update(ref rightAnkle, info[i].role, info[i].standardPose);
					sb.Clear().Append(func).Append("rightAnkle poseState: ").Append(rightAnkle.poseState)
						.Append(", position (").Append(rightAnkle.translation.x.ToString("N3")).Append(", ").Append(rightAnkle.translation.y.ToString("N3")).Append(", ").Append(rightAnkle.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(rightAnkle.rotation.x.ToString("N3")).Append(", ").Append(rightAnkle.rotation.y.ToString("N3")).Append(", ").Append(rightAnkle.rotation.z.ToString("N3")).Append(", ").Append(rightAnkle.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
				if (role == TrackedDeviceRole.ROLE_RIGHTFOOT)
				{
					Rdp.Update(ref rightFoot, info[i].role, info[i].standardPose);
					sb.Clear().Append(func).Append("rightFoot poseState: ").Append(rightFoot.poseState)
						.Append(", position (").Append(rightFoot.translation.x.ToString("N3")).Append(", ").Append(rightFoot.translation.y.ToString("N3")).Append(", ").Append(rightFoot.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(rightFoot.rotation.x.ToString("N3")).Append(", ").Append(rightFoot.rotation.y.ToString("N3")).Append(", ").Append(rightFoot.rotation.z.ToString("N3")).Append(", ").Append(rightFoot.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
			}

			BodyPoseRole ikRoles = GetIKRoles(true);
			sb.Clear().Append(func).Append("ikRoles: ").Append(ikRoles.Name()); DEBUG(sb);

			return (ikRoles == BodyPoseRole.Unknown ? BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID : BodyTrackingResult.SUCCESS);
		}
#endif
	}

	internal struct BodyRotationSpace_t
	{
		public RotateSpace[] spaces;
		public UInt32 count;

		public BodyRotationSpace_t(RotateSpace[] in_spaces, UInt32 in_count)
		{
			spaces = in_spaces;
			count = in_count;
		}
		public void Update(BodyRotationSpace_t in_brt)
		{
			if (count != in_brt.count && in_brt.count > 0)
			{
				count = in_brt.count;
				spaces = new RotateSpace[count];
			}
			for (UInt32 i = 0; i < count; i++)
				spaces[i] = in_brt.spaces[i];
		}
	}

	#region API v1.0.0.1
	public enum JointType : Int32
	{
		UNKNOWN = -1,
		HIP = 0,

		LEFTTHIGH = 1,
		LEFTLEG = 2,
		LEFTANKLE = 3,
		LEFTFOOT = 4,

		RIGHTTHIGH = 5,
		RIGHTLEG = 6,
		RIGHTANKLE = 7,
		RIGHTFOOT = 8,

		WAIST = 9,

		SPINELOWER = 10,
		SPINEMIDDLE = 11,
		SPINEHIGH = 12,

		CHEST = 13,
		NECK = 14,
		HEAD = 15,

		LEFTCLAVICLE = 16,
		LEFTSCAPULA = 17,
		LEFTUPPERARM = 18,
		LEFTFOREARM = 19,
		LEFTHAND = 20,

		RIGHTCLAVICLE = 21,
		RIGHTSCAPULA = 22,
		RIGHTUPPERARM = 23,
		RIGHTFOREARM = 24,
		RIGHTHAND = 25,

		NUMS_OF_JOINT,
		MAX_ENUM = 0x7fffffff
	}
	[Flags]
	public enum PoseState : UInt32
	{
		NODATA = 0,
		ROTATION = 1 << 0,
		TRANSLATION = 1 << 1
	}
	public enum BodyTrackingMode : Int32
	{
		UNKNOWNMODE = -1,
		ARMIK = 0,
		UPPERBODYIK = 1,
		FULLBODYIK = 2,

		UPPERIKANDLEGFK = 3, // controller or hand + hip tracker + leg fk
		SPINEIK = 4,    // used internal
		LEGIK = 5,    // used internal
		LEGFK = 6,    // used internal
		SPINEIKANDLEGFK = 7, // hip tracker + leg fk
		MAX = 0x7fffffff
	}
	public enum TrackedDeviceRole : Int32
	{
		ROLE_UNDEFINED = -1,
		ROLE_HIP = 0,
		ROLE_CHEST = 1,
		ROLE_HEAD = 2,

		ROLE_LEFTELBOW = 3,
		ROLE_LEFTWRIST = 4,
		ROLE_LEFTHAND = 5,
		ROLE_LEFTHANDHELD = 6,

		ROLE_RIGHTELBOW = 7,
		ROLE_RIGHTWRIST = 8,
		ROLE_RIGHTHAND = 9,
		ROLE_RIGHTHANDHELD = 10,

		ROLE_LEFTKNEE = 11,
		ROLE_LEFTANKLE = 12,
		ROLE_LEFTFOOT = 13,

		ROLE_RIGHTKNEE = 14,
		ROLE_RIGHTANKLE = 15,
		ROLE_RIGHTFOOT = 16,

		NUMS_OF_ROLE,
		ROLE_MAX = 0x7fffffff
	}
	public enum Result : Int32
	{
		SUCCESS = 0,
		ERROR_BODYTRACKINGMODE_NOT_FOUND = 100,
		ERROR_TRACKER_AMOUNT_FAILED = 101,
		ERROR_SKELETONID_NOT_FOUND = 102,
		ERROR_INPUTPOSE_NOT_VALID = 103,
		ERROR_NOT_CALIBRATED = 104,
		ERROR_BODYTRACKINGMODE_NOT_ALIGNED = 105,
		ERROR_AVATAR_INIT_FAILED = 200,
		ERROR_CALIBRATE_FAILED = 300,
		ERROR_COMPUTE_FAILED = 400,
		ERROR_TABLE_STATIC = 401,
		ERROR_SOLVER_NOT_FOUND = 402,
		ERROR_NOT_INITIALIZATION = 403,
		ERROR_JOINT_NOT_FOUND = 404,
		ERROR_FATAL_ERROR = 499,
		ERROR_MAX = 0x7fffffff,
	}
	public enum TrackerDirection : Int32
	{
		NODIRECTION = -1,
		FORWARD = 0,
		BACKWARD = 1,
		RIGHT = 2,
		LEFT = 3
	}
	public enum AvatarType : UInt32
	{
		TPOSE = 0,    // T-pose, pre-processing in SDK
		STANDARD_VRM = 1,    // any pose, standard vrm model (all joints' coordinate is identity)
		OTHERS = 2     // any pose, but need to meet the constraint defined by library
	}
	public enum CalibrationType : UInt32
	{
		DEFAULTCALIBRATION =
			0,    // User stands L-pose. Use tracked device poses to calibrate. Need tracked device pose.
		TOFFSETCALIBRATION =
			1,    // User stands straight. Only do translation offset calibration. Need tracked device pose.
		HEIGHTCALIBRATION = 2,    // Set user height directly. No need tracked device pose.
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct Joint
	{
		[FieldOffset(0)] public JointType jointType;
		[FieldOffset(4)] public PoseState poseState;
		[FieldOffset(8)] public Vector3 translation;
		[FieldOffset(20)] public Vector3 velocity;
		[FieldOffset(32)] public Vector3 angularVelocity;
		[FieldOffset(44)] public Quaternion rotation;

		public Joint(JointType in_jointType, PoseState in_poseState, Vector3 in_translation, Vector3 in_velocity, Vector3 in_angularVelocity, Quaternion in_rotation)
		{
			jointType = in_jointType;
			poseState = in_poseState;
			translation = in_translation;
			velocity = in_velocity;
			angularVelocity = in_angularVelocity;
			rotation = in_rotation;
			Rdp.Validate(ref rotation);
		}
		public static Joint identity
		{
			get
			{
				return new Joint(JointType.UNKNOWN, PoseState.NODATA, Vector3.zero, Vector3.zero, Vector3.zero, Quaternion.identity);
			}
		}
		public void Update(Joint in_joint)
		{
			jointType = in_joint.jointType;
			poseState = in_joint.poseState;
			translation = in_joint.translation;
			velocity = in_joint.velocity;
			angularVelocity = in_joint.angularVelocity;
			rotation = in_joint.rotation;
			Rdp.Validate(ref rotation);
		}
		public void Update(PoseState in_poseState, Vector3 in_translation, Vector3 in_velocity, Vector3 in_angularVelocity, Quaternion in_rotation)
		{
			poseState = in_poseState;
			translation = in_translation;
			velocity = in_velocity;
			angularVelocity = in_angularVelocity;
			rotation = in_rotation;
			Rdp.Validate(ref rotation);
		}
		public static Joint init(JointType type)
		{
			return new Joint(type, PoseState.NODATA, Vector3.zero, Vector3.zero, Vector3.zero, Quaternion.identity);
		}
#if WAVE_BODY_IK
		public void Update(ref WVR_BodyLocationPose_t pose)
		{
			pose.locationFlags = (UInt64)poseState;
			Rdp.GetVector3fFromUnity(translation, out pose.position);
			Rdp.GetQuatfFromUnity(rotation, out pose.orientation);
		}
#endif
	}

	[StructLayout(LayoutKind.Explicit)]
	[Serializable]
	public struct Extrinsic
	{
		[FieldOffset(0)] public Vector3 translation;
		[FieldOffset(12)] public Quaternion rotation;

		public Extrinsic(Vector3 in_translation, Quaternion in_rotation)
		{
			translation = in_translation;
			rotation = in_rotation;
			Rdp.Validate(ref rotation);
		}
		public static Extrinsic identity
		{
			get
			{
				return new Extrinsic(Vector3.zero, Quaternion.identity);
			}
		}
		public void Update(Extrinsic in_ext)
		{
			translation = in_ext.translation;
			rotation = in_ext.rotation;
			Rdp.Validate(ref rotation);
		}
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct TrackedDeviceExtrinsic
	{
		[FieldOffset(0)] public TrackedDeviceRole trackedDeviceRole;
		[FieldOffset(4)] public Extrinsic extrinsic;

		public TrackedDeviceExtrinsic(TrackedDeviceRole in_trackedDeviceRole, Extrinsic in_extrinsic)
		{
			trackedDeviceRole = in_trackedDeviceRole;
			extrinsic = in_extrinsic;
		}
		public static TrackedDeviceExtrinsic identity
		{
			get
			{
				return new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_UNDEFINED, Extrinsic.identity);
			}
		}
		public static TrackedDeviceExtrinsic init(TrackedDeviceRole role)
		{
			return new TrackedDeviceExtrinsic(role, Extrinsic.identity);
		}
		public void Update(TrackedDeviceExtrinsic in_ext)
		{
			trackedDeviceRole = in_ext.trackedDeviceRole;
			extrinsic.Update(in_ext.extrinsic);
		}
		public void Update(TrackedDeviceRole in_role, Extrinsic in_ext)
		{
			trackedDeviceRole = in_role;
			extrinsic.Update(in_ext);
		}
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct TrackedDevicePose
	{
		[FieldOffset(0)] public TrackedDeviceRole trackedDeviceRole;
		[FieldOffset(4)] public PoseState poseState;
		[FieldOffset(8)] public Vector3 translation;
		[FieldOffset(20)] public Vector3 velocity;
		[FieldOffset(32)] public Vector3 angularVelocity;
		[FieldOffset(44)] public Vector3 acceleration;
		[FieldOffset(56)] public Quaternion rotation;

		public TrackedDevicePose(TrackedDeviceRole in_trackedDeviceRole, PoseState in_poseState, Vector3 in_translation, Vector3 in_velocity, Vector3 in_angularVelocity, Vector3 in_acceleration, Quaternion in_rotation)
		{
			trackedDeviceRole = in_trackedDeviceRole;
			poseState = in_poseState;
			translation = in_translation;
			velocity = in_velocity;
			angularVelocity = in_angularVelocity;
			acceleration = in_acceleration;
			rotation = in_rotation;
			Rdp.Validate(ref rotation);
		}
		public static TrackedDevicePose identity
		{
			get
			{
				return new TrackedDevicePose(TrackedDeviceRole.ROLE_UNDEFINED, PoseState.NODATA, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Quaternion.identity);
			}
		}
		public void Update([In] TrackedDevicePose in_pose)
		{
			trackedDeviceRole = in_pose.trackedDeviceRole;
			poseState = in_pose.poseState;
			translation = in_pose.translation;
			velocity = in_pose.velocity;
			acceleration = in_pose.acceleration;
			rotation = in_pose.rotation;
			Rdp.Validate(ref rotation);
			angularVelocity = in_pose.angularVelocity;
		}
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct RotateSpace
	{
		[FieldOffset(0)] public JointType jointType;
		[FieldOffset(4)] public Quaternion rotation;

		public RotateSpace(JointType in_jointType, Quaternion in_rotation)
		{
			jointType = in_jointType;
			rotation = in_rotation;
			Rdp.Validate(ref rotation);
		}
		public static RotateSpace identity
		{
			get
			{
				return new RotateSpace(JointType.UNKNOWN, Quaternion.identity);
			}
		}
	}
	#endregion

	#region API v1.0.0.6
	public enum CalibrationStatus : Int32
	{
		STATUS_UNINITIAL = -1,
		STATUS_WAITING_STATIC = 0,
		STATUS_WAITING_POSE_MODE = 1,
		STATUS_READY = 2,
		STATUS_COLLECTING = 3,
		STATUS_COLLECTED = 4,
		STATUS_COLLECTED_AND_COMPUTING = 5,
		STATUS_FINISHED = 6,
		STATUS_WALKFAILED_DISTANCE = 7,
		STATUS_WALKFAILED_TIME = 8,
		STATUS_WAIT_STATIC_FAILED_TIME = 9,
		STATUS_WAIT_POSEMODE_FAILED_TIME = 10,
		STATUS_READYFAILED_TIME = 11,
		STATUS_COMPUTEFAILED_TIME = 12,
		STATUS_COMPUTEFAILED = 13,
		STATUS_REASON_NONSTATIC_START = 32,
		STATUS_REASON_NOTRIGGER = 64,
		STATUS_REASON_NONSTATIC_END = 96,
	}
	public delegate void CalibrationStatusDelegate(object sender, CalibrationStatus status);

	[StructLayout(LayoutKind.Explicit)]
	public struct TrackedDeviceRedirectExtrinsic
	{
		[FieldOffset(0)] public TrackedDeviceRole trackedDeviceRole;
		[FieldOffset(4)] public Quaternion rotation;

		public TrackedDeviceRedirectExtrinsic(TrackedDeviceRole in_role, Quaternion in_rot)
		{
			trackedDeviceRole = in_role;
			rotation = in_rot;
		}
		public static TrackedDeviceRedirectExtrinsic identity
		{
			get
			{
				return new TrackedDeviceRedirectExtrinsic(TrackedDeviceRole.ROLE_UNDEFINED, Quaternion.identity);
			}
		}
		public static TrackedDeviceRedirectExtrinsic init(TrackedDeviceRole role)
		{
			return new TrackedDeviceRedirectExtrinsic(role, Quaternion.identity);
		}
		public void Update(TrackedDeviceRedirectExtrinsic in_ext)
		{
			trackedDeviceRole = in_ext.trackedDeviceRole;
			rotation = in_ext.rotation;
		}
	};

	/// <summary>
	/// The database used to store the Function Calibration data.
	/// Note that the coordinate of data in this struct depend on FBT.
	/// In FBT 1.0.0.8 the coordinate is OpenGL.
	/// </summary>
	public class BodyRedirectives
	{
		#region Log
		const string LOG_TAG = "VIVE.OpenXR.Toolkits.BodyTracking.BodyRedirectives";
		StringBuilder m_sb = null;
		StringBuilder sb
		{
			get
			{
				if (m_sb == null) { m_sb = new StringBuilder(); }
				return m_sb;
			}
		}
		void DEBUG(StringBuilder msg) { Rdp.d(LOG_TAG, msg, true); }
		#endregion

		public bool InUse = false;
		public TrackedDeviceExtrinsic[] DeviceExts = null;
		public TrackedDeviceRedirectExtrinsic[] RedirectExts = null;
		public UInt32 ExtrinsicCount = 0;
		public TrackedDevicePose[] DevicePoses = null;
		public UInt32 PoseCount = 0;

		public void UpdateExts([In] TrackedDeviceExtrinsic[] in_exts, [In] TrackedDeviceRedirectExtrinsic[] in_redirect, UInt32 in_count)
		{
			if (in_exts == null || in_exts.Length != in_count ||
				in_redirect == null || in_redirect.Length != in_count)
			{
				return;
			}

			ExtrinsicCount = in_count;

			if (DeviceExts == null || DeviceExts.Length != ExtrinsicCount) { DeviceExts = new TrackedDeviceExtrinsic[ExtrinsicCount]; }
			if (RedirectExts == null || RedirectExts.Length != ExtrinsicCount) { RedirectExts = new TrackedDeviceRedirectExtrinsic[ExtrinsicCount]; }
			for (int i = 0; i < ExtrinsicCount; i++)
			{
				DeviceExts[i].Update(in_exts[i]);
				RedirectExts[i].Update(in_redirect[i]);
			}
		}
		public void UpdatePoses([In] TrackedDevicePose[] in_poses, UInt32 in_count)
		{
			if (in_poses == null || in_poses.Length != in_count) { return; }

			PoseCount = in_count;

			if (DevicePoses == null || DevicePoses.Length != PoseCount) { DevicePoses = new TrackedDevicePose[PoseCount]; }
			for (int i = 0; i < PoseCount; i++)
			{
				DevicePoses[i].Update(in_poses[i]);
			}
		}
		public void Update(BodyRedirectives in_ext)
		{
			InUse = in_ext.InUse;
			UpdateExts(in_ext.DeviceExts, in_ext.RedirectExts, in_ext.ExtrinsicCount);
			UpdatePoses(in_ext.DevicePoses, in_ext.PoseCount);
		}
		public void Reset()
		{
			InUse = false;
			DeviceExts = null;
			RedirectExts = null;
			ExtrinsicCount = 0;
			DevicePoses = null;
			PoseCount = 0;
		}
#if WAVE_BODY_CALIBRATION
		public BodyTrackingResult InitBodyRedirectivesFromRuntime([In] WVR_BodyTracking_DeviceInfo_t[] in_infos, UInt32 infoCount, [In] WVR_BodyTracking_RedirectExtrinsic_t[] in_exts, UInt32 extCount)
		{
			string func = "InitBodyRedirectivesFromRuntime() ";
			if (in_infos == null || in_infos.Length != infoCount || in_exts == null || in_exts.Length != extCount) { return BodyTrackingResult.ERROR_INVALID_ARGUMENT; }

			PoseCount = infoCount;
			DevicePoses = new TrackedDevicePose[PoseCount];
			for (int i = 0; i < PoseCount; i++)
			{
				Rdp.Update(ref DevicePoses[i], in_infos[i].role, in_infos[i].standardPose);
				sb.Clear().Append(func).Append("DevicePoses[").Append(i).Append("] ").Append(DevicePoses[i].trackedDeviceRole.Name())
					.Append(", poseState: ").Append(DevicePoses[i].poseState)
					.Append(", position (").Append(DevicePoses[i].translation.x.ToString("N3")).Append(", ").Append(DevicePoses[i].translation.y.ToString("N3")).Append(", ").Append(DevicePoses[i].translation.z.ToString("N3")).Append(")")
					.Append(", rotation (").Append(DevicePoses[i].rotation.x.ToString("N3")).Append(", ").Append(DevicePoses[i].rotation.y.ToString("N3")).Append(", ").Append(DevicePoses[i].rotation.z.ToString("N3")).Append(", ").Append(DevicePoses[i].rotation.w.ToString("N3")).Append(")");
				DEBUG(sb);
			}

			ExtrinsicCount = extCount;
			DeviceExts = new TrackedDeviceExtrinsic[ExtrinsicCount];
			RedirectExts = new TrackedDeviceRedirectExtrinsic[ExtrinsicCount];
			for (int i = 0; i < ExtrinsicCount; i++)
			{
				for (int info_index = 0; info_index < in_infos.Length; info_index++)
				{
					if (in_exts[i].role == in_infos[info_index].role)
					{
						Rdp.Update(ref DeviceExts[i], in_infos[info_index].role, in_infos[info_index].extrinsic);
						sb.Clear().Append(func).Append("DeviceExts[").Append(i).Append("] ").Append(DeviceExts[i].trackedDeviceRole.Name())
							.Append(", position (").Append(DeviceExts[i].extrinsic.translation.x.ToString("N3")).Append(", ").Append(DeviceExts[i].extrinsic.translation.y.ToString("N3")).Append(", ").Append(DeviceExts[i].extrinsic.translation.z.ToString("N3")).Append(")")
							.Append(", rotation (").Append(DeviceExts[i].extrinsic.rotation.x.ToString("N3")).Append(", ").Append(DeviceExts[i].extrinsic.rotation.y.ToString("N3")).Append(", ").Append(DeviceExts[i].extrinsic.rotation.z.ToString("N3")).Append(", ").Append(DeviceExts[i].extrinsic.rotation.w.ToString("N3")).Append(")");
						DEBUG(sb);
					}
				}

				Rdp.Update(ref RedirectExts[i], in_exts[i]);
				sb.Clear().Append(func).Append("RedirectExts[").Append(i).Append("] ").Append(RedirectExts[i].trackedDeviceRole.Name())
					.Append(", rotation (").Append(RedirectExts[i].rotation.x.ToString("N3")).Append(", ").Append(RedirectExts[i].rotation.y.ToString("N3")).Append(", ").Append(RedirectExts[i].rotation.z.ToString("N3")).Append(", ").Append(RedirectExts[i].rotation.w.ToString("N3")).Append(")");
				DEBUG(sb);
			}

			return BodyTrackingResult.SUCCESS;
		}
		public bool GetExtrinsicsWVR(out WVR_BodyTracking_RedirectExtrinsic_t[] exts)
		{
			exts = null;
			if (RedirectExts == null) { return false; }

			exts = new WVR_BodyTracking_RedirectExtrinsic_t[RedirectExts.Length];
			for (int i = 0; i < exts.Length; i++)
			{
				exts[i].role = RedirectExts[i].trackedDeviceRole.wvrRole();
				exts[i].rotation = Rdp.GetQuatfFromUnity(RedirectExts[i].rotation);
			}
			return true;
		}
#endif
	}
	#endregion

	public class fbt
	{
		const string LOG_TAG = "VIVE.OpenXR.Toolkits.BodyTracking.fbt";
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
		static void ERROR(StringBuilder msg) { Rdp.e(LOG_TAG, msg, true); }

#if UNITY_EDITOR
		const string kBodyTrackingLib = "BodyTracking";
#else
#if CoordinateOpenGL
		const string kBodyTrackingLib = "bodytracking";
#else
		const string kBodyTrackingLib = "bodytracking1.0.0.7";
#endif
#endif

		#region API v1.0.0.1
		[DllImport(kBodyTrackingLib)]
		/**
		 *  @brief Initial body tracking algorithm with custom skeleton
		 *  @param[in] timestamp (unit:us)
		 *  @param[in] bodyTrackingMode. The body tracking mode which developer wants to use
		 *  @param[in] trackedDeviceExt. The tracked device extrinsic from avatar to tracked device
		 *  @param[in] deviceCount. The amount of tracked devices
		 *  @param[in] avatarJoints. The avatar's joints
		 *  @param[in] avatarJointCount. The amount of the avatar's joints
		 *  @param[in] avatarHeight. The avatar's height
		 *  @param[out] skeleton id.
		 *  @param[in] avatarType. The avatar's type (This paramenter is only for internal use. The default value is TPOSE.)
		 *  @param[out] success or not.
		 **/
		public static extern Result InitBodyTracking(UInt64 ts, BodyTrackingMode bodyTrackingMode,
			TrackedDeviceExtrinsic[] trackedDeviceExt, UInt32 deviceCount,
			Joint[] avatarJoints, UInt32 avatarJointCount, float avatarHeight,
			ref int skeletonId,
			AvatarType avatarType = AvatarType.TPOSE);
		public static Result InitBodyTrackingLog(UInt64 ts, BodyTrackingMode bodyTrackingMode,
			TrackedDeviceExtrinsic[] trackedDeviceExt, UInt32 deviceCount,
			Joint[] avatarJoints, UInt32 avatarJointCount, float avatarHeight,
			ref int skeletonId,
			AvatarType avatarType = AvatarType.TPOSE)
		{
			string func = "InitBodyTracking() ";
			sb.Clear().Append(func).Append(ts).Append(" bodyTrackingMode: ").Append(bodyTrackingMode.Name()); DEBUG(sb);

			sb.Clear().Append(func).Append("deviceCount: ").Append(deviceCount); DEBUG(sb);
			for (UInt32 i = 0; i < deviceCount; i++)
			{
				sb.Clear().Append(func).Append("trackedDeviceExt[").Append(i).Append("] role: ").Append(trackedDeviceExt[i].trackedDeviceRole.Name())
					.Append(", position (").Append(trackedDeviceExt[i].extrinsic.translation.x.ToString("N3")).Append(", ").Append(trackedDeviceExt[i].extrinsic.translation.y.ToString("N3")).Append(", ").Append(trackedDeviceExt[i].extrinsic.translation.z.ToString("N3")).Append(")")
					.Append(", rotation (").Append(trackedDeviceExt[i].extrinsic.rotation.x.ToString("N3")).Append(", ").Append(trackedDeviceExt[i].extrinsic.rotation.y.ToString("N3")).Append(", ").Append(trackedDeviceExt[i].extrinsic.rotation.z.ToString("N3")).Append(", ").Append(trackedDeviceExt[i].extrinsic.rotation.w.ToString("N3")).Append(")");
				DEBUG(sb);
			}

			sb.Clear().Append(func).Append("avatarJointCount: ").Append(avatarJointCount); DEBUG(sb);
			for (UInt32 i = 0; i < avatarJointCount; i++)
			{
				sb.Clear().Append(func).Append("avatarJoints[").Append(i).Append("] jointType: ").Append(avatarJoints[i].jointType.Name())
					.Append(", poseState: ").Append(avatarJoints[i].poseState)
					.Append(", position (").Append(avatarJoints[i].translation.x.ToString("N3")).Append(", ").Append(avatarJoints[i].translation.y.ToString("N3")).Append(", ").Append(avatarJoints[i].translation.z.ToString("N3")).Append(")")
					.Append(", rotation (").Append(avatarJoints[i].rotation.x.ToString("N3")).Append(", ").Append(avatarJoints[i].rotation.y.ToString("N3")).Append(", ").Append(avatarJoints[i].rotation.z.ToString("N3")).Append(", ").Append(avatarJoints[i].rotation.w.ToString("N3")).Append(")")
					.Append(", velocity (").Append(avatarJoints[i].velocity.x.ToString("N3")).Append(", ").Append(avatarJoints[i].velocity.y.ToString("N3")).Append(", ").Append(avatarJoints[i].velocity.z.ToString("N3")).Append(")")
					.Append(", angularVelocity (").Append(avatarJoints[i].angularVelocity.x.ToString("N3")).Append(", ").Append(avatarJoints[i].angularVelocity.y.ToString("N3")).Append(", ").Append(avatarJoints[i].angularVelocity.z.ToString("N3")).Append(")");
				DEBUG(sb);
			}

			sb.Clear().Append(func).Append("avatarHeight: ").Append(avatarHeight)
				.Append(", skeletonId: ").Append(skeletonId)
				.Append(", avatarType: ").Append(avatarType);
			DEBUG(sb);
			return InitBodyTracking(ts, bodyTrackingMode, trackedDeviceExt, deviceCount, avatarJoints, avatarJointCount, avatarHeight, ref skeletonId);
		}

		[DllImport(kBodyTrackingLib)]
		/**
		 *  @brief Initial body trahcking algorithm with default skeleton
		 *  @param[in] timestamp (unit:us)
		 *  @param[in] bodyTrackingMode. The body tracking mode which developer wants to use
		 *  @param[in] trackedDeviceExt. The tracked device extrinsic from avatar to tracked device
		 *  @param[in] deviceCount. The amount of tracked devices
		 *  @param[out] skeleton id.
		 *  @param[out] success or not.
		 **/
		public static extern Result InitDefaultBodyTracking(UInt64 ts, BodyTrackingMode bodyTrackingMode,
			TrackedDeviceExtrinsic[] trackedDeviceExt, UInt32 deviceCount,
			ref int skeletonId);
		public static Result InitDefaultBodyTrackingLog(UInt64 ts, BodyTrackingMode bodyTrackingMode,
			TrackedDeviceExtrinsic[] trackedDeviceExt, UInt32 deviceCount,
			ref int skeletonId)
		{
			sb.Clear().Append("InitDefaultBodyTracking() ").Append(ts)
				.Append(", bodyTrackingMode: ").Append(bodyTrackingMode.Name())
				.Append(", deviceCount: ").Append(deviceCount);
			DEBUG(sb);

			for (int i = 0; i < deviceCount; i++)
			{
				sb.Clear().Append("InitDefaultBodyTracking() trackedDeviceExt[").Append(i).Append("] role: ").Append(trackedDeviceExt[i].trackedDeviceRole.Name())
					.Append(", position (").Append(trackedDeviceExt[i].extrinsic.translation.x.ToString("N3")).Append(", ").Append(trackedDeviceExt[i].extrinsic.translation.y.ToString("N3")).Append(", ").Append(trackedDeviceExt[i].extrinsic.translation.z.ToString("N3")).Append(")")
					.Append(", rotation (").Append(trackedDeviceExt[i].extrinsic.rotation.x.ToString("N3")).Append(", ").Append(trackedDeviceExt[i].extrinsic.rotation.y.ToString("N3")).Append(", ").Append(trackedDeviceExt[i].extrinsic.rotation.z.ToString("N3")).Append(", ").Append(trackedDeviceExt[i].extrinsic.rotation.w.ToString("N3")).Append(")");
				DEBUG(sb);
			}

			return InitDefaultBodyTracking(ts, bodyTrackingMode, trackedDeviceExt, deviceCount, ref skeletonId);
		}

		[DllImport(kBodyTrackingLib)]
		/**
		 *  @brief Calibrate Body Tracking. Must be called after initail. User needs to stand L pose(stand straight, two arms straight forward and let the palm down)
		 *  @param[in] timestamp (unit:us)
		 *  @param[in] skeletonId.
		 *  @param[in] userHeight. The user height.
		 *  @param[in] bodyTrackingMode. The body tracking mode which developer wants to use
		 *  @param[in] trackedDevicePose. The tracked device poses.(Left-Handed coordinate sytstem. x right, y up, z forward. unit: m)
		 *  @param[in] deviceCount. The amount of tracked devices
		 *  @param[out] scale. If used custom skeleton, this value will be the scale of custom skeleton. Otherwise, the value will be 1.
		 *  @param[out] success or not.
		 **/
		public static extern Result CalibrateBodyTracking(UInt64 ts, int skeletonId, float userHeight,
			BodyTrackingMode bodyTrackingMode,
			TrackedDevicePose[] trackedDevicePose, UInt32 deviceCount,
			ref float scale, CalibrationType calibrationType = CalibrationType.DEFAULTCALIBRATION);
		public static Result CalibrateBodyTrackingLog(UInt64 ts, int skeletonId, float userHeight,
			BodyTrackingMode bodyTrackingMode,
			TrackedDevicePose[] trackedDevicePose, UInt32 deviceCount,
			ref float scale, CalibrationType calibrationType = CalibrationType.DEFAULTCALIBRATION)
		{
			sb.Clear().Append("CalibrateBodyTracking() ").Append(ts).Append(", id: ").Append(skeletonId).Append(", bodyTrackingMode: ").Append(bodyTrackingMode.Name());
			DEBUG(sb);

			sb.Clear().Append("CalibrateBodyTracking() deviceCount: ").Append(deviceCount); DEBUG(sb);
			for (UInt32 i = 0; i < deviceCount; i++)
			{
				sb.Clear().Append("CalibrateBodyTracking() trackedDevicePose[").Append(i).Append("] role: ").Append(trackedDevicePose[i].trackedDeviceRole.Name())
					.Append(", position (").Append(trackedDevicePose[i].translation.x.ToString("N3")).Append(", ").Append(trackedDevicePose[i].translation.y.ToString("N3")).Append(", ").Append(trackedDevicePose[i].translation.z.ToString("N3")).Append(")")
					.Append(", rotation (").Append(trackedDevicePose[i].rotation.x.ToString("N3")).Append(", ").Append(trackedDevicePose[i].rotation.y.ToString("N3")).Append(", ").Append(trackedDevicePose[i].rotation.z.ToString("N3")).Append(", ").Append(trackedDevicePose[i].rotation.w.ToString("N3")).Append(")")
					.Append(", velocity (").Append(trackedDevicePose[i].velocity.x.ToString("N3")).Append(", ").Append(trackedDevicePose[i].velocity.y.ToString("N3")).Append(", ").Append(trackedDevicePose[i].velocity.z.ToString("N3")).Append(")")
					.Append(", acceleration (").Append(trackedDevicePose[i].acceleration.x.ToString("N3")).Append(", ").Append(trackedDevicePose[i].acceleration.y.ToString("N3")).Append(", ").Append(trackedDevicePose[i].acceleration.z.ToString("N3")).Append(")")
					.Append(", angularVelocity (").Append(trackedDevicePose[i].angularVelocity.x.ToString("N3")).Append(", ").Append(trackedDevicePose[i].angularVelocity.y.ToString("N3")).Append(", ").Append(trackedDevicePose[i].angularVelocity.z.ToString("N3")).Append(")");
				DEBUG(sb);
			}

			sb.Clear().Append("scale: ").Append(scale).Append(", calibrationType: ").Append(calibrationType.Name());
			DEBUG(sb);

			return CalibrateBodyTracking(ts, skeletonId, userHeight, bodyTrackingMode, trackedDevicePose, deviceCount, ref scale, calibrationType);
		}

		[DllImport(kBodyTrackingLib)]
		/**
		 *  @brief Get the amount of output skeleton joints.
		 *  @param[in] timestamp (unit:us)
		 *  @param[in] skeleton id.
		 *  @param[out] the amount of output skeleton joints.
		 *  @param[out] success or not.
		 **/
		public static extern Result GetOutputJointCount(UInt64 ts, int skeletonId, ref UInt32 jointCount);

		[DllImport(kBodyTrackingLib)]
		/**
		 *  @brief Update and get skeleton joints pose every frame. Must be called after calibrate.
		 *  @param[in] timestamp (unit:us)
		 *  @param[in] skeleton id.
		 *  @param[in] trackedDevicePose. The tracked device poses.(Left-Handed coordinate sytstem. x right, y up, z forward. unit: m)
		 *  @param[in] deviceCount. The amount of tracked devices
		 *  @param[out] output joints of skeleton. If the pose state of joint equals to 3(Translation|Rotation), it means the joint's pose is valid.
		 *  @param[in] jointCount. The amount of joints.
		 *  @param[out] success or not.
		 **/
		public static extern Result UpdateBodyTracking(UInt64 ts, int skeletonId,
			TrackedDevicePose[] trackedDevicePose, UInt32 deviceCount,
			[In, Out] Joint[] outJoint, UInt32 jointCount);
		public static Result UpdateBodyTrackingLog(UInt64 ts, int skeletonId,
			TrackedDevicePose[] trackedDevicePose, UInt32 deviceCount,
			[In, Out] Joint[] outJoint, UInt32 jointCount)
		{
			sb.Clear().Append("UpdateBodyTracking() ").Append(ts).Append(", id: ").Append(skeletonId).Append(", deviceCount: ").Append(deviceCount);
			DEBUG(sb);
			for (UInt32 i = 0; i < deviceCount; i++)
			{
				sb.Clear().Append("UpdateBodyTracking() trackedDevicePose[").Append(i).Append("] role: ").Append(trackedDevicePose[i].trackedDeviceRole.Name())
					.Append(", position (").Append(trackedDevicePose[i].translation.x.ToString("N3")).Append(", ").Append(trackedDevicePose[i].translation.y.ToString("N3")).Append(", ").Append(trackedDevicePose[i].translation.z.ToString("N3")).Append(")")
					.Append(", rotation (").Append(trackedDevicePose[i].rotation.x.ToString("N3")).Append(", ").Append(trackedDevicePose[i].rotation.y.ToString("N3")).Append(", ").Append(trackedDevicePose[i].rotation.z.ToString("N3")).Append(", ").Append(trackedDevicePose[i].rotation.w.ToString("N3")).Append(")")
					.Append(", velocity (").Append(trackedDevicePose[i].velocity.x.ToString("N3")).Append(", ").Append(trackedDevicePose[i].velocity.y.ToString("N3")).Append(", ").Append(trackedDevicePose[i].velocity.z.ToString("N3")).Append(")")
					.Append(", acceleration (").Append(trackedDevicePose[i].acceleration.x.ToString("N3")).Append(", ").Append(trackedDevicePose[i].acceleration.y.ToString("N3")).Append(", ").Append(trackedDevicePose[i].acceleration.z.ToString("N3")).Append(")")
					.Append(", angularVelocity (").Append(trackedDevicePose[i].angularVelocity.x.ToString("N3")).Append(", ").Append(trackedDevicePose[i].angularVelocity.y.ToString("N3")).Append(", ").Append(trackedDevicePose[i].angularVelocity.z.ToString("N3")).Append(")");
				DEBUG(sb);
			}

			Result result = UpdateBodyTracking(ts, skeletonId, trackedDevicePose, deviceCount, outJoint, jointCount);
			if (result == Result.SUCCESS)
			{
				sb.Clear().Append("UpdateBodyTracking() jointCount: ").Append(jointCount);
				DEBUG(sb);
				for (UInt32 i = 0; i < jointCount; i++)
				{
					sb.Clear().Append("UpdateBodyTracking() outJoint[").Append(i).Append("] jointType: ").Append(outJoint[i].jointType.Name())
						.Append(", position (").Append(outJoint[i].translation.x.ToString("N3")).Append(", ").Append(outJoint[i].translation.y.ToString("N3")).Append(", ").Append(outJoint[i].translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(outJoint[i].rotation.x.ToString("N3")).Append(", ").Append(outJoint[i].rotation.y.ToString("N3")).Append(", ").Append(outJoint[i].rotation.z.ToString("N3")).Append(", ").Append(outJoint[i].rotation.w.ToString("N3")).Append(")")
						.Append(", velocity (").Append(outJoint[i].velocity.x.ToString("N3")).Append(", ").Append(outJoint[i].velocity.y.ToString("N3")).Append(", ").Append(outJoint[i].velocity.z.ToString("N3")).Append(")")
						.Append(", angularVelocity (").Append(outJoint[i].angularVelocity.x.ToString("N3")).Append(", ").Append(outJoint[i].angularVelocity.y.ToString("N3")).Append(", ").Append(outJoint[i].angularVelocity.z.ToString("N3")).Append(")");
					DEBUG(sb);
				}
			}

			return result;
		}

		[DllImport(kBodyTrackingLib)]
		/**
		 *  @brief Destroy body tracking.
		 *  @param[in] timestamp (unit:us)
		 *  @param[in] skeleton id.
		 *  @param[out] success or not.
		 **/
		public static extern Result DestroyBodyTracking(UInt64 ts, int skeletonId);
		public static Result DestroyBodyTrackingLog(UInt64 ts, int skeletonId)
		{
			sb.Clear().Append("DestroyBodyTracking() ").Append(ts).Append(", id: ").Append(skeletonId); DEBUG(sb);
			return DestroyBodyTracking(ts, skeletonId);
		}

		[DllImport(kBodyTrackingLib)]
		/**
		 *  @brief Get the amount of default skeleton joints.
		 *  @param[out] the amount of default skeleton joints.
		 *  @param[out] success or not.
		 **/
		public static extern Result GetDefaultSkeletonJointCount(ref UInt32 jointCount);

		[DllImport(kBodyTrackingLib)]
		/**
		 *  @brief Get default skeleton rotate space.
		 *  @param[out] the rotate space of default skeleton.
		 *  @param[out] success or not.
		 * */
		public static extern Result GetDefaultSkeletonRotateSpace([In, Out] RotateSpace[] rotateSpace, UInt32 jointCount);
		#endregion

		#region API v1.0.0.6
		[DllImport(kBodyTrackingLib)]
		public static extern Result StartFunctionalCalibration(UInt64 ts, BodyTrackingMode bodyTrackingMode);
		public static Result StartFunctionalCalibrationLog(UInt64 ts, BodyTrackingMode bodyTrackingMode)
		{
			sb.Clear().Append("StartFunctionalCalibration() bodyTrackingMode: ").Append(bodyTrackingMode.Name()); DEBUG(sb);
			return StartFunctionalCalibration(ts, bodyTrackingMode);
		}
		[DllImport(kBodyTrackingLib)]
		public static extern Result UpdateFunctionalCalibration(UInt64 ts, TrackedDevicePose[] trackedDevicePose, UInt32 deviceCount, ref CalibrationStatus status);
		public static Result UpdateFunctionalCalibrationLog(UInt64 ts, TrackedDevicePose[] trackedDevicePose, UInt32 deviceCount, ref CalibrationStatus status)
		{
			if (trackedDevicePose.Length != deviceCount)
			{
				sb.Clear().Append("UpdateFunctionalCalibrationLog() trackedDevicePose length and deviceCount is not matched."); ERROR(sb);
				return Result.ERROR_INPUTPOSE_NOT_VALID;
			}

			for (int i = 0; i < trackedDevicePose.Length; i++)
			{
				sb.Clear().Append("UpdateFunctionalCalibration() status: ").Append(status.Name())
					.Append(", trackedDevicePose[").Append(i).Append("] role: ").Append(trackedDevicePose[i].trackedDeviceRole.Name())
					.Append(", poseState: ").Append(trackedDevicePose[i].poseState)
					.Append(", position (").Append(trackedDevicePose[i].translation.x.ToString("N3")).Append(", ").Append(trackedDevicePose[i].translation.y.ToString("N3")).Append(", ").Append(trackedDevicePose[i].translation.z.ToString("N3")).Append(")")
					.Append(", velocity (").Append(trackedDevicePose[i].velocity.x.ToString("N3")).Append(", ").Append(trackedDevicePose[i].velocity.y.ToString("N3")).Append(", ").Append(trackedDevicePose[i].velocity.z.ToString("N3")).Append(")")
					.Append(", acceleration (").Append(trackedDevicePose[i].acceleration.x.ToString("N3")).Append(", ").Append(trackedDevicePose[i].acceleration.y.ToString("N3")).Append(", ").Append(trackedDevicePose[i].acceleration.z.ToString("N3")).Append(")")
					.Append(", rotation (").Append(trackedDevicePose[i].rotation.x.ToString("N3")).Append(", ").Append(trackedDevicePose[i].rotation.y.ToString("N3")).Append(", ").Append(trackedDevicePose[i].rotation.z.ToString("N3")).Append(", ").Append(trackedDevicePose[i].rotation.w.ToString("N3")).Append(")")
					.Append(", angularVelocity (").Append(trackedDevicePose[i].angularVelocity.x.ToString("N3")).Append(", ").Append(trackedDevicePose[i].angularVelocity.y.ToString("N3")).Append(", ").Append(trackedDevicePose[i].angularVelocity.z.ToString("N3")).Append(")");
				DEBUG(sb);
			}

			return UpdateFunctionalCalibration(ts, trackedDevicePose, deviceCount, ref status);
		}
		[DllImport(kBodyTrackingLib)]
		public static extern Result DestroyFunctionalCalibration(UInt64 ts);
		public static Result DestroyFunctionalCalibrationLog(UInt64 ts)
		{
			sb.Clear().Append("DestroyFunctionalCalibrationLog()"); DEBUG(sb);
			return DestroyFunctionalCalibration(ts);
		}

		[DllImport(kBodyTrackingLib)]
		public static extern Result GetCalibratedPoseCount(UInt64 ts, ref UInt32 poseCount);
		public static Result GetCalibratedPoseCountLog(UInt64 ts, ref UInt32 poseCount)
		{
			Result ret = GetCalibratedPoseCount(ts, ref poseCount);
			sb.Clear().Append("GetCalibratedPoseCount() poseCount: ").Append(poseCount).Append(", result: ").Append(ret); DEBUG(sb);
			return ret;
		}
		[DllImport(kBodyTrackingLib)]
		public static extern Result GetCalibratedExtrinsicCount(UInt64 ts, ref UInt32 deviceCount);
		public static Result GetCalibratedExtrinsicCountLog(UInt64 ts, ref UInt32 deviceCount)
		{
			Result ret = GetCalibratedExtrinsicCount(ts, ref deviceCount);
			sb.Clear().Append("GetCalibratedExtrinsicCount() deviceCount: ").Append(deviceCount).Append(", result: ").Append(ret); DEBUG(sb);
			return ret;
		}
		[DllImport(kBodyTrackingLib)]
		public static extern Result GetFunctionalCalibrationResult(UInt64 ts, [In, Out] TrackedDeviceExtrinsic[] trackedDeviceExt, [In, Out] TrackedDeviceRedirectExtrinsic[] trackedDeviceRedirectExt, UInt32 deviceCount, [In, Out] TrackedDevicePose[] calibratedTrackedDevicePose, UInt32 poseCount);
		public static Result GetFunctionalCalibrationResultLog(UInt64 ts, [In, Out] TrackedDeviceExtrinsic[] trackedDeviceExt, [In, Out] TrackedDeviceRedirectExtrinsic[] trackedDeviceRedirectExt, UInt32 deviceCount, [In, Out] TrackedDevicePose[] calibratedTrackedDevicePose, UInt32 poseCount)
		{
			Result ret = GetFunctionalCalibrationResult(ts, trackedDeviceExt, trackedDeviceRedirectExt, deviceCount, calibratedTrackedDevicePose, poseCount);

			for (int i = 0; i < deviceCount; i++)
			{
				sb.Clear().Append("GetFunctionalCalibrationResult(").Append(ts).Append(")")
					.Append(" trackedDeviceExt[").Append(i).Append("] role: ").Append(trackedDeviceExt[i].trackedDeviceRole.Name())
					.Append(", position (").Append(trackedDeviceExt[i].extrinsic.translation.x.ToString("N3")).Append(", ").Append(trackedDeviceExt[i].extrinsic.translation.y.ToString("N3")).Append(", ").Append(trackedDeviceExt[i].extrinsic.translation.z.ToString("N3")).Append(")")
					.Append(", rotation (").Append(trackedDeviceExt[i].extrinsic.rotation.x.ToString("N3")).Append(", ").Append(trackedDeviceExt[i].extrinsic.rotation.y.ToString("N3")).Append(", ").Append(trackedDeviceExt[i].extrinsic.rotation.z.ToString()).Append(", ").Append(trackedDeviceExt[i].extrinsic.rotation.w.ToString("N3")).Append(")")
					.Append("\ntrackedDeviceRedirectExt[").Append(i).Append("] role: ").Append(trackedDeviceRedirectExt[i].trackedDeviceRole.Name())
					.Append(", rotation (").Append(trackedDeviceRedirectExt[i].rotation.x.ToString("N3")).Append(", ").Append(trackedDeviceRedirectExt[i].rotation.y.ToString("N3")).Append(", ").Append(trackedDeviceRedirectExt[i].rotation.z.ToString("N3")).Append(", ").Append(trackedDeviceRedirectExt[i].rotation.w.ToString("N3")).Append(")");
				DEBUG(sb);
			}
			for (int i = 0; i < poseCount; i++)
			{
				sb.Clear().Append("GetFunctionalCalibrationResult(").Append(ts).Append(")")
					.Append(" calibratedTrackedDevicePose[").Append(i).Append("] role: ").Append(calibratedTrackedDevicePose[i].trackedDeviceRole.Name())
					.Append(", position (").Append(calibratedTrackedDevicePose[i].translation.x.ToString("N3")).Append(", ").Append(calibratedTrackedDevicePose[i].translation.y.ToString("N3")).Append(", ").Append(calibratedTrackedDevicePose[i].translation.z.ToString("N3")).Append(")");
				DEBUG(sb);
			}

			return ret;
		}

		[DllImport(kBodyTrackingLib)]
		public static extern Result RedirectTrackedDevice(UInt64 ts, int skeletonId, UInt32 deviceCount, TrackedDeviceRedirectExtrinsic[] trackedDeviceRedirectExt);
		public static Result RedirectTrackedDeviceLog(UInt64 ts, int skeletonId, UInt32 deviceCount, TrackedDeviceRedirectExtrinsic[] trackedDeviceRedirectExt)
		{
			sb.Clear().Append("RedirectTrackedDevice(").Append(ts)
				.Append(") skeletonId: ").Append(skeletonId)
				.Append(", deviceCount: ").Append(deviceCount)
				.Append(", trackedDeviceRedirectExt.length: ").Append(trackedDeviceRedirectExt.Length);
			for (int i = 0; i < trackedDeviceRedirectExt.Length; i++)
			{
				sb.Append("\ntrackedDeviceRedirectExt[").Append(i)
					.Append("] Role: ").Append(trackedDeviceRedirectExt[i].trackedDeviceRole.Name())
					.Append(", rotation (").Append(trackedDeviceRedirectExt[i].rotation.x.ToString("N3")).Append(", ").Append(trackedDeviceRedirectExt[i].rotation.y.ToString("N3")).Append(", ").Append(trackedDeviceRedirectExt[i].rotation.z.ToString("N3")).Append(", ").Append(trackedDeviceRedirectExt[i].rotation.w.ToString("N3")).Append(")");
			}
			DEBUG(sb);

			return RedirectTrackedDevice(ts, skeletonId, deviceCount, trackedDeviceRedirectExt);
		}
		#endregion

		#region API v1.0.0.8
		private static void ConvertTranslationFromToGL(ref Vector3 value)
		{
			Vector3 vec3 = value;
			value.x = vec3.x;
			value.y = vec3.y;
			value.z = -vec3.z;
		}
		private static void ConvertRotationFromToGL(ref Quaternion value)
		{
			Quaternion quat = value;
			value.x = quat.x;
			value.y = quat.y;
			value.z = -quat.z;
			value.w = -quat.w;
			Rdp.Validate(ref value);
		}
		private static void ConvertAngularFromToGL(ref Vector3 value)
		{
			Vector3 vec3 = value;
			value.x = -vec3.x;
			value.y = -vec3.y;
			value.z = vec3.z;
		}
		private static void ConvertDataFromToFBT(ref TrackedDeviceExtrinsic ext)
		{
			ConvertTranslationFromToGL(ref ext.extrinsic.translation);
			ConvertRotationFromToGL(ref ext.extrinsic.rotation);
		}
		private static void ConvertDataFromToFBT(ref Joint joint)
		{
			ConvertTranslationFromToGL(ref joint.translation);
			ConvertTranslationFromToGL(ref joint.velocity);
			ConvertRotationFromToGL(ref joint.rotation);
			ConvertAngularFromToGL(ref joint.angularVelocity);
		}
		private static void ConvertDataFromToFBT(ref TrackedDevicePose pose)
		{
			ConvertTranslationFromToGL(ref pose.translation);
			ConvertTranslationFromToGL(ref pose.velocity);
			ConvertTranslationFromToGL(ref pose.acceleration);
			ConvertRotationFromToGL(ref pose.rotation);
			ConvertAngularFromToGL(ref pose.angularVelocity);
		}
		private static void ConvertDataFromToFBT(ref TrackedDeviceRedirectExtrinsic ext)
		{
			ConvertRotationFromToGL(ref ext.rotation);
		}
		public static Result InitBodyTrackingT(UInt64 ts, BodyTrackingMode bodyTrackingMode,
			TrackedDeviceExtrinsic[] trackedDeviceExt, UInt32 deviceCount,
			Joint[] avatarJoints, UInt32 avatarJointCount, float avatarHeight,
			ref int skeletonId,
			AvatarType avatarType = AvatarType.TPOSE)
		{
			for (int i = 0; i < trackedDeviceExt.Length; i++) { ConvertDataFromToFBT(ref trackedDeviceExt[i]); }
			for (int i = 0; i < avatarJoints.Length; i++) { ConvertDataFromToFBT(ref avatarJoints[i]); }

			Result result = InitBodyTrackingLog(ts, bodyTrackingMode, trackedDeviceExt, deviceCount, avatarJoints, avatarJointCount, avatarHeight, ref skeletonId);

			return result;
		}

		public static Result InitDefaultBodyTrackingT(UInt64 ts, BodyTrackingMode bodyTrackingMode,
			TrackedDeviceExtrinsic[] trackedDeviceExt, UInt32 deviceCount,
			ref int skeletonId)
		{
			for (int i = 0; i < trackedDeviceExt.Length; i++) { ConvertDataFromToFBT(ref trackedDeviceExt[i]); }

			Result result = InitDefaultBodyTrackingLog(ts, bodyTrackingMode, trackedDeviceExt, deviceCount, ref skeletonId);

			return result;
		}

		public static Result CalibrateBodyTrackingT(UInt64 ts, int skeletonId, float userHeight,
			BodyTrackingMode bodyTrackingMode,
			TrackedDevicePose[] trackedDevicePose, UInt32 deviceCount,
			ref float scale, CalibrationType calibrationType = CalibrationType.DEFAULTCALIBRATION)
		{
			for (int i = 0; i < trackedDevicePose.Length; i++) { ConvertDataFromToFBT(ref trackedDevicePose[i]); }

			Result result = CalibrateBodyTrackingLog(ts, skeletonId, userHeight, bodyTrackingMode, trackedDevicePose, deviceCount, ref scale, calibrationType);

			return result;
		}

		public static Result UpdateBodyTrackingT(bool printLog,
			UInt64 ts, int skeletonId,
			TrackedDevicePose[] trackedDevicePose, UInt32 deviceCount,
			[In, Out] Joint[] outJoint, UInt32 jointCount)
		{
			for (int i = 0; i < trackedDevicePose.Length; i++) { ConvertDataFromToFBT(ref trackedDevicePose[i]); }

			Result result = Result.ERROR_CALIBRATE_FAILED;

			if (printLog)
			{
				result = UpdateBodyTrackingLog(ts, skeletonId, trackedDevicePose, deviceCount, outJoint, jointCount);
			}
			else
			{
				result = UpdateBodyTracking(ts, skeletonId, trackedDevicePose, deviceCount, outJoint, jointCount);
			}

			for (int i = 0; i < outJoint.Length; i++) { ConvertDataFromToFBT(ref outJoint[i]); }

			return result;
		}

		public static Result UpdateFunctionalCalibrationT(bool printLog, UInt64 ts, TrackedDevicePose[] trackedDevicePose, UInt32 deviceCount, ref CalibrationStatus status)
		{
			for (int i = 0; i < trackedDevicePose.Length; i++) { ConvertDataFromToFBT(ref trackedDevicePose[i]); }

			Result result = Result.ERROR_NOT_CALIBRATED; ;
			if (printLog)
			{
				result = UpdateFunctionalCalibrationLog(ts, trackedDevicePose, deviceCount, ref status);
			}
			else
			{
				result = UpdateFunctionalCalibration(ts, trackedDevicePose, deviceCount, ref status);
			}

			return result;
		}

		public static Result GetFunctionalCalibrationResultT(UInt64 ts,
			[In, Out] TrackedDeviceExtrinsic[] trackedDeviceExt,
			[In, Out] TrackedDeviceRedirectExtrinsic[] trackedDeviceRedirectExt, UInt32 deviceCount,
			[In, Out] TrackedDevicePose[] calibratedTrackedDevicePose, UInt32 poseCount)
		{
			Result result = GetFunctionalCalibrationResultLog(ts, trackedDeviceExt, trackedDeviceRedirectExt, deviceCount, calibratedTrackedDevicePose, poseCount);

			for (int i = 0; i < trackedDeviceExt.Length; i++) { ConvertDataFromToFBT(ref trackedDeviceExt[i]); }
			for (int i = 0; i < trackedDeviceRedirectExt.Length; i++) { ConvertDataFromToFBT(ref trackedDeviceRedirectExt[i]); }
			for (int i = 0; i < calibratedTrackedDevicePose.Length; i++) { ConvertDataFromToFBT(ref calibratedTrackedDevicePose[i]); }

			return result;
		}

		public static Result RedirectTrackedDeviceT(UInt64 ts, int skeletonId, UInt32 deviceCount, TrackedDeviceRedirectExtrinsic[] trackedDeviceRedirectExt)
		{
			for (int i = 0; i < trackedDeviceRedirectExt.Length; i++) { ConvertDataFromToFBT(ref trackedDeviceRedirectExt[i]); }

			Result result = RedirectTrackedDeviceLog(ts, skeletonId, deviceCount, trackedDeviceRedirectExt);

			for (int i = 0; i < trackedDeviceRedirectExt.Length; i++) { ConvertDataFromToFBT(ref trackedDeviceRedirectExt[i]); }

			return result;
		}
		#endregion
	}

	public static class BodyTrackingUtils
	{
		const string LOG_TAG = "VIVE.OpenXR.Toolkits.BodyTracking.BodyTrackingUtils";
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

		public static string Name(this TrackedDeviceRole role)
		{
			if (role == TrackedDeviceRole.ROLE_CHEST) { return "CHEST"; }
			if (role == TrackedDeviceRole.ROLE_HEAD) { return "HEAD"; }
			if (role == TrackedDeviceRole.ROLE_HIP) { return "HIP"; }

			if (role == TrackedDeviceRole.ROLE_LEFTANKLE) { return "LEFTANKLE"; }
			if (role == TrackedDeviceRole.ROLE_LEFTELBOW) { return "LEFTELBOW"; }
			if (role == TrackedDeviceRole.ROLE_LEFTFOOT) { return "LEFTFOOT"; }
			if (role == TrackedDeviceRole.ROLE_LEFTHAND) { return "LEFTHAND"; }
			if (role == TrackedDeviceRole.ROLE_LEFTHANDHELD) { return "LEFTHANDHELD"; }
			if (role == TrackedDeviceRole.ROLE_LEFTKNEE) { return "LEFTKNEE"; }
			if (role == TrackedDeviceRole.ROLE_LEFTWRIST) { return "LEFTWRIST"; }

			if (role == TrackedDeviceRole.ROLE_RIGHTANKLE) { return "RIGHTANKLE"; }
			if (role == TrackedDeviceRole.ROLE_RIGHTELBOW) { return "RIGHTELBOW"; }
			if (role == TrackedDeviceRole.ROLE_RIGHTFOOT) { return "RIGHTFOOT"; }
			if (role == TrackedDeviceRole.ROLE_RIGHTHAND) { return "RIGHTHAND"; }
			if (role == TrackedDeviceRole.ROLE_RIGHTHANDHELD) { return "RIGHTHANDHELD"; }
			if (role == TrackedDeviceRole.ROLE_RIGHTKNEE) { return "RIGHTKNEE"; }
			if (role == TrackedDeviceRole.ROLE_RIGHTWRIST) { return "RIGHTWRIST"; }

			if (role == TrackedDeviceRole.ROLE_UNDEFINED) { return "UNDEFINED"; }

			sb.Clear().Append("TrackedDeviceRole = ").Append(role); DEBUG(sb);
			return "";
		}
		public static string Name(this DeviceExtRole role)
		{
			if (role == DeviceExtRole.Arm_Wrist) { return "Arm_Wrist"; }
			if (role == DeviceExtRole.Arm_Handheld_Hand) { return "Arm_Handheld_Hand"; }

			if (role == DeviceExtRole.UpperBody_Wrist) { return "UpperBody_Wrist"; }
			if (role == DeviceExtRole.UpperBody_Handheld_Hand) { return "UpperBody_Handheld_Hand"; }

			if (role == DeviceExtRole.FullBody_Wrist_Ankle) { return "FullBody_Wrist_Ankle"; }
			if (role == DeviceExtRole.FullBody_Wrist_Foot) { return "FullBody_Wrist_Foot"; }
			if (role == DeviceExtRole.FullBody_Handheld_Hand_Ankle) { return "FullBody_Handheld_Hand_Ankle"; }
			if (role == DeviceExtRole.FullBody_Handheld_Hand_Foot) { return "FullBody_Handheld_Hand_Foot"; }

			if (role == DeviceExtRole.UpperBody_Handheld_Hand_Knee_Ankle) { return "UpperBody_Handheld_Hand_Knee_Ankle"; }

			if (role == DeviceExtRole.Unknown) { return "Unknown"; }

			sb.Clear().Append("DeviceExtRole = ").Append(role); DEBUG(sb);
			return "";
		}
		public static string Name(this BodyPoseRole role)
		{
			if (role == BodyPoseRole.Arm_Wrist) { return "Arm_Wrist"; }
			if (role == BodyPoseRole.Arm_Handheld) { return "Arm_Handheld"; }
			if (role == BodyPoseRole.Arm_Hand) { return "Arm_Hand"; }

			if (role == BodyPoseRole.UpperBody_Wrist) { return "UpperBody_Wrist"; }
			if (role == BodyPoseRole.UpperBody_Handheld) { return "UpperBody_Handheld"; }
			if (role == BodyPoseRole.UpperBody_Hand) { return "UpperBody_Hand"; }

			if (role == BodyPoseRole.FullBody_Wrist_Ankle) { return "FullBody_Wrist_Ankle"; }
			if (role == BodyPoseRole.FullBody_Wrist_Foot) { return "FullBody_Wrist_Foot"; }
			if (role == BodyPoseRole.FullBody_Handheld_Ankle) { return "FullBody_Handheld_Ankle"; }
			if (role == BodyPoseRole.FullBody_Handheld_Foot) { return "FullBody_Handheld_Foot"; }
			if (role == BodyPoseRole.FullBody_Hand_Ankle) { return "FullBody_Hand_Ankle"; }
			if (role == BodyPoseRole.FullBody_Hand_Foot) { return "FullBody_Hand_Foot"; }

			if (role == BodyPoseRole.UpperBody_Handheld_Knee_Ankle) { return "UpperBody_Handheld_Knee_Ankle"; }
			if (role == BodyPoseRole.UpperBody_Hand_Knee_Ankle) { return "UpperBody_Hand_Knee_Ankle"; }

			if (role == BodyPoseRole.Unknown) { return "Unknown"; }

			sb.Clear().Append("BodyPoseRole = ").Append(role); DEBUG(sb);
			return "";
		}
		public static string Name(this TrackedDeviceType type)
		{
			if (type == TrackedDeviceType.Controller) { return "Controller"; }
			if (type == TrackedDeviceType.Hand) { return "Hand"; }
			if (type == TrackedDeviceType.HMD) { return "HMD"; }
			if (type == TrackedDeviceType.ViveSelfTracker) { return "ViveSelfTracker"; }
			if (type == TrackedDeviceType.ViveSelfTrackerIM) { return "ViveSelfTrackerIM"; }
			if (type == TrackedDeviceType.ViveWristTracker) { return "WristTracker"; }

			if (type == TrackedDeviceType.Invalid) { return "Invalid"; }

			sb.Clear().Append("TrackedDeviceType = ").Append(type); DEBUG(sb);
			return "";
		}
		public static string Name(this BodyTrackingMode mode)
		{
			if (mode == BodyTrackingMode.ARMIK) { return "ARMIK"; }
			if (mode == BodyTrackingMode.FULLBODYIK) { return "FULLBODYIK"; }
			if (mode == BodyTrackingMode.LEGFK) { return "LEGFK"; }
			if (mode == BodyTrackingMode.LEGIK) { return "LEGIK"; }
			if (mode == BodyTrackingMode.SPINEIK) { return "SPINEIK"; }
			if (mode == BodyTrackingMode.UNKNOWNMODE) { return "Unknown"; }
			if (mode == BodyTrackingMode.UPPERBODYIK) { return "UPPERBODYIK"; }
			if (mode == BodyTrackingMode.UPPERIKANDLEGFK) { return "UPPERIKANDLEGFK"; }

			sb.Clear().Append("BodyTrackingMode = ").Append(mode); DEBUG(sb);
			return "";
		}
		public static string Name(this BodyTrackingResult result)
		{
			if (result == BodyTrackingResult.ERROR_AVATAR_INIT_FAILED) { return "ERROR_AVATAR_INIT_FAILED"; }
			if (result == BodyTrackingResult.ERROR_BODYTRACKINGMODE_NOT_ALIGNED) { return "ERROR_BODYTRACKINGMODE_NOT_ALIGNED"; }
			if (result == BodyTrackingResult.ERROR_BODYTRACKINGMODE_NOT_FOUND) { return "ERROR_BODYTRACKINGMODE_NOT_FOUND"; }
			if (result == BodyTrackingResult.ERROR_CALIBRATE_FAILED) { return "ERROR_CALIBRATE_FAILED"; }
			if (result == BodyTrackingResult.ERROR_COMPUTE_FAILED) { return "ERROR_COMPUTE_FAILED"; }
			if (result == BodyTrackingResult.ERROR_FATAL_ERROR) { return "ERROR_FATAL_ERROR"; }
			if (result == BodyTrackingResult.ERROR_IK_NOT_DESTROYED) { return "ERROR_IK_NOT_DESTROYED"; }
			if (result == BodyTrackingResult.ERROR_IK_NOT_UPDATED) { return "ERROR_IK_NOT_UPDATED"; }
			if (result == BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID) { return "ERROR_INPUTPOSE_NOT_VALID"; }
			if (result == BodyTrackingResult.ERROR_INVALID_ARGUMENT) { return "ERROR_INVALID_ARGUMENT"; }
			if (result == BodyTrackingResult.ERROR_JOINT_NOT_FOUND) { return "ERROR_JOINT_NOT_FOUND"; }
			if (result == BodyTrackingResult.ERROR_NOT_CALIBRATED) { return "ERROR_NOT_CALIBRATED"; }
			if (result == BodyTrackingResult.ERROR_SKELETONID_NOT_FOUND) { return "ERROR_SKELETONID_NOT_FOUND"; }
			if (result == BodyTrackingResult.ERROR_SOLVER_NOT_FOUND) { return "ERROR_SOLVER_NOT_FOUND"; }
			if (result == BodyTrackingResult.ERROR_TABLE_STATIC) { return "ERROR_TABLE_STATIC"; }
			if (result == BodyTrackingResult.ERROR_TRACKER_AMOUNT_FAILED) { return "ERROR_TRACKER_AMOUNT_FAILED"; }
			if (result == BodyTrackingResult.SUCCESS) { return "SUCCESS"; }

			sb.Clear().Append("BodyTrackingResult = ").Append(result); DEBUG(sb);
			return "";
		}
		public static string Name(this JointType type)
		{
			if (type == JointType.HIP) { return "HIP"; }

			if (type == JointType.LEFTTHIGH) { return "LEFTTHIGH"; }
			if (type == JointType.LEFTLEG) { return "LEFTLEG"; }
			if (type == JointType.LEFTANKLE) { return "LEFTANKLE"; }
			if (type == JointType.LEFTFOOT) { return "LEFTFOOT"; }

			if (type == JointType.RIGHTTHIGH) { return "RIGHTTHIGH"; }
			if (type == JointType.RIGHTLEG) { return "RIGHTLEG"; }
			if (type == JointType.RIGHTANKLE) { return "RIGHTANKLE"; }
			if (type == JointType.RIGHTFOOT) { return "RIGHTFOOT"; }

			if (type == JointType.WAIST) { return "WAIST"; }

			if (type == JointType.SPINELOWER) { return "SPINELOWER"; }
			if (type == JointType.SPINEMIDDLE) { return "SPINEMIDDLE"; }
			if (type == JointType.SPINEHIGH) { return "SPINEHIGH"; }

			if (type == JointType.CHEST) { return "CHEST"; }
			if (type == JointType.NECK) { return "NECK"; }
			if (type == JointType.HEAD) { return "HEAD"; }

			if (type == JointType.LEFTCLAVICLE) { return "LEFTCLAVICLE"; }
			if (type == JointType.LEFTSCAPULA) { return "LEFTSCAPULA"; }
			if (type == JointType.LEFTUPPERARM) { return "LEFTUPPERARM"; }
			if (type == JointType.LEFTFOREARM) { return "LEFTFOREARM"; }
			if (type == JointType.LEFTHAND) { return "LEFTHAND"; }

			if (type == JointType.RIGHTCLAVICLE) { return "RIGHTCLAVICLE"; }
			if (type == JointType.RIGHTSCAPULA) { return "RIGHTSCAPULA"; }
			if (type == JointType.RIGHTUPPERARM) { return "RIGHTUPPERARM"; }
			if (type == JointType.RIGHTFOREARM) { return "RIGHTFOREARM"; }
			if (type == JointType.RIGHTHAND) { return "RIGHTHAND"; }

			sb.Clear().Append("JointType = ").Append(type); DEBUG(sb);
			return "";
		}
		public static string Name(this CalibrationType type)
		{
			if (type == CalibrationType.DEFAULTCALIBRATION) { return "DEFAULTCALIBRATION"; }
			if (type == CalibrationType.HEIGHTCALIBRATION) { return "HEIGHTCALIBRATION"; }
			if (type == CalibrationType.TOFFSETCALIBRATION) { return "TOFFSETCALIBRATION"; }

			sb.Clear().Append("CalibrationType = ").Append(type); DEBUG(sb);
			return "";
		}

		public static bool GetQuaternionDiff(Quaternion src, Quaternion dst, out Quaternion diff)
		{
			if (src.IsValid() && dst.IsValid())
			{
				diff = Quaternion.Inverse(src) * dst;
				Rdp.Validate(ref diff);
				return true;
			}

			diff = Quaternion.identity;
			return false;
		}

		#region API v1.0.0.6
		public static string Name(this CalibrationStatus status)
		{
			if (status == CalibrationStatus.STATUS_UNINITIAL) { return "Uninitial"; }

			if (status == CalibrationStatus.STATUS_WAITING_STATIC) { return "Waiting Static"; }
			if (status == CalibrationStatus.STATUS_WAITING_POSE_MODE) { return "Waiting Pose Mode"; }
			if (status == CalibrationStatus.STATUS_READY) { return "Ready"; }
			if (status == CalibrationStatus.STATUS_COLLECTING) { return "Collecting"; }
			if (status == CalibrationStatus.STATUS_COLLECTED) { return "Collected"; }
			if (status == CalibrationStatus.STATUS_COLLECTED_AND_COMPUTING) { return "Collected and Computing"; }
			if (status == CalibrationStatus.STATUS_FINISHED) { return "Finished"; }

			if (status == CalibrationStatus.STATUS_WALKFAILED_DISTANCE) { return "Walk Failed Distance"; }
			if (status == CalibrationStatus.STATUS_WALKFAILED_TIME) { return "Walk Failed Time"; }
			if (status == CalibrationStatus.STATUS_WAIT_STATIC_FAILED_TIME) { return "Wait Static Failed Time"; }
			if (status == CalibrationStatus.STATUS_WAIT_POSEMODE_FAILED_TIME) { return "Wait Pose Mode Failed Time"; }
			if (status == CalibrationStatus.STATUS_READYFAILED_TIME) { return "Ready Failed Time"; }
			if (status == CalibrationStatus.STATUS_COMPUTEFAILED_TIME) { return "Compute Failed Time"; }
			if (status == CalibrationStatus.STATUS_COMPUTEFAILED) { return "Compute Failed"; }

			return "";
		}
		#endregion

		public static BodyTrackingResult Type(this Result result)
		{
			if (result == Result.ERROR_AVATAR_INIT_FAILED) { return BodyTrackingResult.ERROR_AVATAR_INIT_FAILED; }
			if (result == Result.ERROR_BODYTRACKINGMODE_NOT_ALIGNED) { return BodyTrackingResult.ERROR_BODYTRACKINGMODE_NOT_ALIGNED; }
			if (result == Result.ERROR_BODYTRACKINGMODE_NOT_FOUND) { return BodyTrackingResult.ERROR_BODYTRACKINGMODE_NOT_FOUND; }
			if (result == Result.ERROR_CALIBRATE_FAILED) { return BodyTrackingResult.ERROR_CALIBRATE_FAILED; }
			if (result == Result.ERROR_COMPUTE_FAILED) { return BodyTrackingResult.ERROR_COMPUTE_FAILED; }
			if (result == Result.ERROR_FATAL_ERROR) { return BodyTrackingResult.ERROR_FATAL_ERROR; }
			if (result == Result.ERROR_INPUTPOSE_NOT_VALID) { return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID; }
			if (result == Result.ERROR_JOINT_NOT_FOUND) { return BodyTrackingResult.ERROR_JOINT_NOT_FOUND; }
			if (result == Result.ERROR_NOT_CALIBRATED) { return BodyTrackingResult.ERROR_NOT_CALIBRATED; }
			if (result == Result.ERROR_NOT_INITIALIZATION) { return BodyTrackingResult.ERROR_NOT_INITIALIZATION; }
			if (result == Result.ERROR_SKELETONID_NOT_FOUND) { return BodyTrackingResult.ERROR_SKELETONID_NOT_FOUND; }
			if (result == Result.ERROR_SOLVER_NOT_FOUND) { return BodyTrackingResult.ERROR_SOLVER_NOT_FOUND; }
			if (result == Result.ERROR_TABLE_STATIC) { return BodyTrackingResult.ERROR_TABLE_STATIC; }
			if (result == Result.ERROR_TRACKER_AMOUNT_FAILED) { return BodyTrackingResult.ERROR_TRACKER_AMOUNT_FAILED; }

			return BodyTrackingResult.SUCCESS;
		}

		readonly static DateTime kBeginTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
		public static UInt64 GetTimeStamp(bool bflag = true)
		{
			TimeSpan ts = DateTime.UtcNow - kBeginTime;
			return Convert.ToUInt64(ts.TotalMilliseconds);
		}

		public readonly static TrackedDeviceType[] s_BodyTrackingTypes =
		{
			TrackedDeviceType.HMD,
			TrackedDeviceType.Controller,
			TrackedDeviceType.Hand,
			TrackedDeviceType.ViveWristTracker,
			TrackedDeviceType.ViveSelfTracker,
			TrackedDeviceType.ViveSelfTrackerIM,
		};

		/// <summary> Retrieves the body pose role according to the currently tracked device poses. </summary>
		public static BodyPoseRole GetBodyPoseRole([In] TrackedDevicePose[] trackedDevicePoses, [In] UInt32 trackedDevicePoseCount, bool printLog = true)
		{
			UInt64 ikRoles = 0;
			sb.Clear();
			for (UInt32 i = 0; i < trackedDevicePoseCount; i++)
			{
				sb.Append("GetBodyPoseRole() pose ").Append(i)
					.Append(" role ").Append(trackedDevicePoses[i].trackedDeviceRole.Name())
					.Append("\n");
#pragma warning disable CS0675 // Bitwise-or operator used on a sign-extended operand
				ikRoles |= (UInt64)(1 << (Int32)trackedDevicePoses[i].trackedDeviceRole);
#pragma warning restore CS0675 // Bitwise-or operator used on a sign-extended operand
			}
			if (printLog) DEBUG(sb);

			BodyPoseRole m_IKRoles = BodyPoseRole.Unknown;

			// Upper Body + Leg FK
			if ((ikRoles & (UInt64)BodyPoseRole.UpperBody_Handheld_Knee_Ankle) == (UInt64)BodyPoseRole.UpperBody_Handheld_Knee_Ankle)
				m_IKRoles = BodyPoseRole.UpperBody_Handheld_Knee_Ankle;
			else if ((ikRoles & (UInt64)BodyPoseRole.UpperBody_Hand_Knee_Ankle) == (UInt64)BodyPoseRole.UpperBody_Hand_Knee_Ankle)
				m_IKRoles = BodyPoseRole.UpperBody_Hand_Knee_Ankle;

			// ToDo: else if {Hybrid mode}

			// Full body
			else if ((ikRoles & (UInt64)BodyPoseRole.FullBody_Wrist_Ankle) == (UInt64)BodyPoseRole.FullBody_Wrist_Ankle)
				m_IKRoles = BodyPoseRole.FullBody_Wrist_Ankle;
			else if ((ikRoles & (UInt64)BodyPoseRole.FullBody_Wrist_Foot) == (UInt64)BodyPoseRole.FullBody_Wrist_Foot)
				m_IKRoles = BodyPoseRole.FullBody_Wrist_Foot;
			else if ((ikRoles & (UInt64)BodyPoseRole.FullBody_Handheld_Ankle) == (UInt64)BodyPoseRole.FullBody_Handheld_Ankle)
				m_IKRoles = BodyPoseRole.FullBody_Handheld_Ankle;
			else if ((ikRoles & (UInt64)BodyPoseRole.FullBody_Handheld_Foot) == (UInt64)BodyPoseRole.FullBody_Handheld_Foot)
				m_IKRoles = BodyPoseRole.FullBody_Handheld_Foot;
			else if ((ikRoles & (UInt64)BodyPoseRole.FullBody_Hand_Ankle) == (UInt64)BodyPoseRole.FullBody_Hand_Ankle)
				m_IKRoles = BodyPoseRole.FullBody_Hand_Ankle;
			else if ((ikRoles & (UInt64)BodyPoseRole.FullBody_Hand_Foot) == (UInt64)BodyPoseRole.FullBody_Hand_Foot)
				m_IKRoles = BodyPoseRole.FullBody_Hand_Foot;

			// Upper body
			else if ((ikRoles & (UInt64)BodyPoseRole.UpperBody_Wrist) == (UInt64)BodyPoseRole.UpperBody_Wrist)
				m_IKRoles = BodyPoseRole.UpperBody_Wrist;
			else if ((ikRoles & (UInt64)BodyPoseRole.UpperBody_Handheld) == (UInt64)BodyPoseRole.UpperBody_Handheld)
				m_IKRoles = BodyPoseRole.UpperBody_Handheld;
			else if ((ikRoles & (UInt64)BodyPoseRole.UpperBody_Hand) == (UInt64)BodyPoseRole.UpperBody_Hand)
				m_IKRoles = BodyPoseRole.UpperBody_Hand;

			// Arm
			else if ((ikRoles & (UInt64)BodyPoseRole.Arm_Wrist) == (UInt64)BodyPoseRole.Arm_Wrist)
				m_IKRoles = BodyPoseRole.Arm_Wrist;
			else if ((ikRoles & (UInt64)BodyPoseRole.Arm_Handheld) == (UInt64)BodyPoseRole.Arm_Handheld)
				m_IKRoles = BodyPoseRole.Arm_Handheld;
			else if ((ikRoles & (UInt64)BodyPoseRole.Arm_Hand) == (UInt64)BodyPoseRole.Arm_Hand)
				m_IKRoles = BodyPoseRole.Arm_Hand;

			if (printLog) { sb.Clear().Append("GetBodyPoseRole() role: ").Append(m_IKRoles.Name()); DEBUG(sb); }
			return m_IKRoles;
		}
		/// <summary> Checks if the body pose role and body tracking mode are matched./// </summary>
		public static bool MatchBodyTrackingMode(BodyTrackingMode mode, BodyPoseRole poseRole, bool printLog = true)
		{
			if (printLog) { sb.Clear().Append("MatchBodyTrackingMode() mode: ").Append(mode.Name()).Append(", poseRole: ").Append(poseRole.Name()); DEBUG(sb); }

			if (poseRole == BodyPoseRole.UpperBody_Handheld_Knee_Ankle || poseRole == BodyPoseRole.UpperBody_Hand_Knee_Ankle)
			{
				if (mode == BodyTrackingMode.UPPERIKANDLEGFK)
					return true;
			}
			if (poseRole == BodyPoseRole.FullBody_Wrist_Ankle || poseRole == BodyPoseRole.FullBody_Wrist_Foot ||
				poseRole == BodyPoseRole.FullBody_Handheld_Ankle || poseRole == BodyPoseRole.FullBody_Handheld_Foot ||
				poseRole == BodyPoseRole.FullBody_Hand_Ankle || poseRole == BodyPoseRole.FullBody_Hand_Foot)
			{
				if (mode == BodyTrackingMode.FULLBODYIK || mode == BodyTrackingMode.UPPERBODYIK || mode == BodyTrackingMode.ARMIK)
					return true;
			}
			if (poseRole == BodyPoseRole.UpperBody_Wrist || poseRole == BodyPoseRole.UpperBody_Handheld || poseRole == BodyPoseRole.UpperBody_Hand)
			{
				if (mode == BodyTrackingMode.UPPERBODYIK || mode == BodyTrackingMode.ARMIK)
					return true;
			}
			if (poseRole == BodyPoseRole.Arm_Wrist || poseRole == BodyPoseRole.Arm_Handheld || poseRole == BodyPoseRole.Arm_Hand)
			{
				if (mode == BodyTrackingMode.ARMIK)
					return true;
			}

			return false;
		}


		/// <summary> Retrievs the device extrinsic role according to the calibration pose role and tracked device extrinsics in use. </summary>
		public static DeviceExtRole GetDeviceExtRole(BodyPoseRole calibRole, [In] TrackedDeviceExtrinsic[] bodyTrackedDevices, [In] UInt32 bodyTrackedDeviceCount)
		{
			sb.Clear().Append("GetDeviceExtRole() calibRole: ").Append(calibRole.Name()); DEBUG(sb);

			UInt64 ikRoles = 0;
			sb.Clear();
			for (UInt32 i = 0; i < bodyTrackedDeviceCount; i++)
			{
				sb.Append("GetDeviceExtRole() device ").Append(i)
					.Append(" role ").Append(bodyTrackedDevices[i].trackedDeviceRole.Name())
					.Append("\n");
#pragma warning disable CS0675 // Bitwise-or operator used on a sign-extended operand
				ikRoles |= (UInt64)(1 << (Int32)bodyTrackedDevices[i].trackedDeviceRole);
#pragma warning restore CS0675 // Bitwise-or operator used on a sign-extended operand
			}
			DEBUG(sb);

			DeviceExtRole m_IKRoles = DeviceExtRole.Unknown;

			// Upper Body + Leg FK
			if (calibRole == BodyPoseRole.UpperBody_Handheld_Knee_Ankle || calibRole == BodyPoseRole.UpperBody_Hand_Knee_Ankle)
			{
				if ((ikRoles & (UInt64)DeviceExtRole.UpperBody_Handheld_Hand_Knee_Ankle) == (UInt64)DeviceExtRole.UpperBody_Handheld_Hand_Knee_Ankle)
					m_IKRoles = DeviceExtRole.UpperBody_Handheld_Hand_Knee_Ankle;
			}

			// Full Body
			if (calibRole == BodyPoseRole.FullBody_Wrist_Ankle)
			{
				if ((ikRoles & (UInt64)DeviceExtRole.FullBody_Wrist_Ankle) == (UInt64)DeviceExtRole.FullBody_Wrist_Ankle)
					m_IKRoles = DeviceExtRole.FullBody_Wrist_Ankle;
			}
			if (calibRole == BodyPoseRole.FullBody_Wrist_Foot)
			{
				if ((ikRoles & (UInt64)DeviceExtRole.FullBody_Wrist_Foot) == (UInt64)DeviceExtRole.FullBody_Wrist_Foot)
					m_IKRoles = DeviceExtRole.FullBody_Wrist_Foot;
			}
			if (calibRole == BodyPoseRole.FullBody_Handheld_Ankle || calibRole == BodyPoseRole.FullBody_Hand_Ankle)
			{
				if ((ikRoles & (UInt64)DeviceExtRole.FullBody_Handheld_Hand_Ankle) == (UInt64)DeviceExtRole.FullBody_Handheld_Hand_Ankle)
					m_IKRoles = DeviceExtRole.FullBody_Handheld_Hand_Ankle;
			}
			if (calibRole == BodyPoseRole.FullBody_Handheld_Foot || calibRole == BodyPoseRole.FullBody_Hand_Foot)
			{
				if ((ikRoles & (UInt64)DeviceExtRole.FullBody_Handheld_Hand_Foot) == (UInt64)DeviceExtRole.FullBody_Handheld_Hand_Foot)
					m_IKRoles = DeviceExtRole.FullBody_Handheld_Hand_Foot;
			}

			// Upper Body
			if (calibRole == BodyPoseRole.UpperBody_Wrist)
			{
				if ((ikRoles & (UInt64)DeviceExtRole.UpperBody_Wrist) == (UInt64)DeviceExtRole.UpperBody_Wrist)
					m_IKRoles = DeviceExtRole.UpperBody_Wrist;
			}
			if (calibRole == BodyPoseRole.UpperBody_Handheld || calibRole == BodyPoseRole.UpperBody_Hand)
			{
				if ((ikRoles & (UInt64)DeviceExtRole.UpperBody_Handheld_Hand) == (UInt64)DeviceExtRole.UpperBody_Handheld_Hand)
					m_IKRoles = DeviceExtRole.UpperBody_Handheld_Hand;
			}

			// Arm
			if (calibRole == BodyPoseRole.Arm_Wrist)
			{
				if ((ikRoles & (UInt64)DeviceExtRole.Arm_Wrist) == (UInt64)DeviceExtRole.Arm_Wrist)
					m_IKRoles = DeviceExtRole.Arm_Wrist;
			}
			if (calibRole == BodyPoseRole.Arm_Handheld || calibRole == BodyPoseRole.Arm_Hand)
			{
				if ((ikRoles & (UInt64)DeviceExtRole.Arm_Handheld_Hand) == (UInt64)DeviceExtRole.Arm_Handheld_Hand)
					m_IKRoles = DeviceExtRole.Arm_Handheld_Hand;
			}

			sb.Clear().Append("GetDeviceExtRole() role: ").Append(m_IKRoles.Name()); DEBUG(sb);
			return m_IKRoles;
		}
		/// <summary> Checks if the device extrinsic role and body tracking mode are matched./// </summary>
		public static bool MatchBodyTrackingMode(BodyTrackingMode mode, DeviceExtRole extRole)
		{
			sb.Clear().Append("MatchBodyTrackingMode() mode: ").Append(mode.Name()).Append(", extRole: ").Append(extRole.Name()); DEBUG(sb);

			if (mode == BodyTrackingMode.ARMIK)
			{
				if (extRole == DeviceExtRole.Arm_Wrist || extRole == DeviceExtRole.Arm_Handheld_Hand)
					return true;
			}
			if (mode == BodyTrackingMode.UPPERBODYIK)
			{
				if (extRole == DeviceExtRole.UpperBody_Wrist || extRole == DeviceExtRole.UpperBody_Handheld_Hand)
					return true;
			}
			if (mode == BodyTrackingMode.FULLBODYIK)
			{
				if (extRole == DeviceExtRole.FullBody_Wrist_Ankle ||
					extRole == DeviceExtRole.FullBody_Wrist_Foot ||
					extRole == DeviceExtRole.FullBody_Handheld_Hand_Ankle ||
					extRole == DeviceExtRole.FullBody_Handheld_Hand_Foot)
					return true;
			}
			if (mode == BodyTrackingMode.UPPERIKANDLEGFK)
			{
				if (extRole == DeviceExtRole.UpperBody_Handheld_Hand_Knee_Ankle)
					return true;
			}

			return false;
		}

		public static JointType GetJointType(int index)
		{
			if (index == 0) { return JointType.HIP; }
			if (index == 1) { return JointType.LEFTTHIGH; }
			if (index == 2) { return JointType.LEFTLEG; }
			if (index == 3) { return JointType.LEFTANKLE; }
			if (index == 4) { return JointType.LEFTFOOT; }

			if (index == 5) { return JointType.RIGHTTHIGH; }
			if (index == 6) { return JointType.RIGHTLEG; }
			if (index == 7) { return JointType.RIGHTANKLE; }
			if (index == 8) { return JointType.RIGHTFOOT; }

			if (index == 9) { return JointType.WAIST; }

			if (index == 10) { return JointType.SPINELOWER; }
			if (index == 11) { return JointType.SPINEMIDDLE; }
			if (index == 12) { return JointType.SPINEHIGH; }

			if (index == 13) { return JointType.CHEST; }
			if (index == 14) { return JointType.NECK; }
			if (index == 15) { return JointType.HEAD; }

			if (index == 16) { return JointType.LEFTCLAVICLE; }
			if (index == 17) { return JointType.LEFTSCAPULA; }
			if (index == 18) { return JointType.LEFTUPPERARM; }
			if (index == 19) { return JointType.LEFTFOREARM; }
			if (index == 20) { return JointType.LEFTHAND; }

			if (index == 21) { return JointType.RIGHTCLAVICLE; }
			if (index == 22) { return JointType.RIGHTSCAPULA; }
			if (index == 23) { return JointType.RIGHTUPPERARM; }
			if (index == 24) { return JointType.RIGHTFOREARM; }
			if (index == 25) { return JointType.RIGHTHAND; }

			return JointType.UNKNOWN;
		}
		public static string GetBodyJointName(int index)
		{
			if (index == 0) { return "Hip"; }
			if (index == 1) { return "Left_Thigh"; }
			if (index == 2) { return "Left_Leg"; }
			if (index == 3) { return "Left_Ankle"; }
			if (index == 4) { return "Left_Foot"; }
			if (index == 5) { return "Right_Thigh"; }
			if (index == 6) { return "Right_Leg"; }
			if (index == 7) { return "Right_Ankle"; }
			if (index == 8) { return "Right_Foot"; }
			if (index == 9) { return "Waist"; }
			if (index == 10) { return "Spine_Lower"; }
			if (index == 11) { return "Spine_Middle"; }
			if (index == 12) { return "Spine_High"; }
			if (index == 13) { return "Chest"; }
			if (index == 14) { return "Neck"; }
			if (index == 15) { return "Head"; }
			if (index == 16) { return "Left_Clavicle"; }
			if (index == 17) { return "Left_Scapula"; }
			if (index == 18) { return "Left_Upper_Arm"; }
			if (index == 19) { return "Left_Forearm"; }
			if (index == 20) { return "Left_Hand"; }
			if (index == 21) { return "Right_Clavicle"; }
			if (index == 22) { return "Right_Scapula"; }
			if (index == 23) { return "Right_Upper_Arm"; }
			if (index == 24) { return "Right_Forearm"; }
			if (index == 25) { return "Right_Hand"; }

			return "Unknown";
		}

		#region API v1.0.0.8
		// Translation
		public static float ConvertTranslation([In] Vector3 vec3, [In] AxisDefine axis)
		{
			if (axis == AxisDefine.Right) { return vec3.x; }
			if (axis == AxisDefine.Up) { return vec3.y; }
			if (axis == AxisDefine.Forward) { return vec3.z; }

			if (axis == AxisDefine.Left) { return -vec3.x; }
			if (axis == AxisDefine.Down) { return -vec3.y; }
			if (axis == AxisDefine.Backward) { return -vec3.z; }

			return 0;
		}
		public static void TransformCoordinate(ref Vector3 vec3, CoordinateDefine coor)
		{
			Vector3 v = vec3;
			v.x = ConvertTranslation(vec3, coor.x);
			v.y = ConvertTranslation(vec3, coor.y);
			v.z = ConvertTranslation(vec3, coor.z);
			vec3 = v;
		}
		// Rotation
		private static Matrix4x4 rotationMat = new Matrix4x4();
		/// <summary>
		/// Matrix layout
		/// x | x' y' z' 0
		/// y | x' y' z' 0
		/// z | x' y' z' 0
		/// w | 0  0  0  1
		/// 
		/// Row 0: Unity X axis mappings.
		/// Row 1: Unity Y axis mappings.
		/// Row 2: Unity Z axis mappings.
		/// 
		/// x', y', z': new coordinate axis.
		/// 
		/// For example 1: Unity x-axis is right, new coordinate y' is left.
		/// => -x = y', so row 0 is 0 -1 0 0
		/// For example 2: Unity y-axis is up, new coordinate z' is up.
		/// => y = z', sow row 1 is 0 0 1 0
		/// </summary>
		/// <param name="coor"></param>
		/// <returns></returns>
		public static Matrix4x4 GetRotationConvertMatrix(CoordinateDefine coor)
		{
			/// Row 0: axis right or left.
			// row0: 1 0 0 0
			if (coor.x == AxisDefine.Right)
			{
				rotationMat.m00 = 1;
				rotationMat.m01 = 0;
				rotationMat.m02 = 0;
				rotationMat.m03 = 0;
			}
			// row0: 0 1 0 0
			if (coor.y == AxisDefine.Right)
			{
				rotationMat.m00 = 0;
				rotationMat.m01 = 1;
				rotationMat.m02 = 0;
				rotationMat.m03 = 0;
			}
			// row0: 0 0 1 0
			if (coor.z == AxisDefine.Right)
			{
				rotationMat.m00 = 0;
				rotationMat.m01 = 0;
				rotationMat.m02 = 1;
				rotationMat.m03 = 0;
			}

			// row0: -1 0 0 0
			if (coor.x == AxisDefine.Left)
			{
				rotationMat.m00 = -1;
				rotationMat.m01 = 0;
				rotationMat.m02 = 0;
				rotationMat.m03 = 0;
			}
			// row0: 0 -1 0 0
			if (coor.y == AxisDefine.Left)
			{
				rotationMat.m00 = 0;
				rotationMat.m01 = -1;
				rotationMat.m02 = 0;
				rotationMat.m03 = 0;
			}
			// row0: 0 0 -1 0
			if (coor.z == AxisDefine.Left)
			{
				rotationMat.m00 = 0;
				rotationMat.m01 = 0;
				rotationMat.m02 = -1;
				rotationMat.m03 = 0;
			}

			/// Row 1: axis up or down
			// row1: 1 0 0 0
			if (coor.x == AxisDefine.Up)
			{
				rotationMat.m10 = 1;
				rotationMat.m11 = 0;
				rotationMat.m12 = 0;
				rotationMat.m13 = 0;
			}
			// row1: 0 1 0 0
			if (coor.y == AxisDefine.Up)
			{
				rotationMat.m10 = 0;
				rotationMat.m11 = 1;
				rotationMat.m12 = 0;
				rotationMat.m13 = 0;
			}
			// row1: 0 0 1 0
			if (coor.z == AxisDefine.Up)
			{
				rotationMat.m10 = 0;
				rotationMat.m11 = 0;
				rotationMat.m12 = 1;
				rotationMat.m13 = 0;
			}

			// row1: -1 0 0 0
			if (coor.x == AxisDefine.Down)
			{
				rotationMat.m10 = -1;
				rotationMat.m11 = 0;
				rotationMat.m12 = 0;
				rotationMat.m13 = 0;
			}
			// row1: 0 -1 0 0
			if (coor.y == AxisDefine.Down)
			{
				rotationMat.m10 = 0;
				rotationMat.m11 = -1;
				rotationMat.m12 = 0;
				rotationMat.m13 = 0;
			}
			// row1: 0 0 -1 0
			if (coor.z == AxisDefine.Down)
			{
				rotationMat.m10 = 0;
				rotationMat.m11 = 0;
				rotationMat.m12 = -1;
				rotationMat.m13 = 0;
			}

			// row2: 1 0 0 0
			if (coor.x == AxisDefine.Forward)
			{
				rotationMat.m20 = 1;
				rotationMat.m21 = 0;
				rotationMat.m22 = 0;
				rotationMat.m23 = 0;
			}
			// row2: 0 1 0 0
			if (coor.y == AxisDefine.Forward)
			{
				rotationMat.m20 = 0;
				rotationMat.m21 = 1;
				rotationMat.m22 = 0;
				rotationMat.m23 = 0;
			}
			// row2: 0 0 1 0
			if (coor.z == AxisDefine.Forward)
			{
				rotationMat.m20 = 0;
				rotationMat.m21 = 0;
				rotationMat.m22 = 1;
				rotationMat.m23 = 0;
			}

			// row2: -1 0 0 0
			if (coor.x == AxisDefine.Backward)
			{
				rotationMat.m20 = -1;
				rotationMat.m21 = 0;
				rotationMat.m22 = 0;
				rotationMat.m23 = 0;
			}
			// row2: 0 -1 0 0
			if (coor.y == AxisDefine.Backward)
			{
				rotationMat.m20 = 0;
				rotationMat.m21 = -1;
				rotationMat.m22 = 0;
				rotationMat.m23 = 0;
			}
			// row2: 0 0 -1 0
			if (coor.z == AxisDefine.Backward)
			{
				rotationMat.m20 = 0;
				rotationMat.m21 = 0;
				rotationMat.m22 = -1;
				rotationMat.m23 = 0;
			}

			rotationMat.m30 = 0;
			rotationMat.m31 = 0;
			rotationMat.m32 = 0;
			rotationMat.m33 = 1;

			return rotationMat;
		}
		public static void TransformCoordinate(ref Quaternion quat, CoordinateDefine coor)
		{
			Matrix4x4 inMatrix = Matrix4x4.TRS(Vector3.zero, quat, Vector3.one);

			Matrix4x4 convertMatrix = GetRotationConvertMatrix(coor);

			Matrix4x4 outMatrix = convertMatrix * inMatrix;

			quat = outMatrix.rotation;
			Rdp.Validate(ref quat);
		}
		// Angular Velocity
		public static Quaternion GetRotationQuaternion(float angleZ, float angleX, float angleY)
		{
			Quaternion rotationZ = Quaternion.Euler(0, 0, angleZ);
			Quaternion rotationX = Quaternion.Euler(angleX, 0, 0);
			Quaternion rotationY = Quaternion.Euler(0, angleY, 0);

			Quaternion toRotation = rotationY * rotationX * rotationZ;
			return toRotation;
		}
		public static void TransformCoordinate(ref Vector3 angularVelocity, Quaternion toRotation)
		{
			Quaternion inverseRotation = Quaternion.Inverse(toRotation);
			angularVelocity = inverseRotation * angularVelocity;
		}

		public static void TransformCoordinate(ref Joint joint, CoordinateDefine coorTranslation)
		{
			TransformCoordinate(ref joint.translation, coorTranslation);
			TransformCoordinate(ref joint.rotation, coorTranslation);
		}
		#endregion

#if WAVE_BODY_CALIBRATION
		public static string Name(this WVR_BodyTrackingDeviceRole role)
		{
			if (role == WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_Chest) { return "Chest"; }
			if (role == WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_Head) { return "Head"; }
			if (role == WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_Hip) { return "Hip"; }

			if (role == WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_LeftAnkle) { return "LeftAnkle"; }
			if (role == WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_LeftElbow) { return "LeftElbow"; }
			if (role == WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_LeftFoot) { return "LeftFoot"; }
			if (role == WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_LeftHand) { return "LeftHand"; }
			if (role == WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_LeftHandheld) { return "LeftHandheld"; }
			if (role == WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_LeftKnee) { return "LeftKnee"; }
			if (role == WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_LeftWrist) { return "LeftWrist"; }

			if (role == WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_RightAnkle) { return "RightAnkle"; }
			if (role == WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_RightElbow) { return "RightElbow"; }
			if (role == WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_RightFoot) { return "RightFoot"; }
			if (role == WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_RightHand) { return "RightHand"; }
			if (role == WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_RightHandheld) { return "RightHandheld"; }
			if (role == WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_RightKnee) { return "RightKnee"; }
			if (role == WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_RightWrist) { return "RightWrist"; }

			if (role == WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_Max) { return "Max"; }

			return "Invalid";
		}
		public static string Name(this WVR_BodyTrackingCalibrationMode mode)
		{
			if (mode == WVR_BodyTrackingCalibrationMode.WVR_BodyTrackingCalibrationMode_ArmIK) { return "ArmIK"; }
			if (mode == WVR_BodyTrackingCalibrationMode.WVR_BodyTrackingCalibrationMode_FullBodyIK) { return "FullBodyIK"; }
			if (mode == WVR_BodyTrackingCalibrationMode.WVR_BodyTrackingCalibrationMode_LegFK) { return "LegFK"; }
			if (mode == WVR_BodyTrackingCalibrationMode.WVR_BodyTrackingCalibrationMode_LegIK) { return "LegIK"; }
			if (mode == WVR_BodyTrackingCalibrationMode.WVR_BodyTrackingCalibrationMode_SpineIK) { return "SpineIK"; }
			if (mode == WVR_BodyTrackingCalibrationMode.WVR_BodyTrackingCalibrationMode_SpineIKAndLegFK) { return "SpineIKAndLegFK"; }
			if (mode == WVR_BodyTrackingCalibrationMode.WVR_BodyTrackingCalibrationMode_Unknown) { return "Unknown"; }
			if (mode == WVR_BodyTrackingCalibrationMode.WVR_BodyTrackingCalibrationMode_UpperBodyIK) { return "UpperBodyIK"; }
			if (mode == WVR_BodyTrackingCalibrationMode.WVR_BodyTrackingCalibrationMode_UpperIKAndLegFK) { return "UpperIKAndLegFK"; }

			return "";
		}

		public static bool UseDeviceExtrinsic(BodyPoseRole calibRole, WVR_BodyTrackingDeviceRole wvrRole, TrackedDeviceType wvrType)
		{
			if (wvrRole == WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_Head)
				return true;

			if (wvrRole == WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_LeftWrist || wvrRole == WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_RightWrist)
			{
				if (calibRole == BodyPoseRole.Arm_Wrist ||
					calibRole == BodyPoseRole.UpperBody_Wrist ||
					calibRole == BodyPoseRole.FullBody_Wrist_Ankle ||
					calibRole == BodyPoseRole.FullBody_Wrist_Foot)
				{
					if (wvrType == TrackedDeviceType.ViveSelfTracker ||
						wvrType == TrackedDeviceType.ViveWristTracker)
						return true;
				}
			}
			if (wvrRole == WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_Hip)
			{
				if (calibRole == BodyPoseRole.UpperBody_Wrist || calibRole == BodyPoseRole.UpperBody_Handheld || calibRole == BodyPoseRole.UpperBody_Hand ||
					
					calibRole == BodyPoseRole.UpperBody_Handheld_Knee_Ankle || calibRole == BodyPoseRole.UpperBody_Hand_Knee_Ankle ||
					
					calibRole == BodyPoseRole.FullBody_Wrist_Ankle || calibRole == BodyPoseRole.FullBody_Wrist_Foot ||
					calibRole == BodyPoseRole.FullBody_Handheld_Ankle || calibRole == BodyPoseRole.FullBody_Handheld_Foot ||
					calibRole == BodyPoseRole.FullBody_Hand_Ankle || calibRole == BodyPoseRole.FullBody_Handheld_Foot)
				{
					if (wvrType == TrackedDeviceType.ViveSelfTracker)
						return true;
				}
			}

			return false;
		}

		public static TrackedDeviceRole Role(this WVR_BodyTrackingDeviceRole role)
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
		public static WVR_BodyTrackingDeviceRole wvrRole(this TrackedDeviceRole role)
		{
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

		public static WVR_BodyTrackingCalibrationMode GetBodyTrackingMode(this BodyPoseRole role)
		{
			if (role == BodyPoseRole.Arm_Wrist ||
				role == BodyPoseRole.Arm_Handheld ||
				role == BodyPoseRole.Arm_Hand)
			{
				return WVR_BodyTrackingCalibrationMode.WVR_BodyTrackingCalibrationMode_ArmIK;
			}
			if (role == BodyPoseRole.UpperBody_Wrist ||
				role == BodyPoseRole.UpperBody_Handheld ||
				role == BodyPoseRole.UpperBody_Hand)
			{
				return WVR_BodyTrackingCalibrationMode.WVR_BodyTrackingCalibrationMode_UpperBodyIK;
			}
			if (role == BodyPoseRole.FullBody_Wrist_Ankle ||
				role == BodyPoseRole.FullBody_Wrist_Foot ||
				role == BodyPoseRole.FullBody_Handheld_Ankle ||
				role == BodyPoseRole.FullBody_Handheld_Foot ||
				role == BodyPoseRole.FullBody_Hand_Ankle ||
				role == BodyPoseRole.FullBody_Hand_Foot)
			{
				return WVR_BodyTrackingCalibrationMode.WVR_BodyTrackingCalibrationMode_FullBodyIK;
			}
			if (role == BodyPoseRole.UpperBody_Handheld_Knee_Ankle ||
				role == BodyPoseRole.UpperBody_Hand_Knee_Ankle)
			{
				return WVR_BodyTrackingCalibrationMode.WVR_BodyTrackingCalibrationMode_UpperIKAndLegFK;
			}

			return WVR_BodyTrackingCalibrationMode.WVR_BodyTrackingCalibrationMode_Unknown;
		}

		public static BodyTrackingMode Mode(this WVR_BodyTrackingCalibrationMode mode)
		{
			if (mode == WVR_BodyTrackingCalibrationMode.WVR_BodyTrackingCalibrationMode_ArmIK) { return BodyTrackingMode.ARMIK; }
			if (mode == WVR_BodyTrackingCalibrationMode.WVR_BodyTrackingCalibrationMode_UpperBodyIK) { return BodyTrackingMode.UPPERBODYIK; }
			if (mode == WVR_BodyTrackingCalibrationMode.WVR_BodyTrackingCalibrationMode_FullBodyIK) { return BodyTrackingMode.FULLBODYIK; }
			if (mode == WVR_BodyTrackingCalibrationMode.WVR_BodyTrackingCalibrationMode_UpperIKAndLegFK) { return BodyTrackingMode.UPPERIKANDLEGFK; }
			// Other modes are not supported.

			return BodyTrackingMode.UNKNOWNMODE;
		}
		public static WVR_BodyTrackingCalibrationMode wvrMode(this BodyTrackingMode mode)
		{
			if (mode == BodyTrackingMode.ARMIK) { return WVR_BodyTrackingCalibrationMode.WVR_BodyTrackingCalibrationMode_ArmIK; }
			if (mode == BodyTrackingMode.UPPERBODYIK) { return WVR_BodyTrackingCalibrationMode.WVR_BodyTrackingCalibrationMode_UpperBodyIK; }
			if (mode == BodyTrackingMode.FULLBODYIK) { return WVR_BodyTrackingCalibrationMode.WVR_BodyTrackingCalibrationMode_FullBodyIK; }
			if (mode == BodyTrackingMode.UPPERIKANDLEGFK) { return WVR_BodyTrackingCalibrationMode.WVR_BodyTrackingCalibrationMode_UpperIKAndLegFK; }
			// Other modes are not supported.

			return WVR_BodyTrackingCalibrationMode.WVR_BodyTrackingCalibrationMode_Unknown;
		}
#endif

#if WAVE_BODY_IK
		public static string Name(this WVR_BodyJoint joint)
		{
			if (joint == WVR_BodyJoint.WVR_BodyJoint_Hip) { return "Hip"; }
			if (joint == WVR_BodyJoint.WVR_BodyJoint_Left_Thigh) { return "Left_Thigh"; }
			if (joint == WVR_BodyJoint.WVR_BodyJoint_Left_Leg) { return "Left_Leg"; }
			if (joint == WVR_BodyJoint.WVR_BodyJoint_Left_Ankle) { return "Left_Ankle"; }
			if (joint == WVR_BodyJoint.WVR_BodyJoint_Left_Foot) { return "Left_Foot"; }
			if (joint == WVR_BodyJoint.WVR_BodyJoint_Right_Thigh) { return "Right_Thigh"; }
			if (joint == WVR_BodyJoint.WVR_BodyJoint_Right_Leg) { return "Right_Leg"; }
			if (joint == WVR_BodyJoint.WVR_BodyJoint_Right_Ankle) { return "Right_Ankle"; }
			if (joint == WVR_BodyJoint.WVR_BodyJoint_Right_Foot) { return "Right_Foot"; }
			if (joint == WVR_BodyJoint.WVR_BodyJoint_Waist) { return "Waist"; }
			if (joint == WVR_BodyJoint.WVR_BodyJoint_Spine_Lower) { return "Spine_Lower"; }
			if (joint == WVR_BodyJoint.WVR_BodyJoint_Spine_Middle) { return "Spine_Middle"; }
			if (joint == WVR_BodyJoint.WVR_BodyJoint_Spine_High) { return "Spine_High"; }
			if (joint == WVR_BodyJoint.WVR_BodyJoint_Chest) { return "Chest"; }
			if (joint == WVR_BodyJoint.WVR_BodyJoint_Neck) { return "Neck"; }
			if (joint == WVR_BodyJoint.WVR_BodyJoint_Head) { return "Head"; }
			if (joint == WVR_BodyJoint.WVR_BodyJoint_Left_Clavicle) { return "Left_Clavicle"; }
			if (joint == WVR_BodyJoint.WVR_BodyJoint_Left_Scapula) { return "Left_Scapula"; }
			if (joint == WVR_BodyJoint.WVR_BodyJoint_Left_Upper_Arm) { return "Left_Upper_Arm"; }
			if (joint == WVR_BodyJoint.WVR_BodyJoint_Left_Forearm) { return "Left_Forearm"; }
			if (joint == WVR_BodyJoint.WVR_BodyJoint_Left_Hand) { return "Left_Hand"; }
			if (joint == WVR_BodyJoint.WVR_BodyJoint_Right_Clavicle) { return "Right_Clavicle"; }
			if (joint == WVR_BodyJoint.WVR_BodyJoint_Right_Scapula) { return "Right_Scapula"; }
			if (joint == WVR_BodyJoint.WVR_BodyJoint_Right_Upper_Arm) { return "Right_Upper_Arm"; }
			if (joint == WVR_BodyJoint.WVR_BodyJoint_Right_Forearm) { return "Right_Forearm"; }
			if (joint == WVR_BodyJoint.WVR_BodyJoint_Right_Hand) { return "Right_Hand"; }

			return "Unknown";
		}
#endif
	}

	public static class Numeric
	{
		public static bool IsBoolean(string value)
		{
			try
			{
				bool i = Convert.ToBoolean(value);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public static bool IsFloat(string value)
		{
			try
			{
				float i = Convert.ToSingle(value);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public static bool IsNumeric(string value)
		{
			try
			{
				int i = Convert.ToInt32(value);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public static bool IsValid(this Quaternion quat)
		{
			if (quat.x > 1 || quat.x < -1 || float.IsNaN(quat.x)) { return false; }
			if (quat.y > 1 || quat.y < -1 || float.IsNaN(quat.y)) { return false; }
			if (quat.z > 1 || quat.z < -1 || float.IsNaN(quat.z)) { return false; }
			if (quat.w > 1 || quat.w < -1 || float.IsNaN(quat.w)) { return false; }
			return true;
		}
	} // class Numeric
}
