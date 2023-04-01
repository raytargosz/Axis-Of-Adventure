using UnityEngine;
using System.Collections.Generic;

public class PillarManager : MonoBehaviour
{
    public static PillarManager Instance;

    public List<RunePressurePlate> pressurePlates;
    public GameObject dungeonDoor;
    public List<GameObject> assetsToDisable;
    public List<GameObject> assetsToEnable;

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
    }
}