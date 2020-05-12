using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;


public class EnemyMovement : MonoBehaviour
{
    [Header("Stats")]
    public Bartender enemyHealth;
    [Range(0, 100)] public float health = 100;

    [Header("Nav Agent Vars")]
    [Range(2, 5)] public float agentSpeed;
    public float patrolTime;
    public float aggroRange;

    [Header("Waypoints")]
    public Transform[] waypoints;

    NavMeshAgent agent;
    Transform player;
    Animator animator;

    int currWaypoint = 0;
    int prevWaypoint = 0;

    void Awake()
    {
        if (gameObject.GetComponent<NavMeshAgent>() != null)
            agent = gameObject.GetComponent<NavMeshAgent>();

        if (gameObject.GetComponent<Animator>() != null)
            animator = gameObject.GetComponent<Animator>();

        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Random starting waypoint
        currWaypoint = Random.Range(0, waypoints.Length);
    }

    /* ---------------------------------------------------------------- */
    /*                              Updates                             */
    /* ---------------------------------------------------------------- */

    void FixedUpdate()
    {
        if (waypoints.Length > 0)
        {
            InvokeRepeating("Patrol", 0, patrolTime);
        }

        Tick();

        if (health <= 0) {
            Destroy(gameObject);
        }
    }

    /* ---------------------------------------------------------------- */
    /*                           Helper Methods                         */
    /* ---------------------------------------------------------------- */
    public void TakeDamage(float amount)
    {
        health -= amount;
        enemyHealth.setValue(health);
    }

    void Tick()
    {
        // move to next waypoint position
        agent.destination = waypoints[currWaypoint].position;

        // set non-aggro speed to "walking pace"
        // NOTE: could set patSpeed, chaseSpeed, etc as variables and swap between them based on state
        agent.speed = agentSpeed;

        // if player in aggro range...
        if (player != null && Vector3.Distance(transform.position, player.position) < aggroRange)
        {
            // move to player position at full speed
            agent.destination = player.position;
            agent.speed = agentSpeed;
        }
    }

    void Patrol()
    {
        // if currWaypoint is at last waypoint, go back to waypoints[0], else iterate currWaypoint
        currWaypoint = (currWaypoint == (waypoints.Length - 1)) ? 0 : currWaypoint++;
    }
}

