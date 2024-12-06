using UnityEditor;
using UnityEngine;

namespace HTC.UnityPlugin.CommonEventVariable
{
    [CustomEditor(typeof(CommonEventBehaviourBase), true)]
    [CanEditMultipleObjects]
    public class CommonEventBehaviourEditor : Editor
    {
        private SerializedProperty scriptProperty;
        private SerializedProperty eventAssetProperty;
        private SerializedProperty onTriggerProperty;

        private void OnEnable()
        {
            scriptProperty = serializedObject.FindProperty("m_Script");
            eventAssetProperty = serializedObject.FindProperty("eventAsset");
            onTriggerProperty = serializedObject.FindProperty("onTrigger");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUI.enabled = false;
            EditorGUILayout.PropertyField(scriptProperty);
            GUI.enabled = true;

            var targetObj = target as CommonEventBehaviourBase;

            if (EditorApplication.isPlaying && targetObj != null)
            {
                EditorGUILayout.LabelField("Event Name", targetObj.HandlerName);
                EditorGUILayout.PropertyField(onTriggerProperty);
            }
            else
            {
                EditorGUILayout.PropertyField(eventAssetProperty);
                EditorGUILayout.PropertyField(onTriggerProperty);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
