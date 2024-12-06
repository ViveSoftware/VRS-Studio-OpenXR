// Copyright HTC Corporation All Rights Reserved.

using System.Collections;
using UnityEngine;
using VIVE.OpenXR.Toolkits.BodyTracking.RuntimeDependency;

namespace VIVE.OpenXR.Toolkits.BodyTracking.Demo
{
	public class TrackerUpdater : MonoBehaviour
	{
		[SerializeField]
		private Rdp.Tracker.Id m_TrackIndex = 0;
		private bool lastTracking = false;
		private bool toUpdate = false;

		private void OnEnable()
		{
			EnableChildren(false);
			if (!toUpdate)
			{
				toUpdate = true;
				StartCoroutine(UpdatePose());
			}
		}

		private void OnDisable()
		{
			if (toUpdate)
			{
				toUpdate = false;
				StopCoroutine(UpdatePose());
			}
		}

		private IEnumerator UpdatePose()
		{

			while (toUpdate)
			{
				yield return new WaitForEndOfFrame();

				CheckTrackingState();
				if (!lastTracking) yield return null;

				UpdateTransform();
			}
		}

		private void CheckTrackingState()
		{
			bool isTracked = Rdp.Tracker.IsTracked(m_TrackIndex);
			if (isTracked != lastTracking)
			{
				EnableChildren(isTracked);
				lastTracking = isTracked;
			}
		}

		private void UpdateTransform()
		{
			if (Rdp.Tracker.GetTrackerPosition(m_TrackIndex, out Vector3 position) &&
				Rdp.Tracker.GetTrackerRotation(m_TrackIndex, out Quaternion rotation))
			{
				transform.localPosition = position;
				transform.localRotation = rotation;
			}
		}

		private void EnableChildren(bool enable)
		{
			Transform parentTransform = transform;

			for (int i = 0; i < parentTransform.childCount; i++)
			{
				GameObject child = parentTransform.GetChild(i).gameObject;
				child.SetActive(enable);
			}
		}
	}
}