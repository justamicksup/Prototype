using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class playerController : MonoBehaviour
{
    [Header("----- Components -----")] [SerializeField]
    CharacterController controller;
    [SerializeField] AudioSource aud;
    [SerializeField] Animator animator;
    
    [Header("----- Player Stats -----")] [Range(1, 100)] [SerializeField]
    int HP;

    private Coroutine staminaRegen;
    [Range(1, 100)] [SerializeField] public float stamina;
    [SerializeField] public int playerSpeed;
    [SerializeField] int jumpVelocity;
    [SerializeField] int gravity;
    [SerializeField] int jumpMax;
    [SerializeField] int coins;
    [Range(0.01f, 5)] [SerializeField] float actionRange;
    public int ammo;
    int jumpTimes;
    Vector3 move;
    Vector3 playerVelocity;

    [Header("----- Audio -----")] [SerializeField]
    AudioClip[] audPlayerDamage;
    [Range(0, 1)] [SerializeField] float audPlayerDamageVol;
    [SerializeField] AudioClip[] audPlayerJump;
    [Range(0, 1)] [SerializeField] float audPlayerJumpVol;
    [SerializeField] AudioClip[] audPlayerSteps;
    [Range(0, 1)] [SerializeField] float audPlayerStepsVol;

    [Header("----- Gun Stats -----")]
    
    [SerializeField] int gunLevel;
    [SerializeField] public int shootDamage;
    [SerializeField] int range;
    [SerializeField] float shootRate;
    [SerializeField] float shootForce;
    [SerializeField] public int ammoCapacity;
    [SerializeField] public int ammoRemaining;
    [SerializeField] float reloadTime;

    [Header("----- Melee Stats -----")] 
    
    [SerializeField] int meleeLevel;
    [SerializeField] public int meleeDamage;
    [SerializeField] int meleeReach;
    [SerializeField] float knockbackForce;
    [SerializeField] float swingSpeed;


    //public GameObject viewModel;

    [Header("----- Player Info -----")] [Header("----- Weapon Slots -----")] 
    [SerializeField] public List<MasterWeapon> weaponList = new List<MasterWeapon>();

    [SerializeField] public int currentWeapon;

  //[SerializeField] List<ProjectileWeaponScriptableObjects> gunList = new List<ProjectileWeaponScriptableObjects>();
    
    //[SerializeField] List<MeleeWeaponScriptableObjects> meleeList = new List<MeleeWeaponScriptableObjects>();

 

    [SerializeField] private List<GameObject> WeaponSlots;
    
    
    

    int HPOrig;
    float staminaOrig;
    public Vector3 pushBack;
    [SerializeField] int pushBackTime;
    
    

    bool isShooting;
    bool isReloading;
    bool staminaLeft;
    bool isAttacking;
    public bool inActionRange;
    bool isPlayingSteps;
    bool isSprinting;

    // Start is called before the first frame update
    void Start()
    {
        HPOrig = HP;
        staminaOrig = stamina;
        updatePlayerHP();
        updatePlayerStamina();
        WeaponSlots[0].SetActive(false);
        WeaponSlots[1].SetActive(false);
        WeaponSlots[2].SetActive(false);
        currentWeapon = 0;
    }

    // Update is called once per frame
    void Update()
    {
      
        //Code to turn on if we use the pirate player with animation
         animator.SetFloat("Speed", move.normalized.magnitude);

        if (Input.GetKey(KeyCode.B))
        {
            animator.SetTrigger("MeleeAttack");
        }
        if (move.normalized.magnitude > 0.3f && !isPlayingSteps)
            StartCoroutine(playSteps());
        movement();

        if (weaponList.Count > 0)
        {
            if (!isAttacking && Input.GetButton("Shoot"))
            {
                Attack();
            }

            if (!isReloading && Input.GetButtonDown("Reload"))
            {
                //StartCoroutine(reload(projectileWeaponScriptableObjects));
            }
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
            isSprinting = true;
            controller.Move(move * Time.deltaTime * playerSpeed * 2);
            useStamina(0.5f);
        }
        else
        {
            isSprinting = false;
            controller.Move(move * Time.deltaTime * playerSpeed);
        }

        //jump
        if (Input.GetButtonDown("Jump") && jumpTimes < jumpMax)
        {
            playerVelocity.y = jumpVelocity;
            jumpTimes++;
            aud.PlayOneShot(audPlayerJump[Random.Range(0, audPlayerJump.Length)], audPlayerJumpVol);
        }

        if (Input.GetButtonDown("Weapon1"))
        {
            if(weaponList.Count > 0 && weaponList[0] != null)
            {
                changeWeapon(0);
                try
                {
                    SetGunStats((ProjectileWeaponScriptableObjects)weaponList[0], 0);
                }
                catch(InvalidCastException)
                {
                    SetMeleeStats((MeleeWeaponScriptableObjects)weaponList[0], 0);
                }
                
            }       
        }

        if (Input.GetButtonDown("Weapon2"))
        {
            if (weaponList.Count >= 1 && weaponList[1] != null)
            {
                changeWeapon(1);
                try
                {
                    SetGunStats((ProjectileWeaponScriptableObjects)weaponList[1], 1);
                }
                catch (InvalidCastException)
                {
                    SetMeleeStats((MeleeWeaponScriptableObjects)weaponList[1], 1);
                }
            }
        }

        if (Input.GetButtonDown("Weapon3"))
        {
            if (weaponList.Count >= 2 && weaponList[2] != null)
            {
                changeWeapon(2);
                try
                {
                    SetGunStats((ProjectileWeaponScriptableObjects)weaponList[2], 2);
                }
                catch (InvalidCastException)
                {
                    SetMeleeStats((MeleeWeaponScriptableObjects)weaponList[2], 2);
                }
            }
        }
        if(Input.GetButtonDown("Reload"))
        {
            StartCoroutine(reload((ProjectileWeaponScriptableObjects)weaponList[currentWeapon]));
        }

        //add gravity
        playerVelocity.y -= gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    IEnumerator shoot(ProjectileWeaponScriptableObjects projectileWeaponScriptableObjects)
    {
        isAttacking = true;
        RaycastHit hit;

        gameManager.instance.UpdateUI();
        ammo = projectileWeaponScriptableObjects.ammoRemaining;
        ammoRemaining = ammo;

        if (projectileWeaponScriptableObjects.ammoRemaining > 0 && !isReloading)
        {
            
            projectileWeaponScriptableObjects.ammoRemaining -= 1;
            if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit,
                    projectileWeaponScriptableObjects.range))
            {
                if (hit.collider.GetComponent<IDamage>() != null)
                {
                    hit.collider.GetComponent<IDamage>().takeDamage(projectileWeaponScriptableObjects.shootDamage);
                }

                if (hit.rigidbody != null)
                {
                    hit.rigidbody.AddForceAtPosition(transform.forward * projectileWeaponScriptableObjects.shootForce,
                        hit.point);
                }
            }
        }
        else
        {
            StartCoroutine(reload(projectileWeaponScriptableObjects));
        }

        yield return new WaitForSeconds(projectileWeaponScriptableObjects.shootRate);
        isAttacking = false;
    }

    IEnumerator reload(ProjectileWeaponScriptableObjects projectileWeaponScriptableObjects)
    {
        isReloading = true;
        yield return new WaitForSeconds(projectileWeaponScriptableObjects.reloadTime);
        projectileWeaponScriptableObjects.ammoRemaining = projectileWeaponScriptableObjects.ammoCapacity;
        isReloading = false;
    }

    
    public void takeDamage(int damage)
    {
        HP -= damage;
        updatePlayerHP();
        StartCoroutine(gameManager.instance.flash());
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

    public void changeWeapon(int weapon)
    {
        if (weaponList.Count > weapon)
        {
            Debug.Log("Correct number");
            WeaponSlots[currentWeapon].SetActive(false);
            WeaponSlots[weapon].SetActive(true);
            currentWeapon = weapon;
        }
        else
        {
            Debug.Log("bad math");
        }
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

    public void Attack()
    {
        // if no weapons
        //show no weapon dialog to player

        //if current weapon is a gun
        if (weaponList[currentWeapon].GetType() == typeof(ProjectileWeaponScriptableObjects))
        {
            //call shoot logic
            Debug.Log("Call Shoot attack");
            StartCoroutine(shoot((ProjectileWeaponScriptableObjects)weaponList[currentWeapon]));
           
        }
        // else if current weapon is a melee
        else if (weaponList[currentWeapon].GetType() == typeof(MeleeWeaponScriptableObjects))
        {
            Debug.Log("Call Melee attack");
            //call melee logic
        }

        //if current weapon is a explosive
        // call explosive logic

        //if current weapon is a heal
        // call heal logic
    }

    public void powerPickup(PowerStat power)
    {
        if (power.speedBonus != 0)
        {
            playerSpeed += power.speedBonus;
        }
        if (power.staminaBonus != 0)
        {
            stamina += power.staminaBonus;
        }
        if (power.shootDmgBonus != 0)
        {
            shootDamage += power.shootDmgBonus;
        }
        if (power.meleeDmgBonus != 0)
        {
            meleeDamage += power.meleeDmgBonus;
        }
        if (power.goldBonus != 0)
        {
            coins+= power.goldBonus;
        }
    }


    // public void gunPickup(ProjectileWeaponScriptableObjects projectileWeaponScriptableObjects)
    // {
    //     gunList.Add(projectileWeaponScriptableObjects);
    //
    //     gunLevel = projectileWeaponScriptableObjects.gunLevel;
    //     shootDamage = projectileWeaponScriptableObjects.shootDamage;
    //     range = projectileWeaponScriptableObjects.range;
    //     shootRate = projectileWeaponScriptableObjects.shootRate;
    //     shootForce = projectileWeaponScriptableObjects.shootForce;
    //     ammoCapacity = projectileWeaponScriptableObjects.ammoCapacity;
    //     ammoRemaining = projectileWeaponScriptableObjects.ammoRemaining;
    //     reloadTime = projectileWeaponScriptableObjects.reloadTime;
    //
    //     gunModel.GetComponent<MeshFilter>().sharedMesh =
    //         projectileWeaponScriptableObjects.gunModel.GetComponent<MeshFilter>().sharedMesh;
    //     
    //      gunModel.GetComponent<MeshRenderer>().sharedMaterial =
    //          projectileWeaponScriptableObjects.gunModel.GetComponent<MeshRenderer>().sharedMaterial;
    // }

    public void SetGunStats(ProjectileWeaponScriptableObjects projectileWeaponScriptableObjects, int index)
    {
        //gunList.Add(projectileWeaponScriptableObjects);

        gunLevel = projectileWeaponScriptableObjects.gunLevel;
        shootDamage = projectileWeaponScriptableObjects.shootDamage;
        range = projectileWeaponScriptableObjects.range;
        shootRate = projectileWeaponScriptableObjects.shootRate;
        shootForce = projectileWeaponScriptableObjects.shootForce;
        ammoCapacity = projectileWeaponScriptableObjects.ammoCapacity;
        ammoRemaining = projectileWeaponScriptableObjects.ammoRemaining;
        reloadTime = projectileWeaponScriptableObjects.reloadTime;


        WeaponSlots[index].GetComponent<MeshFilter>().sharedMesh =
            projectileWeaponScriptableObjects.Model.GetComponent<MeshFilter>().sharedMesh;

        WeaponSlots[index].GetComponent<MeshRenderer>().sharedMaterials =
            projectileWeaponScriptableObjects.Model.GetComponent<MeshRenderer>().sharedMaterials;
        
        WeaponSlots[index].transform.localScale = projectileWeaponScriptableObjects.Model.transform.localScale;
        WeaponSlots[index].transform.localRotation = projectileWeaponScriptableObjects.Model.transform.rotation;

       
        ammo = projectileWeaponScriptableObjects.ammoRemaining;
    }

    public void SetMeleeStats(MeleeWeaponScriptableObjects meleeWeaponScriptableObjects, int index)
    {
        //meleeList.Add(meleeWeaponScriptableObjects);

        meleeLevel = meleeWeaponScriptableObjects.meleeLevel;
        meleeReach = meleeWeaponScriptableObjects.meleeDamage;
        knockbackForce = meleeWeaponScriptableObjects.knockbackForce;
        swingSpeed = meleeWeaponScriptableObjects.swingSpeed;


        WeaponSlots[index].GetComponent<MeshFilter>().sharedMesh =
            meleeWeaponScriptableObjects.Model.GetComponent<MeshFilter>().sharedMesh;

        WeaponSlots[index].GetComponent<MeshRenderer>().sharedMaterials =
            meleeWeaponScriptableObjects.Model.GetComponent<MeshRenderer>().sharedMaterials;

        WeaponSlots[index].transform.localScale = meleeWeaponScriptableObjects.Model.transform.localScale;
        WeaponSlots[index].transform.localRotation = meleeWeaponScriptableObjects.Model.transform.rotation;
       
    }

    public void GetWeaponType(MasterWeapon tempArmoryListOfWeapon, int index)
    {
        if (tempArmoryListOfWeapon.GetType() == typeof(ProjectileWeaponScriptableObjects))
        {
            Debug.Log("Projectile");
            SetGunStats((ProjectileWeaponScriptableObjects)tempArmoryListOfWeapon, index);
        }

        else if (tempArmoryListOfWeapon.GetType() == typeof(MeleeWeaponScriptableObjects))
        {
            Debug.Log("Melee");
            SetMeleeStats((MeleeWeaponScriptableObjects)tempArmoryListOfWeapon, index);
        }
    }


    public void WeaponPickup(MasterWeapon masterWeapon)
    {
        AddWeaponToInventory(masterWeapon);
    }

    public void AddWeaponToInventory(MasterWeapon tempArmoryListOfWeapon)
    {
        //if no weapons in list 
        if (weaponList.Count == 0)
        {
            weaponList.Add(tempArmoryListOfWeapon);
            currentWeapon = 0;
            GetWeaponType(tempArmoryListOfWeapon, 0);
            WeaponSlots[currentWeapon].SetActive(true);
        } //if weapon inventory is full
        else if (weaponList.Count == 3)
        {
            weaponList[currentWeapon] = tempArmoryListOfWeapon;
            GetWeaponType(tempArmoryListOfWeapon, currentWeapon);
        } //if weapon inventory is over max
        else if (weaponList.Count > 3)
        {
            Debug.Log("Too Many Weapons in AddWeaponToInventory");
        } //if weapon inventory is in between
        else
        {
            weaponList.Add(tempArmoryListOfWeapon);
            WeaponSlots[currentWeapon].SetActive(false);
            currentWeapon += 1;
            GetWeaponType(tempArmoryListOfWeapon, currentWeapon);
            WeaponSlots[currentWeapon].SetActive(true);
        }
    }
    
    IEnumerator playSteps()
    {
        if (controller.isGrounded)
        {
            isPlayingSteps = true;
            aud.PlayOneShot(audPlayerSteps[Random.Range(0, audPlayerSteps.Length)], audPlayerStepsVol);
            if (!isSprinting)
            {
                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                yield return new WaitForSeconds(0.3f);
            }

            isPlayingSteps = false;
        }
        
    }
    
}