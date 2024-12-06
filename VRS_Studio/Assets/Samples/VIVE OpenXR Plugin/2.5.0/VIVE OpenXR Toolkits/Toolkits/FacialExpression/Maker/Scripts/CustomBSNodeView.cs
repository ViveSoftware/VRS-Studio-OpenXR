// Copyright HTC Corporation All Rights Reserved.
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using VIVE.OpenXR.Toolkits.FacialExpression.Maker.Editor;

[NodeCustomEditor(typeof(CustomBSNode))]
public class CustomBSNodeView : FacialTrackingGraphNode
{
    public List<CustomStatePortData> ListofCustomStateData = new List<CustomStatePortData>();
    public override void Enable()
	{
        style.width = 240;
        var floatNode = nodeTarget as CustomBSNode;
        if (floatNode == null) { return; }

        //Debug.Log("FloatNodeView Enable() " + floatNode.name);
        //BaseNode.SetCustomName("Fcl_Eye_Blink");


        //var container = new VisualElement();
        // container.Add(new IMGUIContainer(() =>
        //{
        //    float tmp = EditorGUILayout.Slider("newvalue", 0, 0.0f, 100.0f);
        //    floatNode.weight = Mathf.Clamp(tmp, 0, 100);
        //}));

        //float val = 100.0f;
        //var _Label = new Label();
        //_Label.name = floatNode.name;
        //_Label.text = "Weight:"+val;
        ////_Label.RegisterValueChangedCallback(ValueTuple =>
        ////{//get label text as node name need 1 first blank
        ////    Debug.Log("FloatNodeView Labe RegisterValueChangedCallback");
        ////    //owner.RegisterCompleteObjectUndo("Updated label");
        ////    //floatNode.Custom = val;
        ////});

        //UnityEngine.UIElements.Slider _slider = new UnityEngine.UIElements.Slider(0, 100, SliderDirection.Horizontal) { /*value = Int32.Parse(floatNode.Custom)*/ };
        //_slider.value = val;
        ////_slider.RegisterValueChangedCallback(v =>
        ////{
        ////    //slider component register undo doesn't work
        ////    Debug.Log("FloatNodeView Slider RegisterValueChangedCallback");
        ////    owner.RegisterCompleteObjectUndo("Updated slider");
        ////    val =_slider.value = Mathf.RoundToInt(v.newValue);
        ////    //floatNode.Custom = val;
        ////    _Label.text = "Weight:" + val;
        ////});
        //ListofCustomStateData.Add(new CustomStatePortData(_Label, _slider, owner));

        //bottomPortContainer.Add(_Label);
        //bottomPortContainer.Add(_slider);


        DoubleField floatField = new DoubleField
        {
            value = floatNode.weight
        };

        floatNode.onProcessed += () => floatField.value = floatNode.weight;

        floatField.RegisterValueChangedCallback((v) =>
        {
            owner.RegisterCompleteObjectUndo("Updated floatNode input");
            //floatNode.Custom = nodeTarget.nodeCustomName;//(float)v.newValue;
            //_Label.text = "Weight:" + v.newValue;
            nodeTarget.weight = (float)v.newValue;//floatNode.Custom;
        });
        //controlsContainer.Add(floatField);
        bottomPortContainer.Add(floatField);


        // test multiple field redo/undo
        //IntegerField intField = new IntegerField { value = (int)floatNode.Custom };
        //intField.RegisterValueChangedCallback((v) =>
        //{
        //    Debug.Log("FloatNodeView IntegerField RegisterValueChangedCallback");
        //    owner.RegisterCompleteObjectUndo("Updated floatNode integer");
        //    floatNode.Custom = (float)v.newValue;
        //});
        //bottomPortContainer.Add(intField);
    }



    #region customFacialPortElement
    public class CustomStatePortData
    {
        public Label LinkingLabel { get; set; }
        public UnityEngine.UIElements.Slider LinkingSlider { get; set; }
        public FacialTrackingGraphView owner { get; set; }

        public CustomStatePortData(Label _Label, UnityEngine.UIElements.Slider _Slider, FacialTrackingGraphView _owner)
        {
            LinkingLabel = _Label;
            LinkingSlider = _Slider;
            owner= _owner;

            //if (LinkingImage.image != null)
            {
                LinkingSlider.RegisterValueChangedCallback(v =>
                {
                    //slider component register undo doesn't work
                    Debug.Log("FloatNodeView Slider RegisterValueChangedCallback");
                    //owner.RegisterCompleteObjectUndo("Updated slider");
                    //val = v.value = Mathf.RoundToInt(v.newValue);
                    //floatNode.Custom = val;
                    _Label.text = "Weight:" + (Mathf.RoundToInt(v.newValue).ToString());
                });
            }
        }
    }
    #endregion
}
#endif