using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaveIBeenShot : MonoBehaviour
{
    public GameManager Manager;
    public GameObject spawn;
    // Start is called before the first frame update
    void Start()
    {
        Manager = FindObjectOfType<GameManager>();
    }

    public void OnCollisionEnter(Collision collision)
    {
        if ("Bullet" == collision.gameObject.tag)
        {
            Manager.FarmerScore += 1;
            gameObject.SetActive(false);
            StartCoroutine(Respawn());

        }
    }
    public IEnumerator Respawn()
    {
        new WaitForSeconds(30f);
        transform.position = spawn.transform.position;  
        gameObject.SetActive (true);
        yield return 0;
    }
}
