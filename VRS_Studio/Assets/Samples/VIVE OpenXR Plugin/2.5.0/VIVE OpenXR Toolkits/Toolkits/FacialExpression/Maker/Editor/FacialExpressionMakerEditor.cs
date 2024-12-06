using UnityEngine;
using VIVE.OpenXR.Toolkits.FacialExpression.Maker;
//using UnityEditor.UIElements;
//using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
using VIVE.OpenXR.Toolkits.FacialExpression.Maker.Editor;

//namespace Maker {

[CustomEditor(typeof(FacialExpressionAdapter))]
    public class FacialExpressionMakerEditor : Editor
    {
        //protected VisualElement root;
        //protected BaseGraph graph;
    //protected ExposedParameterFieldFactory exposedParameterFactory;
    //VisualElement           parameterContainer;


    public static SerializedProperty MappingConfigProp = null;
    public static SerializedProperty TargetFaceProp = null;
    public static SerializedProperty MappingConfigRuntimeProp = null;
    public static SerializedProperty TargetFaceAppendProp = null;
    SerializedProperty BlendShapeOnlyProp;
    private GUIStyle BtnStyle;
    //public GameObject TokenMappingConfig { get; set; }

    public static FacialExpressionConfig tmpConfig = null;
    public static FacialExpressionConfigRuntime tmpConfigRuntime = null;
    public static void SetMappingConfig(FacialExpressionConfig _config, FacialExpressionConfigRuntime _configRuntime) { 
        tmpConfig = _config;
        tmpConfigRuntime = _configRuntime;
        //FacialExpressionAdapter.SetMappingConfigTmp(tmpConfig);
        Debug.Log("SetMappingConfig() config name:" + _config.name + ", node/edges end:" + _config.nodes.Count + " / " + _config.edges.Count + " / " + _config.edges.Count);
    }


    protected virtual void OnEnable()
    {
        MappingConfigProp = serializedObject.FindProperty("m_MappingConfigGUI");
        TargetFaceProp = serializedObject.FindProperty("m_TargetFace");
        TargetFaceAppendProp = serializedObject.FindProperty("m_TargetFaceAppend");
        BlendShapeOnlyProp = serializedObject.FindProperty("blendShapeOnly");

        MappingConfigRuntimeProp = serializedObject.FindProperty("MappingConfigExec");


        //auto call by CustomEditor declare
        //graph = target as FacialExpressionConfig;

        //root = new VisualElement();
        //CreateInspector();
        //if (MappingConfigProp != null) Debug.Log("GraphInspector OnEnable() edges:"+ (MappingConfigProp.objectReferenceValue as BaseGraph).edges.Count);
    }

    //protected virtual void OnDisable() {
    //}

    //    public sealed override VisualElement CreateInspectorGUI()
    //    {
    //    //auto call by CustomEditor declare
    //    Debug.Log("GraphInspector CreateInspectorGUI()");
    //    root = new VisualElement();
    //    CreateInspector();
    //    return root;
    //}

    //protected virtual void CreateInspector()
    //{
            //Debug.Log("GraphInspector CreateInspector()");
            //if (graph == null)
            //{
            //    //DefaultGraphWindow.OpenWithTmpGraph();
            //    root.Add(new Button(() => DefaultGraphWindow.OpenWithTmpGraph())
            //    { text = "0.Create base graph window" });
            //}

        //parameterContainer = new VisualElement
        //{
        //    name = "ExposedParameters"
        //};
        //FillExposedParameters(parameterContainer);
        //root.Add(parameterContainer);

    //}

    // CreateInspectorGUI and OnInspectorGUI only one can callback, can use in the same time
    // Don't use ImGUI
    //public sealed override void OnInspectorGUI() {}
    //public /*sealed*/ override void OnInspectorGUI()
    public override void OnInspectorGUI()
    {
        //Debug.Log("GraphInspector OnInspectorGUI()");
        //scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        //root.Add(new Button(() => EditorWindow.GetWindow<DefaultGraphWindow>().InitializeGraph(target as BaseGraph))
        //{ text = "Open base graph window" });
        if (tmpConfig != null)
        {
            MappingConfigProp.objectReferenceValue = tmpConfig/*VIVEFacialTrackingGraph.CurrentControllerNew*/ as FacialExpressionConfig;
            MappingConfigRuntimeProp.objectReferenceValue = tmpConfigRuntime/*VIVEFacialTrackingGraph.CurrentControllerNew*/ as FacialExpressionConfigRuntime;
            //Debug.Log("GraphInspector OnInspectorGUI() update tmpconfig: " + tmpConfig.edges.Count);
        }
        Begin_WrapWord();
        Begin_Bold();
        Begin_BiggerFrontSize();
        //EditorGUILayout.LabelField("Note:");
        End_Bold();
        End_BiggerFrontSize();
        Begin_Italic();

        End_Italic();
        Begin_BoldItalic();
        //EditorGUILayout.LabelField("The VIVE Facial Tracking Maker is a tool which helps you bind any model with the data provided by VIVE Facial Tracking.", GUILayout.MinHeight(48));
        EditorGUILayout.LabelField("The Facial Expression Adapter helps you bind the VIVE Facial Expressions to a face model¡¦s blend shapes.\nAssign the target face object before creating the facial expression config for the face object.", GUILayout.MinHeight(72));
        End_BoldItalic();


        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(TargetFaceProp);
        EditorGUILayout.PropertyField(MappingConfigProp);
        EditorGUILayout.PropertyField(MappingConfigRuntimeProp);
        EditorGUILayout.PropertyField(TargetFaceAppendProp);
        EditorGUILayout.PropertyField(BlendShapeOnlyProp);
        if (EditorGUI.EndChangeCheck())
        {
            //if (MappingConfigProp != null && MappingConfigProp.objectReferenceValue != null)
                //Debug.Log("MakerEditor OnInspectorGUI() mapping config changed:" + MappingConfigProp.objectReferenceValue.ToString()+", edge:"+ (MappingConfigProp.objectReferenceValue as FacialExpressionConfig).edges.Count);
            tmpConfig = null;
            tmpConfigRuntime = null;
            //Debug.Log("MakerEditor OnInspectorGUI() assign tmpconfig null***");
        }
        serializedObject.ApplyModifiedProperties();


        string btnName = "Open Facial Expression Make Editor";
        if (MappingConfigProp != null && MappingConfigProp.objectReferenceValue != null)
        {
            btnName = "Open Facial Expression Make Editor";
        }
        else
        {
            btnName = "Create BlendShape Mapping Config";
        }
        if (TargetFaceProp != null && TargetFaceProp.objectReferenceValue != null)
        {
            if (MappingConfigProp != null && MappingConfigProp.objectReferenceValue != null)
            {
                //"Open Maker Asset"
                if (GUILayout.Button(btnName, GUI.skin.button, GUILayout.MaxWidth(256), GUILayout.MinHeight(25)))
                {
                    //Debug.Log("OnInspectorGUI() Open target name:" + target.name);
                    FacialExpressionAdapter.Prepareblendshapes();
                    FacialExpressionAdapter.targetFaceStr = TargetFaceProp.name.ToString();
                    //GameObject GameObjectProp = GameObject.Find("Face");
                    FacialTrackingGraph.TargetFacial = TargetFaceProp.objectReferenceValue as GameObject;
                    EditorWindow.GetWindow<FacialTrackingGraphDefault>().InitializeGraph(/*target*/MappingConfigProp.objectReferenceValue as FacialExpressionConfig, MappingConfigRuntimeProp.objectReferenceValue as FacialExpressionConfigRuntime);
                    //BaseGraphWindow.LoadFacialTrackingController(MappingConfigProp.objectReferenceValue as BaseGraph, (target as MonoBehaviour).gameObject, TargetFaceProp.objectReferenceValue as GameObject);
                    FacialTrackingGraph.TokenForController.value = MappingConfigProp.objectReferenceValue;
                    FacialTrackingGraph.TokenForControllerRuntime.value = MappingConfigRuntimeProp.objectReferenceValue;
                }
            }
            else
            {
                //"New Maker config"
                if (GUILayout.Button(btnName, GUI.skin.button, GUILayout.MaxWidth(256), GUILayout.MinHeight(25)))
                {
                    //Debug.Log("OnInspectorGUI() New target name:" + target.name);
                    FacialExpressionAdapter.Prepareblendshapes();
                    FacialExpressionAdapter.targetFaceStr = TargetFaceProp.name.ToString();
                    //DefaultGraphWindow.AssignTargetFacial((target as MonoBehaviour).gameObject, TargetFaceProp.objectReferenceValue as GameObject);
                    //GraphInspector.SetMappingConfig(DefaultGraphWindow.OpenWithTmpGraph((target as MonoBehaviour).gameObject, TargetFaceProp.objectReferenceValue as GameObject)); //ok
                    FacialTrackingGraphDefault.OpenWithTmpGraph((target as MonoBehaviour).gameObject, TargetFaceProp.objectReferenceValue as GameObject);
                    //BaseGraphWindow.LoadFacialTrackingController(MappingConfigProp.objectReferenceValue as BaseGraph, (target as MonoBehaviour).gameObject, TargetFaceProp.objectReferenceValue as GameObject);
                    FacialTrackingGraph.TokenForController.value = MappingConfigProp.objectReferenceValue;
                    FacialTrackingGraph.TokenForControllerRuntime.value = MappingConfigRuntimeProp.objectReferenceValue;
                }
            }
        } else
        {
            //if (TargetFaceProp.objectReferenceValue == null)
            GUI.enabled = false;
            GUILayout.Button(btnName, GUI.skin.button, GUILayout.MaxWidth(216), GUILayout.MinHeight(25));

        }

        Begin_WrapWord();
        Begin_Bold();
        Begin_BiggerFrontSize();
        //EditorGUILayout.LabelField("Note:");
        End_Bold();
        End_BiggerFrontSize();
        Begin_Italic();
        //EditorGUILayout.LabelField("This component aims for making a GameObject behave like a screen space UI in the MR/VR world.");
        End_Italic();
        Begin_BoldItalic();
        //EditorGUILayout.LabelField("In order for the GameObject to be seen in the game, please search the Camera_for_UILike prefab under in the assets and place it under your main camera as a child.");
        End_BoldItalic();

            //if (TargetFaceProp != null && TargetFaceProp.objectReferenceValue != null)
            //{
            //    if (GUILayout.Button("Open Maker Asset", GUI.skin.button, GUILayout.MaxWidth(180), GUILayout.MinHeight(25)))
            //    {
            //        Debug.Log("OnInspectorGUI() target name:" + target.name);
            //        EditorWindow.GetWindow<DefaultGraphWindow>().InitializeGraph(/*target*/MappingConfigProp.objectReferenceValue as BaseGraph);
            //    }
            //    //else if (GUILayout.Button("New Maker config", GUI.skin.button, GUILayout.MaxWidth(180), GUILayout.MinHeight(25)))
            //    //    DefaultGraphWindow.OpenWithTmpGraph();
            //}
            //else
            //{
            //    GUI.enabled = false;
            //    GUILayout.Button("New Maker Config", GUI.skin.button, GUILayout.MaxWidth(180), GUILayout.MinHeight(25));
            //}
            //base.OnInspectorGUI();
            //EditorGUILayout.EndScrollView();
            //Repaint();
        }


    void Begin_WrapWord()
    {
        EditorStyles.label.wordWrap = true;
    }

    void End_WrapWord()
    {
        EditorStyles.label.wordWrap = false;
    }

    void Begin_BiggerFrontSize()
    {
        EditorStyles.label.fontSize += 2;
    }

    void End_BiggerFrontSize()
    {
        EditorStyles.label.fontSize -= 2;
    }

    void Begin_Bold()
    {
        EditorStyles.label.fontStyle = FontStyle.Bold;
    }

    void End_Bold()
    {
        EditorStyles.label.fontStyle = FontStyle.Normal;
    }

    void Begin_Italic()
    {
        EditorStyles.label.fontStyle = FontStyle.Italic;
    }

    void End_Italic()
    {
        EditorStyles.label.fontStyle = FontStyle.Normal;
    }

    void Begin_BoldItalic()
    {
        EditorStyles.label.fontStyle = FontStyle.BoldAndItalic;
    }

    void End_BoldItalic()
    {
        EditorStyles.label.fontStyle = FontStyle.Normal;
    }
}
//}
#endif