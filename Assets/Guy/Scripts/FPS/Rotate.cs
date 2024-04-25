using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{

    public float swayAmount = 0.02f;
    public float maxSwayAmount = 0.06f;
    public float swaySmoothness = 4.0f;

    private Vector3 initialPosition;
    private float horizontalInput;
    private float verticalInput;

    private void Start()
    {
        initialPosition = transform.localPosition;
    }

    private void Update()
    {

        horizontalInput = Input.GetAxis("Mouse X");
        verticalInput = Input.GetAxis("Mouse Y");


        float newXPosition = Mathf.Clamp(horizontalInput * swayAmount, -maxSwayAmount, maxSwayAmount);
        float newYPosition = Mathf.Clamp(verticalInput * swayAmount, -maxSwayAmount, maxSwayAmount);

        Vector3 targetPosition = new Vector3(newXPosition, newYPosition, 0);


        transform.localPosition = Vector3.Lerp(transform.localPosition, initialPosition + targetPosition, Time.deltaTime * swaySmoothness);
    }
}

