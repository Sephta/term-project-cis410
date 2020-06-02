using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        // InitShop();
    }

    private void InitWeapons()
    {
        // Instantiate the weapons into the scene
        sword = Instantiate(_sword.WeaponModel, transform.position, Quaternion.Euler(_sword.WeaponRotation));
        sword.name = "Sword";
        sword.transform.SetParent(transform);

        scimitar = Instantiate(_scimitar.WeaponModel, transform.position, Quaternion.Euler(_scimitar.WeaponRotation));
        scimitar.name = "Scimitar";
        scimitar.transform.SetParent(transform);

        axe = Instantiate(_axe.WeaponModel, transform.position, Quaternion.Euler(_axe.WeaponRotation));
        axe.name = "Axe";
        axe.transform.SetParent(transform);

        // Set Player default weapon
        playerWeapon = sword;
    }

    private void InitShop()
    {
        if (shop != null)
        {
            UI_Shop ui = shop.GetComponent<UI_Shop>();
            ui.sword = sword;
            ui.scimitar = scimitar;
            ui.axe = axe;
        }
    }
}
