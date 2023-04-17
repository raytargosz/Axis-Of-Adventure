using System;
using UnityEngine;

public class PressurePadController_2 : MonoBehaviour
{
    public event Action<PressurePadController_2> OnPadTriggered;
    public bool PadTriggered { get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PadTriggered = true;
            OnPadTriggered?.Invoke(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PadTriggered = false;
            OnPadTriggered?.Invoke(this);
        }
    }
}
