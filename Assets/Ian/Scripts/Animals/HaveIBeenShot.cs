using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaveIBeenShot : MonoBehaviour
{
    public LivesCounter Manager;
    public GameObject spawn;
    // Start is called before the first frame update
    void Start()
    {
        GameObject Object = GameObject.Find("GameManager"); 
        Manager = Object.GetComponent<LivesCounter>(); 
    }

    public void OnCollisionEnter(Collision collision)
    {
        if ("Bullet" == collision.gameObject.tag)
        {
            Manager.LivesLeft -= 1;
            gameObject.SetActive(false);
            StartCoroutine(Respawn());

        }
    }
    public IEnumerator Respawn()
    {
        new WaitForSeconds(60f);
        transform.position = spawn.transform.position;  
        gameObject.SetActive (true);
        yield return 0;
    }
}
