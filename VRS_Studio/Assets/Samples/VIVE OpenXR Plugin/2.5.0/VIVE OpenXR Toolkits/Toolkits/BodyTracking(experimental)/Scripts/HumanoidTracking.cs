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
#if USE_VRM_0_x
	[RequireComponent(typeof(UniHumanoid.Humanoid))]
#endif
	public class HumanoidTracking : MonoBehaviour
	{
		#region Log
		const string LOG_TAG = "VIVE.OpenXR.Toolkits.BodyTracking.HumanoidTracking";
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

		[Tooltip("If AutoGround is true, the avatar will be shift automatically to prevent the foot height < 0.")]
		//[SerializeField]
		private bool m_AutoGround = false;
		public bool AutoGround => m_AutoGround;

		[Tooltip("The thickness between avatar's foot and sole.")]
		[SerializeField]
		private float m_HeightFromSole = 0.035f;
		public float HeightFromSole => m_HeightFromSole;

		[SerializeField]
		private bool m_CustomizeExtrinsics = false;
		public bool CustomizeExtrinsics { get { return m_CustomizeExtrinsics; } set { m_CustomizeExtrinsics = value; } }

		/// Humanoid Head
		[SerializeField]
		private ExtrinsicInfo_t m_Head = new ExtrinsicInfo_t(true, wvr.extHeadT);
		public ExtrinsicInfo_t Head => m_Head;

		/// Humanoid Hand
		[SerializeField]
		private ExtrinsicInfo_t m_LeftWrist = new ExtrinsicInfo_t(true, wvr.extSelfTracker_Wrist_LeftT);
		public ExtrinsicInfo_t LeftWrist => m_LeftWrist;
		[SerializeField]
		private ExtrinsicInfo_t m_RightWrist = new ExtrinsicInfo_t(true, wvr.extSelfTracker_Wrist_RightT);
		public ExtrinsicInfo_t RightWrist => m_RightWrist;

		/// Humanoid Hand
		[SerializeField]
		private ExtrinsicInfo_t m_LeftHandheld = new ExtrinsicInfo_t(true, wvr.extController_Handheld_LeftT);
		public ExtrinsicInfo_t LeftHandheld => m_LeftHandheld;
		[SerializeField]
		private ExtrinsicInfo_t m_RightHandheld = new ExtrinsicInfo_t(true, wvr.extController_Handheld_RightT);
		public ExtrinsicInfo_t RightHandheld => m_RightHandheld;

		/// Humanoid Hand
		[SerializeField]
		private ExtrinsicInfo_t m_LeftHand = new ExtrinsicInfo_t(true, wvr.extHand_Hand_LeftT);
		public ExtrinsicInfo_t LeftHand => m_LeftHand;
		[SerializeField]
		private ExtrinsicInfo_t m_RightHand = new ExtrinsicInfo_t(true, wvr.extHand_Hand_RightT);
		public ExtrinsicInfo_t RightHand => m_RightHand;

		/// Humanoid Hips
		[SerializeField]
		private ExtrinsicInfo_t m_Hips = new ExtrinsicInfo_t(true, wvr.extSelfTracker_HipT);
		public ExtrinsicInfo_t Hips => m_Hips;

		/// Humanoid LowerLeg = TrackedDeviceRole Knee
		[SerializeField]
		private ExtrinsicInfo_t m_LeftLowerLeg = new ExtrinsicInfo_t(true, wvr.extSelfTrackerIM_Knee_LeftT);
		public ExtrinsicInfo_t LeftLowerLeg => m_LeftLowerLeg;
		[SerializeField]
		private ExtrinsicInfo_t m_RightLowerLeg = new ExtrinsicInfo_t(true, wvr.extSelfTrackerIM_Knee_RightT);
		public ExtrinsicInfo_t RightLowerLeg => m_RightLowerLeg;

		/// Humanoid Foot = TrackedDeviceRole Ankle
		[SerializeField]
		private ExtrinsicInfo_t m_LeftFoot = new ExtrinsicInfo_t(true, wvr.extSelfTrackerIM_Ankle_LeftT);
		public ExtrinsicInfo_t LeftFoot => m_LeftFoot;
		[SerializeField]
		private ExtrinsicInfo_t m_RightFoot = new ExtrinsicInfo_t(true, wvr.extSelfTrackerIM_Ankle_RightT);
		public ExtrinsicInfo_t RightFoot => m_RightFoot;

		/// Humanoid Toes = TrackedDeviceRole Foot
		[SerializeField]
		private ExtrinsicInfo_t m_LeftToes = new ExtrinsicInfo_t(true, wvr.extSelfTracker_Foot_LeftT);
		public ExtrinsicInfo_t LeftToes => m_LeftToes;
		[SerializeField]
		private ExtrinsicInfo_t m_RightToes = new ExtrinsicInfo_t(true, wvr.extSelfTracker_Foot_RightT);
		public ExtrinsicInfo_t RightToes => m_RightToes;
		#endregion

#if USE_VRM_0_x
		private UniHumanoid.Humanoid m_Humanoid = null;
#endif
		private Body m_Body = null, m_InitialBody = null;
		private TransformData m_InitialTransform;
		/// <summary> Humanoid should have at least 20 joints in function. </summary>
		private bool AssignHumanoidToBody(ref Body body)
		{
			if (body == null) { return false; }
#if USE_VRM_0_x
			m_Humanoid = GetComponent<UniHumanoid.Humanoid>();
			if (m_Humanoid == null)
			{
				sb.Clear().Append("AssignHumanoidToBody() no Humanoid."); ERROR(sb);
				return false;
			}

			// 0.hip
			if (m_Humanoid.Hips == null)
			{
				sb.Clear().Append("AssignHumanoidToBody() no Humanoid Hips."); ERROR(sb);
				return false;
			}
			else
			{
				sb.Clear().Append("AssignHumanoidToBody() Humanoid Hips -> Body root."); DEBUG(sb);
				body.root = m_Humanoid.Hips;
			}

			// 1.leftThigh
			if (m_Humanoid.LeftUpperLeg == null)
			{
				sb.Clear().Append("AssignHumanoidToBody() no Humanoid LeftUpperLeg."); ERROR(sb);
				return false;
			}
			else
			{
				sb.Clear().Append("AssignHumanoidToBody() Humanoid LeftUpperLeg -> Body leftThigh."); DEBUG(sb);
				body.leftThigh = m_Humanoid.LeftUpperLeg;
			}
			// 2.leftLeg
			if (m_Humanoid.LeftLowerLeg == null)
			{
				sb.Clear().Append("AssignHumanoidToBody() no Humanoid LeftLowerLeg."); ERROR(sb);
				return false;
			}
			else
			{
				sb.Clear().Append("AssignHumanoidToBody() Humanoid LeftLowerLeg -> Body leftLeg."); DEBUG(sb);
				body.leftLeg = m_Humanoid.LeftLowerLeg;
			}
			// 3.leftAnkle
			if (m_Humanoid.LeftFoot == null)
			{
				sb.Clear().Append("AssignHumanoidToBody() no Humanoid LeftFoot."); ERROR(sb);
				return false;
			}
			else
			{
				sb.Clear().Append("AssignHumanoidToBody() Humanoid LeftFoot -> Body leftAnkle."); DEBUG(sb);
				body.leftAnkle = m_Humanoid.LeftFoot;
			}
			// 4.leftFoot
			if (m_Humanoid.LeftToes == null)
			{
				sb.Clear().Append("AssignHumanoidToBody() no Humanoid LeftToes."); ERROR(sb);
				return false;
			}
			else
			{
				sb.Clear().Append("AssignHumanoidToBody() Humanoid LeftToes -> Body leftFoot."); DEBUG(sb);
				body.leftFoot = m_Humanoid.LeftToes;
			}

			// 5.rightThigh
			if (m_Humanoid.RightUpperLeg == null)
			{
				sb.Clear().Append("AssignHumanoidToBody() no Humanoid RightUpperLeg."); ERROR(sb);
				return false;
			}
			else
			{
				sb.Clear().Append("AssignHumanoidToBody() Humanoid RightUpperLeg -> Body rightThigh."); DEBUG(sb);
				body.rightThigh = m_Humanoid.RightUpperLeg;
			}
			// 6.rightLeg
			if (m_Humanoid.RightLowerLeg == null)
			{
				sb.Clear().Append("AssignHumanoidToBody() no Humanoid RightLowerLeg."); ERROR(sb);
				return false;
			}
			else
			{
				sb.Clear().Append("AssignHumanoidToBody() Humanoid RightLowerLeg -> Body rightLeg."); DEBUG(sb);
				body.rightLeg = m_Humanoid.RightLowerLeg;
			}
			// 7.rightAnkle
			if (m_Humanoid.RightFoot == null)
			{
				sb.Clear().Append("AssignHumanoidToBody() no Humanoid RightFoot."); ERROR(sb);
				return false;
			}
			else
			{
				sb.Clear().Append("AssignHumanoidToBody() Humanoid RightFoot -> Body rightAnkle."); DEBUG(sb);
				body.rightAnkle = m_Humanoid.RightFoot;
			}
			// 8.rightFoot
			if (m_Humanoid.RightToes == null)
			{
				sb.Clear().Append("AssignHumanoidToBody() no Humanoid RightToes."); ERROR(sb);
				return false;
			}
			else
			{
				sb.Clear().Append("AssignHumanoidToBody() Humanoid RightToes -> Body rightFoot."); DEBUG(sb);
				body.rightFoot = m_Humanoid.RightToes;
			}

			// 13.Chest
			if (m_Humanoid.Spine == null)
			{
				sb.Clear().Append("AssignHumanoidToBody() no Humanoid Spine."); ERROR(sb);
				return false;
			}
			else
			{
				// (UpperChest) -> (Chest) -> Spine
				if (m_Humanoid.UpperChest != null)
				{
					sb.Clear().Append("AssignHumanoidToBody() Humanoid UpperChest -> Body chest."); DEBUG(sb);
					body.chest = m_Humanoid.UpperChest;

					if (m_Humanoid.Chest != null)
					{
						sb.Clear().Append("AssignHumanoidToBody() Humanoid Chest -> Body spineHigh."); DEBUG(sb);
						body.spineHigh = m_Humanoid.Chest;

						sb.Clear().Append("AssignHumanoidToBody() Humanoid Spine -> Body spineLower."); DEBUG(sb);
						body.spineLower = m_Humanoid.Spine;
					}
					else
					{
						sb.Clear().Append("AssignHumanoidToBody() Humanoid Spine -> Body spineHigh."); DEBUG(sb);
						body.spineHigh = m_Humanoid.Spine;
					}
				}
				else // UpperChest is null
				{
					if (m_Humanoid.Chest != null)
					{
						sb.Clear().Append("AssignHumanoidToBody() Humanoid Chest -> Body chest."); DEBUG(sb);
						body.chest = m_Humanoid.Chest;

						sb.Clear().Append("AssignHumanoidToBody() Humanoid Spine -> Body spineHigh."); DEBUG(sb);
						body.spineHigh = m_Humanoid.Spine;
					}
					else // Chest is null
					{
						sb.Clear().Append("AssignHumanoidToBody() Humanoid Spine -> Body chest."); DEBUG(sb);
						body.chest = m_Humanoid.Spine;
					}
				}
			}

			// 14.neck
			if (m_Humanoid.Neck == null)
			{
				sb.Clear().Append("AssignHumanoidToBody() no Humanoid Neck."); ERROR(sb);
				return false;
			}
			else
			{
				sb.Clear().Append("AssignHumanoidToBody() Humanoid Neck -> Body neck."); DEBUG(sb);
				body.neck = m_Humanoid.Neck;
			}
			// 15.head
			if (m_Humanoid.Head == null)
			{
				sb.Clear().Append("AssignHumanoidToBody() no Humanoid Head."); ERROR(sb);
				return false;
			}
			else
			{
				sb.Clear().Append("AssignHumanoidToBody() Humanoid Head -> Body head."); DEBUG(sb);
				body.head = m_Humanoid.Head;
			}

			// 16.leftClavicle
			if (m_Humanoid.LeftShoulder == null)
			{
				sb.Clear().Append("AssignHumanoidToBody() no Humanoid LeftShoulder."); ERROR(sb);
				return false;
			}
			else
			{
				sb.Clear().Append("AssignHumanoidToBody() Humanoid LeftShoulder -> Body leftClavicle."); DEBUG(sb);
				body.leftClavicle = m_Humanoid.LeftShoulder;
			}
			// 18.leftUpperarm
			if (m_Humanoid.LeftUpperArm == null)
			{
				sb.Clear().Append("AssignHumanoidToBody() no Humanoid LeftUpperArm."); ERROR(sb);
				return false;
			}
			else
			{
				sb.Clear().Append("AssignHumanoidToBody() Humanoid LeftUpperArm -> Body leftUpperarm."); DEBUG(sb);
				body.leftUpperarm = m_Humanoid.LeftUpperArm;
			}
			// 19.leftForearm
			if (m_Humanoid.LeftLowerArm == null)
			{
				sb.Clear().Append("AssignHumanoidToBody() no Humanoid LeftLowerArm."); ERROR(sb);
				return false;
			}
			else
			{
				sb.Clear().Append("AssignHumanoidToBody() Humanoid LeftLowerArm -> Body leftForearm."); DEBUG(sb);
				body.leftForearm = m_Humanoid.LeftLowerArm;
			}
			// 20.leftHand
			if (m_Humanoid.LeftHand == null)
			{
				sb.Clear().Append("AssignHumanoidToBody() no Humanoid LeftHand."); ERROR(sb);
				return false;
			}
			else
			{
				sb.Clear().Append("AssignHumanoidToBody() Humanoid LeftHand -> Body leftHand."); DEBUG(sb);
				body.leftHand = m_Humanoid.LeftHand;
			}

			// 21.rightClavicle
			if (m_Humanoid.RightShoulder == null)
			{
				sb.Clear().Append("AssignHumanoidToBody() no Humanoid RightShoulder."); ERROR(sb);
				return false;
			}
			else
			{
				sb.Clear().Append("AssignHumanoidToBody() Humanoid RightShoulder -> Body rightClavicle."); DEBUG(sb);
				body.rightClavicle = m_Humanoid.RightShoulder;
			}
			// 23.rightUpperarm
			if (m_Humanoid.RightUpperArm == null)
			{
				sb.Clear().Append("AssignHumanoidToBody() no Humanoid RightUpperArm."); ERROR(sb);
				return false;
			}
			else
			{
				sb.Clear().Append("AssignHumanoidToBody() Humanoid RightUpperArm -> Body rightUpperarm."); DEBUG(sb);
				body.rightUpperarm = m_Humanoid.RightUpperArm;
			}
			// 24.rightForearm
			if (m_Humanoid.RightLowerArm == null)
			{
				sb.Clear().Append("AssignHumanoidToBody() no Humanoid RightLowerArm."); ERROR(sb);
				return false;
			}
			else
			{
				sb.Clear().Append("AssignHumanoidToBody() Humanoid RightLowerArm -> Body rightForearm."); DEBUG(sb);
				body.rightForearm = m_Humanoid.RightLowerArm;
			}
			// 25.rightHand
			if (m_Humanoid.RightHand == null)
			{
				sb.Clear().Append("AssignHumanoidToBody() no Humanoid RightHand."); ERROR(sb);
				return false;
			}
			else
			{
				sb.Clear().Append("AssignHumanoidToBody() Humanoid RightHand -> Body rightHand."); DEBUG(sb);
				body.rightHand = m_Humanoid.RightHand;
			}

			if (m_CustomSettings)
			{
				body.height = m_AvatarHeight;

				sb.Clear().Append("AssignHumanoidToBody() height: ").Append(body.height);
				DEBUG(sb);
			}
			else
			{
				float floor = Mathf.Min(m_Humanoid.LeftToes.position.y, m_Humanoid.RightToes.position.y);
				body.height = m_Humanoid.Head.position.y - floor;

				sb.Clear().Append("AssignHumanoidToBody() Calculates height:")
					.Append(" LeftToes (").Append(m_Humanoid.LeftToes.position.y).Append(")")
					.Append(", RightToes(").Append(m_Humanoid.RightToes.position.y).Append(")")
					.Append(", Head(").Append(m_Humanoid.Head.position.y).Append(")")
					.Append(", height: ").Append(body.height);
				DEBUG(sb);
			}

			return true;
#else
			return false;
#endif
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
				sb.Clear().Append("Awake() Configures Humanoid avatar."); DEBUG(sb);
				m_Body = new Body();
				if (!AssignHumanoidToBody(ref m_Body))
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

			if (m_AutoGround)
			{
				Vector3 heightOffset = Vector3.zero;
				if (m_Body != null && m_Body.leftFoot.position.y < 0) { heightOffset.y = heightOffset.y > (-m_Body.leftFoot.position.y) ? heightOffset.y : (-m_Body.leftFoot.position.y); }
				if (m_Body != null && m_Body.rightFoot.position.y < 0) { heightOffset.y = heightOffset.y > (-m_Body.rightFoot.position.y) ? heightOffset.y : (-m_Body.rightFoot.position.y); }
				heightOffset.y += m_HeightFromSole;
				transform.localPosition += heightOffset;
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

#if WAVE_BODY_IK
			result = BodyManager.Instance.DestroyAvatarIK(m_AvatarID);
			sb.Clear().Append("StartFixUpdateBodyTracking() DestroyAvatarIK(").Append(m_AvatarID).Append(") result ").Append(result.Name()); DEBUG(sb);
#else
			result = BodyManager.Instance.DestroyBodyTracking(m_AvatarID);
			sb.Clear().Append("StartFixUpdateBodyTracking() DestroyBodyTracking result ").Append(result.Name()).Append(", id: ").Append(m_AvatarID); DEBUG(sb);
#endif
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
