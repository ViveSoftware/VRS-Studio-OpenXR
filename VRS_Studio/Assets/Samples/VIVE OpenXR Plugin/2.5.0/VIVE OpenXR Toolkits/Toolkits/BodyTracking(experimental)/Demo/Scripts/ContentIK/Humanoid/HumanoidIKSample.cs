// Copyright HTC Corporation All Rights Reserved.

using System;
using System.Collections;
using System.Text;
using System.Threading;
using UnityEngine;

using VIVE.OpenXR.Toolkits.BodyTracking.RuntimeDependency;

namespace VIVE.OpenXR.Toolkits.BodyTracking.Demo
{
#if USE_VRM_0_x
	[RequireComponent(typeof(UniHumanoid.Humanoid))]
#endif
	public class HumanoidIKSample : MonoBehaviour
	{
		#region Log
		const string LOG_TAG = "VIVE.OpenXR.Toolkits.BodyTracking.Demo.HumanoidIKSample";
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
		private Transform m_AvatarOffset = null;
		public Transform AvatarOffset { get { return m_AvatarOffset; } set { m_AvatarOffset = value; } }

		[SerializeField]
		[Range(0.2f, 5f)]
		private float m_AvatarScale = 1;
		public float AvatarScale { get { return m_AvatarScale; } set { m_AvatarScale = value; } }

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

			body.spineLower = m_Humanoid.Spine;

			// 9.chest
			if (m_Humanoid.UpperChest == null)
			{
				sb.Clear().Append("AssignHumanoidToBody() no Humanoid UpperChest."); WARNING(sb);
				if (m_Humanoid.Chest == null)
				{
					sb.Clear().Append("AssignHumanoidToBody() no Humanoid Chest."); ERROR(sb);
					return false;
				}
				else
				{
					sb.Clear().Append("AssignHumanoidToBody() Humanoid Chest -> Body chest."); DEBUG(sb);
					body.chest = m_Humanoid.Chest;
				}
			}
			else
			{
				sb.Clear().Append("AssignHumanoidToBody() Humanoid UpperChest -> Body chest."); DEBUG(sb);
				body.chest = m_Humanoid.UpperChest;
				sb.Clear().Append("AssignHumanoidToBody() Humanoid Chest -> Body spineHigh."); DEBUG(sb);
				body.spineHigh = m_Humanoid.Chest;
			}
			// 10.neck
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
			// 11.head
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

			// 12.leftClavicle
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
			// 13.leftUpperarm
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
			// 14.leftForearm
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
			// 15.leftHand
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

			// 16.rightClavicle
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
			// 17.rightUpperarm
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
			// 18.rightForearm
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
			// 19.rightHand
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

		private void Awake()
		{
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
			if (m_Body != null && m_InitialBody != null) { m_InitialBody.UpdateBody(ref m_Body); }
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
#if WAVE_BODY_IK
		private int m_AvatarID = -1;
#endif
		private float avatarScale = 1;
		private BodyAvatar avatarBody = null;
		public IEnumerator StartBodyTracking()
		{
			/// State machine Starting -> StartFailure
			if (BodyManager.Instance == null)
			{
				SetTrackingStatus(TrackingStatus.StartFailure);
				yield break;
			}
			sb.Clear().Append("StartBodyTracking()"); DEBUG(sb);

			BodyTrackingResult result = BodyTrackingResult.ERROR_FATAL_ERROR;
#if WAVE_BODY_IK
			// Creates a body tracker.
			result = BodyManager.Instance.InitAvatarIK(m_Body, out m_AvatarID);
			sb.Clear().Append("StartBodyTracking() InitAvatarIK(").Append(m_AvatarID).Append(") result ").Append(result.Name()); DEBUG(sb);
#endif
			/// State machine Starting -> StartFailure
			if (result != BodyTrackingResult.SUCCESS)
			{
				SetTrackingStatus(TrackingStatus.StartFailure);
				yield break;
			}

#if WAVE_BODY_IK
			yield return new WaitForSeconds(3);
			result = BodyManager.Instance.GetAvatarIKInfo(m_AvatarID, out float avatarHeight, out avatarScale);
			sb.Clear().Append("StartBodyTracking() GetAvatarIKInfo result ").Append(result.Name()).Append(", avatarHeight: ").Append(avatarHeight).Append(", avatarScale: ").Append(avatarScale); DEBUG(sb);
#endif
			if (result == BodyTrackingResult.SUCCESS)
			{
				// Due to the pose from GetAvatarIKData is "scaled pose", we need to change the avatar mesh size first.
				// The avatarHeight is user's height in calibration.
				// The m_InitialBody.height is the height of avatar used in this content.
				sb.Clear().Append("StartBodyTracking() Apply avatar scale with ").Append(avatarScale); DEBUG(sb);
				ApplyBodyScale(avatarScale * m_AvatarScale);

				/// State machine Starting -> Available
				SetTrackingStatus(TrackingStatus.Available); // Tracking is available then going into the loop for retrieving poses.
				updateTrackingData = true;
				while (updateTrackingData)
				{
#if WAVE_BODY_IK
					result = BodyManager.Instance.GetAvatarIKData(m_AvatarID, out avatarBody);
#endif
					if (result == BodyTrackingResult.SUCCESS)
					{
						if (BodyManager.Instance.EnableTrackingLog)
						{
							sb.Clear().Append("StartBodyTracking() avatarBody confidence: ").Append(avatarBody.confidence);
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
			sb.Clear().Append("StartBodyTracking() DestroyAvatarIK(").Append(m_AvatarID).Append(") result ").Append(result.Name()); DEBUG(sb);
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
					.Append(", scale: ").Append(avatarBody.scale)
					.Append(", confidence: ").Append(avatarBody.confidence);
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
