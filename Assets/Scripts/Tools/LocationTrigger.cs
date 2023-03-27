using UnityEngine;

public class LocationTrigger : MonoBehaviour
{
    public string locationName;
    private LocationTextController locationTextController;

    void Start()
    {
        locationTextController = FindObjectOfType<LocationTextController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            locationTextController.TriggerLocationText(locationName);
            Destroy(gameObject);
        }
    }
}