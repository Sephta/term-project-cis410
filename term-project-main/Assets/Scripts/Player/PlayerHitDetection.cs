using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerHitDetection : MonoBehaviour
{
    private PlayerMovement pm;
    private PlayerController pc;

    private float knockback = 2f;

    //[SerializeField] private float damageModifier = 1f;
    //[SerializeField] private float baseDamage = 25f;

    void Start()
    {
        if (gameObject.GetComponent<PlayerMovement>() != null)
            pm = gameObject.GetComponent<PlayerMovement>();

        if (gameObject.GetComponent<PlayerController>() != null)
            pc = gameObject.GetComponent<PlayerController>();
    }

    Rigidbody enemyRB;
    NavMeshAgent enemyAgent;
    EnemyMovement em;
    bool startTimer = false;
    float timer = 0.0f;
    void FixedUpdate()
    {
        if (startTimer)
            timer += Time.deltaTime;

        if (timer >= 0.5f)
        {
            startTimer = false;
            timer = 0f;
            if (enemyAgent != null)
                enemyAgent.enabled = true;
            if (enemyRB != null)
                enemyRB.isKinematic = true;
            if (em != null)
                em.isHit = false;
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            em = other.gameObject.GetComponent<EnemyMovement>();
            em.isHit = true;
            pc.cameraAnimator.SetTrigger("CamShake");
            Debug.Log("I hit: " + other.gameObject.name);
            enemyRB = other.gameObject.GetComponent<Rigidbody>();
            enemyAgent = other.gameObject.GetComponent<NavMeshAgent>();
            if (enemyRB != null && enemyAgent != null)
            {
                enemyAgent.enabled = false;
                enemyRB.isKinematic = false;

                enemyRB.AddForce(pm.directionVector * knockback, ForceMode.VelocityChange);
                enemyRB.AddForce(Vector3.up * knockback, ForceMode.VelocityChange);
                startTimer = true;
            }
            em.TakeDamage(pc.baseDamage * pc.damageModifier);
        }
    }

    void OnTriggerExit(Collider other) {}
}
