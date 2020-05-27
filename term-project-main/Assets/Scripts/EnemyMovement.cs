using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;


public class EnemyMovement : MonoBehaviour
{
    [Header("Model Animator")]
    public Animator animator;

    [Header("Stats")]
    /* [ReadOnly] */ public bool isHit = false;
    public Bartender enemyHealth;
    [Range(0, 100)] public float health = 100;

    [Header("Nav Agent Vars")]
    [Range(2, 5)] public float agentSpeed;
    public float patrolTime;
    public float aggroRange;
    public float attackRange;

    [Header("Waypoints")]
    public Transform[] waypoints;

    NavMeshAgent agent;
    Transform player;

    [Header("Debug")]

    /* [ReadOnly] */ public int currWaypoint = 0;
    /* [ReadOnly] */ public int prevWaypoint = 0;

    /* [ReadOnly] */ public bool waypointReached = false;
    /* [ReadOnly] */ public float idleTime = 0.0f;
    /* [ReadOnly] */ public bool startIdleTimer = false;

    void Awake()
    {
        if (gameObject.GetComponent<NavMeshAgent>() != null)
        {
            agent = gameObject.GetComponent<NavMeshAgent>();
            agent.destination = waypoints[0].position;
            // agent.speed = agentSpeed;
        }

        player = GameObject.FindGameObjectWithTag("Player").transform;

        // * Random starting waypoint
        // currWaypoint = Random.Range(0, waypoints.Length);
        
        animator.SetBool("IsIdle", false);
        animator.SetBool("IsWalking", true);

        prevWaypoint = currWaypoint;
    }

    /* ---------------------------------------------------------------- */
    /*                              Updates                             */
    /* ---------------------------------------------------------------- */

    void Update()
    {
        if (health <= 0)
        {
            PlayerController pc = player.gameObject.GetComponent<PlayerController>();
            if (pc != null)
                pc.wallet += 50;
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {

        if (!isHit)
            UpdateEnemyMovement();
    }

    /* ---------------------------------------------------------------- */
    /*                           Helper Methods                         */
    /* ---------------------------------------------------------------- */

    void UpdateEnemyMovement()
    {
        // Patrol behavior
        if (Vector3.Distance(agent.transform.position, waypoints[currWaypoint].position) < 0.5f)
        {
            animator.SetBool("IsIdle", false);
            animator.SetBool("IsWalking", true);
            if (currWaypoint == waypoints.Length - 1)
                currWaypoint = 0;
            else
                currWaypoint++;
            agent.destination = waypoints[currWaypoint].position;
        }

        // if player in aggro range...
        if (WithinEyeRange())
        {
            waypointReached = false;
            animator.SetBool("IsIdle", false);
            animator.SetBool("IsWalking", true);

            if (WithinAttackRange())
            {
                animator.SetBool("IsAttacking", true);
            }
            else
            {
                animator.SetBool("IsAttacking", false);
            }


            agent.destination = player.position;
        }
    }

    bool WithinEyeRange()
    {
        if (player != null && Vector3.Distance(transform.position, player.position) < aggroRange)
        {
            return true;
        }

        return false;
    }

    bool WithinAttackRange()
    {
        if (player != null && Vector3.Distance(transform.position, player.position) < attackRange)
            return true;
        else
            return false;
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        enemyHealth.setValue(health);
    }
}

