using System.Collections;
using UnityEngine;

public class PigWandererUpdated : MonoBehaviour
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

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isFree = false;
            Vector3 pigpos = transform.position;
            Vector3 birdpos = other.transform.position;
            Vector3 direction = pigpos - birdpos;
            Quaternion targetRotation = Quaternion.LookRotation(-direction, Vector3.up);
            // Apply rotation only to the Y-axis (around the vertical axis)
            transform.rotation = Quaternion.Euler(0f, targetRotation.eulerAngles.y, 0f);
        }
        else if (other.gameObject.CompareTag("Obstacle"))
        {
            transform.Rotate(0f, 180f, 0f);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isFree = true;
        }
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
        // Get current rotation
        Quaternion currentRotation = transform.rotation;
        // Calculate random rotation around the Y-axis (vertical axis)
        float randomAngle = Random.Range(minRotationAngle, maxRotationAngle);
        Quaternion targetRotation = Quaternion.Euler(0f, currentRotation.eulerAngles.y + randomAngle, 0f);
        // Apply the rotation
        transform.rotation = targetRotation;
    }
}
