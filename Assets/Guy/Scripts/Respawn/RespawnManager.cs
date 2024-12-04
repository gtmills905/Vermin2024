using System.Collections;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{

    public Transform[] spawnPoints;
    


    public static RespawnManager instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        foreach(Transform spawn in spawnPoints)
        {
            spawn.gameObject.SetActive(false);
        }
    }

    
    public Transform GetSpawnPoint()
    {
        return spawnPoints[Random.Range(0, spawnPoints.Length)];  
    }

  
}
