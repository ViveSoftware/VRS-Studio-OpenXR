// Copyright HTC Corporation All Rights Reserved.

using System.Collections;
using UnityEngine;
using VIVE.OpenXR.Toolkits.BodyTracking.RuntimeDependency;

namespace VIVE.OpenXR.Toolkits.BodyTracking
{
	public class RolePoseController : RolePose
	{
		public bool isLeft = false;

		private void Awake()
		{
			m_PoseType = isLeft ? RolePoseType.CONTROLLER_LEFT : RolePoseType.CONTROLLER_RIGHT;
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
				if (!Rdp.Controller.IsTracked(isLeft)) { continue; }
				if (Rdp.Controller.GetRotation(isLeft, ref m_Rotation)) { m_LocationFlag |= LocationFlag.ROTATION; }
				if (Rdp.Controller.GetPosition(isLeft, ref m_Position)) { m_LocationFlag |= LocationFlag.POSITION; }
				if (Rdp.Controller.GetAngularVelocity(isLeft, ref m_AngularVelocity)) { m_VelocityFlag |= VelocityFlag.ANGULAR; }
				if (Rdp.Controller.GetVelocity(isLeft, ref m_LinearVelocity)) { m_VelocityFlag |= VelocityFlag.LINEAR; }
				Rdp.Controller.GetAcceleration(isLeft, ref m_Acceleration);
			}
		}
	}
}
