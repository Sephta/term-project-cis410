using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordLogic : MonoBehaviour
{
    public Animator animator;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy" && animator.GetBool("HasAttacked")) {
            EnemyMovement em = other.gameObject.GetComponent<EnemyMovement>();
            em.health -= 25;
        }
    }

    void OnTriggerExit(Collider other) {}
}
