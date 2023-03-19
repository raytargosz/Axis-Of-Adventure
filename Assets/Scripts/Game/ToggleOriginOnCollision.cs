using UnityEngine;

public class ToggleOriginOnCollision : MonoBehaviour
{
    [SerializeField] private Transform targetObject;
    [SerializeField] private Vector3 newOrigin;
    [SerializeField] private string playerTag = "Player";

    private Vector3 originalOrigin;
    private bool isToggled;

    private void Start()
    {
        originalOrigin = targetObject.position;
        isToggled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            if (!isToggled)
            {
                targetObject.position = newOrigin;
                isToggled = true;
            }
            else
            {
                targetObject.position = originalOrigin;
                isToggled = false;
            }
        }
    }
}
