// Copyright HTC Corporation All Rights Reserved.using System.Collections.Generic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VIVE.OpenXR.Toolkits.FacialExpression.Maker
{
    [System.Serializable]
    public class FacialExpressionConfigRuntime : ScriptableObject
    {
        [SerializeField]
        public List<FacialTrackingNode> nodes = new List<FacialTrackingNode>();

        [SerializeField]
        public List<SerializableEdgeData> edges = new List<SerializableEdgeData>();
        public Vector3 position = Vector3.zero;

    }
}