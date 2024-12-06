// Copyright HTC Corporation All Rights Reserved.

using System;
using System.Collections;
using System.Text;
using System.Threading;
using UnityEngine;

using VIVE.OpenXR.Toolkits.BodyTracking.RuntimeDependency;

namespace VIVE.OpenXR.Toolkits.BodyTracking.Demo
{
	public class BodyIKSample : MonoBehaviour
	{
		#region Rdp
		const string LOG_TAG = "VIVE.OpenXR.Toolkits.BodyTracking.Demo.BodyIKSample";
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
		int logFrame = -1;
		bool printIntervalLog = false;
		void WARNING(StringBuilder msg) { Rdp.w(LOG_TAG, msg, true); }
		void ERROR(StringBuilder msg) { Rdp.e(LOG_TAG, msg, true); }
		#endregion

		#region Life Cycle
		public enum TrackingStatus
		{
			// Not tracking, can call CreateBodyTracking in this state.
			NotStart,
			StartFailure,

			// Processing, should NOT call API in this state.
			Starting,
			Stopping,

			// Tracking, can call DestroyBodyTracking in this state.
			Available,

			// Do nothing
			Unsupported
		}
		private TrackingStatus m_TrackingStatus = TrackingStatus.NotStart;
		private static ReaderWriterLockSlim m_TrackingStatusRWLock = new ReaderWriterLockSlim();
		public TrackingStatus GetTrackingStatus()
		{
			try
			{
				m_TrackingStatusRWLock.TryEnterReadLock(2000);
				return m_TrackingStatus;
			}
			catch (Exception e)
			{
				sb.Clear().Append("GetTrackingStatus() ").Append(e.Message); ERROR(sb);
				throw;
			}
			finally
			{
				m_TrackingStatusRWLock.ExitReadLock();
			}
		}
		private void SetTrackingStatus(TrackingStatus status)
		{
			try
			{
				m_TrackingStatusRWLock.TryEnterWriteLock(2000);
				m_TrackingStatus = status;
			}
			catch (Exception e)
			{
				sb.Clear().Append("SetTrackingStatus() ").Append(e.Message); ERROR(sb);
				throw;
			}
			finally
			{
				m_TrackingStatusRWLock.ExitWriteLock();
			}
		}
		private bool CanStartTracking()
		{
			TrackingStatus status = GetTrackingStatus();
			if (status == TrackingStatus.NotStart || status == TrackingStatus.StartFailure) { return true; }
			sb.Clear().Append("CanStartTracking() Cannot start tracking, status: ").Append(status); WARNING(sb);
			return false;
		}
		private bool CanStopTracking()
		{
			TrackingStatus status = GetTrackingStatus();
			if (status == TrackingStatus.Available) { return true; }
			sb.Clear().Append("CanStopTracking() Cannot stop tracking, status: ").Append(status); WARNING(sb);
			return false;
		}
		#endregion

		#region Inspector
		public Body inputBody;
		public Transform avatarOffset = null;
		private Body m_InitialBody = null;
		private TransformData m_InitialTransform;
		#endregion

		private void Awake()
		{
			sb.Clear().Append("Awake() Records the initial body position and scale."); DEBUG(sb);
			m_InitialTransform = new TransformData(transform);

			if (m_InitialBody == null)
			{
				sb.Clear().Append("Awake() Records the initial standard pose."); DEBUG(sb);
				m_InitialBody = new Body();
				m_InitialBody.UpdateData(inputBody);
			}
		}
		private void Update()
		{
			logFrame++;
			logFrame %= 300;
			printIntervalLog = (logFrame == 0);
		}
		private void OnDisable()
		{
			sb.Clear().Append("OnDisable()"); DEBUG(sb);
			StopTracking();
		}

		bool updateTrackingData = false;
		public void BeginTracking()
		{
			if (!CanStartTracking()) { return; }

			sb.Clear().Append("BeginTracking()"); DEBUG(sb);

			/// State machine NotStart/StartFailure -> Starting
			SetTrackingStatus(TrackingStatus.Starting);
			StartCoroutine(StartBodyTracking());
		}
		public void StopTracking()
		{
			if (!CanStopTracking()) { return; }

			/// State machine Available -> Stopping
			SetTrackingStatus(TrackingStatus.Stopping);
			updateTrackingData = false;

			sb.Clear().Append("StopTracking() Recovers the initial standard pose, body position and scale."); DEBUG(sb);
			if (inputBody != null && m_InitialBody != null) { m_InitialBody.UpdateBody(ref inputBody); }
			RecoverBodyScale();
			RecoverBodyOffset();
		}

		private void ApplyBodyScale(float scale)
		{
			transform.localScale *= scale;
		}
		private void RecoverBodyScale()
		{
			transform.localScale = m_InitialTransform.localScale;
		}
		private void ApplyBodyOffsetEachFrame(Transform offset)
		{
			if (offset != null)
			{
				transform.localPosition = offset.rotation * transform.localPosition;
				transform.localPosition += offset.position;
				transform.localRotation *= offset.rotation;
			}
		}
		private void RecoverBodyOffset()
		{
			transform.localPosition = m_InitialTransform.localPosition;
			transform.localRotation = m_InitialTransform.localRotation;
		}
		private int m_TrackerId = -1;
		private BodyAvatar avatarBody = null;
		private IEnumerator StartBodyTracking()
		{
			/// State machine Starting -> StartFailure
			if (BodyManager.Instance == null || inputBody == null)
			{
				SetTrackingStatus(TrackingStatus.StartFailure);
				yield break;
			}
			sb.Clear().Append("StartBodyTracking()"); DEBUG(sb);

			// Creates a body tracker.
			BodyTrackingResult result = BodyTrackingResult.ERROR_FATAL_ERROR;
#if WAVE_BODY_IK
			result = BodyManager.Instance.InitAvatarIK(null, out m_TrackerId);
#endif
			sb.Clear().Append("StartBodyTracking() InitAvatarIK(").Append(m_TrackerId).Append(") result: ").Append(result.Name()); DEBUG(sb);
			/// State machine Starting -> StartFailure
			if (result != BodyTrackingResult.SUCCESS)
			{
				SetTrackingStatus(TrackingStatus.StartFailure);
				yield break;
			}

			float userCalibrationHeight = 0, avatarScale = 0;
#if WAVE_BODY_IK
			// Default Body Tracking does NOT provide the user calibration height so we have to retrieve the user's height.
			Vector3 headPos = Vector3.zero;
			if (Rdp.Head.GetPosition(ref headPos)) { userCalibrationHeight = headPos.y; }

			if (userCalibrationHeight > 0 && inputBody.height > 0) { avatarScale = userCalibrationHeight / inputBody.height; }
			result = (avatarScale > 0 ? BodyTrackingResult.SUCCESS : BodyTrackingResult.ERROR_INVALID_ARGUMENT);
#endif
			sb.Clear().Append("StartBodyTracking() GetAvatarIKInfo result ").Append(result.Name()).Append(", userCalibrationHeight: ").Append(userCalibrationHeight).Append(", avatarScale: ").Append(avatarScale); DEBUG(sb);
			if (result == BodyTrackingResult.SUCCESS)
			{
				// Due to the pose from GetAvatarIKData is "scaled pose", we need to change the avatar mesh size first.
				// The userCalibrationHeight is user's height in calibration.
				// The m_InitialBody.height is the height of avatar used in this content.
				// Due to the avatarScale in Body Tracking is always 1, so we calculate the scale with (user height / avatar height).
				float scale = userCalibrationHeight / m_InitialBody.height;
				sb.Clear().Append("StartBodyTracking() Apply avatar scale with ").Append(scale); DEBUG(sb);
				ApplyBodyScale(scale);

				/// State machine Starting -> Available
				SetTrackingStatus(TrackingStatus.Available); // Tracking is available then going into the loop for retrieving poses.
				updateTrackingData = true;
				while (updateTrackingData)
				{
#if WAVE_BODY_IK
					result = BodyManager.Instance.GetAvatarIKData(m_TrackerId, out BodyAvatar avatarBody);
#endif
					if (result == BodyTrackingResult.SUCCESS)
					{
						RecoverBodyOffset();
						UpdateBodyPosesInOrder(avatarBody, null, 0);
						ApplyBodyOffsetEachFrame(avatarOffset);
					}
					yield return new WaitForEndOfFrame();
				}
			}

#if WAVE_BODY_IK
			result = BodyManager.Instance.DestroyAvatarIK(m_TrackerId);
#endif
			sb.Clear().Append("StartBodyTracking() DestroyAvatarIK(").Append(m_TrackerId).Append(") result: ").Append(result.Name()); DEBUG(sb);
			yield return null; // waits next frame

			/// State machine Stopping -> NotStart
			SetTrackingStatus(TrackingStatus.NotStart); // Resets the tracking status last.
		}

		/// <summary>
		/// Update the body joints poses according to the avatar joint order.
		/// If your avatar joint order is different, you have to modify this function.
		/// </summary>
		/// <param name="avatarBody">The avatar IK pose from plugin.</param>
		private void UpdateBodyPosesInOrder(BodyAvatar avatarBody, RotateSpace[] rotationSpaces, UInt32 rotationSpaceCount)
		{
			if (inputBody == null || m_InitialBody == null || avatarBody == null) { return; }
			if (printIntervalLog)
			{
				sb.Clear().Append("UpdateBodyPosesInOrder() new avatar height ").Append(avatarBody.height)
					.Append(", original avatar height ").Append(m_InitialBody.height)
					.Append(", scale: ").Append(avatarBody.scale);
				DEBUG(sb);
			}

			if (inputBody.root != null)
			{
				avatarBody.Update(JointType.HIP, ref inputBody.root);
				if (rotationSpaces != null) UpdateBodyTransform(JointType.HIP, rotationSpaces, rotationSpaceCount, m_InitialBody.HipData.rotation, ref inputBody.root);
			}

			if (inputBody.leftThigh != null)
			{
				inputBody.leftThigh.rotation = avatarBody.leftThigh.rotation;
				if (rotationSpaces != null) UpdateBodyTransform(JointType.LEFTTHIGH, rotationSpaces, rotationSpaceCount, m_InitialBody.LeftThighData.rotation, ref inputBody.leftThigh);
			}
			if (inputBody.leftLeg != null)
			{
				inputBody.leftLeg.rotation = avatarBody.leftLeg.rotation;
				if (rotationSpaces != null) UpdateBodyTransform(JointType.LEFTLEG, rotationSpaces, rotationSpaceCount, m_InitialBody.LeftLegData.rotation, ref inputBody.leftLeg);
			}
			if (inputBody.leftAnkle != null)
			{
				inputBody.leftAnkle.rotation = avatarBody.leftAnkle.rotation;
				if (rotationSpaces != null) UpdateBodyTransform(JointType.LEFTANKLE, rotationSpaces, rotationSpaceCount, m_InitialBody.LeftAnkleData.rotation, ref inputBody.leftAnkle);
			}
			if (inputBody.leftFoot != null)
			{
				inputBody.leftFoot.rotation = avatarBody.leftFoot.rotation;
				if (rotationSpaces != null) UpdateBodyTransform(JointType.LEFTFOOT, rotationSpaces, rotationSpaceCount, m_InitialBody.LeftFootData.rotation, ref inputBody.leftFoot);
			}

			if (inputBody.rightThigh != null)
			{
				inputBody.rightThigh.rotation = avatarBody.rightThigh.rotation;
				if (rotationSpaces != null) UpdateBodyTransform(JointType.RIGHTTHIGH, rotationSpaces, rotationSpaceCount, m_InitialBody.RightThighData.rotation, ref inputBody.rightThigh);
			}
			if (inputBody.rightLeg != null)
			{
				inputBody.rightLeg.rotation = avatarBody.rightLeg.rotation;
				if (rotationSpaces != null) UpdateBodyTransform(JointType.RIGHTLEG, rotationSpaces, rotationSpaceCount, m_InitialBody.RightLegData.rotation, ref inputBody.rightLeg);
			}
			if (inputBody.rightAnkle != null)
			{
				inputBody.rightAnkle.rotation = avatarBody.rightAnkle.rotation;
				if (rotationSpaces != null) UpdateBodyTransform(JointType.RIGHTANKLE, rotationSpaces, rotationSpaceCount, m_InitialBody.RightAnkleData.rotation, ref inputBody.rightAnkle);
			}
			if (inputBody.rightFoot != null)
			{
				inputBody.rightFoot.rotation = avatarBody.rightFoot.rotation;
				if (rotationSpaces != null) UpdateBodyTransform(JointType.RIGHTFOOT, rotationSpaces, rotationSpaceCount, m_InitialBody.RightFootData.rotation, ref inputBody.rightFoot);
			}

			if (inputBody.waist != null)
			{
				inputBody.waist.rotation = avatarBody.waist.rotation;
				if (rotationSpaces != null) UpdateBodyTransform(JointType.WAIST, rotationSpaces, rotationSpaceCount, m_InitialBody.WaistData.rotation, ref inputBody.waist);
			}

			if (inputBody.spineLower != null)
			{
				inputBody.spineLower.rotation = avatarBody.spineLower.rotation;
				if (rotationSpaces != null) UpdateBodyTransform(JointType.SPINELOWER, rotationSpaces, rotationSpaceCount, m_InitialBody.SpineLowerData.rotation, ref inputBody.spineLower);
			}
			if (inputBody.spineMiddle != null)
			{
				inputBody.spineMiddle.rotation = avatarBody.spineMiddle.rotation;
				if (rotationSpaces != null) UpdateBodyTransform(JointType.SPINEMIDDLE, rotationSpaces, rotationSpaceCount, m_InitialBody.SpineMiddleData.rotation, ref inputBody.spineMiddle);
			}
			if (inputBody.spineHigh != null)
			{
				inputBody.spineHigh.rotation = avatarBody.spineHigh.rotation;
				if (rotationSpaces != null) UpdateBodyTransform(JointType.SPINEHIGH, rotationSpaces, rotationSpaceCount, m_InitialBody.SpineHighData.rotation, ref inputBody.spineHigh);
			}

			if (inputBody.chest != null)
			{
				inputBody.chest.rotation = avatarBody.chest.rotation;
				if (rotationSpaces != null) UpdateBodyTransform(JointType.CHEST, rotationSpaces, rotationSpaceCount, m_InitialBody.ChestData.rotation, ref inputBody.chest);
			}
			if (inputBody.neck != null)
			{
				inputBody.neck.rotation = avatarBody.neck.rotation;
				if (rotationSpaces != null) UpdateBodyTransform(JointType.NECK, rotationSpaces, rotationSpaceCount, m_InitialBody.NeckData.rotation, ref inputBody.neck);
			}
			if (inputBody.head != null)
			{
				inputBody.head.rotation = avatarBody.head.rotation;
				if (rotationSpaces != null) UpdateBodyTransform(JointType.HEAD, rotationSpaces, rotationSpaceCount, m_InitialBody.HeadData.rotation, ref inputBody.head);
			}

			if (inputBody.leftClavicle != null)
			{
				inputBody.leftClavicle.rotation = avatarBody.leftClavicle.rotation;
				if (rotationSpaces != null) UpdateBodyTransform(JointType.LEFTCLAVICLE, rotationSpaces, rotationSpaceCount, m_InitialBody.LeftClavicleData.rotation, ref inputBody.leftClavicle);
			}
			if (inputBody.leftScapula != null)
			{
				inputBody.leftScapula.rotation = avatarBody.leftScapula.rotation;
				if (rotationSpaces != null) UpdateBodyTransform(JointType.LEFTSCAPULA, rotationSpaces, rotationSpaceCount, m_InitialBody.LeftScapulaData.rotation, ref inputBody.leftScapula);
			}
			if (inputBody.leftUpperarm != null)
			{
				inputBody.leftUpperarm.rotation = avatarBody.leftUpperarm.rotation;
				if (rotationSpaces != null) UpdateBodyTransform(JointType.LEFTUPPERARM, rotationSpaces, rotationSpaceCount, m_InitialBody.LeftUpperarmData.rotation, ref inputBody.leftUpperarm);
			}
			if (inputBody.leftForearm != null)
			{
				inputBody.leftForearm.rotation = avatarBody.leftForearm.rotation;
				if (rotationSpaces != null) UpdateBodyTransform(JointType.LEFTFOREARM, rotationSpaces, rotationSpaceCount, m_InitialBody.LeftForearmData.rotation, ref inputBody.leftForearm);
			}
			if (inputBody.leftHand != null)
			{
				inputBody.leftHand.rotation = avatarBody.leftHand.rotation;
				if (rotationSpaces != null) UpdateBodyTransform(JointType.LEFTHAND, rotationSpaces, rotationSpaceCount, m_InitialBody.LeftHandData.rotation, ref inputBody.leftHand);
			}

			if (inputBody.rightClavicle != null)
			{
				inputBody.rightClavicle.rotation = avatarBody.rightClavicle.rotation;
				if (rotationSpaces != null) UpdateBodyTransform(JointType.RIGHTCLAVICLE, rotationSpaces, rotationSpaceCount, m_InitialBody.RightClavicleData.rotation, ref inputBody.rightClavicle);
			}
			if (inputBody.rightScapula != null)
			{
				inputBody.rightScapula.rotation = avatarBody.rightScapula.rotation;
				if (rotationSpaces != null) UpdateBodyTransform(JointType.RIGHTSCAPULA, rotationSpaces, rotationSpaceCount, m_InitialBody.RightScapulaData.rotation, ref inputBody.rightScapula);
			}
			if (inputBody.rightUpperarm != null)
			{
				inputBody.rightUpperarm.rotation = avatarBody.rightUpperarm.rotation;
				if (rotationSpaces != null) UpdateBodyTransform(JointType.RIGHTUPPERARM, rotationSpaces, rotationSpaceCount, m_InitialBody.RightUpperarmData.rotation, ref inputBody.rightUpperarm);
			}
			if (inputBody.rightForearm != null)
			{
				inputBody.rightForearm.rotation = avatarBody.rightForearm.rotation;
				if (rotationSpaces != null) UpdateBodyTransform(JointType.RIGHTFOREARM, rotationSpaces, rotationSpaceCount, m_InitialBody.RightForearmData.rotation, ref inputBody.rightForearm);
			}
			if (inputBody.rightHand != null)
			{
				inputBody.rightHand.rotation = avatarBody.rightHand.rotation;
				if (rotationSpaces != null) UpdateBodyTransform(JointType.RIGHTHAND, rotationSpaces, rotationSpaceCount, m_InitialBody.RightHandData.rotation, ref inputBody.rightHand);
			}
		}
		private void UpdateBodyTransform(JointType type, RotateSpace[] rotationSpaces, UInt32 rotationSpaceCount, Quaternion customRot, ref Transform joint)
		{
			if (joint == null) { return; }

			if (printIntervalLog)
			{
				sb.Clear().Append("UpdateJoint() ").Append(type.Name())
					.Append(", rotTPose(").Append(customRot.x).Append(", ").Append(customRot.y).Append(", ").Append(customRot.z).Append(", ").Append(customRot.w).Append(")");
				DEBUG(sb);
				sb.Clear().Append(", rotation before (").Append(joint.rotation.x).Append(", ").Append(joint.rotation.y).Append(", ").Append(joint.rotation.z).Append(", ").Append(joint.rotation.w).Append(")");
				DEBUG(sb);
			}

			// and apply the rotation space to body tracking avatar joint rotation.
			UInt32 index = 0;
			Quaternion diff = Quaternion.identity;
			for (index = 0; index < rotationSpaceCount; index++)
			{
				if (rotationSpaces[index].jointType == type)
				{
					// Calculate the rotation diff from default rotation space to custom avatar standard rotation.
					if (BodyTrackingUtils.GetQuaternionDiff(rotationSpaces[index].rotation, customRot, out diff))
					{
						// Apply the joint rotation with rotation diff.
						joint.rotation *= diff;
					}
					break;
				}
			}

			if (printIntervalLog)
			{
				sb.Clear().Append(", rotation diff (").Append(diff.x).Append(", ").Append(diff.y).Append(", ").Append(diff.z).Append(", ").Append(diff.w).Append(")");
				DEBUG(sb);
				sb.Clear().Append(", rotation after (").Append(joint.rotation.x).Append(", ").Append(joint.rotation.y).Append(", ").Append(joint.rotation.z).Append(", ").Append(joint.rotation.w).Append(")");
				DEBUG(sb);
			}
		}
	}
}
