using UnityEngine;
using UnityEngine.UI;

public class ObjectiveManager : MonoBehaviour
{
    public GameObject objectivePanel;
    public Text objectiveText;
    public Objective[] objectives;

    private int currentObjectiveIndex;

    void Start()
    {
        currentObjectiveIndex = 0;
        UpdateObjectiveUI();
    }

    void Update()
    {
        objectives[currentObjectiveIndex].CheckCompletion();

        if (objectives[currentObjectiveIndex].isCompleted)
        {
            TaskCompleted();
        }
    }

    public void TaskCompleted()
    {
        if (currentObjectiveIndex < objectives.Length - 1)
        {
            currentObjectiveIndex++;
            UpdateObjectiveUI();
        }
        else
        {
            objectiveText.text = "All objectives complete!";
        }
    }

    private void UpdateObjectiveUI()
    {
        objectiveText.text = objectives[currentObjectiveIndex].description;
    }
}
