using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HTC.UnityPlugin.CommonEventVariable
{
    [CustomEditor(typeof(CommonVariableAssetBase), true)]
    [CanEditMultipleObjects]
    public class CommonVariableAssetEditor : Editor
    {
        private const float REPAINT_INTERVAL = 0.3f;
        private static float nextRepaintTime;
        private static int handlersVersion;
        private static long handlerLastSetValueTime;
        private static List<CommonVariableHandlerBase> sortedHandlers = new List<CommonVariableHandlerBase>();

        private SerializedProperty enableDebugMessageProperty;
        private SerializedProperty overrideHandleNameProperty;
        private SerializedProperty valueProperty;
        private SerializedProperty onChangeProperty;
        private SerializedProperty descriptionProperty;

        private void OnEnable()
        {
            enableDebugMessageProperty = serializedObject.FindProperty("enableDebugMessage");
            overrideHandleNameProperty = serializedObject.FindProperty("overrideHandleName");
            valueProperty = serializedObject.FindProperty("value");
            onChangeProperty = serializedObject.FindProperty("onChange");
            descriptionProperty = serializedObject.FindProperty("description");

            if (EditorApplication.isPlaying)
            {
                EditorApplication.update += EditorUpdate;
            }
        }

        private void OnDisable()
        {
            if (EditorApplication.isPlaying)
            {
                EditorApplication.update -= EditorUpdate;
            }
        }

        private void EditorUpdate()
        {
            if (Time.unscaledTime >= nextRepaintTime) { DoRepaint(); }
        }

        private void DoRepaint()
        {
            Repaint();
            nextRepaintTime = Time.unscaledTime + REPAINT_INTERVAL;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var targetObj = target as CommonVariableAssetBase;
            if (targetObj == null) { return; }

            EditorGUILayout.PropertyField(descriptionProperty, GUILayout.ExpandHeight(true));

            if (EditorApplication.isPlaying)
            {
                nextRepaintTime = Time.unscaledTime + REPAINT_INTERVAL;

                CommonHandler.EnableAllDebugMessage = EditorGUILayout.Toggle("Enable All Debug Message", CommonHandler.EnableAllDebugMessage);
                CommonHandler.EnableAllVariableDebugMessage = EditorGUILayout.Toggle("Enable All Variable Debug Message", CommonHandler.EnableAllVariableDebugMessage);
                CommonHandler.VariableDebugMessageOnlyOnChange = EditorGUILayout.Toggle("Print Set Value Message Only On Change", CommonHandler.VariableDebugMessageOnlyOnChange);

                EditorGUILayout.Space();

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(enableDebugMessageProperty);
                if (EditorGUI.EndChangeCheck()) { targetObj.EnableDebugMessage = enableDebugMessageProperty.boolValue; }

                EditorGUILayout.LabelField("Variable Name", targetObj.HandlerName);
                EditorGUILayout.PropertyField(valueProperty);

                if (GUILayout.Button("Set to Value"))
                {
                    EditorApplication.delayCall += () => { targetObj.SetAssetValue(); DoRepaint(); };
                }

                if (GUILayout.Button("Reset Count"))
                {
                    EditorApplication.delayCall += () => { targetObj.ResetSetValueCount(); DoRepaint(); };
                }

                EditorGUILayout.LabelField("Set Value Count", targetObj.SetValueCount.ToString());
                EditorGUILayout.LabelField("Previous Value", targetObj.VariableHandlerBase.PreviousValueString);
                EditorGUILayout.LabelField("Current Value", targetObj.VariableHandlerBase.CurrentValueString);

                EditorGUILayout.Space();

                if (onChangeProperty != null) { EditorGUILayout.PropertyField(onChangeProperty); }

                if (handlersVersion != CommonVariable.VariablesCount())
                {
                    handlersVersion = CommonVariable.VariablesCount();
                    sortedHandlers.Clear();
                    sortedHandlers.AddRange(CommonVariable.AllVariables());
                    handlerLastSetValueTime = 0L;
                }

                if (handlerLastSetValueTime != CommonVariable.LastSetValueTime())
                {
                    handlerLastSetValueTime = CommonVariable.LastSetValueTime();
                    sortedHandlers.Sort((x, y) =>
                    {
                        if (x.LastSetValueTime > y.LastSetValueTime) { return -1; }
                        else if (x.LastSetValueTime < y.LastSetValueTime) { return 1; }
                        if (x.SetValueCount > y.SetValueCount) { return -1; }
                        else if (x.SetValueCount < y.SetValueCount) { return 1; }
                        return string.Compare(x.Name, y.Name, true);
                    });
                }

                foreach (var h in sortedHandlers)
                {
                    EditorGUILayout.LabelField(h.Name + " (" + h.ValueType.Name + ")", " " + h.CurrentValueString);
                }
            }
            else
            {
                EditorGUILayout.PropertyField(enableDebugMessageProperty);
                overrideHandleNameProperty.stringValue = EditorGUILayout.TextField("Override Variable Name", overrideHandleNameProperty.stringValue);
                EditorGUILayout.PropertyField(valueProperty);
                if (onChangeProperty != null) { EditorGUILayout.PropertyField(onChangeProperty); }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
