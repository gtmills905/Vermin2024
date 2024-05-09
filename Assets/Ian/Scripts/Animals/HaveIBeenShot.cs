using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaveIBeenShot : MonoBehaviour
{

    public GameObject spawn;//respawn point
    
    GameObject child;
    Component Pig;
    // Start is called before the first frame update
    void Start()
    {
       // Manager = FindObjectOfType<GameManager>();
       
        child = this.transform.GetChild(1).gameObject;
        Pig = this.GetComponentInChildren<PickupControlPlayer1>();
    }

    public void OnCollisionEnter(Collision collision)
    {
        if ("Bullet" == collision.gameObject.tag)
        {
            // Manager.FarmerScore += 1;
            // if carrying pig destroy pig call spawn
            if (  Pig.animalAttached == false)
            {

            }


            
            GameObject.Find("Game Manager").GetComponent<LivesCounter>().LivesLeft  -= 1;
            child.SetActive(false);
            StartCoroutine(Respawn());

        }
    }
    public IEnumerator Respawn()
    {
        new WaitForSeconds(30f);
        transform.position = spawn.transform.position;  
        child.SetActive (true);
        yield return 0;
    }
}
