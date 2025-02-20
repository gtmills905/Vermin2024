using System.Runtime.Serialization;
using UnityEngine;
using Unity.Netcode;


public class Player3 : NetworkBehaviour
{
    private Animator anim;
    private CharacterController controller;
    public float upanddownspeed = 20f;
    public float forwardspeed = 600.0f;
    public float turnspeed = 400.0f;
    public float sensitivity = 5.0f;
    public bool inZone = false;
    public PickupControl pickupControl;

    public bool slowBirdsActive3 = false;
    // Maximum number of animals that can be carried
    public int maxAnimalsCarried = 1;

    public SlowBirds slowBirds;

    public Transform birdHandleTransform; // Reference to the bird handle's transform
    public float maxHandleRotationAngle = 45f; // Maximum rotation angle of the bird handle

    public enum PlayerCharacter
    {
        Character1,
        Character2,
        Character3,
        Character4
    }

    public PlayerCharacter characterType;

    private float originalForwardSpeed;
    private float lastYPosition;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = gameObject.GetComponentInChildren<Animator>();
        originalForwardSpeed = forwardspeed;
        lastYPosition = transform.position.y;
    }
    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            enabled = false;
            return;
        }
    }

    void Update()
    {
        OnNetworkSpawn();
        string verticalAxis = "Vertical" + characterType.ToString();

        float vertical = Input.GetAxis(verticalAxis);
        float rightVertical = Input.GetAxis("RightJoystickVertical3");
        float rightHorizontal = Input.GetAxis("RightJoystickHorizontal3");
        float characterHorizontal = Input.GetAxis("HorizontalCharacter3");
        float rightandleftHorizontal = rightHorizontal + characterHorizontal;

        // Calculate rotation based on right joystick input
        Vector3 rotation = new Vector3(0, rightandleftHorizontal * turnspeed * Time.deltaTime, 0);
        transform.Rotate(rotation);

        // Movement based on left joystick
        Vector3 moveDirection = transform.forward * vertical * forwardspeed;

        // Turning based on left joystick
        transform.Rotate(0, rightandleftHorizontal * turnspeed * Time.deltaTime, 0);

        // Rotate bird handle forward when moving downward on the y-axis
        if (rightVertical < 0)
        {
            float rotationAngle = Mathf.Lerp(0, maxHandleRotationAngle, -rightVertical);
            birdHandleTransform.localRotation = Quaternion.Euler(rotationAngle, 0, 0);
        }
        else
        {
            birdHandleTransform.localRotation = Quaternion.identity;
        }

        // Up and down based on left and right joystick vertical
        moveDirection.y += rightVertical * upanddownspeed * Time.deltaTime;

        // Adjust forward speed based on Y-axis drop
        float currentYPosition = transform.position.y;
        float dropDistance = lastYPosition - currentYPosition;

        if (dropDistance > 0)
        {
            float dropFactor = Mathf.Clamp01(dropDistance / 10f); // Assuming max drop distance is 10 for full speed increase
            forwardspeed = originalForwardSpeed * (1 + dropFactor * 0.3f); // Up to 30% speed increase
        }
        else
        {
            forwardspeed = originalForwardSpeed;
        }

        // Update lastYPosition for the next frame
        lastYPosition = currentYPosition;

        // Move the character
        controller.Move(moveDirection * Time.deltaTime);

        // Animation control
        if (Mathf.Abs(vertical) > 0.1f)
        {
            anim.SetInteger("AnimationPar", 1);
        }
        else
        {
            anim.SetInteger("AnimationPar", 0);
        }
        if (Input.GetButtonDown("Place"))
        {
            slowBirds = FindObjectOfType<SlowBirds>();
        }

        AnimalsControlled();
        
    }

    public void AnimalsControlled()
    {
        if (!inZone && !(pickupControl != null && pickupControl.animalAttached))
        {
            // Reset speeds when not slowed by other scripts
            ResetSpeeds();
        }
        else
        {
            // Adjust speeds when slowed by other scripts
            AdjustSpeeds();
        }
    }

    public void ResetSpeeds()
    {
        upanddownspeed = 700f;
        forwardspeed = 25f;
    }

    public void AdjustSpeeds()
    {
        upanddownspeed = 400f;
        forwardspeed = 14f;
    }
}
