using UnityEditor;
using UnityEngine;

namespace HTC.UnityPlugin.Utility.Switchable
{
    [CustomEditor(typeof(SwitchableAnimationTrigger), true)]
    [CanEditMultipleObjects]
    public class SwitchableAnimationTriggerEditor : Editor
    {
        SerializedProperty animatorProperty;
        SerializedProperty keepStateOnDisableProperty;
        SerializedProperty openStartTriggerProperty;
        SerializedProperty openSnapTriggerProperty;
        SerializedProperty closeStartTriggerProperty;
        SerializedProperty closeSnapTriggerProperty;
        private static bool foldout;

        protected virtual void OnEnable()
        {
            animatorProperty = serializedObject.FindProperty("animator");
            keepStateOnDisableProperty = serializedObject.FindProperty("keepStateOnDisable");
            openStartTriggerProperty = serializedObject.FindProperty("openStartTrigger");
            openSnapTriggerProperty = serializedObject.FindProperty("openSnapTrigger");
            closeStartTriggerProperty = serializedObject.FindProperty("closeStartTrigger");
            closeSnapTriggerProperty = serializedObject.FindProperty("closeSnapTrigger");
        }

        private bool ShouldShowSetupDelegateButton(out SwitchableAnimationTrigger thisTarget, out GameObject delegateGo, out SwitchableAnimationTriggerDelegate delegateComp)
        {
            thisTarget = serializedObject.targetObject as SwitchableAnimationTrigger;
            if (thisTarget == null)
            {
                delegateGo = null;
                delegateComp = null;
                return false;
            }

            var animator = thisTarget.Animator;
            if (animator == null) { delegateGo = null; delegateComp = null; return false; }

            delegateGo = animator.gameObject;
            if (thisTarget.gameObject == delegateGo) { delegateComp = null; return false; }

            delegateComp = delegateGo.GetComponent<SwitchableAnimationTriggerDelegate>();
            if (delegateComp == null) { return true; }

            return delegateComp.AnimationTrigger != thisTarget;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(animatorProperty);
            if (EditorGUI.EndChangeCheck() && EditorApplication.isPlaying)
            {
                var target = serializedObject.targetObject as SwitchableAnimationTrigger;
                if (target != null)
                    target.SetAnimatorDirty();
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(keepStateOnDisableProperty);
            if (EditorGUI.EndChangeCheck() && EditorApplication.isPlaying)
            {
                var target = serializedObject.targetObject as SwitchableAnimationTrigger;
                if (target != null)
                    target.KeepStateOnDisable = keepStateOnDisableProperty.boolValue;
            }

            if (foldout = EditorGUILayout.Foldout(foldout, "Triggers"))
            {
                EditorGUI.indentLevel++;
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.DelayedTextField(openStartTriggerProperty);
                    if (EditorGUI.EndChangeCheck() && EditorApplication.isPlaying)
                    {
                        var target = serializedObject.targetObject as SwitchableAnimationTrigger;
                        if (target != null)
                            target.OpenStartTrigger = openStartTriggerProperty.stringValue;
                    }

                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.DelayedTextField(openSnapTriggerProperty);
                    if (EditorGUI.EndChangeCheck() && EditorApplication.isPlaying)
                    {
                        var target = serializedObject.targetObject as SwitchableAnimationTrigger;
                        if (target != null)
                            target.OpenSnapTrigger = openSnapTriggerProperty.stringValue;
                    }

                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.DelayedTextField(closeStartTriggerProperty);
                    if (EditorGUI.EndChangeCheck() && EditorApplication.isPlaying)
                    {
                        var target = serializedObject.targetObject as SwitchableAnimationTrigger;
                        if (target != null)
                            target.CloseStartTrigger = closeStartTriggerProperty.stringValue;
                    }

                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.DelayedTextField(closeSnapTriggerProperty);
                    if (EditorGUI.EndChangeCheck() && EditorApplication.isPlaying)
                    {
                        var target = serializedObject.targetObject as SwitchableAnimationTrigger;
                        if (target != null)
                            target.CloseSnapTrigger = closeSnapTriggerProperty.stringValue;
                    }
                }
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();

            if (ShouldShowSetupDelegateButton(out var thisTarget, out var delegateGo, out var delegateComp))
            {
                var buttonRect = EditorGUILayout.GetControlRect();
                var padding = EditorGUIUtility.labelWidth * 0.2f;
                buttonRect.xMin += padding;
                buttonRect.xMax -= padding;
                if (GUI.Button(buttonRect, new GUIContent("Setup Animation Trigger Delegate", "Setup to catch OpenEnd/CloseEnd event from the animatior")))
                {
                    if (delegateComp == null)
                    {
                        delegateComp = delegateGo.AddComponent<SwitchableAnimationTriggerDelegate>();
                    }

                    delegateComp.AnimationTrigger = thisTarget;
                }
            }
        }
    }
}