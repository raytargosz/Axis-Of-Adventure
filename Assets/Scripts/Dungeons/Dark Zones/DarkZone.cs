using UnityEngine;
using TMPro;

public class DarkZone : MonoBehaviour
{
    public Inventory inventory;
    public string requiredItemName;
    public TextMeshProUGUI darkZonePrompt;

    private bool hasRequiredItem => inventory.HasItem(requiredItemName);

    private void Start()
    {
        SetDarkZonePromptAlpha(0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!hasRequiredItem)
            {
                SetDarkZonePromptAlpha(1f);
            }
            else
            {
                DeactivateDarkZoneCube();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SetDarkZonePromptAlpha(0f);
        }
    }

    private void DeactivateDarkZoneCube()
    {
        Transform darkCube = transform.Find("DarkCube");
        if (darkCube != null)
        {
            darkCube.gameObject.SetActive(false);
        }
    }

    private void SetDarkZonePromptAlpha(float alpha)
    {
        darkZonePrompt.color = new Color(darkZonePrompt.color.r, darkZonePrompt.color.g, darkZonePrompt.color.b, alpha);
    }
}
