using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/* Gathers user input and sends it to the Behavior Machine class
 * Will gather input from the Keyboard and Mouse
*/
public class PlayerInput : MonoBehaviour
{
    /* ---------------------------------------------------------------- */
    /*                             Variables                            */
    /* ---------------------------------------------------------------- */

    // Public Vars -------------------------------------------------------

    // ? Look up "Get" and "Set" C# keywords to see how this works
    public Vector2 InputAxis {
        get {
            Vector2 input = Vector2.zero;

            // * Either Unity's Input axis
            input.x = Input.GetAxis("Horizontal");
            input.y = Input.GetAxis("Vertical");

            input.Normalize();
            return input;
        }
    }

    public bool runKey {
        get { return Input.GetKey(KeyCode.LeftShift); }
    }

    public bool jumpKey {
        get { return Input.GetKey(KeyCode.Space); }
    }

    public bool interactKey {
        get { return Input.GetKeyDown(KeyCode.E); }
    }

    public bool attackKey {
        get { return Input.GetMouseButtonDown(0); }
    }

    // public bool chargeAttackKey {
    //     get { return Input.GetMouseButton(0); }
    // }

    // Private Vars ------------------------------------------------------

    private Vector2 previousInput = Vector2.zero;
    int jumpTimer;
    bool canJump;


    /* ---------------------------------------------------------------- */
    /*                              Updates()                           */
    /* ---------------------------------------------------------------- */
    
    void Start()
    {
        jumpTimer = -1;
    }

    void Update()
    {
        if (InputAxis.x != previousInput.x) {
            previousInput.x = InputAxis.x;
        }
        if (InputAxis.y != previousInput.y) {
            previousInput.y = InputAxis.y;
        }
    }

    void FixedUpdate()
    {
        if (!Input.GetKey(KeyCode.Space))
        {
            canJump = false;
            jumpTimer++;
        }
        else if (jumpTimer > 0)
            canJump = true;
    }

    public bool CanJump()
    {
        return canJump;
    }

    private void OnApplicationFocus(bool focusStatus) {
        Cursor.lockState = CursorLockMode.Locked;
    }
}
