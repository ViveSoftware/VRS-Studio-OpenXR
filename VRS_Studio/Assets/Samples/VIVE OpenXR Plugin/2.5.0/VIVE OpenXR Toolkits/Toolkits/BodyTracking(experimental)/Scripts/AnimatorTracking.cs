// Copyright HTC Corporation All Rights Reserved.

using System;
using System.Collections;
using System.Text;
using System.Threading;
using UnityEngine;
using VIVE.OpenXR.Toolkits.BodyTracking.AvatarCoordinate;
using VIVE.OpenXR.Toolkits.BodyTracking.RuntimeDependency;

namespace VIVE.OpenXR.Toolkits.BodyTracking
{
	[RequireComponent(typeof(Animator))]
	public class AnimatorTracking : MonoBehaviour
	{
		#region Log
		const string LOG_TAG = "VIVE.OpenXR.Toolkits.BodyTracking.AnimatorTracking";
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

		public enum TrackingMode : Int32
		{
			/// <summary> Tracking only head and arms. </summary>
			Arm = BodyTrackingMode.ARMIK,
			/// <summary> Tracking head, arms and hip. </summary>
			UpperBody = BodyTrackingMode.UPPERBODYIK,
			/// <summary> Tracking head, arms, hip and ankles. </summary>
			FullBody = BodyTrackingMode.FULLBODYIK,
			/// <summary> Tracking head, arms, hip, knees and ankles. </summary>
			UpperBodyAndLeg = BodyTrackingMode.UPPERIKANDLEGFK,
		}

		#region Inspector
		[SerializeField]
		private bool m_ControlByGesture = false;
		public bool ControlByGesture { get { return m_ControlByGesture; } set { m_ControlByGesture = value; } }

		[SerializeField]
		private TrackingMode m_Tracking = TrackingMode.UpperBodyAndLeg;
		public TrackingMode Tracking { get { return m_Tracking; } set { m_Tracking = value; } }

		[SerializeField]
		private bool m_CustomSettings = false;
		public bool CustomSettings { get { return m_CustomSettings; } set { m_CustomSettings = value; } }

		[SerializeField]
		private float m_AvatarHeight = 1.5f;
		public float AvatarHeight
		{
			get { return m_AvatarHeight; }
			set
			{
				if (value > 0) { m_AvatarHeight = value; }
			}
		}

		[SerializeField]
		private Transform m_AvatarOffset = null;
		public Transform AvatarOffset { get { return m_AvatarOffset; } set { m_AvatarOffset = value; } }

		[SerializeField]
		[Range(0.2f, 5f)]
		private float m_AvatarScale = 1;
		public float AvatarScale { get { return m_AvatarScale; } set { m_AvatarScale = value; } }

		[SerializeField]
		private AvatarCoordinateProducer m_JointCoordinate = null;
		public AvatarCoordinateProducer JointCoordinate => m_JointCoordinate;

		[SerializeField]
		private bool m_CustomizeExtrinsics = false;
		public bool CustomizeExtrinsics { get { return m_CustomizeExtrinsics; } set { m_CustomizeExtrinsics = value; } }

		/// Animator Head
		[SerializeField]
		private ExtrinsicInfo_t m_Head = new ExtrinsicInfo_t(true, wvr.extHeadT);
		public ExtrinsicInfo_t Head => m_Head;

		/// Animator Hand
		[SerializeField]
		private ExtrinsicInfo_t m_LeftWrist = new ExtrinsicInfo_t(true, wvr.extSelfTracker_Wrist_LeftT);
		public ExtrinsicInfo_t LeftWrist => m_LeftWrist;
		[SerializeField]
		private ExtrinsicInfo_t m_RightWrist = new ExtrinsicInfo_t(true, wvr.extSelfTracker_Wrist_RightT);
		public ExtrinsicInfo_t RightWrist => m_RightWrist;

		/// Animator Hand
		[SerializeField]
		private ExtrinsicInfo_t m_LeftHandheld = new ExtrinsicInfo_t(true, wvr.extController_Handheld_LeftT);
		public ExtrinsicInfo_t LeftHandheld => m_LeftHandheld;
		[SerializeField]
		private ExtrinsicInfo_t m_RightHandheld = new ExtrinsicInfo_t(true, wvr.extController_Handheld_RightT);
		public ExtrinsicInfo_t RightHandheld => m_RightHandheld;

		/// Animator Hand
		[SerializeField]
		private ExtrinsicInfo_t m_LeftHand = new ExtrinsicInfo_t(true, wvr.extHand_Hand_LeftT);
		public ExtrinsicInfo_t LeftHand => m_LeftHand;
		[SerializeField]
		private ExtrinsicInfo_t m_RightHand = new ExtrinsicInfo_t(true, wvr.extHand_Hand_RightT);
		public ExtrinsicInfo_t RightHand => m_RightHand;

		/// Animator Hips
		[SerializeField]
		private ExtrinsicInfo_t m_Hips = new ExtrinsicInfo_t(true, wvr.extSelfTracker_HipT);
		public ExtrinsicInfo_t Hips => m_Hips;

		/// Animator LowerLeg = TrackedDeviceRole Knee
		[SerializeField]
		private ExtrinsicInfo_t m_LeftLowerLeg = new ExtrinsicInfo_t(true, wvr.extSelfTrackerIM_Knee_LeftT);
		public ExtrinsicInfo_t LeftLowerLeg => m_LeftLowerLeg;
		[SerializeField]
		private ExtrinsicInfo_t m_RightLowerLeg = new ExtrinsicInfo_t(true, wvr.extSelfTrackerIM_Knee_RightT);
		public ExtrinsicInfo_t RightLowerLeg => m_RightLowerLeg;

		/// Animator Foot = TrackedDeviceRole Ankle
		[SerializeField]
		private ExtrinsicInfo_t m_LeftFoot = new ExtrinsicInfo_t(true, wvr.extSelfTrackerIM_Ankle_LeftT);
		public ExtrinsicInfo_t LeftFoot => m_LeftFoot;
		[SerializeField]
		private ExtrinsicInfo_t m_RightFoot = new ExtrinsicInfo_t(true, wvr.extSelfTrackerIM_Ankle_RightT);
		public ExtrinsicInfo_t RightFoot => m_RightFoot;

		/// Animator Toes = TrackedDeviceRole Foot
		[SerializeField]
		private ExtrinsicInfo_t m_LeftToes = new ExtrinsicInfo_t(true, wvr.extSelfTracker_Foot_LeftT);
		public ExtrinsicInfo_t LeftToes => m_LeftToes;
		[SerializeField]
		private ExtrinsicInfo_t m_RightToes = new ExtrinsicInfo_t(true, wvr.extSelfTracker_Foot_RightT);
		public ExtrinsicInfo_t RightToes => m_RightToes;
		#endregion

		private Animator m_Animator = null;
		private Body m_Body = null, m_InitialBody = null;
		private TransformData m_InitialTransform;
		/// <summary> HumanBodyBones should have at least 20 joints in function. </summary>
		private bool AssignAnimatorToBody(ref Body body)
		{
			if (body == null) { return false; }

			m_Animator = GetComponent<Animator>();
			if (m_Animator == null || m_Animator.avatar == null || !m_Animator.avatar.isValid || !m_Animator.avatar.isHuman)
			{
				sb.Clear().Append("AssignAnimatorToBody() no HumanBodyBones."); ERROR(sb);
				return false;
			}

			// 0.hip
			else { body.root = m_Animator.GetBoneTransform(HumanBodyBones.Hips); }
			if (body.root == null)
			{
				sb.Clear().Append("AssignAnimatorToBody() no HumanBodyBones Hips."); ERROR(sb);
				return false;
			}
			sb.Clear().Append("AssignAnimatorToBody() HumanBodyBones Hips -> Body root, name: ").Append(body.root.gameObject.name); DEBUG(sb);

			// 1.leftThigh
			body.leftThigh = m_Animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
			if (body.leftThigh == null)
			{
				sb.Clear().Append("AssignAnimatorToBody() no HumanBodyBones LeftUpperLeg."); ERROR(sb);
				return false;
			}
			sb.Clear().Append("AssignAnimatorToBody() HumanBodyBones LeftUpperLeg -> Body leftThigh."); DEBUG(sb);
			// 2.leftLeg
			body.leftLeg = m_Animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
			if (body.leftLeg == null)
			{
				sb.Clear().Append("AssignAnimatorToBody() no HumanBodyBones LeftLowerLeg."); ERROR(sb);
				return false;
			}
			sb.Clear().Append("AssignAnimatorToBody() HumanBodyBones LeftLowerLeg -> Body leftLeg."); DEBUG(sb);
			// 3.leftAnkle
			body.leftAnkle = m_Animator.GetBoneTransform(HumanBodyBones.LeftFoot);
			if (body.leftAnkle == null)
			{
				sb.Clear().Append("AssignAnimatorToBody() no HumanBodyBones LeftFoot."); ERROR(sb);
				return false;
			}
			sb.Clear().Append("AssignAnimatorToBody() HumanBodyBones LeftFoot -> Body leftAnkle."); DEBUG(sb);
			// 4.leftFoot
			body.leftFoot = m_Animator.GetBoneTransform(HumanBodyBones.LeftToes);
			if (body.leftFoot == null)
			{
				sb.Clear().Append("AssignAnimatorToBody() no HumanBodyBones LeftToes."); ERROR(sb);
				return false;
			}
			sb.Clear().Append("AssignAnimatorToBody() HumanBodyBones LeftToes -> Body leftFoot."); DEBUG(sb);

			// 5.rightThigh
			body.rightThigh = m_Animator.GetBoneTransform(HumanBodyBones.RightUpperLeg);
			if (body.rightThigh == null)
			{
				sb.Clear().Append("AssignAnimatorToBody() no HumanBodyBones RightUpperLeg."); ERROR(sb);
				return false;
			}
			sb.Clear().Append("AssignAnimatorToBody() HumanBodyBones RightUpperLeg -> Body rightThigh."); DEBUG(sb);
			// 6.rightLeg
			body.rightLeg = m_Animator.GetBoneTransform(HumanBodyBones.RightLowerLeg);
			if (body.rightLeg == null)
			{
				sb.Clear().Append("AssignAnimatorToBody() no HumanBodyBones RightLowerLeg."); ERROR(sb);
				return false;
			}
			sb.Clear().Append("AssignAnimatorToBody() HumanBodyBones RightLowerLeg -> Body rightLeg."); DEBUG(sb);
			// 7.rightAnkle
			body.rightAnkle = m_Animator.GetBoneTransform(HumanBodyBones.RightFoot);
			if (body.rightAnkle == null)
			{
				sb.Clear().Append("AssignAnimatorToBody() no HumanBodyBones RightFoot."); ERROR(sb);
				return false;
			}
			sb.Clear().Append("AssignAnimatorToBody() HumanBodyBones RightFoot -> Body rightAnkle."); DEBUG(sb);
			// 8.rightFoot
			body.rightFoot = m_Animator.GetBoneTransform(HumanBodyBones.RightToes);
			if (body.rightFoot == null)
			{
				sb.Clear().Append("AssignAnimatorToBody() no HumanBodyBones RightToes."); ERROR(sb);
				return false;
			}
			sb.Clear().Append("AssignAnimatorToBody() HumanBodyBones RightToes -> Body rightFoot."); DEBUG(sb);

			// 13.chest
			body.chest = m_Animator.GetBoneTransform(HumanBodyBones.Spine);
			if (body.chest == null)
			{
				sb.Clear().Append("AssignAnimatorToBody() no HumanBodyBones Spine."); ERROR(sb);
				return false;
			}
			else
			{
				// (UpperChest) -> (Chest) -> Spine
				body.chest = m_Animator.GetBoneTransform(HumanBodyBones.UpperChest);
				if (body.chest != null)
				{
					// Assign (UpperChest) to chest
					sb.Clear().Append("AssignAnimatorToBody() HumanBodyBones UpperChest -> Body chest."); DEBUG(sb);

					body.spineHigh = m_Animator.GetBoneTransform(HumanBodyBones.Chest);
					if (body.spineHigh != null)
					{
						// Assign (Chest) to spineHigh
						sb.Clear().Append("AssignAnimatorToBody() HumanBodyBones Chest -> Body spineHigh."); DEBUG(sb);

						// Assign Spine to spineLower
						body.spineLower = m_Animator.GetBoneTransform(HumanBodyBones.Spine);
						sb.Clear().Append("AssignAnimatorToBody() HumanBodyBones Spine -> Body spineLower."); DEBUG(sb);
					}
					else
					{
						// Assign Spine to spineHigh
						body.spineHigh = m_Animator.GetBoneTransform(HumanBodyBones.Spine);
						sb.Clear().Append("AssignAnimatorToBody() HumanBodyBones Spine -> Body spineHigh."); DEBUG(sb);
					}
				}
				else
				{
					body.chest = m_Animator.GetBoneTransform(HumanBodyBones.Chest);
					if (body.chest != null)
					{
						// Assign Chest to chest
						sb.Clear().Append("AssignAnimatorToBody() HumanBodyBones Chest -> Body chest."); DEBUG(sb);

						// Assign Spine to spineHigh
						body.spineHigh = m_Animator.GetBoneTransform(HumanBodyBones.Spine);
						sb.Clear().Append("AssignAnimatorToBody() HumanBodyBones Spine -> Body spineHigh."); DEBUG(sb);
					}
					else
					{
						// Assign Spine to chest
						body.chest = m_Animator.GetBoneTransform(HumanBodyBones.Spine);
						sb.Clear().Append("AssignAnimatorToBody() HumanBodyBones Spine -> Body chest."); DEBUG(sb);
					}
				}

			}

			// 14.neck
			body.neck = m_Animator.GetBoneTransform(HumanBodyBones.Neck);
			if (body.neck == null)
			{
				sb.Clear().Append("AssignAnimatorToBody() no HumanBodyBones Neck."); ERROR(sb);
				return false;
			}
			sb.Clear().Append("AssignAnimatorToBody() HumanBodyBones Neck -> Body neck."); DEBUG(sb);
			// 15.head
			body.head = m_Animator.GetBoneTransform(HumanBodyBones.Head);
			if (body.head == null)
			{
				sb.Clear().Append("AssignAnimatorToBody() no HumanBodyBones Head."); ERROR(sb);
				return false;
			}
			sb.Clear().Append("AssignAnimatorToBody() HumanBodyBones Head -> Body head."); DEBUG(sb);

			// 16.leftClavicle
			body.leftClavicle = m_Animator.GetBoneTransform(HumanBodyBones.LeftShoulder);
			if (body.leftClavicle == null)
			{
				sb.Clear().Append("AssignAnimatorToBody() no HumanBodyBones LeftShoulder."); ERROR(sb);
				return false;
			}
			sb.Clear().Append("AssignAnimatorToBody() HumanBodyBones LeftShoulder -> Body leftClavicle."); DEBUG(sb);
			// 18.leftUpperarm
			body.leftUpperarm = m_Animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
			if (body.leftUpperarm == null)
			{
				sb.Clear().Append("AssignAnimatorToBody() no HumanBodyBones LeftUpperArm."); ERROR(sb);
				return false;
			}
			sb.Clear().Append("AssignAnimatorToBody() HumanBodyBones LeftUpperArm -> Body leftUpperarm."); DEBUG(sb);
			// 19.leftForearm
			body.leftForearm = m_Animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
			if (body.leftForearm == null)
			{
				sb.Clear().Append("AssignAnimatorToBody() no HumanBodyBones LeftLowerArm."); ERROR(sb);
				return false;
			}
			sb.Clear().Append("AssignAnimatorToBody() HumanBodyBones LeftLowerArm -> Body leftForearm."); DEBUG(sb);
			// 20.leftHand
			body.leftHand = m_Animator.GetBoneTransform(HumanBodyBones.LeftHand);
			if (body.leftHand == null)
			{
				sb.Clear().Append("AssignAnimatorToBody() no HumanBodyBones LeftHand."); ERROR(sb);
				return false;
			}
			sb.Clear().Append("AssignAnimatorToBody() HumanBodyBones LeftHand -> Body leftHand."); DEBUG(sb);

			// 21.rightClavicle
			body.rightClavicle = m_Animator.GetBoneTransform(HumanBodyBones.RightShoulder);
			if (body.rightClavicle == null)
			{
				sb.Clear().Append("AssignAnimatorToBody() no HumanBodyBones RightShoulder."); ERROR(sb);
				return false;
			}
			sb.Clear().Append("AssignAnimatorToBody() HumanBodyBones RightShoulder -> Body rightClavicle."); DEBUG(sb);
			// 23.rightUpperarm
			body.rightUpperarm = m_Animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
			if (body.rightUpperarm == null)
			{
				sb.Clear().Append("AssignAnimatorToBody() no HumanBodyBones RightUpperArm."); ERROR(sb);
				return false;
			}
			sb.Clear().Append("AssignAnimatorToBody() HumanBodyBones RightUpperArm -> Body rightUpperarm."); DEBUG(sb);
			// 24.rightForearm
			body.rightForearm = m_Animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
			if (body.rightForearm == null)
			{
				sb.Clear().Append("AssignAnimatorToBody() no HumanBodyBones RightLowerArm."); ERROR(sb);
				return false;
			}
			sb.Clear().Append("AssignAnimatorToBody() HumanBodyBones RightLowerArm -> Body rightForearm."); DEBUG(sb);
			// 25.rightHand
			body.rightHand = m_Animator.GetBoneTransform(HumanBodyBones.RightHand);
			if (body.rightHand == null)
			{
				sb.Clear().Append("AssignAnimatorToBody() no HumanBodyBones RightHand."); ERROR(sb);
				return false;
			}
			sb.Clear().Append("AssignAnimatorToBody() HumanBodyBones RightHand -> Body rightHand."); DEBUG(sb);

			if (m_CustomSettings)
			{
				body.height = m_AvatarHeight;

				sb.Clear().Append("AssignAnimatorToBody() height: ").Append(body.height);
				DEBUG(sb);
			}
			else
			{
				float floor = Mathf.Min(body.leftFoot.position.y, body.rightFoot.position.y);
				body.height = body.head.position.y - floor;

				sb.Clear().Append("AssignAnimatorToBody() Calculates height:")
					.Append(" LeftToes (").Append(body.leftFoot.position.y).Append(")")
					.Append(", RightToes(").Append(body.rightFoot.position.y).Append(")")
					.Append(", Head(").Append(body.head.position.y).Append(")")
					.Append(", height: ").Append(body.height);
				DEBUG(sb);
			}

			return true;
		}

		private TrackerExtrinsic m_CustomExts = new TrackerExtrinsic();
		private void Awake()
		{
			if (m_CustomizeExtrinsics)
			{
				sb.Clear().Append("Awake() Customize device extrinsics."); DEBUG(sb);
				m_CustomExts.Update(TrackedDeviceRole.ROLE_HEAD, m_Head);

				m_CustomExts.Update(TrackedDeviceRole.ROLE_LEFTWRIST, m_LeftWrist);
				m_CustomExts.Update(TrackedDeviceRole.ROLE_RIGHTWRIST, m_RightWrist);
				m_CustomExts.Update(TrackedDeviceRole.ROLE_LEFTHANDHELD, m_LeftHandheld);
				m_CustomExts.Update(TrackedDeviceRole.ROLE_RIGHTHANDHELD, m_RightHandheld);
				m_CustomExts.Update(TrackedDeviceRole.ROLE_LEFTHAND, m_LeftHand);
				m_CustomExts.Update(TrackedDeviceRole.ROLE_RIGHTHAND, m_RightHand);

				m_CustomExts.Update(TrackedDeviceRole.ROLE_HIP, m_Hips);

				m_CustomExts.Update(TrackedDeviceRole.ROLE_LEFTKNEE, m_LeftLowerLeg);
				m_CustomExts.Update(TrackedDeviceRole.ROLE_RIGHTKNEE, m_RightLowerLeg);

				m_CustomExts.Update(TrackedDeviceRole.ROLE_LEFTANKLE, m_LeftFoot);
				m_CustomExts.Update(TrackedDeviceRole.ROLE_RIGHTANKLE, m_RightFoot);

				m_CustomExts.Update(TrackedDeviceRole.ROLE_LEFTFOOT, m_LeftToes);
				m_CustomExts.Update(TrackedDeviceRole.ROLE_RIGHTFOOT, m_RightToes);
			}
			sb.Clear().Append("Awake() Records the initial body position and scale."); DEBUG(sb);
			m_InitialTransform = new TransformData(transform);

			if (m_Body == null)
			{
				sb.Clear().Append("Awake() Configures Animator avatar."); DEBUG(sb);
				m_Body = new Body();
				if (!AssignAnimatorToBody(ref m_Body))
				{
					sb.Clear().Append("Awake() AssignHumanoidToBody failed."); ERROR(sb);
					m_Body = null;
					return;
				}
			}
			if (m_InitialBody == null)
			{
				sb.Clear().Append("Awake() Records the initial standard pose."); DEBUG(sb);
				m_InitialBody = new Body();
				m_InitialBody.UpdateData(m_Body);
			}
		}
		private void Update()
		{
			logFrame++;
			logFrame %= 300;
			printIntervalLog = (logFrame == 0);

			if (m_ControlByGesture && (logFrame % 90 == 0))
			{
				if (Rdp.Hand.IsGestureOK(true) && Rdp.Hand.IsGestureOK(false))
				{
#if !WAVE_BODY_IK
					BeginCalibration();
#endif
					sb.Clear().Append("Update() OK to BeginTracking."); DEBUG(sb);
					BeginTracking();
				}
				if (Rdp.Hand.IsGestureLike(true) && Rdp.Hand.IsGestureLike(false))
				{
#if !WAVE_BODY_IK
					StopCalibration();
#endif
					sb.Clear().Append("Update() Like to StopTracking."); DEBUG(sb);
					StopTracking();
				}
			}
		}
		private void OnDisable()
		{
			StopTracking();
		}

#if !WAVE_BODY_IK
		public void BeginCalibration(CalibrationStatusDelegate callback = null)
		{
			if (BodyManager.Instance == null) { return; }

			sb.Clear().Append("BeginCalibration() ").Append(m_Tracking); DEBUG(sb);
			BodyManager.Instance.StartCalibration((BodyTrackingMode)m_Tracking, callback);
		}
		public void StopCalibration()
		{
			if (BodyManager.Instance == null) { return; }

			sb.Clear().Append("StopCalibration() ").Append(m_Tracking); DEBUG(sb);
			BodyManager.Instance.StopCalibration((BodyTrackingMode)m_Tracking);
		}
#endif

		bool updateTrackingData = false;
		public void BeginTracking()
		{
			if (!CanStartTracking()) { return; }

			sb.Clear().Append("BeginTracking() tracking mode: ").Append(m_Tracking); DEBUG(sb);

			/// State machine NotStart/StartFailure -> Starting
			SetTrackingStatus(TrackingStatus.Starting);
			StartCoroutine(StartFixUpdateBodyTracking());
		}
		public void StopTracking()
		{
			if (!CanStopTracking()) { return; }

			/// State machine Available -> Stopping
			SetTrackingStatus(TrackingStatus.Stopping);
			updateTrackingData = false;

			sb.Clear().Append("StopTracking() Recovers the initial standard pose, body position and scale."); DEBUG(sb);
			if (m_Body != null && m_InitialBody != null) { m_InitialBody.UpdateBody(ref m_Body); }
			RecoverBodyScale();
			RecoverBodyOffset();
#if !WAVE_BODY_IK
			StopCalibration();
#endif
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
		private int m_AvatarID = -1;
		public IEnumerator StartFixUpdateBodyTracking()
		{
			/// State machine Starting -> StartFailure
			if (BodyManager.Instance == null)
			{
				SetTrackingStatus(TrackingStatus.StartFailure);
				yield break;
			}
			sb.Clear().Append("StartFixUpdateBodyTracking()"); DEBUG(sb);

			BodyTrackingResult result = BodyTrackingResult.ERROR_FATAL_ERROR;
#if WAVE_BODY_IK
			// Creates a body tracker.
			result = BodyManager.Instance.InitAvatarIK(m_Body, out m_AvatarID);
			sb.Clear().Append("StartFixUpdateBodyTracking() InitAvatarIK(").Append(m_AvatarID).Append(") result ").Append(result.Name()); DEBUG(sb);
#else
			if (!m_CustomizeExtrinsics)
			{
				sb.Clear().Append("StartFixUpdateBodyTracking() CreateBodyTracking with custom avatar + standard extrinsics."); DEBUG(sb);
				result = BodyManager.Instance.CreateBodyTracking(ref m_AvatarID, m_Body, (BodyTrackingMode)m_Tracking);
			}
			else
			{
				sb.Clear().Append("StartFixUpdateBodyTracking() CreateBodyTracking with custom avatar + custom extrinsics."); DEBUG(sb);
				result = BodyManager.Instance.CreateBodyTracking(ref m_AvatarID, m_Body, m_CustomExts, (BodyTrackingMode)m_Tracking);
			}
			sb.Clear().Append("StartFixUpdateBodyTracking() CreateBodyTracking result ").Append(result.Name()).Append(", id: ").Append(m_AvatarID); DEBUG(sb);
#endif
			/// State machine Starting -> StartFailure
			if (result != BodyTrackingResult.SUCCESS)
			{
				SetTrackingStatus(TrackingStatus.StartFailure);
				yield break;
			}

#if WAVE_BODY_IK
			yield return new WaitForSeconds(3);
			result = BodyManager.Instance.GetAvatarIKInfo(m_AvatarID, out float avatarHeight, out float avatarScale);
			sb.Clear().Append("StartFixUpdateBodyTracking() GetAvatarIKInfo result ").Append(result.Name()).Append(", avatarHeight: ").Append(avatarHeight).Append(", avatarScale: ").Append(avatarScale); DEBUG(sb);
#else
			result = BodyManager.Instance.GetBodyTrackingInfo(m_AvatarID, out float avatarHeight, out float avatarScale);
			sb.Clear().Append("StartFixUpdateBodyTracking() GetBodyTrackingInfo result ").Append(result.Name()); DEBUG(sb);
#endif
			if (result == BodyTrackingResult.SUCCESS)
			{
				// Due to the pose from GetAvatarIKData is "scaled pose", we need to change the avatar mesh size first.
				// The avatarHeight is user's height in calibration.
				// The m_InitialBody.height is the height of avatar used in this content.
				sb.Clear().Append("StartFixUpdateBodyTracking() Apply avatar scale with ").Append(avatarScale); DEBUG(sb);
				ApplyBodyScale(avatarScale * m_AvatarScale);

				/// State machine Starting -> Available
				SetTrackingStatus(TrackingStatus.Available); // Tracking is available then going into the loop for retrieving poses.
				updateTrackingData = true;
				while (updateTrackingData)
				{
#if WAVE_BODY_IK
					result = BodyManager.Instance.GetAvatarIKData(m_AvatarID, out BodyAvatar avatarBody);
#else
					result = BodyManager.Instance.GetBodyTrackingPoseOnce(m_AvatarID, out BodyAvatar avatarBody);
#endif
					if (result == BodyTrackingResult.SUCCESS)
					{
						if (BodyManager.Instance.EnableTrackingLog)
						{
							sb.Clear().Append("StartFixUpdateBodyTracking() avatarBody confidence: ").Append(avatarBody.confidence);
							DEBUG(sb);
						}
						RecoverBodyOffset();
						UpdateBodyPosesInOrder(avatarBody, m_AvatarScale);
						ApplyBodyOffsetEachFrame(m_AvatarOffset);
					}
					yield return new WaitForEndOfFrame();
				}
			}

			result = BodyManager.Instance.DestroyBodyTracking(m_AvatarID);
			sb.Clear().Append("StartFixUpdateBodyTracking() DestroyBodyTracking result ").Append(result.Name()).Append(", id: ").Append(m_AvatarID); DEBUG(sb);
			yield return null; // waits next frame

			/// State machine Stopping -> NotStart
			SetTrackingStatus(TrackingStatus.NotStart); // Resets the tracking status last.
		}

		/// <summary>
		/// Update the body joints poses according to the avatar joint order.
		/// If your avatar joint order is different, you have to modify this function.
		/// </summary>
		/// <param name="avatarBody">The avatar IK pose from plugin.</param>
		private void UpdateBodyPosesInOrder(BodyAvatar avatarBody, float scale = 1)
		{
			if (m_Body == null || avatarBody == null) { return; }
			if (printIntervalLog)
			{
				sb.Clear().Append("UpdateBodyPosesInOrder() new avatar height ").Append(avatarBody.height)
					.Append(", original avatar height ").Append(m_InitialBody.height)
					.Append(", scale: ").Append(avatarBody.scale);
				DEBUG(sb);
			}

			//avatarBody.ChangeJointCoordinate(m_JointCoordinate);

			if (m_Body.root != null) avatarBody.Update(JointType.HIP, ref m_Body.root, scale); // 0

			if (m_Body.leftThigh != null) avatarBody.Update(JointType.LEFTTHIGH, ref m_Body.leftThigh, scale);
			if (m_Body.leftLeg != null) avatarBody.Update(JointType.LEFTLEG, ref m_Body.leftLeg, scale);
			if (m_Body.leftAnkle != null) avatarBody.Update(JointType.LEFTANKLE, ref m_Body.leftAnkle, scale);
			if (m_Body.leftFoot != null) avatarBody.Update(JointType.LEFTFOOT, ref m_Body.leftFoot, scale);

			if (m_Body.rightThigh != null) avatarBody.Update(JointType.RIGHTTHIGH, ref m_Body.rightThigh, scale); // 5
			if (m_Body.rightLeg != null) avatarBody.Update(JointType.RIGHTLEG, ref m_Body.rightLeg, scale);
			if (m_Body.rightAnkle != null) avatarBody.Update(JointType.RIGHTANKLE, ref m_Body.rightAnkle, scale);
			if (m_Body.rightFoot != null) avatarBody.Update(JointType.RIGHTFOOT, ref m_Body.rightFoot, scale);

			if (m_Body.waist != null) avatarBody.Update(JointType.WAIST, ref m_Body.waist, scale);

			if (m_Body.spineLower != null) avatarBody.Update(JointType.SPINELOWER, ref m_Body.spineLower, scale); // 10
			if (m_Body.spineMiddle != null) avatarBody.Update(JointType.SPINEMIDDLE, ref m_Body.spineMiddle, scale);
			if (m_Body.spineHigh != null) avatarBody.Update(JointType.SPINEHIGH, ref m_Body.spineHigh, scale);

			if (m_Body.chest != null) avatarBody.Update(JointType.CHEST, ref m_Body.chest, scale);
			if (m_Body.neck != null) avatarBody.Update(JointType.NECK, ref m_Body.neck, scale);
			if (m_Body.head != null) avatarBody.Update(JointType.HEAD, ref m_Body.head, scale); // 15

			if (m_Body.leftClavicle != null) avatarBody.Update(JointType.LEFTCLAVICLE, ref m_Body.leftClavicle, scale);
			if (m_Body.leftScapula != null) avatarBody.Update(JointType.LEFTSCAPULA, ref m_Body.leftScapula, scale);
			if (m_Body.leftUpperarm != null) avatarBody.Update(JointType.LEFTUPPERARM, ref m_Body.leftUpperarm, scale);
			if (m_Body.leftForearm != null) avatarBody.Update(JointType.LEFTFOREARM, ref m_Body.leftForearm, scale);
			if (m_Body.leftHand != null) avatarBody.Update(JointType.LEFTHAND, ref m_Body.leftHand, scale); // 20

			if (m_Body.rightClavicle != null) avatarBody.Update(JointType.RIGHTCLAVICLE, ref m_Body.rightClavicle, scale);
			if (m_Body.rightScapula != null) avatarBody.Update(JointType.RIGHTSCAPULA, ref m_Body.rightScapula, scale);
			if (m_Body.rightUpperarm != null) avatarBody.Update(JointType.RIGHTUPPERARM, ref m_Body.rightUpperarm, scale);
			if (m_Body.rightForearm != null) avatarBody.Update(JointType.RIGHTFOREARM, ref m_Body.rightForearm, scale);
			if (m_Body.rightHand != null) avatarBody.Update(JointType.RIGHTHAND, ref m_Body.rightHand, scale); // 25
		}
	}
}
