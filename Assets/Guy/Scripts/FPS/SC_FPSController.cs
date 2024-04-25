using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class SC_FPSController : MonoBehaviour
{
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public Camera playerCamera;
    public Transform leftArmPivot; // The pivot point around which the left arm rotates
    public Transform rightArmPivot; // The pivot point around which the right arm rotates
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;


    public enum PlayerCharacter
    {
        Character1,
        Character2,
        Character3,
        Character4
    }

    public PlayerCharacter currentPlayerCharacter = PlayerCharacter.Character1;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    [HideInInspector]
    public bool canMove = true;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

    }

    void Update()
    {
        // We are grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Input for Character 4 using right stick
        float horizontalInput = Input.GetAxis("HorizontalCharacter4");
        float verticalInput = Input.GetAxis("VerticalCharacter4");
        bool isSprinting = Input.GetKey(KeyCode.JoystickButton8); // L3 button

        float curSpeedX = canMove ? (isSprinting ? runningSpeed : walkingSpeed) * verticalInput : 0;
        float curSpeedY = canMove ? (isSprinting ? runningSpeed : walkingSpeed) * horizontalInput : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        // Apply gravity
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);

        // Player and Camera rotation
        if (canMove)
        {
            RotatePlayerAndArms();
        }

        // Handle different character behaviors
        switch (currentPlayerCharacter)
        {
            case PlayerCharacter.Character1:
                // Character 1 specific behavior
                break;
            case PlayerCharacter.Character2:
                // Character 2 specific behavior
                break;
            case PlayerCharacter.Character3:
                // Character 3 specific behavior
                break;
            case PlayerCharacter.Character4:
                // Character 4 specific behavior
                // Aim with right stick for Character 4
                RotateWithRightStick();
                break;
            default:
                break;
        }
    }

    void RotatePlayerAndArms()
    {
        float verticalRotation = Input.GetAxis("RightJoystickVerticalCharacter4") * lookSpeed;
        float horizontalRotation = Input.GetAxis("RightJoystickHorizontalCharacter4") * lookSpeed;

        rotationX += verticalRotation;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

        transform.rotation *= Quaternion.Euler(0, horizontalRotation, 0);

        // Rotate only the left arm around its pivot point
        Vector3 leftArmRotation = leftArmPivot.localEulerAngles;
        leftArmRotation.x = rotationX;
        leftArmPivot.localEulerAngles = leftArmRotation;

        // Rotate only the right arm around its pivot point
        Vector3 rightArmRotation = rightArmPivot.localEulerAngles;
        rightArmRotation.x = rotationX;
        rightArmPivot.localEulerAngles = rightArmRotation;
    }

    void RotateWithRightStick()
    {
        float horizontalRotation = Input.GetAxis("RightJoystickHorizontalCharacter4") * lookSpeed;
        float verticalRotation = -Input.GetAxis("RightJoystickVerticalCharacter4") * lookSpeed;

        rotationX += verticalRotation;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

        transform.rotation *= Quaternion.Euler(0, horizontalRotation, 0);

        // Rotate only the left arm around its pivot point
        Vector3 leftArmRotation = leftArmPivot.localEulerAngles;
        leftArmRotation.x = rotationX;
        leftArmPivot.localEulerAngles = leftArmRotation;

        // Rotate only the right arm around its pivot point
        Vector3 rightArmRotation = rightArmPivot.localEulerAngles;
        rightArmRotation.x = rotationX;
        rightArmPivot.localEulerAngles = rightArmRotation;
    }
}
