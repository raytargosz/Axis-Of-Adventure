using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton instance
    public static GameManager Instance { get; private set; }

    // Variables
    [Tooltip("List of WorldSpaces in the game")]
    public GameObject[] worldSpaces;

    private int collectedCubes = 0;
    private bool[] dungeonCompletionStatus;
    private int currentWorldSpace = 1;

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Initialize dungeon completion status array
        dungeonCompletionStatus = new bool[worldSpaces.Length];

        // Initialize WorldSpaces
        for (int i = 1; i < worldSpaces.Length; i++)
        {
            worldSpaces[i].SetActive(false);
        }
    }

    // Cube Collection
    public void AddCubes(int count)
    {
        collectedCubes += count;
    }

    public int GetCollectedCubes()
    {
        return collectedCubes;
    }

    // Dungeon Unlocking
    public bool CheckDungeonUnlockStatus(int worldSpaceIndex)
    {
        // Check if the dungeon in the specified WorldSpace should be unlocked (based on collected cube count)
        // You may need to adjust the unlocking logic based on your game's requirements.
        return collectedCubes >= worldSpaceIndex * 10;
    }

    public void UnlockDungeon(int worldSpaceIndex)
    {
        // Unlock the dungeon in the specified WorldSpace
        dungeonCompletionStatus[worldSpaceIndex - 1] = true;
    }

    // Dungeon Completion
    public void CompleteDungeon(int worldSpaceIndex)
    {
        // Mark the dungeon in the specified WorldSpace as completed
        dungeonCompletionStatus[worldSpaceIndex - 1] = true;

        // Unlock the next WorldSpace if not already unlocked
        if (worldSpaceIndex < worldSpaces.Length && !dungeonCompletionStatus[worldSpaceIndex])
        {
            UnlockNextWorldSpace();
        }
    }

    // WorldSpace Management
    private void UnlockNextWorldSpace()
    {
        // Unlock the next WorldSpace
        currentWorldSpace++;
        worldSpaces[currentWorldSpace - 1].SetActive(true);
    }

    public int GetCurrentWorldSpace()
    {
        return currentWorldSpace;
    }

    public void SetCurrentWorldSpace(int worldSpaceIndex)
    {
        currentWorldSpace = worldSpaceIndex;
    }
}
