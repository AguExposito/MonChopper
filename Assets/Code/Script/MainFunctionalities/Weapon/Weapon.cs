using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor.Animations;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour {

    [Header("References")]
    [SerializeField] private FPSController playerFPSController;
    [SerializeField] private Transform cam;
    [SerializeField] private HudController hud;

    [Space]
    [Header("Inputs")]
    [SerializeField] InputActionProperty shootInput;
    [SerializeField] InputActionProperty aimInput;
    [SerializeField] InputActionProperty reloadInput;

    [Space]
    [Header("Read Only Variables"), ReadOnly]
    public WeaponData weaponData;
    [SerializeField] private ParticleSystem shotPS;
    [SerializeField] private Animator weaponAnimator;
    [SerializeField] float timeSinceLastShot;
    [SerializeField] WeaponData[] weaponDataScriptObj;

    private void Awake() {
        weaponDataScriptObj=Resources.LoadAll<WeaponData>("ScriptableObjects/Weapons");

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.activeInHierarchy) //Checks some weapon is active
            {
                foreach (WeaponData data in weaponDataScriptObj)
                {
                    weaponData = transform.GetChild(i).name == data.name ? data : null;
                    break;
                }
                shotPS = transform.GetChild(i).Find("ShootPoint").GetChild(0).GetComponent<ParticleSystem>();
                weaponAnimator = transform.GetChild(i).GetComponent<Animator>();
                weaponAnimator.SetFloat("ReloadSpeed", 1 / weaponData.reloadTime);
                break;
            }
        }
    }
    private void Start()
    {
        
    }

    private void Update() {
        #region Handles Inputs
        //when shoot input
        if (shootInput.action.inProgress) { 
            Shoot();
        }//when aim input
        if (aimInput.action.inProgress) { 
            Aim();            
        }
        else
        {
            StopAim();
        }
        //when reload input
        if (reloadInput.action.inProgress && weaponData.currentAmmo!=weaponData.magSize)
        {
            StartReload();
        }
        #endregion

        //Shoot Delay
        if (timeSinceLastShot < 1f / (weaponData.fireRate)) {
            timeSinceLastShot += Time.deltaTime;
        }

        //debug methods
        Debug.DrawRay(cam.position, cam.forward * weaponData.maxDistance);
    }

    #region Reload Methods
    public void StartReload()
    {
        if (!weaponData.reloading && this.gameObject.activeSelf && weaponData.ammoAmount!=0)
            StartCoroutine(Reload());
    }

    private IEnumerator Reload()
    {
        weaponData.reloading = true;
        weaponAnimator.SetBool("Reload", true);
        
        AnimatorStateInfo stateInfo = weaponAnimator.GetCurrentAnimatorStateInfo(0);

        yield return new WaitForSeconds(weaponData.reloadTime);

        if (weaponData.ammoAmount - (weaponData.magSize - weaponData.currentAmmo) >= 0)
        {
            weaponData.ammoAmount -= (weaponData.magSize - weaponData.currentAmmo);
            weaponData.currentAmmo = weaponData.magSize;
        }
        else {
            weaponData.currentAmmo = weaponData.ammoAmount;
            weaponData.ammoAmount = 0;
        }        

        hud.UpdateHudValues();

        weaponData.reloading = false;
        weaponAnimator.SetBool("Reload", false);
    }
    #endregion

    #region Shoot Methods
    //Condicion para disparar
    private bool CanShoot() => !weaponData.reloading && timeSinceLastShot > 1f / (weaponData.fireRate);

    private void Shoot()
    {
        if (weaponData.currentAmmo > 0)
        {
            if (CanShoot()) //Chequea condición para disparar
            {
                if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hitInfo, weaponData.maxDistance))
                {

                    IDamageable damageable = hitInfo.transform.GetComponent<IDamageable>();
                    float distance = Vector3.Distance(transform.position, hitInfo.point);
                    damageable?.TakeDamage(CalculateDmg(distance), weaponData);
                }

                weaponData.currentAmmo -= weaponData.bulletAmount;
                timeSinceLastShot = 0;
                OnGunShot();
            }
        }
    }
    //Calculates damage based on distance
    private float CalculateDmg(float distance)
    {
        if (distance <= weaponData.maxDmgDistance) { Debug.Log(weaponData.bulletDmgMax); return weaponData.bulletDmgMax; }
        else {
            float damage = Mathf.Lerp(weaponData.bulletDmgMax, weaponData.bulletDmgMin, (distance-weaponData.maxDmgDistance) / (weaponData.maxDistance-weaponData.maxDmgDistance));
            damage = MathF.Round(damage,2);
            Debug.Log(damage);
            return damage;
        }
    }
    private void OnGunShot() {
        shotPS.Play();
        hud.UpdateHudValues();
    }
#endregion

    #region Aim Methods
    private void Aim() {        
        if (!weaponAnimator.GetBool("Aim")) //Checks animation bool to active correctly 
        {
            weaponAnimator.SetBool("Aim", true);
            playerFPSController.ChangeMovementVariables(playerFPSController.walkSpeed / 2, playerFPSController.runSpeed / 2, playerFPSController.jumpPower);
        }
    }

    private void StopAim() {
        if (weaponAnimator != null && weaponAnimator.GetBool("Aim"))
        {
            weaponAnimator.SetBool("Aim", false);
            playerFPSController.ChangeMovementVariables(playerFPSController.walkSpeed * 2, playerFPSController.runSpeed * 2, playerFPSController.jumpPower);
        }
    }
    #endregion

    private void OnEnable()
    {
        reloadInput.action.Enable();
        shootInput.action.Enable();
        aimInput.action.Enable();
    }
    private void OnDisable()
    {
        weaponData.reloading = false;
        reloadInput.action.Disable();
        shootInput.action.Disable();
        aimInput.action.Disable();
    }
    
}
