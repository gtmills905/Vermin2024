using System.Collections;
using UnityEngine;

public class Player1 : MonoBehaviour
{
    private Animator anim;
    private CharacterController controller;
    public float upanddownspeed = 20f;
    public float forwardspeed = 600.0f;
    public float turnspeed = 400.0f;
    public float sensitivity = 5.0f;
    public bool inZone = false;
    public PickupControl pickupControl;

    public bool slowBirdsActive1 = false;
    public int maxAnimalsCarried = 1;

    public SlowBirds slowBirds;

    public Transform birdHandleTransform;
    public float maxHandleRotationAngle = 45f;

    public float rollDuration = 1f;

    private bool isRolling = false;

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

    void Update()
    {
        if (!isRolling)
        {
            HandleMovement();
            HandleRotation();
        }

        if (Input.GetButtonDown("LTJoystick1"))
        {

            StartCoroutine(BarrelRoll(-1));
        }
        else if (Input.GetButtonDown("RTJoystick1"))
        {
            StartCoroutine(BarrelRoll(1));
        }

        HandleAnimation();
        HandleBirdHandleRotation();
        AdjustForwardSpeedBasedOnYPosition();

        AnimalsControlled();
    }

    private void HandleMovement()
    {
        string verticalAxis = "Vertical" + characterType.ToString();
        float vertical = Input.GetAxis(verticalAxis);
        Vector3 moveDirection = transform.forward * vertical * forwardspeed;
        float rightVertical = Input.GetAxis("RightJoystickVertical1");

        moveDirection.y += rightVertical * upanddownspeed * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
    }

    private void HandleRotation()
    {
        float rightHorizontal = Input.GetAxis("RightJoystickHorizontal1");
        float characterHorizontal = Input.GetAxis("HorizontalCharacter1");
        float rightandleftHorizontal = rightHorizontal + characterHorizontal;
        Vector3 rotation = new Vector3(0, rightandleftHorizontal * turnspeed * Time.deltaTime, 0);
        transform.Rotate(rotation);
    }

    private void HandleAnimation()
    {
        string verticalAxis = "Vertical" + characterType.ToString();
        float vertical = Input.GetAxis(verticalAxis);
        if (Mathf.Abs(vertical) > 0.1f)
        {
            anim.SetInteger("AnimationPar", 1);
        }
        else
        {
            anim.SetInteger("AnimationPar", 0);
        }
    }

    private void HandleBirdHandleRotation()
    {
        float rightVertical = Input.GetAxis("RightJoystickVertical1");
        if (rightVertical < 0)
        {
            float rotationAngle = Mathf.Lerp(0, maxHandleRotationAngle, -rightVertical);
            birdHandleTransform.localRotation = Quaternion.Euler(rotationAngle, 0, 0);
        }
        else
        {
            birdHandleTransform.localRotation = Quaternion.identity;
        }
    }

    private void AdjustForwardSpeedBasedOnYPosition()
    {
        float currentYPosition = transform.position.y;
        float dropDistance = lastYPosition - currentYPosition;
        if (dropDistance > 0)
        {
            float dropFactor = Mathf.Clamp01(dropDistance / 10f);
            forwardspeed = originalForwardSpeed * (1 + dropFactor * 0.3f);
        }
        else
        {
            forwardspeed = originalForwardSpeed;
        }
        lastYPosition = currentYPosition;
    }

    private IEnumerator BarrelRoll(int direction)
    {
        isRolling = true;
        float elapsedTime = 0f;
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(0f, 0f, 360f * direction);

        while (elapsedTime < rollDuration)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsedTime / rollDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.rotation = endRotation;
        isRolling = false;
    }

    public void AnimalsControlled()
    {
        if (!inZone && !(pickupControl != null && pickupControl.animalAttached))
        {
            ResetSpeeds();
        }
        else
        {
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
