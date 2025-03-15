using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Weapon", menuName = "Item/Weapon", order = 0)]
public class WeaponData : ItemData
{
    public enum weapon { Shotgun, Pistol, MeleeWeapon }

    [Header("WeaponInfo")]
    public weapon weaponType;
    public string weaponName;
    public tier weaponTier;

    [Header("Shooting")]
    public float fireRate;
    public float maxDistance;
    public float maxDmgDistance;
    public float dispersion;
    
    [Space]
    public float bulletDmgMin;
    public float bulletDmgMax;
    public float bulletCritMultiplier;
    public int bulletAmount;

    [Space]
    [Header("Aim")]
    public bool canAim;
    public bool aiming;
    public float aimTime;
    public float dispersionSight;
    public float aimFOV;

    [Header("Reloading")]
    public int currentAmmo;
    public int ammoAmount;
    public int magSize;
    public float reloadTime;
    public bool reloading;

    [Header("OnKill")]
    public float explosionForce;

    [Header("Sever")]
    public bool canSever;
    public int severDmg;
    public static WeaponData CreateFromJson(string json)
    {
        WeaponData instance = ScriptableObject.CreateInstance<WeaponData>();
        JsonUtility.FromJsonOverwrite(json, instance);
        return instance;
    }
    public void GenerateRandomWeapon(weapon weaponType) {
        switch (weaponType) { 
            case weapon.Pistol: 
                {
                    fireRate = UnityEngine.Random.Range(2,4);
                    maxDistance = UnityEngine.Random.Range(20,30);
                    maxDmgDistance = UnityEngine.Random.Range(maxDistance/3, maxDistance*2/3);
                    dispersion = 0;

                    bulletDmgMax = MathF.Round(UnityEngine.Random.Range(150,250),2);
                    bulletDmgMin = MathF.Round(UnityEngine.Random.Range(150, 250), 2);
                    bulletCritMultiplier = UnityEngine.Random.Range(1.5f,2.5f);
                    bulletAmount = UnityEngine.Random.Range(1,3);

                    aimFOV = 70;
                    aimTime = 1;

                    magSize = UnityEngine.Random.Range(6, 17);
                    currentAmmo = magSize;
                    reloadTime = UnityEngine.Random.Range(1, 3);

                    explosionForce = UnityEngine.Random.Range(10, 30);
                }
                break;
            case weapon.Shotgun: { }break;
            case weapon.MeleeWeapon: { }break;
        }
    }
}
