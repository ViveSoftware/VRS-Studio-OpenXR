using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Wave.Project
{
    public class GrepCSharpInPath : EditorWindow
    {
        public static void FindCSharpFiles(string path, ref List<string> fileList, bool recursively = true)
        {
            if (!Directory.Exists(path))
                return;

            fileList.AddRange(Directory.GetFiles(path, "*.cs"));

            var dirs = Directory.GetDirectories(path);
            foreach (var dir in dirs)
            {
                FindCSharpFiles(dir, ref fileList);
            }
        }

        public static void SearchInTextFile(ref StringBuilder sb, string path, string pattern, RegexOptions opt = RegexOptions.IgnoreCase)
        {
            //path = Path.GetFullPath(path);
            string[] lines = File.ReadAllLines(path);

            for (int i = 0; i < lines.Length; i++)
            {
                MatchCollection mc = Regex.Matches(lines[i], pattern, opt);
                if (mc.Count != 0)
                    sb.Append(string.Format("{0}:{1}: {2}", path, i, lines[i])).AppendLine();
            }
        }

        private string searchPath;
        private string pattern = "";
        private string patternKey = "GrepCSharpInPathPatternKey";
        private bool ignoreCase = true;

        public GrepCSharpInPath(string path)
        {
            searchPath = path;
            pattern = EditorPrefs.GetString(patternKey, "");
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField("Search in path:");
            searchPath = EditorGUILayout.TextField(searchPath);

            ignoreCase = EditorGUILayout.Toggle("Ignore Case", ignoreCase);
            pattern = EditorGUILayout.TextField("Regex", pattern);
            if (GUILayout.Button("Search"))
            {
                if (string.IsNullOrEmpty(pattern) || pattern == ".*")
                {
                    Debug.LogError("pattern \"" + pattern + "\"is not acceptable.");
                    return;
                }

                EditorPrefs.SetString(patternKey, pattern);

                List<string> files = new List<string>();
                FindCSharpFiles(searchPath, ref files);
                if (files.Count == 0)
                    return;

                StringBuilder sb = new StringBuilder("Search for pattern: ").Append(pattern).AppendLine();
                foreach (var file in files)
                {
                    SearchInTextFile(ref sb, file, pattern, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
                }
                Debug.Log(sb.ToString());
                Close();
            }
            EditorGUILayout.LabelField("Check result in log");
        }

        [MenuItem("Assets/Wave/Grep c# in folder")]
        static void DoGrepCSharpInFolder()
        {
            var obj = Selection.activeObject;
            if (obj == null)
                return;
            var path = AssetDatabase.GetAssetPath(obj);

            var window = new GrepCSharpInPath(path);
            window.ShowUtility();
        }
    }
}
