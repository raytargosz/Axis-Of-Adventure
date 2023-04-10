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
    public float cameraZoomAmount = 15f;

    private UltimateCharacterLocomotion playerController; 
    private CameraController cameraController; 
    private int currentLineIndex = 0;
    private bool inRange = false;
    private bool isTyping = false;
    private Camera mainCamera;
    private float originalFov;

    void Start()
    {
        dialogueCanvas.SetActive(false);
        mainCamera = Camera.main;
        originalFov = mainCamera.fieldOfView;
    }

    void Update()
    {
        if (inRange && (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Space)))
        {
            if (isTyping)
            {
                StopAllCoroutines();
                dialogueText.text = dialogueLines[currentLineIndex];
                isTyping = false;
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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inRange = true;
            playerController = other.GetComponent<UltimateCharacterLocomotion>();
            cameraController = other.GetComponent<CameraController>();
            playerController.enabled = false;
            cameraController.enabled = false;
            mainCamera.fieldOfView = originalFov - cameraZoomAmount;
            dialogueCanvas.SetActive(true);
            dialogueBackground.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inRange = false;
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        currentLineIndex = 0;
        dialogueCanvas.SetActive(false);
        dialogueBackground.enabled = false;
        playerController.enabled = true;
        cameraController.enabled = true;
        mainCamera.fieldOfView = originalFov;
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
}
