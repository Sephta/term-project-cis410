using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody rb;

    public bool cameraMode; // False = Isometric, True = TopDown

    public enum PlayerState {
        grounded,
        jumping,
    }
    public PlayerState currentState;

    [SerializeField]
    Vector3 directionVector = Vector2.zero;
    public float movementSpeed;
    public float walkSpeed;
    public float runSpeed;
    // [SerializeField] float t = 0f;
    // public float t_acc; // walk -> run acceleration
    public float jumpForce;
    public float height;
    public float playerVelocity = 0.0f;

    float gravity = 9.8f;

    void Start()
    {
        currentState = PlayerState.grounded;
    }

    void Update()
    {
        // Determine player direction (normalized)
        if (cameraMode) {
            directionVector = GetAxisInput_TopDown();
        } else {
            directionVector = GetAxisInput_Isometric();
        }

        HandleOtherInput();

        // Move player's position given inputed direction * speed * amount of time passed
        // rb.MovePosition(transform.position + (directionVector * movementSpeed * Time.deltaTime));
        transform.position += directionVector * movementSpeed * Time.deltaTime;

        // Applies constant gravity to the player of about 9.8 (m/s)^2
        rb.AddForce(-transform.up * gravity, ForceMode.Acceleration);
    }

    // Returns a vector based on horizontal and vertical input axis
    Vector3 GetAxisInput_TopDown()
    {
        /*
        *  Here in 3D space (in Unity) the vertical and horizontal
        *  movement axis are defined by X and Z, not X and Y
        *  Also im no longer using GetAxis() from unity's input
        *  system because the acceleration and decceleration values
        *  are too slow...
        */
        // Vector3 direction = new Vector3(
        //     Input.GetAxis("Horizontal"),  // X
        //     0f,                           // Y
        //     Input.GetAxis("Vertical")     // Z
        // );

        Vector3 direction = Vector3.zero;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
            direction.z = 1.0f;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
            direction.z = -1.0f;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            direction.x = 1.0f;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            direction.x = -1.0f;
        }


        // will help maintain a consistant velocity when moving diagonally
        direction.Normalize();

        return direction;
    }

    Vector3 GetAxisInput_Isometric()
    {
        Vector3 direction = Vector3.zero;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
            direction.x = 1.0f;
            direction.y = 0.0f;
            direction.z = 1.0f;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
            direction.x = -1.0f;
            direction.y = 0.0f;
            direction.z = -1.0f;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            direction.x = 1.0f;
            direction.y = 0.0f;
            direction.z = -1.0f;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            direction.x = -1.0f;
            direction.y = 0.0f;
            direction.z = 1.0f;
        }

        direction.Normalize();
        return direction;
    }

    void HandleOtherInput()
    {
        // Jumping if on ground
        if (Input.GetKeyDown(KeyCode.Space) && currentState == PlayerState.grounded) {
            PerformJump(jumpForce);
        }

        // Running logic
        if (Input.GetKey(KeyCode.LeftShift)) {
            // movementSpeed = Mathf.Lerp(movementSpeed, runSpeed, t);
            // t += t_acc;
            movementSpeed = runSpeed;
        } else {
            movementSpeed = walkSpeed;
            // t = 0f;
        }
    }

    void PerformJump(float velocity)
    {
        // The bellow equations were used to derive the velocity value 'v'
        // float PE = gravity * rb.mass * (float)height;  // This is potential energy (PE = mgh)
        // float KE = (((1/2)*rb.mass)*(v*v)); // This is Kinetic Energy (KE = ((1/2)m)*(V)^2)

        // height = 10.0f;                        // Height of the jump
        playerVelocity = Mathf.Sqrt(2 * gravity * height);  // in physics this represents velecotity just before impact

        rb.AddForce(transform.up * playerVelocity, ForceMode.VelocityChange);
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Ground") {
            currentState = PlayerState.grounded;
        }
    }

    void OnCollisionExit(Collision other) {
        if (other.gameObject.tag == "Ground") {
            currentState = PlayerState.jumping;
        }
    }
}
