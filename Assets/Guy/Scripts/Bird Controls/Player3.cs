using UnityEngine;

public class Player3 : MonoBehaviour
{
    private Animator anim;
    private CharacterController controller;
    public float upanddownspeed = 20f;
    public float forwardspeed = 600.0f;
    public float turnSpeed = 400.0f;
    public float sensitivity = 5.0f;

    public Transform birdHandleTransform; // Reference to the bird handle's transform
    public float maxHandleRotationAngle = 45f; // Maximum rotation angle of the bird handle

    private GameObject attachmentPoint;
    private GameObject currentTarget;
    private bool isAttached;

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
        isAttached = false;
        attachmentPoint = new GameObject("AttachmentPoint");
        attachmentPoint.transform.parent = transform;
        attachmentPoint.transform.localPosition = new Vector3(0, 0.5f, 0);
    }

    void Update()
    {
        string verticalAxis = "Vertical" + characterType.ToString();

        float vertical = Input.GetAxis(verticalAxis);
        float rightVertical = Input.GetAxis("RightJoystickVertical3");
        float rightHorizontal = Input.GetAxis("RightJoystickHorizontal3");
        float characterHorizontal = Input.GetAxis("HorizontalCharacter3");
        float rightandleftHorizontal = rightHorizontal + characterHorizontal;

        // Calculate rotation based on right joystick input
        Vector3 rotation = new Vector3(0, rightandleftHorizontal * turnSpeed * Time.deltaTime, 0);
        transform.Rotate(rotation);

        // Movement based on left joystick
        Vector3 moveDirection = transform.forward * vertical * forwardspeed;

        // Turning based on left joystick
        transform.Rotate(0, rightandleftHorizontal * turnSpeed * Time.deltaTime, 0);

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
    }


    void TryAttach()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 3))
        {
            if (hit.collider.CompareTag("Food"))
            {
                isAttached = true;
                currentTarget = hit.collider.gameObject;
                Debug.Log("Attaching Target");
                currentTarget.transform.SetParent(attachmentPoint.transform);
                currentTarget.transform.localPosition = Vector3.zero;
            }
        }
    }

    public void Detach()
    {
        isAttached = false;
        Debug.Log("Detaching Target");
        if (currentTarget != null)
        {
            currentTarget.transform.SetParent(null);
            currentTarget = null;
        }
    }
}