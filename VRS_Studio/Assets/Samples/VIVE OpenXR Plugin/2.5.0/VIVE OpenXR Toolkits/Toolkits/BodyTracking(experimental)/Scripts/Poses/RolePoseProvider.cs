// Copyright HTC Corporation All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Text;

using VIVE.OpenXR.Toolkits.BodyTracking.RuntimeDependency;

namespace VIVE.OpenXR.Toolkits.BodyTracking
{
	public enum RolePoseType : UInt32
	{
		HMD = 100,

		CONTROLLER_LEFT = 200,
		CONTROLLER_RIGHT = 201,

		HAND_LEFT = 300,
		HAND_RIGHT = 301,

		TRACKER_0 = 400,
		TRACKER_1 = 401,
		TRACKER_2 = 402,
		TRACKER_3 = 403,
		TRACKER_4 = 404,
		TRACKER_5 = 405,
		TRACKER_6 = 406,
		TRACKER_7 = 407,
		TRACKER_8 = 408,
		TRACKER_9 = 409,
		TRACKER_10 = 410,
		TRACKER_11 = 411,
		TRACKER_12 = 412,
		TRACKER_13 = 413,
		TRACKER_14 = 414,
		TRACKER_15 = 415,

		UNKNOWN = 0x7FFFFFFF
	}
	public static class RolePoseProvider
	{
		const string LOG_TAG = "VIVE.OpenXR.Toolkits.BodyTracking.RolePoseProvider";
		private static StringBuilder m_sb = null;
		private static StringBuilder sb
		{
			get
			{
				if (m_sb == null) { m_sb = new StringBuilder(); }
				return m_sb;
			}
		}
		private static void DEBUG(StringBuilder msg) { Rdp.d(LOG_TAG, msg, true); }

		private static Dictionary<RolePoseType, RolePose> m_RolePoseMap = new Dictionary<RolePoseType, RolePose>();
		public static Dictionary<RolePoseType, RolePose> RolePoseMap
		{
			get
			{
				if (m_RolePoseMap == null) { m_RolePoseMap = new Dictionary<RolePoseType, RolePose>(); }
				return m_RolePoseMap;
			}
			private set { m_RolePoseMap = value; }
		}

		public static void RegisterRolePose(in RolePoseType type, in RolePose pose)
		{
			if (type == RolePoseType.UNKNOWN || pose == null) { return; }

			if (!RolePoseMap.ContainsKey(type))
				RolePoseMap.Add(type, pose);
			else
				RolePoseMap[type] = pose;

			sb.Clear().Append("RegisterRolePose() ").Append(type.Name()).Append(" from ").Append(RolePoseMap[type].gameObject.name);
			DEBUG(sb);
		}
		public static void RemoveRolePose(in RolePoseType type)
		{
			if (type == RolePoseType.UNKNOWN) { return; }

			if (RolePoseMap.ContainsKey(type))
			{
				RolePoseMap.Remove(type);

				sb.Clear().Append("RemoveRolePose() ").Append(type.Name());
				DEBUG(sb);
			}
		}
		public static RolePose GetRolePose(in RolePoseType type)
		{
			if (RolePoseMap.ContainsKey(type))
				return RolePoseMap[type];

			return null;
		}

		public static string Name(this RolePoseType type)
		{
			if (type == RolePoseType.HMD) { return "HMD"; }
			if (type == RolePoseType.CONTROLLER_LEFT) { return "CONTROLLER_LEFT"; }
			if (type == RolePoseType.CONTROLLER_RIGHT) { return "CONTROLLER_RIGHT"; }
			if (type == RolePoseType.HAND_LEFT) { return "HAND_LEFT"; }
			if (type == RolePoseType.HAND_RIGHT) { return "HAND_RIGHT"; }

			if (type == RolePoseType.TRACKER_0) { return "TRACKER_0"; }
			if (type == RolePoseType.TRACKER_1) { return "TRACKER_1"; }
			if (type == RolePoseType.TRACKER_2) { return "TRACKER_2"; }
			if (type == RolePoseType.TRACKER_3) { return "TRACKER_3"; }
			if (type == RolePoseType.TRACKER_4) { return "TRACKER_4"; }
			if (type == RolePoseType.TRACKER_5) { return "TRACKER_5"; }
			if (type == RolePoseType.TRACKER_6) { return "TRACKER_6"; }
			if (type == RolePoseType.TRACKER_7) { return "TRACKER_7"; }
			if (type == RolePoseType.TRACKER_8) { return "TRACKER_8"; }
			if (type == RolePoseType.TRACKER_9) { return "TRACKER_9"; }
			if (type == RolePoseType.TRACKER_10) { return "TRACKER_10"; }
			if (type == RolePoseType.TRACKER_11) { return "TRACKER_11"; }
			if (type == RolePoseType.TRACKER_12) { return "TRACKER_12"; }
			if (type == RolePoseType.TRACKER_13) { return "TRACKER_13"; }
			if (type == RolePoseType.TRACKER_14) { return "TRACKER_14"; }
			if (type == RolePoseType.TRACKER_15) { return "TRACKER_15"; }

			return "";
		}
	}
}
