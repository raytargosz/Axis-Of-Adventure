using System.Collections;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public float moveDistance = 5f;
    public float moveSpeed = 1f;
    private Vector3 initialPosition;
    private Vector3 targetPosition;
    private bool isUnlocked;

    private void Start()
    {
        initialPosition = transform.position;
        targetPosition = initialPosition + (Vector3.up * moveDistance);
    }

    private void Update()
    {
        if (isUnlocked)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, initialPosition, moveSpeed * Time.deltaTime);
        }
    }

    public void Unlock()
    {
        isUnlocked = true;
    }

    public void Lock()
    {
        isUnlocked = false;
    }
}
