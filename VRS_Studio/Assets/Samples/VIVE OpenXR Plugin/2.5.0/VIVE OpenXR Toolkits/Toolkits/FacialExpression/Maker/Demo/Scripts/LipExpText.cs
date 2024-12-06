// "Wave SDK 
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the Wave SDK(s).
// You shall fully comply with all of HTC\u2019s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using UnityEngine;
using UnityEngine.UI;
using VIVE.OpenXR.FacialTracking;

namespace VIVE.OpenXR.Toolkits.FacialExpression.Maker
{
	[RequireComponent(typeof(Text))]
	public class LipExpText : MonoBehaviour
	{
        [SerializeField]
		private XrLipExpressionHTC m_LipExp = XrLipExpressionHTC.XR_LIP_EXPRESSION_JAW_RIGHT_HTC;//LipExp.Max;

        public XrLipExpressionHTC LipExp { get { return m_LipExp; } set { m_LipExp = value; } }

		Text m_Text = null;
        private void Start()
        {
            m_Text = GetComponent<Text>();
        }

        void Update()
        {
            if (m_Text == null) { return; }
            m_Text.text = m_LipExp.ToString() + ": " + FacialExpressionData.LipExpression((XrLipExpressionHTC)m_LipExp).ToString("N5");
        }
	}
}
