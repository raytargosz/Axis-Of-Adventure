using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class OutlineWhenOccluded : MonoBehaviour
{
    [SerializeField] private Material outlineMaterial;
    private Material[] originalMaterials;
    private Material[] combinedMaterials;
    private Renderer objectRenderer;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        objectRenderer = GetComponent<Renderer>();
        originalMaterials = objectRenderer.materials;

        // Combine the original materials with the outline material
        combinedMaterials = new Material[originalMaterials.Length + 1];
        originalMaterials.CopyTo(combinedMaterials, 0);
        combinedMaterials[combinedMaterials.Length - 1] = outlineMaterial;
    }

    private void Update()
    {
        RaycastHit hit;
        Vector3 direction = transform.position - mainCamera.transform.position;

        if (Physics.Raycast(mainCamera.transform.position, direction, out hit))
        {
            if (hit.transform.gameObject != gameObject)
            {
                EnableOutline();
            }
            else
            {
                DisableOutline();
            }
        }
    }

    private void EnableOutline()
    {
        objectRenderer.materials = combinedMaterials;
    }

    private void DisableOutline()
    {
        objectRenderer.materials = originalMaterials;
    }
}