using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPositionManager : MonoBehaviour
{
    private Vector3 lastPosition;
    private Quaternion lastRotation;
    private bool shouldRespawn;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void SetPosition(Vector3 position, Quaternion rotation)
    {
        lastPosition = position;
        lastRotation = rotation;
        shouldRespawn = true;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (shouldRespawn)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = lastPosition;
                player.transform.rotation = lastRotation;
            }
            shouldRespawn = false;
        }
    }
}