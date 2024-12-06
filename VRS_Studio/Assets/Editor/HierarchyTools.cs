using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

/*
 * The Hierarchy Tools window help do more complex job of searching and selection
 * in Unity Editor.  It enhance the lack search and filter feature of Unity 
 * editor.
 * 
 * There are "Search and Select" feature, "Selection keep and restore" feature,
 * and "Copy and Replace" feature here.
 * 
 * 
 * Reference
 *     http://www.clonefactor.com/wordpress/public/1769/
 */
public class HierarchyTools : EditorWindow
{
    // Unity finally add deselect
    //[MenuItem("Edit/Deselect All &d", false, -101)]
    static void DeselectAll()
    {
        Selection.activeGameObject = null;
    }

    enum CompareMethod
    {
        PartialName,
        FullName,
        Pattern,
    }

    enum CompareTarget
    {
        GameObjectName,
        Component,
        Tag,
        Layer,
    }

    private string[] methodStrings =
    {
        "Partial Name",
        "Full Name",
        "Regex Pattern"
    };

    private string[] targetStrings =
    {
        "GameObjectName",
        "Component Name",
        "Tag",
        "Layer"
    };

    class SearchContext
    {
        public CompareMethod method;
        public CompareTarget target;
        public string pattern;
        public RegexOptions options;
        public Regex regex;
        public bool invertSelection;

        // output
        public List<GameObject> list;

        public SearchContext(CompareMethod m, CompareTarget t, string p, RegexOptions o = RegexOptions.None, bool inv = false)
        {
            list = new List<GameObject>();

            method = m;
            target = t;
            pattern = p;
            options = o;
            invertSelection = inv;
            regex = new Regex(pattern, options);
        }
    }

    bool Compare(string input, SearchContext context)
    {
        switch (context.method)
        {
            case CompareMethod.PartialName:
            case CompareMethod.Pattern:
                return context.regex.IsMatch(input);
            case CompareMethod.FullName:
                return context.pattern == input;
        }
        return false;
    }

    void AddToList(GameObject obj, bool hit, SearchContext context)
    {
        if (hit != context.invertSelection)
        {
            if (!context.list.Contains(obj))
                context.list.Add(obj);
        }
    }

    void CompareAndAddToList(GameObject obj, string target, SearchContext context)
    {
        AddToList(obj, Compare(target, context), context);
    }

    void FindSelf(GameObject obj, SearchContext context)
    {
        string target = "";
        switch (context.target)
        {
            case CompareTarget.GameObjectName:
                target = obj.name;
                break;
            case CompareTarget.Component:
                {
                    Component[] components = obj.GetComponents(typeof(Component));
                    bool hit = false;
                    // One hit is hit
                    foreach (Component c in components)
                    {
                        target = c.GetType().ToString();
                        if (Compare(target, context))
                        {
                            hit = true;
                            break;
                        }
                    }
                    AddToList(obj, hit, context);
                    return;
                }
            case CompareTarget.Layer:
                target = LayerMask.LayerToName(obj.layer);
                break;
            case CompareTarget.Tag:
                target = obj.tag;
                break;
        }

        CompareAndAddToList(obj, target, context);
    }

    void FindChildrenRecursivly(GameObject parent, SearchContext context)
    {
        foreach (Transform child in parent.transform)
        {
            FindChildrenRecursivly(child.gameObject, context);
            FindSelf(child.gameObject, context);
        }
    }


    void SelectChildren()
    {
        if (Selection.gameObjects == null)
        {
            Debug.Log("Need select one or more gameobjects before selection");
            return;
        }

        RegexOptions options = caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;

        SearchContext context = new SearchContext(method, target, pattern, options, invertSelection);
        foreach (GameObject parent in Selection.gameObjects)
        {
            if (includingParentSelf)
                FindSelf(parent, context);
            FindChildrenRecursivly(parent, context);
        }

        if (context.list.Count > 0)
            Selection.objects = context.list.ToArray();
        else
            Selection.activeGameObject = null;
    }

    void SelectParents()
    {
        if (Selection.gameObjects == null)
        {
            Debug.Log("Need select one or more gameobjects before selection");
            return;
        }
        List<GameObject> parents = new List<GameObject>();
        foreach (GameObject child in Selection.gameObjects)
        {
            if (child.transform.parent == null)
                continue;
            GameObject parent = child.transform.parent.gameObject;
            if (parents.Contains(parent))
                continue;
            parents.Add(parent);
        }
        if (parents.Count > 0)
            Selection.objects = parents.ToArray();
        else
            Selection.activeGameObject = null;
    }

    [MenuItem("Window/Hierarchy Tools")]
    static void Init()
    {
        //HierarchyTools window = (HierarchyTools)
        EditorWindow.GetWindow(typeof(HierarchyTools), false, "Hierarchy Tools");
    }

    private Vector2 scroll;
    private bool selectionFoldout = false;
    private bool storageFoldout = false;
    private bool copyPasteFoldout = false;

    private string pattern = "";
    private bool caseSensitive = false;
    private bool invertSelection = false;
    private bool includingParentSelf = false;
    private CompareMethod method = CompareMethod.PartialName;
    private CompareTarget target = CompareTarget.GameObjectName;

    private static List<string> history = new List<string>();

    private static GameObject[][] storedSelections = new GameObject[9][];

    private GameObject clipboard;

    void updateHistory(string pattern)
    {
        // Remove old one from the list
        for (int i = 0; i < history.Count; i++)
        {
            string p = history[i];
            if (p == pattern)
                history.RemoveAt(i);
        }

        // Put in the front
        history.Insert(0, pattern);
    }

    void loadSaveSlot(int slot, bool loadOrSave)
    {
        slot = slot - 1;
        if (slot >= 0 && slot < 9)
        {
            if (loadOrSave)
            {
                if (storedSelections[slot] != null)
                {
                    // Check if game object is been destoryed
                    //foreach (GameObject o in storedSelections[slot])
                    //{
                    //    if (ReferenceEquals(o, null))
                    //        continue;
                    //}
                    Selection.objects = storedSelections[slot];
                    //Debug.Log("Loaded from slot " + slot);
                }
            }
            else
            {
                storedSelections[slot] = Selection.gameObjects;
                //Debug.Log("Saved to slot " + slot);
            }
        }
    }

    void CopySelected()
    {
        if (Selection.gameObjects == null || Selection.gameObjects.Length > 1)
        {
            Debug.Log("You can copy only one object to clipboad.");
            return;
        }
        clipboard = Selection.activeGameObject;
    }

    void PasteAsChild()
    {
        if (Selection.gameObjects == null)
            return;

        if (clipboard == null)
            return;

        foreach (GameObject parent in Selection.gameObjects)
        {
            Instantiate(clipboard, parent.transform);
        }
    }

    void Replace()
    {
        if (Selection.gameObjects == null)
            return;

        if (clipboard == null)
            return;

        foreach (GameObject obj in Selection.gameObjects)
        {
            Transform parent = obj.transform.parent;
            GameObject copy = Instantiate(clipboard, parent);
            copy.transform.localRotation = obj.transform.localRotation;
            copy.transform.localPosition = obj.transform.localPosition;
            copy.transform.localScale = obj.transform.localScale;
        }
    }

    void OnGUI()
    {
        scroll = EditorGUILayout.BeginScrollView(scroll);
        selectionFoldout = EditorGUILayout.Foldout(selectionFoldout, "Selection", true);
        if (selectionFoldout)
        {
            EditorGUILayout.HelpBox("Select one or more GameObjects from Hierarchy. Find their children by filter and selected them.", MessageType.Info);

            method = (CompareMethod)EditorGUILayout.Popup((int)method, methodStrings);
            target = (CompareTarget)EditorGUILayout.Popup((int)target, targetStrings);

            caseSensitive = EditorGUILayout.Toggle("Case Sensitivie", caseSensitive);
            invertSelection = EditorGUILayout.Toggle("Invert Selection", invertSelection);
            includingParentSelf = EditorGUILayout.Toggle("Including Parent Self", includingParentSelf);

            pattern = EditorGUILayout.TextField("Pattern", pattern);
            if (GUILayout.Button("Select Children by filter"))
                SelectChildren();

            if (GUILayout.Button("Select Parents"))
                SelectParents();
        }

        storageFoldout = EditorGUILayout.Foldout(storageFoldout, "Storage", true);
        if (storageFoldout)
        {
            bool loadOrSave = Selection.activeGameObject == null;
            if (loadOrSave)
                GUILayout.Label("LoadGroup", EditorStyles.boldLabel);
            else
                GUILayout.Label("SaveGroup", EditorStyles.boldLabel);

            GUILayout.Space(3);
            if (GUILayout.Button("Deselect All"))
                DeselectAll();
            GUILayout.Space(3);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("1")) loadSaveSlot(1, loadOrSave);
            if (GUILayout.Button("2")) loadSaveSlot(2, loadOrSave);
            if (GUILayout.Button("3")) loadSaveSlot(3, loadOrSave);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("4")) loadSaveSlot(4, loadOrSave);
            if (GUILayout.Button("5")) loadSaveSlot(5, loadOrSave);
            if (GUILayout.Button("6")) loadSaveSlot(6, loadOrSave);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("7")) loadSaveSlot(7, loadOrSave);
            if (GUILayout.Button("8")) loadSaveSlot(8, loadOrSave);
            if (GUILayout.Button("9")) loadSaveSlot(9, loadOrSave);
            EditorGUILayout.EndHorizontal();
        }

        copyPasteFoldout = EditorGUILayout.Foldout(copyPasteFoldout, "CopyPaste", true);
        if (copyPasteFoldout)
        {
            GUILayout.Label("Clipboard", EditorStyles.boldLabel);
            clipboard = (GameObject)EditorGUILayout.ObjectField(clipboard, typeof(GameObject), true);
            if (GUILayout.Button("Copy to Clipboard"))
                CopySelected();
            if (GUILayout.Button("Paste as child"))
                PasteAsChild();
            if (GUILayout.Button("Keep Transform and replace *"))
                Replace();
            EditorGUILayout.HelpBox("Copy selected to clipboard.  It will be instantiated " +
                "as a new one when pasting.  The Replace button will put a new one under same " +
                "parent and copy the selected's transform, and then you need delete the " +
                "old one by yourself.", MessageType.Info);
        }

        //groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
        //myBool = EditorGUILayout.Toggle("Toggle", myBool);
        //myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);
        //EditorGUILayout.EndToggleGroup();
        EditorGUILayout.EndScrollView();
    }

    //[MenuItem("GameObject/HierarchyTools/SelectChildrenByName", false, 0)]
}
