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
    [Header("----- Components -----")]
    [SerializeField]
    CharacterController controller;
    [SerializeField] AudioSource aud;
    [SerializeField] Animator animator;
    [SerializeField] private Animation anim;


    [Header("----- Player Stats -----")]
    [Range(1, 100)]
    [SerializeField]
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

    [Header("----- Audio -----")]
    [SerializeField] AudioClip[] audPlayerDamage;
    [Range(0, 1)] [SerializeField] float audPlayerDamageVol;
    [SerializeField] AudioClip[] audPlayerJump;
    [Range(0, 1)] [SerializeField] float audPlayerJumpVol;
    [SerializeField] AudioClip[] audPlayerSteps;
    [Range(0, 1)] [SerializeField] float audPlayerStepsVol;
    [SerializeField] AudioClip audReload;
    [Range(0, 1)] [SerializeField] float audReloadVol;

    [Header("----- Gun Stats -----")]

    [SerializeField] int gunLevel;
    [SerializeField] public int shootDamage;
    [SerializeField] int range;
    [SerializeField] float shootRate;
    [SerializeField] float shootForce;
    [SerializeField] public int ammoRemaining;
    [SerializeField] public int maxAmmo;
    //[SerializeField] public int weaponList[currentWeapon].currentClip;
    [SerializeField] float reloadTime;
    [SerializeField] GameObject bullet;
    [SerializeField] public int bulletSpeed;
    public Transform muzzle;


    [Header("----- Melee Stats -----")]

    [SerializeField] int meleeLevel;
    [SerializeField] public int meleeDamage;
    [SerializeField] int meleeReach;
    [SerializeField] float knockbackForce;
    [SerializeField] float swingSpeed;
    private bool canMeleeAttack = true;
    [SerializeField] private LayerMask meleeMask;


    //public GameObject viewModel;

    [Header("----- Player Info -----")]
    [Header("----- Weapon Slots -----")]
    //[SerializeField] public List<MasterWeapon> weaponList = new List<MasterWeapon>();
    public List<Weapon> weaponList = new List<Weapon>();

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
        respawnPlayer();
        WeaponSlots[0].SetActive(false);
        WeaponSlots[1].SetActive(false);
        WeaponSlots[2].SetActive(false);
        currentWeapon = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //Code to turn on if we use the pirate player with animation
        //  animator.SetFloat("Speed", move.normalized.magnitude);
        //
        //
        // if (Input.GetKey(KeyCode.B))
        // {
        //     animator.SetTrigger("MeleeAttack");
        // }

        //
        // if (Input.GetKey(KeyCode.B))
        // {
        //     animator.SetTrigger("MeleeAttack");
        // }

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
                StartCoroutine(reload((ProjectileWeaponScriptableObjects)weaponList[currentWeapon].weapon));
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
            if (weaponList.Count > 0 && weaponList[0].weapon != null)
            {
                changeWeapon(0);
                try
                {
                    SetGunStats((ProjectileWeaponScriptableObjects)weaponList[0].weapon, 0, false);
                }
                catch (InvalidCastException)
                {
                    SetMeleeStats((MeleeWeaponScriptableObjects)weaponList[0].weapon, 0);
                }
            }
        }

        if (Input.GetButtonDown("Weapon2"))
        {
            if (weaponList.Count > 1 && weaponList[1].weapon != null)
            {
                changeWeapon(1);
                try
                {
                    SetGunStats((ProjectileWeaponScriptableObjects)weaponList[1].weapon, 1, false);
                }
                catch (InvalidCastException)
                {
                    SetMeleeStats((MeleeWeaponScriptableObjects)weaponList[1].weapon, 1);
                }
            }
        }

        if (Input.GetButtonDown("Weapon3"))
        {
            if (weaponList.Count > 2 && weaponList[2].weapon != null)
            {
                changeWeapon(2);
                try
                {
                    SetGunStats((ProjectileWeaponScriptableObjects)weaponList[2].weapon, 2, false);
                }
                catch (InvalidCastException)
                {
                    SetMeleeStats((MeleeWeaponScriptableObjects)weaponList[2].weapon, 2);
                }
            }
        }

        //if (Input.GetButtonDown("Reload") && currentWeapon > 0)
        //{
        //    StartCoroutine(reload((ProjectileWeaponScriptableObjects)weaponList[currentWeapon]));
        //}

        //add gravity
        playerVelocity.y -= gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    public void AddAmmo(int amount)
    {
        ammoRemaining += amount;
    }

    IEnumerator shoot(ProjectileWeaponScriptableObjects projectileWeaponScriptableObjects)
    {

        isAttacking = true;
        RaycastHit hit;
        //gameManager.instance.UpdateUI();
        //ammo = projectileWeaponScriptableObjects.ammoRemaining;
        //weaponList[currentWeapon].currentClip = ammo;

        if (weaponList[currentWeapon].currentClip > 0 && !isReloading) //projectileWeaponScriptableObjects.ammoRemaining > 0 && !isReloading)
        {

            aud.PlayOneShot(projectileWeaponScriptableObjects.audGunShot,
                projectileWeaponScriptableObjects.audGunShotVol);

            //projectileWeaponScriptableObjects.ammoRemaining -= 1;
            weaponList[currentWeapon].currentClip -= projectileWeaponScriptableObjects.ammoCapacity;
            gameManager.instance.updateAmmoUI();
            // if bullet prefab is a collection of bullets 
            if (bullet.transform.childCount > 0)
            {
                for (int i = 0; i < bullet.transform.childCount; i++)
                {
                    GameObject BuckShot = Instantiate(bullet, muzzle.position, bullet.transform.rotation);
                    Rigidbody[] buckShotRigidbodies = BuckShot.GetComponentsInChildren<Rigidbody>();
                    foreach (Rigidbody buckShotRigidbody in buckShotRigidbodies)
                    {

                        buckShotRigidbody.velocity = Camera.main.transform.forward * bulletSpeed;
                        buckShotRigidbody.GetComponent<bullet>().bulletDamage = shootDamage;
                    }
                }
            } // regular single bullet weapon
            else
            {
                GameObject bulletClone = Instantiate(bullet, muzzle.position, bullet.transform.rotation);
                bulletClone.GetComponent<Rigidbody>().velocity =
                  Camera.main.transform.forward * bulletSpeed;
                bulletClone.GetComponent<bullet>().bulletDamage = shootDamage;
            }



            if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit,
                    projectileWeaponScriptableObjects.range))
            {

                if (hit.collider.GetComponent<IDamage>() != null)
                {
                    hit.collider.GetComponent<IDamage>().takeDamage(shootDamage);
                }

                if (hit.rigidbody != null)
                {
                    hit.rigidbody.AddForceAtPosition(
                        transform.forward * projectileWeaponScriptableObjects.shootForce,
                        hit.point);
                }
            }

            yield return new WaitForSeconds(projectileWeaponScriptableObjects.shootRate);
            StartCoroutine(reload((ProjectileWeaponScriptableObjects)weaponList[currentWeapon].weapon));
        }
        isAttacking = false;
        //else
        //{
        //    StartCoroutine(reload(projectileWeaponScriptableObjects));
        //}
    }

    IEnumerator reload(ProjectileWeaponScriptableObjects projectileWeaponScriptableObjects)
    {
        isReloading = true;

        if (ammoRemaining >= projectileWeaponScriptableObjects.ammoCapacity)
        {
            yield return new WaitForSeconds(projectileWeaponScriptableObjects.reloadTime);
            //projectileWeaponScriptableObjects.ammoRemaining = projectileWeaponScriptableObjects.ammoCapacity;
            weaponList[currentWeapon].currentClip = projectileWeaponScriptableObjects.ammoCapacity;
            aud.PlayOneShot(audReload, audReloadVol);
            ammoRemaining -= projectileWeaponScriptableObjects.ammoCapacity;
            gameManager.instance.updateAmmoUI();
        }
        isReloading = false;
    }


    public void takeDamage(int damage)
    {
        HP -= damage;
        updatePlayerHP();
        StartCoroutine(gameManager.instance.flash());
        aud.PlayOneShot(audPlayerDamage[Random.Range(0, audPlayerDamage.Length)], audPlayerDamageVol);
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
        gameManager.instance.updateCoinUI();
    }

    public void changeWeapon(int weapon)
    {
        if (weaponList.Count > weapon)
        {
            //Debug.Log("Correct number");
            WeaponSlots[currentWeapon].SetActive(false);
            WeaponSlots[weapon].SetActive(true);
            currentWeapon = weapon;
            //change to infinity if melee weapon
            //Debug.Log(weaponList[currentWeapon].isGun);
            if (weaponList[currentWeapon].isGun)
                gameManager.instance.updateAmmoUI(true);
            else
                gameManager.instance.updateAmmoUI(false);
        }
        else
        {
            //Debug.Log("bad math");
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
        if (weaponList[currentWeapon].isGun)
        {
            //call shoot logic
            //Debug.Log("Call Shoot attack");
            StartCoroutine(shoot((ProjectileWeaponScriptableObjects)weaponList[currentWeapon].weapon));
        }
        // else if current weapon is a melee
        else if (!weaponList[currentWeapon].isGun && canMeleeAttack)
        {
            //Debug.Log("Call Melee attack");
            StartCoroutine(MeleeAttack());
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
            gameManager.instance.speedBoostIcon.SetActive(true);
        }
        if (power.staminaBonus != 0)
        {
            stamina += power.staminaBonus;
        }
        if (power.shootDmgBonus != 0)
        {
            shootDamage += power.shootDmgBonus;
            gameManager.instance.instaKillIcon.SetActive(true);
        }
        if (power.meleeDmgBonus != 0)
        {
            meleeDamage += power.meleeDmgBonus;
            gameManager.instance.instaKillIcon.SetActive(true);
        }
        if (power.goldBonus != 0)
        {
            addCoins(power.goldBonus);
        }
        if (power.healthBonus != 0)
        {
            StartCoroutine(healOverTime(power.effectDuration, power.healthBonus));
            gameManager.instance.healingIcon.SetActive(true);
        }
        if (power.ammoBonus != 0)
        {
            int temp = maxAmmo - ammoRemaining;

            if (temp <= 0)
            {
                ammoRemaining = maxAmmo;
            }
            else if (temp > power.ammoBonus)
            {
                AddAmmo(temp);
            }
            else
            {
                AddAmmo(power.ammoBonus);
            }
            gameManager.instance.updateAmmoUI();
        }
    }

    IEnumerator MeleeAttack()
    {
        anim.Play();
        canMeleeAttack = false;
        Collider[] cols = Physics.OverlapSphere(transform.position, meleeReach, meleeMask);
        //RaycastHit[] hits = Physics.SphereCastAll(transform.position, meleeReach, transform.forward, meleeMask);
        for (int i = 0; i < cols.Length; i++)
        {
            IDamage enemy = cols[i].GetComponent<IDamage>();
            if (enemy != null)
            {
                enemy.takeDamage(meleeDamage);
            }
        }
        yield return new WaitForSeconds(0.5f);
        anim.Stop();
        canMeleeAttack = true;
        yield break;
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

    public void SetGunStats(ProjectileWeaponScriptableObjects projectileWeaponScriptableObjects, int index, bool newWeapon = true)
    {
        //gunList.Add(projectileWeaponScriptableObjects);

        gunLevel = projectileWeaponScriptableObjects.gunLevel;
        shootDamage = projectileWeaponScriptableObjects.damage;
        range = projectileWeaponScriptableObjects.range;
        shootRate = projectileWeaponScriptableObjects.shootRate;
        shootForce = projectileWeaponScriptableObjects.shootForce;
        //ammoRemaining = projectileWeaponScriptableObjects.ammoCapacity;
        //weaponList[currentWeapon].currentClip = projectileWeaponScriptableObjects.ammoRemaining;
        if (newWeapon)
            weaponList[currentWeapon].currentClip = 0;
        reloadTime = projectileWeaponScriptableObjects.reloadTime;

        if (projectileWeaponScriptableObjects.bullet != null)
        {
            bullet = projectileWeaponScriptableObjects.bullet;
            bulletSpeed = projectileWeaponScriptableObjects.bulletSpeed;
        }


        WeaponSlots[index].GetComponent<MeshFilter>().sharedMesh =
            projectileWeaponScriptableObjects.Model.GetComponent<MeshFilter>().sharedMesh;

        WeaponSlots[index].GetComponent<MeshRenderer>().sharedMaterials =
            projectileWeaponScriptableObjects.Model.GetComponent<MeshRenderer>().sharedMaterials;

        WeaponSlots[index].transform.localScale = projectileWeaponScriptableObjects.Model.transform.localScale;
        WeaponSlots[index].transform.localRotation = projectileWeaponScriptableObjects.Model.transform.rotation;

        muzzle.transform.localPosition = projectileWeaponScriptableObjects.GetMuzzleLocation().localPosition;

        ammo = projectileWeaponScriptableObjects.ammoRemaining;
    }

    public void SetMeleeStats(MeleeWeaponScriptableObjects meleeWeaponScriptableObjects, int index)
    {
        //meleeList.Add(meleeWeaponScriptableObjects);

        meleeLevel = meleeWeaponScriptableObjects.meleeLevel;
        meleeReach = meleeWeaponScriptableObjects.damage;
        knockbackForce = meleeWeaponScriptableObjects.knockbackForce;
        swingSpeed = meleeWeaponScriptableObjects.swingSpeed;


        WeaponSlots[index].GetComponent<MeshFilter>().sharedMesh =
            meleeWeaponScriptableObjects.Model.GetComponent<MeshFilter>().sharedMesh;

        WeaponSlots[index].GetComponent<MeshRenderer>().sharedMaterials =
            meleeWeaponScriptableObjects.Model.GetComponent<MeshRenderer>().sharedMaterials;

        WeaponSlots[index].transform.localScale = meleeWeaponScriptableObjects.Model.transform.localScale;
        WeaponSlots[index].transform.localRotation = meleeWeaponScriptableObjects.Model.transform.rotation;
    }

    public string GetWeaponType(MasterWeapon tempArmoryListOfWeapon, int index)
    {
        if (tempArmoryListOfWeapon.GetType() == typeof(ProjectileWeaponScriptableObjects))
        {
            //Debug.Log("Projectile");
            return "Projectile";
        }

        else if (tempArmoryListOfWeapon.GetType() == typeof(MeleeWeaponScriptableObjects))
        {
            //Debug.Log("Melee");
            return "Melee";
        }
        return "";
    }

    public void AddWeaponToInventory(MasterWeapon tempArmoryListOfWeapon)
    {
        //if no weapons in list 
        if (weaponList.Count == 0)
        {
            currentWeapon = 0;
            weaponList.Add(new Weapon(tempArmoryListOfWeapon, (GetWeaponType(tempArmoryListOfWeapon, currentWeapon) == "Projectile" ? true : false), 0));
            WeaponSlots[currentWeapon].SetActive(true);
        } //if weapon inventory is full
        else if (weaponList.Count == 3)
        {
            weaponList[currentWeapon] = new Weapon(tempArmoryListOfWeapon, (GetWeaponType(tempArmoryListOfWeapon, currentWeapon) == "Projectile" ? true : false), 0);
        } //if weapon inventory is over max
        else if (weaponList.Count > 3)
        {
            Debug.Log("Too Many Weapons in AddWeaponToInventory");
        } //if weapon inventory is in between
        else
        {
            WeaponSlots[currentWeapon].SetActive(false);
            currentWeapon += 1;
            weaponList.Add(new Weapon(tempArmoryListOfWeapon, (GetWeaponType(tempArmoryListOfWeapon, currentWeapon) == "Projectile" ? true : false), 0));
            WeaponSlots[currentWeapon].SetActive(true);
        }
        gameManager.instance.UpdateUI();
        if (weaponList[currentWeapon].isGun)
        {
            SetGunStats((ProjectileWeaponScriptableObjects)tempArmoryListOfWeapon, currentWeapon);
            gameManager.instance.updateAmmoUI(true);
        }
        else
        {
            SetMeleeStats((MeleeWeaponScriptableObjects)tempArmoryListOfWeapon, currentWeapon);
            gameManager.instance.updateAmmoUI(false);
        }

        if(weaponList[currentWeapon].isGun)
            StartCoroutine(reload((ProjectileWeaponScriptableObjects)weaponList[currentWeapon].weapon));

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
    IEnumerator healOverTime(float effectDuration, int healStep)
    {
        if (HP + 1 > HPOrig)
        {
            HP = HPOrig;
            updatePlayerHP();

        }
        if (getHP() < HPOrig)
        {
            for (int i = 0; i < effectDuration; i++)
            {
                if (HP + 1 > HPOrig)
                {
                    HP = HPOrig;
                    break;
                }
                yield return new WaitForSeconds(1.0f);
                HP += healStep;
                updatePlayerHP();
            }
        }
    }
}

public class Weapon
{
    public MasterWeapon weapon;
    public bool isGun;
    public int currentClip;

    public Weapon(MasterWeapon weapon, bool isGun, int currentClip)
    {
        this.weapon = weapon;
        this.isGun = isGun;
        this.currentClip = currentClip;
    }
}