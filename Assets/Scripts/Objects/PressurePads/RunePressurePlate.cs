using UnityEngine;

public class RunePressurePlate : MonoBehaviour
{
    public int requiredPillarID;
    private bool isActivated = false;

    public bool IsCorrectPillar(RunePillar pillar)
    {
        return pillar.pillarID == requiredPillarID;
    }

    public void SetActivated(bool activated)
    {
        isActivated = activated;
    }

    public bool IsActivated()
    {
        return isActivated;
    }
}
