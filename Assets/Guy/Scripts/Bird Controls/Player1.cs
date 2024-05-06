using UnityEngine;

public class Player1 : MonoBehaviour
{
    private Animator anim;
    private CharacterController controller;
    public float upanddownspeed = 20f;
    public float forwardspeed = 600.0f;
    public float turnspeed = 400.0f;
    public float sensitivity = 5.0f;

    public PickupControlPlayer1 pickupControl1;

    // Maximum number of animals that can be carried
    public int maxAnimalsCarried = 1;

    // Current number of animals carried
    private int currentAnimalsCarried = 0;


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

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = gameObject.GetComponentInChildren<Animator>();
    }

    void Update()
    {
        string verticalAxis = "Vertical" + characterType.ToString();

        float vertical = Input.GetAxis(verticalAxis);
        float rightVertical = Input.GetAxis("RightJoystickVertical1");
        float rightHorizontal = Input.GetAxis("RightJoystickHorizontal1");
        float characterHorizontal = Input.GetAxis("HorizontalCharacter1");
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
        AnimalsControlled();
    }
    void AnimalsControlled()
    {
        if (pickupControl1 != null && pickupControl1.animalAttached == true)
        {
            AdjustSpeeds();
        }
        else
        {
            upanddownspeed = 500f;
            forwardspeed = 20f;
        }
    }

    public void AdjustSpeeds()
    {
        upanddownspeed = 300f;
        forwardspeed = 14f;
    }

    // Reset carried animal count if the bird is destroyed
    void OnDestroy()
    {
        if (this != null)
        {
            currentAnimalsCarried = 0;
        }
    }

}
