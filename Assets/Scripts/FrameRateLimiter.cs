using UnityEngine;

public class FrameRateLimiter : MonoBehaviour
{
    public int maxFrameRate = 120;

    void Start()
    {
        Application.targetFrameRate = maxFrameRate;
    }
}