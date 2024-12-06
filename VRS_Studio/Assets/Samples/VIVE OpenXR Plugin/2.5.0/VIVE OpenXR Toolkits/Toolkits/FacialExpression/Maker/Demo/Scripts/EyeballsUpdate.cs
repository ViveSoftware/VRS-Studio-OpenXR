using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using VIVE.OpenXR.FacialTracking;
//using UnityEngine.XR.OpenXR.Input;
//using UnityEngine.InputSystem.XR;

namespace VIVE.OpenXR.Toolkits.FacialExpression.Maker.Demo
{
    public class EyeballsUpdate : MonoBehaviour
    {
        public enum EyeShape_v2
        {
            None = -1,
            Eye_Left_Blink = 0,
            Eye_Left_Wide,
            Eye_Left_Right,
            Eye_Left_Left,
            Eye_Left_Up,
            Eye_Left_Down,
            Eye_Right_Blink = 6,
            Eye_Right_Wide,
            Eye_Right_Right,
            Eye_Right_Left,
            Eye_Right_Up,
            Eye_Right_Down,
            Eye_Frown = 12,
            Eye_Left_Squeeze,
            Eye_Right_Squeeze,
            Max = 15,
        }
        [Serializable]
        public class EyeShapeTable_v2
        {
            public SkinnedMeshRenderer skinnedMeshRenderer;
            public EyeShape_v2[] eyeShapes;
        }
        //public Transform eyeBallL, eyeBallR;
        public float EyeballsRotOffset = 1.1f;
        [SerializeField]
        private float m_AvatarHeight = 1.5f;
        public float AvatarHeight
        {
            get { return m_AvatarHeight; }
            set
            {
                if (value > 0) { m_AvatarHeight = value; }
            }
        }
        private const int NUM_OF_EYES = 2;
        [SerializeField] private Transform[] EyesModels = new Transform[NUM_OF_EYES];
        public InputDevice eyeTrackingDev, hmd;
        //public Quaternion defRotL, defRotR;
        [SerializeField] private List<EyeShapeTable_v2> EyeShapeTables;
        /// <summary>
        /// Customize this curve to fit the blend shapes of your avatar.
        /// </summary>
        //[SerializeField] private AnimationCurve EyebrowAnimationCurveUpper;
        /// <summary>
        /// Customize this curve to fit the blend shapes of your avatar.
        /// </summary>
        //[SerializeField] private AnimationCurve EyebrowAnimationCurveLower;
        /// <summary>
        /// Customize this curve to fit the blend shapes of your avatar.
        /// </summary>
        //[SerializeField] private AnimationCurve EyebrowAnimationCurveHorizontal;

        public bool NeededToGetData = true;
        private Dictionary<EyeballsData.XrEyeShapeHTC, float> EyeWeightings = new Dictionary<EyeballsData.XrEyeShapeHTC, float>();
        //Map Openxr eye shape to Avatar eye blendshape
        private static Dictionary<EyeShape_v2, EyeballsData.XrEyeShapeHTC> ShapeMap;
        private AnimationCurve[] EyebrowAnimationCurves = new AnimationCurve[(int)EyeShape_v2.Max];
        public const int WeightingCount = (int)EyeballsData.XrEyeShapeHTC.XR_EYE_EXPRESSION_MAX_ENUM_HTC;

        private GameObject[] EyeAnchors;
        private static float[] eyeExps = new float[(int)XrEyeExpressionHTC.XR_EYE_EXPRESSION_MAX_ENUM_HTC];

        Vector3 GazeDirectionCombinedLocal = Vector3.zero;
        Vector3 target = Vector3.zero;
        static EyeballsUpdate()
        {
            ShapeMap = new Dictionary<EyeShape_v2, EyeballsData.XrEyeShapeHTC>();
            ShapeMap.Add(EyeShape_v2.Eye_Left_Blink, EyeballsData.XrEyeShapeHTC.XR_EYE_EXPRESSION_LEFT_BLINK_HTC);
            ShapeMap.Add(EyeShape_v2.Eye_Left_Wide, EyeballsData.XrEyeShapeHTC.XR_EYE_EXPRESSION_LEFT_WIDE_HTC);
            ShapeMap.Add(EyeShape_v2.Eye_Right_Blink, EyeballsData.XrEyeShapeHTC.XR_EYE_EXPRESSION_RIGHT_BLINK_HTC);
            ShapeMap.Add(EyeShape_v2.Eye_Right_Wide, EyeballsData.XrEyeShapeHTC.XR_EYE_EXPRESSION_RIGHT_WIDE_HTC);
            ShapeMap.Add(EyeShape_v2.Eye_Left_Squeeze, EyeballsData.XrEyeShapeHTC.XR_EYE_EXPRESSION_LEFT_SQUEEZE_HTC);
            ShapeMap.Add(EyeShape_v2.Eye_Right_Squeeze, EyeballsData.XrEyeShapeHTC.XR_EYE_EXPRESSION_RIGHT_SQUEEZE_HTC);
            ShapeMap.Add(EyeShape_v2.Eye_Left_Down, EyeballsData.XrEyeShapeHTC.XR_EYE_EXPRESSION_LEFT_DOWN_HTC);
            ShapeMap.Add(EyeShape_v2.Eye_Right_Down, EyeballsData.XrEyeShapeHTC.XR_EYE_EXPRESSION_RIGHT_DOWN_HTC);
            ShapeMap.Add(EyeShape_v2.Eye_Left_Left, EyeballsData.XrEyeShapeHTC.XR_EYE_EXPRESSION_LEFT_OUT_HTC);
            ShapeMap.Add(EyeShape_v2.Eye_Right_Left, EyeballsData.XrEyeShapeHTC.XR_EYE_EXPRESSION_RIGHT_IN_HTC);
            ShapeMap.Add(EyeShape_v2.Eye_Left_Right, EyeballsData.XrEyeShapeHTC.XR_EYE_EXPRESSION_LEFT_IN_HTC);
            ShapeMap.Add(EyeShape_v2.Eye_Left_Up, EyeballsData.XrEyeShapeHTC.XR_EYE_EXPRESSION_LEFT_UP_HTC);
            ShapeMap.Add(EyeShape_v2.Eye_Right_Up, EyeballsData.XrEyeShapeHTC.XR_EYE_EXPRESSION_RIGHT_UP_HTC);
        }

        //IEnumerator Start()
        private void Start()
        {
            if (!EyeballsFramework.Instance.EnableEye)
            {
                enabled = false;
                return;
                //yield break;
                //yield return null;
            }
            //Debug.Log("Start EyeballsUpdate Start() ");
            SetEyesModels(EyesModels[0], EyesModels[1]);
            SetEyeShapeTables(EyeShapeTables);

            //Debug.Log("End EyeballsUpdate Start()");


        }
        private void Update()
        {

            if (EyeballsFramework.Status != EyeballsFramework.FrameworkStatus.WORKING &&
                EyeballsFramework.Status != EyeballsFramework.FrameworkStatus.NOT_SUPPORT) {
                //Debug.Log("EyeballsUpdate not ready");
                return;
            }


            if (NeededToGetData)
            {
                EyeballsData.GetEyeWeightings(out EyeWeightings); //weight related
                //UpdateEyeShapes(EyeWeightings); //weight related
                //Debug.Log("EyeballsUpdate ready");


                if (EyeWeightings != null)
                {
                    //Debug.Log("EyeballsUpdate EyeWeightings is null");

                    if (EyeWeightings[EyeballsData.XrEyeShapeHTC.XR_EYE_EXPRESSION_LEFT_IN_HTC] > EyeWeightings[EyeballsData.XrEyeShapeHTC.XR_EYE_EXPRESSION_LEFT_OUT_HTC])
                    {
                        GazeDirectionCombinedLocal.x = EyeWeightings[EyeballsData.XrEyeShapeHTC.XR_EYE_EXPRESSION_LEFT_IN_HTC]* EyeballsRotOffset;
                    }
                    else
                    {
                        GazeDirectionCombinedLocal.x = -EyeWeightings[EyeballsData.XrEyeShapeHTC.XR_EYE_EXPRESSION_LEFT_OUT_HTC]* EyeballsRotOffset;
                    }
                    if (EyeWeightings[EyeballsData.XrEyeShapeHTC.XR_EYE_EXPRESSION_LEFT_UP_HTC] > EyeWeightings[EyeballsData.XrEyeShapeHTC.XR_EYE_EXPRESSION_LEFT_DOWN_HTC])
                    {
                        GazeDirectionCombinedLocal.y = EyeWeightings[EyeballsData.XrEyeShapeHTC.XR_EYE_EXPRESSION_LEFT_UP_HTC] * EyeballsRotOffset;
                    }
                    else
                    {
                        GazeDirectionCombinedLocal.y = -EyeWeightings[EyeballsData.XrEyeShapeHTC.XR_EYE_EXPRESSION_LEFT_DOWN_HTC]* EyeballsRotOffset;
                    }
                }
                GazeDirectionCombinedLocal.z = (float)1.0f;
                //Debug.Log("EyeballsUpdate ready eye EyeAnchors[0]:" + EyeAnchors[0].transform.position.x + ", " + EyeAnchors[0].transform.position.y);
                //Debug.Log("EyeballsUpdate ready eye 0: " + GazeDirectionCombinedLocal.x + ", " + GazeDirectionCombinedLocal.y);
                //target = EyeAnchors[0].transform.TransformPoint(GazeDirectionCombinedLocal);
                //EyesModels[0].LookAt(target);
                target = EyeAnchors[0].transform.InverseTransformPoint(EyeAnchors[0].transform.TransformPoint(GazeDirectionCombinedLocal));
                EyeAnchors[0].transform.localRotation = Quaternion.LookRotation(target);
                //eyeBallL.LookAt(target);

                if (EyeWeightings != null)
                {
                    if (EyeWeightings[EyeballsData.XrEyeShapeHTC.XR_EYE_EXPRESSION_RIGHT_IN_HTC] > EyeWeightings[EyeballsData.XrEyeShapeHTC.XR_EYE_EXPRESSION_RIGHT_OUT_HTC])
                    {
                        GazeDirectionCombinedLocal.x = -EyeWeightings[EyeballsData.XrEyeShapeHTC.XR_EYE_EXPRESSION_RIGHT_IN_HTC] * EyeballsRotOffset;
                    }
                    else
                    {
                        GazeDirectionCombinedLocal.x = EyeWeightings[EyeballsData.XrEyeShapeHTC.XR_EYE_EXPRESSION_RIGHT_OUT_HTC] * EyeballsRotOffset;
                    }
                    if (EyeWeightings[EyeballsData.XrEyeShapeHTC.XR_EYE_EXPRESSION_RIGHT_UP_HTC] > EyeWeightings[EyeballsData.XrEyeShapeHTC.XR_EYE_EXPRESSION_RIGHT_DOWN_HTC])
                    {
                        GazeDirectionCombinedLocal.y = EyeWeightings[EyeballsData.XrEyeShapeHTC.XR_EYE_EXPRESSION_RIGHT_UP_HTC] * EyeballsRotOffset;
                    }
                    else
                    {
                        GazeDirectionCombinedLocal.y = -EyeWeightings[EyeballsData.XrEyeShapeHTC.XR_EYE_EXPRESSION_RIGHT_DOWN_HTC] * EyeballsRotOffset;
                    }
                }
                GazeDirectionCombinedLocal.z = (float)1.0f;
                //Debug.Log("EyeballsUpdate ready eye EyeAnchors[1]:" + EyeAnchors[1].transform.position.x + ", " + EyeAnchors[1].transform.position.y);
                //Debug.Log("EyeballsUpdate ready eye 1: " + GazeDirectionCombinedLocal.x + ", " + GazeDirectionCombinedLocal.y);
                //target = EyeAnchors[1].transform.TransformPoint(GazeDirectionCombinedLocal);
                //EyesModels[1].LookAt(target);
                target = EyeAnchors[1].transform.InverseTransformPoint(EyeAnchors[1].transform.TransformPoint(GazeDirectionCombinedLocal));
                EyeAnchors[1].transform.localRotation = Quaternion.LookRotation(target);
                //eyeBallR.LookAt(target);

            }
        }
        private void Release()
        {
        }
        private void OnDestroy()
        {
            DestroyEyeAnchors();
        }
        public void SetEyesModels(Transform leftEye, Transform rightEye)
        {
            if (leftEye != null && rightEye != null)
            {
                EyesModels = new Transform[NUM_OF_EYES] { leftEye, rightEye };
                DestroyEyeAnchors();
                CreateEyeAnchors();
            }
        }
        private void CreateEyeAnchors()
        {
            Debug.Log("CreateEyeAnchors() :"+gameObject.name+", height:"+gameObject.transform.InverseTransformPoint(gameObject.transform.localPosition));

            EyeAnchors = new GameObject[NUM_OF_EYES];
            for (int i = 0; i < NUM_OF_EYES; ++i)
            {
                EyeAnchors[i] = new GameObject();
                EyeAnchors[i].name = "EyeAnchor_" + i;
                EyeAnchors[i].transform.SetParent(gameObject.transform);
                //Debug.Log("CreateEyeAnchors() gameObj:" + gameObject.transform.localPosition.x + ", " + gameObject.transform.localPosition.y + ", " + gameObject.transform.localPosition.z);
                //Debug.Log("CreateEyeAnchors() EyeAnchors:" + EyeAnchors[i].transform.localPosition.x + ", "+ EyeAnchors[i].transform.localPosition.y + ", "+ EyeAnchors[i].transform.localPosition.z);
                //EyeAnchors[i].transform.localPosition = EyesModels[i].localPosition;
                EyeAnchors[i].transform.localRotation = EyesModels[i].localRotation;
                EyeAnchors[i].transform.localScale = EyesModels[i].localScale;
            }
            EyeAnchors[0].transform.localPosition = new Vector3(-1* EyesModels[0].localPosition.x, m_AvatarHeight, 0.0f);//EyesModels[i].localPosition;
            EyeAnchors[1].transform.localPosition = new Vector3(1 * EyesModels[1].localPosition.x, m_AvatarHeight, 0.0f);//EyesModels[i].localPosition;
            //Debug.Log("CreateEyeAnchors ready eye EyeAnchors[0]:" + EyeAnchors[0].transform.localPosition.x + ", " + EyeAnchors[0].transform.localPosition.y);
            //Debug.Log("CreateEyeAnchors ready eye EyeAnchors[1]:" + EyeAnchors[1].transform.localPosition.x + ", " + EyeAnchors[1].transform.localPosition.y);
            ////Debug.Log("CreateEyeAnchors() name:" + gameObject.name + ", height:" + GameObject.Find("Female1").transform.lossyScale.y + ", width" + GameObject.Find("Female1").transform.lossyScale.z);

        }

        public void SetEyeShapeTables(List<EyeShapeTable_v2> eyeShapeTables)
        {
            bool valid = true;
            if (eyeShapeTables == null)
            {
                valid = false;
            }
            else
            {
                for (int table = 0; table < eyeShapeTables.Count; ++table)
                {
                    if (eyeShapeTables[table].skinnedMeshRenderer == null)
                    {
                        valid = false;
                        break;
                    }
                    for (int shape = 0; shape < eyeShapeTables[table].eyeShapes.Length; ++shape)
                    {
                        EyeShape_v2 eyeShape = eyeShapeTables[table].eyeShapes[shape];
                        if (eyeShape > EyeShape_v2.Max || eyeShape < 0)
                        {
                            valid = false;
                            break;
                        }
                    }
                }
            }
            if (valid)
                EyeShapeTables = eyeShapeTables;
        }

        //public void SetEyeShapeAnimationCurves(AnimationCurve[] eyebrowAnimationCurves)
        //{
        //    if (eyebrowAnimationCurves.Length == (int)EyeShape_v2.Max)
        //        EyebrowAnimationCurves = eyebrowAnimationCurves;
        //}
        //public void UpdateEyeShapes(Dictionary<EyeballsData.XrEyeShapeHTC, float> eyeWeightings)
        //{
        //    foreach (var table in EyeShapeTables)
        //        RenderModelEyeShape(table, eyeWeightings);
        //}

        //private void RenderModelEyeShape(EyeShapeTable_v2 eyeShapeTable, Dictionary<EyeballsData.XrEyeShapeHTC, float> weighting)
        //{
        //    for (int i = 0; i < eyeShapeTable.eyeShapes.Length; ++i)
        //    {
        //        EyeShape_v2 eyeShape = eyeShapeTable.eyeShapes[i];
        //        if (eyeShape > EyeShape_v2.Max || eyeShape < 0 || !ShapeMap.ContainsKey(eyeShape)) continue;
        //        EyeballsData.XrEyeShapeHTC xreyeshape = ShapeMap[eyeShape];
        //        if (eyeShape == EyeShape_v2.Eye_Left_Blink || eyeShape == EyeShape_v2.Eye_Right_Blink)
        //        {

        //            eyeShapeTable.skinnedMeshRenderer.SetBlendShapeWeight(i, weighting[xreyeshape] * 100f);
        //        }
        //        else
        //        {
        //            AnimationCurve curve = EyebrowAnimationCurves[(int)eyeShape];
        //            eyeShapeTable.skinnedMeshRenderer.SetBlendShapeWeight(i, curve.Evaluate(weighting[xreyeshape]) * 100f);
        //        }


        //    }
        //}


        private void DestroyEyeAnchors()
        {
            if (EyeAnchors != null)
            {
                foreach (var obj in EyeAnchors)
                    if (obj != null) Destroy(obj);
            }
        }


    }
}
