/*
 * The PillarManager script manages the pressure plates and dungeon door in a puzzle game. 
 * When all pressure plates are activated, the dungeon door unlocks and a set of assets 
 * are enabled while another set are disabled. An array of sounds can be played when the 
 * dungeon door is unlocked.
 *
 * To use this script, create a new GameObject and attach the script to it. Assign the 
 * required pressure plates, dungeon door, and assets to enable/disable in the inspector.
 * Add any unlock sounds to the unlockSounds array. When all pressure plates are activated, 
 * the dungeon door will unlock and the corresponding assets will be enabled/disabled. 
 * The unlock sounds will also play if any are assigned.
 */

using UnityEngine;
using System.Collections.Generic;

public class PillarManager : MonoBehaviour
{
    [Header("Pressure Plate Settings")]
    [Tooltip("The list of pressure plates to check for activation.")]
    public List<RunePressurePlate> pressurePlates;

    [Header("Door Settings")]
    [Tooltip("The dungeon door to unlock when all pressure plates are activated.")]
    public GameObject dungeonDoor;

    [Header("Asset Settings")]
    [Tooltip("The list of assets to disable when the dungeon door is unlocked.")]
    public List<GameObject> assetsToDisable;
    [Tooltip("The list of assets to enable when the dungeon door is unlocked.")]
    public List<GameObject> assetsToEnable;

    [Header("Sound Settings")]
    [Tooltip("The array of sounds to play when the dungeon door is unlocked.")]
    public AudioClip[] unlockSounds;

    public static PillarManager Instance;

    void Awake()
    {
        Instance = this;
    }

    public void CheckPressurePlates()
    {
        bool allActivated = true;
        foreach (RunePressurePlate plate in pressurePlates)
        {
            if (!plate.IsActivated())
            {
                allActivated = false;
                break;
            }
        }

        if (allActivated)
        {
            UnlockDungeon();
        }
    }

    private void UnlockDungeon()
    {
        // Disable and enable the corresponding assets
        foreach (GameObject asset in assetsToDisable)
        {
            asset.SetActive(false);
        }

        foreach (GameObject asset in assetsToEnable)
        {
            asset.SetActive(true);
        }

        // Play unlock sounds
        foreach (AudioClip sound in unlockSounds)
        {
            AudioSource.PlayClipAtPoint(sound, transform.position);
        }
    }
}
