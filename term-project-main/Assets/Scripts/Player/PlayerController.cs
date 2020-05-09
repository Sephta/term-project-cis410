﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// ------------------------------------------------------------------
// THIS CODE ALLOWS FOR READ ONLY VARIABLES VISIBLE WITHIN THE EDITOR
// Credit: It3ration on the Unity Forums
// Link: https://answers.unity.com/questions/489942/how-to-make-a-readonly-property-in-inspector.html
public class ReadOnlyAttribute : PropertyAttribute {}
 
 [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
 public class ReadOnlyDrawer : PropertyDrawer
 {
     public override float GetPropertyHeight(SerializedProperty property,
                                             GUIContent label)
     {
         return EditorGUI.GetPropertyHeight(property, label, true);
     }
 
     public override void OnGUI(Rect position,
                                SerializedProperty property,
                                GUIContent label)
     {
         GUI.enabled = false;
         EditorGUI.PropertyField(position, property, label, true);
         GUI.enabled = true;
     }
 }
// ------------------------------------------------------------------

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    // Public Vars
    [Header("Animators")]
    public Animator animator;
    public Animator cameraAnimator;
    
    [Header("Prefabs")]
    public Bartender healthbar;
    public Bartender staminabar;

    // HP & Stamina
    [Header("Player Stats")]
    public float maxHealth = 100;
    public float maxStamina = 100;
    [ReadOnly] public float currentHealth;
    [ReadOnly] public float currentStamina;
    
    [Header("Camera Settings")]
    /* Player Camera Vars
     * Camera Game Object
     * pos
     * angle
     * follow speed
    */
    public Camera playerCamera;
    public Vector3 cameraPosition;
    public Vector3 cameraAngle;
    [Range(0, 5)] public float camFollowSpeed;
    /* ------------------- */

    public enum PlayerState { idle, walking, running, attacking };
    [HideInInspector] public PlayerState prevState;
    [ReadOnly] public PlayerState currentState;

    [Header("Grounded State")]
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

        currentHealth = maxHealth;
        healthbar.setMax(maxHealth);

        currentStamina = pm.stamina;
    }

    void Update()
    {
        UpdateMovementState();
        UpdateAttackState();
        
        UpdateStamina(pm.stamina);

        // TEST: testing HP system functionality
        if (Input.GetKeyDown(KeyCode.P))
            TakeDamage(15);

        // stamina regen
        if (currentState != PlayerState.running && pm.stamina < maxStamina)
            pm.stamina += pm.regenRate;
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
                animator.SetFloat("AnimationSpeed", 1.0f);
                pm.CombatState();
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
        // ! Removed canCombo check
        if (pi.attackKey && GroundCheck() && !pm.isJumping) {
            // animator.SetTrigger("DoCombo");
            // pm.PlayerAttack();
            // pm.DetectEnemies();
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

    // void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.yellow;
    //     Gizmos.DrawWireSphere(transform.position, 1.25f);
    // }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthbar.setValue(currentHealth);
    }

    void UpdateStamina(float value)
    {
        staminabar.setValue(value);
    }
}
