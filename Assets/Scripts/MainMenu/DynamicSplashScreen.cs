using UnityEngine;
using System.Collections;

public class DynamicSplashScreen : MonoBehaviour
{
    [Header("Camera Settings")]
    [Tooltip("Camera Transform for moving")]
    public Transform cameraTransform;
    [Tooltip("Camera movement speed")]
    public float cameraSpeed = 10f;

    [Header("Water Settings")]
    [Tooltip("Transform of the water plane")]
    public Transform waterPlane;
    [Tooltip("Transform of the water surface")]
    public Transform waterSurface;
    [Tooltip("Rate at which the water expands in X and Z")]
    public Vector2 waterExpansionRate = new Vector2(1, 1);
    [Tooltip("Distance before repeating the water expansion")]
    public float waterRepeatDistance = 50f;

    [Header("Mountain Settings")]
    [Tooltip("Array of mountain prefabs to spawn")]
    public GameObject[] mountainPrefabs;
    [Tooltip("Parent transform for spawned mountains")]
    public Transform mountainSpawnParent;
    [Tooltip("Interval between mountain spawns")]
    public float spawnInterval = 5f;
    [Tooltip("Distance from the camera for mountains to spawn")]
    public float spawnDistance = 100f;
    [Tooltip("Minimum height of mountains")]
    public float minHeight = 0f;
    [Tooltip("Maximum height of mountains")]
    public float maxHeight = 50f;
    [Tooltip("Range of mountain scale sizes")]
    public Vector2 scaleRange = new Vector2(1, 3);

    private float lastSpawnTime = 0f;
    private Vector3 lastCameraPosition;

    // Initialize the last camera position on start
    void Start()
    {
        lastCameraPosition = cameraTransform.position;
    }

    // Update method for camera movement, water expansion, and mountain spawning
    void Update()
    {
        MoveCamera();
        ExpandWater();
        SpawnMountains();
    }

    // Move the camera forward based on its current direction
    void MoveCamera()
    {
        Vector3 cameraForward = cameraTransform.forward;
        cameraTransform.position += cameraForward * cameraSpeed * Time.deltaTime;
    }

    // Expand the water plane and surface based on the camera's distance
    void ExpandWater()
    {
        Vector3 currentCameraPosition = cameraTransform.position;
        Vector3 waterPosition = waterPlane.position;
        Vector3 waterSurfacePosition = waterSurface.position;

        if (Vector3.Distance(currentCameraPosition, waterPosition) > waterRepeatDistance)
        {
            waterPlane.localScale = new Vector3(waterPlane.localScale.x + waterExpansionRate.x, waterPlane.localScale.y, waterPlane.localScale.z + waterExpansionRate.y);
            waterSurface.localScale = new Vector3(waterSurface.localScale.x + waterExpansionRate.x, waterSurface.localScale.y, waterSurface.localScale.z + waterExpansionRate.y);
        }
    }

    void SpawnMountains()
    {
        if (Time.time - lastSpawnTime > spawnInterval)
        {
            float spawnAngle = Random.Range(0, 2 * Mathf.PI);
            Vector3 spawnDirection = new Vector3(Mathf.Cos(spawnAngle), 0, Mathf.Sin(spawnAngle));
            Vector3 spawnPosition = cameraTransform.position + spawnDirection * spawnDistance;
            spawnPosition.y = Random.Range(minHeight, maxHeight);

            GameObject mountainPrefab = mountainPrefabs[Random.Range(0, mountainPrefabs.Length)];
            GameObject spawnedMountain = Instantiate(mountainPrefab, spawnPosition, Quaternion.identity, mountainSpawnParent);
            float randomScale = Random.Range(scaleRange.x, scaleRange.y);
            spawnedMountain.transform.localScale = new Vector3(randomScale, randomScale, randomScale);

            StartCoroutine(DespawnMountain(spawnedMountain, 5f));
            lastSpawnTime = Time.time;
        }
    }

    private IEnumerator DespawnMountain(GameObject mountain, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(mountain);
    }
}