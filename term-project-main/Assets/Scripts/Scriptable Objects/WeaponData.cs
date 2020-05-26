using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDataAsset", menuName = "Items/Weapon", order = 1)]
public class WeaponData : ScriptableObject
{
    /* --------------------------------------- */
    /*                   Data                  */
    /* --------------------------------------- */
    [SerializeField] private string weaponName = "";

    [SerializeField] private Sprite weaponIcon = null;

    [SerializeField] private GameObject weaponModel = null;

    [SerializeField] private int weaponDamage = 0;

    [SerializeField] private int weaponCost = 0;


    /* --------------------------------------- */
    /*                Get / Set                */
    /* --------------------------------------- */
    
    public string WeaponName { get { return weaponName; } }

    public Sprite WeaponIcon { get { return weaponIcon; } }

    public GameObject WeaponModel { get { return weaponModel; } }

    public int WeaponDamage { get { return weaponDamage; } }

    public int WeaponCost { get { return weaponCost; } }
}
