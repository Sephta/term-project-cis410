using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Public Vars
    public Animator animator;

    /* Player Camera Vars
     * Camera Game Object
     * pos
     * angle
     * follow speed
    */
    public Camera playerCamera;
    public Vector3 cameraPosition;
    public Vector3 cameraAngle;
    public float camFollowSpeed;
    /* ------------------- */

    public enum PlayerState { idle, walking, running, attacking };
    public PlayerState currentState;

    public bool grounded;

    // Private Vars
    private PlayerInput pi;
    private PlayerMovement pm;


    /* ---------------------------------------------------------------- */
    /*                              Updates                             */
    /* ---------------------------------------------------------------- */

    void Start() 
    {
        // Get Player Input Script
        if (gameObject.GetComponent<PlayerInput>() != null)
            pi = gameObject.GetComponent<PlayerInput>();

        // Get Player Movement Script
        if (gameObject.GetComponent<PlayerMovement>() != null)
            pm = gameObject.GetComponent<PlayerMovement>();

        currentState = PlayerState.idle;

        playerCamera.transform.eulerAngles = cameraAngle;
    }

    void Update()
    {
        UpdateMovementState();
    }

    void FixedUpdate()
    {
        // Update Player State
        switch(currentState)
        {
            case PlayerState.idle:
                animator.SetBool("IsIdle", true);
                animator.SetBool("IsWalking", false);
                animator.SetFloat("AnimationSpeed", 2f);
                break;

            case PlayerState.walking:
                animator.SetBool("IsWalking", true);
                animator.SetBool("IsIdle", false);
                animator.SetFloat("AnimationSpeed", pm.currentSpeed - 1f);
                pm.MovePlayer(pi.runKey);
                break;

            case PlayerState.running:
                animator.SetBool("IsWalking", true);
                animator.SetBool("IsIdle", false);
                animator.SetFloat("AnimationSpeed", pm.currentSpeed - 1f);
                pm.MovePlayer(pi.runKey);
                break;

            case PlayerState.attacking:
                animator.SetBool("IsIdle", false);
                break;
        }
            
        animator.SetBool("IsGrounded", GroundCheck());

        UpdatePlayerCamera();
        // GroundCheck();
    }


    /* ---------------------------------------------------------------- */
    /*                           Helper Methods                         */
    /* ---------------------------------------------------------------- */

    void ChangeState(PlayerState newState)
    {
        if (currentState == newState)
            return;

        currentState = newState;
    }

    public void UpdatePlayerState(PlayerState state)
    {

        ChangeState(state);
    }

    void UpdateMovementState()
    {
        if (pi.InputAxis.magnitude > 0 && !pm.isJumping) {
            ChangeState((pi.runKey) ? PlayerState.running : PlayerState.walking);
        }

        if (pi.InputAxis.magnitude == 0 && currentState != PlayerState.attacking) {
            ChangeState(PlayerState.idle);
        }
    }

    RaycastHit hitData;
    Ray ray;
    public bool GroundCheck()
    {
        ray = new Ray(transform.position, Vector3.down);
        grounded = Physics.Raycast(ray, out hitData, 0.55f);
        // Debug.DrawRay(transform.position, Vector3.down * hitData.distance, Color.red, 5f);

        return grounded;
    }

    void UpdatePlayerCamera()
    {
        playerCamera.transform.position = Vector3.Lerp(playerCamera.transform.position, transform.position + cameraPosition, camFollowSpeed * Time.deltaTime);
    }
}
