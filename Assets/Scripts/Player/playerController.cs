using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class playerController : MonoBehaviour
{
    [Header("----- Components -----")] [SerializeField]
    CharacterController controller;

    [Header("----- Player Stats -----")] [Range(1, 100)] [SerializeField]
    int HP;

    private Coroutine staminaRegen;

    [Range(1, 100)] [SerializeField] float stamina;
    [SerializeField] int playerSpeed;
    [SerializeField] int jumpVelocity;
    [SerializeField] int gravity;
    [SerializeField] int jumpMax;
    [SerializeField] int coins;
    [Range(0.01f, 5)] [SerializeField] float actionRange;

    [Header("----- Shooting -----")]
    public Weapon weapons;
    int currentWeapon;

    public GameObject viewModel;
    public int ammo;
    int jumpTimes;
    Vector3 move;
    Vector3 playerVelocity;

    [Header("----- Weapon Slots -----")] public Weapon _currentWeaponSlot;

    int HPOrig;
    float staminaOrig;

    bool isShooting;
    bool isReloading;
    bool staminaLeft;
    bool isAttacking;

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

        if (!isAttacking && Input.GetButton("Shoot"))
        {
            Attack();
        }
        
        if (!isReloading && Input.GetButtonDown("Reload"))
        {
            StartCoroutine(reload());
        }
        
        // if (!isShooting && Input.GetButton("Shoot") && gameManager.instance.currWeapon != null) //weapons[currentWeapon] != null
        // {
        //     if (!isReloading && gameManager.instance.currWeapon.ammoRemaining > 0) //weapons[currentWeapon].ammoRemaining > 0
        //     {
        //         StartCoroutine(shoot());
        //     }
        //     else if (!isReloading && gameManager.instance.currWeapon.ammoRemaining <= 0)  //weapons[currentWeapon].ammoRemaining <= 0
        //     {
        //         StartCoroutine(reload());
        //     }
        // }
        //
        // if (!isReloading && Input.GetButtonDown("Reload"))
        // {
        //     StartCoroutine(reload());
        // }
        //
        // if (Input.GetButtonDown("Pause"))
        // {
        //     gameManager.instance.pauseGame();
        // }
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

        // if (Input.GetButtonDown("Weapon1"))
        // {
        //     changeWeapon(0);
        // }
        //
        // if (Input.GetButtonDown("Weapon2"))
        // {
        //     changeWeapon(1);
        // }
        //
        // if (Input.GetButtonDown("Weapon3"))
        // {
        //     changeWeapon(2);
        // }

        //add gravity
        playerVelocity.y -= gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    IEnumerator shoot()
    {
        isAttacking = true;
        RaycastHit hit;
        
        gameManager.instance.UpdateUI();
        ammo = gameManager.instance.GunSlots[0].ammoRemaining;

        

       
        if (gameManager.instance.GunSlots[0].ammoRemaining > 0)
        {
            gameManager.instance.updateAmmo(1);

            if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit,
                    gameManager.instance.GunSlots[0].range))
            {
                if (hit.collider.GetComponent<IDamage>() != null)
                {
                    hit.collider.GetComponent<IDamage>().takeDamage(gameManager.instance.GunSlots[0].attack);
                }

                if (hit.rigidbody != null)
                {
                    hit.rigidbody.AddForceAtPosition(transform.forward * gameManager.instance.GunSlots[0].shootForce,
                        hit.point);
                }
            }
        }
        else
        {
            StartCoroutine(reload());
        }

        yield return new WaitForSeconds(gameManager.instance.GunSlots[0].shootRate);
        isAttacking = false;
    }

    IEnumerator reload()
     {
         isReloading = true;
         yield return new WaitForSeconds(gameManager.instance.GunSlots[0].reloadTime);
         gameManager.instance.GunSlots[0].ammoRemaining = gameManager.instance.GunSlots[0].ammoCapacity;
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

    public int GetCoins()
    {
        return coins;
    }

    public int getHP()
    {
        return HP;
    }

    public float getStamina()
    {
        return stamina;
    }

    public void addCoins(int amount)
    {
        coins += amount;
    }

    // public void changeWeapon(int weapon)
    // {
    //     if (weapons[weapon] != null)
    //     {
    //         currentWeapon = weapon;
    //         viewModel.GetComponent<MeshFilter>().mesh = weapons[weapon].viewModel;
    //     }
    // }

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

    public void Attack()
    {
        if (gameManager.instance.currWeapon is RangedWeapons)
        {
            Debug.Log("Call Attack With Range Weapon");
            
            StartCoroutine(shoot());
        }
    }
    
}