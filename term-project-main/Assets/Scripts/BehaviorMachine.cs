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
        running
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
        CheckGround();
    }

    void UpdateMovementBehavior()
    {
        if (input.InputAxis.magnitude > 0) {
            /* Terniary operation 
            * ChangeBehavior takes a PlayerBehavior as an arguement
            * If the run key (left shift) if being pressed then the currentBehavior gets
            * changed to the running behavior, else the walking behavior */
            ChangeBehavior((input.runKey) ? PlayerBehavior.running : PlayerBehavior.walking);
        }
        if (input.InputAxis.magnitude == 0) {
            ChangeBehavior(PlayerBehavior.idle);
        }
        // if space bar is pressed and player is grounded
        if (input.jumpKey && CheckGround()) {
            canJump = true;
        } else {
            canJump = false;
        }
    }


    /* ---------------------------------------------------------------- */
    /*                           Helper Methods                         */
    /* ---------------------------------------------------------------- */

    void ChangeBehavior(PlayerBehavior behavior)
    {
        if (currentBehavior == behavior)
            return;

        currentBehavior = behavior;
    }

    public bool CheckGround()
    {
        Ray ray = new Ray(transform.position, transform.TransformDirection(Vector3.down));
        RaycastHit hitData;
        // Physics.Raycast(ray, out hitData, 10);
        if (Physics.Raycast(ray, out hitData, 0.5f)) {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * hitData.distance, Color.red);
            // Debug.Log("distance: " + hitData.distance);
            grounded = true;
        } else {
            grounded = false;
        }

        return grounded;
    }
}
