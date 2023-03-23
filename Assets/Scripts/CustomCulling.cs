using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class CustomCulling : MonoBehaviour
{
    public Camera mainCamera;
    public float cullingInterval = 0.2f;
    public LayerMask cullingLayers;
    public List<Renderer> excludedRenderers;

    private Renderer[] renderers;

    void Start()
    {
        renderers = FindObjectsOfType<Renderer>();
        StartCoroutine(CullingCoroutine());
    }

    IEnumerator CullingCoroutine()
    {
        while (true)
        {
            foreach (Renderer renderer in renderers)
            {
                if (!excludedRenderers.Contains(renderer))
                {
                    Plane[] planes = GeometryUtility.CalculateFrustumPlanes(mainCamera);
                    renderer.enabled = GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
                }
            }

            yield return new WaitForSeconds(cullingInterval);
        }
    }
}
