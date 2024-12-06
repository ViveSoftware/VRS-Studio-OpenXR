// Copyright HTC Corporation All Rights Reserved.

using System.Collections;
using UnityEngine;
using VIVE.OpenXR.Toolkits.BodyTracking.RuntimeDependency;

namespace VIVE.OpenXR.Toolkits.BodyTracking
{
	public class RolePoseHead : RolePose
	{
		private void Awake()
		{
			m_PoseType = RolePoseType.HMD;
		}

		bool toUpdate = false;
		protected override void OnEnable()
		{
			base.OnEnable();

			if (!toUpdate)
			{
				toUpdate = true;
				StartCoroutine(UpdatePose());
			}
		}
		protected override void OnDisable()
		{
			base.OnDisable();

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

				m_LocationFlag = 0;
				m_VelocityFlag = 0;
				if (!Rdp.Head.IsTracked()) { continue; }
				if (Rdp.Head.GetRotation(ref m_Rotation)) { m_LocationFlag |= LocationFlag.ROTATION; }
				if (Rdp.Head.GetPosition(ref m_Position)) { m_LocationFlag |= LocationFlag.POSITION; }
				if (Rdp.Head.GetAngularVelocity(ref m_AngularVelocity)) { m_VelocityFlag |= VelocityFlag.ANGULAR; }
				if (Rdp.Head.GetVelocity(ref m_LinearVelocity)) { m_VelocityFlag |= VelocityFlag.LINEAR; }
				Rdp.Head.GetAcceleration(ref m_Acceleration);
			}
		}
	}
}
