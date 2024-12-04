using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class Player2 : NetworkBehaviour
{
    private Animator anim;
    private CharacterController controller;
    public float upanddownspeed = 20f;
    public float forwardspeed = 600.0f;
    public float turnspeed = 400.0f;
    public bool inZone = false;
    public PickupControl pickupControl;

    public Transform birdHandleTransform;
    public float maxHandleRotationAngle = 45f;
    public float rollDuration = 1f;

    private bool isRolling = false;
    private float originalForwardSpeed;
    private float lastYPosition;

    public enum PlayerCharacter { Character1, Character2, Character3, Character4 }
    public PlayerCharacter characterType;

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
        if (!IsOwner) return;

        if (!isRolling)
        {
            HandleMovement();
            HandleRotation();
        }

        if (Input.GetButtonDown("LTJoystick2"))
        {
            StartCoroutine(BarrelRoll(-1));
        }
        else if (Input.GetButtonDown("RTJoystick2"))
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
        float rightVertical = Input.GetAxis("RightJoystickVertical2");

        moveDirection.y += rightVertical * upanddownspeed * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
    }

    private void HandleRotation()
    {
        float rightHorizontal = Input.GetAxis("RightJoystickHorizontal2");
        float characterHorizontal = Input.GetAxis("HorizontalCharacter2");
        float rightandleftHorizontal = rightHorizontal + characterHorizontal;
        Vector3 rotation = new Vector3(0, rightandleftHorizontal * turnspeed * Time.deltaTime, 0);
        transform.Rotate(rotation);
    }

    private void HandleAnimation()
    {
        string verticalAxis = "Vertical" + characterType.ToString();
        float vertical = Input.GetAxis(verticalAxis);
        anim.SetInteger("AnimationPar", Mathf.Abs(vertical) > 0.1f ? 1 : 0);
    }

    private void HandleBirdHandleRotation()
    {
        float rightVertical = Input.GetAxis("RightJoystickVertical2");
        birdHandleTransform.localRotation = rightVertical < 0
            ? Quaternion.Euler(Mathf.Lerp(0, maxHandleRotationAngle, -rightVertical), 0, 0)
            : Quaternion.identity;
    }

    private void AdjustForwardSpeedBasedOnYPosition()
    {
        float currentYPosition = transform.position.y;
        float dropDistance = lastYPosition - currentYPosition;
        forwardspeed = dropDistance > 0
            ? originalForwardSpeed * (1 + Mathf.Clamp01(dropDistance / 10f) * 0.3f)
            : originalForwardSpeed;

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

