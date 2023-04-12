using UnityEngine;
using TMPro;

public class GemCounter : MonoBehaviour
{
    // Reference to the UI Text component displaying the gem counter.
    public TextMeshProUGUI gemCounterText;

    private int gemCounter = 0;

    // Call this method to update the gem counter and its UI text.
    public void UpdateGemCounter(int value)
    {
        gemCounter += value;
        gemCounterText.text = gemCounter.ToString();
    }
}
