using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


// ! The bellow code caused issues when compiling the project into a playable build
// ------------------------------------------------------------------
// THIS CODE ALLOWS FOR READ ONLY VARIABLES VISIBLE WITHIN THE EDITOR
// Credit: It3ration on the Unity Forums
// Link: https://answers.unity.com/questions/489942/how-to-make-a-readonly-property-in-inspector.html
// public class ReadOnlyAttribute : PropertyAttribute {}
 
//  [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
//  public class ReadOnlyDrawer : PropertyDrawer
//  {
//      public override float GetPropertyHeight(SerializedProperty property,
//                                              GUIContent label)
//      {
//          return EditorGUI.GetPropertyHeight(property, label, true);
//      }
 
//      public override void OnGUI(Rect position,
//                                 SerializedProperty property,
//                                 GUIContent label)
//      {
//          GUI.enabled = false;
//          EditorGUI.PropertyField(position, property, label, true);
//          GUI.enabled = true;
//      }
//  }
// ------------------------------------------------------------------
// ! ----------------------------------------------------------------------------


[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    // TESTING: score system & timer

    // Public Vars
    [Header("Animators")]
    public Animator animator;
    public Animator cameraAnimator;
    
    [Header("StatBars")]
    public Bartender healthbar;
    public Bartender staminabar;

    // HP & Stamina
    [Header("Player Stats")]
    public float maxHealth = 100;
    public float maxStamina = 100;
    public int wallet = 500;
    public int score;
    public Text money;
    /* [ReadOnly] */ public float currentHealth;
    /* [ReadOnly] */ public float currentStamina;
    public float damageModifier = 1f;
    public float baseDamage = 25f;
    
    [Header("Equipment")]
    public GameObject hand;
    public GameObject activeWeapon = null;      // persistent global weapon
    public GameObject equippedWeapon = null;    // instance of activeWeapon
    
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
    public bool rotateAroundPlayer = true;
    public float rotationSpeed = 5.0f;
    private Vector3 cameraOffset;
    /* ------------------- */

    public enum PlayerState { idle, walking, running, attacking };
    [HideInInspector] public PlayerState prevState;
    /* [ReadOnly] */ public PlayerState currentState;

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
        // Getters
        if (gameObject.GetComponent<PlayerInput>() != null)
            pi = gameObject.GetComponent<PlayerInput>();

        if (gameObject.GetComponent<PlayerMovement>() != null)
            pm = gameObject.GetComponent<PlayerMovement>();
        
        if (gameObject.GetComponent<Rigidbody>() != null)
            rb = gameObject.GetComponent<Rigidbody>();

        // States
        currentState = PlayerState.idle;
        prevState = PlayerState.idle;

        // Cam
        playerCamera.transform.eulerAngles = cameraAngle;
        cameraOffset = playerCamera.transform.position - transform.position;
        
        // Stats
        healthbar.setMax(maxHealth);
        currentHealth = GlobalControl.Instance.playerHealth;
        currentStamina = pm.stamina;

        LoadPlayer();
    }

    void Update()
    {
        UpdateMovementState();
        UpdateAttackState();
        UpdateStamina(pm.stamina);

        // TEST: testing HP system functionality
        //if (Input.GetKeyDown(KeyCode.L))
        //    TakeDamage(15);

        // Stamina Regen
        if (currentState != PlayerState.running && pm.stamina < maxStamina)
            pm.stamina += pm.regenRate;

        money.text = "$" + wallet.ToString();
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
                animator.SetFloat("AnimationSpeed", pm.currentSpeed - 1f);
                pm.PlayerMove(pi.runKey);
                break;

            case PlayerState.running:
                animator.SetBool("IsWalking", true);
                animator.SetBool("IsIdle", false);
                animator.SetBool("HasAttacked", false);
                animator.SetFloat("AnimationSpeed", pm.currentSpeed - 1.5f);
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
        if (rotateAroundPlayer) 
        {
            Quaternion newRotation = Quaternion.LookRotation(transform.position - playerCamera.transform.position);
            playerCamera.transform.rotation = Quaternion.Slerp(playerCamera.transform.rotation, newRotation, rotationSpeed * Time.deltaTime);

            // Vector3 newPosition = transform.position + transform.forward * cameraOffset.z + transform.up * cameraOffset.y;
            Vector3 newPosition = transform.position + transform.forward * cameraPosition.z + transform.up * cameraPosition.y;
            playerCamera.transform.position = Vector3.Slerp(playerCamera.transform.position, newPosition, rotationSpeed * Time.deltaTime);
        } 
        else 
        {
            Vector3 newPosition = transform.position + cameraOffset;
            playerCamera.transform.position = Vector3.Lerp(playerCamera.transform.position, newPosition, camFollowSpeed * Time.deltaTime);
        }

        if (rotateAroundPlayer) 
        {
           playerCamera.transform.LookAt(transform);
        }
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

    public void EquipItem(GameObject weapon)
    {
        // disable currently equipped weapon
        if (equippedWeapon != null)
            equippedWeapon.SetActive(false);

        activeWeapon = weapon;
        GameObject newWep = Instantiate(weapon);
        WeaponController wc = newWep.GetComponent<WeaponController>();

        newWep.transform.parent = hand.transform;
        newWep.transform.localPosition = wc.wepPosition;
        newWep.transform.localEulerAngles = wc.wepRotation;
        
        equippedWeapon = newWep;
        damageModifier = wc.damageMod;
        equippedWeapon.SetActive(true);
    }

    public bool SpendCurrency(int amount)
    {
        if (wallet >= amount)
        {
            wallet -= amount;
            return true;
        }
        
        else
        {
            Debug.Log("not enough cash!");
            return false;
        }
    }

    public void SavePlayer()
    {
        if (GlobalControl.Instance == null)
            return;

        GlobalControl.Instance.playerHealth = currentHealth;
        GlobalControl.Instance.playerWallet = wallet;
        GlobalControl.Instance.playerScore = score;
        GlobalControl.Instance.playerWeapon = activeWeapon;
    }

    public void LoadPlayer()
    {
        if (GlobalControl.Instance == null)
            return;

        activeWeapon = GlobalControl.Instance.playerWeapon;
        wallet = GlobalControl.Instance.playerWallet;
        currentHealth = GlobalControl.Instance.playerHealth;
        score = GlobalControl.Instance.playerScore;
        
        healthbar.setValue(currentHealth);
        EquipItem(activeWeapon);
    }

    void OnCollisionEnter(Collision collision) {
        rb.angularVelocity = Vector3.zero;
    }

    void OnCollisionStay(Collision collision) {
        rb.angularVelocity = Vector3.zero;
    }
    void OnCollisionExit(Collision collision) {
        rb.angularVelocity = Vector3.zero;
    }
}
