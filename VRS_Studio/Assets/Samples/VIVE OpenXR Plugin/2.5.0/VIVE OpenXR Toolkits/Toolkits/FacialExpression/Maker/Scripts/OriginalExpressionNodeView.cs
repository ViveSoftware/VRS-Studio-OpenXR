// Copyright HTC Corporation All Rights Reserved.
//using VIVE.OpenXR.Toolkits.FacialExpression.Maker;
//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
using VIVE.OpenXR.Toolkits.FacialExpression.Maker.Editor;

[NodeCustomEditor(typeof(OriginalExpressionNode))]
public class OriginalExpressionNodeView : FacialTrackingGraphNode
{
    public override void Enable()
    {
        Debug.Log("CustomPortDataNodeView Enable()");
        style.width = 420;
        var node = nodeTarget as OriginalExpressionNode;

        //var textArea = new TextField(-1, true, false, '*') { value = node.output };
        //textArea.Children().First().style.unityTextAlign = TextAnchor.UpperLeft;
        //textArea.style.width = 200;
        //textArea.style.height = 100;
        //textArea.RegisterValueChangedCallback(v => {
        //    owner.RegisterCompleteObjectUndo("Edit string node");
        //    node.output = v.newValue;
        //});
        //controlsContainer.Add(textArea);
        //bottomPortContainer.style.width = 422;
    }
}
#endif