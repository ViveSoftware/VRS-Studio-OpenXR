// "WaveVR SDK 
// © 2017 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the WaveVR SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using UnityEditor;

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class MeshSaverEditor
{
	[MenuItem("CONTEXT/MeshFilter/Save Mesh...")]
	public static void SaveMeshInPlace(MenuCommand menuCommand)
	{
		MeshFilter mf = menuCommand.context as MeshFilter;
		if (mf != null)
		{
            Mesh m = mf.sharedMesh;
            SaveMesh(m, m.name, false);
        }
    }

	[MenuItem("CONTEXT/MeshFilter/Save Mesh As New Instance...")]
	public static void SaveMeshNewInstanceItem(MenuCommand menuCommand)
	{
		MeshFilter mf = menuCommand.context as MeshFilter;
		if (mf != null)
		{
            Mesh m = mf.sharedMesh;
            SaveMesh(m, m.name, true);
        }
    }

    public static void SaveMesh(Mesh mesh, string name, bool makeNewInstance)
	{
		string path = EditorUtility.SaveFilePanel("Save Separate Mesh Asset", "Assets/", name, "asset");
		if (string.IsNullOrEmpty(path)) return;

		path = FileUtil.GetProjectRelativePath(path);

		Mesh meshToSave = (makeNewInstance) ? Object.Instantiate(mesh) as Mesh : mesh;

		AssetDatabase.CreateAsset(meshToSave, path);
		AssetDatabase.SaveAssets();
	}

}
