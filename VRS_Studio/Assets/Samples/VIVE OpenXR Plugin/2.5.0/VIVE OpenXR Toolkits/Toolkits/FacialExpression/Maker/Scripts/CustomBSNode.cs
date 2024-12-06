// Copyright HTC Corporation All Rights Reserved.
using UnityEngine;
using VIVE.OpenXR.Toolkits.FacialExpression;
using VIVE.OpenXR.Toolkits.FacialExpression.Maker;
using System.Collections.Generic;

[System.Serializable, NodeMenuItem("CustomBSNode")]
public class CustomBSNode : FacialTrackingNode
{
    //[Output, SerializeField]
    //public float		output;
    //public IEnumerable<object> output;

    [Input] //must have declare for port
    public string Custom;//= 100.0f;
    //public IEnumerable<object> input; //auto generate new port by ListPortBehavior when port connected

    public override string name => "CustomExpression";

    protected override void Process() { }
}