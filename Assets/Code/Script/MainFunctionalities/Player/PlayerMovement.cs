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
    [SerializeField] MenuController menu;

    [Space]
    [Header("Inputs")]
    [SerializeField] InputActionProperty runInput;
    [SerializeField] InputActionProperty jumpInput;
    [SerializeField] InputActionProperty menuInput;

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

    void Start()
    {
        hudController = transform.Find("CanvasHUD").GetComponent<HudController>();
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        #region Handles UI events
        if (menuInput.action.WasPerformedThisFrame()) {
            if (menu.gameObject.activeInHierarchy) 
            {
                menu.gameObject.SetActive(false); //Deactivates menu on input
            }
            else 
            { 
                menu.gameObject.SetActive(true); //Activates menu on input
            }
        }
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
        if ((Input.mouseScrollDelta.y==1 || Input.mouseScrollDelta.y == -1) && !weapon.weaponData.reloading && !weapon.weaponData.aiming) {
            for (int i = 0; i < weapon.transform.childCount; i++)
            {
                GameObject equipedWeapon = weapon.transform.GetChild(i).gameObject;
                if (equipedWeapon.activeInHierarchy) { 
                    equipedWeapon.SetActive(false);
                    if (i == weapon.transform.childCount - 1)
                    {
                        weapon.transform.GetChild(0).gameObject.SetActive(true);
                        OnWeaponChange();
                    }
                    else {
                        weapon.transform.GetChild(i+1).gameObject.SetActive(true);
                        OnWeaponChange();
                    }
                    break;
                }
            }
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
    }
    private void OnDisable()
    {
        jumpInput.action.Disable();
        runInput.action.Disable();
        menuInput.action.Disable();
    }
}