using UnityEngine;

public class WorldCenterManager : MonoBehaviour
{
    [SerializeField] private WorldRotation[] worldRotations;
    private Vector3[] initialPositions;

    private void Start()
    {
        initialPositions = new Vector3[worldRotations.Length];
        for (int i = 0; i < worldRotations.Length; i++)
        {
            initialPositions[i] = worldRotations[i].transform.position;
        }
    }

    private void Update()
    {
        for (int i = 0; i < worldRotations.Length; i++)
        {
            worldRotations[i].transform.position = initialPositions[i] + Camera.main.transform.position;
            worldRotations[i].transform.rotation = Camera.main.transform.rotation;
        }
    }
}
