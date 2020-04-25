﻿using System.Collections;
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

            // * Or directly check the keyboard keys
            /*
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
                input.y = 1.0f;
            }
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
                input.y = -1.0f;
            }
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
                input.x = 1.0f;
            }
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
                input.x = -1.0f;
            }
            */

            input.Normalize();
            return input;
        }
    }

    // Private Vars ------------------------------------------------------

    private Vector2 previousInput = Vector2.zero;


    /* ---------------------------------------------------------------- */
    /*                              Updates()                           */
    /* ---------------------------------------------------------------- */
    
    void Start() {}

    void Update()
    {
        if (InputAxis.x != previousInput.x) {
            previousInput.x = InputAxis.x;
        }
        if (InputAxis.y != previousInput.y) {
            previousInput.y = InputAxis.y;
        }
    }

    void FixedUpdate() {}


    /* ---------------------------------------------------------------- */
    /*                             Input Keys                           */
    /* ---------------------------------------------------------------- */

    public bool runKey {
        get { return Input.GetKey(KeyCode.LeftShift); }
    }

    public bool jumpKey {
        get { return Input.GetKey(KeyCode.Space); }
    }
}