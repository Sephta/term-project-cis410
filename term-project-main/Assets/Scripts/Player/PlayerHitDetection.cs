using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitDetection : MonoBehaviour
{
    private PlayerMovement pm;
    private PlayerController pc;

    private float knockback = 2f;

    void Start()
    {
        if (gameObject.GetComponent<PlayerMovement>() != null)
            pm = gameObject.GetComponent<PlayerMovement>();

        if (gameObject.GetComponent<PlayerController>() != null)
            pc = gameObject.GetComponent<PlayerController>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            pc.cameraAnimator.SetTrigger("CamShake");
            Debug.Log("I hit: " + other.gameObject.name);
            Rigidbody enemyRB = other.gameObject.GetComponent<Rigidbody>();
            if (enemyRB != null)
            {
                enemyRB.AddForce(pm.directionVector * knockback, ForceMode.VelocityChange);
            }

            EnemyMovement em = other.gameObject.GetComponent<EnemyMovement>();
            em.TakeDamage(25);
        }
    }

    void OnTriggerExit(Collider other) {}
}
