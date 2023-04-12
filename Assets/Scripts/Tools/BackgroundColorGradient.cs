using UnityEngine;

public class BackgroundColorGradient : MonoBehaviour
{
    // Public gradient variable for defining the color gradient
    public Gradient gradient;

    // Update is called once per frame
    void Update()
    {
        // Set the camera background color to the color at the top of the gradient
        Camera.main.backgroundColor = gradient.Evaluate(1f);
    }
}
