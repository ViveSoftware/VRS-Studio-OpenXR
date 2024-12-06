using HTC.UnityPlugin.Utility.Switchable;
using UnityEditor;

namespace HTC.Triton.LensUI
{
    [CustomEditor(typeof(LensUIToggleSwitch), true)]
    [CanEditMultipleObjects]
    public class LensUIToggleSwitchEditor : LensUISelectableEditor
    {
        SerializedProperty isOnProperty;
        SerializedProperty groupProperty;
        SerializedProperty animateTypeProperty;
        SerializedProperty onValueChangedProperty;

        protected override void OnEnable()
        {
            base.OnEnable();
            isOnProperty = serializedObject.FindProperty("isOn");
            groupProperty = serializedObject.FindProperty("group");
            animateTypeProperty = serializedObject.FindProperty("switchAnimateType");
            onValueChangedProperty = serializedObject.FindProperty("onValueChanged");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(isOnProperty);
            if (EditorGUI.EndChangeCheck() && EditorApplication.isPlaying)
            {
                var target = serializedObject.targetObject as LensUIToggleSwitch;
                if (target != null)
                {
                    target.Initialize();
                    target.IsOn = isOnProperty.boolValue;
                    isOnProperty.boolValue = target.IsOn;
                }
            }

            EditorGUILayout.PropertyField(animateTypeProperty);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(groupProperty);
            if (EditorGUI.EndChangeCheck() && EditorApplication.isPlaying)
            {
                var target = serializedObject.targetObject as LensUIToggleSwitch;
                if (target != null)
                {
                    target.Initialize();
                    target.GroupBehaviour = groupProperty.objectReferenceValue as SwitchableGroupBehaviour;
                }
            }

            EditorGUILayout.PropertyField(onValueChangedProperty);

            serializedObject.ApplyModifiedProperties();
        }
    }
}