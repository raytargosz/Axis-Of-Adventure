using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class WonderingNPC : MonoBehaviour
{
    [Header("Wandering Settings")]
    [Tooltip("The radius within which the NPC will wander")]
    public float wanderRadius = 10f;

    [Tooltip("Time spent wandering before stopping for a moment")]
    public float wanderDuration = 5f;

    [Tooltip("Time spent idling between wanderings")]
    public float idleDuration = 3f;

    [Tooltip("Wandering speed of the NPC")]
    public float walkSpeed = 2f;

    [Header("Chase Settings")]
    [Tooltip("Field of view angle of the NPC's sight")]
    public float fieldOfViewAngle = 60f;

    [Tooltip("Sight distance of the NPC")]
    public float sightDistance = 10f;

    [Tooltip("The target player object")]
    public Transform player;

    [Tooltip("Chasing speed of the NPC")]
    public float runSpeed = 5f;

    private NavMeshAgent agent;
    private Animator animator;
    private Vector3 initialPosition;
    private bool isChasing = false;
    private bool isIdle = false;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        initialPosition = transform.position;
        StartCoroutine(Wander());
    }

    private void Update()
    {
        if (IsPlayerInSight() && !isChasing)
        {
            StopAllCoroutines();
            isIdle = false;
            isChasing = true;
            agent.speed = runSpeed;
            animator.SetFloat("Speed", runSpeed);
        }

        if (isChasing)
        {
            agent.SetDestination(player.position);

            if (!IsPlayerInSight())
            {
                isChasing = false;
                agent.speed = walkSpeed;
                StartCoroutine(ReturnToWanderArea());
            }
        }
    }

    private bool IsPlayerInSight()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        if (angle < fieldOfViewAngle * 0.5f && directionToPlayer.magnitude < sightDistance)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position + transform.up, directionToPlayer, out hit, sightDistance))
            {
                if (hit.collider.transform == player)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private IEnumerator Wander()
    {
        while (true)
        {
            if (!isIdle)
            {
                Vector3 newPosition = RandomNavSphere(initialPosition, wanderRadius);
                agent.SetDestination(newPosition);
                agent.speed = walkSpeed;
                animator.SetFloat("Speed", walkSpeed);
                yield return new WaitForSeconds(wanderDuration);

                isIdle = true;
                animator.SetFloat("Speed", 0);
                yield return new WaitForSeconds(idleDuration);
            }
            else
            {
                isIdle = false;
            }
        }
    }

    private IEnumerator ReturnToWanderArea()
    {
        agent.SetDestination(initialPosition);

        while (Vector3.Distance(transform.position, initialPosition) > agent.stoppingDistance)
        {
            yield return null;
        }

        StartCoroutine(Wander());
    }

    private Vector3 RandomNavSphere(Vector3 origin, float distance)
    {
        Vector3 randomDirection = Random.insideUnitSphere * distance;
        randomDirection += origin;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, distance, NavMesh.AllAreas);

        return navHit.position;
    }
}