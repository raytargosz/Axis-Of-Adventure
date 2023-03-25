using System.Collections.Generic;
using UnityEngine;

public class ColliderManager : MonoBehaviour
{
    public GameObject player;
    public float colliderActivationDistance = 10f;
    public List<Collider> alwaysEnabledColliders;

    private Collider[] colliders;

    void Start()
    {
        colliders = FindObjectsOfType<Collider>();
    }

    void Update()
    {
        foreach (Collider collider in colliders)
        {
            if (!alwaysEnabledColliders.Contains(collider))
            {
                float distanceToPlayer = Vector3.Distance(player.transform.position, collider.transform.position);

                if (distanceToPlayer < colliderActivationDistance)
                {
                    collider.enabled = true;
                }
                else
                {
                    collider.enabled = false;
                }
            }
        }
    }
}