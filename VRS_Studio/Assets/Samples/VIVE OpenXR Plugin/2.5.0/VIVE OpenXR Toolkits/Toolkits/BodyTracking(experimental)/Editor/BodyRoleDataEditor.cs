// Copyright HTC Corporation All Rights Reserved.

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace VIVE.OpenXR.Toolkits.BodyTracking.Editor
{
	[CustomEditor(typeof(BodyRoleData))]
	public class BodyRoleDataEditor : UnityEditor.Editor
	{
		SerializedProperty m_TrackerPose, m_TrackerIndexInputs, m_TrackerTypeInputs;
		private void OnEnable()
		{
			m_TrackerPose = serializedObject.FindProperty("m_TrackerPose");
			m_TrackerIndexInputs = serializedObject.FindProperty("m_TrackerIndexInputs");
			m_TrackerTypeInputs = serializedObject.FindProperty("m_TrackerTypeInputs");
		}
		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			BodyRoleData myScript = target as BodyRoleData;

			EditorGUILayout.HelpBox(
				"Selects the pose source of Tracker, either from a specific type or a RolePoseProvider.",
				MessageType.Info);
			EditorGUILayout.PropertyField(m_TrackerPose);
			if (myScript.TrackerPose == BodyRoleData.TrackerBase.IndexBase)
			{
				EditorGUILayout.PropertyField(m_TrackerIndexInputs);
			}
			else
			{
				EditorGUILayout.PropertyField(m_TrackerTypeInputs);
			}

			serializedObject.ApplyModifiedProperties();
			if (GUI.changed)
				EditorUtility.SetDirty((BodyRoleData)target);
		}

#if !TMPExist
		[InitializeOnLoadMethod]
		static void CheckTextMeshProInstallation()
		{
			EditorUtility.DisplayDialog("TextMeshPro Not Found",
				"The Body Tracking sample needs TextMeshPro.",
				"OK");
		}
#endif
	}
}
#endif
