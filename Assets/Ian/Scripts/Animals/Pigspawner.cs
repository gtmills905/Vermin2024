using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pigspawner : MonoBehaviour
{


    public bool CanSpawn = true;

    public GameObject Pig;
    private Vector3 MyPos;
    [SerializeField]
    private int SpawnArea = 1;


    // Start is called before the first frame update
    void Start()
    {
        MyPos = this.transform.position;
        for (int i = 0; 5 > i; i++)
        {
            StartCoroutine(NewPig());

        }


    }

    // Update is called once per frame
    void Update()
    {

    }
    public IEnumerator NewPig()
    {
        CanSpawn = false;


        Pig = GameObject.Instantiate(Pig);
        Pig.transform.position = new Vector3(MyPos.x + Random.Range(0, SpawnArea), MyPos.y, MyPos.z + Random.Range(0, SpawnArea));//spawns pig at position of sets by
        yield return new WaitForSeconds(1.0f);


    }
}