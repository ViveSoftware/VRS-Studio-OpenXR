// Copyright HTC Corporation All Rights Reserved.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VIVE.OpenXR.Toolkits.FacialExpression.Maker;
using System.Linq;
using UnityEditor;
using UnityEngine.UI;
using VIVE.OpenXR.FacialTracking;
using VIVE.OpenXR.Toolkits.FacialExpression;

#if UNITY_EDITOR

//[System.Serializable, NodeMenuItem("MultiPortData")]
public class OriginalExpressionNode : FacialTrackingNode
{
    //[Input(name = "In Values", allowMultiple = true)]
    //public IEnumerable< object >	inputs = null;

    [Output]
    //public float outputs;
    public IEnumerable<object> OriginalExpression = null;

    static PortData[] portDatas = new PortData[(int)XrEyeExpressionHTC.XR_EYE_EXPRESSION_MAX_ENUM_HTC + (int)XrLipExpressionHTC.XR_LIP_EXPRESSION_MAX_ENUM_HTC];

    public override bool deletable => false;
    public override bool duplicatable => false;

    public override string		name => "OriginalExpressionState"; //nodename



    protected override void Process()
	{
        //Debug.Log("CustomPortDataNode Process()");
        //output = 0;

        //if (inputs == null)
        //    return;

        //foreach (float input in inputs)
        //    output += input;
    }

    //[CustomPortBehavior(nameof(inputs))]
	//IEnumerable< PortData > GetPortsForInputs(List< SerializableEdge > edges)
	//{
        //Debug.Log("CustomPortDataNode GetPortsForInputs() from");
        //PortData pd = new PortData();

		//foreach (var portData in portDatas)
		//{
        //    yield return portData;
		//}
	//}
	[CustomPortBehavior(nameof(OriginalExpression))]
	IEnumerable< PortData > GetPortsForOutput(List<FacialExpressionSerializableEdge> edges)
	{
        if (BuildPipeline.isBuildingPlayer || EditorApplication.isCompiling) yield return null;
        //Debug.Log("CustomPortDataNode GetPortsForOutput() ");
        int i = 0;
        for (i = 0; i < ((int)XrEyeExpressionHTC.XR_EYE_EXPRESSION_MAX_ENUM_HTC); i++)
        {
            //var imgPath = FindHintPic(i, isEye: true);
            //if (!string.IsNullOrEmpty(imgPath))
            //_Img.image = AssetDatabase.LoadAssetAtPath<Texture2D>(imgPath);
            portDatas[i] = new PortData
            {
                displayName = ((XrEyeExpressionHTC)i).ToString(),
                //sizeInPixel = 10, //port circle size
                displayType = typeof(float),
                identifier = i.ToString(),
                acceptMultipleEdges = true,
                /*tooltip = "Wave Eye expression"+ i.ToString(),*/
                thumbnail = FindHintPic(i, isEye: true)//imgPath.ToString()
            };
            yield return portDatas[i];
        }
        for (int j = 0; j < ((int)XrLipExpressionHTC.XR_LIP_EXPRESSION_MAX_ENUM_HTC); j++,i++)
        {
            //var imgPath = FindHintPic(j, isEye: false);
            portDatas[i] = new PortData
            {
                displayName = ((XrLipExpressionHTC)j).ToString(),
                //sizeInPixel = 10, //port circle size
                displayType = typeof(float),
                identifier = i.ToString(),
                acceptMultipleEdges = true,
                /*tooltip = "wave Lip expression"+ j.ToString(),*/
                thumbnail = FindHintPic(j, isEye: false)//imgPath.ToString()
            };
            yield return portDatas[i];
        }

        //yield return new PortData{ displayName = "Out 0", displayType = typeof(float), identifier = "0" };
        //yield return new PortData{ displayName = "Out 1", displayType = typeof(Color), identifier = "1" };
        //yield return new PortData{ displayName = "Out 2", displayType = typeof(Vector4), identifier = "2" };
        //yield return new PortData{ displayName = "Out 3", displayType = typeof(GameObject), identifier = "3" };
    }

    //[CustomPortInput(nameof(inputs), typeof(float), allowCast = true)]
	public void GetOutputs(List<FacialExpressionSerializableEdge> edges)
	{
        //Debug.Log("CustomPortDataNode GetOutputs() ");
        // inputs = edges.Select(e => (float)e.passThroughBuffer);
    }

    public string FindHintPic(int i, bool isEye)
    {
        //_Img.image = LoadPNG(_FilePath + "/Packages/com.htc.upm.wave.openxr.toolkit/Editor/FacialExpressionMaker/VIVEFacialTrackingGraphHintPics/EYE_" + i + ".png");
        //_Img.image = Resources.Load<Texture2D>("VIVEFacialTrackingGraphHintPics/EYE_" + i );

        string name = (isEye ? "EYE_" : "LIP_") + i;
        string[] guids = AssetDatabase.FindAssets(name + " t:texture");
        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            // filter the exact name
            if (path.Contains("GraphHintPicsNew/" + name + "."))
            {
                return path;
            }
        }
        return "";
    }
}
#endif