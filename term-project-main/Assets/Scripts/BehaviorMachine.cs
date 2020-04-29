using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/* This is the brain of the player movement controller system
 * This script is the middle man between grabbing input from the user and
 * executing the desired behavior in game.
*/
public class BehaviorMachine : MonoBehaviour
{
    // Refferences to necessary scripts
    public PlayerInput input;
    public BehaviorExecutor executor;

    // Defining the different kinds of behaviors the player can perform
    public enum PlayerBehavior {
        idle,
        walking,
        running,
        jumping,
        attack
    }

    public PlayerBehavior currentBehavior;

    public bool grounded;

    // Jumping logic vars
    public bool canJump = false;


    /* ---------------------------------------------------------------- */
    /*                             Updates()                            */
    /* ---------------------------------------------------------------- */

    // ? Only initialize vars that will not be set in the editor
    void Start() {}

    // Update currentBehavior Status, and check for any new Behaviors
    void Update()
    {
        UpdateMovementBehavior();

        // UpdateAttackBehavior()
        if (input.attackKey && CheckGround()) {
            ChangeBehavior(PlayerBehavior.attack);
            executor.animator.SetBool("HasAttacked", true);
        }
    }

    void UpdateMovementBehavior()
    {
        if (input.InputAxis.magnitude > 0 && currentBehavior != PlayerBehavior.jumping && currentBehavior != PlayerBehavior.attack) {
            /* Terniary operation 
            * ChangeBehavior takes a PlayerBehavior as an arguement
            * If the run key (left shift) if being pressed then the currentBehavior gets
            * changed to the running behavior, else the walking behavior */
            ChangeBehavior((input.runKey) ? PlayerBehavior.running : PlayerBehavior.walking);
        }
        if (input.InputAxis.magnitude == 0 && currentBehavior != PlayerBehavior.jumping && currentBehavior != PlayerBehavior.attack) {
            ChangeBehavior(PlayerBehavior.idle);
        }
        // if space bar is pressed and player is grounded
        if (input.jumpKey && CheckGround()) {
            ChangeBehavior(PlayerBehavior.jumping);
            executor.animator.SetBool("HasJumped", true);
            canJump = true;
        } else {
            canJump = false;
        }
    }


    /* ---------------------------------------------------------------- */
    /*                           Helper Methods                         */
    /* ---------------------------------------------------------------- */

    public void ChangeBehavior(PlayerBehavior behavior)
    {
        if (currentBehavior == behavior)
            return;

        currentBehavior = behavior;
    }

    public bool CheckGround()
    {
        Ray ray = new Ray(transform.position + new Vector3(0f, 0.5f, 0f), transform.TransformDirection(Vector3.down));
        RaycastHit hitData;
        if (Physics.Raycast(ray, out hitData, 0.55f)) {
            // Debug.DrawRay(transform.position + new Vector3(0f, 0.5f, 0f), transform.TransformDirection(Vector3.down) * hitData.distance, Color.red, 5f);
            // Debug.Log("hitData: " + hitData.distance + " was Hit? - " + hit);
            grounded = true;
        } else {
            grounded = false;
        }

        // Capsule Cast code not working yet
        // CapsuleCollider cap = gameObject.GetComponent<CapsuleCollider>();

        // float distanceToPoints = cap.height / 2f - cap.radius;

        // Vector3 p1 = transform.position + cap.center + Vector3.up * distanceToPoints;
        // Vector3 p2 = transform.position + cap.center - Vector3.up * distanceToPoints;

        // if (Physics.CapsuleCast(p1, p2, 0.3f, Vector3.down, 0.1f)) {
        //     grounded = true;
        // } else {
        //     grounded = false;
        // }

        return grounded;
    }
}
