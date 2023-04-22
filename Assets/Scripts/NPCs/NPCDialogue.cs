using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Opsive.UltimateCharacterController.Camera;
using Opsive.UltimateCharacterController.Character;
using Opsive.UltimateCharacterController.Events; 

public class NPCDialogue : MonoBehaviour
{
    [Header("Dialogue UI")]
    [Tooltip("The parent GameObject containing the dialogue UI elements.")]
    public GameObject dialogueCanvas;
    [Tooltip("The Image component for the dialogue background.")]
    public Image dialogueBackground;
    [Tooltip("The TextMeshProUGUI component displaying the dialogue text.")]
    public TextMeshProUGUI dialogueText;

    [Header("Dialogue Content")]
    [Tooltip("An array of strings containing the dialogue lines.")]
    public string[] dialogueLines;
    [Tooltip("The typing speed of the dialogue text.")]
    public float textTypeSpeed = 0.05f;

    [Header("Interact Indicator")]
    [Tooltip("The TextMeshProUGUI component that displays the interact indicator.")]
    public TextMeshProUGUI interactIndicator;

    [Header("Dialogue Fade")]
    [Tooltip("The duration of the dialogue canvas fade in and out.")]
    public float fadeDuration = 0.5f;

    [Header("NPC Text Controller")]
    [Tooltip("The Canvas component containing the NPC's text UI elements.")]
    public Canvas npcTextController;

    [Header("Audio")]
    [Tooltip("An array of audio clips corresponding to the dialogue lines.")]
    public AudioClip[] dialogueAudioClips;
    [Tooltip("The AudioSource component responsible for playing the audio.")]
    public AudioSource audioSource;
    [Tooltip("The volume of the dialogue audio clips.")]
    [Range(0f, 1f)]
    public float dialogueVolume = 1f;

    private bool startDialogCalled = false;
    private UltimateCharacterLocomotion playerController;
    private CameraController cameraController;
    private int currentLineIndex = 0;
    private bool inRange = false;
    private bool isTyping = false;
    private bool firstInteraction = true;

    void Start()
    {
        // Set the initial alpha of the dialogue canvas elements
        SetDialogueCanvasAlpha(0);

        // Set the initial alpha of the interact indicator to 0
        Color textColor = interactIndicator.color;
        textColor.a = 0;
        interactIndicator.color = textColor;
    }

    void Update()
    {
        if (inRange && Input.GetKeyDown(KeyCode.F))
        {
            if (!IsDialogueCanvasVisible())
            {
                if (firstInteraction)
                {
                    StartDialogue();
                    DisplayNextLine();
                    firstInteraction = false;
                }
                else
                {
                    DisplayNextLine();
                }
            }
            else
            {
                DisplayNextLine();
            }
        }
    }

    private void DisplayNextLine()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.text = dialogueLines[currentLineIndex];
            isTyping = false;
            audioSource.Stop();
        }
        else
        {
            if (currentLineIndex < dialogueLines.Length)
            {
                StartCoroutine(TypeText(dialogueLines[currentLineIndex]));
                currentLineIndex++;
            }
            else
            {
                EndDialogue();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inRange = true;
            playerController = other.GetComponent<UltimateCharacterLocomotion>();
            cameraController = other.GetComponent<CameraController>();

            // Show the interact indicator by setting alpha to 1
            Color textColor = interactIndicator.color;
            textColor.a = 1;
            interactIndicator.color = textColor;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inRange = false;
            EndDialogue();

            // Hide the interact indicator by setting alpha to 0
            Color textColor = interactIndicator.color;
            textColor.a = 0;
            interactIndicator.color = textColor;
        }
    }

    private void StartDialogue()
    {
        EnableControllers(false);
        StartCoroutine(FadeDialogueCanvas(1, fadeDuration));
        npcTextController.enabled = false;
        startDialogCalled = true;
    }

    private void EndDialogue()
    {
        Debug.Log("F key pressed");
        currentLineIndex = 0;
        StartCoroutine(FadeDialogueCanvas(0, fadeDuration));
        EnableControllers(true);
        npcTextController.enabled = true;

    }

    private void EnableControllers(bool enabled)
    {
        if (playerController != null)
        {
            playerController.enabled = enabled;
        }

        if (cameraController != null)
        {
            cameraController.enabled = enabled;
        }
    }

    IEnumerator TypeText(string line)
    {
        Debug.Log("TypeText Coroutine called");
        isTyping = true;
        dialogueText.text = "";
        audioSource.clip = dialogueAudioClips[currentLineIndex - 1];
        audioSource.Play();
        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(textTypeSpeed);
        }
        isTyping = false;
        audioSource.Stop();
    }

    IEnumerator FadeDialogueCanvas(float targetAlpha, float duration)
    {
        float startAlpha = dialogueBackground.color.a;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);

            SetDialogueCanvasAlpha(newAlpha);

            yield return null;
        }
    }

    private void SetDialogueCanvasAlpha(float alpha)
    {
        Color backgroundColor = dialogueBackground.color;
        backgroundColor.a = alpha;
        dialogueBackground.color = backgroundColor;

        Color textColor = dialogueText.color;
        textColor.a = alpha;
        dialogueText.color = textColor;
    }

    private bool IsDialogueCanvasVisible()
    {
        return dialogueText.color.a > 0.1f;
    }
}