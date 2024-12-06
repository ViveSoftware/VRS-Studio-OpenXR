// Copyright HTC Corporation All Rights Reserved.

#define CoordinateOpenGL

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;
using VIVE.OpenXR.Toolkits.BodyTracking.RuntimeDependency;

#if WAVE_BODY_CALIBRATION || WAVE_BODY_IK
using Wave.Native;
#endif

namespace VIVE.OpenXR.Toolkits.BodyTracking
{
	[Serializable]
	public class Body
	{
		public Transform root; // hip
		private TransformData hipData = TransformData.identity;
		public TransformData HipData { get { return hipData; } private set { hipData = value; } }

		public Transform leftThigh;
		private TransformData leftThighData = TransformData.identity;
		public TransformData LeftThighData { get { return leftThighData; } private set { leftThighData = value; } }
		public Transform leftLeg;
		private TransformData leftLegData = TransformData.identity;
		public TransformData LeftLegData { get { return leftLegData; } private set { leftLegData = value; } }
		public Transform leftAnkle;
		private TransformData leftAnkleData = TransformData.identity;
		public TransformData LeftAnkleData { get { return leftAnkleData; } private set { leftAnkleData = value; } }
		public Transform leftFoot;
		private TransformData leftFootData = TransformData.identity;
		public TransformData LeftFootData { get { return leftFootData; } private set { leftFootData = value; } }

		public Transform rightThigh;
		private TransformData rightThighData = TransformData.identity;
		public TransformData RightThighData { get { return rightThighData; } private set { rightThighData = value; } }
		public Transform rightLeg;
		private TransformData rightLegData = TransformData.identity;
		public TransformData RightLegData { get { return rightLegData; } private set { rightLegData = value; } }
		public Transform rightAnkle;
		private TransformData rightAnkleData = TransformData.identity;
		public TransformData RightAnkleData { get { return rightAnkleData; } private set { rightAnkleData = value; } }
		public Transform rightFoot;
		private TransformData rightFootData = TransformData.identity;
		public TransformData RightFootData { get { return rightFootData; } private set { rightFootData = value; } }

		public Transform waist;
		private TransformData waistData = TransformData.identity;
		public TransformData WaistData { get { return waistData; } private set { waistData = value; } }

		public Transform spineLower;
		private TransformData spineLowerData = TransformData.identity;
		public TransformData SpineLowerData { get { return spineLowerData; } private set { spineLowerData = value; } }
		public Transform spineMiddle;
		private TransformData spineMiddleData = TransformData.identity;
		public TransformData SpineMiddleData { get { return spineMiddleData; } private set { spineMiddleData = value; } }
		public Transform spineHigh;
		private TransformData spineHighData = TransformData.identity;
		public TransformData SpineHighData { get { return spineHighData; } private set { spineHighData = value; } }

		public Transform chest;
		private TransformData chestData = TransformData.identity;
		public TransformData ChestData { get { return chestData; } private set { chestData = value; } }
		public Transform neck;
		private TransformData neckData = TransformData.identity;
		public TransformData NeckData { get { return neckData; } private set { neckData = value; } }
		public Transform head;
		private TransformData headData = TransformData.identity;
		public TransformData HeadData { get { return headData; } private set { headData = value; } }

		public Transform leftClavicle;
		private TransformData leftClavicleData = TransformData.identity;
		public TransformData LeftClavicleData { get { return leftClavicleData; } private set { leftClavicleData = value; } }
		public Transform leftScapula;
		private TransformData leftScapulaData = TransformData.identity;
		public TransformData LeftScapulaData { get { return leftScapulaData; } private set { leftScapulaData = value; } }
		public Transform leftUpperarm;
		private TransformData leftUpperarmData = TransformData.identity;
		public TransformData LeftUpperarmData { get { return leftUpperarmData; } private set { leftUpperarmData = value; } }
		public Transform leftForearm;
		private TransformData leftForearmData = TransformData.identity;
		public TransformData LeftForearmData { get { return leftForearmData; } private set { leftForearmData = value; } }
		public Transform leftHand;
		private TransformData leftHandData = TransformData.identity;
		public TransformData LeftHandData { get { return leftHandData; } private set { leftHandData = value; } }

		public Transform rightClavicle;
		private TransformData rightClavicleData = TransformData.identity;
		public TransformData RightClavicleData { get { return rightClavicleData; } private set { rightClavicleData = value; } }
		public Transform rightScapula;
		private TransformData rightScapulaData = TransformData.identity;
		public TransformData RightScapulaData { get { return rightScapulaData; } private set { rightScapulaData = value; } }
		public Transform rightUpperarm;
		private TransformData rightUpperarmData = TransformData.identity;
		public TransformData RightUpperarmData { get { return rightUpperarmData; } private set { rightUpperarmData = value; } }
		public Transform rightForearm;
		private TransformData rightForearmData = TransformData.identity;
		public TransformData RightForearmData { get { return rightForearmData; } private set { rightForearmData = value; } }
		public Transform rightHand;
		private TransformData rightHandData = TransformData.identity;
		public TransformData RightHandData { get { return rightHandData; } private set { rightHandData = value; } }

		public float height = 0;

		public void UpdateData(Body in_body)
		{
			height = in_body.height;

			hipData.Update(in_body.root);

			leftThighData.Update(in_body.leftThigh);
			leftLegData.Update(in_body.leftLeg);
			leftAnkleData.Update(in_body.leftAnkle);
			leftFootData.Update(in_body.leftFoot);

			rightThighData.Update(in_body.rightThigh);
			rightLegData.Update(in_body.rightLeg);
			rightAnkleData.Update(in_body.rightAnkle);
			rightFootData.Update(in_body.rightFoot);

			waistData.Update(in_body.waist);

			spineLowerData.Update(in_body.spineLower);
			spineMiddleData.Update(in_body.spineMiddle);
			spineHighData.Update(in_body.spineHigh);

			chestData.Update(in_body.chest);
			neckData.Update(in_body.neck);
			headData.Update(in_body.head);

			leftClavicleData.Update(in_body.leftClavicle);
			leftScapulaData.Update(in_body.leftScapula);
			leftUpperarmData.Update(in_body.leftUpperarm);
			leftForearmData.Update(in_body.leftForearm);
			leftHandData.Update(in_body.leftHand);

			rightClavicleData.Update(in_body.rightClavicle);
			rightScapulaData.Update(in_body.rightScapula);
			rightUpperarmData.Update(in_body.rightUpperarm);
			rightForearmData.Update(in_body.rightForearm);
			rightHandData.Update(in_body.rightHand);
		}
		public void UpdateBody(ref Body body)
		{
			body.height = height;

			hipData.Update(ref body.root);

			leftThighData.Update(ref body.leftThigh);
			leftLegData.Update(ref body.leftLeg);
			leftAnkleData.Update(ref body.leftAnkle);
			leftFootData.Update(ref body.leftFoot);

			rightThighData.Update(ref body.rightThigh);
			rightLegData.Update(ref body.rightLeg);
			rightAnkleData.Update(ref body.rightAnkle);
			rightFootData.Update(ref body.rightFoot);

			waistData.Update(ref body.waist);

			spineLowerData.Update(ref body.spineLower);
			spineMiddleData.Update(ref body.spineMiddle);
			spineHighData.Update(ref body.spineHigh);

			chestData.Update(ref body.chest);
			neckData.Update(ref body.neck);
			headData.Update(ref body.head);

			leftClavicleData.Update(ref body.leftClavicle);
			leftScapulaData.Update(ref body.leftScapula);
			leftUpperarmData.Update(ref body.leftUpperarm);
			leftForearmData.Update(ref body.leftForearm);
			leftHandData.Update(ref body.leftHand);

			rightClavicleData.Update(ref body.rightClavicle);
			rightScapulaData.Update(ref body.rightScapula);
			rightUpperarmData.Update(ref body.rightUpperarm);
			rightForearmData.Update(ref body.rightForearm);
			rightHandData.Update(ref body.rightHand);
		}
	}

	[Serializable]
	public class TrackerExtrinsic
	{
		const string LOG_TAG = "VIVE.OpenXR.Toolkits.BodyTracking.TrackerExtrinsic";
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

		public ExtrinsicInfo_t hip = ExtrinsicInfo_t.identity; // 0
		public ExtrinsicInfo_t chest = ExtrinsicInfo_t.identity; // 1
		public ExtrinsicInfo_t head = ExtrinsicInfo_t.identity; // 2

		public ExtrinsicInfo_t leftElbow = ExtrinsicInfo_t.identity; // 3
		public ExtrinsicInfo_t leftWrist = ExtrinsicInfo_t.identity; // 4
		public ExtrinsicInfo_t leftHand = ExtrinsicInfo_t.identity; // 5
		public ExtrinsicInfo_t leftHandheld = ExtrinsicInfo_t.identity; // 6

		public ExtrinsicInfo_t rightElbow = ExtrinsicInfo_t.identity; // 7
		public ExtrinsicInfo_t rightWrist = ExtrinsicInfo_t.identity; // 8
		public ExtrinsicInfo_t rightHand = ExtrinsicInfo_t.identity; // 9
		public ExtrinsicInfo_t rightHandheld = ExtrinsicInfo_t.identity; // 10

		public ExtrinsicInfo_t leftKnee = ExtrinsicInfo_t.identity; // 11
		public ExtrinsicInfo_t leftAnkle = ExtrinsicInfo_t.identity; // 12
		public ExtrinsicInfo_t leftFoot = ExtrinsicInfo_t.identity; // 13

		public ExtrinsicInfo_t rightKnee = ExtrinsicInfo_t.identity; // 14
		public ExtrinsicInfo_t rightAnkle = ExtrinsicInfo_t.identity; // 15
		public ExtrinsicInfo_t rightFoot = ExtrinsicInfo_t.identity; // 16

		public void Reset()
		{
			hip.Reset();
			chest.Reset();
			head.Reset();

			leftElbow.Reset();
			leftWrist.Reset();
			leftHand.Reset();
			leftHandheld.Reset();

			rightElbow.Reset();
			rightWrist.Reset();
			rightHand.Reset();
			rightHandheld.Reset();

			leftKnee.Reset();
			leftAnkle.Reset();
			leftFoot.Reset();

			rightKnee.Reset();
			rightAnkle.Reset();
			rightFoot.Reset();
		}
		public void UseDefault()
		{
			string func = "UseDefault() ";

			Rdp.Update(ref hip, wvr.extSelfTracker_Hip);
			sb.Clear().Append(func).Append("hip isTracking: ").Append(hip.isTracking)
				.Append(", position (").Append(hip.extrinsic.translation.x.ToString("N3")).Append(", ").Append(hip.extrinsic.translation.y.ToString("N3")).Append(", ").Append(hip.extrinsic.translation.z.ToString("N3")).Append(")")
				.Append(", rotation (").Append(hip.extrinsic.rotation.x.ToString("N3")).Append(", ").Append(hip.extrinsic.rotation.y.ToString("N3")).Append(", ").Append(hip.extrinsic.rotation.z.ToString("N3")).Append(", ").Append(hip.extrinsic.rotation.w.ToString("N3")).Append(")");
			DEBUG(sb);

			Rdp.Update(ref head, wvr.extHead);
			sb.Clear().Append(func).Append("head isTracking: ").Append(head.isTracking)
				.Append(", position (").Append(head.extrinsic.translation.x.ToString("N3")).Append(", ").Append(head.extrinsic.translation.y.ToString("N3")).Append(", ").Append(head.extrinsic.translation.z.ToString("N3")).Append(")")
				.Append(", rotation (").Append(head.extrinsic.rotation.x.ToString("N3")).Append(", ").Append(head.extrinsic.rotation.y.ToString("N3")).Append(", ").Append(head.extrinsic.rotation.z.ToString("N3")).Append(", ").Append(head.extrinsic.rotation.w.ToString("N3")).Append(")");
			DEBUG(sb);

			Rdp.Update(ref leftWrist, wvr.extSelfTracker_Wrist_Left);
			sb.Clear().Append(func).Append("leftWrist isTracking: ").Append(leftWrist.isTracking)
				.Append(", position (").Append(leftWrist.extrinsic.translation.x.ToString("N3")).Append(", ").Append(leftWrist.extrinsic.translation.y.ToString("N3")).Append(", ").Append(leftWrist.extrinsic.translation.z.ToString("N3")).Append(")")
				.Append(", rotation (").Append(leftWrist.extrinsic.rotation.x.ToString("N3")).Append(", ").Append(leftWrist.extrinsic.rotation.y.ToString("N3")).Append(", ").Append(leftWrist.extrinsic.rotation.z.ToString("N3")).Append(", ").Append(leftWrist.extrinsic.rotation.w.ToString("N3")).Append(")");
			DEBUG(sb);
			Rdp.Update(ref leftHand, wvr.extHand_Hand_Left);
			sb.Clear().Append(func).Append("leftHand isTracking: ").Append(leftHand.isTracking)
				.Append(", position (").Append(leftHand.extrinsic.translation.x.ToString("N3")).Append(", ").Append(leftHand.extrinsic.translation.y.ToString("N3")).Append(", ").Append(leftHand.extrinsic.translation.z.ToString("N3")).Append(")")
				.Append(", rotation (").Append(leftHand.extrinsic.rotation.x.ToString("N3")).Append(", ").Append(leftHand.extrinsic.rotation.y.ToString("N3")).Append(", ").Append(leftHand.extrinsic.rotation.z.ToString("N3")).Append(", ").Append(leftHand.extrinsic.rotation.w.ToString("N3")).Append(")");
			DEBUG(sb);
			Rdp.Update(ref leftHandheld, wvr.extController_Handheld_Left);
			sb.Clear().Append(func).Append("leftHandheld isTracking: ").Append(leftHandheld.isTracking)
				.Append(", position (").Append(leftHandheld.extrinsic.translation.x.ToString("N3")).Append(", ").Append(leftHandheld.extrinsic.translation.y.ToString("N3")).Append(", ").Append(leftHandheld.extrinsic.translation.z.ToString("N3")).Append(")")
				.Append(", rotation (").Append(leftHandheld.extrinsic.rotation.x.ToString("N3")).Append(", ").Append(leftHandheld.extrinsic.rotation.y.ToString("N3")).Append(", ").Append(leftHandheld.extrinsic.rotation.z.ToString("N3")).Append(", ").Append(leftHandheld.extrinsic.rotation.w.ToString("N3")).Append(")");
			DEBUG(sb);

			Rdp.Update(ref rightWrist, wvr.extSelfTracker_Wrist_Right);
			sb.Clear().Append(func).Append("rightWrist isTracking: ").Append(rightWrist.isTracking)
				.Append(", position (").Append(rightWrist.extrinsic.translation.x.ToString("N3")).Append(", ").Append(rightWrist.extrinsic.translation.y.ToString("N3")).Append(", ").Append(rightWrist.extrinsic.translation.z.ToString("N3")).Append(")")
				.Append(", rotation (").Append(rightWrist.extrinsic.rotation.x.ToString("N3")).Append(", ").Append(rightWrist.extrinsic.rotation.y.ToString("N3")).Append(", ").Append(rightWrist.extrinsic.rotation.z.ToString("N3")).Append(", ").Append(rightWrist.extrinsic.rotation.w.ToString("N3")).Append(")");
			DEBUG(sb);
			Rdp.Update(ref rightHand, wvr.extHand_Hand_Right);
			sb.Clear().Append(func).Append("rightHand isTracking: ").Append(rightHand.isTracking)
				.Append(", position (").Append(rightHand.extrinsic.translation.x.ToString("N3")).Append(", ").Append(rightHand.extrinsic.translation.y.ToString("N3")).Append(", ").Append(rightHand.extrinsic.translation.z.ToString("N3")).Append(")")
				.Append(", rotation (").Append(rightHand.extrinsic.rotation.x.ToString("N3")).Append(", ").Append(rightHand.extrinsic.rotation.y.ToString("N3")).Append(", ").Append(rightHand.extrinsic.rotation.z.ToString("N3")).Append(", ").Append(rightHand.extrinsic.rotation.w.ToString("N3")).Append(")");
			DEBUG(sb);
			Rdp.Update(ref rightHandheld, wvr.extController_Handheld_Right);
			sb.Clear().Append(func).Append("rightHandheld isTracking: ").Append(rightHandheld.isTracking)
				.Append(", position (").Append(rightHandheld.extrinsic.translation.x.ToString("N3")).Append(", ").Append(rightHandheld.extrinsic.translation.y.ToString("N3")).Append(", ").Append(rightHandheld.extrinsic.translation.z.ToString("N3")).Append(")")
				.Append(", rotation (").Append(rightHandheld.extrinsic.rotation.x.ToString("N3")).Append(", ").Append(rightHandheld.extrinsic.rotation.y.ToString("N3")).Append(", ").Append(rightHandheld.extrinsic.rotation.z.ToString("N3")).Append(", ").Append(rightHandheld.extrinsic.rotation.w.ToString("N3")).Append(")");
			DEBUG(sb);

			Rdp.Update(ref leftKnee, wvr.extSelfTrackerIM_Knee_Left); // self tracker im for Knee.
			sb.Clear().Append(func).Append("leftKnee isTracking: ").Append(leftKnee.isTracking)
				.Append(", position (").Append(leftKnee.extrinsic.translation.x.ToString("N3")).Append(", ").Append(leftKnee.extrinsic.translation.y.ToString("N3")).Append(", ").Append(leftKnee.extrinsic.translation.z.ToString("N3")).Append(")")
				.Append(", rotation (").Append(leftKnee.extrinsic.rotation.x.ToString("N3")).Append(", ").Append(leftKnee.extrinsic.rotation.y.ToString("N3")).Append(", ").Append(leftKnee.extrinsic.rotation.z.ToString("N3")).Append(", ").Append(leftKnee.extrinsic.rotation.w.ToString("N3")).Append(")");
			DEBUG(sb);
			Rdp.Update(ref leftAnkle, wvr.extSelfTrackerIM_Ankle_Left); // self tracker im for Ankle by default
			sb.Clear().Append(func).Append("leftAnkle isTracking: ").Append(leftAnkle.isTracking)
				.Append(", position (").Append(leftAnkle.extrinsic.translation.x.ToString("N3")).Append(", ").Append(leftAnkle.extrinsic.translation.y.ToString("N3")).Append(", ").Append(leftAnkle.extrinsic.translation.z.ToString("N3")).Append(")")
				.Append(", rotation (").Append(leftAnkle.extrinsic.rotation.x.ToString("N3")).Append(", ").Append(leftAnkle.extrinsic.rotation.y.ToString("N3")).Append(", ").Append(leftAnkle.extrinsic.rotation.z.ToString("N3")).Append(", ").Append(leftAnkle.extrinsic.rotation.w.ToString("N3")).Append(")");
			DEBUG(sb);
			Rdp.Update(ref leftFoot, wvr.extSelfTracker_Foot_Left);
			sb.Clear().Append(func).Append("leftFoot isTracking: ").Append(leftFoot.isTracking)
				.Append(", position (").Append(leftFoot.extrinsic.translation.x.ToString("N3")).Append(", ").Append(leftFoot.extrinsic.translation.y.ToString("N3")).Append(", ").Append(leftFoot.extrinsic.translation.z.ToString("N3")).Append(")")
				.Append(", rotation (").Append(leftFoot.extrinsic.rotation.x.ToString("N3")).Append(", ").Append(leftFoot.extrinsic.rotation.y.ToString("N3")).Append(", ").Append(leftFoot.extrinsic.rotation.z.ToString("N3")).Append(", ").Append(leftFoot.extrinsic.rotation.w.ToString("N3")).Append(")");
			DEBUG(sb);

			Rdp.Update(ref rightKnee, wvr.extSelfTrackerIM_Knee_Right); // self tracker im for Knee.
			sb.Clear().Append(func).Append("rightKnee isTracking: ").Append(rightKnee.isTracking)
				.Append(", position (").Append(rightKnee.extrinsic.translation.x.ToString("N3")).Append(", ").Append(rightKnee.extrinsic.translation.y.ToString("N3")).Append(", ").Append(rightKnee.extrinsic.translation.z.ToString("N3")).Append(")")
				.Append(", rotation (").Append(rightKnee.extrinsic.rotation.x.ToString("N3")).Append(", ").Append(rightKnee.extrinsic.rotation.y.ToString("N3")).Append(", ").Append(rightKnee.extrinsic.rotation.z.ToString("N3")).Append(", ").Append(rightKnee.extrinsic.rotation.w.ToString("N3")).Append(")");
			DEBUG(sb);
			Rdp.Update(ref rightAnkle, wvr.extSelfTrackerIM_Ankle_Right); // self tracker im for Ankle by default
			sb.Clear().Append(func).Append("rightAnkle isTracking: ").Append(rightAnkle.isTracking)
				.Append(", position (").Append(rightAnkle.extrinsic.translation.x.ToString("N3")).Append(", ").Append(rightAnkle.extrinsic.translation.y.ToString("N3")).Append(", ").Append(rightAnkle.extrinsic.translation.z.ToString("N3")).Append(")")
				.Append(", rotation (").Append(rightAnkle.extrinsic.rotation.x.ToString("N3")).Append(", ").Append(rightAnkle.extrinsic.rotation.y.ToString("N3")).Append(", ").Append(rightAnkle.extrinsic.rotation.z.ToString("N3")).Append(", ").Append(rightAnkle.extrinsic.rotation.w.ToString("N3")).Append(")");
			DEBUG(sb);
			Rdp.Update(ref rightFoot, wvr.extSelfTracker_Foot_Right);
			sb.Clear().Append(func).Append("rightFoot isTracking: ").Append(rightFoot.isTracking)
				.Append(", position (").Append(rightFoot.extrinsic.translation.x.ToString("N3")).Append(", ").Append(rightFoot.extrinsic.translation.y.ToString("N3")).Append(", ").Append(rightFoot.extrinsic.translation.z.ToString("N3")).Append(")")
				.Append(", rotation (").Append(rightFoot.extrinsic.rotation.x.ToString("N3")).Append(", ").Append(rightFoot.extrinsic.rotation.y.ToString("N3")).Append(", ").Append(rightFoot.extrinsic.rotation.z.ToString("N3")).Append(", ").Append(rightFoot.extrinsic.rotation.w.ToString("N3")).Append(")");
			DEBUG(sb);
		}
		public void Update(TrackedDeviceRole role, ExtrinsicInfo_t ext)
		{
			if (role == TrackedDeviceRole.ROLE_HIP) { hip.Update(ext); }
			if (role == TrackedDeviceRole.ROLE_CHEST) { chest.Update(ext); }
			if (role == TrackedDeviceRole.ROLE_HEAD) { head.Update(ext); }

			if (role == TrackedDeviceRole.ROLE_LEFTELBOW) { leftElbow.Update(ext); }
			if (role == TrackedDeviceRole.ROLE_LEFTWRIST) { leftWrist.Update(ext); }
			if (role == TrackedDeviceRole.ROLE_LEFTHANDHELD) { leftHandheld.Update(ext); }
			if (role == TrackedDeviceRole.ROLE_LEFTHAND) { leftHand.Update(ext); }

			if (role == TrackedDeviceRole.ROLE_RIGHTELBOW) { rightElbow.Update(ext); }
			if (role == TrackedDeviceRole.ROLE_RIGHTWRIST) { rightWrist.Update(ext); }
			if (role == TrackedDeviceRole.ROLE_RIGHTHANDHELD) { rightHandheld.Update(ext); }
			if (role == TrackedDeviceRole.ROLE_RIGHTHAND) { rightHand.Update(ext); }

			if (role == TrackedDeviceRole.ROLE_LEFTKNEE) { leftKnee.Update(ext); }
			if (role == TrackedDeviceRole.ROLE_LEFTANKLE) { leftAnkle.Update(ext); }
			if (role == TrackedDeviceRole.ROLE_LEFTFOOT) { leftFoot.Update(ext); }

			if (role == TrackedDeviceRole.ROLE_RIGHTKNEE) { rightKnee.Update(ext); }
			if (role == TrackedDeviceRole.ROLE_RIGHTANKLE) { rightAnkle.Update(ext); }
			if (role == TrackedDeviceRole.ROLE_RIGHTFOOT) { rightFoot.Update(ext); }
		}
		public void Update(TrackedDeviceExtrinsic[] exts)
		{
			string func = "Update() ";
			for (int i = 0; i < exts.Length; i++)
			{
				if (exts[i].trackedDeviceRole == TrackedDeviceRole.ROLE_HIP)
				{
					hip.Update(exts[i]);
					sb.Clear().Append(func).Append("hip isTracking: ").Append(hip.isTracking)
						.Append(", position (").Append(hip.extrinsic.translation.x.ToString("N3")).Append(", ").Append(hip.extrinsic.translation.y.ToString("N3")).Append(", ").Append(hip.extrinsic.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(hip.extrinsic.rotation.x.ToString("N3")).Append(", ").Append(hip.extrinsic.rotation.y.ToString("N3")).Append(", ").Append(hip.extrinsic.rotation.z.ToString("N3")).Append(", ").Append(hip.extrinsic.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
				if (exts[i].trackedDeviceRole == TrackedDeviceRole.ROLE_CHEST)
				{
					chest.Update(exts[i]);
					sb.Clear().Append(func).Append("chest isTracking: ").Append(chest.isTracking)
						.Append(", position (").Append(chest.extrinsic.translation.x.ToString("N3")).Append(", ").Append(chest.extrinsic.translation.y.ToString("N3")).Append(", ").Append(chest.extrinsic.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(chest.extrinsic.rotation.x.ToString("N3")).Append(", ").Append(chest.extrinsic.rotation.y.ToString("N3")).Append(", ").Append(chest.extrinsic.rotation.z.ToString("N3")).Append(", ").Append(chest.extrinsic.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
				if (exts[i].trackedDeviceRole == TrackedDeviceRole.ROLE_HEAD)
				{
					head.Update(exts[i]);
					sb.Clear().Append(func).Append("head isTracking: ").Append(head.isTracking)
						.Append(", position (").Append(head.extrinsic.translation.x.ToString("N3")).Append(", ").Append(head.extrinsic.translation.y.ToString("N3")).Append(", ").Append(head.extrinsic.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(head.extrinsic.rotation.x.ToString("N3")).Append(", ").Append(head.extrinsic.rotation.y.ToString("N3")).Append(", ").Append(head.extrinsic.rotation.z.ToString("N3")).Append(", ").Append(head.extrinsic.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}

				if (exts[i].trackedDeviceRole == TrackedDeviceRole.ROLE_LEFTELBOW)
				{
					leftElbow.Update(exts[i]);
					sb.Clear().Append(func).Append("leftElbow isTracking: ").Append(leftElbow.isTracking)
						.Append(", position (").Append(leftElbow.extrinsic.translation.x.ToString("N3")).Append(", ").Append(leftElbow.extrinsic.translation.y.ToString("N3")).Append(", ").Append(leftElbow.extrinsic.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(leftElbow.extrinsic.rotation.x.ToString("N3")).Append(", ").Append(leftElbow.extrinsic.rotation.y.ToString("N3")).Append(", ").Append(leftElbow.extrinsic.rotation.z.ToString("N3")).Append(", ").Append(leftElbow.extrinsic.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
				if (exts[i].trackedDeviceRole == TrackedDeviceRole.ROLE_LEFTWRIST)
				{
					leftWrist.Update(exts[i]);
					sb.Clear().Append(func).Append("leftWrist isTracking: ").Append(leftWrist.isTracking)
						.Append(", position (").Append(leftWrist.extrinsic.translation.x.ToString("N3")).Append(", ").Append(leftWrist.extrinsic.translation.y.ToString("N3")).Append(", ").Append(leftWrist.extrinsic.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(leftWrist.extrinsic.rotation.x.ToString("N3")).Append(", ").Append(leftWrist.extrinsic.rotation.y.ToString("N3")).Append(", ").Append(leftWrist.extrinsic.rotation.z.ToString("N3")).Append(", ").Append(leftWrist.extrinsic.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
				if (exts[i].trackedDeviceRole == TrackedDeviceRole.ROLE_LEFTHANDHELD)
				{
					leftHandheld.Update(exts[i]);
					sb.Clear().Append(func).Append("leftHandheld isTracking: ").Append(leftHandheld.isTracking)
						.Append(", position (").Append(leftHandheld.extrinsic.translation.x.ToString("N3")).Append(", ").Append(leftHandheld.extrinsic.translation.y.ToString("N3")).Append(", ").Append(leftHandheld.extrinsic.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(leftHandheld.extrinsic.rotation.x.ToString("N3")).Append(", ").Append(leftHandheld.extrinsic.rotation.y.ToString("N3")).Append(", ").Append(leftHandheld.extrinsic.rotation.z.ToString("N3")).Append(", ").Append(leftHandheld.extrinsic.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
				if (exts[i].trackedDeviceRole == TrackedDeviceRole.ROLE_LEFTHAND)
				{
					leftHand.Update(exts[i]);
					sb.Clear().Append(func).Append("leftHand isTracking: ").Append(leftHand.isTracking)
						.Append(", position (").Append(leftHand.extrinsic.translation.x.ToString("N3")).Append(", ").Append(leftHand.extrinsic.translation.y.ToString("N3")).Append(", ").Append(leftHand.extrinsic.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(leftHand.extrinsic.rotation.x.ToString("N3")).Append(", ").Append(leftHand.extrinsic.rotation.y.ToString("N3")).Append(", ").Append(leftHand.extrinsic.rotation.z.ToString("N3")).Append(", ").Append(leftHand.extrinsic.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}

				if (exts[i].trackedDeviceRole == TrackedDeviceRole.ROLE_RIGHTELBOW)
				{
					rightElbow.Update(exts[i]);
					sb.Clear().Append(func).Append("rightElbow isTracking: ").Append(rightElbow.isTracking)
						.Append(", position (").Append(rightElbow.extrinsic.translation.x.ToString("N3")).Append(", ").Append(rightElbow.extrinsic.translation.y.ToString("N3")).Append(", ").Append(rightElbow.extrinsic.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(rightElbow.extrinsic.rotation.x.ToString("N3")).Append(", ").Append(rightElbow.extrinsic.rotation.y.ToString("N3")).Append(", ").Append(rightElbow.extrinsic.rotation.z.ToString("N3")).Append(", ").Append(rightElbow.extrinsic.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
				if (exts[i].trackedDeviceRole == TrackedDeviceRole.ROLE_RIGHTWRIST)
				{
					rightWrist.Update(exts[i]);
					sb.Clear().Append(func).Append("rightWrist isTracking: ").Append(rightWrist.isTracking)
						.Append(", position (").Append(rightWrist.extrinsic.translation.x.ToString("N3")).Append(", ").Append(rightWrist.extrinsic.translation.y.ToString("N3")).Append(", ").Append(rightWrist.extrinsic.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(rightWrist.extrinsic.rotation.x.ToString("N3")).Append(", ").Append(rightWrist.extrinsic.rotation.y.ToString("N3")).Append(", ").Append(rightWrist.extrinsic.rotation.z.ToString("N3")).Append(", ").Append(rightWrist.extrinsic.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
				if (exts[i].trackedDeviceRole == TrackedDeviceRole.ROLE_RIGHTHANDHELD)
				{
					rightHandheld.Update(exts[i]);
					sb.Clear().Append(func).Append("rightHandheld isTracking: ").Append(rightHandheld.isTracking)
						.Append(", position (").Append(rightHandheld.extrinsic.translation.x.ToString("N3")).Append(", ").Append(rightHandheld.extrinsic.translation.y.ToString("N3")).Append(", ").Append(rightHandheld.extrinsic.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(rightHandheld.extrinsic.rotation.x.ToString("N3")).Append(", ").Append(rightHandheld.extrinsic.rotation.y.ToString("N3")).Append(", ").Append(rightHandheld.extrinsic.rotation.z.ToString("N3")).Append(", ").Append(rightHandheld.extrinsic.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
				if (exts[i].trackedDeviceRole == TrackedDeviceRole.ROLE_RIGHTHAND)
				{
					rightHand.Update(exts[i]);
					sb.Clear().Append(func).Append("rightHand isTracking: ").Append(rightHand.isTracking)
						.Append(", position (").Append(rightHand.extrinsic.translation.x.ToString("N3")).Append(", ").Append(rightHand.extrinsic.translation.y.ToString("N3")).Append(", ").Append(rightHand.extrinsic.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(rightHand.extrinsic.rotation.x.ToString("N3")).Append(", ").Append(rightHand.extrinsic.rotation.y.ToString("N3")).Append(", ").Append(rightHand.extrinsic.rotation.z.ToString("N3")).Append(", ").Append(rightHand.extrinsic.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}

				if (exts[i].trackedDeviceRole == TrackedDeviceRole.ROLE_LEFTKNEE)
				{
					leftKnee.Update(exts[i]);
					sb.Clear().Append(func).Append("leftKnee isTracking: ").Append(leftKnee.isTracking)
						.Append(", position (").Append(leftKnee.extrinsic.translation.x.ToString("N3")).Append(", ").Append(leftKnee.extrinsic.translation.y.ToString("N3")).Append(", ").Append(leftKnee.extrinsic.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(leftKnee.extrinsic.rotation.x.ToString("N3")).Append(", ").Append(leftKnee.extrinsic.rotation.y.ToString("N3")).Append(", ").Append(leftKnee.extrinsic.rotation.z.ToString("N3")).Append(", ").Append(leftKnee.extrinsic.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
				if (exts[i].trackedDeviceRole == TrackedDeviceRole.ROLE_LEFTANKLE)
				{
					leftAnkle.Update(exts[i]);
					sb.Clear().Append(func).Append("leftAnkle isTracking: ").Append(leftAnkle.isTracking)
						.Append(", position (").Append(leftAnkle.extrinsic.translation.x.ToString("N3")).Append(", ").Append(leftAnkle.extrinsic.translation.y.ToString("N3")).Append(", ").Append(leftAnkle.extrinsic.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(leftAnkle.extrinsic.rotation.x.ToString("N3")).Append(", ").Append(leftAnkle.extrinsic.rotation.y.ToString("N3")).Append(", ").Append(leftAnkle.extrinsic.rotation.z.ToString("N3")).Append(", ").Append(leftAnkle.extrinsic.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
				if (exts[i].trackedDeviceRole == TrackedDeviceRole.ROLE_LEFTFOOT)
				{
					leftFoot.Update(exts[i]);
					sb.Clear().Append(func).Append("leftFoot isTracking: ").Append(leftFoot.isTracking)
						.Append(", position (").Append(leftFoot.extrinsic.translation.x.ToString("N3")).Append(", ").Append(leftFoot.extrinsic.translation.y.ToString("N3")).Append(", ").Append(leftFoot.extrinsic.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(leftFoot.extrinsic.rotation.x.ToString("N3")).Append(", ").Append(leftFoot.extrinsic.rotation.y.ToString("N3")).Append(", ").Append(leftFoot.extrinsic.rotation.z.ToString("N3")).Append(", ").Append(leftFoot.extrinsic.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}

				if (exts[i].trackedDeviceRole == TrackedDeviceRole.ROLE_RIGHTKNEE)
				{
					rightKnee.Update(exts[i]);
					sb.Clear().Append(func).Append("rightKnee isTracking: ").Append(rightKnee.isTracking)
						.Append(", position (").Append(rightKnee.extrinsic.translation.x.ToString("N3")).Append(", ").Append(rightKnee.extrinsic.translation.y.ToString("N3")).Append(", ").Append(rightKnee.extrinsic.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(rightKnee.extrinsic.rotation.x.ToString("N3")).Append(", ").Append(rightKnee.extrinsic.rotation.y.ToString("N3")).Append(", ").Append(rightKnee.extrinsic.rotation.z.ToString("N3")).Append(", ").Append(rightKnee.extrinsic.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
				if (exts[i].trackedDeviceRole == TrackedDeviceRole.ROLE_RIGHTANKLE)
				{
					rightAnkle.Update(exts[i]);
					sb.Clear().Append(func).Append("rightAnkle isTracking: ").Append(rightAnkle.isTracking)
						.Append(", position (").Append(rightAnkle.extrinsic.translation.x.ToString("N3")).Append(", ").Append(rightAnkle.extrinsic.translation.y.ToString("N3")).Append(", ").Append(rightAnkle.extrinsic.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(rightAnkle.extrinsic.rotation.x.ToString("N3")).Append(", ").Append(rightAnkle.extrinsic.rotation.y.ToString("N3")).Append(", ").Append(rightAnkle.extrinsic.rotation.z.ToString("N3")).Append(", ").Append(rightAnkle.extrinsic.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
				if (exts[i].trackedDeviceRole == TrackedDeviceRole.ROLE_RIGHTFOOT)
				{
					rightFoot.Update(exts[i]);
					sb.Clear().Append(func).Append("rightFoot isTracking: ").Append(rightFoot.isTracking)
						.Append(", position (").Append(rightFoot.extrinsic.translation.x.ToString("N3")).Append(", ").Append(rightFoot.extrinsic.translation.y.ToString("N3")).Append(", ").Append(rightFoot.extrinsic.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(rightFoot.extrinsic.rotation.x.ToString("N3")).Append(", ").Append(rightFoot.extrinsic.rotation.y.ToString("N3")).Append(", ").Append(rightFoot.extrinsic.rotation.z.ToString("N3")).Append(", ").Append(rightFoot.extrinsic.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
			}
		}

		public bool GetDevicesExtrinsics(BodyPoseRole calibRole, out TrackedDeviceExtrinsic[] bodyTrackedDevices, out UInt32 bodyTrackedDeviceCount)
		{
			bodyTrackedDevices = null;
			bodyTrackedDeviceCount = 0;
			bool ret = false;

			if (calibRole == BodyPoseRole.Arm_Wrist)
			{
				if (head.isTracking &&
					leftWrist.isTracking && rightWrist.isTracking // Tracker
				)
				{
					bodyTrackedDevices = new TrackedDeviceExtrinsic[3]
					{
						new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_HEAD, head.extrinsic.GetExtrinsic()),
						new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_LEFTWRIST, leftWrist.extrinsic.GetExtrinsic()),
						new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_RIGHTWRIST, rightWrist.extrinsic.GetExtrinsic()),
					};
					bodyTrackedDeviceCount = (UInt32)(bodyTrackedDevices.Length & 0x7FFFFFFF);
					ret = true;
				}
			}
			if (calibRole == BodyPoseRole.UpperBody_Wrist)
			{
				if (head.isTracking &&
					leftWrist.isTracking && rightWrist.isTracking && // Tracker
					hip.isTracking
				)
				{
					bodyTrackedDevices = new TrackedDeviceExtrinsic[4]
					{
						new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_HEAD, head.extrinsic.GetExtrinsic()),
						new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_LEFTWRIST, leftWrist.extrinsic.GetExtrinsic()),
						new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_RIGHTWRIST, rightWrist.extrinsic.GetExtrinsic()),
						new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_HIP, hip.extrinsic.GetExtrinsic()),
					};
					bodyTrackedDeviceCount = (UInt32)(bodyTrackedDevices.Length & 0x7FFFFFFF);
					ret = true;
				}
			}
			if (calibRole == BodyPoseRole.FullBody_Wrist_Ankle)
			{
				if (head.isTracking &&
					leftWrist.isTracking && rightWrist.isTracking && // Tracker
					hip.isTracking && // Tracker
					leftAnkle.isTracking && rightAnkle.isTracking // Tracker
				)
				{
					bodyTrackedDevices = new TrackedDeviceExtrinsic[6]
					{
						new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_HEAD, head.extrinsic.GetExtrinsic()),
						new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_LEFTWRIST, leftWrist.extrinsic.GetExtrinsic()),
						new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_RIGHTWRIST, rightWrist.extrinsic.GetExtrinsic()),
						new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_HIP, hip.extrinsic.GetExtrinsic()),
						new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_LEFTANKLE, leftAnkle.extrinsic.GetExtrinsic()),
						new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_RIGHTANKLE, rightAnkle.extrinsic.GetExtrinsic()),
					};
					bodyTrackedDeviceCount = (UInt32)(bodyTrackedDevices.Length & 0x7FFFFFFF);
					ret = true;
				}
			}
			if (calibRole == BodyPoseRole.FullBody_Wrist_Foot)
			{
				if (head.isTracking &&
					leftWrist.isTracking && rightWrist.isTracking && // Tracker
					hip.isTracking && // Tracker
					leftFoot.isTracking && rightFoot.isTracking // Tracker
				)
				{
					bodyTrackedDevices = new TrackedDeviceExtrinsic[6]
					{
						new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_HEAD, head.extrinsic.GetExtrinsic()),
						new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_LEFTWRIST, leftWrist.extrinsic.GetExtrinsic()),
						new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_RIGHTWRIST, rightWrist.extrinsic.GetExtrinsic()),
						new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_HIP, hip.extrinsic.GetExtrinsic()),
						new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_LEFTFOOT, leftFoot.extrinsic.GetExtrinsic()),
						new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_RIGHTFOOT, rightFoot.extrinsic.GetExtrinsic()),
					};
					bodyTrackedDeviceCount = (UInt32)(bodyTrackedDevices.Length & 0x7FFFFFFF);
					ret = true;
				}
			}

			if (calibRole == BodyPoseRole.Arm_Hand || calibRole == BodyPoseRole.Arm_Handheld)
			{
				if (head.isTracking &&
					leftHandheld.isTracking && rightHandheld.isTracking && // Controller
					leftHand.isTracking && rightHand.isTracking // Hand
				)
				{
					bodyTrackedDevices = new TrackedDeviceExtrinsic[5]
					{
						new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_HEAD, head.extrinsic.GetExtrinsic()),
						new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_LEFTHANDHELD, leftHandheld.extrinsic.GetExtrinsic()),
						new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_RIGHTHANDHELD, rightHandheld.extrinsic.GetExtrinsic()),
						new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_LEFTHAND, leftHand.extrinsic.GetExtrinsic()),
						new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_RIGHTHAND, rightHand.extrinsic.GetExtrinsic()),
					};
					bodyTrackedDeviceCount = (UInt32)(bodyTrackedDevices.Length & 0x7FFFFFFF);
					ret = true;
				}
			}
			if (calibRole == BodyPoseRole.UpperBody_Hand || calibRole == BodyPoseRole.UpperBody_Handheld)
			{
				if (head.isTracking &&
					leftHandheld.isTracking && rightHandheld.isTracking && // Controller
					leftHand.isTracking && rightHand.isTracking && // Hand
					hip.isTracking // Tracker
				)
				{
					bodyTrackedDevices = new TrackedDeviceExtrinsic[6]
					{
						new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_HEAD, head.extrinsic.GetExtrinsic()),
						new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_LEFTHANDHELD, leftHandheld.extrinsic.GetExtrinsic()),
						new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_RIGHTHANDHELD, rightHandheld.extrinsic.GetExtrinsic()),
						new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_LEFTHAND, leftHand.extrinsic.GetExtrinsic()),
						new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_RIGHTHAND, rightHand.extrinsic.GetExtrinsic()),
						new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_HIP, hip.extrinsic.GetExtrinsic()),
					};
					bodyTrackedDeviceCount = (UInt32)(bodyTrackedDevices.Length & 0x7FFFFFFF);
					ret = true;
				}
			}
			if (calibRole == BodyPoseRole.FullBody_Hand_Ankle || calibRole == BodyPoseRole.FullBody_Handheld_Ankle)
			{
				if (head.isTracking &&
					leftHandheld.isTracking && rightHandheld.isTracking && // Controller
					leftHand.isTracking && rightHand.isTracking && // Hand
					hip.isTracking && // Tracker
					leftAnkle.isTracking && rightAnkle.isTracking // Tracker
				)
				{
					bodyTrackedDevices = new TrackedDeviceExtrinsic[8]
					{
						new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_HEAD, head.extrinsic.GetExtrinsic()),
						new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_LEFTHANDHELD, leftHandheld.extrinsic.GetExtrinsic()),
						new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_RIGHTHANDHELD, rightHandheld.extrinsic.GetExtrinsic()),
						new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_LEFTHAND, leftHand.extrinsic.GetExtrinsic()),
						new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_RIGHTHAND, rightHand.extrinsic.GetExtrinsic()),
						new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_HIP, hip.extrinsic.GetExtrinsic()),
						new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_LEFTANKLE, leftAnkle.extrinsic.GetExtrinsic()),
						new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_RIGHTANKLE, rightAnkle.extrinsic.GetExtrinsic()),
					};
					bodyTrackedDeviceCount = (UInt32)(bodyTrackedDevices.Length & 0x7FFFFFFF);
					ret = true;
				}
			}
			if (calibRole == BodyPoseRole.FullBody_Hand_Foot || calibRole == BodyPoseRole.FullBody_Handheld_Foot)
			{
				if (head.isTracking &&
					leftHandheld.isTracking && rightHandheld.isTracking && // Controller
					leftHand.isTracking && rightHand.isTracking && // Hand
					hip.isTracking && // Tracker
					leftFoot.isTracking && rightFoot.isTracking // Tracker
				)
				{
					bodyTrackedDevices = new TrackedDeviceExtrinsic[8]
					{
						new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_HEAD, head.extrinsic.GetExtrinsic()),
						new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_LEFTHANDHELD, leftHandheld.extrinsic.GetExtrinsic()),
						new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_RIGHTHANDHELD, rightHandheld.extrinsic.GetExtrinsic()),
						new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_LEFTHAND, leftHand.extrinsic.GetExtrinsic()),
						new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_RIGHTHAND, rightHand.extrinsic.GetExtrinsic()),
						new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_HIP, hip.extrinsic.GetExtrinsic()),
						new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_LEFTFOOT, leftFoot.extrinsic.GetExtrinsic()),
						new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_RIGHTFOOT, rightFoot.extrinsic.GetExtrinsic()),
					};
					bodyTrackedDeviceCount = (UInt32)(bodyTrackedDevices.Length & 0x7FFFFFFF);
					ret = true;
				}
			}

			if (calibRole == BodyPoseRole.UpperBody_Hand_Knee_Ankle || calibRole == BodyPoseRole.UpperBody_Handheld_Knee_Ankle)
			{
				if (head.isTracking &&
					leftHandheld.isTracking && rightHandheld.isTracking && // Controller
					leftHand.isTracking && rightHand.isTracking && // Hand
					hip.isTracking && // Tracker
					leftKnee.isTracking && rightKnee.isTracking && // Tracker(IM)
					leftAnkle.isTracking && rightAnkle.isTracking // Tracker(IM)
				)
				{
					bodyTrackedDevices = new TrackedDeviceExtrinsic[10]
					{
						new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_HEAD, head.extrinsic.GetExtrinsic()),
						new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_LEFTHANDHELD, leftHandheld.extrinsic.GetExtrinsic()),
						new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_RIGHTHANDHELD, rightHandheld.extrinsic.GetExtrinsic()),
						new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_LEFTHAND, leftHand.extrinsic.GetExtrinsic()),
						new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_RIGHTHAND, rightHand.extrinsic.GetExtrinsic()),
						new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_HIP, hip.extrinsic.GetExtrinsic()),
						new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_LEFTKNEE, leftKnee.extrinsic.GetExtrinsic()),
						new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_RIGHTKNEE, rightKnee.extrinsic.GetExtrinsic()),
						new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_LEFTANKLE, leftAnkle.extrinsic.GetExtrinsic()),
						new TrackedDeviceExtrinsic(TrackedDeviceRole.ROLE_RIGHTANKLE, rightAnkle.extrinsic.GetExtrinsic()),
					};
					bodyTrackedDeviceCount = (UInt32)(bodyTrackedDevices.Length & 0x7FFFFFFF);
					ret = true;
				}
			}

			return ret;
		}

#if WAVE_BODY_CALIBRATION
		public BodyTrackingResult InitExtrinsicFromRuntime(WVR_BodyTracking_DeviceInfo_t[] devices, UInt32 deviceCount)
		{
			if (devices == null || devices.Length != deviceCount) { return BodyTrackingResult.ERROR_INVALID_ARGUMENT; }
			string func = "InitExtrinsicFromRuntime() ";

			for (int i = 0; i < devices.Length; i++)
			{
				if (devices[i].role == WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_Hip)
				{
					hip.Update(devices[i].extrinsic);
					sb.Clear().Append(func).Append("hip isTracking: ").Append(hip.isTracking)
						.Append(", extrinsic position (").Append(hip.extrinsic.translation.x.ToString("N3")).Append(", ").Append(hip.extrinsic.translation.y.ToString("N3")).Append(", ").Append(hip.extrinsic.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(hip.extrinsic.rotation.x.ToString("N3")).Append(", ").Append(hip.extrinsic.rotation.y.ToString("N3")).Append(", ").Append(hip.extrinsic.rotation.z.ToString("N3")).Append(", ").Append(hip.extrinsic.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
				if (devices[i].role == WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_Chest) { chest.Update(devices[i].extrinsic); }
				if (devices[i].role == WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_Head)
				{
					head.Update(devices[i].extrinsic);
					sb.Clear().Append(func).Append("head isTracking: ").Append(head.isTracking)
						.Append(", extrinsic position (").Append(head.extrinsic.translation.x.ToString("N3")).Append(", ").Append(head.extrinsic.translation.y.ToString("N3")).Append(", ").Append(head.extrinsic.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(head.extrinsic.rotation.x.ToString("N3")).Append(", ").Append(head.extrinsic.rotation.y.ToString("N3")).Append(", ").Append(head.extrinsic.rotation.z.ToString("N3")).Append(", ").Append(head.extrinsic.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}

				if (devices[i].role == WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_LeftElbow) { leftElbow.Update(devices[i].extrinsic); }
				if (devices[i].role == WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_LeftWrist)
				{
					leftWrist.Update(devices[i].extrinsic);
					sb.Clear().Append(func).Append("leftWrist isTracking: ").Append(leftWrist.isTracking)
						.Append(", extrinsic position (").Append(leftWrist.extrinsic.translation.x.ToString("N3")).Append(", ").Append(leftWrist.extrinsic.translation.y.ToString("N3")).Append(", ").Append(leftWrist.extrinsic.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(leftWrist.extrinsic.rotation.x.ToString("N3")).Append(", ").Append(leftWrist.extrinsic.rotation.y.ToString("N3")).Append(", ").Append(leftWrist.extrinsic.rotation.z.ToString("N3")).Append(", ").Append(leftWrist.extrinsic.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
				if (devices[i].role == WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_LeftHandheld)
				{
					leftHandheld.Update(devices[i].extrinsic);
					sb.Clear().Append(func).Append("leftHandheld isTracking: ").Append(leftHandheld.isTracking)
						.Append(", extrinsic position (").Append(leftHandheld.extrinsic.translation.x.ToString("N3")).Append(", ").Append(leftHandheld.extrinsic.translation.y.ToString("N3")).Append(", ").Append(leftHandheld.extrinsic.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(leftHandheld.extrinsic.rotation.x.ToString("N3")).Append(", ").Append(leftHandheld.extrinsic.rotation.y.ToString("N3")).Append(", ").Append(leftHandheld.extrinsic.rotation.z.ToString("N3")).Append(", ").Append(leftHandheld.extrinsic.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
				if (devices[i].role == WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_LeftHand)
				{
					leftHand.Update(devices[i].extrinsic);
					sb.Clear().Append(func).Append("leftHand isTracking: ").Append(leftHand.isTracking)
						.Append(", extrinsic position (").Append(leftHand.extrinsic.translation.x.ToString("N3")).Append(", ").Append(leftHand.extrinsic.translation.y.ToString("N3")).Append(", ").Append(leftHand.extrinsic.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(leftHand.extrinsic.rotation.x.ToString("N3")).Append(", ").Append(leftHand.extrinsic.rotation.y.ToString("N3")).Append(", ").Append(leftHand.extrinsic.rotation.z.ToString("N3")).Append(", ").Append(leftHand.extrinsic.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}

				if (devices[i].role == WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_RightElbow) { rightElbow.Update(devices[i].extrinsic); }
				if (devices[i].role == WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_RightWrist)
				{
					rightWrist.Update(devices[i].extrinsic);
					sb.Clear().Append(func).Append("rightWrist isTracking: ").Append(rightWrist.isTracking)
						.Append(", extrinsic position (").Append(rightWrist.extrinsic.translation.x.ToString("N3")).Append(", ").Append(rightWrist.extrinsic.translation.y.ToString("N3")).Append(", ").Append(rightWrist.extrinsic.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(rightWrist.extrinsic.rotation.x.ToString("N3")).Append(", ").Append(rightWrist.extrinsic.rotation.y.ToString("N3")).Append(", ").Append(rightWrist.extrinsic.rotation.z.ToString("N3")).Append(", ").Append(rightWrist.extrinsic.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
				if (devices[i].role == WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_RightHandheld)
				{
					rightHandheld.Update(devices[i].extrinsic);
					sb.Clear().Append(func).Append("rightHandheld isTracking: ").Append(rightHandheld.isTracking)
						.Append(", extrinsic position (").Append(rightHandheld.extrinsic.translation.x.ToString("N3")).Append(", ").Append(rightHandheld.extrinsic.translation.y.ToString("N3")).Append(", ").Append(rightHandheld.extrinsic.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(rightHandheld.extrinsic.rotation.x.ToString("N3")).Append(", ").Append(rightHandheld.extrinsic.rotation.y.ToString("N3")).Append(", ").Append(rightHandheld.extrinsic.rotation.z.ToString("N3")).Append(", ").Append(rightHandheld.extrinsic.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
				if (devices[i].role == WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_RightHand)
				{
					rightHand.Update(devices[i].extrinsic);
					sb.Clear().Append(func).Append("rightHand isTracking: ").Append(rightHand.isTracking)
						.Append(", extrinsic position (").Append(rightHand.extrinsic.translation.x.ToString("N3")).Append(", ").Append(rightHand.extrinsic.translation.y.ToString("N3")).Append(", ").Append(rightHand.extrinsic.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(rightHand.extrinsic.rotation.x.ToString("N3")).Append(", ").Append(rightHand.extrinsic.rotation.y.ToString("N3")).Append(", ").Append(rightHand.extrinsic.rotation.z.ToString("N3")).Append(", ").Append(rightHand.extrinsic.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}

				if (devices[i].role == WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_LeftKnee)
				{
					leftKnee.Update(devices[i].extrinsic);
					sb.Clear().Append(func).Append("leftKnee isTracking: ").Append(leftKnee.isTracking)
						.Append(", extrinsic position (").Append(leftKnee.extrinsic.translation.x.ToString("N3")).Append(", ").Append(leftKnee.extrinsic.translation.y.ToString("N3")).Append(", ").Append(leftKnee.extrinsic.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(leftKnee.extrinsic.rotation.x.ToString("N3")).Append(", ").Append(leftKnee.extrinsic.rotation.y.ToString("N3")).Append(", ").Append(leftKnee.extrinsic.rotation.z.ToString("N3")).Append(", ").Append(leftKnee.extrinsic.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
				if (devices[i].role == WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_LeftAnkle)
				{
					leftAnkle.Update(devices[i].extrinsic);
					sb.Clear().Append(func).Append("leftAnkle isTracking: ").Append(leftAnkle.isTracking)
						.Append(", extrinsic position (").Append(leftAnkle.extrinsic.translation.x.ToString("N3")).Append(", ").Append(leftAnkle.extrinsic.translation.y.ToString("N3")).Append(", ").Append(leftAnkle.extrinsic.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(leftAnkle.extrinsic.rotation.x.ToString("N3")).Append(", ").Append(leftAnkle.extrinsic.rotation.y.ToString("N3")).Append(", ").Append(leftAnkle.extrinsic.rotation.z.ToString("N3")).Append(", ").Append(leftAnkle.extrinsic.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
				if (devices[i].role == WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_LeftFoot)
				{
					leftFoot.Update(devices[i].extrinsic);
					sb.Clear().Append(func).Append("leftFoot isTracking: ").Append(leftFoot.isTracking)
						.Append(", extrinsic position (").Append(leftFoot.extrinsic.translation.x.ToString("N3")).Append(", ").Append(leftFoot.extrinsic.translation.y.ToString("N3")).Append(", ").Append(leftFoot.extrinsic.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(leftFoot.extrinsic.rotation.x.ToString("N3")).Append(", ").Append(leftFoot.extrinsic.rotation.y.ToString("N3")).Append(", ").Append(leftFoot.extrinsic.rotation.z.ToString("N3")).Append(", ").Append(leftFoot.extrinsic.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}

				if (devices[i].role == WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_RightKnee)
				{
					rightKnee.Update(devices[i].extrinsic);
					sb.Clear().Append(func).Append("rightKnee isTracking: ").Append(rightKnee.isTracking)
						.Append(", extrinsic position (").Append(rightKnee.extrinsic.translation.x.ToString("N3")).Append(", ").Append(rightKnee.extrinsic.translation.y.ToString("N3")).Append(", ").Append(rightKnee.extrinsic.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(rightKnee.extrinsic.rotation.x.ToString("N3")).Append(", ").Append(rightKnee.extrinsic.rotation.y.ToString("N3")).Append(", ").Append(rightKnee.extrinsic.rotation.z.ToString("N3")).Append(", ").Append(rightKnee.extrinsic.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
				if (devices[i].role == WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_RightAnkle)
				{
					rightAnkle.Update(devices[i].extrinsic);
					sb.Clear().Append(func).Append("rightAnkle isTracking: ").Append(rightAnkle.isTracking)
						.Append(", extrinsic position (").Append(rightAnkle.extrinsic.translation.x.ToString("N3")).Append(", ").Append(rightAnkle.extrinsic.translation.y.ToString("N3")).Append(", ").Append(rightAnkle.extrinsic.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(rightAnkle.extrinsic.rotation.x.ToString("N3")).Append(", ").Append(rightAnkle.extrinsic.rotation.y.ToString("N3")).Append(", ").Append(rightAnkle.extrinsic.rotation.z.ToString("N3")).Append(", ").Append(rightAnkle.extrinsic.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
				if (devices[i].role == WVR_BodyTrackingDeviceRole.WVR_BodyTrackingDeviceRole_RightFoot)
				{
					rightFoot.Update(devices[i].extrinsic);
					sb.Clear().Append(func).Append("rightFoot isTracking: ").Append(rightFoot.isTracking)
						.Append(", extrinsic position (").Append(rightFoot.extrinsic.translation.x.ToString("N3")).Append(", ").Append(rightFoot.extrinsic.translation.y.ToString("N3")).Append(", ").Append(rightFoot.extrinsic.translation.z.ToString("N3")).Append(")")
						.Append(", rotation (").Append(rightFoot.extrinsic.rotation.x.ToString("N3")).Append(", ").Append(rightFoot.extrinsic.rotation.y.ToString("N3")).Append(", ").Append(rightFoot.extrinsic.rotation.z.ToString("N3")).Append(", ").Append(rightFoot.extrinsic.rotation.w.ToString("N3")).Append(")");
					DEBUG(sb);
				}
			}

			return BodyTrackingResult.SUCCESS;
		}
#endif
	}

	[DisallowMultipleComponent]
	[RequireComponent(typeof(BodyRoleData))]
	public sealed class BodyManager : MonoBehaviour
	{
		#region Log
		const string LOG_TAG = "VIVE.OpenXR.Toolkits.BodyTracking.BodyManager";
		private StringBuilder m_sb = null;
		internal StringBuilder sb
		{
			get
			{
				if (m_sb == null) { m_sb = new StringBuilder(); }
				return m_sb;
			}
		}

		void DEBUG(StringBuilder msg) { Rdp.d(LOG_TAG, msg, true); }
		int logFrame = 0;
		bool printIntervalLog = false;
		void WARNING(StringBuilder msg) { Rdp.w(LOG_TAG, msg, true); }
		void ERROR(StringBuilder msg) { Rdp.e(LOG_TAG, msg, true); }
		#endregion

		private bool m_EnableTrackingLog = false;
		public bool EnableTrackingLog { get { return m_EnableTrackingLog; } set { m_EnableTrackingLog = value; } }

		private static BodyManager m_Instance = null;
		public static BodyManager Instance { get { return m_Instance; } }

		private void Awake()
		{
			m_Instance = this;
		}

		private List<int> s_SkeletonIds = new List<int>();
		private void Update()
		{
			logFrame++;
			logFrame %= 300;
			printIntervalLog = (logFrame == 0);

			BodyTrackingResult result = BodyTrackingResult.ERROR_FATAL_ERROR;
			for (int i = 0; i < s_SkeletonIds.Count; i++)
			{
				int skeletonId = s_SkeletonIds[i];
				if (printIntervalLog)
				{
					sb.Clear().Append("Update() body tracking with id ").Append(skeletonId);
					DEBUG(sb);
				}
				result = UpdateBodyTrackingOnce(skeletonId);
			}

#if WAVE_BODY_IK
			for (int i = 0; i < s_BodyIK.Count; i++)
			{
				result = UpdateAvatarIKInfo(i, m_EnableTrackingLog);
				if (m_EnableTrackingLog)
				{
					sb.Clear().Append("Update() UpdateAvatarIKInfo(").Append(i).Append(") result: ").Append(result.Name());
					DEBUG(sb);
				}
			}
#endif
		}

		private BodyPose m_StandardPose = null;
		// ------ Calculate Standard Pose ------
		private BodyTrackingResult GetStandardPoseInContent(ref BodyPose bodyPose, BodyTrackingMode mode = BodyTrackingMode.FULLBODYIK)
		{
			if (bodyPose == null)
			{
				sb.Clear().Append("GetStandardPoseInContent() Invalid BodyPose."); ERROR(sb);
				return BodyTrackingResult.ERROR_INVALID_ARGUMENT;
			}

			BodyTrackingResult result = bodyPose.InitPoseInContent(mode);
			sb.Clear().Append("GetStandardPoseInContent() InitPoseInContent result: ").Append(result.Name()); DEBUG(sb);

			return result;
		}
		[Obsolete("This function is deprecated.")]
		public BodyTrackingResult GetStandardPose(ref BodyPose bodyPose, BodyTrackingMode mode = BodyTrackingMode.FULLBODYIK)
		{
			if (bodyPose == null) { return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID; }

			BodyTrackingResult result = BodyTrackingResult.ERROR_NOT_CALIBRATED;
			if (result != BodyTrackingResult.SUCCESS)
			{
				sb.Clear().Append("GetStandardPose() failed to retrieve the calibration pose from runtime, result: ").Append(result.Name()); ERROR(sb);
				// No cached system standard pose, calibrate NOW!
				result = GetStandardPoseInContent(ref bodyPose, mode);
				if (result != BodyTrackingResult.SUCCESS)
				{
					sb.Clear().Append("GetStandardPose() failed to retrieve the calibration pose in content, result: ").Append(result.Name()); ERROR(sb);
					return BodyTrackingResult.ERROR_NOT_CALIBRATED;
				}
			}

			return result;
		}
		[Obsolete("This function is deprecated.")]
		public BodyTrackingResult SetStandardPose(BodyTrackingMode mode = BodyTrackingMode.FULLBODYIK)
		{
			sb.Clear().Append("SetStandardPose() ").Append(mode.Name()); DEBUG(sb);

			if (m_StandardPose == null) { m_StandardPose = new BodyPose(mode); }
			m_StandardPose.Clear(mode);
			BodyTrackingResult result = GetStandardPoseInContent(ref m_StandardPose, mode);
			if (result != BodyTrackingResult.SUCCESS)
			{
				m_StandardPose = null;
				sb.Clear().Append("SetStandardPose() ").Append(mode.Name()).Append(", GetStandardPoseInContent failed, result: ").Append(result.Name()); ERROR(sb);
				return result;
			}

			if (m_TrackerExtrinsic == null) { m_TrackerExtrinsic = new TrackerExtrinsic(); }
			m_TrackerExtrinsic.UseDefault();

			return result;
		}

		// ------ Get Standard Tracked Device Extrinsics ------
		/// <summary>
		/// Tracker extrinsics are initialized in below cases:
		/// 1. SetStandardPose: Uses wvr default extrinsics in GetStandardPoseInContent().
		/// 2. SetStandardPoseUpperIKLegFKLock: GetStandardPoseFromRuntime failed, starts coroutine and uses wvr default extrinsics in GetStandardPoseInContent().
		/// 3. SetStandardPoseUpperIKLegFKLock: GetStandardPoseFromRuntime succeeded, uses extrinsics in GetStandardPoseFromRuntime(). 
		/// </summary>
		private TrackerExtrinsic m_TrackerExtrinsic = null;

		#region API v1.0.0.6
		private event CalibrationStatusDelegate upperIKLegFKCallback = null;
		private BodyRedirectives m_Redirectives = null;

#if WAVE_BODY_CALIBRATION
		private BodyTrackingResult GetBodyTrackingInfo(BodyTrackingMode mode, out float height, out WVR_BodyTracking_DeviceInfo_t[] devices, out UInt32 deviceCount, out WVR_BodyTracking_RedirectExtrinsic_t[] redirectExtrinsics, out UInt32 redirectCount)
		{
			height = 0;
			devices = null;
			deviceCount = 0;
			redirectExtrinsics = null;
			redirectCount = 0;

			deviceCount = Rdp.GetBodyTrackingDeviceCount();
			if (deviceCount <= 0)
			{
				sb.Clear().Append("GetBodyTrackingInfo() GetBodyTrackingDeviceCount is 0."); ERROR(sb);
				return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
			}
			sb.Clear().Append("GetBodyTrackingInfo() GetBodyTrackingDeviceCount ").Append(deviceCount); DEBUG(sb);
			devices = new WVR_BodyTracking_DeviceInfo_t[deviceCount];

			redirectCount = Rdp.GetBodyTrackingRedirectExtrinsicCount();
			sb.Clear().Append("GetBodyTrackingInfo() GetBodyTrackingRedirectExtrinsicCount ").Append(redirectCount); DEBUG(sb);
			redirectExtrinsics = new WVR_BodyTracking_RedirectExtrinsic_t[redirectCount];

			WVR_BodyTrackingCalibrationMode wvrMode = WVR_BodyTrackingCalibrationMode.WVR_BodyTrackingCalibrationMode_Unknown;
			WVR_Result wvrResult = Rdp.GetBodyTrackingDeviceInfo(ref height, ref wvrMode, devices, ref deviceCount, redirectExtrinsics, ref redirectCount);
			if (wvrResult != WVR_Result.WVR_Success || wvrMode == WVR_BodyTrackingCalibrationMode.WVR_BodyTrackingCalibrationMode_Unknown)
			{
				sb.Clear().Append("GetBodyTrackingInfo() GetBodyTrackingDeviceInfo result: ").Append(wvrResult).Append(", wvrMode: ").Append(wvrMode.Name()); ERROR(sb);
				return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
			}
			if (!wvrMode.Contains(mode))
			{
				sb.Clear().Append("GetBodyTrackingInfo() Runtime Mode ").Append(wvrMode.Name()).Append(" is NOT capable with Content Mode ").Append(mode.Name());
				return BodyTrackingResult.ERROR_NOT_CALIBRATED;
			}

			sb.Clear().Append("GetBodyTrackingInfo() GetBodyTrackingDeviceInfo height: ").Append(height).Append(", wvrMode: ").Append(wvrMode.Name()); DEBUG(sb);
			if (devices != null && devices.Length == deviceCount)
			{
				float floorHeight = 0;
				// standardPose is origin on head, add floor height if AP is origin on ground.
				if (Rdp.GetOrigin() == TrackingOrigin.Global) { floorHeight = Rdp.GetFloorHeight(); }
				sb.Clear().Append("GetBodyTrackingInfo() deviceCount: ").Append(deviceCount).Append(", add floorHeight: ").Append(floorHeight); DEBUG(sb);
				for (int i = 0; i < deviceCount; i++)
				{
					if (devices[i].standardPose.poseState.HasFlag(WVR_BodyTrackingPoseState.WVR_BodyTrackingPoseState_Position))
					{
						devices[i].standardPose.position.v1 += floorHeight;
					}
					sb.Clear().Append("GetBodyTrackingInfo() devices[").Append(i).Append("] ").Append(devices[i].log());
					DEBUG(sb);
				}
			}
			if (redirectExtrinsics != null && redirectExtrinsics.Length == redirectCount)
			{
				sb.Clear().Append("GetBodyTrackingInfo() redirectCount: ").Append(redirectCount); DEBUG(sb);
				for (int i = 0; i < redirectCount; i++)
				{
					sb.Clear().Append("GetBodyTrackingInfo() redirectExtrinsics[").Append(i).Append("] ").Append(redirectExtrinsics[i].log());
					DEBUG(sb);
				}
			}

			return BodyTrackingResult.SUCCESS;
		}
#endif
		private BodyTrackingResult GetStandardPoseFromRuntime(BodyTrackingMode mode)
		{
#if WAVE_BODY_CALIBRATION
			sb.Clear().Append("GetStandardPoseFromRuntime()"); DEBUG(sb);

			BodyTrackingResult result = GetBodyTrackingInfo(mode, out float height, out WVR_BodyTracking_DeviceInfo_t[] devices, out UInt32 deviceCount, out WVR_BodyTracking_RedirectExtrinsic_t[] redirectExtrinsics, out UInt32 redirectCount);
			if (result != BodyTrackingResult.SUCCESS)
			{
				sb.Clear().Append("GetStandardPoseFromRuntime() GetBodyTrackingInfo failed, result: ").Append(result.Name()); ERROR(sb);
				return result;
			}

			/// Initialize the Standard Pose (including Redirective Poses) and Device Extrinsics (including Redirective Extrinsics)
			/// = m_StandardPose.Update(m_Redirectives.DevicePoses);
			if (m_StandardPose == null) { m_StandardPose = new BodyPose(mode); }
			m_StandardPose.Clear(mode);
			result = m_StandardPose.InitPoseFromRuntime(height, devices, deviceCount);
			if (result != BodyTrackingResult.SUCCESS)
			{
				m_StandardPose = null;
				sb.Clear().Append("GetStandardPoseFromRuntime() InitTrackingInfosFromRuntime failed, result: ").Append(result.Name()); ERROR(sb);
				return result;
			}

			// The device extrinsics from runtime contains either hand or controller, NOT both.
			// We need both extrinsics so UseDefault first.
			if (m_TrackerExtrinsic == null) { m_TrackerExtrinsic = new TrackerExtrinsic(); }
			m_TrackerExtrinsic.UseDefault();
			result = m_TrackerExtrinsic.InitExtrinsicFromRuntime(devices, deviceCount);
			if (result != BodyTrackingResult.SUCCESS)
			{
				m_TrackerExtrinsic = null;
				sb.Clear().Append("GetStandardPoseFromRuntime() InitExtrinsicFromRuntime failed, result: ").Append(result.Name()); ERROR(sb);
				return result;
			}

			if (mode == BodyTrackingMode.UPPERIKANDLEGFK)
			{
				if (m_Redirectives == null) { m_Redirectives = new BodyRedirectives(); }
				m_Redirectives.InUse = false;
				result = m_Redirectives.InitBodyRedirectivesFromRuntime(devices, deviceCount, redirectExtrinsics, redirectCount);
				if (result != BodyTrackingResult.SUCCESS)
				{
					sb.Clear().Append("GetStandardPoseFromRuntime() Updates InitBodyRedirectivesFromRuntime failed, result: ").Append(result.Name()); ERROR(sb);
					return result;
				}
				m_Redirectives.InUse = true;
			}

			return result;
#else
			return BodyTrackingResult.ERROR_NOT_CALIBRATED;
#endif
		}

		private bool inCalibration = false;
		private CalibrationStatus m_FunctionalCalibrationStatus = CalibrationStatus.STATUS_UNINITIAL;
		private IEnumerator SetStandardPoseUpperIKLegFKLock()
		{
			string func = "SetStandardPoseUpperIKLegFKLock() ";
			sb.Clear().Append(func).Append("starts."); DEBUG(sb);

			// -------------- 1. Initializes tracked device poses. --------------
			if (m_StandardPose == null) { m_StandardPose = new BodyPose(BodyTrackingMode.UPPERIKANDLEGFK); }
			m_StandardPose.Clear(BodyTrackingMode.UPPERIKANDLEGFK);
			BodyTrackingResult result = m_StandardPose.InitPoseInContent(BodyTrackingMode.UPPERIKANDLEGFK);
			if (result != BodyTrackingResult.SUCCESS)
			{
				m_StandardPose = null;
				sb.Clear().Append(func).Append("InitPoseInContent failed, result: ").Append(result.Name()); ERROR(sb);
				if (upperIKLegFKCallback != null) { upperIKLegFKCallback(this, CalibrationStatus.STATUS_COMPUTEFAILED); }
				yield break; // ending this Coroutine instantly.
			}

			// -------------- 2. Starts Functional Calibration. --------------
			var ts = (UInt64)(1_000_000.0 * Stopwatch.GetTimestamp() / Stopwatch.Frequency);
			Result fbtResult = fbt.StartFunctionalCalibrationLog(ts, BodyTrackingMode.UPPERIKANDLEGFK);
			if (fbtResult != Result.SUCCESS)
			{
				sb.Clear().Append(func).Append("StartFunctionalCalibration failed, fbtResult: ").Append(fbtResult); ERROR(sb);
				if (upperIKLegFKCallback != null) { upperIKLegFKCallback(this, CalibrationStatus.STATUS_COMPUTEFAILED); }
				yield break; // ending this Coroutine instantly.
			}

			// -------------- 3. Updates Functional Calibration each frame until calibration completed or stopped. --------------
			CalibrationStatus fbtStatusEx = m_FunctionalCalibrationStatus;
			while (inCalibration && m_FunctionalCalibrationStatus < CalibrationStatus.STATUS_FINISHED)
			{
				// 3.1 Retrieves trakced device poses.
				if (m_StandardPose.UpdatePoseInContent() != BodyTrackingResult.SUCCESS)
				{
					if (printIntervalLog)
					{
						sb.Clear().Append(func).Append("UpdatePose failed.");
						ERROR(sb);
					}
					yield return null; // waits next frame
					continue;
				}
				if (!m_StandardPose.GetTrackedDevicePoses(false, out TrackedDevicePose[] trackedDevicePoses, out UInt32 trackedDevicePoseCount))
				{
					if (printIntervalLog)
					{
						sb.Clear().Append(func).Append("GetTrackedDevicePoses failed.");
						ERROR(sb);
					}
					yield return null; // waits next frame
					continue;
				}
				BodyPoseRole calibRole = BodyTrackingUtils.GetBodyPoseRole(trackedDevicePoses, trackedDevicePoseCount, false);
				if (!BodyTrackingUtils.MatchBodyTrackingMode(BodyTrackingMode.UPPERIKANDLEGFK, calibRole, false))
				{
					if (printIntervalLog)
					{
						sb.Clear().Append(func).Append("The body tracking mode UPPERIKANDLEGFK and calibration role ").Append(calibRole.Name()).Append(" are NOT matched.");
						ERROR(sb);
					}
					yield return null; // waits next frame
					continue;
				}

				// 3.2 Updates Functional Calibration.
				ts = (UInt64)(1_000_000.0 * Stopwatch.GetTimestamp() / Stopwatch.Frequency);
				fbtStatusEx = m_FunctionalCalibrationStatus;
#if CoordinateOpenGL
				fbtResult = fbt.UpdateFunctionalCalibrationT(m_EnableTrackingLog, ts, trackedDevicePoses, trackedDevicePoseCount, ref m_FunctionalCalibrationStatus);
#else
				if (m_EnableTrackingLog)
				{
					fbtResult = fbt.UpdateFunctionalCalibrationLog(ts, trackedDevicePoses, trackedDevicePoseCount, ref m_FunctionalCalibrationStatus);
				}
				else
				{
					fbtResult = fbt.UpdateFunctionalCalibration(ts, trackedDevicePoses, trackedDevicePoseCount, ref m_FunctionalCalibrationStatus);
				}
#endif
				if (upperIKLegFKCallback != null) { upperIKLegFKCallback(this, m_FunctionalCalibrationStatus); }
				if (fbtStatusEx != m_FunctionalCalibrationStatus)
				{
					sb.Clear().Append(func).Append("Calibration status changed to ").Append(m_FunctionalCalibrationStatus.Name());
					DEBUG(sb);
				}
				if (fbtResult != Result.SUCCESS)
				{
					sb.Clear().Append(func).Append("UpdateFunctionalCalibration failed, result: ").Append(fbtResult).Append(", status: ").Append(m_FunctionalCalibrationStatus.Name());
					ERROR(sb);
					yield return null; // waits next frame
					continue;
				}
				if (printIntervalLog)
				{
					sb.Clear().Append(func).Append("Calibration status: ").Append(m_FunctionalCalibrationStatus.Name());
					DEBUG(sb);
				}

				// 3.3 Initializes tracked device extrinsics.
				switch (m_FunctionalCalibrationStatus)
				{
					case CalibrationStatus.STATUS_WAITING_POSE_MODE: // 1
					case CalibrationStatus.STATUS_COLLECTED: // 4
						break;
					case CalibrationStatus.STATUS_FINISHED: // 6
						// 1. Tracked device extrinsics and redirect extrinsics.
						{
							if (m_Redirectives == null) { m_Redirectives = new BodyRedirectives(); }
							m_Redirectives.InUse = false;
							fbtResult = fbt.GetCalibratedExtrinsicCountLog(ts, ref m_Redirectives.ExtrinsicCount);
							if (fbtResult != Result.SUCCESS)
							{
								sb.Clear().Append(func).Append(m_FunctionalCalibrationStatus.Name()).Append(" GetCalibratedExtrinsicCount failed, result: ").Append(fbtResult); ERROR(sb);
								continue;
							}

							m_Redirectives.DeviceExts = new TrackedDeviceExtrinsic[m_Redirectives.ExtrinsicCount];
							m_Redirectives.RedirectExts = new TrackedDeviceRedirectExtrinsic[m_Redirectives.ExtrinsicCount];
						}
						// 2. Tracked device poses.
						{
							fbtResult = fbt.GetCalibratedPoseCountLog(ts, ref m_Redirectives.PoseCount);
							if (fbtResult != Result.SUCCESS)
							{
								sb.Clear().Append(func).Append(m_FunctionalCalibrationStatus.Name()).Append(" GetCalibratedPoseCount failed, result: ").Append(fbtResult); ERROR(sb);
								continue;
							}

							m_Redirectives.DevicePoses = new TrackedDevicePose[m_Redirectives.PoseCount];
						}
						// 3. Get calibration result.
						{
#if CoordinateOpenGL
							fbtResult = fbt.GetFunctionalCalibrationResultT(ts,
								trackedDeviceExt: m_Redirectives.DeviceExts,
								trackedDeviceRedirectExt: m_Redirectives.RedirectExts,
								deviceCount: m_Redirectives.ExtrinsicCount,
								calibratedTrackedDevicePose: m_Redirectives.DevicePoses,
								poseCount: m_Redirectives.PoseCount);
#else
							fbtResult = fbt.GetFunctionalCalibrationResultLog(ts,
								trackedDeviceExt: m_Redirectives.DeviceExts,
								trackedDeviceRedirectExt: m_Redirectives.RedirectExts,
								deviceCount: m_Redirectives.ExtrinsicCount,
								calibratedTrackedDevicePose: m_Redirectives.DevicePoses,
								poseCount: m_Redirectives.PoseCount);
#endif

							if (fbtResult != Result.SUCCESS)
							{
								sb.Clear().Append(func).Append(m_FunctionalCalibrationStatus.Name()).Append(" GetFunctionalCalibrationResult failed, result: ").Append(fbtResult); ERROR(sb);
								continue;
							}

							sb.Clear().Append(func).Append("redirect poses."); DEBUG(sb);
							m_StandardPose.Update(m_Redirectives.DevicePoses);

							if (m_TrackerExtrinsic == null) { m_TrackerExtrinsic = new TrackerExtrinsic(); }
							m_TrackerExtrinsic.UseDefault();

							m_Redirectives.InUse = true;
						}
						break;

					// Functional calibration is ongoing, loop will keep running.
					case CalibrationStatus.STATUS_WAITING_STATIC: // 0
					case CalibrationStatus.STATUS_READY: // 2
					case CalibrationStatus.STATUS_COLLECTING: // 3
					case CalibrationStatus.STATUS_COLLECTED_AND_COMPUTING: // 5
						break;

					// Functional calibration is failed, loop will be ended after 1 frame.
					case CalibrationStatus.STATUS_WALKFAILED_DISTANCE: // 7
					case CalibrationStatus.STATUS_WALKFAILED_TIME: // 8
					case CalibrationStatus.STATUS_WAIT_STATIC_FAILED_TIME: // 9
					case CalibrationStatus.STATUS_WAIT_POSEMODE_FAILED_TIME: // 10
					case CalibrationStatus.STATUS_READYFAILED_TIME: // 11
					case CalibrationStatus.STATUS_COMPUTEFAILED_TIME: // 12
					case CalibrationStatus.STATUS_COMPUTEFAILED: // 13
					case CalibrationStatus.STATUS_REASON_NONSTATIC_START: // 32
					case CalibrationStatus.STATUS_REASON_NOTRIGGER: // 64
					case CalibrationStatus.STATUS_REASON_NONSTATIC_END: // 96
						break;

					default:
						break;
				}

				yield return new WaitForEndOfFrame();
			}

			// -------------- 4. Ends Functional Calibration. --------------
			fbtResult = fbt.DestroyFunctionalCalibrationLog(ts);
			sb.Clear().Append(func).Append("ends.").Append(" DestroyFunctionalCalibration result: ").Append(fbtResult); DEBUG(sb);

			//Rdp.SetOETIMTrackingMode();

			yield return null; // waits 1 frame then ending this Coroutine.

			inCalibration = false;
		}

#if WAVE_BODY_IK
		[Obsolete("This function is deprecated.")]
#endif
		public void StartCalibration(BodyTrackingMode mode, CalibrationStatusDelegate callback = null)
		{
			string func = "StartCalibration() ";
			sb.Clear().Append(func).Append(mode.Name()); DEBUG(sb);

			upperIKLegFKCallback = callback;
			BodyTrackingResult result = GetStandardPoseFromRuntime(mode);
			if (result != BodyTrackingResult.SUCCESS)
			{
				sb.Clear().Append(func).Append(mode.Name()).Append(", GetStandardPoseFromRuntime failed, result: ").Append(result.Name()); ERROR(sb);
				sb.Clear().Append(func).Append(mode.Name()).Append(", in content calibration."); DEBUG(sb);
				if (mode == BodyTrackingMode.UPPERIKANDLEGFK)
				{
					if (!inCalibration)
					{
						sb.Clear().Append(func).Append("from "); DEBUG(sb);

						inCalibration = true;
						m_FunctionalCalibrationStatus = CalibrationStatus.STATUS_UNINITIAL;
						if (m_Redirectives != null) { m_Redirectives.Reset(); }

						StartCoroutine(SetStandardPoseUpperIKLegFKLock());
					}
					else
					{
						// Still calibrating...
						if (upperIKLegFKCallback != null)
							upperIKLegFKCallback(this, CalibrationStatus.STATUS_COLLECTING);
					}

					return;
				}
				if (mode == BodyTrackingMode.ARMIK || mode == BodyTrackingMode.UPPERBODYIK || mode == BodyTrackingMode.FULLBODYIK)
				{
					if (m_StandardPose == null) { m_StandardPose = new BodyPose(mode); }
					m_StandardPose.Clear(mode);
					result = m_StandardPose.InitPoseInContent(mode);
					if (result != BodyTrackingResult.SUCCESS)
					{
						m_StandardPose = null;
						sb.Clear().Append(func).Append(mode.Name()).Append(", GetStandardPoseInContent failed, result: ").Append(result.Name()); ERROR(sb);
						if (upperIKLegFKCallback != null) { upperIKLegFKCallback(this, CalibrationStatus.STATUS_COMPUTEFAILED); }
						return;
					}

					if (m_TrackerExtrinsic == null) { m_TrackerExtrinsic = new TrackerExtrinsic(); }
					m_TrackerExtrinsic.UseDefault();
				}
			}

			if (upperIKLegFKCallback != null) { upperIKLegFKCallback(this, CalibrationStatus.STATUS_FINISHED); }
		}
#if WAVE_BODY_IK
		[Obsolete("This function is deprecated.")]
#endif
		public void StopCalibration(BodyTrackingMode mode)
		{
			string func = "StopCalibration() ";
			sb.Clear().Append(func).Append(mode.Name()); DEBUG(sb);

			upperIKLegFKCallback = null;

			if (mode == BodyTrackingMode.UPPERIKANDLEGFK) { inCalibration = false; }
		}
		#endregion

#if WAVE_BODY_IK
		private List<Rdp.BodyTrackingIK> s_BodyIK = new List<Rdp.BodyTrackingIK>();
		private Dictionary<int, BodyAvatar> s_AvatarIK = new Dictionary<int, BodyAvatar>();
		public BodyTrackingResult InitAvatarIK(Body avatarBody, out int trackingId)
		{
			string func = "InitAvatarIK() ";
			sb.Clear().Append(func); INFO(sb);

			// BodyTrackingIK.useDefaultTracking
			s_BodyIK.Add(new Rdp.BodyTrackingIK(avatarBody == null));
			trackingId = s_BodyIK.Count - 1;

			// BodyTrackingIK.customAvatar
			Rdp.Update(ref s_BodyIK[trackingId].customAvatar.avatarData, avatarBody);

			// BodyTrackingIK.ikTracker
			WVR_Result wvrResult = Rdp.CreateBodyTracker(ref s_BodyIK[trackingId].customAvatar, ref s_BodyIK[trackingId].ikTracker);
			sb.Clear().Append(func).Append("CreateBodyTracker(").Append(s_BodyIK[trackingId].ikTracker)
				.Append(") height: ").Append(s_BodyIK[trackingId].customAvatar.avatarData.height)
				.Append(", origin: ").Append(s_BodyIK[trackingId].customAvatar.avatarData.originType)
				.Append(", result: ").Append(wvrResult);
			DEBUG(sb);
			if (wvrResult != WVR_Result.WVR_Success || s_BodyIK[trackingId].ikTracker == 0)
			{
				s_BodyIK.RemoveAt(trackingId);
				return BodyTrackingResult.ERROR_AVATAR_INIT_FAILED;
			}

			BodyAvatar avatar = new BodyAvatar();
			avatar.Update(avatarBody);
			if (!s_AvatarIK.ContainsKey(trackingId)) { s_AvatarIK.Add(trackingId, avatar); }
			else { s_AvatarIK[trackingId].Update(avatar); }

			return BodyTrackingResult.SUCCESS;
		}
		public BodyTrackingResult DestroyAvatarIK(int trackingId)
		{
			string func = "DestroyAvatarIK() ";
			sb.Clear().Append(func).Append(trackingId); INFO(sb);
			if (trackingId < s_BodyIK.Count)
			{
				var result = Rdp.DestroyBodyTracker(s_BodyIK[trackingId].ikTracker);
				sb.Clear().Append(func).Append("DestroyBodyTracker(").Append(s_BodyIK[trackingId].ikTracker).Append(") result: ").Append(result); DEBUG(sb);

				s_BodyIK.RemoveAt(trackingId);
				if (s_AvatarIK.ContainsKey(trackingId)) { s_AvatarIK.Remove(trackingId); }

				if (result != WVR_Result.WVR_Success) { return BodyTrackingResult.ERROR_FATAL_ERROR; }
			}
			return BodyTrackingResult.SUCCESS;
		}
		private int avatarIKFrame = -1;
		private BodyTrackingResult UpdateAvatarIKData(int trackingId, bool printLog)
		{
			string func = "UpdateAvatarIKData() ";
			// Do NOT update IK if the focus is captured by system.
			if (!ClientInterface.IsFocused)
			{
				if (printLog)
				{
					sb.Clear().Append(func).Append(trackingId).Append(" no system focus.");
					DEBUG(sb);
				}
				return BodyTrackingResult.ERROR_IK_NOT_UPDATED;
			}
			if (printLog) { sb.Clear().Append(func).Append(trackingId); DEBUG(sb); }
			if (trackingId < s_BodyIK.Count && s_AvatarIK.ContainsKey(trackingId))
			{
				if (avatarIKFrame == Time.frameCount) { return BodyTrackingResult.SUCCESS; }
				avatarIKFrame = Time.frameCount;

				WVR_Result wvrResult = Rdp.GetBodyJointData(s_BodyIK[trackingId].ikTracker, s_BodyIK[trackingId].customAvatar.avatarData.originType, ref s_BodyIK[trackingId].ikJoint);
				if (printLog)
				{
					sb.Clear().Append(func).Append("GetBodyJointData(").Append(s_BodyIK[trackingId].ikTracker)
						.Append(") origin: ").Append(s_BodyIK[trackingId].customAvatar.avatarData.originType)
						.Append(", confidence: ").Append(s_BodyIK[trackingId].ikJoint.confidence)
						.Append(", result: ").Append(wvrResult);
					DEBUG(sb);
				}
				if (wvrResult != WVR_Result.WVR_Success) { return BodyTrackingResult.ERROR_IK_NOT_UPDATED; }

				s_AvatarIK[trackingId].confidence = s_BodyIK[trackingId].ikJoint.confidence;
				for (int i = 0; i < (UInt32)WVR_BodyJoint.WVR_BodyJoint_Count; i++)
				{
					JointType jointType = BodyTrackingUtils.GetJointType(i);

					PoseState poseState = PoseState.NODATA;
					if ((s_BodyIK[trackingId].ikJoint.joints[i].location.locationFlags & (UInt64)WVR_BodyIKFlag.WVR_ORIENTATION_VALID_BIT) != 0) { poseState |= PoseState.ROTATION; }
					if ((s_BodyIK[trackingId].ikJoint.joints[i].location.locationFlags & (UInt64)WVR_BodyIKFlag.WVR_POSITION_VALID_BIT) != 0) { poseState |= PoseState.TRANSLATION; }

					Quaternion rotation = Quaternion.identity;
					if (poseState.HasFlag(PoseState.ROTATION)) { Rdp.GetQuaternionFromGL(s_BodyIK[trackingId].ikJoint.joints[i].location.orientation, out rotation); }
					Vector3 translation = Vector3.zero;
					if (poseState.HasFlag(PoseState.TRANSLATION)) { Rdp.GetVector3FromGL(s_BodyIK[trackingId].ikJoint.joints[i].location.position, out translation); }

					Vector3 velocity = Vector3.zero;
					bool hasVelocity = ((s_BodyIK[trackingId].ikJoint.joints[i].velocity.velocityFlags & (UInt64)WVR_BodyIKFlag.WVR_VELOCITY_LINEAR_VALID_BIT) != 0);
					if (hasVelocity) { Rdp.GetVector3FromGL(s_BodyIK[trackingId].ikJoint.joints[i].velocity.linearVelocity, out velocity); }

					Vector3 angularVelocity = Vector3.zero;
					bool hasAngularVelocity = ((s_BodyIK[trackingId].ikJoint.joints[i].velocity.velocityFlags & (UInt64)WVR_BodyIKFlag.WVR_VELOCITY_ANGULAR_VALID_BIT) != 0);
					if (hasAngularVelocity) { Rdp.GetAngularVector3FromGL(s_BodyIK[trackingId].ikJoint.joints[i].velocity.angularVelocity, out angularVelocity); }

					s_AvatarIK[trackingId].Update(jointType, poseState, translation, velocity, angularVelocity, rotation, printLog);
				}
				return BodyTrackingResult.SUCCESS;
			}

			return BodyTrackingResult.ERROR_AVATAR_INIT_FAILED;
		}
		public BodyTrackingResult GetAvatarIKData(int trackingId, out BodyAvatar avatarBody)
		{
			if (UpdateAvatarIKData(trackingId, m_EnableTrackingLog) == BodyTrackingResult.SUCCESS)
			{
				avatarBody = s_AvatarIK[trackingId];
				return BodyTrackingResult.SUCCESS;
			}

			avatarBody = null;
			return BodyTrackingResult.ERROR_IK_NOT_UPDATED;
		}
		private BodyTrackingResult UpdateAvatarIKInfo(int trackingId, bool printLog)
		{
			string func = "UpdateAvatarIKInfo() ";
			// Do NOT update IK if the focus is captured by system.
			if (!ClientInterface.IsFocused)
			{
				if (printLog)
				{
					sb.Clear().Append(func).Append(trackingId).Append(" no system focus.");
					DEBUG(sb);
				}
				return BodyTrackingResult.ERROR_IK_NOT_UPDATED;
			}
			if (printLog) { sb.Clear().Append(func).Append(trackingId); DEBUG(sb); }
			// Call GetBodyProperties if not Default Body Tracking.
			if (trackingId < s_BodyIK.Count && !s_BodyIK[trackingId].useDefaultTracking && s_AvatarIK.ContainsKey(trackingId))
			{
				WVR_Result wvrResult = Rdp.GetBodyProperties(s_BodyIK[trackingId].ikTracker, ref s_BodyIK[trackingId].ikInfo);
				if (printLog)
				{
					sb.Clear().Append(func).Append("GetBodyProperties(").Append(s_BodyIK[trackingId].ikTracker)
						.Append(") scale: ").Append(s_BodyIK[trackingId].ikInfo.scale)
						.Append(", result: ").Append(wvrResult);
					DEBUG(sb);
				}
				if (wvrResult != WVR_Result.WVR_Success || s_BodyIK[trackingId].ikInfo.scale <= 0) { return BodyTrackingResult.ERROR_AVATAR_INIT_FAILED; }

				s_AvatarIK[trackingId].scale = s_BodyIK[trackingId].ikInfo.scale;
				if (printLog)
				{
					sb.Clear().Append(func).Append("s_AvatarIK[").Append(trackingId)
						.Append("] height: ").Append(s_AvatarIK[trackingId].height)
						.Append(", scale: ").Append(s_AvatarIK[trackingId].scale);
					DEBUG(sb);
				}
			}

			return BodyTrackingResult.SUCCESS;
		}
		public BodyTrackingResult GetAvatarIKInfo(int trackingId, out float avatarHeight, out float avatarScale)
		{
			avatarHeight = 1.5f;
			avatarScale = 1;

			if (s_AvatarIK.ContainsKey(trackingId))
			{
				avatarHeight = s_AvatarIK[trackingId].height;
				avatarScale = s_AvatarIK[trackingId].scale;
				return BodyTrackingResult.SUCCESS;
			}

			return BodyTrackingResult.ERROR_NOT_CALIBRATED;
		}
#endif

		// ------ Init Avatar IK: Init BodyIKInfo (from Body & TrackerExtrinsic) ------
		/// <summary>
		/// Creates Body Tracking IK with the custom avatar (<see cref="Body">Body</see>) and tracker extrinsics (<see cref="TrackerExtrinsic">TrackerExtrinsic</see>).
		/// <br></br>
		/// Before initializing calibration:
		/// <br></br>
		/// 1. Checks if specified <see cref="BodyTrackingMode">BodyTrackingMode</see> is supported by the calibration pose.
		/// <br></br>
		/// 2. Checks if tracked device extrinsics support the <see cref="BodyTrackingMode">BodyTrackingMode</see> and calibration pose.
		/// </summary>
		/// <param name="skeletonId">Output ID for the IK.</param>
		/// <param name="avatarBody">Custom avatar.</param>
		/// <param name="trackerExtrinsic">Custom tracked device extrinsics.</param>
		/// <param name="mode">Body tracking mode which is full body tracking by default.</param>
		/// <returns>SUCCESS for creating custom body tracking IK successfully.</returns>
#if WAVE_BODY_IK
		[Obsolete("This function is deprecated, please use InitAvatarIK() instead.")]
#endif
		public BodyTrackingResult CreateBodyTracking(ref int skeletonId, Body avatarBody, TrackerExtrinsic trackerExtrinsic, BodyTrackingMode mode = BodyTrackingMode.FULLBODYIK)
		{
			sb.Clear().Append("CreateBodyTracking() with Avatar and Tracker Extrinsics, mode: ").Append(mode.Name()); DEBUG(sb);

			if (m_StandardPose == null)
			{
				sb.Clear().Append("CreateBodyTracking() Never calibrated standard poses before."); ERROR(sb);
				return BodyTrackingResult.ERROR_NOT_CALIBRATED;
			}
			if (avatarBody == null)
			{
				sb.Clear().Append("CreateBodyTracking() Invalid Avatar Body."); ERROR(sb);
				return BodyTrackingResult.ERROR_INVALID_ARGUMENT;
			}
			if (trackerExtrinsic == null)
			{
				sb.Clear().Append("CreateBodyTracking() Invalid TrackerExtrinsic.");
				ERROR(sb);
				return BodyTrackingResult.ERROR_INVALID_ARGUMENT;
			}

			#region API v1.0.0.6
			if (m_Redirectives != null && m_Redirectives.InUse)
			{
				/// The Standard Pose is already redirected in runtime and in SetStandardPoseUpperIKLegFKLock()
				/// Since the trackerExtrinsic can be customized, we still need to redirect extrinsics here.
				sb.Clear().Append("CreateBodyTracking() Redirect extrinsics."); DEBUG(sb);
				trackerExtrinsic.Update(m_Redirectives.DeviceExts);
			}
			#endregion

			// Duplicates an BodyPose instance to prevent modifying m_StandardPose.
			// 1-1. Retrieves the calibration pose.
			BodyPose calibPose = new BodyPose(mode);
			calibPose.Update(m_StandardPose);
			BodyPoseRole calibRole = calibPose.GetIKRoles(true);
			sb.Clear().Append("CreateBodyTracking() mode: ").Append(mode.Name()).Append(", calibration role: ").Append(calibRole.Name()); DEBUG(sb);

			// 1-2. Checks if the body tracking mode is available.
			if (!BodyTrackingUtils.MatchBodyTrackingMode(mode, calibRole))
			{
				sb.Clear().Append("CreateBodyTracking() The body tracking mode ").Append(mode.Name()).Append(" and calibration role ").Append(calibRole.Name()).Append(" are NOT matched."); ERROR(sb);
				return BodyTrackingResult.ERROR_BODYTRACKINGMODE_NOT_ALIGNED;
			}

			// Updates BodyAvatar.
			BodyAvatar calibAvatar = new BodyAvatar();
			calibAvatar.Update(avatarBody);

			// 2-1. Retrieves the tracked device extrinsics according to the calibration role.
			if (!trackerExtrinsic.GetDevicesExtrinsics(calibRole, out TrackedDeviceExtrinsic[] bodyTrackedDevices, out UInt32 bodyTrackedDeviceCount))
			{
				sb.Clear().Append("CreateBodyTracking() Cannot get tracked device extrinsics."); ERROR(sb);
				return BodyTrackingResult.ERROR_AVATAR_INIT_FAILED;
			}
			sb.Clear().Append("CreateBodyTracking() Init IK uses ").Append(bodyTrackedDeviceCount).Append(" tracked device extrinsics:");
			for (UInt32 u = 0; u < bodyTrackedDeviceCount; u++)
			{
				sb.Append("\nCreateBodyTracking() ").Append(u).Append(": ").Append(bodyTrackedDevices[u].trackedDeviceRole.Name())
					.Append(" (").Append(bodyTrackedDevices[u].extrinsic.translation.x.ToString("N3")).Append(", ").Append(bodyTrackedDevices[u].extrinsic.translation.y.ToString("N3")).Append(", ").Append(bodyTrackedDevices[u].extrinsic.translation.z.ToString("N3")).Append(")")
					.Append(" (").Append(bodyTrackedDevices[u].extrinsic.rotation.x.ToString("N3")).Append(", ").Append(bodyTrackedDevices[u].extrinsic.rotation.y.ToString("N3")).Append(", ").Append(bodyTrackedDevices[u].extrinsic.rotation.z.ToString("N3")).Append(", ").Append(bodyTrackedDevices[u].extrinsic.rotation.w.ToString("N3")).Append(")");
			}
			DEBUG(sb);

			DeviceExtRole extRole = BodyTrackingUtils.GetDeviceExtRole(calibRole, bodyTrackedDevices, bodyTrackedDeviceCount);
			sb.Clear().Append("CreateBodyTracking() retrieves tracked device extrinsics successfully, mode: ").Append(mode.Name()).Append(", extrinsic role: ").Append(extRole.Name()); DEBUG(sb);

			// 2-2. Checks if the body tracking mode is available.
			if (!BodyTrackingUtils.MatchBodyTrackingMode(mode, extRole))
			{
				sb.Clear().Append("CreateBodyTracking() The body tracking mode ").Append(mode.Name()).Append(" and device extrinsic role ").Append(extRole.Name()).Append(" are NOT matched."); ERROR(sb);
				return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
			}

			// 3. Retrieves the Avatar T-Pose joints.
			if (!calibAvatar.GetJoints(out Joint[] avatarJoints, out UInt32 avatarJointCount, true))
			{
				sb.Clear().Append("CreateBodyTracking() Cannot get 6DoF joints.");
				ERROR(sb);
				return BodyTrackingResult.ERROR_AVATAR_INIT_FAILED;
			}
			sb.Clear().Append("CreateBodyTracking() Init IK uses ").Append(avatarJointCount).Append(" avatar joints: \n");
			for (UInt32 u = 0; u < avatarJointCount; u++)
			{
				sb.Append(u).Append(": ").Append(avatarJoints[u].jointType.Name()).Append("\n");
			}
			DEBUG(sb);

			// Initializes IK
			UInt64 ts = BodyTrackingUtils.GetTimeStamp();
#if CoordinateOpenGL
			Result result = fbt.InitBodyTrackingT(
				ts: ts,
				bodyTrackingMode: mode,
				trackedDeviceExt: bodyTrackedDevices,
				deviceCount: bodyTrackedDeviceCount,
				avatarJoints: avatarJoints,
				avatarJointCount: avatarJointCount,
				avatarHeight: calibAvatar.height, ref skeletonId);
#else
			Result result = fbt.InitBodyTrackingLog(
				ts: ts,
				bodyTrackingMode: mode,
				trackedDeviceExt: bodyTrackedDevices,
				deviceCount: bodyTrackedDeviceCount,
				avatarJoints: avatarJoints,
				avatarJointCount: avatarJointCount,
				avatarHeight: calibAvatar.height, ref skeletonId);
#endif
			if (result != Result.SUCCESS)
			{
				sb.Clear().Append("CreateBodyTracking() InitBodyTracking failed, result: ").Append(result.Type().Name());
				ERROR(sb);
				return result.Type();
			}

			sb.Clear().Append("CreateBodyTracking() Initial IK successfully, mode: ").Append(mode.Name()); DEBUG(sb);

			// Calibrates IK.
			if (CalibrateBodyTracking(skeletonId, ts, mode, calibAvatar, calibPose, false) != BodyTrackingResult.SUCCESS)
			{
				sb.Clear().Append("CreateBodyTracking() CalibrateBodyTracking failed."); ERROR(sb);
				return BodyTrackingResult.ERROR_CALIBRATE_FAILED;
			}
			sb.Clear().Append("CreateBodyTracking() Calibrate IK successfully, mode: ").Append(mode.Name()); DEBUG(sb);

			return BodyTrackingResult.SUCCESS;
		}

		// ------ Init Avatar IK: Init BodyIKInfo (from Body & default TrackerExtrinsic) ------
		/// <summary>
		/// Creates Body Tracking IK with the custom avatar (<see cref="Body">Body</see>).
		/// <br></br>
		/// Before initializing calibration:
		/// <br></br>
		/// 1. Checks if specified <see cref="BodyTrackingMode">BodyTrackingMode</see> is supported by the calibration pose.
		/// <br></br>
		/// 2. Checks if default tracked device extrinsics support the <see cref="BodyTrackingMode">BodyTrackingMode</see> and calibration pose.
		/// </summary>
		/// <param name="skeletonId">Output ID for the IK.</param>
		/// <param name="avatarBody">Custom avatar.</param>
		/// <param name="mode">Body tracking mode which is full body tracking by default.</param>
		/// <returns>SUCCESS for creating custom body tracking IK successfully.</returns>
#if WAVE_BODY_IK
		[Obsolete("This function is deprecated, please use InitAvatarIK() instead.")]
#endif
		public BodyTrackingResult CreateBodyTracking(ref int skeletonId, Body avatarBody, BodyTrackingMode mode = BodyTrackingMode.FULLBODYIK)
		{
			sb.Clear().Append("CreateBodyTracking() with Avatar, mode: ").Append(mode.Name()); DEBUG(sb);

			if (m_StandardPose == null)
			{
				sb.Clear().Append("CreateBodyTracking() Never calibrated standard poses before."); ERROR(sb);
				return BodyTrackingResult.ERROR_NOT_CALIBRATED;
			}
			if (m_TrackerExtrinsic == null)
			{
				sb.Clear().Append("CreateBodyTracking() Never calibrated device extrinsics before."); ERROR(sb);
				return BodyTrackingResult.ERROR_NOT_CALIBRATED;
			}

			return CreateBodyTracking(ref skeletonId, avatarBody, m_TrackerExtrinsic, mode);
		}

		// ------ Init Default Body Tracking: Init BodyIKInfo and retrieve the Default Rotation Spaces ------
		private Dictionary<int, BodyRotationSpace_t> s_BodyRotationSpaces = new Dictionary<int, BodyRotationSpace_t>();
		/// <summary>
		/// Creates Body Tracking IK with all default configurations.
		/// <br></br>
		/// Before initializing calibration:
		/// <br></br>
		/// 1. Checks if specified <see cref="BodyTrackingMode">BodyTrackingMode</see> is supported by the calibration pose.
		/// <br></br>
		/// 2. Checks if default tracked device extrinsics support the <see cref="BodyTrackingMode">BodyTrackingMode</see> and calibration pose.
		/// </summary>
		/// <param name="skeletonId">Output ID for the IK.</param>
		/// <param name="mode">Body tracking mode which is full body tracking by default.</param>
		/// <returns>SUCCESS for creating default body tracking IK successfully.</returns>
#if WAVE_BODY_IK
		[Obsolete("This function is deprecated, please use InitAvatarIK() instead.")]
#endif
		public BodyTrackingResult CreateBodyTracking(ref int skeletonId, BodyTrackingMode mode = BodyTrackingMode.FULLBODYIK)
		{
			sb.Clear().Append("CreateBodyTracking() with body tracking mode: ").Append(mode.Name()); DEBUG(sb);

			if (m_StandardPose == null)
			{
				sb.Clear().Append("CreateBodyTracking() Never calibrated standard poses before."); ERROR(sb);
				return BodyTrackingResult.ERROR_NOT_CALIBRATED;
			}
			if (m_TrackerExtrinsic == null)
			{
				sb.Clear().Append("CreateBodyTracking() Never calibrated device extrinsics before."); ERROR(sb);
				return BodyTrackingResult.ERROR_NOT_CALIBRATED;
			}

			// Duplicates an BodyPose instance to prevent modifying m_StandardPose.
			// 1-1. Retrieves the calibration pose.
			BodyPose calibPose = new BodyPose(mode);
			calibPose.Update(m_StandardPose);

			BodyPoseRole calibRole = calibPose.GetIKRoles(true);
			sb.Clear().Append("CreateBodyTracking() retrieves calibration pose successfully, mode: ").Append(mode.Name()).Append(", calibration role: ").Append(calibRole.Name()); DEBUG(sb);

			// 1-2. Checks if the body tracking mode is available.
			if (!BodyTrackingUtils.MatchBodyTrackingMode(mode, calibRole))
			{
				sb.Clear().Append("CreateBodyTracking() The body tracking mode ").Append(mode.Name()).Append(" and calibration role ").Append(calibRole.Name()).Append(" are NOT matched."); ERROR(sb);
				return BodyTrackingResult.ERROR_BODYTRACKINGMODE_NOT_ALIGNED;
			}

			// No need to update BodyAvatar.
			BodyAvatar calibAvatar = new BodyAvatar();
			//calibAvatar.Update(avatarBody);

			// 2-1. Retrieves the tracked device extrinsics according to the calibration role.
			if (!m_TrackerExtrinsic.GetDevicesExtrinsics(calibRole, out TrackedDeviceExtrinsic[] bodyTrackedDevices, out UInt32 bodyTrackedDeviceCount))
			{
				sb.Clear().Append("CreateBodyTracking() Cannot get tracked device extrinsics."); ERROR(sb);
				return BodyTrackingResult.ERROR_AVATAR_INIT_FAILED;
			}
			sb.Clear().Append("CreateBodyTracking() Init IK uses ").Append(bodyTrackedDeviceCount).Append(" tracked devices: \n");
			for (UInt32 u = 0; u < bodyTrackedDeviceCount; u++)
			{
				sb.Append(u).Append(": ").Append(bodyTrackedDevices[u].trackedDeviceRole.Name()).Append("\n");
			}
			DEBUG(sb);

			DeviceExtRole extRole = BodyTrackingUtils.GetDeviceExtRole(calibRole, bodyTrackedDevices, bodyTrackedDeviceCount);
			sb.Clear().Append("CreateBodyTracking retrieves tracked device extrinsics successfully, mode: ").Append(mode.Name()).Append(", extrinsic role: ").Append(extRole.Name()); DEBUG(sb);

			// 2-2. Checks if the body tracking mode is available.
			if (!BodyTrackingUtils.MatchBodyTrackingMode(mode, extRole))
			{
				sb.Clear().Append("CreateBodyTracking() The body tracking mode ").Append(mode.Name()).Append(" and device extrinsic role ").Append(extRole.Name()).Append(" are NOT matched."); ERROR(sb);
				return BodyTrackingResult.ERROR_INPUTPOSE_NOT_VALID;
			}

			// Initializes IK
			UInt64 ts = BodyTrackingUtils.GetTimeStamp();
#if CoordinateOpenGL
			Result result = fbt.InitDefaultBodyTrackingT(
				ts: ts,
				bodyTrackingMode: mode,
				trackedDeviceExt: bodyTrackedDevices,
				deviceCount: bodyTrackedDeviceCount,
				ref skeletonId);
#else
			Result result = fbt.InitDefaultBodyTrackingLog(
				ts: ts,
				bodyTrackingMode: mode,
				trackedDeviceExt: bodyTrackedDevices,
				deviceCount: bodyTrackedDeviceCount,
				ref skeletonId);
#endif
			if (result != Result.SUCCESS)
			{
				sb.Clear().Append("CreateBodyTracking() InitDefaultBodyTracking failed, result: ").Append(result.Type().Name());
				ERROR(sb);
				return result.Type();
			}

			sb.Clear().Append("CreateBodyTracking() Initial IK successfully, tracking mode: ").Append(mode.Name()); DEBUG(sb);

			// Retrieves Default Rotation Spaces
			UInt32 jointCount = 0;
			result = fbt.GetDefaultSkeletonJointCount(ref jointCount);
			if (result != Result.SUCCESS || jointCount == 0 || jointCount > (UInt32)JointType.MAX_ENUM)
			{
				sb.Clear().Append("CreateBodyTracking() GetDefaultSkeletonJointCount failed, count: ").Append(jointCount).Append(", result: ").Append(result.Type().Name()); ERROR(sb);
				return BodyTrackingResult.ERROR_AVATAR_INIT_FAILED;
			}

			RotateSpace[] rotateSpaces = new RotateSpace[jointCount];
			result = fbt.GetDefaultSkeletonRotateSpace(rotateSpaces, jointCount);
			if (result != Result.SUCCESS || rotateSpaces.Length <= 0)
			{
				sb.Clear().Append("CreateBodyTracking() GetDefaultSkeletonRotateSpace failed, count: ").Append(jointCount).Append(", result: ").Append(result.Type().Name());
				ERROR(sb);
				return BodyTrackingResult.ERROR_AVATAR_INIT_FAILED;
			}

			s_BodyRotationSpaces.Add(skeletonId, new BodyRotationSpace_t(rotateSpaces, jointCount));

			// Calibrates IK.
			if (CalibrateBodyTracking(skeletonId, ts, mode, calibAvatar, calibPose, true) != BodyTrackingResult.SUCCESS)
			{
				sb.Clear().Append("CreateBodyTracking() CalibrateBodyTracking failed."); ERROR(sb);
				return BodyTrackingResult.ERROR_CALIBRATE_FAILED;
			}
			sb.Clear().Append("CreateBodyTracking() Calibrate IK successfully, tracking mode: ").Append(mode.Name()); DEBUG(sb);

			return BodyTrackingResult.SUCCESS;
		}
#if WAVE_BODY_IK
		[Obsolete("This function is deprecated.")]
#endif
		public BodyTrackingResult GetDefaultRotationSpace(int skeletonId, out RotateSpace[] spaces, out UInt32 count)
		{
			if (s_BodyRotationSpaces != null && s_BodyRotationSpaces.ContainsKey(skeletonId))
			{
				spaces = s_BodyRotationSpaces[skeletonId].spaces;
				count = s_BodyRotationSpaces[skeletonId].count;
				return BodyTrackingResult.SUCCESS;
			}
			spaces = null;
			count = 0;
			return BodyTrackingResult.ERROR_AVATAR_INIT_FAILED;
		}

		// ------ Calibrate IK: Calculate BodyPose (from standard pose) ------
		private Dictionary<int, BodyPose> s_BodyPoses = new Dictionary<int, BodyPose>();
		private Dictionary<int, BodyAvatar> s_BodyAvatars = new Dictionary<int, BodyAvatar>();
		private const float kHeightMin = 0.5f;
		/// <summary>
		/// Calibrate Default/Custom Body Tracking IK.
		/// </summary>
		/// <param name="id">Skeleton ID from Init IK.</param>
		/// <param name="ts">Timestamp in UInt64.</param>
		/// <param name="mode">Tracking mode from Init IK.</param>
		/// <param name="isDefault">True for default Body Tracking IK.</param>
		/// <returns>SUCCESS for successful calibration.</returns>
		private BodyTrackingResult CalibrateBodyTracking(int id, UInt64 ts, BodyTrackingMode mode, BodyAvatar calibAvatar, BodyPose calibPose, bool isDefault)
		{
			sb.Clear().Append("CalibrateBodyTracking() ").Append(id)
				.Append(", timestamp: ").Append(ts)
				.Append(", mode: ").Append(mode.Name())
				.Append(", isDefault: ").Append(isDefault);
			DEBUG(sb);

			if (!calibPose.GetTrackedDevicePoses(true, out TrackedDevicePose[] trackedDevicePoses, out UInt32 trackedDevicePoseCount))
			{
				sb.Clear().Append("UpdateBodyTrackingOnce() Cannot tracked device poses."); ERROR(sb);
				return BodyTrackingResult.ERROR_CALIBRATE_FAILED;
			}
			BodyPoseRole calibRole = BodyTrackingUtils.GetBodyPoseRole(trackedDevicePoses, trackedDevicePoseCount);
			sb.Clear().Append("CalibrateBodyTracking() ").Append(id).Append(", calibRole: ").Append(calibRole.Name()); DEBUG(sb);

			// Checks if the body tracking mode is available.
			if (!BodyTrackingUtils.MatchBodyTrackingMode(mode, calibRole))
			{
				sb.Clear().Append("CalibrateBodyTracking() Can not figure out the body tracking initial mode ").Append(mode.Name()); ERROR(sb);
				return BodyTrackingResult.ERROR_CALIBRATE_FAILED;
			}

			// Calibrates IK with the L-Pose.
			if (calibPose.userCalibrationHeight <= kHeightMin)
			{
				sb.Clear().Append("CalibrateBodyTracking() Invalid HMD height ").Append(calibPose.userCalibrationHeight).Append(", make sure your camera in Tracking Origin - Floor mode.");
				ERROR(sb);
				return BodyTrackingResult.ERROR_INVALID_ARGUMENT;
			}

			// scale = user_height / avatar_height
			float scale = isDefault ? 1 : (calibPose.userCalibrationHeight / (calibAvatar.height > 0 ? calibAvatar.height : BodyAvatar.kAvatarHeight));
			sb.Clear().Append("CalibrateBodyTracking() Head height: ").Append(calibPose.userCalibrationHeight).Append(", avatar height: ").Append(calibAvatar.height).Append(", scale: ").Append(scale);
			DEBUG(sb);
#if CoordinateOpenGL
			Result result = fbt.CalibrateBodyTrackingT(
				ts: ts,
				skeletonId: id,
				userHeight: calibPose.userCalibrationHeight,
				bodyTrackingMode: mode,
				trackedDevicePose: trackedDevicePoses,
				deviceCount: trackedDevicePoseCount, ref scale);
#else
			Result result = fbt.CalibrateBodyTrackingLog(
				ts: ts,
				skeletonId: id,
				userHeight: calibPose.userCalibrationHeight,
				bodyTrackingMode: mode,
				trackedDevicePose: trackedDevicePoses,
				deviceCount: trackedDevicePoseCount, ref scale);
#endif
			if (result != Result.SUCCESS)
			{
				sb.Clear().Append("CalibrateBodyTracking() CalibrateBodyTracking failed, result: ").Append(result.Type().Name());
				ERROR(sb);
				return result.Type();
			}

			#region API v1.0.0.6
			if (mode == BodyTrackingMode.UPPERIKANDLEGFK)
			{
				if (m_Redirectives == null)
				{
					sb.Clear().Append("CalibrateBodyTracking() ").Append(mode.Name()).Append(" never calibrated before.");
					WARNING(sb);
				}
				if (m_Redirectives != null && !m_Redirectives.InUse)
				{
					sb.Clear().Append("CalibrateBodyTracking() ").Append(mode.Name()).Append(" redirective data is unused.");
					WARNING(sb);
				}
				if (m_Redirectives != null && m_Redirectives.InUse)
				{
#if CoordinateOpenGL
					result = fbt.RedirectTrackedDeviceT(ts, id, m_Redirectives.ExtrinsicCount, m_Redirectives.RedirectExts);
#else
					result = fbt.RedirectTrackedDeviceLog(ts, id, m_Redirectives.ExtrinsicCount, m_Redirectives.RedirectExts);
#endif
					if (result != Result.SUCCESS)
					{
						sb.Clear().Append("CalibrateBodyTracking() RedirectTrackedDevice failed, result: ").Append(result.Type().Name()); ERROR(sb);
						return result.Type();
					}
				}
			}
			#endregion

			// Save the calibration pose as cached device pose and will be updated at UpdateBodyTrackingOnce().
			s_BodyPoses.Add(id, calibPose);

			s_BodyAvatars.Add(id, new BodyAvatar());
			s_BodyAvatars[id].Update(calibAvatar);
			s_BodyAvatars[id].height = (calibPose.userCalibrationHeight / scale);
			s_BodyAvatars[id].scale = scale;

			sb.Clear().Append("CalibrateBodyTracking() Calibrate ").Append(isDefault ? "Default" : "Custom").Append(" IK successfully")
				.Append(", avatar height: ").Append(s_BodyAvatars[id].height)
				.Append(", scale: ").Append(s_BodyAvatars[id].scale);
			DEBUG(sb);

			return BodyTrackingResult.SUCCESS;
		}

		/// <summary>
		/// After <see cref="CreateBodyTracking">CreateBodyTracking</see> you can retrieve the calibrated avatar height and scale.
		/// </summary>
		/// <param name="skeletonId"> The skeleton ID from <see cref="CreateBodyTracking">CreateBodyTracking</see>.</param>
		/// <param name="avatarHeight">The calibrated avatar height which is usually the HMD's y-axis of position.</param>
		/// <param name="avatarScale">The calibrated avatar scale which is 1 when using default Body Tracking. Otherwise scale = {HMD y-axis} / your avatar's height.</param>
		/// <returns>True if the calibration completes successfully.</returns>
#if WAVE_BODY_IK
		[Obsolete("This function is deprecated, please use GetAvatarIKData() instead.")]
#endif
		public BodyTrackingResult GetBodyTrackingInfo(int skeletonId, out float avatarHeight, out float avatarScale)
		{
			avatarHeight = 1.5f;
			avatarScale = 1;

			if (s_BodyAvatars.ContainsKey(skeletonId))
			{
				avatarHeight = s_BodyAvatars[skeletonId].height;
				avatarScale = s_BodyAvatars[skeletonId].scale;
				return BodyTrackingResult.SUCCESS;
			}

			return BodyTrackingResult.ERROR_NOT_CALIBRATED;
		}

		// ------ Update IK: Calculate BodyAvatar according to BodyIKInfo & BodyPose ------
		private BodyTrackingResult IsValidSkeletonId(int id)
		{
			if (!s_BodyPoses.ContainsKey(id)) { return BodyTrackingResult.ERROR_NOT_CALIBRATED; }
			return BodyTrackingResult.SUCCESS;
		}
		private UInt32 m_OutputJointCount = 0;
		private Joint[] s_OutputJoints = null;

		private BodyTrackingResult UpdateBodyTrackingOnce(int id)
		{
			// Do NOT update IK if the focus is captured by system.
			//if (!ClientInterface.IsFocused)
			//{
			//	if (printIntervalLog)
			//	{
			//		sb.Clear().Append("UpdateBodyTrackingOnce() ").Append(id).Append(" no system focus.");
			//		DEBUG(sb);
			//	}
			//	return BodyTrackingResult.ERROR_IK_NOT_UPDATED;
			//}
			// Checks if the initialization succeeds.
			var btResult = IsValidSkeletonId(id);
			if (btResult != BodyTrackingResult.SUCCESS)
			{
				if (printIntervalLog)
				{
					sb.Clear().Append("UpdateBodyTrackingOnce() invalid id ").Append(id).Append(" btResult ").Append(btResult.Name());
					ERROR(sb);
				}
				return btResult;
			}

			// Updates the device pose.
			if (s_BodyPoses[id].UpdatePoseInContent(m_EnableTrackingLog) != BodyTrackingResult.SUCCESS)
			{
				sb.Clear().Append("UpdateBodyTrackingOnce() UpdateBodyTrackingOnce poses failed for ID ").Append(id);
				ERROR(sb);
				return BodyTrackingResult.ERROR_IK_NOT_UPDATED;
			}

			// Updates IK with the calibrate pose.
			UInt64 ts = BodyTrackingUtils.GetTimeStamp();

			UInt32 jointCount = 0;
			Result result = fbt.GetOutputJointCount(ts, id, ref jointCount);
			if (result != Result.SUCCESS || jointCount == 0)
			{
				sb.Clear().Append("UpdateBodyTrackingOnce() GetOutputJointCount failed, jointCount: ").Append(jointCount).Append(", result: ").Append(result.Type().Name());
				ERROR(sb);
				return BodyTrackingResult.ERROR_IK_NOT_UPDATED;
			}
			if (m_OutputJointCount != jointCount)
			{
				m_OutputJointCount = jointCount;
				s_OutputJoints = new Joint[m_OutputJointCount];
			}
			if (!s_BodyPoses[id].GetTrackedDevicePoses(false, out TrackedDevicePose[] trackedDevicePoses, out UInt32 trackedDevicePoseCount))
			{
				sb.Clear().Append("UpdateBodyTrackingOnce() Cannot tracked device poses."); ERROR(sb);
				return BodyTrackingResult.ERROR_IK_NOT_UPDATED;
			}
#if CoordinateOpenGL
			result = fbt.UpdateBodyTrackingT(m_EnableTrackingLog, ts, id, trackedDevicePoses, trackedDevicePoseCount, s_OutputJoints, m_OutputJointCount);
#else
			if (m_EnableTrackingLog)
			{
				result = fbt.UpdateBodyTrackingLog(ts, id, trackedDevicePoses, trackedDevicePoseCount, s_OutputJoints, m_OutputJointCount);
			}
			else
			{
				result = fbt.UpdateBodyTracking(ts, id, trackedDevicePoses, trackedDevicePoseCount, s_OutputJoints, m_OutputJointCount);
			}
#endif
			if (result != Result.SUCCESS)
			{
				sb.Clear().Append("UpdateBodyTrackingOnce() UpdateBodyTracking failed, result: ").Append(result.Type().Name());
				ERROR(sb);
				return result.Type();
			}

			// Comes out the bodyAvatar.
			if (s_BodyAvatars.ContainsKey(id))
			{
				s_BodyAvatars[id].Set6DoFJoints(s_OutputJoints, m_OutputJointCount);
			}
			if (printIntervalLog)
			{
				LogJoint("UpdateBodyTrackingOnce()", s_BodyAvatars[id].head);
				LogJoint("UpdateBodyTrackingOnce()", s_BodyAvatars[id].leftHand);
				LogJoint("UpdateBodyTrackingOnce()", s_BodyAvatars[id].rightHand);
				LogJoint("UpdateBodyTrackingOnce()", s_BodyAvatars[id].hip);
				LogJoint("UpdateBodyTrackingOnce()", s_BodyAvatars[id].leftLeg);
				LogJoint("UpdateBodyTrackingOnce()", s_BodyAvatars[id].rightLeg);
				LogJoint("UpdateBodyTrackingOnce()", s_BodyAvatars[id].leftAnkle);
				LogJoint("UpdateBodyTrackingOnce()", s_BodyAvatars[id].rightAnkle);
			}

			return BodyTrackingResult.SUCCESS;
		}
		private void LogJoint(string tag, Joint joint)
		{
			sb.Clear().Append(tag).Append(joint.jointType.Name()).Append(" poseState: ").Append(joint.poseState)
				.Append(", pos (").Append(joint.translation.x.ToString("N2")).Append(", ").Append(joint.translation.y.ToString("N2")).Append(", ").Append(joint.translation.z.ToString("N2")).Append(")")
				.Append(", rot (").Append(joint.rotation.x.ToString("N2")).Append(", ").Append(joint.rotation.y.ToString("N2")).Append(", ").Append(joint.rotation.z.ToString("N2")).Append(", ").Append(joint.rotation.w.ToString("N2")).Append(")");
			DEBUG(sb);
		}
#if WAVE_BODY_IK
		[Obsolete("This function is deprecated, please use GetAvatarIKData() instead.")]
#endif
		public BodyTrackingResult GetBodyTrackingPoses(int skeletonId, out BodyAvatar avatarBody)
		{
			if (s_BodyAvatars.ContainsKey(skeletonId))
			{
				avatarBody = s_BodyAvatars[skeletonId];
				return BodyTrackingResult.SUCCESS;
			}

			avatarBody = null;
			return BodyTrackingResult.ERROR_IK_NOT_UPDATED;
		}
#if WAVE_BODY_IK
		[Obsolete("This function is deprecated, please use GetAvatarIKData() instead.")]
#endif
		public BodyTrackingResult GetBodyTrackingPoseOnce(int skeletonId, out BodyAvatar avatarBody)
		{
			var result = UpdateBodyTrackingOnce(skeletonId);
			if (result == BodyTrackingResult.SUCCESS)
			{
				return GetBodyTrackingPoses(skeletonId, out avatarBody);
			}

			avatarBody = null;
			return result;
		}

		// ------ Destroy IK: Destroy IK according to BodyIKInfo ------
#if WAVE_BODY_IK
		[Obsolete("This function is deprecated, please use DestroyAvatarIK() instead.")]
#endif
		public BodyTrackingResult DestroyBodyTracking(int skeletonId)
		{
			BodyTrackingResult btResult = BodyTrackingResult.ERROR_IK_NOT_DESTROYED;

			UInt64 ts = BodyTrackingUtils.GetTimeStamp();
			btResult = fbt.DestroyBodyTrackingLog(ts, skeletonId).Type();
			sb.Clear().Append("DestroyBodyTracking() DestroyBodyTracking ").Append(skeletonId).Append(" result ").Append(btResult.Name()); DEBUG(sb);

			// After InitDefaultBodyTracking, we collected the default rotation spaces to s_BodyRotationSpaces.
			if (s_BodyRotationSpaces.ContainsKey(skeletonId)) { s_BodyRotationSpaces.Remove(skeletonId); }
			// After CalibrateBodyTracking, we collected the initial pose to s_BodyPoses. s_BodyPoses will be updated in UpdateBodyTrackingOnce().
			if (s_BodyPoses.ContainsKey(skeletonId)) { s_BodyPoses.Remove(skeletonId); }
			// In UpdateBodyTrackingOnce, we collected the Body Avatar information in s_BodyAvatars
			if (s_BodyAvatars.ContainsKey(skeletonId)) { s_BodyAvatars.Remove(skeletonId); }

			return btResult;
		}

#if WAVE_BODY_IK
		[Obsolete("This function is deprecated, please use GetAvatarIKData() instead.")]
#endif
		public BodyTrackingResult StartUpdatingBodyTracking(List<int> skeletonIds)
		{
			if (skeletonIds == null || skeletonIds.Count <= 0)
			{
				sb.Clear().Append("StartUpdatingBodyTracking() Invalid input.");
				ERROR(sb);
				return BodyTrackingResult.ERROR_INVALID_ARGUMENT;
			}

			if (s_SkeletonIds == null) { s_SkeletonIds = new List<int>(); }
			s_SkeletonIds.Clear();

			for (int i = 0; i < skeletonIds.Count; i++)
			{
				if (IsValidSkeletonId(skeletonIds[i]) == BodyTrackingResult.SUCCESS)
				{
					if (!s_SkeletonIds.Contains(skeletonIds[i])) { s_SkeletonIds.Add(skeletonIds[i]); }
					sb.Clear().Append("StartUpdatingBodyTracking() id: ").Append(skeletonIds[i]);
					DEBUG(sb);
				}
			}

			if (s_SkeletonIds.Count <= 0)
			{
				sb.Clear().Append("StartUpdatingBodyTracking() Invalid skeleton IDs.");
				ERROR(sb);
				return BodyTrackingResult.ERROR_IK_NOT_UPDATED;
			}

			return BodyTrackingResult.SUCCESS;
		}
#if WAVE_BODY_IK
		[Obsolete("This function is deprecated, please use GetAvatarIKData() instead.")]
#endif
		public BodyTrackingResult StartUpdatingBodyTracking(int skeletonId)
		{
			if (s_SkeletonIds == null) { s_SkeletonIds = new List<int>(); }

			BodyTrackingResult result = IsValidSkeletonId(skeletonId);
			if (result != BodyTrackingResult.SUCCESS)
			{
				sb.Clear().Append("StartUpdatingBodyTracking() invalid id ").Append(skeletonId);
				ERROR(sb);
				return result;
			}

			if (!s_SkeletonIds.Contains(skeletonId)) { s_SkeletonIds.Add(skeletonId); }

			sb.Clear().Append("StartUpdatingBodyTracking() id: ").Append(skeletonId);
			DEBUG(sb);
			return BodyTrackingResult.SUCCESS;
		}
#if WAVE_BODY_IK
		[Obsolete("This function is deprecated, please use GetAvatarIKData() instead.")]
#endif
		public BodyTrackingResult StopUpdatingBodyTracking(List<int> skeletonIds)
		{
			if (skeletonIds == null || skeletonIds.Count <= 0)
			{
				sb.Clear().Append("StopUpdatingBodyTracking() Invalid input.");
				ERROR(sb);
				return BodyTrackingResult.ERROR_INVALID_ARGUMENT;
			}

			if (s_SkeletonIds == null || s_SkeletonIds.Count <= 0)
			{
				sb.Clear().Append("StopUpdatingBodyTracking() No available IK.");
				ERROR(sb);
				return BodyTrackingResult.ERROR_IK_NOT_DESTROYED;
			}

			sb.Clear();
			for (int i = 0; i < skeletonIds.Count; i++)
			{
				if (s_SkeletonIds.Contains(skeletonIds[i])) { s_SkeletonIds.Remove(skeletonIds[i]); }
				sb.Append("StopUpdatingBodyTracking() id: ").Append(skeletonIds[i]).Append("\n");
			}
			DEBUG(sb);

			return BodyTrackingResult.SUCCESS;
		}
#if WAVE_BODY_IK
		[Obsolete("This function is deprecated, please use GetAvatarIKData() instead.")]
#endif
		public BodyTrackingResult StopUpdatingBodyTracking(int skeletonId)
		{
			if (s_SkeletonIds == null || s_SkeletonIds.Count <= 0)
			{
				sb.Clear().Append("StopUpdatingBodyTracking() No available IK.");
				ERROR(sb);
				return BodyTrackingResult.ERROR_IK_NOT_DESTROYED;
			}

			if (s_SkeletonIds.Contains(skeletonId)) { s_SkeletonIds.Remove(skeletonId); }

			sb.Clear().Append("StopUpdatingBodyTracking() id: ").Append(skeletonId);
			DEBUG(sb);
			return BodyTrackingResult.SUCCESS;
		}
	}
}
