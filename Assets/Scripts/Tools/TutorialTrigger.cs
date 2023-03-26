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
    [Header("Tutorial Settings")]
    [Tooltip("Reference to the TutorialText script.")]
    [SerializeField] private TutorialText tutorialText;

    [Tooltip("Index of the tutorial text to display when the trigger is activated.")]
    [SerializeField] private int tutorialIndex;

    [Header("Activation Settings")]
    [Tooltip("The delay in seconds after the camera swoop is completed before activating the trigger.")]
    [SerializeField] private float activationDelay = 1f;

    private void Start()
    {
        // Disable the collider at the beginning
        GetComponent<Collider>().enabled = false;

        // Subscribe to the onSwoopComplete event from the CameraSwoop script
        FindObjectOfType<CameraSwoop>().onSwoopComplete.AddListener(ActivateTriggerAfterDelay);
    }

    IEnumerator ActivateTriggerAfterDelay()
    {
        yield return new WaitForSeconds(activationDelay);

        // Enable the collider after the specified delay
        GetComponent<Collider>().enabled = true;
    }

    private void OnDestroy()
    {
        // Unsubscribe from the onSwoopComplete event when the object is destroyed
        FindObjectOfType<CameraSwoop>().onSwoopComplete.RemoveListener(ActivateTriggerAfterDelay);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            tutorialText.ActivateTutorialFromCollider(tutorialIndex);
            Destroy(gameObject);
        }
    }
}
