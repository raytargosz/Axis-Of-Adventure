using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private CameraSwoop cameraSwoop;

    private float elapsedTime;
    public bool HasFinishedLevel { get; private set; }

    void Update()
    {
        if (!HasFinishedLevel && !cameraSwoop.IsSwooping)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerText();
        }
    }

    private void UpdateTimerText()
    {
        int minutes = (int)(elapsedTime / 60);
        int seconds = (int)(elapsedTime % 60);
        int milliseconds = (int)((elapsedTime * 1000) % 1000);

        timerText.text = $"{minutes:00}:{seconds:00}.{milliseconds:000}";
    }

    public void FinishLevel()
    {
        HasFinishedLevel = true;
    }
}