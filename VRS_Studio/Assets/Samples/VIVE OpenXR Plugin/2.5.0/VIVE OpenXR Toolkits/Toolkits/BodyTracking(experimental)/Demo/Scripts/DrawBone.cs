// Copyright HTC Corporation All Rights Reserved.

using UnityEngine;

namespace VIVE.OpenXR.Toolkits.BodyTracking.Demo
{
	public class DrawBone : MonoBehaviour
	{
		[SerializeField]
		private GameObject m_TargetJoint = null;
		public GameObject TargetJoint { get { return m_TargetJoint; } set { m_TargetJoint = value; } }

		[SerializeField]
		private float m_BoneWidth = 0.001f;
		public float BoneWidth { get { return m_BoneWidth; } set { m_BoneWidth = value; } }

		[SerializeField]
		private Color m_BoneColor = Color.red;
		public Color BoneColor { get { return m_BoneColor; } set { m_BoneColor = value; } }

		private LineRenderer bone = null;

		private Vector3 startPos = Vector3.zero;
		private Vector3 endPos = Vector3.zero;

		void Start()
		{
			if (m_TargetJoint != null)
			{
				bone = gameObject.AddComponent<LineRenderer>();
#if UNITY_2019_1_OR_NEWER
				bone.material = new Material(Shader.Find("Legacy Shaders/Particles/Additive"));
#else
				bone.material = new Material (Shader.Find ("Particles/Additive"));
#endif

#if UNITY_5_6_OR_NEWER
				bone.positionCount = 2;
#else
				bone.SetVertexCount (2);
#endif
			}
		}

		void Update()
		{
			if (bone == null)
				return;

			if (m_TargetJoint.GetComponent<MeshRenderer>() != null)
			{
				bone.enabled = m_TargetJoint.GetComponent<MeshRenderer>().enabled;
			}

			startPos = transform.position;
			endPos = m_TargetJoint.transform.position;

			bone.startColor = m_BoneColor;
			bone.endColor = m_BoneColor;
			bone.startWidth = m_BoneWidth;
			bone.endWidth = m_BoneWidth;
			bone.SetPosition(0, startPos);
			bone.SetPosition(1, endPos);
		}
	}
}
