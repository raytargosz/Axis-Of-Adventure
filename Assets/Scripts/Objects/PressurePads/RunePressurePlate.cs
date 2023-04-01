using UnityEngine;

public class RunePressurePlate : MonoBehaviour
{
    public int requiredPillarID;
    private bool isActivated = false;
    public AudioClip activationSound; // New variable for the activation sound

    public bool IsCorrectPillar(RunePillar pillar)
    {
        return pillar.pillarID == requiredPillarID;
    }

    public void SetActivated(bool activated)
    {
        isActivated = activated;
        if (activated)
        {
            AudioSource.PlayClipAtPoint(activationSound, transform.position, 1.0f); 
        }
    }

    public bool IsActivated()
    {
        return isActivated;
    }
}