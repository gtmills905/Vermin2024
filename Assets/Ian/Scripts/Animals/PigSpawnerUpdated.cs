using System;
using System.Collections;
using UnityEngine;

public class PigSpawnerUpdated : MonoBehaviour
{
    public bool CanSpawn = true;
    public GameObject PigPrefab;
    private Vector3 MyPos;
    [SerializeField]
    private int SpawnArea = 1;
    private const int MaxInitialPigs = 15; // Changed to 7
    private const int MaxTotalPigs = 20; // Changed to 10
    private int currentPigCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        MyPos = transform.position;
        SpawnInitialPigs();
        StartCoroutine(SpawnPigs());
    }

    void SpawnInitialPigs()
    {
        for (int i = 0; i < MaxInitialPigs; i++)
        {
            GameObject newPig = Instantiate(PigPrefab);
            newPig.transform.position = new Vector3(MyPos.x + UnityEngine.Random.Range(0, SpawnArea), MyPos.y, MyPos.z + UnityEngine.Random.Range(0, SpawnArea));
            currentPigCount++;
        }
    }

    IEnumerator SpawnPigs()
    {
        while (currentPigCount < MaxTotalPigs)
        {
            SpawnNewPig();
            yield return null; // wait for one frame before spawning next pig
        }
    }

    void SpawnNewPig()
    {
        if (CanSpawn)
        {
            GameObject newPig = Instantiate(PigPrefab);
            newPig.transform.position = new Vector3(MyPos.x + UnityEngine.Random.Range(0, SpawnArea), MyPos.y, MyPos.z + UnityEngine.Random.Range(0, SpawnArea));
            currentPigCount++;
            StartCoroutine(DelayNextSpawn());
            CanSpawn = false;
        }
    }

    IEnumerator DelayNextSpawn()
    {
        yield return new WaitForSeconds(1.0f);
        CanSpawn = true;
    }

    public void PigDestroyed()
    {
        currentPigCount--;
        CanSpawn = true;
    }
}
