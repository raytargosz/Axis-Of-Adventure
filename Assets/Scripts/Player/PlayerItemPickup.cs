using UnityEngine;
using TMPro;

public class PlayerItemPickup : MonoBehaviour
{
    public GameObject interactionPrompt;
    public TextMeshProUGUI interactionText;
    public AudioSource audioSource;
    public AudioClip pickupSFX;
    public AudioClip dropSFX;
    public float interactionDistance = 2.0f;
    public Transform holdPosition;

    private GameObject itemInRange;
    private bool holdingItem = false;

    void Update()
    {
        if (holdingItem)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                DropItem();
            }
        }
        else if (itemInRange != null)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                PickUpItem();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PickupItem"))
        {
            itemInRange = other.gameObject;
            interactionText.text = "Press E to pick up";
            interactionPrompt.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PickupItem"))
        {
            itemInRange = null;
            interactionPrompt.SetActive(false);
        }
    }

    void PickUpItem()
    {
        holdingItem = true;
        itemInRange.transform.SetParent(holdPosition);
        itemInRange.transform.localPosition = Vector3.zero;
        itemInRange.GetComponent<Rigidbody>().isKinematic = true;
        interactionText.text = "Press E to drop";
        audioSource.PlayOneShot(pickupSFX);
    }

    void DropItem()
    {
        holdingItem = false;
        itemInRange.transform.SetParent(null);
        itemInRange.GetComponent<Rigidbody>().isKinematic = false;
        interactionPrompt.SetActive(false);
        audioSource.PlayOneShot(dropSFX);
        itemInRange = null;
    }
}
