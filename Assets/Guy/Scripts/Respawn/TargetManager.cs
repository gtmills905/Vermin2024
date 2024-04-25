using UnityEngine;

public class TargetManager : MonoBehaviour
{
    public GameObject targetPrefab;
    public Transform spawnPoint;
    private GameObject[] targets;

    private void Start()
    {
        InvokeRepeating("CheckAndSpawnTarget", 10f, 10f); // Check and spawn every 10 seconds
    }

    private void CheckAndSpawnTarget()
    {
        GameObject[] currentTargets = GameObject.FindGameObjectsWithTag("Target");
        bool isTargetPresent = false;
        foreach (GameObject target in currentTargets)
        {
            if (target != null && target.transform.position == spawnPoint.position)
            {
                isTargetPresent = true;
                break;
            }
        }

        if (!isTargetPresent)
        {
            SpawnTarget();
        }
    }

    private void SpawnTarget()
    {
        Instantiate(targetPrefab, spawnPoint.position, Quaternion.identity);
    }
}
