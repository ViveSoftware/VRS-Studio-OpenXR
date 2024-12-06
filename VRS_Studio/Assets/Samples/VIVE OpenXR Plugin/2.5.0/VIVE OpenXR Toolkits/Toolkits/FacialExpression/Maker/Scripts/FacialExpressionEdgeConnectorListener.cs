// Copyright HTC Corporation All Rights Reserved.
//using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//using UnityEngine.UIElements;
//using UnityEditor;

#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
using VIVE.OpenXR.Toolkits.FacialExpression.Maker.Editor;

namespace VIVE.OpenXR.Toolkits.FacialExpression.Maker
{
    /// <summary>
    /// Base class to write your own edge handling connection system
    /// </summary>
    public class FacialExpressionEdgeConnectorListener : IEdgeConnectorListener
    {
        public readonly FacialTrackingGraphView graphView;

        Dictionary< Edge, FacialExpressionPortView >    edgeInputPorts = new Dictionary< Edge, FacialExpressionPortView >();
        Dictionary< Edge, FacialExpressionPortView >    edgeOutputPorts = new Dictionary< Edge, FacialExpressionPortView >();

        //static CreateNodeMenuWindow     edgeNodeCreateMenuWindow;

        public FacialExpressionEdgeConnectorListener(FacialTrackingGraphView graphView)
        {
            this.graphView = graphView;
        }

        public virtual void OnDropOutsidePort(Edge edge, Vector2 position)
        {
			this.graphView.RegisterCompleteObjectUndo("Disconnect edge");

			//If the edge was already existing, remove it
			if (!edge.isGhostEdge)
				graphView.Disconnect(edge as EdgeView);

            // when on of the port is null, then the edge was created and dropped outside of a port
            if (edge.input == null || edge.output == null)
                ShowNodeCreationMenuFromEdge(edge as EdgeView, position);
        }

        public virtual void OnDrop(GraphView graphView, Edge edge)
        {
			var edgeView = edge as EdgeView;
            bool wasOnTheSamePort = false;

			if (edgeView?.input == null || edgeView?.output == null)
				return ;

			//If the edge was moved to another port
			if (edgeView.isConnected)
			{
                if (edgeInputPorts.ContainsKey(edge) && edgeOutputPorts.ContainsKey(edge))
                    if (edgeInputPorts[edge] == edge.input && edgeOutputPorts[edge] == edge.output)
                        wasOnTheSamePort = true;

                if (!wasOnTheSamePort)
                    this.graphView.Disconnect(edgeView);
			}

            if (edgeView.input.node == null || edgeView.output.node == null)
                return;

            edgeInputPorts[edge] = edge.input as FacialExpressionPortView;
            edgeOutputPorts[edge] = edge.output as FacialExpressionPortView;
            try
            {

                //Debug.Log("BaseEdgeConnectorListener Connected OnDrop() (" + edgeView.input.node.name + ") and (" + edgeView.output.node.name+")");
                this.graphView.RegisterCompleteObjectUndo("Connected " + edgeView.input.node.name + " and " + edgeView.output.node.name);
                if (!this.graphView.Connect(edge as EdgeView, autoDisconnectInputs: !wasOnTheSamePort))
                    this.graphView.Disconnect(edge as EdgeView);
                PrintConnectedPortsInfo(graphView);
            } catch (System.Exception)
            {
                this.graphView.Disconnect(edge as EdgeView);
            }
        }


        void PrintConnectedPortsInfo(GraphView graphView)
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

                //Debug.Log($"Edge connects '{_AllEdges.Count}' port(out) '{outputPortName}' of nodeA '{nodeAName}' to port(in) '{inputPortName}' of nodeB '{nodeBName}'");
            }

        }

        void ShowNodeCreationMenuFromEdge(EdgeView edgeView, Vector2 position)
        {
            //if (edgeNodeCreateMenuWindow == null)
                //edgeNodeCreateMenuWindow = ScriptableObject.CreateInstance< CreateNodeMenuWindow >();

            //edgeNodeCreateMenuWindow.Initialize(graphView, EditorWindow.focusedWindow, edgeView);
			//SearchWindow.Open(new SearchWindowContext(position + EditorWindow.focusedWindow.position.position), edgeNodeCreateMenuWindow);
        }
    }
}
#endif