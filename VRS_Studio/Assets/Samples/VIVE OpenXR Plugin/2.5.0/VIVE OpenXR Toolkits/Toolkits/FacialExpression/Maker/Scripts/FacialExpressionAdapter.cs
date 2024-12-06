// Copyright HTC Corporation All Rights Reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using UnityEngine.XR.OpenXR;
using VIVE.OpenXR.FacialTracking;

namespace VIVE.OpenXR.Toolkits.FacialExpression.Maker
{
    public class FacialExpressionAdapter : MonoBehaviour
    {
        public static readonly FacialExpressionData.ExpressionsEye[] s_EyeExpressions = new FacialExpressionData.ExpressionsEye[(int)FacialExpressionData.ExpressionsEye.MAX]
        {
            FacialExpressionData.ExpressionsEye.LEFT_BLINK, // 0
            FacialExpressionData.ExpressionsEye.LEFT_WIDE,
            FacialExpressionData.ExpressionsEye.RIGHT_BLINK,
            FacialExpressionData.ExpressionsEye.RIGHT_WIDE,
            FacialExpressionData.ExpressionsEye.LEFT_SQUEEZE,
            FacialExpressionData.ExpressionsEye.RIGHT_SQUEEZE, // 5
            FacialExpressionData.ExpressionsEye.LEFT_DOWN,
            FacialExpressionData.ExpressionsEye.RIGHT_DOWN,
            FacialExpressionData.ExpressionsEye.LEFT_OUT,
            FacialExpressionData.ExpressionsEye.RIGHT_IN,
            FacialExpressionData.ExpressionsEye.LEFT_IN, // 10
            FacialExpressionData.ExpressionsEye.RIGHT_OUT,
            FacialExpressionData.ExpressionsEye.LEFT_UP,
            FacialExpressionData.ExpressionsEye.RIGHT_UP,
        };
        public static readonly FacialExpressionData.ExpressionsLip[] s_LipExps = new FacialExpressionData.ExpressionsLip[(int)FacialExpressionData.ExpressionsLip.Max]
        {
            FacialExpressionData.ExpressionsLip.Jaw_Right,               // 0
            FacialExpressionData.ExpressionsLip.Jaw_Left,
            FacialExpressionData.ExpressionsLip.Jaw_Forward,
            FacialExpressionData.ExpressionsLip.Jaw_Open,
            FacialExpressionData.ExpressionsLip.Mouth_Ape_Shape,
            FacialExpressionData.ExpressionsLip.Mouth_Upper_Right,       // 5
            FacialExpressionData.ExpressionsLip.Mouth_Upper_Left,
            FacialExpressionData.ExpressionsLip.Mouth_Lower_Right,
            FacialExpressionData.ExpressionsLip.Mouth_Lower_Left,
            FacialExpressionData.ExpressionsLip.Mouth_Upper_Overturn,
            FacialExpressionData.ExpressionsLip.Mouth_Lower_Overturn,    // 10
            FacialExpressionData.ExpressionsLip.Mouth_Pout,
            FacialExpressionData.ExpressionsLip.Mouth_Raiser_Right,
            FacialExpressionData.ExpressionsLip.Mouth_Raiser_Left,
            FacialExpressionData.ExpressionsLip.Mouth_Stretcher_Right,
            FacialExpressionData.ExpressionsLip.Mouth_Stretcher_Left,          // 15
            FacialExpressionData.ExpressionsLip.Cheek_Puff_Right,
            FacialExpressionData.ExpressionsLip.Cheek_Puff_Left,
            FacialExpressionData.ExpressionsLip.Cheek_Suck,
            FacialExpressionData.ExpressionsLip.Mouth_Upper_UpRight,
            FacialExpressionData.ExpressionsLip.Mouth_Upper_UpLeft,      // 20
            FacialExpressionData.ExpressionsLip.Mouth_Lower_DownRight,
            FacialExpressionData.ExpressionsLip.Mouth_Lower_DownLeft,
            FacialExpressionData.ExpressionsLip.Mouth_Upper_Inside,
            FacialExpressionData.ExpressionsLip.Mouth_Lower_Inside,
            FacialExpressionData.ExpressionsLip.Mouth_Lower_Overlay,     // 25
            FacialExpressionData.ExpressionsLip.Tongue_Longstep1,
            FacialExpressionData.ExpressionsLip.Tongue_Left,
            FacialExpressionData.ExpressionsLip.Tongue_Right,
            FacialExpressionData.ExpressionsLip.Tongue_Up,
            FacialExpressionData.ExpressionsLip.Tongue_Down,             // 30
            FacialExpressionData.ExpressionsLip.Tongue_Roll,
            FacialExpressionData.ExpressionsLip.Tongue_Longstep2,
            FacialExpressionData.ExpressionsLip.Tongue_UpRight_Morph,
            FacialExpressionData.ExpressionsLip.Tongue_UpLeft_Morph,
            FacialExpressionData.ExpressionsLip.Tongue_DownRight_Morph,  // 35
            FacialExpressionData.ExpressionsLip.Tongue_DownLeft_Morph,
        };
        #region Log
        const string LOG_TAG = "Wave.Essence.FacialExpression.Maker.VIVEFacialExpressionAdapter";
        StringBuilder m_sb = null;
        StringBuilder sb
        {
            get
            {
                if (m_sb == null) { m_sb = new StringBuilder(); }
                return m_sb;
            }
        }
        void DEBUG(StringBuilder msg) { Debug.Log(msg); }
        int logFrame = -1;
        bool printIntervalLog = false;
        void INFO(StringBuilder msg) { Debug.Log(msg); }
        #endregion

        [SerializeField]
        private FacialExpressionConfig m_MappingConfigGUI;
        public FacialExpressionConfig MappingConfigGUI { get { return m_MappingConfigGUI; } set { m_MappingConfigGUI = value; } }
        [SerializeField]
        private  GameObject m_TargetFace = null;
        public GameObject TargetFace { get { return m_TargetFace; } set { m_TargetFace = value; } }

        [SerializeField]
        private List<GameObject> m_TargetFaceAppend = null; //= new List<GameObject>();
        public List<GameObject> TargetFaceAppend { get { return m_TargetFaceAppend; } set { m_TargetFaceAppend = value; } }

        public bool blendShapeOnly = true;
        public static string targetFaceStr = "";
        public static List<string> targetFaceAppendStr = new List<String>();
        [SerializeField] public FacialExpressionConfigRuntime MappingConfigExec;


        static bool initAdapterData = false;
        private class AdapterDataItem
        {
            public List<int> Id { get; set; }
            public List<float> Weight { get; set; }
            public AdapterDataItem(List<int> id, List<float> weight)
            {
                Id = id;
                Weight = weight;
            }
        }
        //List<List<AdapterDataItem>> listAdapterItem = new List<List<AdapterDataItem>>();
        List<AdapterDataItem[]> listAdapterItem = new List<AdapterDataItem[]>();


        public class ObjSkinnedMeshInfo
        {
            public string Target { get; set; }
            public int ID { get; set; }
            public string Name { get; set; }
            public float OriginWeight { get; set; }
        }
        static List<List<ObjSkinnedMeshInfo>> ListOfBlendShapes = new List<List<ObjSkinnedMeshInfo>>();
        static List<ObjSkinnedMeshInfo> ListOfBlendShapesSub;//= new List<ObjSkinnedMeshInfo>();
        public static List<ObjSkinnedMeshInfo> ListOfBlendShapesAll = new List<ObjSkinnedMeshInfo>();
        static Transform rootTransform = null;

        private static float[] eyeExps = new float[(int)XrEyeExpressionHTC.XR_EYE_EXPRESSION_MAX_ENUM_HTC];//new float[(int)InputDeviceEye.Expressions.MAX];
        private static float[] lipExps = new float[(int)XrLipExpressionHTC.XR_LIP_EXPRESSION_MAX_ENUM_HTC];//new float[(int)InputDeviceLip.Expressions.Max];

        public static List<Transform> ListOfTrans=null;

        void OnValidate() { rootTransform = this.transform; }
        void Reset()
        {
            if (null == rootTransform)
                rootTransform = this.transform;
        }
        void OnEnable()
        {
            if (null == rootTransform)
                rootTransform = this.transform;
        }
        public static void Prepareblendshapes()
        {
            ListOfTrans = Get_ListOfTrans(rootTransform);
            List<Transform> Get_ListOfTrans(Transform root)
            {
                List<Transform> _TempList = new List<Transform>();

                _AddJointsInfo(root);

                void _AddJointsInfo(Transform _Joint)
                {
                    if (_Joint == null) { return; }
                    for (int i = 0; i < _Joint.childCount; i++)
                    {
                        Transform _TempTrans = _Joint.GetChild(i);
                        _AddJointsInfo(_TempTrans);
                    }
                    if (_Joint.tag != "Dont")
                    {
                        _TempList.Add(_Joint);
                    }
                }
                return _TempList;
            }
            //if (null == ListOfTrans) Debug.Log("Prepareblendshapes() ListOfTrans is null");

            for (int i = 0; i < ListOfTrans.Count; i++)
            {
                ListOfBlendShapesSub = new List<ObjSkinnedMeshInfo>();

                if ((ListOfTrans[i].GetComponent<SkinnedMeshRenderer>() != null) &&
                        ListOfTrans[i].GetComponent<SkinnedMeshRenderer>().sharedMesh.blendShapeCount > 0)
                {
                    for (int j = 0; j < ListOfTrans[i].GetComponent<SkinnedMeshRenderer>().sharedMesh.blendShapeCount; j++)
                        ListOfBlendShapesSub.Add(new ObjSkinnedMeshInfo { Target = ListOfTrans[i].name, ID = j, Name = ListOfTrans[i].GetComponent<SkinnedMeshRenderer>().sharedMesh.GetBlendShapeName(j), OriginWeight = ListOfTrans[i].GetComponent<SkinnedMeshRenderer>().GetBlendShapeWeight(j) });

                    ListOfBlendShapes.Add(ListOfBlendShapesSub);
                }
            }
            for (int i = 0; i < ListOfBlendShapes.Count; i++)
            {
                List<ObjSkinnedMeshInfo> blendShapesList = ListOfBlendShapes[i];
                for (int j = 0; j < blendShapesList.Count; j++)
                {
                    ObjSkinnedMeshInfo info = blendShapesList[j];
                    // must collect all blendshape
                    ListOfBlendShapesAll.Add(new ObjSkinnedMeshInfo { Target = info.Target, ID = info.ID, Name = info.Name, OriginWeight = info.OriginWeight });
                }
            }
            if (null == ListOfTrans) Get_ListOfTrans(rootTransform);
        }
        public void PrepareAdapterMapping() {
            AdapterDataItem[] subAdapterItem = new AdapterDataItem[58];
            for (int i = 0; i < 58; i++)
            {
                List<int> ids = new List<int>();
                List<float> weights = new List<float>();
                subAdapterItem[i] = new AdapterDataItem(ids, weights);
            }
            //Debug.Log("subAdapterItem[Int32.Parse(MappingConfigExec.edges[i].outputPortIdentifier)].Id.Count="+ subAdapterItem[Int32.Parse(MappingConfigExec.edges[5].outputPortIdentifier)].Id.Count);
            initAdapterData = true;

            if ("" == targetFName) return;
            for (int i = 0; i < MappingConfigExec.edges.Count; i++)
            {
                if (/*(Int32.Parse(MappingConfigExec.edges[i].outputPortIdentifier) < eyeExps.Length) &&*/ (MappingConfigExec.edges[i].weightValue > 0))
                {
                    /*var*/
                    result = from r in ListOfBlendShapesAll where (r.Target == targetFName && r.Name == MappingConfigExec.edges[i].blendShapeName) select r;
                    //if (result.Any())
                    Debug.Log(i+"subAdapterItem[Int32.Parse(MappingConfigExec.edges[i].outputPortIdentifier)].Id.Count=" 
                        + subAdapterItem[Int32.Parse(MappingConfigExec.edges[i].outputPortIdentifier)].Id.Count);
                    if (subAdapterItem[Int32.Parse(MappingConfigExec.edges[i].outputPortIdentifier)].Id.Count == 0)
                    {
                        //firstID = result.First().ID;
                        //tmpId.Add(result.First().ID);
                        //tmpWeight.Add(MappingConfigExec.edges[i].weightValue);
                        subAdapterItem[Int32.Parse(MappingConfigExec.edges[i].outputPortIdentifier)] = new AdapterDataItem(new List<int> { result.First().ID },
                            new List<float> { MappingConfigExec.edges[i].weightValue });
                    }
                    else {
                        subAdapterItem[Int32.Parse(MappingConfigExec.edges[i].outputPortIdentifier)].Id.Add(result.First().ID);
                        subAdapterItem[Int32.Parse(MappingConfigExec.edges[i].outputPortIdentifier)].Weight.Add(MappingConfigExec.edges[i].weightValue);
                    }

                }
                //else if ((MappingConfigExec.edges[i].weightValue > 0))
                //{
                //    //lipIdx = Int32.Parse(MappingConfigExec.edges[i].outputPortIdentifier) - (int)FacialExpressionData.ExpressionsEye.MAX;//eyeExps.Length;
                //    /*var*/
                //    result = from r in ListOfBlendShapesAll where (r.Target == targetFName && r.Name == MappingConfigExec.edges[i].blendShapeName) select r;
                //    //if (result.Any())
                //    if (subAdapterItem[Int32.Parse(MappingConfigExec.edges[i].outputPortIdentifier)].Id.Count < 1)
                //    {
                //        //firstID = result.First().ID;
                //        tmpId.Add(result.First().ID);
                //        tmpWeight.Add(MappingConfigExec.edges[i].weightValue);
                //        subAdapterItem[Int32.Parse(MappingConfigExec.edges[i].outputPortIdentifier)] = new AdapterDataItem(tmpId, tmpWeight);
                //    }
                //    else
                //    {
                //        subAdapterItem[Int32.Parse(MappingConfigExec.edges[i].outputPortIdentifier)].Id.Add(result.First().ID);
                //        subAdapterItem[Int32.Parse(MappingConfigExec.edges[i].outputPortIdentifier)].Weight.Add(MappingConfigExec.edges[i].weightValue);
                //    }
                //}
            }
            listAdapterItem.Add(subAdapterItem);



            if (TargetFaceAppend.Count < 1) return;
            // To-Do for others blendshapes action
            for (int n = 0; n < TargetFaceAppend.Count; n++)
            {
                subAdapterItem = new AdapterDataItem[58];
                //if (ListOfTrans[i].name.ToString().Equals(TargetFaceAppend[n].name.ToString()))
                {
                    for (int j = 0; j < MappingConfigExec.edges.Count; j++) //51 wave
                    {
                        if (/*(Int32.Parse(MappingConfigExec.edges[j].outputPortIdentifier) < eyeExps.Length) &&*/ (MappingConfigExec.edges[j].weightValue > 0))
                        {
                            /*var*/
                            result = from r in ListOfBlendShapesAll where (r.Target == TargetFaceAppend[n].name.ToString() && r.Name == MappingConfigExec.edges[j].blendShapeName) select r;
                            //if (result.Any())
                            if (subAdapterItem[Int32.Parse(MappingConfigExec.edges[j].outputPortIdentifier)].Id.Count == 0)
                            {
                                //firstID = result.First().ID;
                                //tmpId.Add(result.First().ID);
                                //tmpWeight.Add(MappingConfigExec.edges[i].weightValue);
                                subAdapterItem[Int32.Parse(MappingConfigExec.edges[j].outputPortIdentifier)] = new AdapterDataItem(new List<int> { result.First().ID },
                                    new List<float> { MappingConfigExec.edges[j].weightValue });
                            }
                            else
                            {
                                subAdapterItem[Int32.Parse(MappingConfigExec.edges[j].outputPortIdentifier)].Id.Add(result.First().ID);
                                subAdapterItem[Int32.Parse(MappingConfigExec.edges[j].outputPortIdentifier)].Weight.Add(MappingConfigExec.edges[j].weightValue);
                            }
                        }
                        //else if ((MappingConfigExec.edges[j].weightValue > 0))
                        //{
                        //    //lipIdx = Int32.Parse(MappingConfigExec.edges[j].outputPortIdentifier) - (int)XrEyeExpressionHTC.XR_EYE_EXPRESSION_MAX_ENUM_HTC;//eyeExps.Length;
                        //    /*var*/
                        //    result = from r in ListOfBlendShapesAll where (r.Target == TargetFaceAppend[n].name.ToString() && r.Name == MappingConfigExec.edges[j].blendShapeName) select r;
                        //    //if (result.Any())
                        //    {
                        //        firstID = result.First().ID;
                        //        subAdapterItem[Int32.Parse(MappingConfigExec.edges[j].outputPortIdentifier)] = new AdapterDataItem(result.First().ID, MappingConfigExec.edges[j].weightValue);
                        //    }
                        //}
                    }
                }
                listAdapterItem.Add(subAdapterItem);
            }
            //for (int k = 0; k < listAdapterItem.Count; k++)
            //    for (int i = 0; i < listAdapterItem[k].Length; i++)
            //{
            //    Debug.Log(listAdapterItem.Count + ", " + listAdapterItem[k].Length + " listAdapterItem[k]=(" + i + ", " + listAdapterItem[k][i].Id.Count + ")");
            //    for (int j = 0; j < listAdapterItem[k][i].Id.Count; j++)
            //        Debug.Log("listAdapterItem[k]=(" + i + ", " + j + "), " + listAdapterItem[k][i].Id[j] + ", " + listAdapterItem[k][i].Weight[j]);
            //}


        }


        void Awake(){ Debug.Log("Awake() LerModel3 "); }
        void Start()
        {
            Debug.Log("start() LerModel3 config edges begin:");
            ListOfTrans = Get_ListOfTrans(rootTransform);
            List<Transform> Get_ListOfTrans(Transform root)
            {
                List<Transform> _TempList = new List<Transform>();

                _AddJointsInfo(root);

                void _AddJointsInfo(Transform _Joint)
                {
                    if (_Joint == null) { return; }
                    for (int i = 0; i < _Joint.childCount; i++)
                    {
                        Transform _TempTrans = _Joint.GetChild(i);
                        _AddJointsInfo(_TempTrans);
                    }
                    if (_Joint.tag != "Dont")
                    {
                        _TempList.Add(_Joint);
                    }
                }
                return _TempList;
            }
            //if (null == ListOfTrans) Debug.Log("Start() ListOfTrans is null");

            //InputDeviceEye.ActivateEyeExpression(true);
            //InputDeviceLip.ActivateLipExp(true);
            //sb.Clear().Append("VIVEFacialExpressionMakerAdapter Start() Eye Avilable: ").Append(InputDeviceEye.IsEyeExpressionAvailable()); INFO(sb);
            //sb.Clear().Append("VIVEFacialExpressionMakerAdapter Start() Lip Avilable: ").Append(InputDeviceLip.IsLipExpAvailable()); INFO(sb);
            for (int i = 0; i < ListOfTrans.Count; i++)
            {
                ListOfBlendShapesSub = new List<ObjSkinnedMeshInfo>();
                //Debug.Log("Start() ListOfTrans[] name: " + ListOfTrans[i].name+", /i: "+i+", count:"+ ListOfTrans.Count);
                if ((ListOfTrans[i].GetComponent<SkinnedMeshRenderer>() != null) &&
                        ListOfTrans[i].GetComponent<SkinnedMeshRenderer>().sharedMesh.blendShapeCount > 0)
                {
                    //Debug.Log("Start() Obj has SkinnedMeshRenderer! " + ListOfTrans[i].name + ", count: " + ListOfTrans[i].GetComponent<SkinnedMeshRenderer>().sharedMesh.blendShapeCount);
                    for (int j = 0; j < ListOfTrans[i].GetComponent<SkinnedMeshRenderer>().sharedMesh.blendShapeCount; j++)
                        ListOfBlendShapesSub.Add(new ObjSkinnedMeshInfo { Target = ListOfTrans[i].name, ID = j, Name = ListOfTrans[i].GetComponent<SkinnedMeshRenderer>().sharedMesh.GetBlendShapeName(j), OriginWeight = ListOfTrans[i].GetComponent<SkinnedMeshRenderer>().GetBlendShapeWeight(j) });

                    ListOfBlendShapes.Add(ListOfBlendShapesSub);
                }
            }

            // trace the avatar blendshapes
            for (int i = 0; i < ListOfBlendShapes.Count; i++)
            {
                //Debug.Log("Start() ListOfBlendShapes[{i}] data:===>");
                List<ObjSkinnedMeshInfo> blendShapesList = ListOfBlendShapes[i];
                for (int j = 0; j < blendShapesList.Count; j++)
                {
                    //Debug.Log($"Start() i: " + i + ", j: " + j);
                    ObjSkinnedMeshInfo info = blendShapesList[j];
                    //Debug.Log($"Start() Count: {blendShapesList.Count}, Target: {info.Target}, ID: {info.ID}, Name: {info.Name}");
                    // must collect all blendshape
                    ListOfBlendShapesAll.Add(new ObjSkinnedMeshInfo { Target = info.Target, ID = info.ID, Name = info.Name, OriginWeight = info.OriginWeight });
                }
            }
            // debug all blendshape information
            //for (int i = 0; i < ListOfBlendShapesAll.Count; i++)
            //	Debug.Log($"Start() BlendShapesAll: " + ListOfBlendShapesAll[i].Target + ", id: " + ListOfBlendShapesAll[i].ID+",name:"+ ListOfBlendShapesAll[i].Name);

            targetF = TargetFace.GetComponent<SkinnedMeshRenderer>();
            targetFName = TargetFace.name.ToString();
            targetBS = targetF.sharedMesh.blendShapeCount;
            PrepareAdapterMapping();
            Debug.Log("Start(262) LerModel3 config name:" + MappingConfigExec.name + ", node/edges end:" + MappingConfigExec.nodes.Count+" / "+ MappingConfigExec.edges.Count);
        }

        #region LogPrint
        void LogEyeValues()
        {
            if (printIntervalLog)
            {
                for (int i = 0; i < (int)XrEyeExpressionHTC.XR_EYE_EXPRESSION_MAX_ENUM_HTC; i++)
                {
                    sb.Clear().Append(s_EyeExpressions[i]).Append(": ").Append(eyeExps[i]);
                    DEBUG(sb);
                }
            }
        }
        void LogLipValues()
        {
            if (printIntervalLog)
            {
                for (int i = 0; i < (int)XrLipExpressionHTC.XR_LIP_EXPRESSION_MAX_ENUM_HTC; i++)
                {
                    sb.Clear().Append(s_LipExps[i]).Append(": ").Append(lipExps[i]);
                    DEBUG(sb);
                }
            }
        }
        #endregion

        ViveFacialTracking feature;
        // Update is called once per frame
        void Update()
        {
            logFrame++;
            logFrame %= 300;
            printIntervalLog = (logFrame == 0);

            feature = OpenXRSettings.Instance.GetFeature<ViveFacialTracking>();
            if (feature != null)
            {
                // Eye expressions
                {
                    if (feature.GetFacialExpressions(XrFacialTrackingTypeHTC.XR_FACIAL_TRACKING_TYPE_EYE_DEFAULT_HTC, out float[] exps))
                    {
                        eyeExps = exps;
                        LogEyeValues();
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

            //How large is the user's mouth is opening. 0 = closed 1 = full opened
            //Debug.Log("Left Blink: " + eyeExps[(int)XrEyeExpressionHTC.XR_EYE_EXPRESSION_LEFT_BLINK_HTC]);
            //Is the user's left eye opening? 0 = opened 1 = full closed
            //Debug.Log("Right Blink : " + eyeExps[(int)XrEyeExpressionHTC.XR_EYE_EXPRESSION_RIGHT_BLINK_HTC]);

            LerModel3();
        }

        private IEnumerable<ObjSkinnedMeshInfo> result { get; set; }
        private int lipIdx, firstID, targetBS;
        private SkinnedMeshRenderer targetF;
        private string targetFName = "";
        void LerModel3()
        {
            
            //Debug.Log("VIVEAdapter LerModel3(FaceObject) has BlendShapeSets:" + TargetFace.GetComponent<SkinnedMeshRenderer>().sharedMesh.blendShapeCount + ", name:" + TargetFace.name);
            if (null == ListOfTrans) PrepareTransform(); //Debug.Log("LerModel3() ListOfTrans is null");


            if ((targetF == null) && (TargetFaceAppend == null))
                return;
            if ((targetF != null) && /*targetF.sharedMesh.blendShapeCount*/targetBS > 0)
            {
                for (int j = 0; j < /*targetF.sharedMesh.blendShapeCount*/targetBS; j++)
                    targetF.SetBlendShapeWeight(j, 0.0f);
            }
            //Debug.Log("VIVEAdapter LerModel3(FaceObject) has BlendShapeSets:260");
            if (TargetFaceAppend != null)
            {
                for (int n = 0; n < TargetFaceAppend.Count; n++)
                {
                    for (int m = 0; m < TargetFaceAppend[n].GetComponent<SkinnedMeshRenderer>().sharedMesh.blendShapeCount; m++)
                        TargetFaceAppend[n].GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(m, 0.0f);
                }
            }
            //Debug.Log("VIVEAdapter LerModel3(FaceObject) has BlendShapeSets:269");

            //List <JointInfo> originalJointInfos = MappingConfig.OriginalNodeNew.OriginalRigStatus.JointInfos;
            for (int i = 0; i < ListOfTrans.Count; i++) //111 objects
            {
                if (ListOfTrans[i] == null) { continue; }

            }


            //Debug.Log("VIVEAdapter LerModel3() Adatpter data:" + listAdapterItem.Count /*+ ", targetFAdater:" + listAdapterItem[0].Length*/);
            if (!initAdapterData) return;//PrepareAdapterMapping();
            if (listAdapterItem.Count == 0) return;


            for (int j = 0; j < listAdapterItem[0].Length; j++)
            {
                for (int i = 0; i < listAdapterItem[0][j].Id.Count; i++) {
                    if ((j < eyeExps.Length) && (listAdapterItem[0][j].Weight[i] > 0))
                    {
                        if (targetF.GetBlendShapeWeight(listAdapterItem[0][j].Id[i]) > 0.0f)
                            targetF.SetBlendShapeWeight(listAdapterItem[0][j].Id[i], targetF.GetBlendShapeWeight(listAdapterItem[0][j].Id[i]) + (listAdapterItem[0][j].Weight[i] * eyeExps[j]));
                        else
                            targetF.SetBlendShapeWeight(listAdapterItem[0][j].Id[i], listAdapterItem[0][j].Weight[i] * eyeExps[j]);
                    }
                    else if ((listAdapterItem[0][j].Weight[i] > 0))
                    {
                        lipIdx = j - (int)XrEyeExpressionHTC.XR_EYE_EXPRESSION_MAX_ENUM_HTC; //eyeExps.Length;
                        if (targetF.GetBlendShapeWeight(listAdapterItem[0][j].Id[i]) > 0.0f)
                            targetF.SetBlendShapeWeight(listAdapterItem[0][j].Id[i], targetF.GetBlendShapeWeight(listAdapterItem[0][j].Id[i]) + (listAdapterItem[0][j].Weight[i] * lipExps[lipIdx]));
                        else
                            targetF.SetBlendShapeWeight(listAdapterItem[0][j].Id[i], listAdapterItem[0][j].Weight[i] * lipExps[lipIdx]);
                    }
                }
            }


            if (TargetFaceAppend.Count < 1) return;
            // To-Do for others blendshapes action
            for (int n = 1; n < listAdapterItem.Count; n++)
            {
                //Debug.Log("LerModel3(Additional) has BlendShapeSets51: count:" + TargetFaceAppend.Count + ", targetAdd: " + TargetFaceAppend[n].name.ToString()+", eyeLeng:"+ eyeExps.Length);
                //if (ListOfTrans[i].name.ToString().Equals(TargetFaceAppend[n].name.ToString()))
                {
                    for (int j = 0; j < listAdapterItem[n].Length; j++)
                    {
                        for (int i = 0; i < listAdapterItem[0][j].Id.Count; i++)
                        {
                            if ((j < eyeExps.Length) && (listAdapterItem[n][j].Weight[i] > 0))
                            {
                                if (TargetFaceAppend[n].GetComponent<SkinnedMeshRenderer>().GetBlendShapeWeight(listAdapterItem[n][j].Id[i]) > 0.0f)
                                    TargetFaceAppend[n].GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(listAdapterItem[n][j].Id[i], TargetFaceAppend[n].GetComponent<SkinnedMeshRenderer>().GetBlendShapeWeight(listAdapterItem[n][j].Id[i]) + (listAdapterItem[n][j].Weight[i] * eyeExps[j]));
                                else
                                    TargetFaceAppend[n].GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(listAdapterItem[n][j].Id[i], listAdapterItem[n][j].Weight[i] * eyeExps[j]);
                            }
                            else if ((listAdapterItem[n][j].Weight[i] > 0))
                            {
                                lipIdx = j - (int)XrEyeExpressionHTC.XR_EYE_EXPRESSION_MAX_ENUM_HTC;//eyeExps.Length;
                                if (TargetFaceAppend[n].GetComponent<SkinnedMeshRenderer>().GetBlendShapeWeight(listAdapterItem[n][j].Id[i]) > 0.0f)
                                    TargetFaceAppend[n].GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(listAdapterItem[n][j].Id[i], TargetFaceAppend[n].GetComponent<SkinnedMeshRenderer>().GetBlendShapeWeight(listAdapterItem[n][j].Id[i]) + (listAdapterItem[n][j].Weight[i] * lipExps[lipIdx]));
                                else
                                    TargetFaceAppend[n].GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(listAdapterItem[n][j].Id[i], listAdapterItem[n][j].Weight[i] * lipExps[lipIdx]);
                            }

                        }
                        //Debug.Log("LerModel3(Additional) has BlendShapeSets51(j):" + j + ", targetAdd: " + TargetFaceAppend[n].name.ToString() + ", count: " + TargetFaceAppend.Count);
                        //Debug.Log("LerModel3() has BlendShapeSets51(k):" + k + ", BSName:" + MappingConfig.OriginalNodeNew.LinkingExpNodes[j].BlendShapeSets[k].BSName + ", count: " + MappingConfig.OriginalNodeNew.LinkingExpNodes[j].BlendShapeSets.Count);

                    }
                }
            }




            ////Debug.Log("VIVEAdapter LerModel3() config:"+ MappingConfigExec.name+ ", edges counts(m):" + MappingConfigExec.edges.Count+" ,nodes:"+ MappingConfigExec.nodes.Count);
            //for (int i = 0; i < MappingConfigExec.edges.Count; i++)
            //{
            //    if ((Int32.Parse(MappingConfigExec.edges[i].outputPortIdentifier) < eyeExps.Length) && (MappingConfigExec.edges[i].weightValue > 0))
            //    {
            //        //Debug.Log("LerModel3() Eye target: " + MappingConfigExec.edges[i].blendShapeName+", weight: "+ MappingConfigExec.edges[i].weightValue);
            //        /*var*/ result = from r in ListOfBlendShapesAll where (r.Target == targetFName && r.Name == MappingConfigExec.edges[i].blendShapeName) select r;
            //        //if (result.Any())
            //        {
            //            firstID = result.First().ID;
            //            //Debug.Log("LerModel3() Eye target: " + result.First().Target + " ,BS: " + result.First().Name + ", ID:" + result.First().ID);
            //            //GameObject.Find(TargetFace.name.ToString()).GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(MappingConfig.OriginalNodeNew.LinkingExpNodes[j].BlendShapeSets[k].BSId, MappingConfig.OriginalNodeNew.LinkingExpNodes[j].BlendShapeSets[k].Weight * eyeExps[j]);
            //            if (targetF.GetBlendShapeWeight(firstID) > 0.0f)
            //                targetF.SetBlendShapeWeight(firstID, targetF.GetBlendShapeWeight(firstID) + (MappingConfigExec.edges[i].weightValue * eyeExps[Int32.Parse(MappingConfigExec.edges[i].outputPortIdentifier)]));
            //            else
            //                targetF.SetBlendShapeWeight(firstID, MappingConfigExec.edges[i].weightValue * eyeExps[Int32.Parse(MappingConfigExec.edges[i].outputPortIdentifier)]);
            //        }
            //    }
            //    else if ((MappingConfigExec.edges[i].weightValue > 0))
            //    {
            //        lipIdx = Int32.Parse(MappingConfigExec.edges[i].outputPortIdentifier) - (int)FacialExpressionData.ExpressionsEye.MAX;//eyeExps.Length;
            //        /*var*/ result = from r in ListOfBlendShapesAll where (r.Target == targetFName && r.Name == MappingConfigExec.edges[i].blendShapeName) select r;
            //        //if (result.Any())
            //        {
            //            firstID = result.First().ID;
            //            //Debug.Log("LerModel3() Lip target: " + result.First().Target + " ,BS: " + result.First().Name + ", ID:" + result.First().ID+", weight: " + MappingConfigExec.edges[i].weightValue);
            //            //GameObject.Find(TargetFaceAppend[n].name.ToString()).GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(MappingConfig.OriginalNodeNew.LinkingExpNodes[j].BlendShapeSets[k].BSId, MappingConfig.OriginalNodeNew.LinkingExpNodes[j].BlendShapeSets[k].Weight * lipExps[lipIdx]);
            //            if (targetF.GetBlendShapeWeight(firstID) > 0.0f)
            //                targetF.SetBlendShapeWeight(firstID, targetF.GetBlendShapeWeight(firstID) + (MappingConfigExec.edges[i].weightValue * lipExps[lipIdx]));
            //            else
            //                targetF.SetBlendShapeWeight(firstID, MappingConfigExec.edges[i].weightValue * lipExps[lipIdx]);
            //        }

            //    }
            //}

            //if (TargetFaceAppend.Count < 1) return;
            //// To-Do for others blendshapes action
            //for (int n = 0; n < TargetFaceAppend.Count; n++)
            //{
            //    //Debug.Log("LerModel3(Additional) has BlendShapeSets51: count:" + TargetFaceAppend.Count + ", targetAdd: " + TargetFaceAppend[n].name.ToString()+", eyeLeng:"+ eyeExps.Length);
            //    //if (ListOfTrans[i].name.ToString().Equals(TargetFaceAppend[n].name.ToString()))
            //    {
            //        for (int j = 0; j < MappingConfigExec.edges.Count; j++) //51 wave
            //        {
            //            //Debug.Log("LerModel3(Additional) has BlendShapeSets51(j):" + j + ", targetAdd: " + TargetFaceAppend[n].name.ToString() + ", count: " + TargetFaceAppend.Count);
            //            //Debug.Log("LerModel3() has BlendShapeSets51(k):" + k + ", BSName:" + MappingConfig.OriginalNodeNew.LinkingExpNodes[j].BlendShapeSets[k].BSName + ", count: " + MappingConfig.OriginalNodeNew.LinkingExpNodes[j].BlendShapeSets.Count);
            //            if ((Int32.Parse(MappingConfigExec.edges[j].outputPortIdentifier) < eyeExps.Length) && (MappingConfigExec.edges[j].weightValue > 0))
            //            {
            //                /*var*/ result = from r in ListOfBlendShapesAll where (r.Target == TargetFaceAppend[n].name.ToString() && r.Name == MappingConfigExec.edges[j].blendShapeName) select r;
            //                //if (result.Any())
            //                {
            //                    firstID = result.First().ID;
            //                    //Debug.Log("LerModel3() Eye targetAdd: " + result.First().Target + " ,BS: " + result.First().Name + ", ID" + result.First().ID + ", weight: " + MappingConfigExec.edges[j].weightValue);
            //                    if (TargetFaceAppend[n].GetComponent<SkinnedMeshRenderer>().GetBlendShapeWeight(firstID) > 0.0f)
            //                        TargetFaceAppend[n].GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(firstID, TargetFaceAppend[n].GetComponent<SkinnedMeshRenderer>().GetBlendShapeWeight(firstID) + (MappingConfigExec.edges[j].weightValue * eyeExps[Int32.Parse(MappingConfigExec.edges[j].outputPortIdentifier)]));
            //                    else
            //                        TargetFaceAppend[n].GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(firstID, MappingConfigExec.edges[j].weightValue * (eyeExps[Int32.Parse(MappingConfigExec.edges[j].outputPortIdentifier)]));
            //                }
            //            }
            //            else if ((MappingConfigExec.edges[j].weightValue > 0))
            //            {
            //                lipIdx = Int32.Parse(MappingConfigExec.edges[j].outputPortIdentifier) - (int)XrEyeExpressionHTC.XR_EYE_EXPRESSION_MAX_ENUM_HTC;//eyeExps.Length;
            //                /*var*/ result = from r in ListOfBlendShapesAll where (r.Target == TargetFaceAppend[n].name.ToString() && r.Name == MappingConfigExec.edges[j].blendShapeName) select r;
            //                //if (result.Any())
            //                {
            //                    firstID = result.First().ID;
            //                    //Debug.Log("LerModel3() Lip target: " + result.First().Target + " ,BS: " + result.First().Name + ", ID:" + result.First().ID + ", weight: " + MappingConfigExec.edges[j].weightValue);
            //                    //GameObject.Find(TargetFaceAppend[n].name.ToString()).GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(MappingConfig.OriginalNodeNew.LinkingExpNodes[j].BlendShapeSets[k].BSId, MappingConfig.OriginalNodeNew.LinkingExpNodes[j].BlendShapeSets[k].Weight * lipExps[lipIdx]);
            //                    if (TargetFaceAppend[n].GetComponent<SkinnedMeshRenderer>().GetBlendShapeWeight(firstID) > 0.0f)
            //                        TargetFaceAppend[n].GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(firstID, TargetFaceAppend[n].GetComponent<SkinnedMeshRenderer>().GetBlendShapeWeight(firstID) + (MappingConfigExec.edges[j].weightValue * lipExps[lipIdx]));
            //                    else
            //                        TargetFaceAppend[n].GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(firstID, MappingConfigExec.edges[j].weightValue * lipExps[lipIdx]);
            //                }
            //            }
            //        }
            //    }
            //}


        }


        static Transform _TempTrans;
        static List<Transform> _TempList;
        public static void PrepareTransform()
        {
            if (null == rootTransform) Debug.Log("PrepareTransform() rootTransform is null");
            ListOfTrans = Get_ListOfTrans(rootTransform);
            
            List<Transform> Get_ListOfTrans(Transform root)
            {
                /*List<Transform>*/ _TempList = new List<Transform>();

                _AddJointsInfo(root);

                void _AddJointsInfo(Transform _Joint)
                {
                    if (_Joint == null) { return; }
                    for (int i = 0; i < _Joint.childCount; i++)
                    {
                        /*Transform*/ _TempTrans = _Joint.GetChild(i);
                        _AddJointsInfo(_TempTrans);
                    }
                    if (_Joint.tag != "Dont")
                    {
                        _TempList.Add(_Joint);
                    }
                }
                return _TempList;
            }
            if (null == ListOfTrans) Debug.Log("PrepareTransform() ListOfTrans is null");
        }
        List<Transform> Get_ListOfTrans()
        {
            List<Transform> _TempList = new List<Transform>();

            _AddJointsInfo(transform);

            void _AddJointsInfo(Transform _Joint)
            {
                if (_Joint == null) { return; }
                for (int i = 0; i < _Joint.childCount; i++)
                {
                    Transform _TempTrans = _Joint.GetChild(i);
                    _AddJointsInfo(_TempTrans);
                }
                if (_Joint.tag != "Dont")
                {
                    _TempList.Add(_Joint);
                }
            }
            return _TempList;
        }
    }
}
