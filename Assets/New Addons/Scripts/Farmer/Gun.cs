using UnityEngine;

public class Gun : MonoBehaviour
{
    public bool isAutomatic;
    public float timeBetweenShots = .1f, heatPerShot = 3f;

    public GameObject muzzleFlash;

    public int shotDamage;
    public float gunRange = 1000.0f;
}

