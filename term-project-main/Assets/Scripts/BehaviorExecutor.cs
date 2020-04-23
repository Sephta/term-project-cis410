using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/* The executor class for executing defined behaviors given user input.
 * Will simply contain a list of functions to be executed at runtime.
*/
public class BehaviorExecutor : MonoBehaviour
{
    public BehaviorMachine behaviorMachine;
    public Rigidbody rb;
    // public GameObject player;


    /* ---------------------------------------------------------------- */
    /*                         Any Related Vars                         */
    /* ---------------------------------------------------------------- */

    public float currentSpeed = 0f;
    public float walkSpeed = 0f;
    public float runSpeed = 0f;
    public float jumpHeight = 0f;
    public float jumpVelocity = 0f;
    public float gravity = 9.8f;

    public Vector3 directionVector = Vector3.zero;


    /* ---------------------------------------------------------------- */
    /*                              Updates                             */
    /* ---------------------------------------------------------------- */

    void Update()
    {
        directionVector = new Vector3(behaviorMachine.input.InputAxis.x, 0f, behaviorMachine.input.InputAxis.y);
    }

    void FixedUpdate()
    {
        if (behaviorMachine.canJump) {
            ExecuteJump();
        }

        switch(behaviorMachine.currentBehavior)
        {
            case BehaviorMachine.PlayerBehavior.idle:
                break;
            
            case BehaviorMachine.PlayerBehavior.walking:
                ExecuteMove(behaviorMachine.input.runKey);
                break;

            case BehaviorMachine.PlayerBehavior.running:
                ExecuteMove(behaviorMachine.input.runKey);
                break;
        }
    }


    /* ---------------------------------------------------------------- */
    /*                         Executor Methods                         */
    /* ---------------------------------------------------------------- */

    public void ExecuteJump()
    {
        // The bellow equations were used to derive the velocity value 'v'
        // float PE = gravity * rb.mass * (float)height;  // This is potential energy (PE = mgh)
        // float KE = (((1/2)*rb.mass)*(v*v)); // This is Kinetic Energy (KE = ((1/2)m)*(V)^2)

        // height = 10.0f;                        // Height of the jump
        jumpVelocity = Mathf.Sqrt(2 * gravity * jumpHeight);  // in physics this represents velecotity just before impact

        rb.AddForce(transform.up * jumpVelocity, ForceMode.VelocityChange);
        // rb.AddForce(directionVector * 2.0f, ForceMode.VelocityChange);
    }

    public void ExecuteMove(bool moveFlag)
    {
        // if player is walking
        if (!moveFlag) {
            transform.position += directionVector * walkSpeed * Time.deltaTime;
        } else {
            transform.position += directionVector * runSpeed * Time.deltaTime;
        }
    }
}
