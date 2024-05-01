using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigWander : MonoBehaviour
{
    private float movementSpeed = 3;
    private bool isFree = true;
    public float minRotationAngle = 90.0f;
    public float maxRotationAngle = 180.0f;
    public float minWaitTime = 1.0f;
    public float maxWaitTime = 5.0f;
    private bool isRotating = false;
    

    void Update()
    {
        if (isFree)
        {
            transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);

            if (!isRotating)
            {
                StartCoroutine(RotateObject());
            }
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            
            isFree = false;
           
        }
         RotateRandomAngle();
            
        
    }
    void OnTriggerEnter(Collider other)
    {



        isFree = false;
            Vector3 pigpos = transform.position;
            Vector3 birdpos = other.transform.position;


            Vector3 direction = pigpos - birdpos;
        Quaternion targetRotation = Quaternion.LookRotation(-direction, Vector3.up);
        transform.rotation = targetRotation;


    }
    void OnTriggerExit(Collider other)
    {
        isFree = true;
    }
    

    IEnumerator RotateObject()
    {
        isRotating = true;
        yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));

        RotateRandomAngle();

        yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
        isRotating = false;
    }

    void RotateRandomAngle()
    {
        Quaternion currentRotation = transform.rotation;
        Quaternion targetRotation = currentRotation * Quaternion.Euler(0f, Random.Range(minRotationAngle, maxRotationAngle), 0f);
        transform.rotation = targetRotation;
    }
}