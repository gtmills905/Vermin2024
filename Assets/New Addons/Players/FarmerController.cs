using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;
using Photon.Pun;
using System.Collections.Concurrent;

public class FarmerController : MonoBehaviourPunCallbacks
{

    public Transform viewPoint;
    public float mouseSensitivity = 1f;
    private float verticalRotStore;
    private Vector2 mouseInput;

    public bool invertLook;

    public float moveSpeed = 5f, runSpeed = 8f;
    private Vector3 moveDir, movement;
    private float activeMoveSpeed;

    public CharacterController charCon;

    public Camera cam;
    public GameObject bulletImpact;
    //public float timeBetweenShots = .1f;
    private float shotCounter;
    public float muzzleDisplayTime;
    private float muzzleCounter;

    public float maxHeat = 10f, /*heatPerShot = 3f,*/ coolRate = 3f, overHeatCoolRate = 5f;

    private float heatCounter;

    private bool overheated;

    public Gun[] allGuns;
    private int selectedGun;

    public int maxHealth = 100;
    private int currentHealth;

    public Transform groundCheckPoint;
    private bool isGrounded;
    public LayerMask groundLayers;

    public float jumpForce = 12f, gravityMod = 2.5f;

    public GameObject playerHitImpact;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        maxHealth = currentHealth;
        Cursor.lockState = CursorLockMode.Locked;

        cam = Camera.main;

        UiController.instance.weaponTempSlider.maxValue = maxHeat;

        SwitchGun();
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {



            // Mouse input
            mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensitivity;

            // Horizontal (Yaw) rotation
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x, transform.rotation.eulerAngles.z);

            // Vertical (Pitch) rotation
            verticalRotStore -= mouseInput.y;
            verticalRotStore = Mathf.Clamp(verticalRotStore, -60f, 60f);

            // Apply vertical rotation to viewPoint
            viewPoint.localRotation = Quaternion.Euler(verticalRotStore, 0f, 0f);
            moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));


            if (Input.GetKey(KeyCode.LeftShift))
            {
                activeMoveSpeed = runSpeed;
            }
            else
            {
                activeMoveSpeed = moveSpeed;
            }

            float yVel = movement.y;

            movement = ((transform.forward * moveDir.z) + (transform.right * moveDir.x)).normalized * activeMoveSpeed;

            movement.y = yVel;


            if (charCon.isGrounded)
            {
                movement.y = 0f;
            }


            isGrounded = Physics.Raycast(groundCheckPoint.position, Vector3.down, .25f, groundLayers);




            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                movement.y = jumpForce;
            }

            movement.y += Physics.gravity.y * Time.deltaTime * gravityMod;

            charCon.Move(movement * activeMoveSpeed * Time.deltaTime);



            if (allGuns[selectedGun].muzzleFlash.activeInHierarchy)
            {
                muzzleCounter -= Time.deltaTime;


                if (muzzleCounter <= 0)
                {
                    allGuns[selectedGun].muzzleFlash.SetActive(false);
                }
            }



            if (!overheated)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Shoot();
                }

                if (Input.GetMouseButton(0) && allGuns[selectedGun].isAutomatic)
                {
                    shotCounter -= Time.deltaTime;
                    if (shotCounter <= 0)
                    {
                        Shoot();
                    }
                }


            }
            else
            {
                heatCounter -= overHeatCoolRate * Time.deltaTime;
                if (heatCounter <= 0)
                {
                    overheated = false;


                    UiController.instance.overheatedMessage.gameObject.SetActive(false);
                }
            }
            if (heatCounter < 0)
            {
                heatCounter = 0;
            }


            UiController.instance.weaponTempSlider.value = heatCounter;

            if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
            {
                selectedGun++;
                if (selectedGun >= allGuns.Length)

                {
                    selectedGun = 0;
                }
                SwitchGun();

            }

            else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
            {
                selectedGun--;
                if (selectedGun < 0)

                {
                    selectedGun = allGuns.Length - 1;
                }
                SwitchGun();
            }

            for (int i = 0; i < allGuns.Length; i++)
            {
                // Convert i + 1 to a KeyCode (e.g., KeyCode.Alpha1 for 1, KeyCode.Alpha2 for 2, etc.)
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    selectedGun = i;
                    SwitchGun();
                }
            }




            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else if (Cursor.lockState == CursorLockMode.None)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }
        }
    }


    private void Shoot()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(.5f, .5f, 0));
            ray.origin = cam.transform.position;


        if(Physics.Raycast(ray,out RaycastHit hit))
        {
            //Debug.Log("We hit" + hit.collider.gameObject.name);

            if(hit.collider.gameObject.tag == "Player") 
            
            { 
                PhotonNetwork.Instantiate(playerHitImpact.name, hit.point, Quaternion.identity);
                hit.collider.gameObject.GetPhotonView().RPC("DealDamage", RpcTarget.All, photonView.Owner.NickName, allGuns[selectedGun].shotDamage);
            }
            else
            {
                GameObject bulletImpactObject = Instantiate(bulletImpact, hit.point + (hit.normal * .002f), Quaternion.LookRotation(hit.normal, Vector3.up));

                Destroy (bulletImpactObject, 10f );
            }
            
        }


        shotCounter = allGuns[selectedGun].timeBetweenShots ;

        heatCounter += allGuns[selectedGun].heatPerShot;
        if (heatCounter >= maxHeat)
        {
            heatCounter = maxHeat;

            overheated = true;

            UiController.instance.overheatedMessage.gameObject.SetActive(true);
        }

        allGuns[selectedGun].muzzleFlash.SetActive(true);
        muzzleCounter = muzzleDisplayTime;
    }

    [PunRPC]
    public void TakeDamage(string damager, int damageAmount)
    {
        if (photonView.IsMine)
        {
            //Debug.Log(photonView.Owner.NickName + "I've been hit" + damager);

            currentHealth -= damageAmount;

            if (currentHealth <= 0)
            {
                currentHealth = 0;

                PlayerSpawner.Instance.Die(damager);
            }
        }


    }

    public void DealDamage(string damager, int damageAmount)
    {
        TakeDamage(damager, damageAmount);
    }

    private void LateUpdate()
    {
        cam.transform.position = viewPoint.position;
        cam.transform.rotation = viewPoint.rotation;
    }

    void SwitchGun()
    {
        foreach(Gun gun in allGuns)
        {
            gun.gameObject.SetActive (false);
        }

        allGuns[selectedGun].gameObject.SetActive(true);

        allGuns[selectedGun].muzzleFlash.SetActive(false);
    }
}
