using UnityEditor;
using UnityEditor.UI;

namespace HTC.Triton.LensUI
{
    [CustomEditor(typeof(LensUISelectable), true)]
    [CanEditMultipleObjects]
    public class LensUISelectableEditor : SelectableEditor
    {
        SerializedProperty effectiveButtonProperty;
        SerializedProperty selectOnPointerDownProperty;
        SerializedProperty deselectOnPointerEnterProperty;

        protected override void OnEnable()
        {
            base.OnEnable();
            effectiveButtonProperty = serializedObject.FindProperty("effectiveButton");
            selectOnPointerDownProperty = serializedObject.FindProperty("selectOnPointerDown");
            deselectOnPointerEnterProperty = serializedObject.FindProperty("deselectOnPointerEnter");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            serializedObject.Update();
            EditorGUILayout.PropertyField(effectiveButtonProperty);
            EditorGUILayout.PropertyField(selectOnPointerDownProperty);
            EditorGUILayout.PropertyField(deselectOnPointerEnterProperty);
            serializedObject.ApplyModifiedProperties();
        }
    }
}