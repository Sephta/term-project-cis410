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

    [SerializeField] private Vector3 weaponPosition = Vector3.zero;

    [SerializeField] private Vector3 weaponRotation = Vector3.zero;

    [SerializeField] private float weaponDamageMod = 0;

    [SerializeField] private int weaponCost = 0;


    /* --------------------------------------- */
    /*                Get / Set                */
    /* --------------------------------------- */
    
    public string WeaponName { get { return weaponName; } }

    public Sprite WeaponIcon { get { return weaponIcon; } }

    public GameObject WeaponModel { get { return weaponModel; } }

    public Vector3 WeaponPosition { get { return weaponPosition; } }

    public Vector3 WeaponRotation { get { return weaponRotation; } }

    public float WeaponDamageMod { get { return weaponDamageMod; } }

    public int WeaponCost { get { return weaponCost; } }
}
