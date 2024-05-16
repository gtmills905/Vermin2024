using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dog : MonoBehaviour
{
    public bool Released;
    public float detectionDistance = 10f;
    public float detectionAngle = 45f;
    public LayerMask detectionLayer;
    private float jumpForce = 10f;
    private bool isGrounded = false;
    public GameObject RecallPoint ;
    private Vector3 RecallPos;
    private Rigidbody rb;
    [SerializeField]
    float JumpHeight = 10 ;
    private bool Recalling = false ;

    private float movementSpeed = 3;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        RecallPos = RecallPoint.transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 0.1f);
        if (Released == true)
        {
            transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);// move
            StartCoroutine("LifeTimer");

            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, detectionDistance, detectionLayer))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    // Check if the hit object is within the detection angle
                    Vector3 directionToTarget = hit.point - transform.position;
                    float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);
                    if (angleToTarget <= detectionAngle / 2f)
                    {
                        Vector3 DogPos = transform.position;
                        Vector3 birdPos = hit.transform.position;
                        Vector3 direction = birdPos - DogPos;
                        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
                        transform.rotation = targetRotation;
                        if ((DogPos.z == birdPos.z) && (isGrounded == true) && (DogPos.x == birdPos.x) && ((birdPos.y - DogPos.y) <= JumpHeight))
                        {

                            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

                        }

                    }


                }

            }
        }
        if (Recalling == true)
        {
            Vector3 DogPos = transform.position;
            RecallPos = RecallPoint.transform.position;
            Vector3 direction = RecallPos - DogPos;
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = targetRotation;
            transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);
            if (DogPos == RecallPos)
            {
                Recalling = false;
            }
        }
        else
        {
            this.transform.position = RecallPos;
            SC_FPSController player = GameObject.Find("Farmer P4").GetComponent<SC_FPSController>();
            player.dogAttatched = true;
           
            StartCoroutine("Cooldown");
        }
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
    }

    public void ReleaseDog()
    {
        Released = true;
    }
    private void Recall ()
    {
        RecallPos = RecallPoint.transform.position;

    }
}