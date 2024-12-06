using UnityEditor;

namespace HTC.UnityPlugin.Utility.Switchable
{
    [CustomEditor(typeof(SwitchableBehaviour), true)]
    [CanEditMultipleObjects]
    public class SwitchableBehaviourEditor : Editor
    {
        SerializedProperty isOpenProperty;
        SerializedProperty allowInterruptProperty;
        SerializedProperty groupBehaviourProperty;

        protected virtual void OnEnable()
        {
            isOpenProperty = serializedObject.FindProperty("isOpen");
            allowInterruptProperty = serializedObject.FindProperty("allowInterrupt");
            groupBehaviourProperty = serializedObject.FindProperty("groupBehaviour");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(isOpenProperty);
            if (EditorGUI.EndChangeCheck() && EditorApplication.isPlaying)
            {
                var target = serializedObject.targetObject as SwitchableBehaviour;
                if (target != null)
                {
                    target.Initialize();

                    if (isOpenProperty.boolValue)
                    {
                        if (target.isActiveAndEnabled)
                        { target.OpenStart(); }
                        else
                        { target.SnapOpen(); }
                    }
                    else
                    {
                        if (target.isActiveAndEnabled)
                        { target.CloseStart(); }
                        else
                        { target.SnapClose(); }
                    }
                    isOpenProperty.boolValue = target.IsOpen;
                }
            }

            EditorGUILayout.PropertyField(allowInterruptProperty);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(groupBehaviourProperty);
            if (EditorGUI.EndChangeCheck() && EditorApplication.isPlaying)
            {
                var target = serializedObject.targetObject as SwitchableBehaviour;
                if (target != null)
                {
                    target.Initialize();
                    target.GroupBehaviour = groupBehaviourProperty.objectReferenceValue as SwitchableGroupBehaviour;
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}