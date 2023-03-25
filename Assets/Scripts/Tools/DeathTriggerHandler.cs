using UnityEngine;

public class DeathTriggerHandler : MonoBehaviour
{
    [SerializeField] private PlayerDeath playerDeath;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerDeath.TriggerDeathSequence();
        }
    }
}
