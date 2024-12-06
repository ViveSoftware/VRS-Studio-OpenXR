using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HTC.UnityPlugin.CommonEventVariable
{
    [CustomEditor(typeof(CommonEventAssetBase), true)]
    [CanEditMultipleObjects]
    public class CommonEventAssetEditor : Editor
    {
        private const float REPAINT_INTERVAL = 0.3f;
        private static float nextRepaintTime;
        private static List<int> handlersVersion = new List<int>();
        private static List<long> handlersLastTriggerTime = new List<long>();
        private static List<CommonEventHandlerBase> sortedHandlers = new List<CommonEventHandlerBase>();

        private SerializedProperty enableDebugMessageProperty;
        private SerializedProperty overrideHandleNameProperty;
        private SerializedProperty paramValue1Property;
        private SerializedProperty paramValue2Property;
        private SerializedProperty onTriggerProperty;
        private SerializedProperty descriptionProperty;

        private void OnEnable()
        {
            enableDebugMessageProperty = serializedObject.FindProperty("enableDebugMessage");
            overrideHandleNameProperty = serializedObject.FindProperty("overrideHandleName");
            paramValue1Property = serializedObject.FindProperty("paramValue1");
            paramValue2Property = serializedObject.FindProperty("paramValue2");
            onTriggerProperty = serializedObject.FindProperty("onTrigger");
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

            var targetObj = target as CommonEventAssetBase;
            if (targetObj == null) { return; }

            var paramCount = targetObj.ParamCount;
            EditorGUILayout.PropertyField(descriptionProperty, GUILayout.ExpandHeight(true));

            if (EditorApplication.isPlaying)
            {
                nextRepaintTime = Time.unscaledTime + REPAINT_INTERVAL;

                CommonHandler.EnableAllDebugMessage = EditorGUILayout.Toggle("Enable All Debug Message", CommonHandler.EnableAllDebugMessage);
                CommonHandler.EnableAllEventDebugMessage = EditorGUILayout.Toggle("Enable All Event Debug Message", CommonHandler.EnableAllEventDebugMessage);

                EditorGUILayout.Space();

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(enableDebugMessageProperty);
                if (EditorGUI.EndChangeCheck()) { targetObj.EnableDebugMessage = enableDebugMessageProperty.boolValue; }

                EditorGUILayout.LabelField("Event Name", targetObj.HandlerName);
                if (paramCount >= 1) { EditorGUILayout.PropertyField(paramValue1Property); }
                if (paramCount >= 2) { EditorGUILayout.PropertyField(paramValue2Property); }

                if (GUILayout.Button("Trigger"))
                {
                    EditorApplication.delayCall += () => { targetObj.Trigger(); DoRepaint(); };
                }

                if (GUILayout.Button("Reset Count"))
                {
                    EditorApplication.delayCall += () => { targetObj.ResetTriggerCount(); DoRepaint(); };
                }

                EditorGUILayout.LabelField("Trigger Count", targetObj.TriggerCount.ToString());

                EditorGUILayout.Space();

                if (onTriggerProperty != null) { EditorGUILayout.PropertyField(onTriggerProperty); }

                EditorGUILayout.Space();

                while (CommonEvent.MaxExistingEventsParamCount > handlersVersion.Count)
                {
                    handlersVersion.Add(0);
                    handlersLastTriggerTime.Add(0L);
                }

                var needFetch = false;
                var needSort = false;
                for (int pc = 0, pcMax = CommonEvent.MaxExistingEventsParamCount; pc < pcMax; ++pc)
                {
                    if (handlersVersion[pc] != CommonEvent.EventsCount(pc))
                    {
                        handlersVersion[pc] = CommonEvent.EventsCount(pc);
                        needFetch = true;
                    }

                    if (handlersLastTriggerTime[pc] != CommonEvent.LastTriggerTime(pc))
                    {
                        handlersLastTriggerTime[pc] = CommonEvent.LastTriggerTime(pc);
                        needSort = true;
                    }
                }

                if (needFetch)
                {
                    sortedHandlers.Clear();
                    for (int pc = 0, pcMax = CommonEvent.MaxExistingEventsParamCount; pc < pcMax; ++pc)
                    {
                        sortedHandlers.AddRange(CommonEvent.AllEvents(pc));
                    }
                }

                if (needSort)
                {
                    sortedHandlers.Sort((x, y) =>
                    {
                        if (x.LastTriggerTime > y.LastTriggerTime) { return -1; }
                        else if (x.LastTriggerTime < y.LastTriggerTime) { return 1; }
                        if (x.TriggerCount > y.TriggerCount) { return -1; }
                        else if (x.TriggerCount < y.TriggerCount) { return 1; }
                        if (x.ParamCount < y.ParamCount) { return -1; }
                        else if (x.ParamCount > y.ParamCount) { return 1; }
                        return string.Compare(x.Name, y.Name, true);
                    });
                }

                foreach (var h in sortedHandlers)
                {
                    var label = h.Name;
                    var pc = h.ParamCount;
                    if (pc > 0)
                    {
                        for (int i = 0, imax = pc; i < imax; ++i)
                        {
                            if (i == 0) { label += " ("; }
                            else { label += ","; }
                            label += h.ParamType(i).Name;
                            if (i == pc - 1) { label += ")"; }
                        }
                    }

                    EditorGUILayout.LabelField(label, " " + h.TriggerCount.ToString());
                }
            }
            else
            {
                EditorGUILayout.PropertyField(enableDebugMessageProperty);
                overrideHandleNameProperty.stringValue = EditorGUILayout.TextField("Override Event Name", overrideHandleNameProperty.stringValue);
                if (paramCount >= 1) { EditorGUILayout.PropertyField(paramValue1Property); }
                if (paramCount >= 2) { EditorGUILayout.PropertyField(paramValue2Property); }
                if (onTriggerProperty != null) { EditorGUILayout.PropertyField(onTriggerProperty); }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}