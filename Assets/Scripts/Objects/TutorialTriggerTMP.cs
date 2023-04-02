using UnityEngine;

public class TutorialTriggerTMP : MonoBehaviour
{
    [SerializeField] private TutorialTextTMP tutorialText;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            tutorialText.ShowTutorialText();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            tutorialText.HideTutorialText();
        }
    }
}