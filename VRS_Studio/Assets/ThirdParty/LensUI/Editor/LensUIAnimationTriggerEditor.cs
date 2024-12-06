using UnityEditor;

namespace HTC.Triton.LensUI
{
    [CustomEditor(typeof(LensUIAnimationTrigger), true)]
    [CanEditMultipleObjects]
    public class LensUIAnimationTriggerEditor : Editor
    {
        SerializedProperty animatorProperty;
        SerializedProperty keepStateOnDisableProperty;
        SerializedProperty normalTriggerProperty;
        SerializedProperty highlightedTriggerProperty;
        SerializedProperty pressedTriggerProperty;
        SerializedProperty selectedTriggerProperty;
        SerializedProperty disabledTriggerProperty;
        private static bool foldout;

        protected virtual void OnEnable()
        {
            animatorProperty = serializedObject.FindProperty("animator");
            keepStateOnDisableProperty = serializedObject.FindProperty("keepStateOnDisable");
            normalTriggerProperty = serializedObject.FindProperty("normalTrigger");
            highlightedTriggerProperty = serializedObject.FindProperty("highlightedTrigger");
            pressedTriggerProperty = serializedObject.FindProperty("pressedTrigger");
            selectedTriggerProperty = serializedObject.FindProperty("selectedTrigger");
            disabledTriggerProperty = serializedObject.FindProperty("disabledTrigger");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(animatorProperty);
            if (EditorGUI.EndChangeCheck() && EditorApplication.isPlaying)
            {
                var target = serializedObject.targetObject as LensUIAnimationTrigger;
                if (target != null)
                    target.SetAnimatorDirty();
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(keepStateOnDisableProperty);
            if (EditorGUI.EndChangeCheck() && EditorApplication.isPlaying)
            {
                var target = serializedObject.targetObject as LensUIAnimationTrigger;
                if (target != null)
                    target.KeepStateOnDisable = keepStateOnDisableProperty.boolValue;
            }

            if (foldout = EditorGUILayout.Foldout(foldout, "Triggers"))
            {
                var prevIndent = EditorGUI.indentLevel;
                EditorGUI.indentLevel += 1;
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.DelayedTextField(normalTriggerProperty);
                    if (EditorGUI.EndChangeCheck() && EditorApplication.isPlaying)
                    {
                        var target = serializedObject.targetObject as LensUIAnimationTrigger;
                        if (target != null)
                            target.NormalTrigger = normalTriggerProperty.stringValue;
                    }

                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.DelayedTextField(highlightedTriggerProperty);
                    if (EditorGUI.EndChangeCheck() && EditorApplication.isPlaying)
                    {
                        var target = serializedObject.targetObject as LensUIAnimationTrigger;
                        if (target != null)
                            target.HighlightedTrigger = highlightedTriggerProperty.stringValue;
                    }

                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.DelayedTextField(pressedTriggerProperty);
                    if (EditorGUI.EndChangeCheck() && EditorApplication.isPlaying)
                    {
                        var target = serializedObject.targetObject as LensUIAnimationTrigger;
                        if (target != null)
                            target.PressedTrigger = pressedTriggerProperty.stringValue;
                    }

                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.DelayedTextField(selectedTriggerProperty);
                    if (EditorGUI.EndChangeCheck() && EditorApplication.isPlaying)
                    {
                        var target = serializedObject.targetObject as LensUIAnimationTrigger;
                        if (target != null)
                            target.SelectedTrigger = selectedTriggerProperty.stringValue;
                    }

                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.DelayedTextField(disabledTriggerProperty);
                    if (EditorGUI.EndChangeCheck() && EditorApplication.isPlaying)
                    {
                        var target = serializedObject.targetObject as LensUIAnimationTrigger;
                        if (target != null)
                            target.DisabledTrigger = disabledTriggerProperty.stringValue;
                    }
                }
                EditorGUI.indentLevel = prevIndent;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}