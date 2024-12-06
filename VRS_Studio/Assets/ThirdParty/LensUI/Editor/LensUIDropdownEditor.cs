using HTC.UnityPlugin.Utility.Switchable;
using UnityEditor;

namespace HTC.Triton.LensUI
{
    [CustomEditor(typeof(LensUIDropdown), true)]
    [CanEditMultipleObjects]
    public class LensUIDropdownEditor : LensUISelectableEditor
    {
        SerializedProperty isOpenProperty;
        SerializedProperty animateTypeProperty;
        SerializedProperty groupProperty;
        SerializedProperty blockerProperty;
        SerializedProperty onValueChangedProperty;

        protected override void OnEnable()
        {
            base.OnEnable();
            isOpenProperty = serializedObject.FindProperty("isOpen");
            groupProperty = serializedObject.FindProperty("group");
            blockerProperty = serializedObject.FindProperty("blocker");
            animateTypeProperty = serializedObject.FindProperty("switchAnimateType");
            onValueChangedProperty = serializedObject.FindProperty("onValueChanged");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(isOpenProperty);
            if (EditorGUI.EndChangeCheck() && EditorApplication.isPlaying)
            {
                var target = serializedObject.targetObject as LensUIDropdown;
                if (target != null)
                {
                    target.Initialize();
                    target.IsOpen = isOpenProperty.boolValue;
                    isOpenProperty.boolValue = target.IsOpen;
                }
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(groupProperty);
            if (EditorGUI.EndChangeCheck() && EditorApplication.isPlaying)
            {
                var target = serializedObject.targetObject as LensUIDropdown;
                if (target != null)
                {
                    target.Initialize();
                    target.GroupBehaviour = groupProperty.objectReferenceValue as SwitchableGroupBehaviour;
                }
            }

            EditorGUILayout.PropertyField(blockerProperty);
            EditorGUILayout.PropertyField(animateTypeProperty);
            EditorGUILayout.PropertyField(onValueChangedProperty);

            serializedObject.ApplyModifiedProperties();
        }
    }
}