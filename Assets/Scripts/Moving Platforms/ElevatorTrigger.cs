using UnityEngine;

public class ElevatorTrigger : MonoBehaviour
{
    [SerializeField] private ElevatorController elevatorController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            elevatorController.SetPlayerOnElevator(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            elevatorController.SetPlayerOnElevator(false);
        }
    }
}
