using UnityEngine;

public class DeathTriggerPosition : MonoBehaviour
{
    private BoxCollider boxCollider;
    private Vector3 center = new Vector3(-762f, -422f, 1885f);
    private Vector3 size = new Vector3(427f, 10f, 300f);

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        boxCollider.center = center;
        boxCollider.size = size;
    }
}
