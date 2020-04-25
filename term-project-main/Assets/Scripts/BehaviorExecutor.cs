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
    public Animator animator;


    /* ---------------------------------------------------------------- */
    /*                         Any Related Vars                         */
    /* ---------------------------------------------------------------- */

    public float walkSpeed = 0f;
    public float runSpeed = 0f;
    public float jumpHeight = 0f;
    public float gravity = 9.8f;
    public float rotationSpeed = 0f;

    [SerializeField] Vector3 directionVector = Vector3.zero;
    [SerializeField] float currentSpeed = 0f;
    [SerializeField] float jumpVelocity = 0f;
    
    // bellow vars are used to lerp between walk and run speeds
    float tw = 0f;
    float tr = 0f;
    float t_acc = 0.02f;

    // used to rotate the player model towards where he is moving
    Quaternion modelRotation = Quaternion.identity;

    /* ---------------------------------------------------------------- */
    /*                              Updates                             */
    /* ---------------------------------------------------------------- */

    void Start()
    {
        currentSpeed = walkSpeed;
    }

    void Update()
    {
        directionVector = new Vector3(behaviorMachine.input.InputAxis.x, 0f, behaviorMachine.input.InputAxis.y);
    }

    void FixedUpdate()
    {

        switch(behaviorMachine.currentBehavior)
        {
            case BehaviorMachine.PlayerBehavior.idle:
                animator.SetBool("IsWalking", false);
                break;
            
            case BehaviorMachine.PlayerBehavior.walking:
                animator.SetBool("IsWalking", true);
                animator.speed = 1f;
                ExecuteMove(behaviorMachine.input.runKey);
                break;

            case BehaviorMachine.PlayerBehavior.running:
                animator.SetBool("IsWalking", true);
                animator.speed = 2f;
                ExecuteMove(behaviorMachine.input.runKey);
                break;
            case BehaviorMachine.PlayerBehavior.jumping:
                ExecuteJump(behaviorMachine.canJump);
                break;
        }

        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, directionVector, rotationSpeed * Time.deltaTime, 0f);
        modelRotation = Quaternion.LookRotation(desiredForward);
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

        // If the player is running 
        if (behaviorMachine.input.runKey) {
            tw = 0f; // resets the lerp t value for walking

            // transition from run to walk by increments of tr
            currentSpeed = Mathf.Lerp(currentSpeed, runSpeed, tr);

            // update the transform accordingly
            transform.position += new Vector3(
                directionVector.x * currentSpeed * Time.deltaTime,
                jumpVelocity * Time.deltaTime,
                directionVector.z * currentSpeed * Time.deltaTime);

            transform.rotation = modelRotation;
            
            // bound the value of tr so it doesnt grow infinitly while moving
            if (tr < 1f)
                tr += t_acc;
            /* The reason its described as a t acceleration is because each
            time t is added to the currentSpeed variable it is growing 
            exponentially by a factor of t_acc */
        }
        // else player is walking
        else {
            tr = 0f; // resets lerp t value for running

            // transition from run to walk by increments of tw
            currentSpeed = Mathf.Lerp(currentSpeed, walkSpeed, tw);

            // update the transform accordingly
            transform.position += new Vector3(
                directionVector.x * currentSpeed * Time.deltaTime,
                jumpVelocity * Time.deltaTime,
                directionVector.z * currentSpeed * Time.deltaTime);

            transform.rotation = modelRotation;

            // bounds value so it doesnt keep growing infinitly
            if (tw < 1f)
                tw += t_acc;
        }
        
        if (behaviorMachine.CheckGround()) {
            behaviorMachine.currentBehavior = BehaviorMachine.PlayerBehavior.idle;
        }
    }

    public void ExecuteMove(bool moveFlag)
    {
        // if player is walking
        if (!moveFlag) {
            tr = 0f;

            currentSpeed = Mathf.Lerp(currentSpeed, walkSpeed, tw);

            transform.position += directionVector * currentSpeed * Time.deltaTime;
            transform.rotation = modelRotation;

            if (tw < 1f)
                tw += t_acc;
            
        }
        // else player is running
        else {
            tw = 0f;

            currentSpeed = Mathf.Lerp(currentSpeed, runSpeed, tr);
            
            transform.position += directionVector * currentSpeed * Time.deltaTime;
            transform.rotation = modelRotation;

            if (tr < 1f)
                tr += t_acc;
        }
    }
}
