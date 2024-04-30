using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowBirds : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider has a tag called "Player"
        if (other.CompareTag("Player"))
        {
            // Check if the collider has a component called Player1, Player2, or Player3
            Player1 player1Component = other.GetComponent<Player1>();
            Player2 player2Component = other.GetComponent<Player2>();
            Player3 player3Component = other.GetComponent<Player3>();

            // If any of the components exist, call their AdjustSpeeds method
            if (player1Component != null)
            {
                player1Component.AdjustSpeeds();
            }
            if (player2Component != null)
            {
                player2Component.AdjustSpeeds();
            }
            if (player3Component != null)
            {
                player3Component.AdjustSpeeds();
            }
        }
    }
}
