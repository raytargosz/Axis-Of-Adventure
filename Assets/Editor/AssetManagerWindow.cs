using UnityEditor;
using UnityEngine;

public class AssetManagerWindow : EditorWindow
{
    [MenuItem("Window/Asset Manager")]
    public static void ShowWindow()
    {
        GetWindow<AssetManagerWindow>("Asset Manager");
    }

    private void OnGUI()
    {
        GUILayout.Label("Asset Management", EditorStyles.boldLabel);

        if (GUILayout.Button("Find Unused Assets"))
        {
            FindUnusedAssets();
        }
    }

    private void FindUnusedAssets()
    {
        // Implement your logic to find unused assets here
    }
}
