// This script should be attached to a trigger collider in your Unity scene.
// When the player enters the collider, it will display the tutorial text
// specified by the tutorialIndex. The text will be displayed using the
// TutorialText script. The game will freeze, and the text's alpha will
// be set to 255. When the player presses the required button(s), the game
// will unfreeze, the text's alpha will return to 0, and the trigger will be destroyed.
//
// To set up the tutorial text:
// 1. Attach this script to a GameObject with a Collider component (set as a trigger).
// 2. Assign the TutorialText script instance to the 'tutorialText' field in the Inspector.
// 3. Set the 'tutorialIndex' field to the index of the tutorial text you want to display.
// 4. Make sure the player GameObject has a tag "Player".
//
// Make sure your tutorial text objects use TextMeshPro (TMP) components.

using UnityEngine;
using System.Collections;

public class TutorialTrigger : MonoBehaviour
{
    [Header("Tutorial Trigger Settings")]
    [Tooltip("Reference to the TutorialText component")]
    [SerializeField] private TutorialText tutorialText;

    [Tooltip("The index of the tutorial that will be activated by this trigger")]
    [SerializeField] private int tutorialIndex;

    [Tooltip("Required button(s) to dismiss the tutorial")]
    [SerializeField] private KeyCode[] requiredKeys;

    private bool isActivated = false;

    private void Start()
    {
        isActivated = false;
        gameObject.SetActive(false);
        CameraSwoop cameraSwoop = FindObjectOfType<CameraSwoop>();
        if (cameraSwoop != null)
        {
            cameraSwoop.onSwoopComplete.AddListener(() => StartCoroutine(ActivateTriggerAfterDelay()));
        }
    }

    private IEnumerator ActivateTriggerAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isActivated)
        {
            tutorialText.ActivateTutorialFromCollider(tutorialIndex);
            isActivated = true;
        }
    }

    private void Update()
    {
        if (isActivated && AreRequiredKeysPressed())
        {
            tutorialText.FadeOutTutorialText();
            Destroy(gameObject);
        }
    }

    private bool AreRequiredKeysPressed()
    {
        foreach (KeyCode key in requiredKeys)
        {
            if (Input.GetKeyDown(key))
            {
                return true;
            }
        }
        return false;
    }
}
