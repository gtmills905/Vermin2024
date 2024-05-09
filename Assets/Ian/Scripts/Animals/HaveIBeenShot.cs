using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaveIBeenShot : MonoBehaviour
{
    public AudioSource audioSource;
    public GameObject particleEffect; // Assign the particle effect prefab in the Unity Editor
    public LivesCounter Manager;
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
            Instantiate(particleEffect, collision.gameObject.transform.position, Quaternion.identity);
            audioSource.Play();

            StartCoroutine(Respawn());

        }
    }
    public IEnumerator Respawn()
    {
        new WaitForSeconds(60f);
        transform.position = Vector3.zero;  
        gameObject.SetActive (true);
        yield return 0;
    }
}
