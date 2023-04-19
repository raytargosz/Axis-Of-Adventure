using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Opsive.UltimateCharacterController.Camera;
using Opsive.UltimateCharacterController.Character;
using Opsive.UltimateCharacterController.Events;

public class NPCDialogue : MonoBehaviour
{
    public GameObject dialogueCanvas;
    public Image dialogueBackground;
    public TextMeshProUGUI dialogueText;
    public string[] dialogueLines;
    public float textTypeSpeed = 0.05f;
    public TextMeshProUGUI interactIndicator;
    public float fadeDuration = 0.5f;
    public Canvas npcTextController;

    private UltimateCharacterLocomotion playerController;
    private CameraController cameraController;
    private int currentLineIndex = 0;
    private bool inRange = false;
    private bool isTyping = false;

    void Start()
    {
        Debug.Log("F key pressed");
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
            if (isTyping)
            {
                StopAllCoroutines();
                dialogueText.text = dialogueLines[currentLineIndex];
                isTyping = false;
            }
            else
            {
                if (!IsDialogueCanvasVisible())
                {
                    StartDialogue();
                    DisplayNextLine();
                }
                else
                {
                    DisplayNextLine();
                }
            }
        }
    }

    private void DisplayNextLine()
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
        Debug.Log("Dialogue Canvas should be visible now"); // Add this debug statement
        npcTextController.enabled = false;
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
        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(textTypeSpeed);
        }
        isTyping = false;
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