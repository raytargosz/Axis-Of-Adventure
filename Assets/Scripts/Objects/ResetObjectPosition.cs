using System.Collections;
using UnityEngine;

public class ResetObjectPosition : MonoBehaviour
{
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Rigidbody objectRigidbody;

    void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        objectRigidbody = GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ResetTrigger"))
        {
            StartCoroutine(ResetObject());
        }
    }

    IEnumerator ResetObject()
    {
        // Wait for a short duration before resetting the object
        yield return new WaitForSeconds(1f);

        // Reset the object's position, rotation, and velocity
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        objectRigidbody.velocity = Vector3.zero;
        objectRigidbody.angularVelocity = Vector3.zero;
    }
}
