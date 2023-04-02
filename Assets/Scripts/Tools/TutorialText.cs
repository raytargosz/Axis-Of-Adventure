using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialText : MonoBehaviour
{
    [SerializeField] private List<TutorialInfo> tutorials = new List<TutorialInfo>();

    [Header("Tutorial Text Settings")]
    [SerializeField] private TextMeshProUGUI tutorialTextMesh;
    [SerializeField] private float fadeOutDuration = 1f;

    [System.Serializable]
    public class TutorialInfo
    {
        [Tooltip("Tutorial text to display")]
        public TMP_Text tutorialText;

        [Tooltip("Duration the tutorial text is displayed")]
        public float duration = 3f;

        [Tooltip("SFX to play when the tutorial text comes up")]
        public AudioClip sfx;

        [Tooltip("Volume of the tutorial SFX")]
        [Range(0, 1)]
        public float volume = 1f;
    }

    private int currentTutorial = 0;
    private bool tutorialActive = false;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !tutorialActive)
        {
            ActivateTutorial();
        }
    }

    private void ActivateTutorial()
    {
        if (currentTutorial < tutorials.Count)
        {
            TutorialInfo current = tutorials[currentTutorial];
            tutorialActive = true;
            current.tutorialText.gameObject.SetActive(true);
            current.tutorialText.alpha = 1;
            audioSource.PlayOneShot(current.sfx, current.volume);

            StartCoroutine(DisplayAndFadeOutText(current.duration));
        }
    }

    private IEnumerator DisplayAndFadeOutText(float displayDuration)
    {
        yield return new WaitForSeconds(displayDuration);
        FadeOutTutorialText();
    }

    public void FadeOutTutorialText()
    {
        StartCoroutine(FadeOutText());
    }

    private IEnumerator FadeOutText()
    {
        float startTime = Time.time;
        Color textColor = tutorialTextMesh.color;

        while (Time.time - startTime < fadeOutDuration)
        {
            float progress = (Time.time - startTime) / fadeOutDuration;
            textColor.a = Mathf.Lerp(1, 0, progress);
            tutorialTextMesh.color = textColor;
            yield return null;
        }

        textColor.a = 0;
        tutorialTextMesh.color = textColor;
        tutorialActive = false;
    }
}
