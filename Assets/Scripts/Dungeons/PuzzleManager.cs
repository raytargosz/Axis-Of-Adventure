using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    [Header("Puzzle Elements")]
    [Tooltip("The LightBeamController script attached to the light source game object")]
    public LightBeamController lightBeamController;

    [Tooltip("The TargetController script attached to the target game object")]
    public TargetController targetController;

    [Space]
    [Header("Puzzle State")]
    [Tooltip("Indicates if the puzzle is currently solved or not")]
    [SerializeField] private bool isSolved = false;

    [Space]
    [Header("Puzzle Rewards and Events")]
    [Tooltip("The game object to be activated upon solving the puzzle, e.g. a door or hidden passage")]
    public GameObject objectToActivate;

    [Tooltip("The reward to be spawned upon solving the puzzle, e.g. a treasure chest or collectible")]
    public GameObject rewardPrefab;

    [Tooltip("The position where the reward will spawn")]
    public Transform rewardSpawnPoint;

    private void Update()
    {
        // Check if the puzzle is solved by detecting if the light beam is hitting the target
        if (targetController.IsActivated() && !isSolved)
        {
            SolvePuzzle();
        }
    }

    private void SolvePuzzle()
    {
        // Set the puzzle state to solved
        isSolved = true;

        // Activate the object associated with solving the puzzle (e.g., open a door or reveal a hidden passage)
        if (objectToActivate != null)
        {
            objectToActivate.SetActive(true);
        }

        // Spawn the reward at the designated spawn point
        if (rewardPrefab != null && rewardSpawnPoint != null)
        {
            Instantiate(rewardPrefab, rewardSpawnPoint.position, rewardSpawnPoint.rotation);
        }

        // Provide feedback to the player, such as a sound effect, particle effect, or on-screen message
        // ...
    }
}
