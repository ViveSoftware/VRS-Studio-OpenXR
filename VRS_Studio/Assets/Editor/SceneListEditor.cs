using UnityEditor;

[CustomEditor(typeof(SceneList))]
public class SceneListEditor : Editor
{
    private SceneList sceneList;

    private void OnEnable()
    {
        sceneList = (SceneList)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        sceneList.UpdateSceneNames();

        bool Changed = EditorGUI.EndChangeCheck();
        if (Changed)
        {
            UnityEditor.EditorUtility.SetDirty(Selection.activeObject);
        }
        serializedObject.ApplyModifiedProperties();
    }
}
