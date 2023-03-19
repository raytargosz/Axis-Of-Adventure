using UnityEditor;
using UnityEngine;

public class MeshCombiner : EditorWindow
{
    [MenuItem("Tools/Mesh Combiner")]
    public static void ShowWindow()
    {
        GetWindow<MeshCombiner>("Mesh Combiner");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Combine Selected Meshes"))
        {
            CombineMeshes();
        }
    }

    private void CombineMeshes()
    {
        GameObject[] selectedObjects = Selection.gameObjects;

        if (selectedObjects.Length == 0)
        {
            Debug.LogError("No objects selected. Please select GameObjects with MeshFilter components.");
            return;
        }

        GameObject combinedObject = new GameObject("CombinedMesh");
        MeshFilter combinedMeshFilter = combinedObject.AddComponent<MeshFilter>();
        MeshRenderer combinedMeshRenderer = combinedObject.AddComponent<MeshRenderer>();

        CombineInstance[] combineInstances = new CombineInstance[selectedObjects.Length];
        Material[] materials = new Material[selectedObjects.Length];

        for (int i = 0; i < selectedObjects.Length; i++)
        {
            GameObject obj = selectedObjects[i];
            MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
            MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();

            if (meshFilter == null || meshRenderer == null)
            {
                Debug.LogError("Selected GameObject '" + obj.name + "' does not have a MeshFilter or MeshRenderer component.");
                return;
            }

            combineInstances[i].mesh = meshFilter.sharedMesh;
            combineInstances[i].transform = meshFilter.transform.localToWorldMatrix;
            materials[i] = meshRenderer.sharedMaterial;
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        combinedMesh.CombineMeshes(combineInstances, false, false);
        combinedMeshFilter.mesh = combinedMesh;
        combinedMeshRenderer.materials = materials;
    }
}
