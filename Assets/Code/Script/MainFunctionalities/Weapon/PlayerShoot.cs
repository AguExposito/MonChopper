using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public static Action shootInput;
    public static Action reloadInput;

    [SerializeField] KeyCode reloadKey;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0)) { 
            shootInput?.Invoke();
        }
        if (Input.GetKeyDown(reloadKey)) { 
            reloadInput?.Invoke();
        }
    }
}
