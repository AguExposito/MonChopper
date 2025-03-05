using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Camera playerCamera;
    [SerializeField] public Weapon weapon;
    

    [Space]
    [Header("Inputs")]
    [SerializeField] InputActionProperty runInput;
    [SerializeField] InputActionProperty jumpInput;
    [SerializeField] InputActionProperty menuInput;
    [SerializeField] InputActionProperty inventoryInput;
    [SerializeField] InputActionProperty weaponChange;

    [Space]
    [Header("Movement Variables")]
    public float walkSpeed;
    public float runSpeed;
    public float jumpPower;
    [SerializeField] float gravity;

    [Space]
    [Header("Camera Variables")]
    [SerializeField] public float lookSpeed;
    [SerializeField] float lookXLimit;

    [Space]
    [Header("State Variables")]
    [SerializeField] bool canMove = true;

    [Space]
    [Header("Read Only Variables"), ReadOnly]
    [SerializeField] Vector3 moveDirection = Vector3.zero;
    [SerializeField] float rotationX = 0;
    [SerializeField] CharacterController characterController;
    [SerializeField] HudController hudController;
    [SerializeField] MenuController screenUI;
    [SerializeField] int currentWeaponIndex = 0;

    void Start()
    {
        hudController = transform.Find("CanvasHUD").GetComponent<HudController>();
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        screenUI = FindAnyObjectByType<MenuController>();
        for (int i = 0; weapon.transform.childCount > 0; i++)
        {
            if (weapon.transform.GetChild(i).gameObject.activeInHierarchy) { 
                currentWeaponIndex = i;
                break;
            }
        }
    }

    void Update()
    {
        #region Handles UI events
            #region Menu
        if (menuInput.action.WasPerformedThisFrame()) {
            if (screenUI.gameObject.activeInHierarchy) 
            {
                screenUI.gameObject.SetActive(false); //Deactivates menu on input
                screenUI.transform.GetChild(0).Find("Menu").gameObject.SetActive(false);
                screenUI.transform.GetChild(0).Find("Inventory").gameObject.SetActive(false); //Also deactivates inventory ui
            }
            else 
            {
                screenUI.gameObject.SetActive(true); //Activates menu on input
                screenUI.transform.GetChild(0).Find("Menu").gameObject.SetActive(true);
            }
        }
        #endregion
            #region Inventory
        if (inventoryInput.action.WasPerformedThisFrame() && !screenUI.transform.GetChild(0).Find("Menu").gameObject.activeInHierarchy) //Checks that menu is not active 
        {
            if (screenUI.gameObject.activeInHierarchy)
            {
                screenUI.gameObject.SetActive(false); //Deactivates inventory on input
                screenUI.transform.GetChild(0).Find("Inventory").gameObject.SetActive(false);
            }
            else
            {
                screenUI.gameObject.SetActive(true); //Activates inventory on input
                screenUI.transform.GetChild(0).Find("Inventory").gameObject.SetActive(true);
            }
        }
        #endregion
        #endregion

        #region Handles Movment
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Press Left Shift to run
        bool isRunning = runInput.action.inProgress;
        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        #endregion

        #region Handles Jumping
        if (jumpInput.action.inProgress && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        #endregion

        #region Handles Rotation
        characterController.Move(moveDirection * Time.deltaTime);

        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }

        #endregion

        #region Handles Weapon Change
        float scrollValue = weaponChange.action.ReadValue<float>();
        Debug.Log("SCROLL VALUE_ "+scrollValue);
        if (Mathf.Abs(scrollValue)>0.1f && !weapon.weaponData.reloading && !weapon.weaponData.aiming) {
            int weaponCount = weapon.transform.childCount;

            // Desactivar el arma actual
            weapon.transform.GetChild(currentWeaponIndex).gameObject.SetActive(false);

            // Determinar el nuevo índice basado en la dirección del scroll
            if (scrollValue > 0)
            {
                currentWeaponIndex = (currentWeaponIndex + 1) % weaponCount; // Cicla hacia adelante
            }
            else
            {
                currentWeaponIndex = (currentWeaponIndex - 1 + weaponCount) % weaponCount; // Cicla hacia atrás
            }

            // Activar la nueva arma
            weapon.transform.GetChild(currentWeaponIndex).gameObject.SetActive(true);
            OnWeaponChange(); // Llamamos la función para actualizar el cambio
        }
        
        #endregion
    }
    public void ChangeMovementVariables(float walkSpeed, float runSpeed, float jumpPower) { 
        this.walkSpeed = walkSpeed;
        this.runSpeed = runSpeed;
        this.jumpPower = jumpPower;
    }
    void OnWeaponChange() {
        weapon.AssignWeaponVariables();
        hudController.UpdateHudValues();
    }
    private void OnEnable()
    {
        jumpInput.action.Enable();
        runInput.action.Enable();
        menuInput.action.Enable();
        inventoryInput.action.Enable();
        weaponChange.action.Enable();
    }
    private void OnDisable()
    {
        jumpInput.action.Disable();
        runInput.action.Disable();
        menuInput.action.Disable();
        inventoryInput.action.Disable();
        weaponChange.action.Disable();
    }
}