// Copyright HTC Corporation All Rights Reserved.

using System;
using UnityEngine;

namespace VIVE.OpenXR.Toolkits.BodyTracking.AvatarCoordinate
{
	public enum AxisDefine
	{
		Right,
		Up,
		Forward,
		Left,
		Down,
		Backward
	}

	[Serializable]
	public struct CoordinateDefine
	{
		public AxisDefine x;
		public AxisDefine y;
		public AxisDefine z;

		public CoordinateDefine(AxisDefine in_x, AxisDefine in_y, AxisDefine in_z)
		{
			x = in_x;
			y = in_y;
			z = in_z;
		}
		public static CoordinateDefine OpenGL
		{
			get
			{
				return new CoordinateDefine(AxisDefine.Right, AxisDefine.Up, AxisDefine.Backward);
			}
		}
		public static CoordinateDefine OpenGLAngular
		{
			get
			{
				return new CoordinateDefine(AxisDefine.Left, AxisDefine.Down, AxisDefine.Forward);
			}
		}
		public static CoordinateDefine Unity
		{
			get
			{
				return new CoordinateDefine(AxisDefine.Right, AxisDefine.Up, AxisDefine.Forward);
			}
		}

		public static bool operator ==(CoordinateDefine left, CoordinateDefine right)
		{
			return (left.x == right.x && left.y == right.y && left.z == right.z);
		}
		public static bool operator !=(CoordinateDefine left, CoordinateDefine right)
		{
			return !(left == right);
		}
		public override bool Equals(object obj)
		{
			if (obj is CoordinateDefine)
			{
				return this == (CoordinateDefine)obj;
			}

			return false;
		}
		public override int GetHashCode()
		{
			return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
		}

		public void Update(CoordinateDefine coor)
		{
			x = coor.x;
			y = coor.y;
			z = coor.z;
		}
		public void Update(AxisDefine in_x, AxisDefine in_y, AxisDefine in_z)
		{
			x = in_x;
			y = in_y;
			z = in_z;
		}

		public void Reset()
		{
			x = AxisDefine.Right;
			y = AxisDefine.Up;
			z = AxisDefine.Forward;
		}
		public void ValidateOrthogonal()
		{
			if (x == AxisDefine.Left || x == AxisDefine.Right)
			{
				if ((y == AxisDefine.Up || y == AxisDefine.Down) && (z == AxisDefine.Forward || z == AxisDefine.Backward))
					return;
				if ((z == AxisDefine.Up || z == AxisDefine.Down) && (y == AxisDefine.Forward || y == AxisDefine.Backward))
					return;
			}
			if (x == AxisDefine.Up || x == AxisDefine.Down)
			{
				if ((y == AxisDefine.Forward || y == AxisDefine.Backward) && (z == AxisDefine.Left || z == AxisDefine.Right))
					return;
				if ((z == AxisDefine.Forward || z == AxisDefine.Backward) && (y == AxisDefine.Left || y == AxisDefine.Right))
					return;
			}
			if (x == AxisDefine.Forward || x == AxisDefine.Backward)
			{
				if ((y == AxisDefine.Left || y == AxisDefine.Right) && (z == AxisDefine.Up || z == AxisDefine.Down))
					return;
				if ((z == AxisDefine.Left || z == AxisDefine.Right) && (y == AxisDefine.Up || y == AxisDefine.Down))
					return;
			}

			Reset();
		}
	}

	[CreateAssetMenu(fileName = "AvatarJointCoordinate",
					 menuName = "Wave/Body Tracking/Avatar Joint Coordinate", order = 200)]
	public class AvatarCoordinateProducer : ScriptableObject
	{
		public CoordinateDefine hip = CoordinateDefine.Unity;

		public CoordinateDefine leftThigh = CoordinateDefine.Unity;
		public CoordinateDefine leftLeg = CoordinateDefine.Unity;
		public CoordinateDefine leftAnkle = CoordinateDefine.Unity;
		public CoordinateDefine leftFoot = CoordinateDefine.Unity;

		public CoordinateDefine rightThigh = CoordinateDefine.Unity;
		public CoordinateDefine rightLeg = CoordinateDefine.Unity;
		public CoordinateDefine rightAnkle = CoordinateDefine.Unity;
		public CoordinateDefine rightFoot = CoordinateDefine.Unity;

		public CoordinateDefine waist = CoordinateDefine.Unity;

		public CoordinateDefine spineLower = CoordinateDefine.Unity;
		public CoordinateDefine spineMiddle = CoordinateDefine.Unity;
		public CoordinateDefine spineHigh = CoordinateDefine.Unity;

		public CoordinateDefine chest = CoordinateDefine.Unity;
		public CoordinateDefine neck = CoordinateDefine.Unity;
		public CoordinateDefine head = CoordinateDefine.Unity;

		public CoordinateDefine leftClavicle = CoordinateDefine.Unity;
		public CoordinateDefine leftScapula = CoordinateDefine.Unity;
		public CoordinateDefine leftUpperarm = CoordinateDefine.Unity;
		public CoordinateDefine leftForearm = CoordinateDefine.Unity;
		public CoordinateDefine leftHand = CoordinateDefine.Unity;

		public CoordinateDefine rightClavicle = CoordinateDefine.Unity;
		public CoordinateDefine rightScapula = CoordinateDefine.Unity;
		public CoordinateDefine rightUpperarm = CoordinateDefine.Unity;
		public CoordinateDefine rightForearm = CoordinateDefine.Unity;
		public CoordinateDefine rightHand = CoordinateDefine.Unity;

		private void Awake()
		{
			hip.ValidateOrthogonal();

			leftThigh.ValidateOrthogonal();
			leftLeg.ValidateOrthogonal();
			leftAnkle.ValidateOrthogonal();
			leftFoot.ValidateOrthogonal();

			rightThigh.ValidateOrthogonal();
			rightLeg.ValidateOrthogonal();
			rightAnkle.ValidateOrthogonal();
			rightFoot.ValidateOrthogonal();

			waist.ValidateOrthogonal();

			spineLower.ValidateOrthogonal();
			spineMiddle.ValidateOrthogonal();
			spineHigh.ValidateOrthogonal();

			chest.ValidateOrthogonal();
			neck.ValidateOrthogonal();
			head.ValidateOrthogonal();

			leftClavicle.ValidateOrthogonal();
			leftScapula.ValidateOrthogonal();
			leftUpperarm.ValidateOrthogonal();
			leftForearm.ValidateOrthogonal();
			leftHand.ValidateOrthogonal();

			rightClavicle.ValidateOrthogonal();
			rightScapula.ValidateOrthogonal();
			rightUpperarm.ValidateOrthogonal();
			rightForearm.ValidateOrthogonal();
			rightHand.ValidateOrthogonal();
		}
	}
}
