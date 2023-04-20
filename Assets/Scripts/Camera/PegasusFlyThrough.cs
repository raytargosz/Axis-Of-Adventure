using UnityEngine;
using Pegasus;

public class PegasusFlyThrough : MonoBehaviour
{
    [Header("Pegasus Settings")]
    [Tooltip("Pegasus Manager for the camera fly-through")]
    public PegasusManager pegasusManager;

    [Header("Completion Waypoint")]
    [Tooltip("Waypoint GameObject representing the completion trigger")]
    public GameObject completionWaypoint;

    [Header("Activation Settings")]
    [Tooltip("GameObjects to enable when the camera enters the completion trigger")]
    public GameObject[] objectsToEnable;

    [Tooltip("GameObjects to disable when the camera enters the completion trigger")]
    public GameObject[] objectsToDisable;

    private bool flythroughCompleted = false;

    void Update()
    {
        if (!flythroughCompleted && pegasusManager != null && CheckCompletion())
        {
            flythroughCompleted = true;
            HandleFlyThroughCompletion();
        }
    }

    private bool CheckCompletion()
    {
        Collider completionCollider = completionWaypoint.GetComponent<Collider>();
        if (completionCollider != null)
        {
            return completionCollider.bounds.Contains(pegasusManager.transform.position);
        }
        return false;
    }

    private void HandleFlyThroughCompletion()
    {
        Debug.Log("Handling fly-through completion...");

        foreach (GameObject objectToEnable in objectsToEnable)
        {
            if (objectToEnable != null)
            {
                objectToEnable.SetActive(true);
            }
        }

        foreach (GameObject objectToDisable in objectsToDisable)
        {
            if (objectToDisable != null)
            {
                objectToDisable.SetActive(false);
            }
        }
    }
}
