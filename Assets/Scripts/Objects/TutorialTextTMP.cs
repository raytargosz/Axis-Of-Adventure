using TMPro;
using UnityEngine;

public class TutorialTextTMP : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sfx;

    public void ActivateTutorialFromCollider()
    {
        // using ShowTutorialText() and HideTutorialText() 
    }

    public void ShowTutorialText()
    {
        tutorialText.gameObject.SetActive(true);
        PlaySFX();
    }

    public void HideTutorialText()
    {
        tutorialText.gameObject.SetActive(false);
    }

    private void PlaySFX()
    {
        if (sfx != null)
        {
            audioSource.PlayOneShot(sfx);
        }
    }
}
