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
    public PlayerState prevState;
    public PlayerState currentState;

    public bool grounded;

    // Private Vars
    private PlayerInput pi;
    private PlayerMovement pm;
    private Rigidbody rb;


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

        if (gameObject.GetComponent<Rigidbody>() != null)
            rb = gameObject.GetComponent<Rigidbody>();

        currentState = PlayerState.idle;
        prevState = PlayerState.idle;

        playerCamera.transform.eulerAngles = cameraAngle;
    }

    void Update()
    {
        UpdateMovementState();
        UpdateAttackState();
    }

    void FixedUpdate()
    {
        // Update Player State
        switch(currentState)
        {
            case PlayerState.idle:
                animator.SetBool("IsIdle", true);
                animator.SetBool("IsWalking", false);
                animator.SetBool("HasAttacked", false);
                animator.SetFloat("AnimationSpeed", 2f);
                break;

            case PlayerState.walking:
                animator.SetBool("IsWalking", true);
                animator.SetBool("IsIdle", false);
                animator.SetBool("HasAttacked", false);
                animator.SetFloat("AnimationSpeed", pm.currentSpeed - 0.5f);
                pm.PlayerMove(pi.runKey);
                break;

            case PlayerState.running:
                animator.SetBool("IsWalking", true);
                animator.SetBool("IsIdle", false);
                animator.SetBool("HasAttacked", false);
                animator.SetFloat("AnimationSpeed", pm.currentSpeed - 1f);
                pm.PlayerMove(pi.runKey);
                break;

            case PlayerState.attacking:
                animator.SetBool("IsIdle", false);
                animator.SetBool("IsWalking", false);
                animator.SetBool("HasAttacked", true);
                animator.SetFloat("AnimationSpeed", 1.7f);
                pm.PlayerAttack();
                break;
        }
            
        animator.SetBool("IsGrounded", GroundCheck());

        UpdatePlayerCamera();
        // GroundCheck();
    }


    /* ---------------------------------------------------------------- */
    /*                           Helper Methods                         */
    /* ---------------------------------------------------------------- */

    /* Changes the currentState of the Player
     * for private use only
    */
    void ChangeState(PlayerState newState)
    {
        if (currentState == newState)
            return;

        prevState = currentState;
        currentState = newState;
    }

    /* public version of the function above
     * For use outside of this PlayerController script
    */
    public void UpdatePlayerState(PlayerState state) { ChangeState(state); }

    /* Tracks movement behavior
     * Tracks behavior of player movement to update the states (walking, running)
    */
    void UpdateMovementState()
    {
        // GroundCheck
        if (GroundCheck()) {
            animator.SetBool("IsFalling", false);
        }

        if (pi.InputAxis.magnitude > 0 && !pm.isJumping && currentState != PlayerState.attacking) {
            ChangeState((pi.runKey) ? PlayerState.running : PlayerState.walking);
        }

        if (pi.InputAxis.magnitude == 0 && currentState != PlayerState.attacking) {
            ChangeState(PlayerState.idle);
        }

        // If player is falling (not because of jumping, i.e. walks off a ledge)
        if (rb.velocity.y < 0  && !pm.isJumping && !GroundCheck()) {
            animator.SetBool("IsFalling", true);
        }
    }

    /* Tracks attack behavior
     * Tracks the attack behavior to update the state(s) > (attacking)
    */
    void UpdateAttackState()
    {
        if (pi.attackKey && GroundCheck() && !pm.isJumping) {
            ChangeState(PlayerState.attacking);
        }
    }

    // Basic Ground check using a Physics.Raycast
    RaycastHit hitData;
    Ray ray;
    public bool GroundCheck()
    {
        ray = new Ray(transform.position, Vector3.down);
        grounded = Physics.Raycast(ray, out hitData, 0.55f);
        // Debug.DrawRay(transform.position, Vector3.down * hitData.distance, Color.red, 5f);

        return grounded;
    }

    /* Updates transfrom of the camera
     * Modifies transform.position
     * Modifies transform.rotation (Maybe? / Eventually?)
    */
    void UpdatePlayerCamera()
    {
        playerCamera.transform.position = Vector3.Lerp(playerCamera.transform.position, transform.position + cameraPosition, camFollowSpeed * Time.deltaTime);
    }
}
