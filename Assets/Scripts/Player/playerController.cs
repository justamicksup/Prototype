using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class playerController : MonoBehaviour
{
    [Header("----- Components -----")] [SerializeField]
    CharacterController controller;

    [Header("----- Player Stats -----")] 
    [Range(1, 100)] [SerializeField] int HP;
    [Range(1, 100)] [SerializeField] float stamina;
    [SerializeField] int playerSpeed;
    [SerializeField] int jumpVelocity;
    [SerializeField] int gravity;
    [SerializeField] int jumpMax;
    [SerializeField] int coins;
    [Range(0.01f, 5)] [SerializeField] float actionRange;
    private Coroutine staminaRegen;

    [Header("----- Shooting -----")]
    [SerializeField] List<RangedWeapon> weaponList = new List<RangedWeapon>(3);
    [SerializeField] GameObject gunModel;
    [SerializeField] float shootRate;
    [SerializeField] int shootDist;
    [SerializeField] int shootDmg;
    [SerializeField] float reloadSpeed;
    [SerializeField] float shootForce;
    public int magSize; //make private later
    public int ammoRemaining;
    public int gunAmmo;
    public int gunAmmoCap;//make private later

    [Header("----- Melee -----")]
    [SerializeField] float attackRate;//to determine how many attacks per second
    [SerializeField] float swingRate; //to determine how fast the swing is (not sure if we'll need)

    int jumpTimes;
    Vector3 move;
    Vector3 playerVelocity;

    [Header("----- Weapon Slots -----")] 
    //public Weapon _currentWeaponSlot;

    int HPOrig;
    float staminaOrig;

    bool isShooting;
    bool isReloading;
    bool staminaLeft;
    public bool inActionRange;

    // Start is called before the first frame update
    void Start()
    {
        HPOrig = HP;
        staminaOrig = stamina;
        updatePlayerHP();
        updatePlayerStamina();
       
    }

    // Update is called once per frame
    void Update()
    {
        movement();

        if (weaponList.Count > 0 && Input.GetButton("Shoot"))
        {
            if (ammoRemaining > 0 && !isShooting)
            {
                StartCoroutine(shoot());
            }
            else
                StartCoroutine(reload());
        }
        if (!isReloading && Input.GetButtonDown("Reload") &&
            weaponList.Count > 0 && ammoRemaining != magSize)
        {
            StartCoroutine(reload());
        }
        if (gameManager.instance.actionObject != null 
            && Input.GetButtonDown("Interact") && inActionRange) //assigned to 'e'
        {
            gameManager.instance.actionObject.GetComponent<actionObject>().primaryAction();
        }
        if (gameManager.instance.actionObject != null
            && Input.GetButtonDown("Submit") && inActionRange) //assigned to 'enter'
        {
            gameManager.instance.actionObject.GetComponent<actionObject>().secondaryAction();
        }
    }

    void movement()
    {
        //check if on ground
        if (controller.isGrounded)
        {
            //reset jump and velocity
            jumpTimes = 0;
            playerVelocity.y = 0;
        }

        //get input
        move = (transform.right * Input.GetAxis("Horizontal")) +
               (transform.forward * Input.GetAxis("Vertical"));
        //move character
        if (Input.GetButton("Sprint") && stamina > 0)
        {
            controller.Move(move * Time.deltaTime * playerSpeed * 2);
            useStamina(0.5f);
        }
        else
        {
            controller.Move(move * Time.deltaTime * playerSpeed);
        }

        //jump
        if (Input.GetButtonDown("Jump") && jumpTimes < jumpMax)
        {
            playerVelocity.y = jumpVelocity;
            jumpTimes++;
        }

        //add gravity
        playerVelocity.y -= gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    IEnumerator shoot()
    {
        isShooting = true;
        RaycastHit hit;
        
        ammoRemaining--;
        gameManager.instance.updateAmmoUI();
       
        if (ammoRemaining > 0)
        {
            if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDist))
            {
                if (hit.collider.GetComponent<IDamage>() != null)
                {
                    hit.collider.GetComponent<IDamage>().takeDamage(shootDmg);
                }

                if (hit.rigidbody != null)
                {
                    hit.rigidbody.AddForceAtPosition(transform.forward * shootForce, hit.point);
                }
            }
        }
        else
        {
            StartCoroutine(reload());
        }

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    IEnumerator reload()
    {
        isReloading = true;
        if (magSize <= gunAmmo)
        {
            gunAmmo -= magSize;
            ammoRemaining += magSize;
        }
        else
        {
            int temp = gunAmmo;
            gunAmmo = 0;
            ammoRemaining += temp;
        }
        gameManager.instance.updateAmmoUI();

        //play reload anim

        yield return new WaitForSeconds(reloadSpeed);
        isReloading = false;
    }

    public void takeDamage(int damage)
    {
        HP -= damage;
        updatePlayerHP();
        if (HP <= 0)
        {
            gameManager.instance.youLose();
        }
    }
    public void useStamina(float energy)
    {
        if (stamina - energy >= 0)
        {
            stamina -= energy;
            updatePlayerStamina();
        }

        if (staminaRegen != null)
        {
            StopCoroutine(regainStamina());
        }

        staminaRegen = StartCoroutine(regainStamina());
    }
    IEnumerator regainStamina()
    {
        yield return new WaitForSeconds(2);

        while (stamina < staminaOrig && !Input.GetButtonDown("Sprint"))
        {
            stamina += staminaOrig / 100;
            updatePlayerStamina();
            yield return new WaitForSeconds(.1f);
        }

        staminaRegen = null;
    }
    public int GetCoins() { return coins; }
    public void addCoins(int amount)
    {
        coins += amount;
        gameManager.instance.updateCoinUI();
    }
    public void updatePlayerHP()
    {
        gameManager.instance.playerHPBar.fillAmount = (float)HP / (float)HPOrig;
    }
    public void updatePlayerStamina()
    {
        gameManager.instance.playerStaminaBar.fillAmount = (float)stamina / (float)staminaOrig;
    }
    public void respawnPlayer()
    {
        controller.enabled = false;
        transform.position = gameManager.instance.playerSpawnPos.transform.position;
        controller.enabled = true;
    }

    public void weaponPickup(RangedWeapon weapon)
    {
        weaponList.Add(weapon);

        shootRate = weapon.shootRate;
        shootDist = weapon.shootDist;
        shootDmg = weapon.shootDmg;
        reloadSpeed = weapon.reloadSpeed;
        shootForce = weapon.shootForce;
        magSize = weapon.magSize;
        ammoRemaining = weapon.ammoRemaining;
        gunAmmo = weapon.gunAmmo;
        gunAmmoCap = weapon.gunAmmoCap;

        gunModel.GetComponent<MeshFilter>().sharedMesh = weapon.gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = weapon.gunModel.GetComponent<MeshRenderer>().sharedMaterial;
    }
}