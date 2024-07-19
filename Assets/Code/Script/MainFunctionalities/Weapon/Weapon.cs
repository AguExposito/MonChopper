using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

    [Header("References")]
    [SerializeField] private WeaponData weaponData;
    [SerializeField] private Transform cam;
    [SerializeField] private ParticleSystem psShotgun;
    
    float timeSinceLastShot;

    private void Start() {
        PlayerShoot.shootInput += Shoot;
        PlayerShoot.reloadInput += StartReload;
    }

    private void OnDisable() => weaponData.reloading = false;

    public void StartReload() {
        if (!weaponData.reloading && this.gameObject.activeSelf)
            StartCoroutine(Reload());
    }

    private IEnumerator Reload() {
        weaponData.reloading = true;

        yield return new WaitForSeconds(weaponData.reloadTime);

        weaponData.currentAmmo = weaponData.magSize;

        weaponData.reloading = false;
    }

    private bool CanShoot() => !weaponData.reloading && timeSinceLastShot > 1f / (weaponData.fireRate);

    private void Shoot() {
        if (weaponData.currentAmmo > 0) {
            if (CanShoot()) {
                if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hitInfo, weaponData.maxDistance)){

                    IDamageable damageable = hitInfo.transform.GetComponent<IDamageable>();
                    float distance = Vector3.Distance(transform.position, hitInfo.point);
                    damageable?.TakeDamage(CalculateDmg(distance));
                }

                weaponData.currentAmmo-=weaponData.bulletAmount;
                timeSinceLastShot = 0;
                OnGunShot();
            }
        }
    }

    private void Update() {
        if (timeSinceLastShot < 1f / (weaponData.fireRate)) {
            timeSinceLastShot += Time.deltaTime;
        }

        Debug.DrawRay(cam.position, cam.forward * weaponData.maxDistance);
    }
    public float CalculateDmg(float distance)
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
}
