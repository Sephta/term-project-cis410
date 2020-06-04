using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[RequireComponent(typeof(PlayerController))]
public class PlayerMovement : MonoBehaviour
{
    // Public Vars
    [Header("Jump State")]
    public bool isJumping = false;

    [Header("Movement")]
    /* [ReadOnly] */ public float currentSpeed = 0f;
    [Range(0, 6)] public float walkSpeed = 0f;
    [Range(2, 8)] public float runSpeed = 0f;
    public float jumpHeight = 0f;
    [Range(0.1f, 0.8f)] public float rotationSpeed = 0f;

    [HideInInspector] public float gravity = 9.8f;

    [Header("Stamina")]
    [Range(0, 100)] public float stamina = 100;
    [Range(0, 2)] public float drainRate = 1f;
    [Range(0, 5)] public float regenRate = 1f;

    [Header("Hit Detection")]
    public BoxCollider hitBox;

    // Private Vars
    Vector3 prevDirection = Vector3.zero;
    [HideInInspector] public Vector3 directionVector = Vector3.zero;
    float jumpVelocity = 0f;
    Vector3 desiredForward = Vector3.zero;
    Quaternion prevRotation = Quaternion.identity;

    private PlayerController pc;
    private PlayerInput pi;
    private Rigidbody rb;

    // Combat Vars
    // [HideInInspector]
    // public bool canCombo;
    // [HideInInspector]
    // public int comboCounter;


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

        // Init combat vars
        // comboCounter = 0;
        // canCombo = true;
    }

    void Update()
    {
        prevDirection = directionVector;
        directionVector = new Vector3(pi.InputAxis.x, 0f, pi.InputAxis.y);
        
        // Increment based on number of clicks
        // if (pi.attackKey) comboCounter++;
        // comboCounter = Mathf.Clamp(comboCounter, 0, 3);
    }

    void FixedUpdate()
    {
        if (pc.GroundCheck()) {
            pc.animator.SetBool("IsFalling", false);
            pc.animator.SetBool("IsJumping", false);
            isJumping = false;
            if (rb.velocity.y != 0.0f) {
                Vector3 vel = rb.velocity;
                vel.y = 0.0f;
                rb.velocity = vel;
            }
        }

        if (transform.position.y <= desiredHeight + 0.1f && transform.position.y >= desiredHeight - 0.1f) {
            isJumping = false;
            pc.animator.SetBool("IsJumping", isJumping);
            pc.animator.SetBool("IsFalling", true);
        }

        if (pc.GroundCheck() && pi.jumpKey && pc.currentState != PlayerController.PlayerState.attacking && Mathf.Approximately(rb.velocity.y, 0.0f)) {
            desiredHeight = transform.position.y + jumpHeight;
            isJumping = true;
            pc.animator.SetBool("IsJumping", isJumping);
            jumpVelocity = Mathf.Sqrt(2 * gravity * jumpHeight);
            rb.AddForce(Vector3.up * jumpVelocity, ForceMode.VelocityChange);
        }

        // Update model facing direction ----------------------------------------------------------
        // desiredForward = Vector3.RotateTowards(transform.forward, directionVector, rotationSpeed * Time.deltaTime, 0f);
        // modelRotation = Quaternion.LookRotation(desiredForward, Vector3.up);
        // transform.forward = Vector3.Normalize(directionVector);
        // if (pc.currentState != PlayerController.PlayerState.attacking)
            // transform.rotation = Quaternion.LookRotation(desiredForward);
         
        if (!pc.rotateAroundPlayer) {
            if (directionVector != Vector3.zero)
            {
                prevRotation = transform.rotation;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(directionVector), rotationSpeed);
            }
            else
            {
                transform.rotation = prevRotation;
            }
        }
        else
        {
            float h = 3.0f * Input.GetAxis("Mouse X");
            transform.Rotate(0, h, 0);
        }
        // ----------------------------------------------------------------------------------------
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
        runFlag = (pi.runKey && stamina > 0);


        // else player is running
        if (runFlag) {
            tw = 0f;

            currentSpeed = Mathf.Lerp(currentSpeed, runSpeed, tr);
            
            // transform.position += directionVector * currentSpeed * Time.deltaTime;
            // transform.rotation = modelRotation;
            // transform.eulerAngles = desiredForward;

            if (pc.rotateAroundPlayer) {
                transform.Translate(transform.rotation * directionVector * currentSpeed * Time.deltaTime, Space.World);
            } else {
                transform.Translate(directionVector * currentSpeed * Time.deltaTime, Space.World);
            }

            stamina -= drainRate;

            if (tr < 1f)
                tr += t_acc;
        }
        // if player is walking
        else {
            tr = 0f;

            currentSpeed = Mathf.Lerp(currentSpeed, walkSpeed, tw);

            // transform.position += directionVector * currentSpeed * Time.deltaTime;
            // transform.rotation = modelRotation;
            // transform.eulerAngles = desiredForward;

            if (pc.rotateAroundPlayer) {
                transform.Translate(transform.rotation * directionVector * currentSpeed * Time.deltaTime, Space.World);
            } else {
                transform.Translate(directionVector * currentSpeed * Time.deltaTime, Space.World);
            }

            if (tw < 1f)
                tw += t_acc;
            
        }
    }

    float currAtkTime = 0f;
    public void CombatState()
    {
        if (pc.animator.GetBool("HasAttacked"))
            pc.animator.SetTrigger("DoCombo");
        // currAtkTime += Time.deltaTime;
        // DetectEnemies();

        // if (currAtkTime >= 0.2f) {
            // hitBox.enabled = false;
            // currAtkTime = 0f;
            // pc.UpdatePlayerState(pc.prevState);
            // comboCounter++;
        // }
    }

    // This fuction doesnt do much now but Im planning on adding stuff here for JUICE
    // public void DetectEnemies()
    // {
    //     hitBox.enabled = true;
    // }

    // public void PlayerAttack()
    // {
    //     // All of the enemies damaged this attack call
    //     HashSet<GameObject> damaged = new HashSet<GameObject>();

    //     // Mask of all layers to ignore
    //     // LayerMask mask = (1) & (1 << 2) & (1 << 4) & (1 << 5) & (1 << 8) & (1 << 10);
    //     LayerMask enemyMask = (1 << 9);

    //     // A list of all of the colliders within range of the player
    //     Collider[] withinRange = Physics.OverlapSphere(transform.position, 1.25f, enemyMask);

    //     // For each enemy within range, apply damage
    //     foreach (Collider col in withinRange) {
    //         Transform target = col.transform;
    //         Vector3 dirToTarget = (target.position - transform.position).normalized;
    //         if (col.gameObject.tag == "Enemy" && !damaged.Contains(col.gameObject) && Vector3.Angle(transform.forward, dirToTarget) < 90) {
    //             damaged.Add(col.gameObject);
    //             Debug.Log("Hit Dummy");
    //             pc.cameraAnimator.SetTrigger("CamShake");
    //         }
    //     }
    // }
}
