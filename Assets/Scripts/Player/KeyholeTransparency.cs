using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class KeyholeTransparency : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private float sphereRadius = 0.2f;
    [SerializeField] private float distanceBuffer = 0.5f;
    [SerializeField] private float fadeSpeed = 2f;

    private readonly Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>();

    private void Update()
    {
        RaycastHit[] hits;
        Vector3 direction = player.position - transform.position;
        float distance = Vector3.Distance(player.position, transform.position);

        hits = Physics.SphereCastAll(transform.position, sphereRadius, direction, distance - distanceBuffer, obstacleMask);

        // Reset the previous frame's transparent objects
        foreach (var entry in originalMaterials)
        {
            entry.Key.materials = entry.Value;

            if (!entry.Key.enabled)
            {
                FadeIn(entry.Key);
            }
        }
        originalMaterials.Clear();

        foreach (RaycastHit hit in hits)
        {
            Renderer renderer = hit.transform.GetComponent<Renderer>();

            if (renderer != null)
            {
                // Store original materials
                if (!originalMaterials.ContainsKey(renderer))
                {
                    originalMaterials[renderer] = renderer.materials;
                }

                // Fade out and disable the renderer
                if (renderer.enabled)
                {
                    StartCoroutine(FadeOut(renderer));
                }
            }
        }
    }

    private IEnumerator FadeOut(Renderer renderer)
    {
        float alpha = 1f;
        while (alpha > 0f)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            SetAlpha(renderer, alpha);
            yield return null;
        }
        renderer.enabled = false;
    }

    private void FadeIn(Renderer renderer)
    {
        StartCoroutine(FadeInCoroutine(renderer));
    }

    private IEnumerator FadeInCoroutine(Renderer renderer)
    {
        renderer.enabled = true;
        float alpha = 0f;
        while (alpha < 1f)
        {
            alpha += Time.deltaTime * fadeSpeed;
            SetAlpha(renderer, alpha);
            yield return null;
        }
    }

    private void SetAlpha(Renderer renderer, float alpha)
    {
        Material[] materials = renderer.materials;
        for (int i = 0; i < materials.Length; i++)
        {
            Color color = materials[i].color;
            color.a = alpha;
            materials[i].color = color;
        }
        renderer.materials = materials;
    }
}