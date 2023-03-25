using UnityEngine;
using UnityEngine.Events;

public class DeathTrigger : MonoBehaviour
{
    public UnityEvent onPlayerDeath;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onPlayerDeath.Invoke();
        }
    }
}
