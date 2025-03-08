using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponCallAnimEvents : MonoBehaviour
{
    private float initialCameraFOV;
    private WeaponData weaponData;
    private FPSController playerFPSController;
    private Animator animator;
    float duration;
    private void Start()
    {        
        weaponData = transform.parent.GetComponent<WeaponController>().weaponData;
        playerFPSController = FindObjectOfType<FPSController>();
        animator = GetComponent<Animator>();
    }
    private IEnumerator ChangeCameraFOV(float targetFOV)
    {
        initialCameraFOV = transform.parent.parent.GetComponent<Camera>().fieldOfView;
        duration = (1 / weaponData.aimTime) / 2;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            transform.parent.parent.GetComponent<Camera>().fieldOfView = Mathf.Lerp(initialCameraFOV, targetFOV, t / duration);
            yield return null;
        }

        // Asegurar que el FOV se establece exactamente en el valor objetivo al final
        transform.parent.parent.GetComponent<Camera>().fieldOfView = targetFOV;

    }
    public void ChangeAimEstate(int estate) {
        if (estate == 0) {
            weaponData.aiming = false;
            if (playerFPSController.alteredMovement)
            {
                playerFPSController.ChangeMovementVariables(playerFPSController.walkSpeed * 2, playerFPSController.runSpeed * 2, playerFPSController.jumpPower, false);
            }
        }
        else {
            weaponData.aiming = true;
            if (!playerFPSController.alteredMovement)
            {
                playerFPSController.ChangeMovementVariables(playerFPSController.walkSpeed / 2, playerFPSController.runSpeed / 2, playerFPSController.jumpPower, true);
            }
        }
    }
}
