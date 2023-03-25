using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralOptimizer : MonoBehaviour
{
    public GameObject player;
    public float checkInterval = 0.5f;
    public float rendererDisableDistance = 100f;
    public float colliderDisableDistance = 50f;
    public float shadowCastingDisableDistance = 100f;
    public LayerMask objectLayer;

    private List<MeshRenderer> meshRenderers;
    private List<Collider> colliders;
    private List<Light> lights;
    private Camera mainCamera;

    void Start()
    {
        meshRenderers = new List<MeshRenderer>(FindObjectsOfType<MeshRenderer>());
        colliders = new List<Collider>(FindObjectsOfType<Collider>());
        lights = new List<Light>(FindObjectsOfType<Light>());
        mainCamera = Camera.main;
        StartCoroutine(Optimize());
    }

    private IEnumerator Optimize()
    {
        while (true)
        {
            OptimizeMeshRenderers();
            OptimizeColliders();
            OptimizeLights();
            yield return new WaitForSeconds(checkInterval);
        }
    }

    private void OptimizeMeshRenderers()
    {
        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            if (meshRenderer != null && (meshRenderer.gameObject.layer & objectLayer) != 0)
            {
                float distance = Vector3.Distance(player.transform.position, meshRenderer.transform.position);
                meshRenderer.enabled = distance <= rendererDisableDistance;
            }
        }
    }

    private void OptimizeColliders()
    {
        foreach (Collider collider in colliders)
        {
            if (collider != null && (collider.gameObject.layer & objectLayer) != 0)
            {
                float distance = Vector3.Distance(player.transform.position, collider.transform.position);
                collider.enabled = distance <= colliderDisableDistance;
            }
        }
    }

    private void OptimizeLights()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(mainCamera);

        foreach (Light light in lights)
        {
            if (light != null)
            {
                float distance = Vector3.Distance(player.transform.position, light.transform.position);
                Bounds lightBounds = new Bounds(light.transform.position, Vector3.one * light.range * 2);
                bool inView = GeometryUtility.TestPlanesAABB(planes, lightBounds);

                if (distance > shadowCastingDisableDistance || !inView)
                {
                    light.shadows = LightShadows.None;
                }
                else
                {
                    light.shadows = LightShadows.Hard;
                }
            }
        }
    }
}
