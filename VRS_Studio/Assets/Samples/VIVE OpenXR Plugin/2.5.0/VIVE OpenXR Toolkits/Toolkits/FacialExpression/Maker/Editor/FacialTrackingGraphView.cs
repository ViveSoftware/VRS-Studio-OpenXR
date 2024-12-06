//using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Linq;
using System;
using System.Reflection;

#if UNITY_EDITOR
//using Status = UnityEngine.UIElements.DropdownMenuAction.Status;
//using Object = UnityEngine.Object;
//using static UnityEngine.RectTransform;
using UnityEditor.Experimental.GraphView;
//using UnityEditor.UIElements;
using UnityEditor.SceneManagement;

namespace VIVE.OpenXR.Toolkits.FacialExpression.Maker.Editor
{
    [System.Serializable]
    public class CopyPasteHelper
    {
        public List<JsonElement> copiedNodes = new List<JsonElement>();
        //public List<JsonElement> copiedGroups = new List<JsonElement>();
        public List<JsonElement> copiedEdges = new List<JsonElement>();
    }


    /// <summary>
    /// Base class to write a custom view for a node
    /// </summary>
    public class FacialTrackingGraphView : GraphView, IDisposable
    {
        public delegate void ComputeOrderUpdatedDelegate();
        public delegate void NodeDuplicatedDelegate(FacialTrackingNode duplicatedNode, FacialTrackingNode newNode);

        /// <summary>
        /// Graph that owns of the node
        /// </summary>
        public FacialExpressionConfig graph;

        /// <summary>
        /// Connector listener that will create the edges between ports
        /// </summary>
        public FacialExpressionEdgeConnectorListener connectorListener;

        /// <summary>
        /// List of all node views in the graph
        /// </summary>
        /// <typeparam name="BaseNodeView"></typeparam>
        /// <returns></returns>
        public List<FacialTrackingGraphNode> nodeViews = new List<FacialTrackingGraphNode>();

        /// <summary>
        /// Dictionary of the node views accessed view the node instance, faster than a Find in the node view list
        /// </summary>
        /// <typeparam name="BaseNode"></typeparam>
        /// <typeparam name="BaseNodeView"></typeparam>
        /// <returns></returns>
        public Dictionary<FacialTrackingNode, FacialTrackingGraphNode> nodeViewsPerNode = new Dictionary<FacialTrackingNode, FacialTrackingGraphNode>();

        /// <summary>
        /// List of all edge views in the graph
        /// </summary>
        /// <typeparam name="EdgeView"></typeparam>
        /// <returns></returns>
        public List<EdgeView> edgeViews = new List<EdgeView>();


        //CreateNodeMenuWindow createNodeMenu;

        /// <summary>
        /// Triggered just after the graph is initialized
        /// </summary>
        public event Action initialized;

        /// <summary>
        /// Triggered just after the compute order of the graph is updated
        /// </summary>
        //public event ComputeOrderUpdatedDelegate computeOrderUpdated;

        // Safe event relay from BaseGraph (safe because you are sure to always point on a valid BaseGraph
        // when one of these events is called), a graph switch can occur between two call tho
        /// <summary>
        /// Same event than BaseGraph.onExposedParameterListChanged
        /// Safe event (not triggered in case the graph is null).
        /// </summary>
        public event Action onExposedParameterListChanged;

        /// <summary>
        /// Same event than BaseGraph.onExposedParameterModified
        /// Safe event (not triggered in case the graph is null).
        /// </summary>
        //public event Action< ExposedParameter >	onExposedParameterModified;

        /// <summary>
        /// Triggered when a node is duplicated (crt-d) or copy-pasted (crtl-c/crtl-v)
        /// </summary>
        public event NodeDuplicatedDelegate nodeDuplicated;

        ///// <summary>
        ///// Object to handle nodes that shows their UI in the inspector.
        ///// </summary>
        //[SerializeField]
        //protected NodeInspectorObject nodeInspector
        //{
        //    get
        //    {

        //        if (graph.nodeInspectorReference == null)
        //            graph.nodeInspectorReference = CreateNodeInspectorObject();
        //        return graph.nodeInspectorReference as NodeInspectorObject;
        //    }
        //}

        public SerializedObject serializedGraph { get; private set; }

        Dictionary<Type, (Type nodeType, MethodInfo initalizeNodeFromObject)> nodeTypePerCreateAssetType = new Dictionary<Type, (Type, MethodInfo)>();

        //List<OriginalStatePortData> ListofOriginalStateData = new List<OriginalStatePortData>();


        public FacialTrackingGraphView(EditorWindow window)
        {
            //// 允许缩放和拖動
            //this.SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            //this.AddManipulator(new ContentDragger());
            //this.AddManipulator(new SelectionDragger());
            //this.AddManipulator(new RectangleSelector());
            //// 設置内容區域，使其足够大以顯示滚动條
            //var contentSize = new Vector2(2000, 2000); // 调整这个大小以適應你需要的内容
            //contentContainer.style.width = contentSize.x;
            //contentContainer.style.height = contentSize.y;
            //// 添加背景網格
            //this.Insert(0, new GridBackground());


            serializeGraphElements = SerializeGraphElementsCallback;
            canPasteSerializedData = CanPasteSerializedDataCallback;
            unserializeAndPaste = UnserializeAndPasteCallback;
            graphViewChanged = GraphViewChangedCallback;
            viewTransformChanged = ViewTransformChangedCallback;
            elementResized = ElementResizedCallback;

            RegisterCallback<KeyDownEvent>(KeyDownCallback);
            RegisterCallback<DragPerformEvent>(DragPerformedCallback);
            RegisterCallback<DragUpdatedEvent>(DragUpdatedCallback);
            RegisterCallback<MouseDownEvent>(MouseDownCallback);
            RegisterCallback<MouseUpEvent>(MouseUpCallback);

            InitializeManipulators();
            Insert(0, new GridBackground());

            SetupZoom(0.05f, 2f);

            Undo.undoRedoPerformed += ReloadView;

            //createNodeMenu = ScriptableObject.CreateInstance<CreateNodeMenuWindow>();
            //createNodeMenu.Initialize(this, window);

            this.StretchToParentSize();
        }

        //protected virtual NodeInspectorObject CreateNodeInspectorObject()
        //{
        //    var inspector = ScriptableObject.CreateInstance<NodeInspectorObject>();
        //    inspector.name = "Node Inspector";
        //    inspector.hideFlags = HideFlags.HideAndDontSave ^ HideFlags.NotEditable;

        //    return inspector;
        //}

        #region Callbacks

        protected override bool canCopySelection
        {
            get { return selection.Any(e => e is FacialTrackingGraphNode); }
        }

        protected override bool canCutSelection
        {
            get { return selection.Any(e => e is FacialTrackingGraphNode); }
        }

        string SerializeGraphElementsCallback(IEnumerable<GraphElement> elements)
        {
            //Debug.Log("BaseGraphView SerializeGraphElementsCallback() " );
            var data = new CopyPasteHelper();

            foreach (FacialTrackingGraphNode nodeView in elements.Where(e => e is FacialTrackingGraphNode))
            {
                data.copiedNodes.Add(JsonSerializer.SerializeNode(nodeView.nodeTarget));
                foreach (var port in nodeView.nodeTarget.GetAllPorts())
                {
                    if (port.portData.vertical)
                    {
                        foreach (var edge in port.GetEdges())
                            data.copiedEdges.Add(JsonSerializer.Serialize(edge));
                    }
                }
            }

            //foreach (GroupView groupView in elements.Where(e => e is GroupView))
            //	data.copiedGroups.Add(JsonSerializer.Serialize(groupView.group));
            foreach (EdgeView edgeView in elements.Where(e => e is EdgeView))
                data.copiedEdges.Add(JsonSerializer.Serialize(edgeView.serializedEdge));

            ClearSelection();

            return JsonUtility.ToJson(data, true);
            return "";
        }

        bool CanPasteSerializedDataCallback(string serializedData)
        {
            //Debug.Log("CanPasteSerializedDataCallback() data:" + serializedData);
            //try
            //{
            //    return JsonUtility.FromJson(serializedData, typeof(CopyPasteHelper)) != null;
            //}
            //catch
            //{
            //    return false;
            //}
            return false;
        }

        void UnserializeAndPasteCallback(string operationName, string serializedData)
        {
            //Debug.Log("UnserializeAndPasteCallback() data:"+ serializedData);
            //if(serializedData == null) Debug.Log("UnserializeAndPasteCallback serializedData is null");
            var data = JsonUtility.FromJson<CopyPasteHelper>(serializedData);

            RegisterCompleteObjectUndo(operationName);

            Dictionary<string, FacialTrackingNode> copiedNodesMap = new Dictionary<string, FacialTrackingNode>();

            //var unserializedGroups = data.copiedGroups.Select(g => JsonSerializer.Deserialize<Group>(g)).ToList();

            foreach (var serializedNode in data.copiedNodes)
            {
                var node = JsonSerializer.DeserializeNode(serializedNode);

                if (node == null)
                    continue;

                string sourceGUID = node.GUID;
                graph.nodesPerGUID.TryGetValue(sourceGUID, out var sourceNode);
                //Call OnNodeCreated on the new fresh copied node
                node.createdFromDuplication = true;
                //node.createdWithinGroup = unserializedGroups.Any(g => g.innerNodeGUIDs.Contains(sourceGUID));
                node.OnNodeCreated();
                //And move a bit the new node
                node.position.position += new Vector2(20, 20);

                var newNodeView = AddNode(node);

                // If the nodes were copied from another graph, then the source is null
                if (sourceNode != null)
                    nodeDuplicated?.Invoke(sourceNode, node);
                copiedNodesMap[sourceGUID] = node;

                //Select the new node
                AddToSelection(nodeViewsPerNode[node]);
            }


            foreach (var serializedEdge in data.copiedEdges)
            {
                var edge = JsonSerializer.Deserialize<FacialExpressionSerializableEdge>(serializedEdge);

                edge.Deserialize();

                // Find port of new nodes:
                copiedNodesMap.TryGetValue(edge.inputNode.GUID, out var oldInputNode);
                copiedNodesMap.TryGetValue(edge.outputNode.GUID, out var oldOutputNode);

                // We avoid to break the graph by replacing unique connections:
                if (oldInputNode == null && !edge.inputPort.portData.acceptMultipleEdges || !edge.outputPort.portData.acceptMultipleEdges)
                    continue;

                oldInputNode = oldInputNode ?? edge.inputNode;
                oldOutputNode = oldOutputNode ?? edge.outputNode;

                var inputPort = oldInputNode.GetPort(edge.inputPort.fieldName, edge.inputPortIdentifier);
                var outputPort = oldOutputNode.GetPort(edge.outputPort.fieldName, edge.outputPortIdentifier);

                var newEdge = FacialExpressionSerializableEdge.CreateNewEdge(graph, inputPort, outputPort);

                if (nodeViewsPerNode.ContainsKey(oldInputNode) && nodeViewsPerNode.ContainsKey(oldOutputNode))
                {
                    var edgeView = CreateEdgeView();
                    edgeView.userData = newEdge;
                    edgeView.input = nodeViewsPerNode[oldInputNode].GetPortViewFromFieldName(newEdge.inputFieldName, ""/*newEdge.inputPortIdentifier*/);
                    edgeView.output = nodeViewsPerNode[oldOutputNode].GetPortViewFromFieldName(""/*newEdge.outputFieldName*/, newEdge.outputPortIdentifier);

                    Connect(edgeView);
                }
            }
        }

        public virtual EdgeView CreateEdgeView()
        {
            return new EdgeView();
        }

        GraphViewChange GraphViewChangedCallback(GraphViewChange changes)
        {
            if (changes.elementsToRemove != null)
            {
                RegisterCompleteObjectUndo("Remove Graph Elements");

                // Destroy priority of objects
                // We need nodes to be destroyed first because we can have a destroy operation that uses node connections
                changes.elementsToRemove.Sort((e1, e2) => {
                    int GetPriority(GraphElement e)
                    {
                        if (e is FacialTrackingGraphNode)
                            return 0;
                        else
                            return 1;
                    }
                    return GetPriority(e1).CompareTo(GetPriority(e2));
                });

                //Handle ourselves the edge and node remove
                changes.elementsToRemove.RemoveAll(e => {
                    //Debug.Log("remove all edge or nodeView");
                    switch (e)
                    {
                        case EdgeView edge:
                            Disconnect(edge);
                            return true;
                        case FacialTrackingGraphNode nodeView:
                            // For vertical nodes, we need to delete them ourselves as it's not handled by GraphView
                            foreach (var pv in nodeView.inputPortViews.Concat(nodeView.outputPortViews))
                                if (pv.orientation == Orientation.Vertical)
                                    foreach (var edge in pv.GetEdges().ToList())
                                        Disconnect(edge);

                            //nodeInspector.NodeViewRemoved(nodeView);
                            //ExceptionToLog.Call(() => nodeView.OnRemoved());
                            graph.RemoveNode(nodeView.nodeTarget);
                            UpdateSerializedProperties();
                            RemoveElement(nodeView);
                            //if (Selection.activeObject == nodeInspector)
                                //UpdateNodeInspectorSelection();

                            SyncSerializedPropertyPathes();
                            return true;
                    }

                    return false;
                });
            }

            return changes;
        }

        void GraphChangesCallback(GraphChanges changes)
        {
            if (changes.removedEdge != null)
            {
                var edge = edgeViews.FirstOrDefault(e => e.serializedEdge == changes.removedEdge);

                DisconnectView(edge);
            }
        }

        void ViewTransformChangedCallback(GraphView view)
        {
            if (graph != null)
            {
                graph.position = viewTransform.position;
                graph.scale = viewTransform.scale;
            }
        }

        void ElementResizedCallback(VisualElement elem)
        {
            //var groupView = elem as GroupView;

            //if (groupView != null)
            //    groupView.group.size = groupView.GetPosition().size;
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            //Debug.Log("BaseGraphView GetCompatiblePorts() ");
            var compatiblePorts = new List<Port>();

            compatiblePorts.AddRange(ports.ToList().Where(p => {
                var portView = p as FacialExpressionPortView;

                if (portView.owner == (startPort as FacialExpressionPortView).owner)
                    return false;

                if (p.direction == startPort.direction)
                    return false;

                //Check for type assignability
                if (!FacialExpressionConfig.TypesAreConnectable(startPort.portType, p.portType))
                    return false;

                //Check if the edge already exists
                if (portView.GetEdges().Any(e => e.input == startPort || e.output == startPort))
                    return false;

                return true;
            }));

            return compatiblePorts;
        }

        /// <summary>
        /// Build the contextual menu shown when right clicking inside the graph view
        /// </summary>
        /// <param name="evt"></param>
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);
            BuildViewContextualMenu(evt);
            BuildSelectAssetContextualMenu(evt);
            //BuildSaveAssetContextualMenu(evt);

        }

        /// <summary>
        /// Add the View entry to the context menu
        /// </summary>
        /// <param name="evt"></param>
        protected virtual void BuildViewContextualMenu(ContextualMenuPopulateEvent evt)
        {
            //evt.menu.AppendAction("View/Processor", (e) => ToggleView< ProcessorView >(), (e) => GetPinnedElementStatus< ProcessorView >());
        }

        /// <summary>
        /// Add the Select Asset entry to the context menu
        /// </summary>
        /// <param name="evt"></param>
        protected virtual void BuildSelectAssetContextualMenu(ContextualMenuPopulateEvent evt)
        {
            //evt.menu.AppendAction("Select Asset", (e) => EditorGUIUtility.PingObject(graph), DropdownMenuAction.AlwaysEnabled);
        }

        /// <summary>
        /// Add the Save Asset entry to the context menu
        /// </summary>
        /// <param name="evt"></param>
        protected virtual void BuildSaveAssetContextualMenu(ContextualMenuPopulateEvent evt)
        {
            //Debug.Log("BuildSaveAssetContextualMenu() right button save asset");
            evt.menu.AppendAction("Save Asset", (e) => {
                EditorUtility.SetDirty(graph);
                AssetDatabase.SaveAssets();
            }, DropdownMenuAction.AlwaysEnabled);
        }

        /// <summary>
        /// Add the Help entry to the context menu
        /// </summary>
        /// <param name="evt"></param>


        protected virtual void KeyDownCallback(KeyDownEvent e)
        {
            if (e.keyCode == KeyCode.S && e.commandKey)
            {
                SaveGraphToDisk();
                e.StopPropagation();
            }
            else if (nodeViews.Count > 0 && e.commandKey && e.altKey)
            {
                //	Node Aligning shortcuts
                switch (e.keyCode)
                {
                    //case KeyCode.LeftArrow:
                    //    nodeViews[0].AlignToLeft();
                    //    e.StopPropagation();
                    //    break;
                    //case KeyCode.RightArrow:
                    //    nodeViews[0].AlignToRight();
                    //    e.StopPropagation();
                    //    break;
                    //case KeyCode.UpArrow:
                    //    nodeViews[0].AlignToTop();
                    //    e.StopPropagation();
                    //    break;
                    //case KeyCode.DownArrow:
                    //    nodeViews[0].AlignToBottom();
                    //    e.StopPropagation();
                    //    break;
                    //case KeyCode.C:
                    //    nodeViews[0].AlignToCenter();
                    //    e.StopPropagation();
                    //    break;
                    //case KeyCode.M:
                    //    nodeViews[0].AlignToMiddle();
                    //    e.StopPropagation();
                    //    break;
                }
            }
        }

        void MouseUpCallback(MouseUpEvent e)
        {
            //schedule.Execute(() => {
            //    if (DoesSelectionContainsInspectorNodes())
            //        UpdateNodeInspectorSelection();
            //}).ExecuteLater(1);
        }

        void MouseDownCallback(MouseDownEvent e)
        {
            // When left clicking on the graph (not a node or something else)
            if (e.button == 0)
            {
                // Close all settings windows:
                //nodeViews.ForEach(v => v.CloseSettings());
            }

            //if (DoesSelectionContainsInspectorNodes())
            //    UpdateNodeInspectorSelection();
        }

        bool DoesSelectionContainsInspectorNodes()
        {
            //Debug.Log("DoesSelectionContainsInspectorNodes() ");
            var selectedNodes = selection.Where(s => s is FacialTrackingGraphNode).ToList();
            //var selectedNodesNotInInspector = selectedNodes.Except(nodeInspector.selectedNodes).ToList();
            //var nodeInInspectorWithoutSelectedNodes = nodeInspector.selectedNodes.Except(selectedNodes).ToList();

            //return selectedNodesNotInInspector.Any() || nodeInInspectorWithoutSelectedNodes.Any();
            return true;
        }

        void DragPerformedCallback(DragPerformEvent e)
        {
            var mousePos = (e.currentTarget as VisualElement).ChangeCoordinatesTo(contentViewContainer, e.localMousePosition);
            var dragData = DragAndDrop.GetGenericData("DragSelection") as List<ISelectable>;

            // Drag and Drop for elements inside the graph
            if (dragData != null)
            {
                //var exposedParameterFieldViews = dragData.OfType<ExposedParameterFieldView>();
                //if (exposedParameterFieldViews.Any())
                //{
                //	foreach (var paramFieldView in exposedParameterFieldViews)
                //	{
                //		RegisterCompleteObjectUndo("Create Parameter Node");
                //		var paramNode = BaseNode.CreateFromType< ParameterNode >(mousePos);
                //		paramNode.parameterGUID = paramFieldView.parameter.guid;
                //		AddNode(paramNode);
                //	}
                //}
            }

            // External objects drag and drop
            if (DragAndDrop.objectReferences.Length > 0)
            {
                RegisterCompleteObjectUndo("Create Node From Object(s)");
                foreach (var obj in DragAndDrop.objectReferences)
                {
                    var objectType = obj.GetType();

                    foreach (var kp in nodeTypePerCreateAssetType)
                    {
                        if (kp.Key.IsAssignableFrom(objectType))
                        {
                            try
                            {
                                var node = FacialTrackingNode.CreateFromType(kp.Value.nodeType, mousePos);
                                if ((bool)kp.Value.initalizeNodeFromObject.Invoke(node, new[] { obj }))
                                {
                                    AddNode(node);
                                    break;
                                }
                            }
                            catch (Exception exception)
                            {
                                Debug.LogException(exception);
                            }
                        }
                    }
                }
            }
        }

        void DragUpdatedCallback(DragUpdatedEvent e)
        {
            var dragData = DragAndDrop.GetGenericData("DragSelection") as List<ISelectable>;
            var dragObjects = DragAndDrop.objectReferences;
            bool dragging = false;

            if (dragData != null)
            {
                //Handle drag from exposed parameter view
                //if (dragData.OfType<ExposedParameterFieldView>().Any())
                //{
                //    dragging = true;
                //}
            }

            if (dragObjects.Length > 0)
                dragging = true;

            if (dragging)
                DragAndDrop.visualMode = DragAndDropVisualMode.Generic;

            //UpdateNodeInspectorSelection();
        }

        #endregion

        #region Initialization

        void ReloadView()
        {
            // Force the graph to reload his data (Undo have updated the serialized properties of the graph
            // so the one that are not serialized need to be synchronized)
            graph.Deserialize();

            // Get selected nodes
            var selectedNodeGUIDs = new List<string>();
            foreach (var e in selection)
            {
                if (e is FacialTrackingGraphNode v && this.Contains(v))
                    selectedNodeGUIDs.Add(v.nodeTarget.GUID);
            }

            // Remove everything
            RemoveNodeViews();
            RemoveEdges();


            UpdateSerializedProperties();

            // And re-add with new up to date datas
            InitializeNodeViews();
            InitializeEdgeViews();
            //InitializeGroups();
            //InitializeStickyNotes();
            //InitializeStackNodes();

            Reload();

            //UpdateComputeOrder();

            // Restore selection after re-creating all views
            // selection = nodeViews.Where(v => selectedNodeGUIDs.Contains(v.nodeTarget.GUID)).Select(v => v as ISelectable).ToList();
            foreach (var guid in selectedNodeGUIDs)
            {
                AddToSelection(nodeViews.FirstOrDefault(n => n.nodeTarget.GUID == guid));
            }

            //UpdateNodeInspectorSelection();
        }

        public void Initialize(FacialExpressionConfig graph)
        {
            if (this.graph != null)
            {
                SaveGraphToDisk();
                // Close pinned windows from old graph:
                ClearGraphElements();
                //NodeProvider.UnloadGraph(graph);
            }

            this.graph = graph;

            //exposedParameterFactory = new ExposedParameterFieldFactory(graph);

            UpdateSerializedProperties();

            connectorListener = CreateEdgeConnectorListener();

            // When pressing ctrl-s, we save the graph
            EditorSceneManager.sceneSaved += _ => SaveGraphToDisk();
            RegisterCallback<KeyDownEvent>(e => {
                if (e.keyCode == KeyCode.S && e.actionKey)
                    SaveGraphToDisk();
            });

            ClearGraphElements();

            InitializeGraphView();
            InitializeNodeViews();
            InitializeEdgeViews();
            InitializeViews();


            initialized?.Invoke();
            UpdateComputeOrder();

            InitializeView();

            //NodeProvider.LoadGraph(graph);

            // Register the nodes that can be created from assets
            //foreach (var nodeInfo in NodeProvider.GetNodeMenuEntries(graph))
            //{
            //	var interfaces = nodeInfo.type.GetInterfaces();
            //             var exceptInheritedInterfaces = interfaces.Except(interfaces.SelectMany(t => t.GetInterfaces()));
            //	foreach (var i in interfaces)
            //	{
            //		if (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICreateNodeFrom<>))
            //		{
            //			var genericArgumentType = i.GetGenericArguments()[0];
            //			var initializeFunction = nodeInfo.type.GetMethod(
            //				nameof(ICreateNodeFrom<Object>.InitializeNodeFromObject),
            //				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
            //				null, new Type[]{ genericArgumentType}, null
            //			);
            //			Debug.Log("BaseGraphView Initialize() call InitializeNodeFromObject");

            //			// We only add the type that implements the interface, not it's children
            //			if (initializeFunction.DeclaringType == nodeInfo.type)
            //				nodeTypePerCreateAssetType[genericArgumentType] = (nodeInfo.type, initializeFunction);
            //		}
            //	}
            //}
        }

        public void ClearGraphElements()
        {
            RemoveNodeViews();
            RemoveEdges();
        }

        void UpdateSerializedProperties()
        {
            serializedGraph = new SerializedObject(graph);
        }

        /// <summary>
        /// Allow you to create your own edge connector listener
        /// </summary>
        /// <returns></returns>
        protected virtual FacialExpressionEdgeConnectorListener CreateEdgeConnectorListener()
         => new FacialExpressionEdgeConnectorListener(this);

        void InitializeGraphView()
        {
            //graph.onExposedParameterListChanged += OnExposedParameterListChanged;
            //graph.onExposedParameterModified += (s) => onExposedParameterModified?.Invoke(s);
            graph.onGraphChanges += GraphChangesCallback;
            viewTransform.position = graph.position;
            viewTransform.scale = graph.scale;

            //nodeCreationRequest = (c) => SearchWindow.Open(new SearchWindowContext(c.screenMousePosition), createNodeMenu);
        }

        void OnExposedParameterListChanged()
        {
            UpdateSerializedProperties();
            onExposedParameterListChanged?.Invoke();
        }

        void InitializeNodeViews()
        {
            graph.nodes.RemoveAll(n => n == null);

            foreach (var node in graph.nodes)
            {
                var v = AddNodeView(node);
            }
        }

        void InitializeEdgeViews()
        {
            // Sanitize edges in case a node broke something while loading
            graph.edges.RemoveAll(edge => edge == null || edge.inputNode == null || edge.outputNode == null);

            foreach (var serializedEdge in graph.edges)
            {
                nodeViewsPerNode.TryGetValue(serializedEdge.inputNode, out var inputNodeView);
                nodeViewsPerNode.TryGetValue(serializedEdge.outputNode, out var outputNodeView);
                if (inputNodeView == null || outputNodeView == null)
                    continue;

                var edgeView = CreateEdgeView();
                edgeView.userData = serializedEdge;
                edgeView.input = inputNodeView.GetPortViewFromFieldName(serializedEdge.inputFieldName, serializedEdge.inputPortIdentifier);
                edgeView.output = outputNodeView.GetPortViewFromFieldName(serializedEdge.outputFieldName, serializedEdge.outputPortIdentifier);


                ConnectView(edgeView);
            }
        }

        void InitializeViews()
        {

        }

        protected virtual void InitializeManipulators()
        {
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
        }

        protected virtual void Reload() { }

        #endregion

        #region Graph content modification

        public void UpdateNodeInspectorSelection()
        {
            return;
            //if (nodeInspector.previouslySelectedObject != Selection.activeObject)
            //    nodeInspector.previouslySelectedObject = Selection.activeObject;

            //HashSet<BaseNodeView> selectedNodeViews = new HashSet<BaseNodeView>();
            //nodeInspector.selectedNodes.Clear();
            //foreach (var e in selection)
            //{
            //    if (e is BaseNodeView v && this.Contains(v))//&& v.nodeTarget.needsInspector
            //        selectedNodeViews.Add(v);
            //}

            //nodeInspector.UpdateSelectedNodes(selectedNodeViews);
            //if (Selection.activeObject != nodeInspector && selectedNodeViews.Count > 0)
            //    Selection.activeObject = nodeInspector;
        }

        public FacialTrackingGraphNode AddNode(FacialTrackingNode node)
        {
            // This will initialize the node using the graph instance
            graph.AddNode(node);

            UpdateSerializedProperties();

            var view = AddNodeView(node);

            // Call create after the node have been initialized
            //view.OnCreated();//ExceptionToLog.Call(() => view.OnCreated());


            //UpdateComputeOrder();

            return view;
        }

        public FacialTrackingGraphNode AddNodeView(FacialTrackingNode node)
        {
            //Debug.Log("BaseNodeView AddNodeView() must call NodeProvider.GetNodeViewTypeFromType()"+ nodeViews.Count);
            var viewType = NodeProvider.GetNodeViewTypeFromType(node.GetType());

            if (viewType == null)
                viewType = typeof(FacialTrackingGraphNode);

            var baseNodeView = Activator.CreateInstance(viewType) as FacialTrackingGraphNode;
            baseNodeView.Initialize(this, node);
            AddElement(baseNodeView);

            nodeViews.Add(baseNodeView);
            nodeViewsPerNode[node] = baseNodeView;

            return baseNodeView;
        }

        public void RemoveNode(FacialTrackingNode node)
        {
            var view = nodeViewsPerNode[node];
            RemoveNodeView(view);
            graph.RemoveNode(node);
        }

        public void RemoveNodeView(FacialTrackingGraphNode nodeView)
        {
            RemoveElement(nodeView);
            nodeViews.Remove(nodeView);
            nodeViewsPerNode.Remove(nodeView.nodeTarget);
        }

        void RemoveNodeViews()
        {
            foreach (var nodeView in nodeViews)
                RemoveElement(nodeView);
            nodeViews.Clear();
            nodeViewsPerNode.Clear();
        }

        public bool CanConnectEdge(EdgeView e, bool autoDisconnectInputs = true)
        {
            if (e.input == null || e.output == null)
                return false;

            var inputPortView = e.input as FacialExpressionPortView;
            var outputPortView = e.output as FacialExpressionPortView;
            var inputNodeView = inputPortView.node as FacialTrackingGraphNode;
            var outputNodeView = outputPortView.node as FacialTrackingGraphNode;

            if (inputNodeView == null || outputNodeView == null)
            {
                Debug.Log("Connect aborted !");
                return false;
            }

            return true;
        }

        public bool ConnectView(EdgeView e, bool autoDisconnectInputs = true)
        {
            if (!CanConnectEdge(e, autoDisconnectInputs))
                return false;

            var inputPortView = e.input as FacialExpressionPortView;
            var outputPortView = e.output as FacialExpressionPortView;
            var inputNodeView = inputPortView.node as FacialTrackingGraphNode;
            var outputNodeView = outputPortView.node as FacialTrackingGraphNode;

            //If the input port does not support multi-connection, we remove them
            if (autoDisconnectInputs && !(e.input as FacialExpressionPortView).portData.acceptMultipleEdges)
            {
                foreach (var edge in edgeViews.Where(ev => ev.input == e.input).ToList())
                {
                    // TODO: do not disconnect them if the connected port is the same than the old connected
                    DisconnectView(edge);
                }
            }
            // same for the output port:
            if (autoDisconnectInputs && !(e.output as FacialExpressionPortView).portData.acceptMultipleEdges)
            {
                foreach (var edge in edgeViews.Where(ev => ev.output == e.output).ToList())
                {
                    // TODO: do not disconnect them if the connected port is the same than the old connected
                    DisconnectView(edge);
                }
            }

            AddElement(e);

            e.input.Connect(e);
            e.output.Connect(e);

            // If the input port have been removed by the custom port behavior
            // we try to find if it's still here
            if (e.input == null)
                e.input = inputNodeView.GetPortViewFromFieldName(inputPortView.fieldName, inputPortView.portData.identifier);
            if (e.output == null)
                e.output = inputNodeView.GetPortViewFromFieldName(outputPortView.fieldName, outputPortView.portData.identifier);

            edgeViews.Add(e);

            inputNodeView.RefreshPorts();
            outputNodeView.RefreshPorts();

            // In certain cases the edge color is wrong so we patch it
            schedule.Execute(() => {
                e.UpdateEdgeControl();
            }).ExecuteLater(1);

            e.isConnected = true;

            return true;
        }

        public bool Connect(FacialExpressionPortView inputPortView, FacialExpressionPortView outputPortView, bool autoDisconnectInputs = true)
        {
            //Debug.Log("Maker BaseGraphView Connect() 3");
            var inputPort = inputPortView.owner.nodeTarget.GetPort(inputPortView.fieldName, inputPortView.portData.identifier);
            var outputPort = outputPortView.owner.nodeTarget.GetPort(outputPortView.fieldName, outputPortView.portData.identifier);

            // Checks that the node we are connecting still exists
            if (inputPortView.owner.parent == null || outputPortView.owner.parent == null)
                return false;

            var newEdge = FacialExpressionSerializableEdge.CreateNewEdge(graph, inputPort, outputPort);

            var edgeView = CreateEdgeView();
            edgeView.userData = newEdge;
            edgeView.input = inputPortView;
            edgeView.output = outputPortView;


            return Connect(edgeView);
        }

        public bool Connect(EdgeView e, bool autoDisconnectInputs = true)
        {
            //Debug.Log("Maker BaseGraphView Connect() 2"); //connect edge callback
            if (!CanConnectEdge(e, autoDisconnectInputs))
                return false;

            var inputPortView = e.input as FacialExpressionPortView;
            var outputPortView = e.output as FacialExpressionPortView;
            var inputNodeView = inputPortView.node as FacialTrackingGraphNode;
            var outputNodeView = outputPortView.node as FacialTrackingGraphNode;
            var inputPort = inputNodeView.nodeTarget.GetPort(inputPortView.fieldName, inputPortView.portData.identifier);
            var outputPort = outputNodeView.nodeTarget.GetPort(outputPortView.fieldName, outputPortView.portData.identifier);

            e.userData = graph.Connect(inputPort, outputPort, autoDisconnectInputs);

            ConnectView(e, autoDisconnectInputs);

            UpdateComputeOrder();

            return true;
        }

        public void DisconnectView(EdgeView e, bool refreshPorts = true)
        {
            if (e == null)
                return;

            RemoveElement(e);

            if (e?.input?.node is FacialTrackingGraphNode inputNodeView)
            {
                e.input.Disconnect(e);
                if (refreshPorts)
                    inputNodeView.RefreshPorts();
            }
            if (e?.output?.node is FacialTrackingGraphNode outputNodeView)
            {
                e.output.Disconnect(e);
                if (refreshPorts)
                    outputNodeView.RefreshPorts();
            }

            edgeViews.Remove(e);
        }

        public void Disconnect(EdgeView e, bool refreshPorts = true)
        {
            // Remove the serialized edge if there is one
            if (e.userData is FacialExpressionSerializableEdge serializableEdge)
                graph.Disconnect(serializableEdge.GUID);

            DisconnectView(e, refreshPorts);

            UpdateComputeOrder();
        }

        public void RemoveEdges()
        {
            foreach (var edge in edgeViews)
                RemoveElement(edge);
            edgeViews.Clear();
        }

        public void UpdateComputeOrder()
        {
            //graph.UpdateComputeOrder();

            //computeOrderUpdated?.Invoke();
        }

        public void RegisterCompleteObjectUndo(string name)
        {
            Undo.RegisterCompleteObjectUndo(graph, name);
        }

        public void SaveGraphToDisk()
        {
            if (graph == null)
            {
                Debug.Log("SaveGraphToDisk() graph is null");
                return;
            }

            //if (graph.edges.Count > 0)
            //{
            //    graph.edgesSaved.Clear();
            //    //foreach (var edge in graph.edges) { graph.edgesSaved.Add(edge); }
            //    foreach (var e in graph.edges) { graph.edgesSaved.Add(SerializableEdgeData.SerializableEdgeDataNew(e, graph)); }
            //    Debug.Log("BaseGraph SaveGraphToDisk() edgeSaved**:" + graph.edgesSaved.Count);
            //}

            EditorUtility.SetDirty(graph);
        }



        public void ResetPositionAndZoom()
        {
            //Debug.Log("ResetPositionAndZoom()"); //function non-use
            graph.position = Vector3.zero;
            graph.scale = Vector3.one;

            UpdateViewTransform(graph.position, graph.scale);
        }

        /// <summary>
        /// Deletes the selected content, can be called form an IMGUI container
        /// </summary>
        public void DelayedDeleteSelection() => this.schedule.Execute(() => DeleteSelectionOperation("Delete", AskUser.DontAskUser)).ExecuteLater(0);

        protected virtual void InitializeView() { }

        public virtual IEnumerable<(string path, Type type)> FilterCreateNodeMenuEntries()
        {
            // By default we don't filter anything
            foreach (var nodeMenuItem in NodeProvider.GetNodeMenuEntries(graph))
                yield return nodeMenuItem;

            // TODO: add exposed properties to this list
        }

        /// <summary>
        /// Update all the serialized property bindings (in case a node was deleted / added, the property pathes needs to be updated)
        /// </summary>
        public void SyncSerializedPropertyPathes()
        {
            foreach (var nodeView in nodeViews)
                nodeView.SyncSerializedPropertyPathes();
            //nodeInspector.RefreshNodes();
        }

        /// <summary>
        /// Call this function when you want to remove this view
        /// </summary>
        public void Dispose()
        {
            ClearGraphElements();
            RemoveFromHierarchy();
            Undo.undoRedoPerformed -= ReloadView;
            //Object.DestroyImmediate(nodeInspector);
            //NodeProvider.UnloadGraph(graph);
            //exposedParameterFactory.Dispose();
            //exposedParameterFactory = null;

            //graph.onExposedParameterListChanged -= OnExposedParameterListChanged;
            //graph.onExposedParameterModified += (s) => onExposedParameterModified?.Invoke(s);
            graph.onGraphChanges -= GraphChangesCallback;
        }

        #endregion



    }
}
#endif