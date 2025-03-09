using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor.Animations;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponController : MonoBehaviour {

    [Header("References")]
    [SerializeField] private FPSController playerFPSController;
    [SerializeField] private Transform cam;
    [SerializeField] private HudController hud;
    [Space]
    [Header("State Variables")]
    [SerializeField] public bool isMenuActive;
    [Space]
    [Header("Inputs")]
    [SerializeField] InputActionProperty shootInput;
    [SerializeField] InputActionProperty aimInput;
    [SerializeField] InputActionProperty reloadInput;

    [Space]
    [Header("Read Only Variables"), ReadOnly]
    [SerializeField] public WeaponData weaponData;
    [SerializeField] private ParticleSystem shotPS;
    [SerializeField] private Animator weaponAnimator;
    [SerializeField] float timeSinceLastShot;
    [SerializeField] PopupDMG popupDmg;
    [SerializeField] Dictionary<Weapon,WeaponData> weaponDictionary = new Dictionary<Weapon, WeaponData>();
    private void Awake() {
        AssignWeaponVariables();
    }
    public void AssignWeaponVariables() {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).TryGetComponent<Weapon>(out Weapon weapon))
            {
                AddWeapon(weapon);//Añade el arma al diccionario
            }
            if (transform.GetChild(i).gameObject.activeInHierarchy) //Checks some weapon is active
            {
                foreach (var keyValue in weaponDictionary)
                {
                    weaponData = transform.GetChild(i).GetComponent<Weapon>() == keyValue.Key? keyValue.Value : null;
                    if (weaponData != null) { break; }
                }
                shotPS = transform.GetChild(i).Find("ShootPoint").GetChild(0).GetComponent<ParticleSystem>()!=null? transform.GetChild(i).Find("ShootPoint").GetChild(0).GetComponent<ParticleSystem>():null;
                weaponAnimator = transform.GetChild(i).GetComponent<Animator>();
                weaponAnimator.Play("Idle", 0, 0f);
                weaponAnimator.SetFloat("ReloadSpeed", 1 / weaponData.reloadTime);
                weaponAnimator.SetFloat("ShootSpeed", weaponData.fireRate);
                weaponAnimator.SetFloat("AimSpeed", 1/weaponData.aimTime);
                break;
            }
        }
    }
    private void Start()
    {
        popupDmg=GetComponent<PopupDMG>();
    }

    private void Update() {
        if (weaponData != null)
        {
            #region Handles Inputs
            //when shoot input
            if (shootInput.action.inProgress)
            {
                Shoot();
            }//when aim input
            if (aimInput.action.inProgress && weaponData.canAim)
            {
                Aim();
            }
            else
            {
                StopAim();
            }
            //when reload input
            if (reloadInput.action.inProgress && weaponData.currentAmmo != weaponData.magSize)
            {
                StopAim();
                StartReload();
            }
            #endregion

            //Shoot Delay
            if (timeSinceLastShot < 1f / (weaponData.fireRate))
            {
                timeSinceLastShot += Time.deltaTime;
            }

            //debug methods
            Debug.DrawRay(cam.position, cam.forward * weaponData.maxDistance);
        }
    }

    #region Reload Methods
    public void StartReload()
    {
        if (!weaponData.reloading && this.gameObject.activeSelf && weaponData.ammoAmount != 0)
        {
            StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        weaponData.reloading = true;
        weaponAnimator.SetBool("Reload", true);
        
        AnimationClip[] animClips = weaponAnimator.runtimeAnimatorController.animationClips;
        AnimationClip reloadClip=null;
        foreach (AnimationClip clip in animClips)
        {
            if (clip.name == "Reload"+weaponAnimator.gameObject.name) { reloadClip = clip; }
        }

        yield return new WaitForSeconds(reloadClip.length * weaponData.reloadTime);
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
    private bool CanShoot() => !weaponData.reloading && !isMenuActive && timeSinceLastShot > 1f / (weaponData.fireRate);

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
                    //DMG feedback
                    if (damageable != null && !hitInfo.transform.GetComponent<EnemyPart>().enemyScript.isDead)//Checks if hits something and is not dead
                    {
                        popupDmg.gotWeakSpotHit = hitInfo.transform.GetComponent<EnemyPart>().isWeak;//Checks if its a weak spot for critical dmg
                        float damage = popupDmg.gotWeakSpotHit ? CalculateDmg(distance) * weaponData.bulletCritMultiplier : CalculateDmg(distance); //if its critial applies critical modifier
                        popupDmg.PopupDmg(damage, hitInfo.point); //Popsup dmg text

                        //Appl DMG
                        damageable?.TakeDamage(CalculateDmg(distance), weaponData);
                    }
                    else //If dead show sever popupdmg
                    {
                        if (damageable != null && weaponData.canSever && hitInfo.transform.GetComponent<EnemyPart>().isDetachable)
                        {
                            popupDmg.isSeverDmg = true;
                            popupDmg.PopupDmg(weaponData.severDmg, hitInfo.point);//Popsup dmg text
                        }
                        damageable?.TakeDamage(weaponData.severDmg, weaponData);//Appl sever DMG
                    }
                }

                weaponData.currentAmmo -= weaponData.bulletAmount;
                timeSinceLastShot = 0;
                OnGunShot();
            }
        }
        else if(CanShoot()) {
            StopAim();
            StartReload();
        }
    }
    //Calculates damage based on distance
    private float CalculateDmg(float distance)
    {
        if (distance <= weaponData.maxDmgDistance) { Debug.Log(weaponData.bulletDmgMax); return weaponData.bulletDmgMax; }
        else {
            float damage = Mathf.Lerp(weaponData.bulletDmgMax, weaponData.bulletDmgMin, (distance-weaponData.maxDmgDistance) / (weaponData.maxDistance-weaponData.maxDmgDistance));
            damage = MathF.Round(damage,2);
            return damage;
        }
    }
    private void OnGunShot() {
        if (shotPS != null)
        {
            shotPS.Play();
        }
        hud.UpdateHudValues();
        weaponAnimator.SetTrigger("Shoot");
        for (int i = 0; i < weaponAnimator.transform.childCount; i++)
        {
            if (weaponAnimator.transform.GetChild(i).name== "Point Light") 
            {
                weaponAnimator.transform.Find("Point Light").gameObject.SetActive(true);
                break;
            }
        }
        Invoke("DisableLight",0.1f);
    }
    void DisableLight() {
        for (int i = 0; i < weaponAnimator.transform.childCount; i++)
        {
            if (weaponAnimator.transform.GetChild(i).name == "Point Light")
            {
                weaponAnimator.transform.Find("Point Light").gameObject.SetActive(false);
                break;
            }
        }
    }
#endregion

    #region Aim Methods
    private void Aim() {        
        if (!weaponAnimator.GetBool("Aim")&&!weaponData.reloading && weaponData.aiming == false) //Checks animation bool to active correctly 
        {
            //weaponData.aiming = true; ---> POR SIMPLEZA SE REALIZA EN SCRIPT: WeaponCallEvents
            weaponAnimator.SetBool("Aim", true);
        }
    }

    private void StopAim() {
        if (weaponAnimator != null && weaponAnimator.GetBool("Aim") && !weaponData.reloading && weaponData.aiming == true)
        {
            //weaponData.aiming = false; ---> POR SIMPLEZA SE REALIZA EN SCRIPT: WeaponCallEvents
            weaponAnimator.SetBool("Aim", false);            
        }
    }
    #endregion

    public void AddWeapon(Weapon weapon)
    {
        if (weapon != null && weapon.weaponData != null)
        {
            if (!weaponDictionary.ContainsKey(weapon))
            {
                weaponDictionary.Add(weapon, weapon.weaponData);
                Debug.Log("Arma agregada: " + weapon.weaponData.weaponName);
            }
        }
        else
        {
            Debug.LogWarning("El WeaponController o su WeaponData son nulos.");
        }
    }
    public void RemoveWeapon(Weapon weapon)
    {
        if (weapon != null && weapon.weaponData != null)
        {
            if (weaponDictionary.ContainsKey(weapon))
            {
                weaponDictionary.Remove(weapon);
                Debug.Log("Arma Removida: " + weapon.weaponData.weaponName);
            }
        }
        else
        {
            Debug.LogWarning("El WeaponController o su WeaponData son nulos.");
        }
    }
    // Método para obtener la WeaponData asociada a un WeaponController
    public WeaponData GetWeaponData(Weapon weapon)
    {
        if (weaponDictionary.TryGetValue(weapon, out WeaponData data))
        {
            return data;
        }
        Debug.LogWarning("No se encontró WeaponData para el arma indicada.");
        return null;
    }
    private void OnEnable()
    {
        reloadInput.action.Enable();
        shootInput.action.Enable();
        aimInput.action.Enable();
    }
    private void OnDisable()
    {
        reloadInput.action.Disable();
        shootInput.action.Disable();
        aimInput.action.Disable();
    }
    
}
