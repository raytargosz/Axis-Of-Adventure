using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class OutlineController : MonoBehaviour
{
    public Material outlineMaterial;
    private Renderer objectRenderer;
    private Material[] originalMaterials;
    private Material[] outlineMaterials;

    private void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        originalMaterials = objectRenderer.sharedMaterials;

        outlineMaterials = new Material[originalMaterials.Length + 1];
        originalMaterials.CopyTo(outlineMaterials, 0);
        outlineMaterials[outlineMaterials.Length - 1] = outlineMaterial;
    }

    public void EnableOutline()
    {
        objectRenderer.sharedMaterials = outlineMaterials;
    }

    public void DisableOutline()
    {
        objectRenderer.sharedMaterials = originalMaterials;
    }
}
