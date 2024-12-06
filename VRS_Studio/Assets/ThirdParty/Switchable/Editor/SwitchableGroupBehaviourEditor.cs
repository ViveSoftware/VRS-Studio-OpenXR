using UnityEditor;

namespace HTC.UnityPlugin.Utility.Switchable
{
    [CustomEditor(typeof(SwitchableGroupBehaviour), true)]
    [CanEditMultipleObjects]
    public class SwitchableGroupBehaviourEditor : Editor
    {
        SerializedProperty sequentialSwitchProperty;
        SerializedProperty allowSwitchOffProperty;

        protected virtual void OnEnable()
        {
            sequentialSwitchProperty = serializedObject.FindProperty("sequentialSwitch");
            allowSwitchOffProperty = serializedObject.FindProperty("allowSwitchOff");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(sequentialSwitchProperty);
            if (EditorGUI.EndChangeCheck() && EditorApplication.isPlaying)
            {
                var target = serializedObject.targetObject as SwitchableGroupBehaviour;
                if (target != null)
                {
                    target.Initialize();
                    target.SequentialSwitch = sequentialSwitchProperty.boolValue;
                }
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(allowSwitchOffProperty);
            if (EditorGUI.EndChangeCheck() && EditorApplication.isPlaying)
            {
                var target = serializedObject.targetObject as SwitchableGroupBehaviour;
                if (target != null)
                {
                    target.Initialize();
                    target.AllowSwitchOff = allowSwitchOffProperty.boolValue;
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}