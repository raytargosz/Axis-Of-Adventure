using UnityEngine;

/// <summary>
/// This script should be attached to an object in a dungeon that represents the completion condition,
/// such as the final boss, last room, or exit door. When the player interacts with this object,
/// the dungeon will be marked as completed, and the next world space will be unlocked as needed.
/// </summary>
public class DungeonCompletion : MonoBehaviour
{
    [Header("Dungeon Configuration")]
    [Tooltip("The index of the WorldSpace this dungeon belongs to.")]
    [SerializeField] private int worldSpaceIndex;

    /// <summary>
    /// This method is called when a Collider enters the trigger zone of the object.
    /// </summary>
    /// <param name="other">The Collider that entered the trigger zone.</param>
    private void OnTriggerEnter(Collider other)
    {
        // Check if the Collider that entered the trigger zone is tagged as "Player".
        if (other.CompareTag("Player"))
        {
            // Call the CompleteDungeon method from the GameManager to mark the dungeon as completed
            // and unlock the next WorldSpace if needed.
            GameManager.Instance.CompleteDungeon(worldSpaceIndex);
        }
    }
}
