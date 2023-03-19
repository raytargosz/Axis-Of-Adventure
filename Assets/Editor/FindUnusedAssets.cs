using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FindUnusedAssets : EditorWindow
{
    private List<string> unusedAssets = new List<string>();
    private Vector2 scrollPosition;

    [MenuItem("Window/Find Unused Assets")]
    public static void ShowWindow()
    {
        GetWindow<FindUnusedAssets>("Find Unused Assets");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Find Unused Assets"))
        {
            FindAssets();
        }

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        foreach (string assetPath in unusedAssets)
        {
            GUILayout.Label(assetPath);
        }

        GUILayout.EndScrollView();
    }

    private void FindAssets()
    {
        unusedAssets.Clear();
        string[] allAssets = AssetDatabase.GetAllAssetPaths();
        HashSet<string> usedAssets = new HashSet<string>();

        foreach (string asset in allAssets)
        {
            string[] dependencies = AssetDatabase.GetDependencies(asset, false);
            foreach (string dependency in dependencies)
            {
                usedAssets.Add(dependency);
            }
        }

        foreach (string asset in allAssets)
        {
            if (!usedAssets.Contains(asset) && !asset.StartsWith("Assets/Editor"))
            {
                unusedAssets.Add(asset);
            }
        }
    }
}
