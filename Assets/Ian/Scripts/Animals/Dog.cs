using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dog : MonoBehaviour
{
    private bool Released;
    public float detectionDistance = 20f;
    public float detectionAngle = 45f;
    public LayerMask detectionLayer;
    Vector3 DogPos;
    Vector3 birdPos;
    public float jumpForce = 10f;
    private Rigidbody rb;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if ( Released == true)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, detectionDistance, detectionLayer))
            {
                // Check if the hit object is within the detection angle
                Vector3 directionToTarget = hit.point - transform.position;
                float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);
                if (angleToTarget <= detectionAngle / 2f)
                {
                    DogPos = transform.position;
                    birdPos = hit.transform.position;
                   Vector3 direction = birdPos - DogPos;
                   Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
                   transform.rotation = targetRotation;

                }
            }
            if (( DogPos.x == birdPos.x)&& (DogPos.y == birdPos.y))
            {
                //jump
                rb.AddForce(Vector3.up * jumpForce);
            }
        }
       
    }

    public void ReleaseDog()
    {
        Released = true;
    }

}

