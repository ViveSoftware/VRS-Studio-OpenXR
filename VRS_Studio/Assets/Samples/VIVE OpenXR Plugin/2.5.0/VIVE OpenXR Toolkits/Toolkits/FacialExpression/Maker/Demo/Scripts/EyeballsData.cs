using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.OpenXR;
using VIVE.OpenXR.FacialTracking;

namespace VIVE.OpenXR.Toolkits.FacialExpression.Maker.Demo
{
    public static class EyeballsData
    {
        public enum XrEyeShapeHTC
        {
            XR_EYE_SHAPE_NONE_HTC = -1,
            XR_EYE_EXPRESSION_LEFT_BLINK_HTC = 0,
            XR_EYE_EXPRESSION_LEFT_WIDE_HTC = 1,
            XR_EYE_EXPRESSION_RIGHT_BLINK_HTC = 2,
            XR_EYE_EXPRESSION_RIGHT_WIDE_HTC = 3,
            XR_EYE_EXPRESSION_LEFT_SQUEEZE_HTC = 4,
            XR_EYE_EXPRESSION_RIGHT_SQUEEZE_HTC = 5,
            XR_EYE_EXPRESSION_LEFT_DOWN_HTC = 6,
            XR_EYE_EXPRESSION_RIGHT_DOWN_HTC = 7,
            XR_EYE_EXPRESSION_LEFT_OUT_HTC = 8,
            XR_EYE_EXPRESSION_RIGHT_IN_HTC = 9,
            XR_EYE_EXPRESSION_LEFT_IN_HTC = 10,
            XR_EYE_EXPRESSION_RIGHT_OUT_HTC = 11,
            XR_EYE_EXPRESSION_LEFT_UP_HTC = 12,
            XR_EYE_EXPRESSION_RIGHT_UP_HTC = 13,
            XR_EYE_EXPRESSION_MAX_ENUM_HTC = 14,
        }
        public const int ANIPAL_TYPE_EYE_V2 = 2;
        public const int WeightingCount = (int)XrEyeShapeHTC.XR_EYE_EXPRESSION_MAX_ENUM_HTC;
        private static XrFacialExpressionsHTC EyeExpression_;
        private static int LastUpdateFrame = -1;
        private static Error LastUpdateResult = Error.FAILED;
        private static Dictionary<XrEyeShapeHTC, float> Weightings;
        private static float[] blendshapes = new float[60];
        static EyeballsData()
        {
            Weightings = new Dictionary<XrEyeShapeHTC, float>();
            for (int i = 0; i < WeightingCount; ++i) Weightings.Add((XrEyeShapeHTC)i, 0.0f);
        }
        private static bool UpdateData()
        {
            if (Time.frameCount == LastUpdateFrame) return LastUpdateResult == Error.WORK;
            else LastUpdateFrame = Time.frameCount;

            var feature = OpenXRSettings.Instance.GetFeature<ViveFacialTracking>();
            if (feature.GetFacialExpressions(XrFacialTrackingTypeHTC.XR_FACIAL_TRACKING_TYPE_EYE_DEFAULT_HTC, out blendshapes))
            {
                LastUpdateResult = Error.WORK;
            }
            else
            {
                LastUpdateResult = Error.FAILED;
            }
            return LastUpdateResult == Error.WORK;
        }
        public static bool GetEyeWeightings(out Dictionary<XrEyeShapeHTC, float> shapes, XrFacialExpressionsHTC expression)
        {
            for (int i = 0; i < WeightingCount; ++i)
            {
                if (i < blendshapes.Length)
                    Weightings[(XrEyeShapeHTC)(i)] = blendshapes[i];

            }
            shapes = Weightings;
            for (int i = 0; i < WeightingCount; ++i)
            {
                //Debug.Log("EyeballsUpdate GetEyeWeightings shapes:" + i + ", value: " + shapes[(XrEyeShapeHTC)(i)]);
            }
            return true;
        }


        /// <summary>
        /// Gets weighting values from anipal's Eye module.
        /// </summary>
        /// <param name="shapes">Weighting values obtained from anipal's Eye module.</param>
        /// <returns>Indicates whether the values received are new.</returns>\
        public static bool GetEyeWeightings(out Dictionary<XrEyeShapeHTC, float> shapes)
        {
            UpdateData();
            return GetEyeWeightings(out shapes, EyeExpression_);
        }


        public enum Error : int
        {
            RUNTIME_NOT_FOUND = -3,
            NOT_INITIAL = -2,
            FAILED = -1,
            WORK = 0,
            INVALID_INPUT = 1,
            FILE_NOT_FOUND = 2,
            DATA_NOT_FOUND = 13,
            UNDEFINED = 319,
            INITIAL_FAILED = 1001,
            NOT_IMPLEMENTED = 1003,
            NULL_POINTER = 1004,
            OVER_MAX_LENGTH = 1005,
            FILE_INVALID = 1006,
            UNINSTALL_STEAM = 1007,
            MEMCPY_FAIL = 1008,
            NOT_MATCH = 1009,
            NODE_NOT_EXIST = 1010,
            UNKONW_MODULE = 1011,
            MODULE_FULL = 1012,
            UNKNOW_TYPE = 1013,
            INVALID_MODULE = 1014,
            INVALID_TYPE = 1015,
            MEMORY_NOT_ENOUGH = 1016,
            BUSY = 1017,
            NOT_SUPPORTED = 1018,
            INVALID_VALUE = 1019,
            COMING_SOON = 1020,
            INVALID_CHANGE = 1021,
            TIMEOUT = 1022,
            DEVICE_NOT_FOUND = 1023,
            INVALID_DEVICE = 1024,
            NOT_AUTHORIZED = 1025,
            ALREADY = 1026,
            INTERNAL = 1027,
            CONNECTION_FAILED = 1028,
            ALLOCATION_FAILED = 1029,
            OPERATION_FAILED = 1030,
            NOT_AVAILABLE = 1031,
            CALLBACK_IN_PROGRESS = 1032,
            SERVICE_NOT_FOUND = 1033,
            DISABLED_BY_USER = 1034,
            EULA_NOT_ACCEPT = 1035,
            RUNTIME_NO_RESPONSE = 1036,
            OPENCL_NOT_SUPPORT = 1037,
            NOT_SUPPORT_EYE_TRACKING = 1038,
        };
    }
}
