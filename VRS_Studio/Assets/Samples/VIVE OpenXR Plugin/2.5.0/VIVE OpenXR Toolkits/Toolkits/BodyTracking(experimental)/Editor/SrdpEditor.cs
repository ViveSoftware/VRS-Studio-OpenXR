// Copyright HTC Corporation All Rights Reserved.

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;

namespace VIVE.OpenXR.Toolkits.BodyTracking.RuntimeDependency.Editor
{
	[CustomEditor(typeof(Srdp))]
	public class SrdpEditor : UnityEditor.Editor
	{
		private SerializedProperty m_LeftControllerPose, m_RightControllerPose;
		private ReorderableList reorderableList;
		private bool showInputActions = false;

		private void OnEnable()
		{
			var myScript = (Srdp)target;

			m_LeftControllerPose = serializedObject.FindProperty("m_LeftControllerPose");
			m_RightControllerPose = serializedObject.FindProperty("m_RightControllerPose");

			#region ReorderableList
			reorderableList = new ReorderableList(serializedObject, serializedObject.FindProperty("m_TrackersInputAction"), true, true, true, true);
			reorderableList.drawHeaderCallback = (Rect rect) =>
			{
				EditorGUI.LabelField(rect, "Tracker Input Actions");
			};
			reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
			{
				SerializedProperty element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
				string elementName = "Tracker " + index;

				rect.y += 2;
				EditorGUI.PropertyField(
					new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
					element, new GUIContent(elementName), true);
			};
			reorderableList.elementHeightCallback = (int index) =>
			{
				SerializedProperty element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
				return EditorGUI.GetPropertyHeight(element, true) + 4;
			};
			#endregion
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			EditorGUILayout.PropertyField(m_LeftControllerPose, new GUIContent("Left Controller Pose (Optional)"), true);
			EditorGUILayout.PropertyField(m_RightControllerPose, new GUIContent("Right Controller Pose (Optional)"), true);
			showInputActions = EditorGUILayout.Foldout(showInputActions, "Tracker Settings");
			if (showInputActions)
			{
				reorderableList.DoLayoutList();
			}
			serializedObject.ApplyModifiedProperties();
		}
	}
}
#endif