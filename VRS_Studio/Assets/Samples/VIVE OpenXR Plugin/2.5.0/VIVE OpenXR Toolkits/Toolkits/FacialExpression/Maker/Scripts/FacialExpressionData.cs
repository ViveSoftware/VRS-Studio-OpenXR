// Copyright HTC Corporation All Rights Reserved.
using System;
using UnityEngine;
using UnityEngine.XR.OpenXR;
using VIVE.OpenXR.FacialTracking;

namespace VIVE.OpenXR.Toolkits.FacialExpression.Maker
{
    public class FacialExpressionData : MonoBehaviour
    {
        public enum ExpressionsEye : UInt32
        {
            LEFT_BLINK = 0,
            LEFT_WIDE = 1,
            RIGHT_BLINK = 2,
            RIGHT_WIDE = 3,
            LEFT_SQUEEZE = 4,
            RIGHT_SQUEEZE = 5,
            LEFT_DOWN = 6,
            RIGHT_DOWN = 7,
            LEFT_OUT = 8,
            RIGHT_IN = 9,
            LEFT_IN = 10,
            RIGHT_OUT = 11,
            LEFT_UP = 12,
            RIGHT_UP = 13,
            MAX,
        }
        public enum ExpressionsLip
        {
            Jaw_Right = 0,
            Jaw_Left = 1,
            Jaw_Forward = 2,
            Jaw_Open = 3,
            Mouth_Ape_Shape = 4,
            Mouth_Upper_Right = 5,
            Mouth_Upper_Left = 6,
            Mouth_Lower_Right = 7,
            Mouth_Lower_Left = 8,
            Mouth_Upper_Overturn = 9,
            Mouth_Lower_Overturn = 10,
            Mouth_Pout = 11,
            Mouth_Raiser_Right = 12,
            Mouth_Raiser_Left = 13,
            Mouth_Stretcher_Right = 14,
            Mouth_Stretcher_Left = 15,
            Cheek_Puff_Right = 16,
            Cheek_Puff_Left = 17,
            Cheek_Suck = 18,
            Mouth_Upper_UpRight = 19,
            Mouth_Upper_UpLeft = 20,
            Mouth_Lower_DownRight = 21,
            Mouth_Lower_DownLeft = 22,
            Mouth_Upper_Inside = 23,
            Mouth_Lower_Inside = 24,
            Mouth_Lower_Overlay = 25,
            Tongue_Longstep1 = 26,
            Tongue_Left = 27,
            Tongue_Right = 28,
            Tongue_Up = 29,
            Tongue_Down = 30,
            Tongue_Roll = 31,
            Tongue_Longstep2 = 32,
            Tongue_UpRight_Morph = 33,
            Tongue_UpLeft_Morph = 34,
            Tongue_DownRight_Morph = 35,
            Tongue_DownLeft_Morph = 36,
            Max,
        }
        private static FacialExpressionData instance = null;
        private static void CheckInstance()
        {
            //if (instance == null)
            //{
            //    var instanceObj = new GameObject("FacialTrackingData");
            //    instance = instanceObj.AddComponent<FacialExpressionData>();
            //    DontDestroyOnLoad(instance);
            //}
        }

        private void Awake()
        {
            instance = this;
        }

        private static float[] eyeExps = new float[(int)XrEyeExpressionHTC.XR_EYE_EXPRESSION_MAX_ENUM_HTC];
        private static float[] lipExps = new float[(int)XrLipExpressionHTC.XR_LIP_EXPRESSION_MAX_ENUM_HTC];
        void Update()
        {
            var feature = OpenXRSettings.Instance.GetFeature<ViveFacialTracking>();
            if (feature != null)
            {
                // Eye expressions
                {
                    if (feature.GetFacialExpressions(XrFacialTrackingTypeHTC.XR_FACIAL_TRACKING_TYPE_EYE_DEFAULT_HTC, out float[] exps))
                    {
                        eyeExps = exps;
                    }
                }
                // Lip expressions
                {
                    if (feature.GetFacialExpressions(XrFacialTrackingTypeHTC.XR_FACIAL_TRACKING_TYPE_LIP_DEFAULT_HTC, out float[] exps))
                    {
                        lipExps = exps;
                    }
                }
            }
        }

        public static float EyeExpression(XrEyeExpressionHTC exp)
        {
            //CheckInstance();
            if ((int)exp < eyeExps.Length) { return eyeExps[(int)exp]; }
            return 0;
        }
        public static float LipExpression(XrLipExpressionHTC exp)
        {
            //CheckInstance();
            if ((int)exp < lipExps.Length) { return lipExps[(int)exp]; }
            return 0;
        }
    }
}
