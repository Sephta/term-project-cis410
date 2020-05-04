using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    // Public Vars
    public bool isJumping = false;
    public float currentSpeed = 0f;
    public float walkSpeed = 0f;
    public float runSpeed = 0f;
    public float jumpHeight = 0f;
    public float gravity = 9.8f;
    public float rotationSpeed = 0f;

    // Private Vars
    [SerializeField] Vector3 directionVector = Vector3.zero;
    float jumpVelocity = 0f;
    Vector3 desiredForward = Vector3.zero;

    private PlayerController pc;
    private PlayerInput pi;
    private Rigidbody rb;


    /* ---------------------------------------------------------------- */
    /*                              Updates                             */
    /* ---------------------------------------------------------------- */

    void Start()
    {
        if (gameObject.GetComponent<PlayerController>() != null)
            pc = gameObject.GetComponent<PlayerController>();

        if (gameObject.GetComponent<PlayerInput>() != null)
            pi = gameObject.GetComponent<PlayerInput>();
        
        if (gameObject.GetComponent<Rigidbody>() != null)
            rb = gameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        directionVector = new Vector3(pi.InputAxis.x, 0f, pi.InputAxis.y);
    }

    void FixedUpdate()
    {
        if (pc.GroundCheck()) {
            pc.animator.SetBool("IsFalling", false);
            pc.animator.SetBool("IsJumping", false);
            isJumping = false;
        }

        if (transform.position.y <= desiredHeight + 0.1f && transform.position.y >= desiredHeight - 0.1f) {
            isJumping = false;
            pc.animator.SetBool("IsJumping", isJumping);
            pc.animator.SetBool("IsFalling", true);
        }

        if (pc.GroundCheck() && pi.jumpKey && pc.currentState != PlayerController.PlayerState.attacking) {
            desiredHeight = transform.position.y + jumpHeight;
            isJumping = true;
            pc.animator.SetBool("IsJumping", isJumping);
            jumpVelocity = Mathf.Sqrt(2 * gravity * jumpHeight);
            rb.AddForce(Vector3.up * jumpVelocity, ForceMode.VelocityChange);
        }
        
        // * Testing different jump types
        // if (rb.velocity.y < 0) {
        //     Debug.Log("grav rb: " + Physics.gravity);
        //     Physics.gravity = new Vector3(0f, gravity * 2, 0f) * Vector3.down;
        // } else Physics.gravity = new Vector3(0f, gravity, 0f) * Vector3.down;

        // Update model facing direction
        desiredForward = Vector3.RotateTowards(transform.forward, directionVector, rotationSpeed * Time.deltaTime, 0f);
        modelRotation = Quaternion.LookRotation(desiredForward);
    }


    /* ---------------------------------------------------------------- */
    /*                           Helper Methods                         */
    /* ---------------------------------------------------------------- */

    // bellow vars are used to lerp between walk and run speeds
    float tw = 0f;
    float tr = 0f;
    float t_acc = 0.02f;

    // Jump related vars
    float desiredHeight = 0f;

    // rotation of player model
    Quaternion modelRotation = Quaternion.identity;

    public void PlayerMove(bool runFlag)
    {

        // if player is walking
        if (!runFlag) {
            tr = 0f;

            currentSpeed = Mathf.Lerp(currentSpeed, walkSpeed, tw);

            transform.position += directionVector * currentSpeed * Time.deltaTime;
            transform.rotation = modelRotation;
            // transform.eulerAngles = desiredForward;

            if (tw < 1f)
                tw += t_acc;
            
        }
        // else player is running
        else {
            tw = 0f;

            currentSpeed = Mathf.Lerp(currentSpeed, runSpeed, tr);
            
            transform.position += directionVector * currentSpeed * Time.deltaTime;
            transform.rotation = modelRotation;
            // transform.eulerAngles = desiredForward;

            if (tr < 1f)
                tr += t_acc;
        }
    }

    float currAtkTime = 0f;
    public void PlayerAttack()
    {
        currAtkTime += Time.deltaTime;
        AnimatorStateInfo currState = pc.animator.GetCurrentAnimatorStateInfo(0);
        if (currAtkTime >= currState.length) {
            currAtkTime = 0f;

            pc.UpdatePlayerState(pc.prevState);
        }
    }
}
