using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField] private TutorialText tutorialText;
    [SerializeField] private int tutorialIndex;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            tutorialText.ActivateTutorialFromCollider(tutorialIndex);
            Destroy(gameObject);
        }
    }
}
