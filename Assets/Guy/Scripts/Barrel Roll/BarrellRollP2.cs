using System.Collections;
using UnityEngine;

public class BarrelRollP2 : MonoBehaviour
{
    public float rollDuration = 1f; // Duration of the roll in seconds

    private bool isRolling = false;
    private float rollSpeed;

    void Update()
    {
        if (!isRolling)
        {
            if (Input.GetButtonDown("LTJoystick2")) // Replace with your actual input name
            {
                StartCoroutine(BarrelRoll(-1));
            }
            else if (Input.GetButtonDown("RTJoystick2")) // Replace with your actual input name
            {
                StartCoroutine(BarrelRoll(1));
            }
        }
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
}
