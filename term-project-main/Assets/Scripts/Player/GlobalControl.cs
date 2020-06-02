using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalControl : MonoBehaviour
{
    public static GlobalControl Instance;

    // player stats
    public float playerHealth = 100;
    public int playerWallet = 500;
    public int playerScore = 0;

    // persistent weapons
    public GameObject playerWeapon;
    public GameObject sword;
    public GameObject axe;
    public GameObject scimitar;

    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }

        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
}
