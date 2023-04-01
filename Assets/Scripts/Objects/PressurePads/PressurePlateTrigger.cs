using UnityEngine;

public class PressurePlateTrigger : MonoBehaviour
{
    private RunePressurePlate pressurePlate;

    void Start()
    {
        pressurePlate = GetComponentInParent<RunePressurePlate>();
    }

    private void OnTriggerEnter(Collider other)
    {
        RunePillar pillar = other.GetComponent<RunePillar>();
        if (pillar != null && pressurePlate.IsCorrectPillar(pillar))
        {
            pressurePlate.SetActivated(true);
            PillarManager.Instance.CheckPressurePlates();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        RunePillar pillar = other.GetComponent<RunePillar>();
        if (pillar != null && pressurePlate.IsCorrectPillar(pillar))
        {
            pressurePlate.SetActivated(false);
            PillarManager.Instance.CheckPressurePlates();
        }
    }
}
