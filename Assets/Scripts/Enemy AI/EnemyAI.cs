using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform[] waypoints;
    public float moveSpeed = 2f;
    public float rotationSpeed = 5f;
    public float chaseRange = 5f;
    public float attackRange = 1f;
    public LayerMask playerLayer;

    private int currentWaypoint = 0;
    private Transform target;
    private Rigidbody rb;
    private Vector3 nextWaypoint;
    private Animator animator; // If your enemy has animations

    private enum State { Patrolling, Chasing, Attacking }
    private State currentState;

    void Start()
    {
        currentState = State.Patrolling;
        rb = GetComponent<Rigidbody>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>(); // If your enemy has animations
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, target.position);

        if (currentState != State.Attacking)
        {
            if (distanceToPlayer < attackRange)
            {
                currentState = State.Chasing;
            }
            else if (distanceToPlayer < chaseRange)
            {
                currentState = State.Chasing;
            }
            else
            {
                currentState = State.Patrolling;
            }
        }

        switch (currentState)
        {
            case State.Patrolling:
                Patrol();
                break;
            case State.Chasing:
                Chase();
                break;
            case State.Attacking:
                Attack();
                break;
        }
    }

    void Patrol()
    {
        if (waypoints.Length == 0) return;

        // If your enemy has animations
        animator.SetBool("isChasing", false);
        animator.SetBool("isAttacking", false);

        nextWaypoint = waypoints[currentWaypoint].position;
        float distanceToWaypoint = Vector3.Distance(transform.position, nextWaypoint);

        if (distanceToWaypoint < 0.1f)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
        }

        MoveTowards(nextWaypoint);
    }

    void Chase()
    {
        // If your enemy has animations
        animator.SetBool("isChasing", true);

        if (Vector3.Distance(transform.position, target.position) > attackRange)
        {
            MoveTowards(target.position);
        }
        else
        {
            currentState = State.Attacking;
        }
    }

    void Attack()
    {
        // Implement your attack logic here, for example:
        // 1. Play attack animation.
        // 2. Deal damage to the player if they're in range.
        // 3. Transition back to chasing state when the attack is finished.

        // If your enemy has animations
        animator.SetBool("isAttacking", true);
    }

    void MoveTowards(Vector3 destination)
    {
        Vector3 direction = (destination - transform.position).normalized;
        rb.MovePosition(transform.position + direction * moveSpeed * Time.deltaTime);
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
}
