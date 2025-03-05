using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD.MinMaxSlider;

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapon")]
public class WeaponData : ScriptableObject
{
    public enum weapon { Shotgun, Pistol, MeatCleaver }
    public enum tier { Common, Rare, SuperRare, Legendary, Mythic }
    [Header("Info")]
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
}
