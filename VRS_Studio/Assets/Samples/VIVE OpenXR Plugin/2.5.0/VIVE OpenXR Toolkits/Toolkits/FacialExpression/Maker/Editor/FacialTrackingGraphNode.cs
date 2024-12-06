using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using System.Reflection;
using System;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
//using Status = UnityEngine.UIElements.DropdownMenuAction.Status;
using System.IO;

#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using NodeView = UnityEditor.Experimental.GraphView.Node;
//using UnityEngine.Windows;

namespace VIVE.OpenXR.Toolkits.FacialExpression.Maker.Editor
{
    [AttributeUsage(AttributeTargets.Class)]
    public class NodeCustomEditor : Attribute
    {
        public Type nodeType;

        public NodeCustomEditor(Type nodeType)
        {
            this.nodeType = nodeType;
        }
    }

    [NodeCustomEditor(typeof(FacialTrackingNode))]
    public class FacialTrackingGraphNode : NodeView /*NodeView = UnityEditor.Experimental.GraphView.Node*/
    {
        public FacialTrackingNode nodeTarget;

        public List<FacialExpressionPortView> inputPortViews = new List<FacialExpressionPortView>();
        public List<FacialExpressionPortView> outputPortViews = new List<FacialExpressionPortView>();
        public List<OriginalStatePortData> ListofOriginalStateData = new List<OriginalStatePortData>();

        public FacialTrackingGraphView owner { private set; get; }

        protected Dictionary<string, List<FacialExpressionPortView>> portsPerFieldName = new Dictionary<string, List<FacialExpressionPortView>>();


        public VisualElement controlsContainer;
        protected VisualElement debugContainer;
        protected VisualElement rightTitleContainer;
        protected VisualElement topPortContainer;
        protected VisualElement bottomPortContainer;
        //private VisualElement inputContainerElement;
        public VisualElement inputContainerElement;

        VisualElement settings;
        //NodeSettingsView						settingsContainer;
        Button settingButton;
        TextField titleTextField;

        Label computeOrderLabel = new Label();

        public event Action<FacialExpressionPortView> onPortConnected;
        public event Action<FacialExpressionPortView> onPortDisconnected;

        protected virtual bool hasSettings { get; set; }

        public bool initializing = false; //Used for applying SetPosition on locked node at init.

        //readonly string baseNodeStyle = "BaseNodeView";

        bool settingsExpanded = false;

        [System.NonSerialized]
        List<IconBadge> badges = new List<IconBadge>();

        private List<Node> selectedNodes = new List<Node>();
        //private float selectedNodesFarLeft;
        //private float selectedNodesNearLeft;
        //private float selectedNodesFarRight;
        //private float selectedNodesNearRight;
        //private float selectedNodesFarTop;
        //private float selectedNodesNearTop;
        //private float selectedNodesFarBottom;
        //private float selectedNodesNearBottom;
        //private float selectedNodesAvgHorizontal;
        //private float selectedNodesAvgVertical;

        #region  Initialization

        public void Initialize(FacialTrackingGraphView owner, FacialTrackingNode node)
        {
            //Debug.Log("BaseNodeView Initialize()");
            nodeTarget = node;
            this.owner = owner;

            if (!node.deletable)
                capabilities &= ~Capabilities.Deletable;
            if (!node.duplicatable)
                capabilities &= ~Capabilities.Copiable;
            // Note that the Renamable capability is useless right now as it haven't been implemented in Graphview
            if (node.isRenamable)
                capabilities |= Capabilities.Renamable;

            //owner.computeOrderUpdated += ComputeOrderUpdatedCallback;
            //node.onMessageAdded += AddMessageView;
            //node.onMessageRemoved += RemoveMessageView;
            node.onPortsUpdated += a => schedule.Execute(_ => UpdatePortsForField(a)).ExecuteLater(0);

            //styleSheets.Add(Resources.Load<StyleSheet>(baseNodeStyle));

            //if (!string.IsNullOrEmpty(node.layoutStyle))
                //styleSheets.Add(Resources.Load<StyleSheet>(node.layoutStyle));

            InitializeView();
            InitializePorts();
            //InitializeDebug();

            // If the standard Enable method is still overwritten, we call it
            if (GetType().GetMethod(nameof(Enable), new Type[] { }).DeclaringType != typeof(FacialTrackingGraphNode))
                Enable(); //ExceptionToLog.Call(() => Enable());
            else
                Enable(false); //ExceptionToLog.Call(() => Enable(false));

            //InitializeSettings();

            //RefreshExpandedState();

            this.RefreshPorts();

            //RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
            //RegisterCallback<DetachFromPanelEvent>(e => ExceptionToLog.Call(Disable));
            //OnGeometryChanged(null);
        }

        void InitializePorts()
        {
            //Debug.Log("BaseNodeView InitializePorts()");
            var listener = owner.connectorListener;

            foreach (var inputPort in nodeTarget.inputPorts)
            {
                //Debug.Log("BaseNodeView InitializePorts() input:"+ inputPort.portData.displayName);
                AddPort(inputPort.fieldInfo, Direction.Input, listener, inputPort.portData, new Image());
            }

            foreach (var outputPort in nodeTarget.outputPorts)
            {   //real add port to container, TBD: wave expression port data sets
                //Debug.Log("BaseNodeView InitializePorts() output:"+ outputPort.fieldName+",  thumbnail:"+ outputPort.portData.thumbnail);
                Image _img = new Image(); //each image need new instance for act the mouse event
                _img.image = AssetDatabase.LoadAssetAtPath<Texture2D>(outputPort.portData.thumbnail);
                FacialExpressionPortView _port = AddPort(outputPort.fieldInfo, Direction.Output, listener, outputPort.portData, _img);
                ListofOriginalStateData.Add(new OriginalStatePortData(_port, _img));
            }
        }

        void InitializeView()
        {
            //Debug.Log("BaseNodeView InitializeView()");
            controlsContainer = new VisualElement { name = "controls" };
            controlsContainer.AddToClassList("NodeControls");
            mainContainer.Add(controlsContainer);


            rightTitleContainer = new VisualElement { name = "RightTitleContainer" };
            rightTitleContainer.style.alignItems = Align.FlexEnd;
            titleContainer.Add(rightTitleContainer);
            titleContainer.style.height = 24;//48 for button


            topPortContainer = new VisualElement { name = "TopPortContainer" };
            this.Insert(0, topPortContainer);

            bottomPortContainer = new VisualElement { name = "BottomPortContainer" };
            //bottomPortContainer.style.alignContent = Align.Auto;
            outputContainer.Add(bottomPortContainer);
            outputContainer.style.alignContent = Align.Auto;
            outputContainer.style.width = 122;


            //if (nodeTarget.showControlsOnHover)
            //{
            //    bool mouseOverControls = false;
            //    controlsContainer.style.display = DisplayStyle.None;
            //    RegisterCallback<MouseOverEvent>(e => {
            //        controlsContainer.style.display = DisplayStyle.Flex;
            //        mouseOverControls = true;
            //    });
            //    RegisterCallback<MouseOutEvent>(e => {
            //        var rect = GetPosition();
            //        var graphMousePosition = owner.contentViewContainer.WorldToLocal(e.mousePosition);
            //        if (rect.Contains(graphMousePosition) || !nodeTarget.showControlsOnHover)
            //            return;
            //        mouseOverControls = false;
            //        schedule.Execute(_ => {
            //            if (!mouseOverControls)
            //                controlsContainer.style.display = DisplayStyle.None;
            //        }).ExecuteLater(500);
            //    });
            //}

            Undo.undoRedoPerformed += UpdateFieldValues;

            //debugContainer = new VisualElement { name = "debug" };
            //if (nodeTarget.debug)
                //outputContainer.Add(debugContainer);

            initializing = true;

            UpdateTitle();
            SetPosition(nodeTarget.position);
            //SetNodeColor(nodeTarget.color);

            AddInputContainer();

            // Add renaming capability
            //if ((capabilities & Capabilities.Renamable) != 0)
            //SetupRenamableTitle();
        }


        void UpdateTitle()
        {
            //Debug.Log("BaseNodeView UpdateTitle()");
            title = (nodeTarget.GetCustomName() == null) ? nodeTarget.GetType().Name : nodeTarget.GetCustomName();
        }

        // Workaround for bug in GraphView that makes the node selection border way too big
        VisualElement selectionBorder, nodeBorder;
        internal void EnableSyncSelectionBorderHeight()
        {
            //Debug.Log("BaseNodeView EnableSyncSelectionBorderHeight()");
            //if (selectionBorder == null || nodeBorder == null)
            //{
            //    selectionBorder = this.Q("selection-border");
            //    nodeBorder = this.Q("node-border");

            //    schedule.Execute(() => {
            //        selectionBorder.style.height = nodeBorder.localBound.height;
            //    }).Every(17);
            //}
        }

        //void InitializeDebug()
        //{
        //    Debug.Log("BaseNodeView InitializeDebug()");
        //    //ComputeOrderUpdatedCallback();
        //    debugContainer.Add(computeOrderLabel);
        //}

        #endregion

        #region API

        public List<FacialExpressionPortView> GetPortViewsFromFieldName(string fieldName)
        {
            //Debug.Log("BaseNodeView GetPortViewsFromFieldName()");
            List<FacialExpressionPortView> ret;

            portsPerFieldName.TryGetValue(fieldName, out ret);

            return ret;
        }

        public FacialExpressionPortView GetFirstPortViewFromFieldName(string fieldName)
        {
            //Debug.Log("BaseNodeView GetFirstPortViewFromFieldName()");
            return GetPortViewsFromFieldName(fieldName)?.First();
        }

        public FacialExpressionPortView GetPortViewFromFieldName(string fieldName, string identifier)
        {
            //Debug.Log("BaseNodeView GetPortViewFromFieldName()");
            return GetPortViewsFromFieldName(fieldName)?.FirstOrDefault(pv => {
                return (pv.portData.identifier == identifier) || (String.IsNullOrEmpty(pv.portData.identifier) && String.IsNullOrEmpty(identifier));
            });
        }

        public FacialExpressionPortView AddPort(FieldInfo fieldInfo, Direction direction, FacialExpressionEdgeConnectorListener listener, PortData portData, Image img)
        {
            FacialExpressionPortView p = CreatePortView(direction, fieldInfo, portData, listener);//PortView equal Port extend more data

            if (p.direction == Direction.Input)
            {
                //Debug.Log("BaseNodeView AddPort() input:"+ portData.displayName);
                inputPortViews.Add(p);

                if (portData.vertical)
                    topPortContainer.Add(p);
                else
                    inputContainer.Add(p);
            }
            else
            {
                //Debug.Log("BaseNodeView AddPort() output:"+portData.identifier+", " + portData.displayName+", container:"+ (portData.vertical?"bottom":"output"));
                outputPortViews.Add(p);

                if (portData.vertical)
                    bottomPortContainer.Add(p);
                else
                {
                    outputContainer.Add(img);
                    outputContainer.Add(p);
            }
            }

            p.Initialize(this, portData?.displayName);

            List<FacialExpressionPortView> ports;
            portsPerFieldName.TryGetValue(p.fieldName, out ports);
            if (ports == null)
            {
                ports = new List<FacialExpressionPortView>();
                portsPerFieldName[p.fieldName] = ports;
            }
            ports.Add(p);

            return p;
        }

        protected virtual FacialExpressionPortView CreatePortView(Direction direction, FieldInfo fieldInfo, PortData portData, FacialExpressionEdgeConnectorListener listener)
            => FacialExpressionPortView.CreatePortView(direction, fieldInfo, portData, listener);

        public void InsertPort(FacialExpressionPortView portView, int index)
        {   // maybe can mark all 0820
            //Debug.Log("BaseNodeView InsertPort()");
            if (portView.direction == Direction.Input)
            {
                if (portView.portData.vertical)
                    topPortContainer.Insert(index, portView);
                else
                    inputContainer.Insert(index, portView);
            }
            else
            {
                if (portView.portData.vertical)
                    bottomPortContainer.Insert(index, portView);
                else
                    outputContainer.Insert(index, portView);
            }
        }

        public void RemovePort(FacialExpressionPortView p)
        {
            //Debug.Log("BaseNodeView RemovePort()");
            // Remove all connected edges:
            var edgesCopy = p.GetEdges().ToList();
            foreach (var e in edgesCopy)
                owner.Disconnect(e, refreshPorts: false);

            if (p.direction == Direction.Input)
            {
                if (inputPortViews.Remove(p))
                    p.RemoveFromHierarchy();
            }
            else
            {
                if (outputPortViews.Remove(p))
                    p.RemoveFromHierarchy();
            }

            List<FacialExpressionPortView> ports;
            portsPerFieldName.TryGetValue(p.fieldName, out ports);
            ports.Remove(p);
        }

        //private void SetValuesForSelectedNodes()
        //{
        //    Debug.Log("BaseNodeView SetValuesForSelectedNodes()");
        //    selectedNodes = new List<Node>();
        //    owner.nodes.ForEach(node =>
        //    {
        //        if (node.selected) selectedNodes.Add(node);
        //    });

        //    if (selectedNodes.Count < 2) return; //	No need for any of the calculations below

        //    selectedNodesFarLeft = int.MinValue;
        //    selectedNodesFarRight = int.MinValue;
        //    selectedNodesFarTop = int.MinValue;
        //    selectedNodesFarBottom = int.MinValue;

        //    selectedNodesNearLeft = int.MaxValue;
        //    selectedNodesNearRight = int.MaxValue;
        //    selectedNodesNearTop = int.MaxValue;
        //    selectedNodesNearBottom = int.MaxValue;

        //    foreach (var selectedNode in selectedNodes)
        //    {
        //        var nodeStyle = selectedNode.style;
        //        var nodeWidth = selectedNode.localBound.size.x;
        //        var nodeHeight = selectedNode.localBound.size.y;

        //        if (nodeStyle.left.value.value > selectedNodesFarLeft) selectedNodesFarLeft = nodeStyle.left.value.value;
        //        if (nodeStyle.left.value.value + nodeWidth > selectedNodesFarRight) selectedNodesFarRight = nodeStyle.left.value.value + nodeWidth;
        //        if (nodeStyle.top.value.value > selectedNodesFarTop) selectedNodesFarTop = nodeStyle.top.value.value;
        //        if (nodeStyle.top.value.value + nodeHeight > selectedNodesFarBottom) selectedNodesFarBottom = nodeStyle.top.value.value + nodeHeight;

        //        if (nodeStyle.left.value.value < selectedNodesNearLeft) selectedNodesNearLeft = nodeStyle.left.value.value;
        //        if (nodeStyle.left.value.value + nodeWidth < selectedNodesNearRight) selectedNodesNearRight = nodeStyle.left.value.value + nodeWidth;
        //        if (nodeStyle.top.value.value < selectedNodesNearTop) selectedNodesNearTop = nodeStyle.top.value.value;
        //        if (nodeStyle.top.value.value + nodeHeight < selectedNodesNearBottom) selectedNodesNearBottom = nodeStyle.top.value.value + nodeHeight;
        //    }

        //    selectedNodesAvgHorizontal = (selectedNodesNearLeft + selectedNodesFarRight) / 2f;
        //    selectedNodesAvgVertical = (selectedNodesNearTop + selectedNodesFarBottom) / 2f;
        //}

        //public static Rect GetNodeRect(Node node, float left = int.MaxValue, float top = int.MaxValue)
        //{
        //    return new Rect(
        //        new Vector2(left != int.MaxValue ? left : node.style.left.value.value, top != int.MaxValue ? top : node.style.top.value.value),
        //        new Vector2(node.style.width.value.value, node.style.height.value.value)
        //    );
        //}

        //public void ToggleDebug()
        //{
        //    //nodeTarget.debug = !nodeTarget.debug;
        //    UpdateDebugView();
        //}

        //public void UpdateDebugView()
        //{
        //    //if (nodeTarget.debug)
        //    outputContainer.Add(debugContainer);
        //    //else
        //        //mainContainer.Remove(debugContainer);
        //}

        #endregion

        #region Callbacks & Overrides

        //void ComputeOrderUpdatedCallback()
        //{
        //    //Update debug compute order
        //    computeOrderLabel.text = "Compute order: " + nodeTarget.computeOrder;
        //}

        public virtual void Enable(bool fromInspector = false) => DrawDefaultInspector(fromInspector);
        public virtual void Enable() => DrawDefaultInspector(false);

        public virtual void Disable() { }

        Dictionary<string, List<(object value, VisualElement target)>> visibleConditions = new Dictionary<string, List<(object value, VisualElement target)>>();
        Dictionary<string, VisualElement> hideElementIfConnected = new Dictionary<string, VisualElement>();
        Dictionary<FieldInfo, List<VisualElement>> fieldControlsMap = new Dictionary<FieldInfo, List<VisualElement>>();

        protected void AddInputContainer()
        {
            //Debug.Log("BaseNodeView AddInputContainer()");
            inputContainerElement = new VisualElement { name = "input-container" };
            mainContainer.parent.Add(inputContainerElement);
            inputContainerElement.SendToBack();
            inputContainerElement.pickingMode = PickingMode.Ignore;
            inputContainer.style.width = 80;
        }

        protected virtual void DrawDefaultInspector(bool fromInspector = false)
        {
            var fields = nodeTarget.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                // Filter fields from the BaseNode type since we are only interested in user-defined fields
                // (better than BindingFlags.DeclaredOnly because we keep any inherited user-defined fields) 
                .Where(f => f.DeclaringType != typeof(FacialTrackingNode));

            fields = nodeTarget.OverrideFieldOrder(fields).Reverse();

            foreach (var field in fields)
            {
                //skip if the field is a node setting
                if (field.GetCustomAttribute(typeof(SettingAttribute)) != null)
                {
                    hasSettings = true;
                    continue;
                }

                //skip if the field is not serializable
                bool serializeField = field.GetCustomAttribute(typeof(SerializeField)) != null;
                if ((!field.IsPublic && !serializeField) || field.IsNotSerialized)
                {
                    AddEmptyField(field, fromInspector);
                    continue;
                }

                //skip if the field is an input/output and not marked as SerializedField
                bool hasInputAttribute = field.GetCustomAttribute(typeof(InputAttribute)) != null;
                bool hasInputOrOutputAttribute = hasInputAttribute || field.GetCustomAttribute(typeof(OutputAttribute)) != null;
                bool showAsDrawer = !fromInspector && field.GetCustomAttribute(typeof(ShowAsDrawer)) != null;
                if (!serializeField && hasInputOrOutputAttribute && !showAsDrawer)
                {
                    AddEmptyField(field, fromInspector);
                    continue;
                }

                //skip if marked with NonSerialized or HideInInspector
                if (field.GetCustomAttribute(typeof(System.NonSerializedAttribute)) != null || field.GetCustomAttribute(typeof(HideInInspector)) != null)
                {
                    AddEmptyField(field, fromInspector);
                    continue;
                }

                // Hide the field if we want to display in in the inspector
                var showInInspector = field.GetCustomAttribute<ShowInInspector>();
                if (!serializeField && showInInspector != null && !showInInspector.showInNode && !fromInspector)
                {
                    AddEmptyField(field, fromInspector);
                    continue;
                }

                var showInputDrawer = field.GetCustomAttribute(typeof(InputAttribute)) != null && field.GetCustomAttribute(typeof(SerializeField)) != null;
                showInputDrawer |= field.GetCustomAttribute(typeof(InputAttribute)) != null && field.GetCustomAttribute(typeof(ShowAsDrawer)) != null;
                showInputDrawer &= !fromInspector; // We can't show a drawer in the inspector
                showInputDrawer &= !typeof(IList).IsAssignableFrom(field.FieldType);

                string displayName = ObjectNames.NicifyVariableName(field.Name);

                var inspectorNameAttribute = field.GetCustomAttribute<InspectorNameAttribute>();
                if (inspectorNameAttribute != null)
                    displayName = inspectorNameAttribute.displayName;

                var elem = AddControlField(field, displayName, showInputDrawer);
                if (hasInputAttribute)
                {
                    hideElementIfConnected[field.Name] = elem;

                    // Hide the field right away if there is already a connection:
                    if (portsPerFieldName.TryGetValue(field.Name, out var pvs))
                        if (pvs.Any(pv => pv.GetEdges().Count > 0))
                            elem.style.display = DisplayStyle.None;
                }
            }
        }

        protected virtual void SetNodeColor(Color color)
        {
            //Debug.Log("BaseNodeView SetNodeColor()");
            titleContainer.style.borderBottomColor = new StyleColor(color);
            titleContainer.style.borderBottomWidth = new StyleFloat(color.a > 0 ? 5f : 0f);
        }

        private void AddEmptyField(FieldInfo field, bool fromInspector)
        {
            //Debug.Log("BaseNodeView AddEmptyField()");
            if (field.GetCustomAttribute(typeof(InputAttribute)) == null || fromInspector)
                return;

            if (field.GetCustomAttribute<VerticalAttribute>() != null)
                return;

            var box = new VisualElement { name = field.Name };
            box.AddToClassList("port-input-element");
            box.AddToClassList("empty");
            inputContainerElement.Add(box);
        }

        void UpdateFieldVisibility(string fieldName, object newValue)
        {
            //Debug.Log("BaseNodeView UpdateFieldVisibility()");
            if (newValue == null)
                return;
            if (visibleConditions.TryGetValue(fieldName, out var list))
            {
                foreach (var elem in list)
                {
                    if (newValue.Equals(elem.value))
                        elem.target.style.display = DisplayStyle.Flex;
                    else
                        elem.target.style.display = DisplayStyle.None;
                }
            }
        }

        void UpdateOtherFieldValueSpecific<T>(FieldInfo field, object newValue)
        {
            //Debug.Log("BaseNodeView UpdateOtherFieldValueSpecific()");
            foreach (var inputField in fieldControlsMap[field])
            {
                var notify = inputField as INotifyValueChanged<T>;
                if (notify != null)
                    notify.SetValueWithoutNotify((T)newValue);
            }
        }

        static MethodInfo specificUpdateOtherFieldValue = typeof(FacialTrackingGraphNode).GetMethod(nameof(UpdateOtherFieldValueSpecific), BindingFlags.NonPublic | BindingFlags.Instance);
        void UpdateOtherFieldValue(FieldInfo info, object newValue)
        {
            //Debug.Log("BaseNodeView UpdateOtherFieldValue()");
            // Warning: Keep in sync with FieldFactory CreateField
            var fieldType = info.FieldType.IsSubclassOf(typeof(UnityEngine.Object)) ? typeof(UnityEngine.Object) : info.FieldType;
            var genericUpdate = specificUpdateOtherFieldValue.MakeGenericMethod(fieldType);

            genericUpdate.Invoke(this, new object[] { info, newValue });
        }

        object GetInputFieldValueSpecific<T>(FieldInfo field)
        {
            //Debug.Log("BaseNodeView GetInputFieldValueSpecific()");
            if (fieldControlsMap.TryGetValue(field, out var list))
            {
                foreach (var inputField in list)
                {
                    if (inputField is INotifyValueChanged<T> notify)
                        return notify.value;
                }
            }
            return null;
        }

        static MethodInfo specificGetValue = typeof(FacialTrackingGraphNode).GetMethod(nameof(GetInputFieldValueSpecific), BindingFlags.NonPublic | BindingFlags.Instance);
        object GetInputFieldValue(FieldInfo info)
        {
            //Debug.Log("BaseNodeView GetInputFieldValue()");
            // Warning: Keep in sync with FieldFactory CreateField
            var fieldType = info.FieldType.IsSubclassOf(typeof(UnityEngine.Object)) ? typeof(UnityEngine.Object) : info.FieldType;
            var genericUpdate = specificGetValue.MakeGenericMethod(fieldType);

            return genericUpdate.Invoke(this, new object[] { info });
        }

        protected VisualElement AddControlField(string fieldName, string label = null, bool showInputDrawer = false, Action valueChangedCallback = null)
            => AddControlField(nodeTarget.GetType().GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance), label, showInputDrawer, valueChangedCallback);

        Regex s_ReplaceNodeIndexPropertyPath = new Regex(@"(^nodes.Array.data\[)(\d+)(\])");
        internal void SyncSerializedPropertyPathes()
        {
            //Debug.Log("BaseNodeView SyncSerializedPropertyPathes()");
            int nodeIndex = owner.graph.nodes.FindIndex(n => n == nodeTarget);

            // If the node is not found, then it means that it has been deleted from serialized data.
            if (nodeIndex == -1)
                return;

            var nodeIndexString = nodeIndex.ToString();
            foreach (var propertyField in this.Query<PropertyField>().ToList())
            {
                propertyField.Unbind();
                // The property path look like this: nodes.Array.data[x].fieldName
                // And we want to update the value of x with the new node index:
                propertyField.bindingPath = s_ReplaceNodeIndexPropertyPath.Replace(propertyField.bindingPath, m => m.Groups[1].Value + nodeIndexString + m.Groups[3].Value);
                propertyField.Bind(owner.serializedGraph);
            }
        }

        protected SerializedProperty FindSerializedProperty(string fieldName)
        {
            //Debug.Log("BaseNodeView FindSerializedProperty()");
            int i = owner.graph.nodes.FindIndex(n => n == nodeTarget);
            return owner.serializedGraph.FindProperty("nodes").GetArrayElementAtIndex(i).FindPropertyRelative(fieldName);
        }

        protected VisualElement AddControlField(FieldInfo field, string label = null, bool showInputDrawer = false, Action valueChangedCallback = null)
        {
            //Debug.Log("BaseNodeView AddControlField()");
            if (field == null)
                return null;

            var element = new PropertyField(FindSerializedProperty(field.Name), showInputDrawer ? "" : label);
            element.Bind(owner.serializedGraph);

#if UNITY_2020_3 // In Unity 2020.3 the empty label on property field doesn't hide it, so we do it manually
			if ((showInputDrawer || String.IsNullOrEmpty(label)) && element != null)
				element.AddToClassList("DrawerField_2020_3");
#endif

            if (typeof(IList).IsAssignableFrom(field.FieldType))
                EnableSyncSelectionBorderHeight();

            element.RegisterValueChangeCallback(e => {
                UpdateFieldVisibility(field.Name, field.GetValue(nodeTarget));
                valueChangedCallback?.Invoke();
                NotifyNodeChanged();
            });

            // Disallow picking scene objects when the graph is not linked to a scene
            if (element != null)//&& !owner.graph.IsLinkedToScene()
            {
                var objectField = element.Q<ObjectField>();
                if (objectField != null)
                    objectField.allowSceneObjects = false;
            }

            if (!fieldControlsMap.TryGetValue(field, out var inputFieldList))
                inputFieldList = fieldControlsMap[field] = new List<VisualElement>();
            inputFieldList.Add(element);

            if (element != null)
            {
                if (showInputDrawer)
                {
                    var box = new VisualElement { name = field.Name };
                    box.AddToClassList("port-input-element");
                    box.Add(element);
                    inputContainerElement.Add(box);
                }
                else
                {
                    controlsContainer.Add(element);
                }
                element.name = field.Name;
            }
            else
            {
                // Make sure we create an empty placeholder if FieldFactory can not provide a drawer
                if (showInputDrawer) AddEmptyField(field, false);
            }

            var visibleCondition = field.GetCustomAttribute(typeof(VisibleIf)) as VisibleIf;
            if (visibleCondition != null)
            {
                // Check if target field exists:
                var conditionField = nodeTarget.GetType().GetField(visibleCondition.fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (conditionField == null)
                    Debug.LogError($"[VisibleIf] Field {visibleCondition.fieldName} does not exists in node {nodeTarget.GetType()}");
                else
                {
                    visibleConditions.TryGetValue(visibleCondition.fieldName, out var list);
                    if (list == null)
                        list = visibleConditions[visibleCondition.fieldName] = new List<(object value, VisualElement target)>();
                    list.Add((visibleCondition.value, element));
                    UpdateFieldVisibility(visibleCondition.fieldName, conditionField.GetValue(nodeTarget));
                }
            }

            return element;
        }

        void UpdateFieldValues()
        {
            //Debug.Log("BaseNodeView UpdateFieldValues()");
            foreach (var kp in fieldControlsMap)
                UpdateOtherFieldValue(kp.Key, kp.Key.GetValue(nodeTarget));
        }


        internal void OnPortConnected(FacialExpressionPortView port)
        {
            //Debug.Log("BaseNodeView OnPortConnected()");
            if (port.direction == Direction.Input && inputContainerElement?.Q(port.fieldName) != null)
                inputContainerElement.Q(port.fieldName).AddToClassList("empty");

            if (hideElementIfConnected.TryGetValue(port.fieldName, out var elem))
                elem.style.display = DisplayStyle.None;

            onPortConnected?.Invoke(port);
        }

        internal void OnPortDisconnected(FacialExpressionPortView port)
        {
            //Debug.Log("BaseNodeView OnPortDisconnected()");
            if (port.direction == Direction.Input && inputContainerElement?.Q(port.fieldName) != null)
            {
                inputContainerElement.Q(port.fieldName).RemoveFromClassList("empty");

                if (nodeTarget.nodeFields.TryGetValue(port.fieldName, out var fieldInfo))
                {
                    var valueBeforeConnection = GetInputFieldValue(fieldInfo.info);

                    if (valueBeforeConnection != null)
                    {
                        fieldInfo.info.SetValue(nodeTarget, valueBeforeConnection);
                    }
                }
            }

            if (hideElementIfConnected.TryGetValue(port.fieldName, out var elem))
                elem.style.display = DisplayStyle.Flex;

            onPortDisconnected?.Invoke(port);
        }

        // TODO: a function to force to reload the custom behavior ports (if we want to do a button to add ports for example)

        public virtual void OnRemoved() { }
        public virtual void OnCreated() { }

        public override void SetPosition(Rect newPos)
        {
            //Debug.Log("BaseNodeView SetPosition()");
            //if (initializing || !nodeTarget.isLocked) //skip for node always can move and keep move action in Undo history
            {
                base.SetPosition(newPos);

                if (!initializing)
                    owner.RegisterCompleteObjectUndo("Moved graph node");

                nodeTarget.position = newPos;
                initializing = false;
            }
        }

        IEnumerable<FacialExpressionPortView> SyncPortCounts(IEnumerable<FacialExpressionNodePort> ports, IEnumerable<FacialExpressionPortView> portViews)
        {
            //Debug.Log("BaseNodeView SyncPortCounts()");
            var listener = owner.connectorListener;
            var portViewList = portViews.ToList();

            // Maybe not good to remove ports as edges are still connected :/
            foreach (var pv in portViews.ToList())
            {
                // If the port have disappeared from the node data, we remove the view:
                // We can use the identifier here because this function will only be called when there is a custom port behavior
                if (!ports.Any(p => p.portData.identifier == pv.portData.identifier))
                {
                    RemovePort(pv);
                    portViewList.Remove(pv);
                }
            }

            Image _img = new Image();
            foreach (var p in ports)
            {
                // Add missing port views
                if (!portViews.Any(pv => p.portData.identifier == pv.portData.identifier))
                {
                    Direction portDirection = nodeTarget.IsFieldInput(p.fieldName) ? Direction.Input : Direction.Output;
                    _img.image = AssetDatabase.LoadAssetAtPath<Texture2D>(p.portData.thumbnail);
                    var pv = AddPort(p.fieldInfo, portDirection, listener, p.portData, _img);
                    portViewList.Add(pv);
                }
            }

            return portViewList;
        }

        void SyncPortOrder(IEnumerable<FacialExpressionNodePort> ports, IEnumerable<FacialExpressionPortView> portViews)
        {
            //Debug.Log("BaseNodeView SyncPortOrder()");
            var portViewList = portViews.ToList();
            var portsList = ports.ToList();

            // Re-order the port views to match the ports order in case a custom behavior re-ordered the ports
            for (int i = 0; i < portsList.Count; i++)
            {
                var id = portsList[i].portData.identifier;

                var pv = portViewList.FirstOrDefault(p => p.portData.identifier == id);
                if (pv != null)
                    InsertPort(pv, i);
            }
        }

        public virtual new bool RefreshPorts()
        {
            //Debug.Log("BaseNodeView RefreshPorts()");
            // If a port behavior was attached to one port, then
            // the port count might have been updated by the node
            // so we have to refresh the list of port views.
            UpdatePortViewWithPorts(nodeTarget.inputPorts, inputPortViews);
            UpdatePortViewWithPorts(nodeTarget.outputPorts, outputPortViews);

            void UpdatePortViewWithPorts(NodePortContainer ports, List<FacialExpressionPortView> portViews)
            {
                if (ports.Count == 0 && portViews.Count == 0) // Nothing to update
                    return;

                // When there is no current portviews, we can't zip the list so we just add all
                if (portViews.Count == 0)
                    SyncPortCounts(ports, new FacialExpressionPortView[] { });
                else if (ports.Count == 0) // Same when there is no ports
                    SyncPortCounts(new FacialExpressionNodePort[] { }, portViews);
                else if (portViews.Count != ports.Count)
                    SyncPortCounts(ports, portViews);
                else
                {
                    //Debug.Log("UpdatePortViewWithPorts() else");
                    var p = ports.GroupBy(n => n.fieldName);
                    var pv = portViews.GroupBy(v => v.fieldName);
                    p.Zip(pv, (portPerFieldName, portViewPerFieldName) => {
                        IEnumerable<FacialExpressionPortView> portViewsList = portViewPerFieldName;
                        if (portPerFieldName.Count() != portViewPerFieldName.Count())
                            portViewsList = SyncPortCounts(portPerFieldName, portViewPerFieldName);
                        SyncPortOrder(portPerFieldName, portViewsList);
                        // We don't care about the result, we just iterate over port and portView
                        return "";
                    }).ToList();
                }

                // Here we're sure that we have the same amount of port and portView
                // so we can update the view with the new port data (if the name of a port have been changed for example)

                for (int i = 0; i < portViews.Count; i++)
                    portViews[i].UpdatePortView(ports[i].portData);
            }

            return base.RefreshPorts();
        }

        //public void ForceUpdatePorts()
        //{
        //    Debug.Log("BaseNodeView ForceUpdatePorts()");
        //    nodeTarget.UpdateAllPorts();

        //    RefreshPorts();
        //}

        void UpdatePortsForField(string fieldName)
        {
            //Debug.Log("BaseNodeView UpdatePortsForField()");
            // TODO: actual code
            RefreshPorts();
        }

        //protected virtual VisualElement CreateSettingsView() => new Label("Settings") { name = "header" };

        /// <summary>
        /// Send an event to the graph telling that the content of this node have changed
        /// </summary>
        public void NotifyNodeChanged() => owner.graph.NotifyNodeChanged(nodeTarget);

        #endregion

        #region waveFacialThumbnal
        public class OriginalStatePortData
        {
            public FacialExpressionPortView LinkingPort { get; set; }
            public Image LinkingImage { get; set; }

            public OriginalStatePortData(FacialExpressionPortView _Port, Image _Img)
            {
                LinkingPort = _Port;
                LinkingImage = _Img;
                LinkingImage.style.width = 0;
                LinkingImage.style.height = 0;
                //LinkingImage.style.marginLeft = 100;
                //LinkingImage.style.transformOrigin = new TransformOrigin(Length.Percent(100), Length.Percent(50), 0);
                LinkingImage.style.position = Position.Absolute; //customize the image position

                if (LinkingImage.image != null)
                {
                    LinkingPort.RegisterCallback<MouseEnterEvent>(evt =>
                    {
                        var portWorldPos = LinkingPort.worldBound;
                        //Debug.Log("OriginalStatePortData() MouseEnter" + LinkingPort.GetPosition());
                        //---Generate Hint Pic
                        LinkingImage.style.height = LinkingImage.image.height * 0.3f;
                        LinkingImage.style.width = LinkingImage.image.width * 0.3f;
                        //---Generate Hint Pic
                        LinkingImage.style.left = 0; //portWorldPos.xMin-175;
                        LinkingImage.style.top = Int32.Parse(LinkingPort.portData.identifier) * 22 + 4;//portWorldPos.yMin-175;
                    });
                    LinkingPort.RegisterCallback<MouseLeaveEvent>(evt =>
                    {
                        //---Remove Hint Pic
                        LinkingImage.style.width = 0;
                        LinkingImage.style.height = 0;
                        //LinkingImage.style.marginLeft = 0;
                        //---Remove Hint Pic
                    });
                }
            }
        }
        #endregion

    }



    public static class NodeProvider
    {
        public struct PortDescription
        {
            public Type nodeType;
            public Type portType;
            public bool isInput;
            public string portFieldName;
            public string portIdentifier;
            public string portDisplayName;
        }

        static Dictionary<Type, MonoScript> nodeViewScripts = new Dictionary<Type, MonoScript>();
        static Dictionary<Type, MonoScript> nodeScripts = new Dictionary<Type, MonoScript>();
        static Dictionary<Type, Type> nodeViewPerType = new Dictionary<Type, Type>();

        public class NodeDescriptions
        {
            public Dictionary<string, Type> nodePerMenuTitle = new Dictionary<string, Type>();
            public List<Type> slotTypes = new List<Type>();
            public List<PortDescription> nodeCreatePortDescription = new List<PortDescription>();
        }

        public struct NodeSpecificToGraph
        {
            public Type nodeType;
            public List<MethodInfo> isCompatibleWithGraph;
            public Type compatibleWithGraphType;
        }

        static Dictionary<FacialExpressionConfig, NodeDescriptions> specificNodeDescriptions = new Dictionary<FacialExpressionConfig, NodeDescriptions>();
        static List<NodeSpecificToGraph> specificNodes = new List<NodeSpecificToGraph>();

        static NodeDescriptions genericNodes = new NodeDescriptions();

        static NodeProvider()
        {
            BuildScriptCache();
            BuildGenericNodeCache();
        }

        public static void LoadGraph(FacialExpressionConfig graph)
        {
            // Clear old graph data in case there was some
            specificNodeDescriptions.Remove(graph);
            var descriptions = new NodeDescriptions();
            specificNodeDescriptions.Add(graph, descriptions);

            var graphType = graph.GetType();
            foreach (var nodeInfo in specificNodes)
            {
                bool compatible = nodeInfo.compatibleWithGraphType == null || nodeInfo.compatibleWithGraphType == graphType;

                if (nodeInfo.isCompatibleWithGraph != null)
                {
                    foreach (var method in nodeInfo.isCompatibleWithGraph)
                        compatible &= (bool)method?.Invoke(null, new object[] { graph });
                }

                if (compatible)
                    BuildCacheForNode(nodeInfo.nodeType, descriptions, graph);
            }
        }

        public static void UnloadGraph(FacialExpressionConfig graph)
        {
            specificNodeDescriptions.Remove(graph);
        }

        static void BuildGenericNodeCache()
        {
            foreach (var nodeType in TypeCache.GetTypesDerivedFrom<FacialTrackingNode>())
            {
                if (!IsNodeAccessibleFromMenu(nodeType))
                    continue;

                if (IsNodeSpecificToGraph(nodeType))
                    continue;

                BuildCacheForNode(nodeType, genericNodes);
            }
        }

        static void BuildCacheForNode(Type nodeType, NodeDescriptions targetDescription, FacialExpressionConfig graph = null)
        {
            var attrs = nodeType.GetCustomAttributes(typeof(NodeMenuItemAttribute), false) as NodeMenuItemAttribute[];

            if (attrs != null && attrs.Length > 0)
            {
                foreach (var attr in attrs)
                    targetDescription.nodePerMenuTitle[attr.menuTitle] = nodeType;
            }

            foreach (var field in nodeType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (field.GetCustomAttribute<HideInInspector>() == null && field.GetCustomAttributes().Any(c => c is InputAttribute || c is OutputAttribute))
                    targetDescription.slotTypes.Add(field.FieldType);
            }

            ProvideNodePortCreationDescription(nodeType, targetDescription, graph);
        }

        static bool IsNodeAccessibleFromMenu(Type nodeType)
        {
            if (nodeType.IsAbstract)
                return false;

            return nodeType.GetCustomAttributes<NodeMenuItemAttribute>().Count() > 0;
        }

        // Check if node has anything that depends on the graph type or settings
        static bool IsNodeSpecificToGraph(Type nodeType)
        {
            var isCompatibleWithGraphMethods = nodeType.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy).Where(m => m.GetCustomAttribute<IsCompatibleWithGraph>() != null);
            var nodeMenuAttributes = nodeType.GetCustomAttributes<NodeMenuItemAttribute>();

            List<Type> compatibleGraphTypes = nodeMenuAttributes.Where(n => n.onlyCompatibleWithGraph != null).Select(a => a.onlyCompatibleWithGraph).ToList();

            List<MethodInfo> compatibleMethods = new List<MethodInfo>();
            foreach (var method in isCompatibleWithGraphMethods)
            {
                // Check if the method is static and have the correct prototype
                var p = method.GetParameters();
                if (method.ReturnType != typeof(bool) || p.Count() != 1 || p[0].ParameterType != typeof(FacialExpressionConfig))
                    Debug.LogError($"The function '{method.Name}' marked with the IsCompatibleWithGraph attribute either doesn't return a boolean or doesn't take one parameter of BaseGraph type.");
                else
                    compatibleMethods.Add(method);
            }

            if (compatibleMethods.Count > 0 || compatibleGraphTypes.Count > 0)
            {
                // We still need to add the element in specificNode even without specific graph
                if (compatibleGraphTypes.Count == 0)
                    compatibleGraphTypes.Add(null);

                foreach (var graphType in compatibleGraphTypes)
                {
                    specificNodes.Add(new NodeSpecificToGraph
                    {
                        nodeType = nodeType,
                        isCompatibleWithGraph = compatibleMethods,
                        compatibleWithGraphType = graphType
                    });
                }
                return true;
            }
            return false;
        }

        static void BuildScriptCache()
        {
            foreach (var nodeType in TypeCache.GetTypesDerivedFrom<FacialTrackingNode>())
            {
                if (!IsNodeAccessibleFromMenu(nodeType))
                    continue;

                AddNodeScriptAsset(nodeType);
            }

            foreach (var nodeViewType in TypeCache.GetTypesDerivedFrom<FacialTrackingGraphNode>())
            {
                if (!nodeViewType.IsAbstract)
                    AddNodeViewScriptAsset(nodeViewType);
            }
        }

        static FieldInfo SetGraph = typeof(FacialTrackingNode).GetField("graph", BindingFlags.NonPublic | BindingFlags.Instance);
        static void ProvideNodePortCreationDescription(Type nodeType, NodeDescriptions targetDescription, FacialExpressionConfig graph = null)
        {
            var node = Activator.CreateInstance(nodeType) as FacialTrackingNode;
            try
            {
                SetGraph.SetValue(node, graph);
                node.InitializePorts();
                node.UpdateAllPorts();
            }
            catch (Exception) { }

            foreach (var p in node.inputPorts)
                AddPort(p, true);
            foreach (var p in node.outputPorts)
                AddPort(p, false);

            void AddPort(FacialExpressionNodePort p, bool input)
            {
                targetDescription.nodeCreatePortDescription.Add(new PortDescription
                {
                    nodeType = nodeType,
                    portType = p.portData.displayType ?? p.fieldInfo.FieldType,
                    isInput = input,
                    portFieldName = p.fieldName,
                    portDisplayName = p.portData.displayName ?? p.fieldName,
                    portIdentifier = p.portData.identifier,
                });
            }
        }

        static void AddNodeScriptAsset(Type type)
        {
            var nodeScriptAsset = FindScriptFromClassName(type.Name);

            // Try find the class name with Node name at the end
            if (nodeScriptAsset == null)
                nodeScriptAsset = FindScriptFromClassName(type.Name + "Node");
            if (nodeScriptAsset != null)
                nodeScripts[type] = nodeScriptAsset;
        }

        static void AddNodeViewScriptAsset(Type type)
        {
            var attrs = type.GetCustomAttributes(typeof(NodeCustomEditor), false) as NodeCustomEditor[];

            if (attrs != null && attrs.Length > 0)
            {
                Type nodeType = attrs.First().nodeType;
                nodeViewPerType[nodeType] = type;

                var nodeViewScriptAsset = FindScriptFromClassName(type.Name);
                if (nodeViewScriptAsset == null)
                    nodeViewScriptAsset = FindScriptFromClassName(type.Name + "View");
                if (nodeViewScriptAsset == null)
                    nodeViewScriptAsset = FindScriptFromClassName(type.Name + "NodeView");

                if (nodeViewScriptAsset != null)
                    nodeViewScripts[type] = nodeViewScriptAsset;
            }
        }

        static MonoScript FindScriptFromClassName(string className)
        {
            var scriptGUIDs = UnityEditor.AssetDatabase.FindAssets($"t:script {className}");

            if (scriptGUIDs.Length == 0)
                return null;

            foreach (var scriptGUID in scriptGUIDs)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(scriptGUID);
                var script = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);

                if (script != null && String.Equals(className, Path.GetFileNameWithoutExtension(assetPath), StringComparison.OrdinalIgnoreCase))
                    return script;
            }

            return null;
        }

        public static Type GetNodeViewTypeFromType(Type nodeType)
        {
            Type view;

            if (nodeViewPerType.TryGetValue(nodeType, out view))
                return view;

            Type baseType = null;

            // Allow for inheritance in node views: multiple C# node using the same view
            foreach (var type in nodeViewPerType)
            {
                // Find a view (not first fitted view) of nodeType
                if (nodeType.IsSubclassOf(type.Key) && (baseType == null || type.Value.IsSubclassOf(baseType)))
                    baseType = type.Value;
            }

            if (baseType != null)
                return baseType;

            return view;
        }

        public static IEnumerable<(string path, Type type)> GetNodeMenuEntries(FacialExpressionConfig graph = null)
        {
            foreach (var node in genericNodes.nodePerMenuTitle)
                yield return (node.Key, node.Value);

            if (graph != null && specificNodeDescriptions.TryGetValue(graph, out var specificNodes))
            {
                foreach (var node in specificNodes.nodePerMenuTitle)
                    yield return (node.Key, node.Value);
            }
        }

        public static MonoScript GetNodeViewScript(Type type)
        {
            nodeViewScripts.TryGetValue(type, out var script);

            return script;
        }

        public static MonoScript GetNodeScript(Type type)
        {
            nodeScripts.TryGetValue(type, out var script);

            return script;
        }

        public static IEnumerable<Type> GetSlotTypes(FacialExpressionConfig graph = null)
        {
            foreach (var type in genericNodes.slotTypes)
                yield return type;

            if (graph != null && specificNodeDescriptions.TryGetValue(graph, out var specificNodes))
            {
                foreach (var type in specificNodes.slotTypes)
                    yield return type;
            }
        }

        public static IEnumerable<PortDescription> GetEdgeCreationNodeMenuEntry(FacialExpressionPortView portView, FacialExpressionConfig graph = null)
        {
            //Debug.Log("NodeProvider GetEdgeCreationNodeMenuEntry()");
            foreach (var description in genericNodes.nodeCreatePortDescription)
            {
                if (!IsPortCompatible(description))
                    continue;

                yield return description;
            }

            if (graph != null && specificNodeDescriptions.TryGetValue(graph, out var specificNodes))
            {
                foreach (var description in specificNodes.nodeCreatePortDescription)
                {
                    if (!IsPortCompatible(description))
                        continue;
                    yield return description;
                }
            }

            bool IsPortCompatible(PortDescription description)
            {
                if ((portView.direction == Direction.Input && description.isInput) || (portView.direction == Direction.Output && !description.isInput))
                    return false;

                if (!FacialExpressionConfig.TypesAreConnectable(description.portType, portView.portType))
                    return false;

                return true;
            }
        }
    }
}
#endif