//using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;
//using System.Linq;
//using System.IO;
using System.Reflection;
using UnityEditor;

#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;

namespace VIVE.OpenXR.Toolkits.FacialExpression.Maker.Editor
{
	public class FacialExpressionPortView : Port
	{
		public string				fieldName => fieldInfo.Name;
		public Type					fieldType => fieldInfo.FieldType;
		public new Type				portType;
        public FacialTrackingGraphNode     	owner { get; private set; }
		public PortData				portData;

		public event Action< FacialExpressionPortView, Edge >	OnConnected;
		public event Action< FacialExpressionPortView, Edge >	OnDisconnected;

		protected FieldInfo		fieldInfo;
		protected FacialExpressionEdgeConnectorListener listener;

		string userPortStyleFile = "PortViewTypes";

		List< EdgeView >		edges = new List< EdgeView >();

		public int connectionCount => edges.Count;

		//readonly string portStyle = "GraphProcessorStyles/PortView";

        protected FacialExpressionPortView(Direction direction, FieldInfo fieldInfo, PortData portData, FacialExpressionEdgeConnectorListener edgeConnectorListener)
            : base(portData.vertical ? Orientation.Vertical : Orientation.Horizontal, direction, Capacity.Multi, portData.displayType ?? fieldInfo.FieldType)
		{
            //Debug.Log("PortView PortView() constructor");
            this.fieldInfo = fieldInfo;
			this.listener = edgeConnectorListener;
			this.portType = portData.displayType ?? fieldInfo.FieldType;
			this.portData = portData;
			this.portName = fieldName;

			//styleSheets.Add(Resources.Load<StyleSheet>(portStyle));

			UpdatePortSize();

			var userPortStyle = Resources.Load<StyleSheet>(userPortStyleFile);
			if (userPortStyle != null)
				styleSheets.Add(userPortStyle);

			if (portData.vertical)
				AddToClassList("Vertical");
			
			this.tooltip = portData.tooltip;
		}

		public static FacialExpressionPortView CreatePortView(Direction direction, FieldInfo fieldInfo, PortData portData, FacialExpressionEdgeConnectorListener edgeConnectorListener)
		{
            //Debug.Log("PortView CreatePortView()");
            var pv = new FacialExpressionPortView(direction, fieldInfo, portData, edgeConnectorListener);
			pv.m_EdgeConnector = new FacialExpressionEdgeConnector(edgeConnectorListener); //for port edge must have
			pv.AddManipulator(pv.m_EdgeConnector);

			// Force picking in the port label to enlarge the edge creation zone
			var portLabel = pv.Q("type");
			if (portLabel != null)
			{
				portLabel.pickingMode = PickingMode.Position;
				portLabel.style.flexGrow = 1;
			}

			// hide label when the port is vertical
			if (portData.vertical && portLabel != null)
				portLabel.style.display = DisplayStyle.None;
			
			// Fixup picking mode for vertical top ports
			if (portData.vertical)
				pv.Q("connector").pickingMode = PickingMode.Position;

			return pv;
		}

		/// <summary>
		/// Update the size of the port view (using the portData.sizeInPixel property)
		/// </summary>
		public void UpdatePortSize()
		{
            //Debug.Log("PortView UpdatePortSize()");
            int size = portData.sizeInPixel == 0 ? 8 : portData.sizeInPixel;
			var connector = this.Q("connector");
			var cap = connector.Q("cap");
			connector.style.width = size;
			connector.style.height = size;
			cap.style.width = size - 4;
			cap.style.height = size - 4;

			// Update connected edge sizes:
			edges.ForEach(e => e.UpdateEdgeSize());
		}

		public virtual void Initialize(FacialTrackingGraphNode nodeView, string name)
		{
            //Debug.Log("PortView Initialize()");
            this.owner = nodeView;
			AddToClassList(fieldName);

			// Correct port type if port accept multiple values (and so is a container)
			if (direction == Direction.Input && portData.acceptMultipleEdges && portType == fieldType) // If the user haven't set a custom field type
			{
				if (fieldType.GetGenericArguments().Length > 0)
					portType = fieldType.GetGenericArguments()[0];
			}

			if (name != null)
				portName = name;
			visualClass = "Port_" + portType.Name;
			tooltip = portData.tooltip;
		}

		public override void Connect(Edge edge)
		{
            //Debug.Log("PortView Connect()");
            OnConnected?.Invoke(this, edge);

			base.Connect(edge);

			var inputNode = (edge.input as FacialExpressionPortView).owner;
			var outputNode = (edge.output as FacialExpressionPortView).owner;

			edges.Add(edge as EdgeView);
            //Debug.Log("Maker PortView Connect() edges List< EdgeView > add to asset file");

			inputNode.OnPortConnected(edge.input as FacialExpressionPortView);
			outputNode.OnPortConnected(edge.output as FacialExpressionPortView);
		}

		public override void Disconnect(Edge edge)
		{
            //Debug.Log("PortView Disconnect()");
            OnDisconnected?.Invoke(this, edge);

			base.Disconnect(edge);

			if (!(edge as EdgeView).isConnected)
				return ;

			var inputNode = (edge.input as FacialExpressionPortView)?.owner;
			var outputNode = (edge.output as FacialExpressionPortView)?.owner;

			inputNode?.OnPortDisconnected(edge.input as FacialExpressionPortView);
			outputNode?.OnPortDisconnected(edge.output as FacialExpressionPortView);

			edges.Remove(edge as EdgeView);
		}

		public void UpdatePortView(PortData data)
		{
            //Debug.Log("PortView UpdatePortView()");
            if (data.displayType != null)
			{
				base.portType = data.displayType;
				portType = data.displayType;
				visualClass = "Port_" + portType.Name;
			}
			if (!String.IsNullOrEmpty(data.displayName))
				base.portName = data.displayName;

			portData = data;

			// Update the edge in case the port color have changed
			schedule.Execute(() => {
				foreach (var edge in edges)
				{
					edge.UpdateEdgeControl();
					edge.MarkDirtyRepaint();
				}
			}).ExecuteLater(50); // Hummm

			UpdatePortSize();
		}

		public List< EdgeView >	GetEdges()
		{
            //Debug.Log("PortView GetEdges()");
            return edges;
		}
	}


    [Serializable]
    public struct JsonElement
    {
        public string type;
        public string jsonDatas;

        public override string ToString()
        {
            return "type: " + type + " | JSON: " + jsonDatas;
        }
    }

    public static class JsonSerializer
    {
        public static JsonElement Serialize(object obj)
        {
            JsonElement elem = new JsonElement();

            elem.type = obj.GetType().AssemblyQualifiedName;
#if UNITY_EDITOR
            elem.jsonDatas = EditorJsonUtility.ToJson(obj);
#else
			elem.jsonDatas = JsonUtility.ToJson(obj);
#endif

            return elem;
        }

        public static T Deserialize<T>(JsonElement e)
        {
            if (typeof(T) != Type.GetType(e.type))
                throw new ArgumentException("Deserializing type is not the same than Json element type");

            var obj = Activator.CreateInstance<T>();
#if UNITY_EDITOR
            EditorJsonUtility.FromJsonOverwrite(e.jsonDatas, obj);
#else
			JsonUtility.FromJsonOverwrite(e.jsonDatas, obj);
#endif

            return obj;
        }

        public static JsonElement SerializeNode(FacialTrackingNode node)
        {
            return Serialize(node);
        }

        public static FacialTrackingNode DeserializeNode(JsonElement e)
        {
            try
            {
                var baseNodeType = Type.GetType(e.type);

                if (e.jsonDatas == null)
                    return null;

                var node = Activator.CreateInstance(baseNodeType) as FacialTrackingNode;
#if UNITY_EDITOR
                EditorJsonUtility.FromJsonOverwrite(e.jsonDatas, node);
#else
				JsonUtility.FromJsonOverwrite(e.jsonDatas, node);
#endif
                return node;
            }
            catch
            {
                return null;
            }
        }
    }
}
#endif