using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCallAnimEvents : MonoBehaviour
{
    private float initialCameraFOV;
    private WeaponData weaponData;
    float duration;
    private void Start()
    {        
        weaponData = transform.parent.GetComponent<Weapon>().weaponData;
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
}
