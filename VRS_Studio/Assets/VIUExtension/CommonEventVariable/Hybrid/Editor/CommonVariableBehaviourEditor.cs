using UnityEditor;
using UnityEngine;

namespace HTC.UnityPlugin.CommonEventVariable
{
    [CustomEditor(typeof(CommonVariableBehaviourBase), true)]
    [CanEditMultipleObjects]
    public class CommonVariableBehaviourEditor : Editor
    {
        private SerializedProperty scriptProperty;
        private SerializedProperty variableAssetProperty;
        private SerializedProperty onChangeProperty;

        private void OnEnable()
        {
            scriptProperty = serializedObject.FindProperty("m_Script");
            variableAssetProperty = serializedObject.FindProperty("variableAsset");
            onChangeProperty = serializedObject.FindProperty("onChange");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUI.enabled = false;
            EditorGUILayout.PropertyField(scriptProperty);
            GUI.enabled = true;

            var targetObj = target as CommonVariableBehaviourBase;

            if (EditorApplication.isPlaying && targetObj != null)
            {
                EditorGUILayout.LabelField("Variable Name", targetObj.HandlerName);
                EditorGUILayout.PropertyField(onChangeProperty);
            }
            else
            {
                EditorGUILayout.PropertyField(variableAssetProperty);
                EditorGUILayout.PropertyField(onChangeProperty);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
