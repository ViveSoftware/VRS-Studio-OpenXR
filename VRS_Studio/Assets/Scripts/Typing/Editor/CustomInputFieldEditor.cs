﻿using HTC.Triton.LensUI;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine.UI;

namespace HTC.ViveSoftware.ExpLab.HandInteractionDemo
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CustomInputField), true)]
    /// <summary>
    /// Custom Editor for the InputField Component.
    /// Extend this class to write a custom editor for a component derived from InputField.
    /// </summary>
    public class CustomInputFieldEditor : LensUISelectableEditor
    {
        SerializedProperty m_TextComponent;
        SerializedProperty m_Text;
        SerializedProperty m_ContentType;
        SerializedProperty m_LineType;
        SerializedProperty m_InputType;
        SerializedProperty m_CharacterValidation;
        SerializedProperty m_KeyboardType;
        SerializedProperty m_CharacterLimit;
        SerializedProperty m_CaretBlinkRate;
        SerializedProperty m_CaretWidth;
        SerializedProperty m_CaretColor;
        SerializedProperty m_CustomCaretColor;
        SerializedProperty m_SelectionColor;
        SerializedProperty m_HideMobileInput;
        SerializedProperty m_Placeholder;
        SerializedProperty m_OnValueChanged;
        SerializedProperty m_OnEndEdit;
        SerializedProperty m_ReadOnly;
        SerializedProperty m_ShouldActivateOnSelect;

        AnimBool m_CustomColor;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_TextComponent = serializedObject.FindProperty("m_TextComponent");
            m_Text = serializedObject.FindProperty("m_Text");
            m_ContentType = serializedObject.FindProperty("m_ContentType");
            m_LineType = serializedObject.FindProperty("m_LineType");
            m_InputType = serializedObject.FindProperty("m_InputType");
            m_CharacterValidation = serializedObject.FindProperty("m_CharacterValidation");
            m_KeyboardType = serializedObject.FindProperty("m_KeyboardType");
            m_CharacterLimit = serializedObject.FindProperty("m_CharacterLimit");
            m_CaretBlinkRate = serializedObject.FindProperty("m_CaretBlinkRate");
            m_CaretWidth = serializedObject.FindProperty("m_CaretWidth");
            m_CaretColor = serializedObject.FindProperty("m_CaretColor");
            m_CustomCaretColor = serializedObject.FindProperty("m_CustomCaretColor");
            m_SelectionColor = serializedObject.FindProperty("m_SelectionColor");
            m_HideMobileInput = serializedObject.FindProperty("m_HideMobileInput");
            m_Placeholder = serializedObject.FindProperty("m_Placeholder");
            m_OnValueChanged = serializedObject.FindProperty("m_OnValueChanged");
            m_OnEndEdit = serializedObject.FindProperty("m_OnEndEdit");
            m_ReadOnly = serializedObject.FindProperty("m_ReadOnly");
            m_ShouldActivateOnSelect = serializedObject.FindProperty("m_ShouldActivateOnSelect");

            m_CustomColor = new AnimBool(m_CustomCaretColor.boolValue);
            m_CustomColor.valueChanged.AddListener(Repaint);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            m_CustomColor.valueChanged.RemoveListener(Repaint);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            base.OnInspectorGUI();

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_TextComponent);

            if (m_TextComponent != null && m_TextComponent.objectReferenceValue != null)
            {
                Text text = m_TextComponent.objectReferenceValue as Text;
                if (text != null && text.supportRichText)
                {
                    EditorGUILayout.HelpBox("Using Rich Text with input is unsupported.", MessageType.Warning);
                }
            }

            using (new EditorGUI.DisabledScope(m_TextComponent == null || m_TextComponent.objectReferenceValue == null))
            {
                EditorGUILayout.PropertyField(m_Text);
                EditorGUILayout.PropertyField(m_CharacterLimit);

                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(m_ContentType);
                if (!m_ContentType.hasMultipleDifferentValues)
                {
                    EditorGUI.indentLevel++;

                    if (m_ContentType.enumValueIndex == (int)InputField.ContentType.Standard ||
                        m_ContentType.enumValueIndex == (int)InputField.ContentType.Autocorrected ||
                        m_ContentType.enumValueIndex == (int)InputField.ContentType.Custom)
                        EditorGUILayout.PropertyField(m_LineType);

                    if (m_ContentType.enumValueIndex == (int)InputField.ContentType.Custom)
                    {
                        EditorGUILayout.PropertyField(m_InputType);
                        EditorGUILayout.PropertyField(m_KeyboardType);
                        EditorGUILayout.PropertyField(m_CharacterValidation);
                    }

                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(m_Placeholder);
                EditorGUILayout.PropertyField(m_CaretBlinkRate);
                EditorGUILayout.PropertyField(m_CaretWidth);

                EditorGUILayout.PropertyField(m_CustomCaretColor);

                m_CustomColor.target = m_CustomCaretColor.boolValue;

                if (EditorGUILayout.BeginFadeGroup(m_CustomColor.faded))
                {
                    EditorGUILayout.PropertyField(m_CaretColor);
                }
                EditorGUILayout.EndFadeGroup();

                EditorGUILayout.PropertyField(m_SelectionColor);
                EditorGUILayout.PropertyField(m_HideMobileInput);
                EditorGUILayout.PropertyField(m_ReadOnly);
                EditorGUILayout.PropertyField(m_ShouldActivateOnSelect);

                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(m_OnValueChanged);
                EditorGUILayout.PropertyField(m_OnEndEdit);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}