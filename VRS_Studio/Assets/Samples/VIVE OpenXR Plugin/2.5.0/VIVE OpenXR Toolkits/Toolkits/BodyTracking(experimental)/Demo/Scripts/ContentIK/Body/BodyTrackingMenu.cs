// Copyright HTC Corporation All Rights Reserved.

using UnityEngine;
using UnityEngine.UI;

namespace VIVE.OpenXR.Toolkits.BodyTracking.Demo
{
	public class BodyTrackingMenu : MonoBehaviour
	{
		public BodyTrackingSample ikScript = null;
		public Button beginTrackingButton = null;
		public Button startCalibrationButton = null;
		public Text trackingTitle = null;
		public Text calibrationTitle = null;
		public Button logButton = null;

		private void Update()
		{
			if (ikScript != null && trackingTitle != null)
			{
				string autoUpdateText = "Manually Tracking";
				trackingTitle.text = ikScript.TrackingMode.Name() + "\n" + autoUpdateText;
			}

			if (logButton != null && BodyManager.Instance != null)
			{
				logButton.GetComponentInChildren<Text>().text = BodyManager.Instance.EnableTrackingLog ? "Log\nOn" : "Log\nOff";
			}
		}

		public void SetArmMode()
		{
			if (ikScript != null)
				ikScript.SetArmMode();
		}
		public void SetUpperMode()
		{
			if (ikScript != null)
				ikScript.SetUpperMode();
		}
		public void SetFullMode()
		{
			if (ikScript != null)
				ikScript.SetFullMode();
		}
		public void SetUpperBodyAndLegMode()
		{
			if (ikScript != null)
				ikScript.SetUpperBodyAndLegMode();
		}

		public void BeginTracking()
		{
			if (ikScript != null)
			{
				if (beginTrackingButton != null) { beginTrackingButton.interactable = false; }
				ikScript.BeginTracking();
			}
		}
		public void EndTracking()
		{
			if (ikScript != null)
			{
				if (beginTrackingButton != null) { beginTrackingButton.interactable = true; }
				ikScript.StopTracking();
			}
		}

		private void CalibrationStatusCallback(object sender, CalibrationStatus status)
		{
			if (startCalibrationButton != null) { startCalibrationButton.interactable = (status >= CalibrationStatus.STATUS_FINISHED); }
			if (calibrationTitle != null) { calibrationTitle.text = "Calibration " + status.Name(); }
		}
		public void StartCalibration()
		{
			if (ikScript != null)
			{
				if (startCalibrationButton != null) { startCalibrationButton.interactable = false; }
				if (calibrationTitle != null) { calibrationTitle.text = "Calibration"; }
				ikScript.BeginCalibration(CalibrationStatusCallback);
			}
		}
		public void StopCalibration()
		{
			if (ikScript != null)
			{
				if (startCalibrationButton != null) { startCalibrationButton.interactable = true; }
				if (calibrationTitle != null) { calibrationTitle.text = "Calibration"; }
				ikScript.StopCalibration();
			}
		}

		public void ActivateTrackingLog()
		{
			if (BodyManager.Instance == null) { return; }

			BodyManager.Instance.EnableTrackingLog = !BodyManager.Instance.EnableTrackingLog;
		}
	}
}
