using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Opsive.UltimateCharacterController.Camera;
using Opsive.UltimateCharacterController.Character;

public class NPCDialogue : MonoBehaviour
{
    public GameObject dialogueCanvas;
    public Image dialogueBackground;
    public TextMeshProUGUI dialogueText;
    public string[] dialogueLines;
    public float textTypeSpeed = 0.05f;
    public TextMeshProUGUI interactIndicator;
    public float fadeDuration = 0.5f;

    private UltimateCharacterLocomotion playerController;
    private CameraController cameraController;
    private int currentLineIndex = 0;
    private bool inRange = false;
    private bool isTyping = false;

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
                }
                else if (currentLineIndex < dialogueLines.Length)
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
        playerController.enabled = false;
        cameraController.enabled = false;
        StartCoroutine(FadeDialogueCanvas(1, fadeDuration));
    }

    private void EndDialogue()
    {
        currentLineIndex = 0;
        StartCoroutine(FadeDialogueCanvas(0, fadeDuration));
        playerController.enabled = true;
        cameraController.enabled = true;
    }

    IEnumerator TypeText(string line)
    {
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
        return dialogueBackground.color.a > 0;
    }
}