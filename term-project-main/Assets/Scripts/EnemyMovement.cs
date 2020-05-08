using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;


public class EnemyMovement : MonoBehaviour
{
    public float patrolTime = 8f;
    public float aggroRange = 5f;
    public Transform[] waypoints;

    public int health = 100;

    int index;
    float speed, agentSpeed;
    Transform player;

    // Animator anim;
    NavMeshAgent agent;

    void Awake()
    {
        // anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        if (agent != null) { agentSpeed = agent.speed; }

        player = GameObject.FindGameObjectWithTag("Player").transform;
        index = Random.Range(0, waypoints.Length);

        // InvokeRepeating("Tick", 0, 0.5f);

        if (waypoints.Length > 0)
        {
            InvokeRepeating("Patrol", 0, patrolTime);
        }
    }

    void Patrol()
    {
        // if index is at last waypoint, go back to waypoints[0], else iterate index
        index = index == (waypoints.Length - 1) ? 0 : index + 1;
    }

    void Tick()
    {
        // move to next waypoint position
        agent.destination = waypoints[index].position;

        // set non-aggro speed to "walking pace"
        // NOTE: could set patSpeed, chaseSpeed, etc as variables and swap between them based on state
        agent.speed = agentSpeed / 2;

        // if player in aggro range...
        if (player != null && Vector3.Distance(transform.position, player.position) < aggroRange)
        {
            // move to player position at full speed
            agent.destination = player.position;
            agent.speed = agentSpeed;
        }
    }

    void FixedUpdate()
    {
        Tick();

        if (health <= 0) {
            Destroy(gameObject);
        }
    }
}

