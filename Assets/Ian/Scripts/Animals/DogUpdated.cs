using System.Collections;
using UnityEngine;

public class DogUpdated : MonoBehaviour
{
    public bool Released;
    public float detectionDistance = 10f;
    public float detectionAngle = 45f;
    public LayerMask detectionLayer;
    public float jumpForce = 10f;
    public GameObject RecallPoint;
    private Vector3 RecallPos;
    private Rigidbody rb;
    [SerializeField]
    private float JumpHeight = 2f; // Height to detect and jump towards the bird
    private bool Recalling = false;
    private float movementSpeed = 3f;
    private bool isGrounded = false;
    private Coroutine lifeTimerCoroutine;
    public Transform farmer; // Reference to the farmer

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        RecallPos = RecallPoint.transform.position;
    }

    void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 0.1f);

        if (Released)
        {
            MoveForward();

            if (lifeTimerCoroutine == null)  // Ensure the LifeTimer coroutine is only started once
            {
                lifeTimerCoroutine = StartCoroutine(LifeTimer());
            }

            DetectPlayer();
        }
        else if (!Recalling)
        {
            FollowFarmer();
        }

        if (Recalling)
        {
            RecallMovement();
        }
        else if (!Released && !Recalling)
        {
            AttachToRecallPoint();
        }
    }

    private void MoveForward()
    {
        transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);
    }

    private void DetectPlayer()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, detectionDistance, detectionLayer))
        {
            if (hit.collider.CompareTag("Player"))
            {
                Vector3 directionToTarget = hit.point - transform.position;
                float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);
                if (angleToTarget <= detectionAngle / 2f)
                {
                    RotateTowardsTarget(hit.transform.position);
                    TryJumpTowardsTarget(hit.transform.position);
                }
            }
        }
    }

    private void RotateTowardsTarget(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = targetRotation;
    }

    private void TryJumpTowardsTarget(Vector3 targetPosition)
    {
        if (isGrounded && Mathf.Abs(targetPosition.y - transform.position.y) <= JumpHeight)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void RecallMovement()
    {
        Vector3 direction = RecallPos - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * movementSpeed);

        Vector3 moveDirection = direction.normalized * movementSpeed * Time.deltaTime;
        transform.position += moveDirection;

        if (Vector3.Distance(transform.position, RecallPos) < 0.1f)
        {
            Recalling = false;
            transform.position = RecallPos; // Snap to the recall position to prevent jittering
            rb.velocity = Vector3.zero; // Stop any movement
        }
    }

    private void FollowFarmer()
    {
        if (farmer != null)
        {
            Vector3 direction = farmer.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * movementSpeed);

            Vector3 moveDirection = direction.normalized * movementSpeed * Time.deltaTime;
            transform.position += moveDirection;
        }
        else
        {
            Debug.LogWarning("Farmer reference is missing.");
        }
    }

    private void AttachToRecallPoint()
    {
        transform.position = RecallPos;
        SC_FPSController player = GameObject.Find("Farmer P4").GetComponent<SC_FPSController>();
        player.dogAttatched = true;

        if (!IsInvoking(nameof(StartCooldown)))  // Ensure cooldown is only started once
        {
            Invoke(nameof(StartCooldown), 0f);
        }
    }

    private void StartCooldown()
    {
        StartCoroutine(Cooldown());
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Recall();
        }
    }

    private IEnumerator LifeTimer()
    {
        yield return new WaitForSeconds(120);
        Recall();
    }

    private IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(20);
        Released = false;
        lifeTimerCoroutine = null;  // Reset the coroutine reference for future use
    }

    public void ReleaseDog()
    {
        Released = true;
        if (lifeTimerCoroutine == null)  // Start the life timer when the dog is released, if not already running
        {
            lifeTimerCoroutine = StartCoroutine(LifeTimer());
        }
    }

    private void Recall()
    {
        RecallPos = RecallPoint.transform.position;
        Recalling = true;
    }
}
