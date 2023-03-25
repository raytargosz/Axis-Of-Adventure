using System;
using UnityEditor;
using UnityEngine;

public class WorktimeTrackerEditor : EditorWindow
{
    private const string TotalTimeKey = "TotalTime";
    private const string StartTimeKey = "StartTime";
    private const string LastOpenedTimeKey = "LastOpenedTime";

    private TimeSpan totalTime;
    private TimeSpan sessionTime;

    [MenuItem("Tools/Worktime Tracker")]
    public static void ShowWindow()
    {
        GetWindow<WorktimeTrackerEditor>("Worktime Tracker");
    }

    private void OnEnable()
    {
        double savedTotalTime = EditorPrefs.GetFloat(TotalTimeKey, 0);
        totalTime = TimeSpan.FromSeconds(savedTotalTime);

        double lastOpenedTime = EditorPrefs.GetFloat(LastOpenedTimeKey, (float)EditorApplication.timeSinceStartup);
        double startTime = EditorPrefs.GetFloat(StartTimeKey, (float)EditorApplication.timeSinceStartup);
        sessionTime = TimeSpan.FromSeconds(EditorApplication.timeSinceStartup - startTime);

        EditorApplication.update += Update;
    }

    private void OnDisable()
    {
        EditorApplication.update -= Update;

        EditorPrefs.SetFloat(TotalTimeKey, (float)totalTime.TotalSeconds);
        EditorPrefs.SetFloat(LastOpenedTimeKey, (float)EditorApplication.timeSinceStartup);
    }

    private void Update()
    {
        TimeSpan deltaTime = TimeSpan.FromSeconds(EditorApplication.timeSinceStartup - EditorPrefs.GetFloat(LastOpenedTimeKey));
        totalTime += deltaTime;
        sessionTime += deltaTime;

        EditorPrefs.SetFloat(LastOpenedTimeKey, (float)EditorApplication.timeSinceStartup);
        Repaint();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Total Worktime:", totalTime.ToString(@"hh\:mm\:ss"));
        EditorGUILayout.LabelField("Current Session:", sessionTime.ToString(@"hh\:mm\:ss"));
    }
}