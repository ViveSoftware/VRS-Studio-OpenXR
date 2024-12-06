// Copyright HTC Corporation All Rights Reserved.

using System;
using UnityEngine;

namespace VIVE.OpenXR.Toolkits.BodyTracking
{
	public abstract class RolePose : MonoBehaviour
	{
		[Flags]
		public enum LocationFlag { ROTATION = 1, POSITION = 2 }
		[Flags]
		public enum VelocityFlag { ANGULAR = 1, LINEAR = 2 }

		protected RolePoseType m_PoseType = RolePoseType.UNKNOWN;

		protected LocationFlag m_LocationFlag = 0;
		protected VelocityFlag m_VelocityFlag = 0;
		protected Quaternion m_Rotation = Quaternion.identity;
		protected Vector3 m_Position = Vector3.zero;
		protected Vector3 m_AngularVelocity = Vector3.zero;
		protected Vector3 m_LinearVelocity = Vector3.zero;
		protected Vector3 m_Acceleration = Vector3.zero;

		protected virtual void OnEnable()
		{
			RolePoseProvider.RegisterRolePose(m_PoseType, this);
		}
		protected virtual void OnDisable()
		{
			RolePoseProvider.RemoveRolePose(m_PoseType);
		}

		public virtual bool IsTracked()
		{
			return (m_LocationFlag != 0);
		}
		public virtual bool GetRotation(out Quaternion value)
		{
			value = m_Rotation;
			return m_LocationFlag.HasFlag(LocationFlag.ROTATION);
		}
		public virtual bool GetPosition(out Vector3 value)
		{
			value = m_Position;
			return m_LocationFlag.HasFlag(LocationFlag.POSITION);
		}
		public virtual bool GetAngularVelocity(out Vector3 value)
		{
			value = m_AngularVelocity;
			return m_VelocityFlag.HasFlag(VelocityFlag.ANGULAR);
		}
		public virtual bool GetLinearVelocity(out Vector3 value)
		{
			value = m_LinearVelocity;
			return m_VelocityFlag.HasFlag(VelocityFlag.LINEAR);
		}
		public virtual bool GetAcceleration(out Vector3 value)
		{
			value = m_Acceleration;
			return m_LocationFlag.HasFlag(LocationFlag.POSITION);
		}
	}
}
