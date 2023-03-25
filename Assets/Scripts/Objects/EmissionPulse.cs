using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class EmissionPulse : MonoBehaviour
{
    [Header("Emission Settings")]
    [Range(0, 5)] public float minEmissionIntensity = 0f;
    [Range(0, 5)] public float maxEmissionIntensity = 2f;
    public float pulseSpeed = 1f;
    public Color emissionColor = Color.white;

    private Renderer objectRenderer;
    private Material objectMaterial;
    private float time;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        objectMaterial = objectRenderer.material;

        // Set emission color
        objectMaterial.SetColor("_EmissionColor", emissionColor * minEmissionIntensity);
    }

    void Update()
    {
        time += Time.deltaTime * pulseSpeed;
        float intensity = Mathf.Lerp(minEmissionIntensity, maxEmissionIntensity, (Mathf.Sin(time) + 1) / 2); // Sin wave oscillates between -1 and 1, so normalize it to 0 - 1
        objectMaterial.SetColor("_EmissionColor", emissionColor * intensity);
    }
}