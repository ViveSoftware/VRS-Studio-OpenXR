// Copyright HTC Corporation All Rights Reserved.
//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
//using static UnityEngine.GraphicsBuffer;
using System.Linq;

#if UNITY_EDITOR
//using static TreeEditor.TreeEditorHelper;
using UnityEditor.Experimental.GraphView;
using VIVE.OpenXR.Toolkits.FacialExpression.Maker.Editor;

namespace VIVE.OpenXR.Toolkits.FacialExpression.Maker
{
	public class FacialTrackingGraphDefault : FacialTrackingGraph
    {
        FacialExpressionConfig tmpGraph;
        FacialExpressionConfigRuntime tmpGraphRuntime;
        //public static GameObject TargetAvatar;
        //public static GameObject TargetFacial;

        //[MenuItem("Window/01 DefaultGraph")]
		public static /*BaseGraphWindow*/FacialExpressionConfig OpenWithTmpGraph(GameObject _Target, GameObject _TargetFacial)
		{
			//Debug.Log("DefaultGraphWindow OpenWithTmpGraph()");
			var graphWindow = CreateWindow<FacialTrackingGraphDefault>();

			// When the graph is opened from the window, we don't save the graph to disk
			graphWindow.tmpGraph = ScriptableObject.CreateInstance<FacialExpressionConfig>();
            graphWindow.tmpGraphRuntime = ScriptableObject.CreateInstance<FacialExpressionConfigRuntime>();
			//graphWindow.tmpGraph.hideFlags = HideFlags.HideAndDontSave;
            graphWindow.InitializeGraph(graphWindow.tmpGraph, graphWindow.tmpGraphRuntime);

            TargetAvatar = _Target;
            TargetFacial = _TargetFacial;
            CurrentControllerNew = null;
            CurrentControllerNewRuntime = null;
            graphWindow.InitializeOriginal(graphWindow.tmpGraph as FacialExpressionConfig/*, TargetAvatar, TargetFacial*/);

            //ProjectWindowUtil.CreateAsset(graphWindow.tmpGraph, "DefaultGraph.asset");
            //graphWindow.BindGraphAsset(graphWindow.tmpGraph);
            //Debug.Log("DefaultGraphWindow OpenWithTmpGraph() name:" + graphWindow.tmpGraph.name);

            //graphWindow.Show();
            //return graphWindow;
            return graphWindow.tmpGraph;

        }


        [MenuItem("Window/01 DefaultGraph")]
        public static FacialTrackingGraph OpenWithTmpGraph()
        {
            //Debug.Log("DefaultGraphWindow OpenWithTmpGraph()");
            var graphWindow = CreateWindow<FacialTrackingGraphDefault>();

            // When the graph is opened from the window, we don't save the graph to disk
            graphWindow.tmpGraph = ScriptableObject.CreateInstance<FacialExpressionConfig>();
            graphWindow.tmpGraphRuntime = ScriptableObject.CreateInstance<FacialExpressionConfigRuntime>();
            //graphWindow.tmpGraph.hideFlags = HideFlags.HideAndDontSave;
            graphWindow.InitializeGraph(graphWindow.tmpGraph, graphWindow.tmpGraphRuntime);
            graphWindow.InitializeOriginal(graphWindow.tmpGraph as FacialExpressionConfig/*, TargetAvatar, TargetFacial*/);

            ProjectWindowUtil.CreateAsset(graphWindow.tmpGraph, "DefaultGraph.asset");
            //graphWindow.BindGraphAsset(graphWindow.tmpGraph);
            //Debug.Log("DefaultGraphWindow OpenWithTmpGraph() name:" + graphWindow.tmpGraph.name);

            graphWindow.Show();
            return graphWindow;
        }


        protected override void OnDestroy()
		{
			Debug.Log("DefaultGraphWindow OnDestroy()");
			graphView?.Dispose();
			//DestroyImmediate(tmpGraph);
		}

		protected override void InitializeWindow(FacialExpressionConfig graph)
		{
			//Debug.Log("DefaultGraphWindow InitializeWindow()");
			titleContent = new GUIContent(/*"Default Graph"*/"FacialExpressionMakerEditor");

			if (graphView == null)
			{
				graphView = new FacialTrackingGraphView(this);
			}

			rootView.Add(graphView);
		}
        protected override void InitializeOriginalExpression(FacialExpressionConfig graph)
        {
            Debug.Log("DefaultGraphWindow InitializeOriginalExpression()"); //+ BaseNode.GetCustomName()
            FacialTrackingNode.SetCustomName("OriginalExpression");
            graphView.RegisterCompleteObjectUndo("Added " + "OriginalExpressionNode");
            //var view = graphView.AddNode(BaseNode.CreateFromType<CustomPortDataNode>(new Vector3(1, 1, 0)));
            graphView.AddNode(FacialTrackingNode.CreateFromType(typeof(OriginalExpressionNode), new Vector2(10, 20)));

            ////for test default graph
            //BaseNode.SetCustomName("Fcl_EYE_Close_L");
            //graphView.RegisterCompleteObjectUndo("Added " + "FloatNode1");
            //graphView.AddNode(BaseNode.CreateFromType(typeof(FloatNode1), new Vector2(360, 20)));
            //BaseNode.SetCustomName("Fcl_EYE_Close_R");
            //graphView.RegisterCompleteObjectUndo("Added " + "FloatNode1");
            //graphView.AddNode(BaseNode.CreateFromType(typeof(FloatNode1), new Vector2(360, 80)));

            rootView.Add(graphView);
        }
        protected override void InitializeCustomExpression(FacialExpressionConfig graph)
        {
            Debug.Log("DefaultGraphWindow InitializeCustomExpression() TargetFaceObjName=" + TargetFacial.name.ToString());
            if ((GameObject.Find(TargetFacial.name.ToString()).GetComponent<SkinnedMeshRenderer>() == null))
            {
                Debug.Log("Button:Auto Expression null SkinnedMeshRenderer");
                return;
            }
            for (int i = 0; i < GameObject.Find(TargetFacial.name.ToString()).GetComponent<SkinnedMeshRenderer>().sharedMesh.blendShapeCount; i++)
            {
                var result = from r in graph.nodes.ToList() where (r.nodeCustomName == GameObject.Find(TargetFacial.name.ToString()).GetComponent<SkinnedMeshRenderer>().sharedMesh.GetBlendShapeName(i).ToString()) select r;
                if (result.Any()) continue;
                
                //Debug.Log("DefaultGraphWindow InitializeCustomExpression() data="+
                //GameObject.Find(TargetFacial.name.ToString()).GetComponent<SkinnedMeshRenderer>().sharedMesh.GetBlendShapeName(i).ToString());
                FacialTrackingNode.SetCustomName(GameObject.Find(TargetFacial.name.ToString()).GetComponent<SkinnedMeshRenderer>().sharedMesh.GetBlendShapeName(i).ToString());
                graphView.RegisterCompleteObjectUndo("Added " + "CustomBSNode "+i.ToString());
                graphView.AddNode(FacialTrackingNode.CreateFromType(typeof(CustomBSNode), new Vector2(500, 20 + i * 15)));
            }

            rootView.Add(graphView);
        }

        //protected override void SaveFacialTrackingController(BaseGraph graph)
        //{
        //    BaseGraph _ControllerNew;
        //    //if (CurrentControllerNew == null)
        //    {
        //        _ControllerNew = CreateAsset<BaseGraph>(_AssetName: "VIVEFacialExpressionConfig");//New Controller
        //        Debug.Log("SaveFacialTrackingController(): CurrentControllerNew is null");
        //    }
        //    _ControllerNew = GraphInspector.tmpConfig;//tmpGraph;

        //    EditorUtility.SetDirty(_ControllerNew);
        //    AssetDatabase.SaveAssets();
        //    AssetDatabase.Refresh();
        //    EditorUtility.FocusProjectWindow();
        //}
        private static string mappingconfigFile = null;
        T CreateAssetAuto<T>(string _AssetName = "new BaseGraph", string _Path = "VIVEFacialExpressionMakerAsset") where T : ScriptableObject
        {
            //ScriptableObject _Asset = ScriptableObject.CreateInstance<ScriptableObject>();
            T _Asset = ScriptableObject.CreateInstance<T>();

            // show save file dialog and let user set the file name and path
            string absoluteFilePath = EditorUtility.SaveFilePanel(
                "Save Asset", //Dialog Title
                "Assets", //Parent folder name
                "VIVEFacialExpressionConfig.asset", //Default Asset file name
                "asset"
            );
            //Debug.Log("CreateAssetAuto absoPath:" + absoluteFilePath);

            // save data to asset file
            if (!string.IsNullOrEmpty(absoluteFilePath))
            {
                //Debug.Log("CreateAssetAuto():" + absoluteFilePath + ", AppPath:" + Application.dataPath);
                // change absoluteFilePath tom mapping "Assets" folder path
                string relativePath = absoluteFilePath.Replace(Application.dataPath, "Assets");
                mappingconfigFile = relativePath;
                //Debug.Log("CreateAssetAuto() mappingconfigFile:" + mappingconfigFile);

                AssetDatabase.CreateAsset(_Asset, /*relativePath*/relativePath.Replace(".asset", "BlendShape.asset"));
                EditorUtility.SetDirty(_Asset);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            else
                return null;
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = _Asset;
            return _Asset;
        }
        public T CreateAsset<T>(string _AssetName = "new BaseGraph", string _Path = "VIVEFacialExpressionMakerAsset") where T : ScriptableObject
        {
            T _Asset = ScriptableObject.CreateInstance<T>();
            //if (!AssetDatabase.IsValidFolder($"Assets/{_Path}"))
            //{
            //	AssetDatabase.CreateFolder("Assets", _Path);
            //}

            // show save file dialog and let user set the file name and path
            string absoluteFilePath = EditorUtility.SaveFilePanel(
                "Save Asset", //Dialog Title
                "Assets", //Parent folder name
                "VIVEFacialExpressionConfig.asset", //Default Asset file name
                "asset"
            );

            //if (!string.IsNullOrEmpty(mappingconfigFile))
            if (!string.IsNullOrEmpty(absoluteFilePath))
            {
                //Debug.Log("CreateAsset() mappingconfigFile:" + mappingconfigFile);
                //AssetDatabase.CreateAsset(_Asset, mappingconfigFile);
                //must to map to filterConfig.yml
                string relativePath = absoluteFilePath.Replace(Application.dataPath, "Assets");
                AssetDatabase.CreateAsset(_Asset, relativePath);
                EditorUtility.SetDirty(_Asset);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            else
                return null;

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = _Asset;
            //mappingconfigFile = null;
            return _Asset;
        }



    }
}
#endif
