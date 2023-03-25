using UnityEngine;

public class VSyncSetup : MonoBehaviour
{
    void Start()
    {
        QualitySettings.vSyncCount = 1;
    }
}