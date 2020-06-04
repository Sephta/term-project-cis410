using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GlobalControl : MonoBehaviour
{
    public static GlobalControl Instance;

    public GameObject shop;

    public WeaponData _sword;
    public WeaponData _scimitar;
    public WeaponData _axe;

    [Header("Player Stats")]
    // player stats
    public float playerHealth = 100;
    public int playerWallet = 0;
    public int playerScore = 0;

    // persistent weapons
    public GameObject playerWeapon;
    public GameObject sword;
    public GameObject scimitar;
    public GameObject axe;

    [Header("UI")]
    public bool controlsUIEnabled = true;

    [Header("Timer Vars")]
    // timer stuff
    public Text timerText;
    public float timeLimit;
    public float timer;
    [SerializeField] private static bool timeSet = false;

    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
            InitWeapons();
        }

        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        if (timeSet == false)
        {
            timer = timeLimit * 60;
            timeSet = true;
        }
    }


    private void Start() {}

    private void Update()
    {
        if (timer >= 0.0f)
        {
            timer -= Time.deltaTime;
            UpdateTimer();
        }

        else if (timer <= 0.0f)
        {
            timer = 0.0f;
            UpdateTimer();
            GameOver();
        }
    }

    private void UpdateTimer()
    {
        string minutes = Mathf.Floor(timer / 60).ToString("00");
        string seconds = Mathf.Floor(timer % 60).ToString("00");
        timerText.text = minutes + ":" + seconds;
    }

    private void InitWeapons()
    {
        // Instantiate the weapons into the scene
        sword = Instantiate(_sword.WeaponModel, transform.position - new Vector3(0f, 1000.0f, 0f), Quaternion.Euler(_sword.WeaponRotation));
        sword.name = "Sword";
        sword.transform.SetParent(transform);

        scimitar = Instantiate(_scimitar.WeaponModel, transform.position - new Vector3(0f, 1000.0f, 0f), Quaternion.Euler(_scimitar.WeaponRotation));
        scimitar.name = "Scimitar";
        scimitar.transform.SetParent(transform);

        axe = Instantiate(_axe.WeaponModel, transform.position - new Vector3(0f, 1000.0f, 0f), Quaternion.Euler(_axe.WeaponRotation));
        axe.name = "Axe";
        axe.transform.SetParent(transform);

        // Set Player default weapon
        playerWeapon = sword;
    }

    public void GameOver()
    {
        playerHealth = 100;
        playerWallet = 0;
        playerScore = 0;
        playerWeapon = sword;
        timer = timeLimit;

        SceneManager.LoadScene(0);
    }
}

