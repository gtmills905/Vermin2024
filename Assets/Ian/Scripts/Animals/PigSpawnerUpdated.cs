using System.Collections;
using UnityEngine;
using Photon.Pun; // Include Photon PUN

public class PigSpawnerUpdated : MonoBehaviourPunCallbacks
{
    public bool CanSpawn = true;
    public GameObject PigPrefab;
    private Vector3 MyPos;
    [SerializeField]
    private int SpawnArea = 1;
    private const int MaxInitialPigs = 7;  // Adjusted initial pig count
    private const int MaxTotalPigs = 10;   // Adjusted max pig count
    private int currentPigCount = 0;

    void Start()
    {
        MyPos = transform.position;

        if (PhotonNetwork.IsMasterClient) // Only the master client should spawn pigs
        {
            SpawnInitialPigs();
            StartCoroutine(SpawnPigs());
        }
    }

    void SpawnInitialPigs()
    {
        for (int i = 0; i < MaxInitialPigs; i++)
        {
            SpawnNewPig();
        }
    }

    IEnumerator SpawnPigs()
    {
        while (currentPigCount < MaxTotalPigs)
        {
            SpawnNewPig();
            yield return new WaitForSeconds(1.0f); // Adjust spawn delay
        }
    }

    void SpawnNewPig()
    {
        if (!CanSpawn || !PhotonNetwork.IsMasterClient) return;

        Vector3 spawnPos = new Vector3(MyPos.x + Random.Range(0, SpawnArea), MyPos.y, MyPos.z + Random.Range(0, SpawnArea));

        GameObject newPig = PhotonNetwork.Instantiate("PigPrefab", spawnPos, Quaternion.identity); // Use PhotonNetwork.Instantiate
        currentPigCount++;
        CanSpawn = false;
        StartCoroutine(DelayNextSpawn());
    }

    IEnumerator DelayNextSpawn()
    {
        yield return new WaitForSeconds(1.0f);
        CanSpawn = true;
    }

    public void PigDestroyed()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        currentPigCount--;
        CanSpawn = true;
    }
}
