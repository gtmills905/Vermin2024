
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
        player1Component = other.transform.parent.parent.GetComponent<Player1>();
        player2Component = other.transform.parent.parent.GetComponent<Player2>();
        player3Component = other.transform.parent.parent.GetComponent<Player3>();

        if (player1Component != null)
        {
            SlowingBirds1 = true;
            player1Component.slowBirdsActive1 = true;
            player1Component.AdjustSpeeds();
        }
        if (player2Component != null)
        {
            SlowingBirds2 = true;
            player2Component.slowBirdsActive2 = true;
            player2Component.AdjustSpeeds();
        }
        if (player3Component != null)
        {
            SlowingBirds3 = true;
            player3Component.slowBirdsActive3 = true;
            player3Component.AdjustSpeeds();
        }
    }
}

public void OnTriggerExit(Collider other)
{
    if (other.CompareTag("Player"))
    {
        if (player1Component != null && other.transform.parent.parent.GetComponent<Player1>() == player1Component)
        {
            SlowingBirds1 = false;
            player1Component.ResetSpeeds();
        }
        if (player2Component != null && other.transform.parent.parent.GetComponent<Player2>() == player2Component)
        {
            SlowingBirds2 = false;
            player2Component.ResetSpeeds();
        }
        if (player3Component != null && other.transform.parent.parent.GetComponent<Player3>() == player3Component)
        {
            SlowingBirds3 = false;
            player3Component.ResetSpeeds();
        }
    }
}


}
