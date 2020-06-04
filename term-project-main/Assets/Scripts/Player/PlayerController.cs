using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    // Public Vars
    [Header("Animators")]
    public Animator animator;
    public Animator cameraAnimator;
    
    [Header("Audio Sources")]
    public AudioSource stepSource;
    public AudioSource attackSource;

    [Header("UI")]
    public Bartender healthbar;
    public Bartender staminabar;
    public Image controlsUI;
    public Text money;

    // HP & Stamina
    [Header("Player Stats")]
    public float maxHealth = 100;
    public float maxStamina = 100;
    public int wallet = 500;
    public int score;
    public float currentHealth;
    public float currentStamina;
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

    // [Header("Particle System")]
    // public ParticleSystem ps;


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

        //if (gameObject.GetComponent<AudioSource>() != null)
        //    stepSource = gameObject.GetComponent<AudioSource>();

        // States
        currentState = PlayerState.idle;
        prevState = PlayerState.idle;

        // Cam
        playerCamera.transform.eulerAngles = cameraAngle;
        cameraOffset = playerCamera.transform.position - transform.position;
        
        // Stats
        healthbar.setMax(maxHealth);
        currentStamina = pm.stamina;

        LoadPlayer();
    }

    void Update()
    {
        if (startIdleTimer)
        {
            idleLookTimer -= Time.deltaTime;
            if (idleLookTimer <= 0f)
                idleLookTimer = 0f;

            animator.SetFloat("IdleLookTimer", idleLookTimer);
        }

        UpdateMovementState();
        UpdateAttackState();
        UpdateStamina(pm.stamina);
        if (stepSource != null && attackSource != null)
            PlaySounds();

        // Disable Controls UI
        if (Input.GetKeyDown(KeyCode.P))
            ToggleUI();
        
        // TEST: testing HP system functionality
        if (Input.GetKeyDown(KeyCode.L))
           TakeDamage(15);

        // Stamina Regen
        if (currentState != PlayerState.running && pm.stamina < maxStamina)
            pm.stamina += pm.regenRate;

        if (money.text != null)
            money.text = "$" + wallet.ToString();
    }

    private float idleLookTimer = 1.5f;
    private bool startIdleTimer = false;
    void FixedUpdate()
    {
        // Update Player State
        switch(currentState)
        {
            case PlayerState.idle:
                startIdleTimer = true;
                animator.SetBool("IsIdle", true);
                animator.SetBool("IsWalking", false);
                animator.SetBool("HasAttacked", false);
                animator.SetFloat("AnimationSpeed", 2f);
                break;

            case PlayerState.walking:
                startIdleTimer = false;
                idleLookTimer = 1.5f;
                animator.SetBool("IsWalking", true);
                animator.SetBool("IsIdle", false);
                animator.SetBool("HasAttacked", false);
                animator.SetFloat("AnimationSpeed", 1f);
                pm.PlayerMove(pi.runKey);
                //audioManager.Play("footstep");
                break;

            case PlayerState.running:
                startIdleTimer = false;
                idleLookTimer = 1.5f;
                animator.SetBool("IsWalking", true);
                animator.SetBool("IsIdle", false);
                animator.SetBool("HasAttacked", false);
                animator.SetFloat("AnimationSpeed", 2f);
                pm.PlayerMove(pi.runKey);
                //audioManager.Play("footstep");
                break;

            case PlayerState.attacking:
                startIdleTimer = false;
                idleLookTimer = 1.5f;
                animator.SetBool("IsIdle", false);
                animator.SetBool("IsWalking", false);
                animator.SetFloat("AnimationSpeed", 1.0f);
                pm.CombatState();
                animator.SetBool("HasAttacked", true);

                break;
        }
            
        animator.SetBool("IsGrounded", GroundCheck());

        UpdatePlayerCamera();
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

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthbar.setValue(currentHealth);
    }

    void UpdateStamina(float value)
    {
        staminabar.setValue(value);
    }

    void PlaySounds()
    {
        // JUMP
        //if (!grounded)
        //    stepSource.Stop();


        // FOOTSTEPS
        if (currentState == PlayerState.walking && grounded)
        {
            if (!stepSource.isPlaying)
            {
                stepSource.pitch = 0.75f;
                stepSource.PlayOneShot(stepSource.clip, 0.5f);
            }
        }

        else if (currentState == PlayerState.running && grounded)
            if (!stepSource.isPlaying)
            {
                stepSource.pitch = 1.3f;
                stepSource.PlayOneShot(stepSource.clip, 0.5f);
            }

        // ATTACK
        if (currentState == PlayerState.attacking)
            if (!attackSource.isPlaying)
                attackSource.PlayOneShot(attackSource.clip, 0.5f);
    }

    public void EquipItem(GameObject weapon)
    {
        // disable currently equipped weapon
        if (equippedWeapon != null)
        {
            // equippedWeapon.SetActive(false);
            Destroy(equippedWeapon);
        }

        activeWeapon = weapon;
        GameObject newWep = Instantiate(weapon);
        // WeaponController wc = newWep.GetComponent<WeaponController>();

        newWep.transform.parent = hand.transform;
        // newWep.transform.localPosition = wc.wepPosition;
        // newWep.transform.localEulerAngles = wc.wepRotation;

        switch(weapon.name)
        {
            case "Sword":
                newWep.transform.localPosition = GlobalControl.Instance._sword.WeaponPosition;
                newWep.transform.localEulerAngles = GlobalControl.Instance._sword.WeaponRotation;
                damageModifier = GlobalControl.Instance._sword.WeaponDamageMod;
                break;
            
            case "Scimitar":
                newWep.transform.localPosition = GlobalControl.Instance._scimitar.WeaponPosition;
                newWep.transform.localEulerAngles = GlobalControl.Instance._scimitar.WeaponRotation;
                damageModifier = GlobalControl.Instance._scimitar.WeaponDamageMod;
                break;

            case "Axe":
                newWep.transform.localPosition = GlobalControl.Instance._axe.WeaponPosition;
                newWep.transform.localEulerAngles = GlobalControl.Instance._axe.WeaponRotation;
                damageModifier = GlobalControl.Instance._axe.WeaponDamageMod;
                break;
        }
        
        equippedWeapon = newWep;
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

    public void ToggleUI()
    {
        controlsUI.enabled = !controlsUI.enabled;
        controlsUI.gameObject.SetActive(controlsUI.enabled);
    }

    public void SavePlayer()
    {
        if (GlobalControl.Instance == null)
            return;

        GlobalControl.Instance.playerCurrentHealth = currentHealth;
        GlobalControl.Instance.playerMaxHealth = maxHealth;
        GlobalControl.Instance.playerWallet = wallet;
        GlobalControl.Instance.playerScore = score;
        GlobalControl.Instance.playerWeapon = activeWeapon;
        GlobalControl.Instance.controlsUIEnabled = controlsUI.enabled;
    }

    public void LoadPlayer()
    {
        if (GlobalControl.Instance == null)
            return;

        activeWeapon = GlobalControl.Instance.playerWeapon;
        wallet = GlobalControl.Instance.playerWallet;
        currentHealth = GlobalControl.Instance.playerCurrentHealth;
        maxHealth = GlobalControl.Instance.playerMaxHealth;
        score = GlobalControl.Instance.playerScore;
        controlsUI.enabled = GlobalControl.Instance.controlsUIEnabled;
        controlsUI.gameObject.SetActive(controlsUI.enabled);
        
        healthbar.setMax(maxHealth);
        healthbar.setValue(currentHealth);
        EquipItem(activeWeapon);
    }
}
