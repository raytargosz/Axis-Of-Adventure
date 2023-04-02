using UnityEngine;
using UnityEngine.SceneManagement;


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

    private void Update()
    {
        for (int i = 1; i <= 9; i++)
        {
            if (Input.GetKeyDown(i.ToString()))
            {
                DevCheatUnlockWorldSpace(i);
            }
        }
    }

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

        // Set the first World Space dungeon completion status to true
        dungeonCompletionStatus[0] = true;

        // Initialize WorldSpaces
        worldSpaces[0].SetActive(true); // Make sure World Space 1 is active
        for (int i = 1; i < worldSpaces.Length; i++)
        {
            worldSpaces[i].SetActive(false);
        }

        // Subscribe to the SceneManager.sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    private void OnDestroy()
    {
        // Unsubscribe from the SceneManager.sceneLoaded event
        SceneManager.sceneLoaded -= OnSceneLoaded;
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
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Update the WorldSpaces based on the dungeonCompletionStatus array
        for (int i = 0; i < worldSpaces.Length; i++)
        {
            worldSpaces[i].SetActive(dungeonCompletionStatus[i]);
        }
    }

    private void DevCheatUnlockWorldSpace(int worldSpaceIndex)
    {
        if (worldSpaceIndex >= 1 && worldSpaceIndex <= worldSpaces.Length)
        {
            CompleteDungeon(worldSpaceIndex - 1);
        }
    }
}
