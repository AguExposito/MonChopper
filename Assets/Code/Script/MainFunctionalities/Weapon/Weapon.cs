using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour {

    [Header("References")]
    [SerializeField] private FPSController playerFPSController;
    [SerializeField] private WeaponData weaponData;
    [SerializeField] private Transform cam;
    [SerializeField] private ParticleSystem psShotgun;

    [Space]
    [Header("Inputs")]
    [SerializeField] InputActionProperty shootInput;
    [SerializeField] InputActionProperty aimInput;
    [SerializeField] InputActionProperty reloadInput;

    [Space]
    [Header("Read Only Variables"), ReadOnly]
    [SerializeField] float timeSinceLastShot;
    [SerializeField] Animator lastAimAnimator;

    private void Start() {

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
        if (reloadInput.action.inProgress)
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
        if (!weaponData.reloading && this.gameObject.activeSelf)
            StartCoroutine(Reload());
    }

    private IEnumerator Reload()
    {
        weaponData.reloading = true;

        yield return new WaitForSeconds(weaponData.reloadTime);

        weaponData.currentAmmo = weaponData.magSize;

        weaponData.reloading = false;
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
                    damageable?.TakeDamage(CalculateDmg(distance));
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
        if (distance <= weaponData.maxDmgDistance) { return weaponData.bulletDmgMax; }
        else {
            float damage = Mathf.Lerp(weaponData.bulletDmgMax, weaponData.bulletDmgMin, (distance-weaponData.maxDmgDistance) / (weaponData.maxDistance-weaponData.maxDmgDistance));
            Debug.Log(damage);
            return damage;
        }
    }
    private void OnGunShot() {  
        psShotgun.Play();
    }
#endregion

    #region Aim Methods
    private void Aim() {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.activeInHierarchy) //Checks some weapon is active
            {
                if (transform.GetChild(i).TryGetComponent<Animator>(out Animator anim)) //Checks it has an animator
                {
                    if (!anim.GetBool("Aim")) //Checks animation bool to active correctly 
                    {
                        lastAimAnimator = anim;
                        anim.SetBool("Aim", true);
                        playerFPSController.ChangeMovementVariables(playerFPSController.walkSpeed / 2, playerFPSController.runSpeed / 2, playerFPSController.jumpPower);
                    }
                }
                else //Else debugs Error
                {
                    Debug.LogError(anim.gameObject.name +" Doesn't have an animator component");
                }
            }
        }
    }

    private void StopAim() {
        if (lastAimAnimator != null && lastAimAnimator.GetBool("Aim"))
        {
            lastAimAnimator.SetBool("Aim", false);
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
