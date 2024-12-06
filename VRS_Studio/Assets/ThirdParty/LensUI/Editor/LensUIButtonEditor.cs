using UnityEditor;
using UnityEngine;

namespace HTC.Triton.LensUI
{
    [CustomEditor(typeof(LensUIButton), true)]
    [CanEditMultipleObjects]
    public class LensUIButtonEditor : LensUISelectableEditor
    {
        SerializedProperty repeatProperty;
        SerializedProperty repeatAnimationProperty;
        SerializedProperty repeatDelayProperty;
        SerializedProperty repeatIntervalProperty;
        SerializedProperty onClickProperty;

        protected override void OnEnable()
        {
            base.OnEnable();
            repeatProperty = serializedObject.FindProperty("repeat");
            repeatAnimationProperty = serializedObject.FindProperty("repeatAnimation");
            repeatDelayProperty = serializedObject.FindProperty("repeatDelay");
            repeatIntervalProperty = serializedObject.FindProperty("repeatInterval");
            onClickProperty = serializedObject.FindProperty("_onClick");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            serializedObject.Update();

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(repeatProperty);

            if (repeatProperty.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(repeatAnimationProperty, new GUIContent("Animation"));
                EditorGUILayout.PropertyField(repeatDelayProperty, new GUIContent("Delay"));
                EditorGUILayout.PropertyField(repeatIntervalProperty, new GUIContent("Interval"));
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(onClickProperty);

            serializedObject.ApplyModifiedProperties();
        }
    }
}