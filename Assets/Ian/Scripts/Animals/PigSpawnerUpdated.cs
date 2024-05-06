using System.Collections;
using UnityEngine;

public class PigSpawnerUpdated : MonoBehaviour
{
    public GameObject pigPrefab; // Reference to the pig prefab to be spawned
    public GameObject[] spawnPoints; // Array of spawn point GameObjects
    public float spawnInterval = 60f; // Interval between pig spawns in seconds
    private int pigsSpawned = 0; // Number of pigs spawned in the current interval
    private bool canSpawn = true; // Flag to control spawning

    void Start()
    {
        // Start spawning coroutine
        StartCoroutine(SpawnPigs());
    }

    IEnumerator SpawnPigs()
    {
        while (true)
        {
            // Wait for spawn interval
            yield return new WaitForSeconds(spawnInterval);

            // Check if spawning is allowed and maximum number of pigs per interval is not exceeded
            if (canSpawn && pigsSpawned < 2)
            {
                // Get a random spawn point from the spawnPoints array
                GameObject spawnPoint = GetRandomSpawnPoint();

                // Spawn a pig at the spawn point position
                Instantiate(pigPrefab, spawnPoint.transform.position, Quaternion.identity);

                // Increment the number of pigs spawned
                pigsSpawned++;
            }
        }
    }

    // Get a random spawn point from the spawnPoints array
    private GameObject GetRandomSpawnPoint()
    {
        // Generate a random index within the range of the spawnPoints array
        int randomIndex = Random.Range(0, spawnPoints.Length);

        // Return the GameObject at the random index
        return spawnPoints[randomIndex];
    }

    // Reset the number of pigs spawned at the start of each interval
    void Update()
    {
        if (Time.time % spawnInterval < Time.deltaTime)
        {
            pigsSpawned = 0;
        }
    }
}
