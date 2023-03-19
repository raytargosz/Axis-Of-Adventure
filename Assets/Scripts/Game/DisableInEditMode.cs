using UnityEngine;

public class DisableInEditMode : MonoBehaviour
{
    void Start()
    {
        gameObject.SetActive(Application.isPlaying);
    }
}
