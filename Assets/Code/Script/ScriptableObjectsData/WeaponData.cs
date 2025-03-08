using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD.MinMaxSlider;

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


}
