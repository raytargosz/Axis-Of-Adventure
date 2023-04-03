using UnityEngine;

public class DoorKeyUnlock : MonoBehaviour
{
    [Header("Unlockable Assets")]
    [Tooltip("Array of LockedDoor objects to unlock")]
    public LockedDoor[] doorsToUnlock;

    [Header("Power Activation")]
    [Tooltip("LeverController objects to activate power")]
    public LeverController[] leversToActivatePower;

    private bool keyAcquired = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !keyAcquired)
        {
            keyAcquired = true;
            UnlockDoors();
            gameObject.SetActive(false);
        }
    }

    private void UnlockDoors()
    {
        foreach (LockedDoor door in doorsToUnlock)
        {
            door.UnlockDoor();
        }

        foreach (LeverController lever in leversToActivatePower)
        {
            lever.ActivatePower();
        }
    }
}