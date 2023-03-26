using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialText : MonoBehaviour
{
    [SerializeField] private List<TutorialInfo> tutorials = new List<TutorialInfo>();

    [System.Serializable]
    public class TutorialInfo
    {
        [Tooltip("Tutorial text to display")]
        public TMP_Text tutorialText;

        [Tooltip("Required buttons to press to continue")]
        public KeyCode[] requiredButtons = { KeyCode.Space };

        [Tooltip("SFX to play when the tutorial text comes up")]
        public AudioClip sfx;

        [Tooltip("Volume of the tutorial SFX")]
        [Range(0, 1)]
        public float volume = 1f;
    }

    private int currentTutorial = 0;
    private bool tutorialActive = false;
    private AudioSource audioSource;
    public bool HasStarted { get; private set; } = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();        
    }

    private void Update()
    {
        if (tutorialActive)
        {
            TutorialInfo current = tutorials[currentTutorial];
            foreach (KeyCode key in current.requiredButtons)
            {
                if (Input.GetKeyDown(key))
                {
                    Time.timeScale = 1;
                    tutorialActive = false;
                    current.tutorialText.gameObject.SetActive(false);
                    if (++currentTutorial < tutorials.Count)
                    {
                        StartCoroutine(ActivateTutorialWithDelay(1f));
                    }
                    break;
                }
            }
        }
    }

    private IEnumerator ActivateTutorialWithDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        ActivateTutorial();
    }

    public void StartInitialTutorial(float delay)
    {
        if (!HasStarted)
        {
            HasStarted = true;
            StartCoroutine(ActivateTutorialWithDelay(delay));
        }
    }

    private void ActivateTutorial()
    {
        if (currentTutorial < tutorials.Count)
        {
            TutorialInfo current = tutorials[currentTutorial];
            tutorialActive = true;
            Time.timeScale = 0;
            current.tutorialText.gameObject.SetActive(true);
            audioSource.PlayOneShot(current.sfx, current.volume);
        }
    }
}
