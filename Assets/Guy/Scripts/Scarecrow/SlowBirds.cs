
using UnityEngine;

public class SlowBirds : MonoBehaviour
{
    public bool slowBirdsActive1 = true;
    public bool slowBirdsActive2 = true;
    public bool slowBirdsActive3 = true;

    public bool SlowingBirds1 = false;
    public bool SlowingBirds2 = false;
    public bool SlowingBirds3 = false;

    public Player1 player1Component;
    public Player2 player2Component;
    public Player3 player3Component;


    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.root.TryGetComponent<Player1>(out player1Component);
            other.transform.root.TryGetComponent<Player2>(out player2Component);
            other.transform.root.TryGetComponent<Player3>(out player3Component);

            if (player1Component != null)
            {
                SlowingBirds1 = true;
                player1Component.inZone = true;
                player1Component.AdjustSpeeds();

            }
            if (player2Component != null)
            {
                SlowingBirds2 = true;
                player2Component.inZone = true;
                player2Component.AdjustSpeeds();
            }
            if (player3Component != null)
            {
                SlowingBirds3 = true;
                player3Component.inZone = true;
                player3Component.AdjustSpeeds();
            }
        }
    }
    public void OnTriggerExit(Collider other)
    {

        other.transform.root.TryGetComponent<Player1>(out player1Component);
        other.transform.root.TryGetComponent<Player2>(out player2Component);
        other.transform.root.TryGetComponent<Player3>(out player3Component);


        if (other.CompareTag("Player"))
        {
            if (player1Component != null)
            {
                player1Component.inZone = false;
                SlowingBirds1 = false;
                player1Component.ResetSpeeds();
                player1Component = null;
            }
            if (player2Component != null)
            {
                player2Component.inZone = false;
                SlowingBirds2 = false;
                player2Component.ResetSpeeds();
                player2Component = null;
            }
            if (player3Component != null)
            {
                player3Component.inZone = false;
                SlowingBirds3 = false;
                player3Component.ResetSpeeds();
                player3Component = null;
            }
        }


    }
}

