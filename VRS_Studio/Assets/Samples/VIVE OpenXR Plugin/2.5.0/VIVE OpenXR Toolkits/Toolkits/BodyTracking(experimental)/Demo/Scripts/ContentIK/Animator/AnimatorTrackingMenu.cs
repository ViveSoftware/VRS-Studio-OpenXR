// Copyright HTC Corporation All Rights Reserved.

using System.Text;
using UnityEngine;
using UnityEngine.UI;

using VIVE.OpenXR.Toolkits.BodyTracking.RuntimeDependency;

namespace VIVE.OpenXR.Toolkits.BodyTracking.Demo
{
	public class AnimatorTrackingMenu : MonoBehaviour
	{
		const string LOG_TAG = "VIVE.OpenXR.Toolkits.BodyTracking.Demo.AnimatorTrackingMenu";
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

		public AnimatorTracking animatorTracking = null;
		public Button beginTrackingButton = null;
		public Button startCalibrationButton = null;
		public Text trackingTitle = null;
		public Text calibrationTitle = null;
		public Button logButton = null;

		private void Update()
		{
			if (animatorTracking == null || trackingTitle == null) { return; }

			trackingTitle.text = animatorTracking.Tracking + "\n" + "Manually Tracking";

			if (logButton != null && BodyManager.Instance != null)
			{
				logButton.GetComponentInChildren<Text>().text = BodyManager.Instance.EnableTrackingLog ? "Log\nOn" : "Log\nOff";
			}
		}

		public void SetArmMode()
		{
			if (animatorTracking != null)
				animatorTracking.Tracking = AnimatorTracking.TrackingMode.Arm;
		}
		public void SetUpperMode()
		{
			if (animatorTracking != null)
				animatorTracking.Tracking = AnimatorTracking.TrackingMode.UpperBody;
		}
		public void SetFullMode()
		{
			if (animatorTracking != null)
				animatorTracking.Tracking = AnimatorTracking.TrackingMode.FullBody;
		}
		public void SetUpperBodyAndLegMode()
		{
			if (animatorTracking != null)
				animatorTracking.Tracking = AnimatorTracking.TrackingMode.UpperBodyAndLeg;
		}

		public void BeginTracking()
		{
			if (animatorTracking != null)
			{
				if (beginTrackingButton != null) { beginTrackingButton.interactable = false; }
				if (startCalibrationButton != null) { startCalibrationButton.interactable = false; }
				animatorTracking.BeginTracking();
			}
		}
		public void EndTracking()
		{
			if (animatorTracking != null)
			{
				if (beginTrackingButton != null) { beginTrackingButton.interactable = true; }
				if (startCalibrationButton != null) { startCalibrationButton.interactable = true; }
				animatorTracking.StopTracking();
			}
		}

		private void CalibrationStatusCallback(object sender, CalibrationStatus status)
		{
			if (startCalibrationButton != null) { startCalibrationButton.interactable = (status >= CalibrationStatus.STATUS_FINISHED); }
			if (calibrationTitle != null) { calibrationTitle.text = "Calibration " + status.Name(); }
		}
		public void StartCalibration()
		{
			if (animatorTracking != null)
			{
				if (startCalibrationButton != null) { startCalibrationButton.interactable = false; }
				if (calibrationTitle != null) { calibrationTitle.text = "Calibration"; }
#if !WAVE_BODY_IK
				animatorTracking.BeginCalibration(CalibrationStatusCallback);
#endif
			}
		}
		public void StopCalibration()
		{
			if (animatorTracking != null)
			{
				if (startCalibrationButton != null) { startCalibrationButton.interactable = true; }
				if (calibrationTitle != null) { calibrationTitle.text = "Calibration"; }
#if !WAVE_BODY_IK
				animatorTracking.StopCalibration();
#endif
			}
		}

		public void OneStepStart()
		{
			StartCalibration();
			BeginTracking();
		}
		public void OneStepStop()
		{
			EndTracking();
		}

		public void ActivateTrackingLog()
		{
			if (BodyManager.Instance == null) { return; }

			BodyManager.Instance.EnableTrackingLog = !BodyManager.Instance.EnableTrackingLog;
		}
	}
}
