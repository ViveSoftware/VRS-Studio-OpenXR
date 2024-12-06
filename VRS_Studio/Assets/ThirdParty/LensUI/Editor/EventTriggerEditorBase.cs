using System;
using UnityEditor;
using UnityEngine;

namespace HTC.UnityPlugin.Utility
{
    public abstract class EventTriggerEditorBase<TEnum> : Editor where TEnum : Enum
    {
        private sealed class EventNameArray : EnumArray<TEnum, GUIContent> { }
        private sealed class ActiveInMenuArray : EnumArray<TEnum, bool> { }

        private EventNameArray eventTypeNames;
        private ActiveInMenuArray eventTypeActiveInMenu;
        private SerializedProperty entriesProperty;
        private GUIContent iconToolbarMinus;
        private GUIContent callbackLabel;
        private GUIContent addButonContent;

        protected abstract string EntriesProp { get; }
        protected abstract string EntryEventProp { get; }
        protected abstract string EntryCallbackProp { get; }

        protected virtual void OnEnable()
        {
            entriesProperty = serializedObject.FindProperty(EntriesProp);

            addButonContent = EditorGUIUtility.TrTextContent("Add New Event Type");
            callbackLabel = new GUIContent("");
            // Have to create a copy since otherwise the tooltip will be overwritten.
            iconToolbarMinus = new GUIContent(EditorGUIUtility.IconContent("Toolbar Minus"));
            iconToolbarMinus.tooltip = "Remove all events in this list.";

            eventTypeActiveInMenu = new ActiveInMenuArray();
            eventTypeNames = new EventNameArray();
            foreach (var e in EventNameArray.StaticEnums)
            {
                eventTypeNames[e] = new GUIContent(e.ToString());
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var toBeRemovedEntry = -1;
            eventTypeActiveInMenu.Clear(true);

            EditorGUILayout.Space();

            Vector2 removeButtonSize = GUIStyle.none.CalcSize(iconToolbarMinus);

            for (int i = 0, imax = entriesProperty.arraySize; i < imax; ++i)
            {
                var entryProperty = entriesProperty.GetArrayElementAtIndex(i);
                var eventTypeProperty = entryProperty.FindPropertyRelative(EntryEventProp);
                var callbacksProperty = entryProperty.FindPropertyRelative(EntryCallbackProp);
                var enumValue = eventTypeProperty.enumValueIndex;

                if (enumValue < ActiveInMenuArray.StaticMinInt || enumValue > ActiveInMenuArray.StaticMaxInt)
                {
                    Debug.LogWarning("Invalid event type found in entris has been removed. type:" + typeof(TEnum).Name + " enumValue:" + enumValue);
                    toBeRemovedEntry = i;
                    break;
                }

                callbackLabel.text = eventTypeProperty.enumDisplayNames[eventTypeProperty.enumValueIndex];

                EditorGUILayout.PropertyField(callbacksProperty, callbackLabel);
                var callbackRect = GUILayoutUtility.GetLastRect();

                var removeButtonPos = new Rect(callbackRect.xMax - removeButtonSize.x - 8, callbackRect.y + 1, removeButtonSize.x, removeButtonSize.y);
                if (GUI.Button(removeButtonPos, iconToolbarMinus, GUIStyle.none))
                {
                    toBeRemovedEntry = i;
                }
                else
                {
                    eventTypeActiveInMenu[enumValue] = false;
                }

                EditorGUILayout.Space();
            }

            if (toBeRemovedEntry > -1)
            {
                // remove entry
                entriesProperty.DeleteArrayElementAtIndex(toBeRemovedEntry);
            }

            if (entriesProperty.arraySize < EventNameArray.StaticLength)
            {
                Rect btPosition = GUILayoutUtility.GetRect(addButonContent, GUI.skin.button);
                const float addButonWidth = 200f;
                btPosition.x = btPosition.x + (btPosition.width - addButonWidth) / 2;
                btPosition.width = addButonWidth;
                if (GUI.Button(btPosition, addButonContent))
                {
                    ShowAddTriggerMenu();
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void ShowAddTriggerMenu()
        {
            // Now create the menu, add items and show it
            var menu = new GenericMenu();
            foreach (var e in ActiveInMenuArray.StaticEnums)
            {
                if (eventTypeActiveInMenu[e])
                {
                    menu.AddItem(eventTypeNames[e], false, OnAddNewSelected, e);
                }
                else
                {
                    menu.AddDisabledItem(eventTypeNames[e]);
                }
            }

            menu.ShowAsContext();
            Event.current.Use();
        }

        private void OnAddNewSelected(object index)
        {
            var selected = (int)index;

            entriesProperty.arraySize += 1;
            var entryProperty = entriesProperty.GetArrayElementAtIndex(entriesProperty.arraySize - 1);
            var eventTypeProperty = entryProperty.FindPropertyRelative(EntryEventProp);
            eventTypeProperty.enumValueIndex = selected;
            serializedObject.ApplyModifiedProperties();

            OnTriggerAdded();
        }

        protected virtual void OnTriggerAdded() { }
    }
}