using UnityEngine;

public class MusicVolumeController : MonoBehaviour
{
    public Transform player;
    public AudioSource mainMusic;
    public AudioSource[] audioSources;
    public float maxDistance = 10f;

    private void Update()
    {
        // Find the minimum distance to the audio sources
        float minDistance = float.MaxValue;
        foreach (AudioSource audioSource in audioSources)
        {
            float distance = Vector3.Distance(player.position, audioSource.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
            }
        }

        // Calculate the volume multiplier based on the minimum distance
        float volumeMultiplier = Mathf.Clamp01(minDistance / maxDistance);

        // Set the main music volume using the volume multiplier
        mainMusic.volume = volumeMultiplier;
    }
}