using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class DisplayControlsUI : MonoBehaviour
{
    private TextMeshProUGUI controlsText;

    void Start()
    {
        controlsText = GetComponent<TextMeshProUGUI>();
    }

    public void SetText(string text)
    {
        controlsText.text = text;
    }

    public void ClearText()
    {
        controlsText.text = "";
    }
}