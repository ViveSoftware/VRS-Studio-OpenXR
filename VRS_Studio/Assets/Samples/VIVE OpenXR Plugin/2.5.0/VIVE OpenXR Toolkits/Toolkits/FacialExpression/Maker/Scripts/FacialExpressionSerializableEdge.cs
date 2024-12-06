// Copyright HTC Corporation All Rights Reserved.
//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

namespace VIVE.OpenXR.Toolkits.FacialExpression.Maker
{
    [System.Serializable]
    public class FacialExpressionSerializableEdge : ISerializationCallbackReceiver
    {
        public string GUID;

        [SerializeField]
        FacialExpressionConfig owner;

        [SerializeField]
        string inputNodeGUID;
        [SerializeField]
        string outputNodeGUID;

        [System.NonSerialized]
        public FacialTrackingNode inputNode;

        [System.NonSerialized]
        public FacialExpressionNodePort inputPort;
        [System.NonSerialized]
        public FacialExpressionNodePort outputPort;

        //temporary object used to send port to port data when a custom input/output function is used.
        [System.NonSerialized]
        public object passThroughBuffer;

        [System.NonSerialized]
        public FacialTrackingNode outputNode;

        public string inputFieldName;
        public string outputFieldName;

        // Use to store the id of the field that generate multiple ports
        public string inputPortIdentifier;
        public string outputPortIdentifier;

        public string blendShapeName = "";
        public float weightValue = 100.0f;

        public FacialExpressionSerializableEdge() { }

        public static FacialExpressionSerializableEdge CreateNewEdge(FacialExpressionConfig graph, FacialExpressionNodePort inputPort, FacialExpressionNodePort outputPort)
        {
            //Debug.Log("Maker SerializableEdge CreateNewEdge() callback from BaseGraph"+inputPort);
	    //Debug.Log("Maker SerializableEdge CreateNewEdge() callback from BaseGraph"+outputPort);
            FacialExpressionSerializableEdge edge = new FacialExpressionSerializableEdge();

            edge.owner = graph;
            edge.GUID = System.Guid.NewGuid().ToString();
            edge.inputNode = inputPort.owner;
            edge.inputFieldName = inputPort.fieldName;
            //edge.inputFieldName = inputPort.portData.displayName;
            edge.outputNode = outputPort.owner;
            edge.outputFieldName = outputPort.fieldName;
            //edge.outputFieldName = outputPort.portData.displayName;
            edge.inputPort = inputPort;
            edge.outputPort = outputPort;
            edge.inputPortIdentifier = inputPort.portData.identifier; //must have and need unique
            edge.outputPortIdentifier = outputPort.portData.identifier;
            edge.blendShapeName = inputPort.owner.nodeCustomName;
            edge.weightValue = inputPort.owner.weight;

            //Debug.Log("Maker SerializableEdge CreateNewEdge() callback from BaseGraph" + edge);
            return edge;
        }

        public void OnBeforeSerialize()
        {
            //Debug.Log("Maker SerializableEdge OnBeforeSerialize() callback many times in editor");
            if (outputNode == null || inputNode == null)
                return;

            outputNodeGUID = outputNode.GUID;
            inputNodeGUID = inputNode.GUID;
        }

        public void OnAfterDeserialize() { }

        //here our owner have been deserialized
        public void Deserialize()
        {
            //Debug.Log("Maker SerializableEdge Deserialize() ");
            if (!owner.nodesPerGUID.ContainsKey(outputNodeGUID) || !owner.nodesPerGUID.ContainsKey(inputNodeGUID))
                return;

            outputNode = owner.nodesPerGUID[outputNodeGUID];
            inputNode = owner.nodesPerGUID[inputNodeGUID];
            inputPort = inputNode.GetPort(inputFieldName, inputPortIdentifier);
            outputPort = outputNode.GetPort(outputFieldName, outputPortIdentifier);
        }

        public override string ToString() => $"{outputNode.name}:{outputPort.fieldName} -> {inputNode.name}:{inputPort.fieldName}";
    }

    [System.Serializable]
    public class SerializableEdgeData
    {
        public string GUID;
        //[System.NonSerialized]
        FacialExpressionConfig owner;
        //[SerializeField] string inputNodeGUID;
        //[SerializeField] string outputNodeGUID;

        [System.NonSerialized] public FacialTrackingNode inputNode;
        public FacialExpressionNodePort inputPort;
        public FacialExpressionNodePort outputPort;
        [System.NonSerialized] public FacialTrackingNode outputNode;

        public string inputFieldName;
        public string outputFieldName;

        // Use to store the id of the field that generate multiple ports
        public string inputPortIdentifier;
        public string outputPortIdentifier;

        public string blendShapeName = "";
        public float weightValue = 100.0f;

        public SerializableEdgeData() { }
        public static SerializableEdgeData SerializableEdgeDataNew(FacialExpressionSerializableEdge e, FacialExpressionConfig graph) {
            SerializableEdgeData edge = new SerializableEdgeData();
            edge.GUID = e.GUID;
            edge.owner = graph;


            edge.inputNode = e.inputPort.owner;
            edge.inputFieldName = e.inputPort.fieldName;
            //edge.inputFieldName = inputPort.portData.displayName;
            edge.outputNode = e.outputPort.owner;
            edge.outputFieldName = e.outputPort.fieldName;
            //edge.outputFieldName = outputPort.portData.displayName;
            edge.inputPort = e.inputPort;
            edge.outputPort = e.outputPort;
            edge.inputPortIdentifier = e.inputPort.portData.identifier; //must have and need unique
            edge.outputPortIdentifier = e.outputPort.portData.identifier;
            edge.blendShapeName = e.inputPort.owner.nodeCustomName;
            edge.weightValue = e.inputPort.owner.weight;
            return edge;
        }
    }
}
