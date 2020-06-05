using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreScreen : MonoBehaviour
{
    //public GameObject globalObject;
    public Text tallyText;
    public Text scoreText;

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        tallyText.text = "Your Score: " + GlobalControl.Instance.playerScore.ToString() + " + $" + GlobalControl.Instance.playerWallet.ToString();
        scoreText.text = "Total: " + (GlobalControl.Instance.playerScore + GlobalControl.Instance.playerWallet);
    }
}
