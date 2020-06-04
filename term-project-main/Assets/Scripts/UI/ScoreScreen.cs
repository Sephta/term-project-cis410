using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreScreen : MonoBehaviour
{
    //public GameObject globalObject;
    public Text scoreText;

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        scoreText.text = "SCORE: " + GlobalControl.Instance.playerScore;
    }
}
