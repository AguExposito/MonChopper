using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD.MinMaxSlider;

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapon")]
public class WeaponData : ScriptableObject
{
    public enum weapon { Shotgun }
    public enum tier { Common, Rare, SuperRare, Legendary, Mythic }
    [Header("Info")]
    public weapon weaponType;
    public string weaponName;
    public tier weaponTier;

    [Header("Shooting")]
    public int fireRate;
    public float maxDistance;
    public float maxDmgDistance;
    public float dispersion;
    public float dispersionSight;
    [Space]
    public float bulletDmgMin;
    public float bulletDmgMax;
    public int bulletAmount;
    [Space]
    public bool aiming;

    [Header("Reloading")]
    public int currentAmmo;
    public int ammoAmount;
    public int magSize;
    public int reloadTime;
    public bool reloading;

    
}
