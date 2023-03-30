using UnityEngine;
using UnityEngine.UI;

public class DoorInteraction : MonoBehaviour
{
    [SerializeField] private GameObject[] doors;
    [SerializeField] private float openAngle;
    [SerializeField] private float openSpeed = 2.0f;
    [SerializeField] private Text interactionText;
    [SerializeField] private bool openAwayFromPlayer = true;

    private bool isOpening;
    private bool playerInsideTrigger;

    private void Start()
    {
        interactionText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (playerInsideTrigger && Input.GetKeyDown(KeyCode.F))
        {
            isOpening = !isOpening;
            ToggleDoorColliders(false);
        }

        OpenCloseDoors();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInsideTrigger = true;
            interactionText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInsideTrigger = false;
            interactionText.gameObject.SetActive(false);
        }
    }

    private void ToggleDoorColliders(bool enabled)
    {
        foreach (GameObject door in doors)
        {
            Collider collider = door.GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = enabled;
            }
        }
    }

    private void OpenCloseDoors()
    {
        foreach (GameObject door in doors)
        {
            Vector3 targetEulerAngles = isOpening ? new Vector3(0, openAngle, 0) : Vector3.zero;
            float openDirection = openAwayFromPlayer ? 1.0f : -1.0f;
            door.transform.localRotation = Quaternion.Lerp(
                door.transform.localRotation,
                Quaternion.Euler(targetEulerAngles * openDirection),
                openSpeed * Time.deltaTime);
        }
    }
}