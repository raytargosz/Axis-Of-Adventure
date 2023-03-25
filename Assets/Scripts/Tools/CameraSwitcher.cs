using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [System.Serializable]
    public class CameraWorldRotationPair
    {
        public Camera camera;
        public Transform worldRotationCenter;
    }

    [SerializeField] private CameraWorldRotationPair[] cameraWorldRotationPairs;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private WorldRotation worldRotation;

    private int currentActiveIndex = 0;

    private void Start()
    {
        ActivateCamera(currentActiveIndex);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            currentActiveIndex = (currentActiveIndex + 1) % cameraWorldRotationPairs.Length;
            ActivateCamera(currentActiveIndex);
        }
    }

    private void ActivateCamera(int index)
    {
        for (int i = 0; i < cameraWorldRotationPairs.Length; i++)
        {
            cameraWorldRotationPairs[i].camera.gameObject.SetActive(i == index);
        }

        worldRotation.SetActiveRotationCenter(cameraWorldRotationPairs[index].worldRotationCenter);
    }
}
