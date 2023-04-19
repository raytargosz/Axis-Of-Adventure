using System.Collections;
using UnityEngine;
using TMPro;

public class NPCTextController : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("The TextMeshProUGUI component for displaying the dialogue")]
    public TextMeshProUGUI textMeshPro;

    [Header("References")]
    [Tooltip("The cameras's Transform component")]
    public Transform mainCameraTransform;
    [Tooltip("The NPC GameObject")]
    public GameObject npcObject;
    [Tooltip("The main camera in the scene")]
    public Camera mainCamera;
    [Tooltip("The canvas containing the TextMeshProUGUI element")]
    public Canvas canvas;

    [Header("Dialogue Settings")]
    [Tooltip("An array of dialogue lines to display")]
    public string[] dialogueLines;
    [Tooltip("The duration for fading in the dialogue text")]
    public float fadeInTime = 1f;
    [Tooltip("The duration for holding the dialogue text on screen")]
    public float holdTime = 1f;
    [Tooltip("The duration for fading out the dialogue text")]
    public float fadeOutTime = 1f;
    [Tooltip("The range of random intervals between dialogue lines")]
    public Vector2 timeBetweenDialogueRange = new Vector2(1f, 4f);

    [Header("Position Settings")]
    [Tooltip("The offset of the TextMeshProUGUI element relative to the NPC's position")]
    public Vector3 textOffset;
    public Vector3 rotationOffset;

    private void Start()
    {
        StartCoroutine(DialogueRoutine());
    }

    private void UpdateTextPosition()
    {
        Vector3 npcPosition = npcObject.transform.position;
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(npcPosition + textOffset);
        textMeshPro.transform.position = screenPosition;
    }

    private void Update()
    {
        // Set the TMP element position directly above the NPC's head
        Vector3 targetPosition = npcObject.transform.position + textOffset;
        textMeshPro.transform.position = targetPosition;

        // Face the camera
        Vector3 directionToFace = (mainCameraTransform.position - textMeshPro.transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToFace, Vector3.up);
        textMeshPro.transform.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);

        // Mirror the text
        textMeshPro.transform.localScale = new Vector3(-1, 1, 1);
    }

    private IEnumerator DialogueRoutine()
    {
        // Display the first dialogue line
        int initialIndex = Random.Range(0, dialogueLines.Length);
        string initialDialogueLine = dialogueLines[initialIndex];
        textMeshPro.text = initialDialogueLine;
        textMeshPro.color = new Color(textMeshPro.color.r, textMeshPro.color.g, textMeshPro.color.b, 1);
        yield return new WaitForSeconds(holdTime);

        while (true)
        {
            // Fade out
            float startTime = Time.time;
            while (Time.time < startTime + fadeOutTime)
            {
                float t = 1 - (Time.time - startTime) / fadeOutTime;
                textMeshPro.color = new Color(textMeshPro.color.r, textMeshPro.color.g, textMeshPro.color.b, t);
                yield return null;
            }
            textMeshPro.color = new Color(textMeshPro.color.r, textMeshPro.color.g, textMeshPro.color.b, 0);

            // Random time between dialogue lines
            float randomInterval = Random.Range(timeBetweenDialogueRange.x, timeBetweenDialogueRange.y);
            yield return new WaitForSeconds(randomInterval);

            // Change text
            int randomIndex = Random.Range(0, dialogueLines.Length);
            string dialogueLine = dialogueLines[randomIndex];
            textMeshPro.text = dialogueLine;

            // Fade in
            startTime = Time.time;
            while (Time.time < startTime + fadeInTime)
            {
                float t = (Time.time - startTime) / fadeInTime;
                textMeshPro.color = new Color(textMeshPro.color.r, textMeshPro.color.g, textMeshPro.color.b, t);
                yield return null;
            }
            textMeshPro.color = new Color(textMeshPro.color.r, textMeshPro.color.g, textMeshPro.color.b, 1);

            // Hold
            yield return new WaitForSeconds(holdTime);
        }
    }
}
