using UnityEngine;
using System.Collections.Generic;

public class PillarManager : MonoBehaviour
{
    public static PillarManager Instance;

    public List<RunePressurePlate> pressurePlates;
    public GameObject dungeonDoor;

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
        // Replace with your own logic to unlock the dungeon door
        dungeonDoor.SetActive(false);
    }
}
