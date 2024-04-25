using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigWander : MonoBehaviour
{
    private float movementSpeed = 3;
    private bool Free = true ; 
    public float RotateSpeed = 2f; //privatize when speed set 
    private bool turn= true  ;
    void Start()
    {


        
    }

    void Update()
    {
        if (Free == true)
        {
            transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);
            
            if ( turn == true)
            {
                StartCoroutine(move());
            }
            
        }
        
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if ("Player" == collision.gameObject.tag)
        {
            Free = false;

        }
        if ("PigsCanHit" == collision.gameObject.tag)
        {
            Quaternion targetRotation = Quaternion.Euler(0f, Random.Range(90, 180), 0f);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, RotateSpeed * Time.deltaTime);

        }
    }
    public IEnumerator move()
    {
      turn = false;
      Quaternion targetRotation = Quaternion.Euler(0f, Random.Range(45, 180), 0f);
      transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, RotateSpeed * Time.deltaTime);
        yield return new WaitForSeconds(Random.Range(1, 5));
        turn = true;

    }
}