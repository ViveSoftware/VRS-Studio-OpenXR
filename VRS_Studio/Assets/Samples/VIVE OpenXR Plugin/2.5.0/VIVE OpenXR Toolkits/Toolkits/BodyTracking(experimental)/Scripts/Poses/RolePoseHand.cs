// Copyright HTC Corporation All Rights Reserved.

using System.Collections;
using UnityEngine;
using VIVE.OpenXR.Toolkits.BodyTracking.RuntimeDependency;
using VIVE.OpenXR.Toolkits.Common;

namespace VIVE.OpenXR.Toolkits.BodyTracking
{
	public class RolePoseHand : RolePose
	{
		public bool isLeft = false;

		private void Awake()
		{
			m_PoseType = isLeft ? RolePoseType.HAND_LEFT : RolePoseType.HAND_RIGHT;
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
				if (!Rdp.Hand.IsTracked(isLeft)) { continue; }
				if (Rdp.Hand.GetJointRotation(HandJointType.Palm, ref m_Rotation, isLeft)) { m_LocationFlag |= LocationFlag.ROTATION; }
				if (Rdp.Hand.GetJointPosition(HandJointType.Palm, ref m_Position, isLeft)) { m_LocationFlag |= LocationFlag.POSITION; }
				if (Rdp.Hand.GetWristAngularVelocity(ref m_AngularVelocity, isLeft)) { m_VelocityFlag |= VelocityFlag.ANGULAR; }
				if (Rdp.Hand.GetWristLinearVelocity(ref m_LinearVelocity, isLeft)) { m_VelocityFlag |= VelocityFlag.LINEAR; }
			}
		}
	}
}
