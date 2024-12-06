using System.Linq;
using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using VIVE.OpenXR.Toolkits.FacialExpression.Maker;

//using System.Collections.Generic;
//using System.Runtime.CompilerServices;

#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace VIVE.OpenXR.Toolkits.FacialExpression.Maker.Editor
{
    public enum DataMode
    {
        Unknown = -1,
        Rig = 0,
        BlendShape = 1,
        Both = 2,
        Max = 3,
    }

    [System.Serializable]
	public abstract class FacialTrackingGraph : EditorWindow
	{
		protected VisualElement		rootView;
		protected FacialTrackingGraphView		graphView;

		[SerializeField]
		protected FacialExpressionConfig			graph;

        [SerializeField]
        protected FacialExpressionConfigRuntime graphRuntime;

        public static EditorWindow FTWindow;
        public static DataMode UsingMode = DataMode.Rig;
        public static GameObject TargetAvatar;//=null;
        public static GameObject TargetFacial;//=null;
        public static ObjectField TokenForController;
        public static ObjectField TokenForControllerRuntime;
        public static FacialExpressionConfig CurrentControllerNew=null;
        public static FacialExpressionConfigRuntime CurrentControllerNewRuntime = null;

        public bool					isGraphLoaded
		{
			get { return graphView != null && graphView.graph != null; }
		}

		bool						reloadWorkaround = false;

		public event Action< FacialExpressionConfig >	graphLoaded;
		public event Action< FacialExpressionConfig >	graphUnloaded;


        private static GUIContent editorContent = new GUIContent("FacialExpressionMakeEditor");
        private Vector2 scrollPos = Vector2.zero;
        private void OnGUI()
        {
            //Debug.Log("BaseGraphWindow OnGUI()");
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            for (int i = 0; i < 50; i++)
                EditorGUILayout.LabelField(i.ToString());
            EditorGUILayout.EndScrollView();
            Repaint();

            if (GUILayout.Button("Close Window"))
            {
                Debug.Log("OnGUI Button Close Window");
                isClosing = true;
                Close();
            }
        }

        /// Called by Unity when the window is enabled / opened
        protected virtual void OnEnable()
		{
			//Debug.Log("BaseGraphWindow OnEnable()");
			InitializeRootView();

			if (graph != null)
				LoadGraph();
			else
				reloadWorkaround = true;
        }

		protected virtual void Update()
		{
			// Workaround for the Refresh option of the editor window:
			// When Refresh is clicked, OnEnable is called before the serialized data in the
			// editor window is deserialized, causing the graph view to not be loaded
			if (reloadWorkaround && graph != null)
			{
                //Debug.Log("BaseGraphWindow Update() to LoadGraph, reloadWorkaround="+ reloadWorkaround);
                //LoadGraph();
				reloadWorkaround = false;
			}

			//PrintConnectedPortsInfo();
		}

        void PrintConnectedPortsInfo()
        {
			List<Edge> _AllEdges = graphView.edges.ToList();//edges.ToList();

            for (int i = 0; i < _AllEdges.Count; i++)
            {
                Port outputPort = _AllEdges[i].output as Port;
                Port inputPort = _AllEdges[i].input as Port;
				if (outputPort == null || inputPort == null) return;
                Node nodeA = outputPort.node as Node;
                Node nodeB = inputPort.node as Node;

                string nodeAName = nodeA.title;//nodeA.title;
                string nodeBName = nodeB.title;//nodeB.title;
                string outputPortName = outputPort.portName;
                string inputPortName = inputPort.portName;

                //Debug.Log($"Edge connects port(out) '{outputPortName}' of nodeA '{nodeAName}' to port(in) '{inputPortName}' of nodeB '{nodeBName}'");
            }
        }

        void LoadGraph()
		{
            //Debug.Log("BaseGraphWindow LoadGraph()");
            // We wait for the graph to be initialized
            if (graph.isEnabled)
                InitializeGraph(graph, graphRuntime);
            else
                graph.onEnabled += () => InitializeGraph(graph, graphRuntime);

            FacialExpressionMakerEditor.SetMappingConfig(graph, graphRuntime);
		}

		/// Called by Unity when the window is disabled (happens on domain reload)
		protected virtual void OnDisable()
		{
            Debug.Log("BaseGraphWindow OnDisable()");
            if (graph != null && graphView != null)
				graphView.SaveGraphToDisk();

            if (isSaveAs) {
                FacialExpressionMakerEditor.SetMappingConfig(CurrentControllerNew, CurrentControllerNewRuntime);
                isSaveAs = false;
            } else
                FacialExpressionMakerEditor.SetMappingConfig(graph, graphRuntime);
            //FacialExpressionAdapter.SetMappingConfigTmp(graph);
        }

        bool isClosing = false;
        /// Called by Unity when the window is closed
        protected virtual void OnDestroy() { 
            Debug.Log("BaseGraphWindow OnDestroy()");
            //if (isSaveAs)
            //{
            //    FacialExpressionMakerEditor.SetMappingConfig(CurrentControllerNew, CurrentControllerNewRuntime);
            //    isSaveAs = false;
            //}
            //else
            //    FacialExpressionMakerEditor.SetMappingConfig(graph, graphRuntime);
            //FacialExpressionAdapter.SetMappingConfigTmp(graph);

            ////add on 0822 need to verify
            //if (CurrentControllerNew == null)
            //{
            //    SaveFacialTrackingController(graph);
            //    //GraphInspector.SetMappingConfig(CurrentControllerNew);
            //}
            //else
            //{
            //    if (!isClosing)
            //    {
            //        bool result = EditorUtility.DisplayDialog("Mapping Config Have Been Modified", "Do you want to save the changes you made in mapping config?", "Save", "Cancel");
            //        if (result)
            //        {
            //            Debug.Log("User clicked Yes.");
            //            isClosing = true;
            //            //Close();
            //            SaveFacialTrackingController(graph);
            //        }
            //        else
            //        {
            //            Debug.Log("User clicked No.");
            //        }
            //    }
            //}

        }

		void InitializeRootView()
		{
            //Debug.Log("BaseGraphWindow InitializeRootView()");
            rootView = base.rootVisualElement;

			rootView.name = "graphRootView";

			//rootView.styleSheets.Add(Resources.Load<StyleSheet>(graphWindowStyle));
		}

		public void InitializeGraph(FacialExpressionConfig _graph, FacialExpressionConfigRuntime _graphRuntime)
		{
            //Debug.Log("BaseGraphWindow InitializeGraph() 199");
            if (this.graph != null && _graph != this.graph)
			{
				// Save the graph to the disk
				EditorUtility.SetDirty(this.graph);
				AssetDatabase.SaveAssets();

                EditorUtility.SetDirty(this.graphRuntime);
                AssetDatabase.SaveAssets();
				// Unload the graph
				graphUnloaded?.Invoke(this.graph);
			}

			graphLoaded?.Invoke(_graph);
			this.graph = _graph;
            this.graphRuntime = _graphRuntime;

            //// need check mark on 0805
            //if (graphView != null)
                //rootView.Remove(graphView);

            //if (graphView == null) Debug.Log("GraphView create not yet. create by InitializeWindow() function");
            //Initialize will provide the FacialTrackingGraphView(BaseGraphView)
            InitializeWindow(_graph); //DefaultGraphWindow.cs

			graphView = rootView.Children().FirstOrDefault(e => e is FacialTrackingGraphView) as FacialTrackingGraphView;

			if (graphView == null)
			{
				Debug.Log("GraphView has not been added to the BaseGraph root view !");
				return ;
			}

			graphView.Initialize(_graph);

			//InitializeGraphView(graphView);

			// TOOD: onSceneLinked...

			//if (graph.IsLinkedToScene())
			//	LinkGraphWindowToScene(graph.GetLinkedScene());
			//else
				graph.onSceneLinked += LinkGraphWindowToScene;


            GenerateToolbar();
            //InitializeOriginalExpression(graph); //DefaultGraphWindow.cs for add OriginalExpressionState node
            if (null == TargetAvatar)
            {
                CurrentControllerNew = graph; //CurrentControllerNew = _Controller;
                CurrentControllerNewRuntime = graphRuntime; //CurrentControllerNew = _Controller;
            }
            TokenForController.value = _graph;
            TokenForControllerRuntime.value = _graphRuntime;
        }

        public void InitializeOriginal(FacialExpressionConfig _Controller/*, GameObject _Target, GameObject _TargetFacial*/)
		{
            //TargetAvatar = _Target;
            //TargetFacial = _TargetFacial;
            //if (null == _Controller)
			{
                InitializeOriginalExpression(graph); //DefaultGraphWindow.cs for add OriginalExpressionState node
            }
        }
        void LinkGraphWindowToScene(Scene scene)
		{
            //Debug.Log("BaseGraphWindow LinkGraphWindowToScene()");
            EditorSceneManager.sceneClosed += CloseWindowWhenSceneIsClosed;

			void CloseWindowWhenSceneIsClosed(Scene closedScene)
			{
				if (scene == closedScene)
				{
					Close();
					EditorSceneManager.sceneClosed -= CloseWindowWhenSceneIsClosed;
				}
			}
		}

		public virtual void OnGraphDeleted()
		{
            //Debug.Log("BaseGraphWindow OnGraphDeleted()");
            if (graph != null && graphView != null)
				rootView.Remove(graphView);

			graphView = null;
		}

		public void BindGraphAsset(FacialExpressionConfig graph) {}
        void GenerateToolbar()
		{
            //Debug.Log("BaseGraphWindow GenerateToolbar()");
            Toolbar _Toolbar = new Toolbar();
			Button _Button;

            //------Genetate Token for Controller
            ObjectField _ObjField = new ObjectField
            {
                objectType = typeof(FacialExpressionConfig),
            };
            _ObjField.style.width = 220;
            _ObjField.SetEnabled(false);
            TokenForController = _ObjField;
            _Toolbar.Add(_ObjField);
            //------Genetate Token for Controller
            //------Genetate Token for Controller
            ObjectField _ObjFieldSaved = new ObjectField
            {
                objectType = typeof(FacialExpressionConfigRuntime),
            };
            _ObjFieldSaved.style.width = 120;
            _ObjFieldSaved.SetEnabled(false);
            TokenForControllerRuntime = _ObjFieldSaved;
            _Toolbar.Add(_ObjFieldSaved);
            //------Genetate Token for Controller


            //--- Generate Rig Expression Button
            _Button = new Button(clickEvent: () => {
                InitializeCustomExpression(graph);
            });
            _Button.text = "Auto add blendShapes";
            _Toolbar.Add(_Button);
            //--- Generate Rig Expression Button


            //--- Generate Save Button
            _Button = new Button(clickEvent: () => {
                if (graph.nodes.Count > 0) {
                    graphRuntime.nodes.Clear();
                    graphRuntime.edges.Clear();
                    foreach (var node in graph.nodes) { graphRuntime.nodes.Add(node); }
                    foreach (var e in graph.edges) { graphRuntime.edges.Add(SerializableEdgeData.SerializableEdgeDataNew(e, graph)); }
                    graphRuntime.position = graph.position;
                    //Debug.Log("BaseGraphWindow SaveButton() "+ graphRuntime.nodes.Count+", "+ graphRuntime.edges.Count);
                }
                FacialExpressionMakerEditor.SetMappingConfig(graph, graphRuntime);
                SaveFacialTrackingController(graph);
            });
            _Button.text = "Save mapping config";
            _Toolbar.Add(_Button);
            //--- Generate Save Button


            //--- Generate SaveAs Button
            _Button = new Button(clickEvent: () =>
            {
                isSaveAs = true;
                CurrentControllerNew = null;
                CurrentControllerNewRuntime = null;
                SaveFacialTrackingController(graph);
                FacialExpressionMakerEditor.SetMappingConfig(CurrentControllerNew, CurrentControllerNewRuntime);
                //Debug.Log("SaveFacialTrackingController(): SaveAs :" + CurrentControllerNew.nodes.Count+" / "+ CurrentControllerNewRuntime.name);
            });
            _Button.text = "SaveAs mapping config";
            _Toolbar.Add(_Button);
            //--- Generate SaveAs Button

            //rootView.Add(_Toolbar); //doesn't work, need graphview add toolbar then rootview add the graphview
            graphView.Add(_Toolbar);
        }
        static bool isSaveAs = false;
        public void SaveFacialTrackingController(FacialExpressionConfig graph) {
            FacialExpressionConfig _ControllerNew = null;
            FacialExpressionConfigRuntime _ControllerNewRuntime = null;

            if (CurrentControllerNew == null)
            {
                _ControllerNew = CreateAsset<FacialExpressionConfig>(_AssetName: "FacialExpressConfig");//New Controller
                _ControllerNewRuntime = CreateAssetRuntime<FacialExpressionConfigRuntime>(_AssetName: "FacialExpressConfigExec");//New Controller
                //Debug.Log("SaveFacialTrackingController(): CurrentControllerNew is null :"+ _ControllerNew.name);
            }
            else
            {
                _ControllerNew = CurrentControllerNew;
                _ControllerNewRuntime = CurrentControllerNewRuntime;
            }
            //_ControllerNew = graph;

            if (_ControllerNew == null) return;
            EditorUtility.SetDirty(graph);
            EditorUtility.SetDirty(_ControllerNew);
            //AssetDatabase.SaveAssets();
            //AssetDatabase.Refresh();
            //EditorUtility.FocusProjectWindow();
            EditorUtility.SetDirty(graphRuntime);
            EditorUtility.SetDirty(_ControllerNewRuntime);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();

            if (isSaveAs)
            {
                _ControllerNew.nodes = graph.nodes;
                _ControllerNew.edges = graph.edges;
                _ControllerNew.position = graph.position;
                _ControllerNewRuntime.nodes = graphRuntime.nodes;
                _ControllerNewRuntime.edges = graphRuntime.edges;
                _ControllerNewRuntime.position = graphRuntime.position;
            }

            if (TokenForController.value != _ControllerNew || CurrentControllerNew != _ControllerNew)
            {
                TokenForController.value = _ControllerNew;
                CurrentControllerNew = _ControllerNew;

                TokenForControllerRuntime.value = _ControllerNewRuntime;
                CurrentControllerNewRuntime = _ControllerNewRuntime;
                //Debug.Log("SaveFacialTrackingController(): Assign new config "+ CurrentControllerNew.name);
            }
        }
        static string fileExec = "";
        public T CreateAsset<T>(string _AssetName = "new BaseGraph", string _Path = "FacialExpressionMakerAsset") where T : ScriptableObject
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
                "FacialExpressionConfig.asset", //Default Asset file name
                "asset"
            );

            Debug.Log("CreateAsset() "+ absoluteFilePath);
            //if (!string.IsNullOrEmpty(mappingconfigFile))
            if (!string.IsNullOrEmpty(absoluteFilePath))
            {
                //Debug.Log("CreateAsset() mappingconfigFile:" + mappingconfigFile);
                string relativePath = absoluteFilePath.Replace(Application.dataPath, "Assets");
                Debug.Log("CreateAsset() " + relativePath); //Assets/FacialExpressionConfig1.asset
                //AssetDatabase.CreateAsset(graph, relativePath);
                if (isSaveAs)
                {
                    AssetDatabase.CreateAsset(_Asset, relativePath);
                }
                else
                ProjectWindowUtil.CreateAsset(graph, relativePath);
                EditorUtility.SetDirty(_Asset);
                fileExec = relativePath.Replace(".asset", "Exec.asset");
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
        public T CreateAssetRuntime<T>(string _AssetName = "new BaseGraph", string _Path = "FacialExpressionMakerAsset") where T : ScriptableObject
        {
            T _Asset = ScriptableObject.CreateInstance<T>();
            //if (!AssetDatabase.IsValidFolder($"Assets/{_Path}"))
            //{
            //	AssetDatabase.CreateFolder("Assets", _Path);
            //}

            //// show save file dialog and let user set the file name and path
            //string absoluteFilePath = EditorUtility.SaveFilePanel(
            //    "Save Asset", //Dialog Title
            //    "Assets", //Parent folder name
            //    "FacialExpressionConfigExec.asset", //Default Asset file name
            //    "asset"
            //);

            //Debug.Log("CreateAsset() " + absoluteFilePath);
            //if (!string.IsNullOrEmpty(absoluteFilePath))
            {
                //string relativePath = absoluteFilePath.Replace(Application.dataPath, "Assets");
                Debug.Log("CreateAsset() " + fileExec);
                //AssetDatabase.CreateAsset(graph, relativePath);
                if (isSaveAs)
                {
                    AssetDatabase.CreateAsset(_Asset, fileExec);
                }
                else
                    ProjectWindowUtil.CreateAsset(graphRuntime, fileExec);
                EditorUtility.SetDirty(_Asset);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            //else
            //    return null;

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = _Asset;
            return _Asset;
        }



		protected abstract void InitializeWindow(FacialExpressionConfig graph);
		//protected virtual void InitializeGraphView(FacialTrackingGraphView view) { Debug.Log("BaseGraphWindow InitializeGraphView()"); }
		protected abstract void InitializeOriginalExpression(FacialExpressionConfig graph);
		protected abstract void InitializeCustomExpression(FacialExpressionConfig graph);
		//protected abstract void SaveFacialTrackingController(FacialExpressionConfig graph);
	}
}
#endif