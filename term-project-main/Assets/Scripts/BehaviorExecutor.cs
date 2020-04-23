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

    [SerializeField]
    bool jumpPeakReached = false;

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
            case BehaviorMachine.PlayerBehavior.jumping:
                ExecuteJump(behaviorMachine.canJump);
                break;
        }
    }


    /* ---------------------------------------------------------------- */
    /*                         Executor Methods                         */
    /* ---------------------------------------------------------------- */

    public void ExecuteJump(bool jumpCheck)
    {
        // The bellow equations were used to derive the velocity equation used to calculate max velocity before impact
        // this means that the value of jumpVelocity should be the velocity needed to obtain a jump of height -> jumpHeight
        // float PE = gravity * rb.mass * (float)height;  // This is potential energy (PE = mgh)
        // float KE = (((1/2)*rb.mass)*(v*v));            // This is Kinetic Energy (KE = ((1/2)m)*(V)^2)
        
        jumpVelocity = Mathf.Sqrt(2 * gravity * jumpHeight);  // in physics this represents velocity just before impact
        // rb.AddForce(transform.up * jumpVelocity, ForceMode.VelocityChange);
        if (transform.position.y < transform.position.y + jumpHeight) {
            if (behaviorMachine.input.runKey)
                transform.position += new Vector3(
                    directionVector.x * runSpeed * Time.deltaTime,
                    jumpVelocity * Time.deltaTime,
                    directionVector.z * runSpeed * Time.deltaTime);
            else
                transform.position += new Vector3(
                    directionVector.x * walkSpeed * Time.deltaTime,
                    jumpVelocity * Time.deltaTime,
                    directionVector.z * walkSpeed * Time.deltaTime);
        } else {
            jumpPeakReached = true;
        }
        
        if (behaviorMachine.CheckGround()) {
            // behaviorMachine.DetermineNewBehavior();
            behaviorMachine.currentBehavior = BehaviorMachine.PlayerBehavior.idle;
        }
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
