using System;
using System.Collections;
using System.Collections.Generic;
using Enemy;
using UnityEngine;
using Random = UnityEngine.Random;

public enum UpgradeTypes { PlayerSpeed, PlayerHealth, PlayerStamina, GunDmg, GunReload, GunRange, GunMaxAmmo }
public class playerController : MonoBehaviour
{
    [Header("----- Components -----")]
    [SerializeField]
    CharacterController controller;
    [SerializeField] AudioSource aud;
    [SerializeField] Animator animator;
    [SerializeField] private Animation anim;
    [SerializeField] public ParticleSystem speedPart;
    [SerializeField] public ParticleSystem healthPart;
    [SerializeField] public ParticleSystem killPart;
    public GameObject mainCamera;

    [Header("----- Player Stats -----")]
    [Range(1,100)] [SerializeField] private float playerBaseHealth = 100;
    [Range(1, 100)] [SerializeField] private float playerBaseStamina = 100;
    private float currentHealth;
    public float currentStamina;

    [SerializeField] private LayerMask enemyMask;
    private Coroutine staminaRegen;
    [SerializeField] public int playerBaseSpeed;
    [SerializeField] int jumpVelocity;
    [SerializeField] int gravity;
    [SerializeField] int jumpMax;
    [SerializeField] int coins;
    [SerializeField] public int upgradeCost = 100;
    [Range(0.01f, 5)] [SerializeField] float actionRange;
    public int ammo;
    int jumpTimes;
    Vector3 move;
    Vector3 playerVelocity;
    [SerializeField] private float playerSpeedMultiplier = 1;
    [SerializeField] private float playerStaminaMultiplier = 1;
    [SerializeField] private float playerHealthMultiplier = 1;

    public float PlayerSpeed { get { return playerBaseSpeed * playerSpeedMultiplier; } }
    public float PlayerMaxStamina { get { return playerBaseStamina * playerStaminaMultiplier; } }
    public float PlayerMaxHealth { get { return playerBaseHealth * playerHealthMultiplier; } }


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
    [SerializeField] float reloadTime;
    [SerializeField] GameObject bullet;
    [SerializeField] public int bulletSpeed;
    public Transform muzzle;
    [SerializeField] private float gunDmgMultiplier = 1;
    [SerializeField] private float gunReloadMultiplier = 1;
    [SerializeField] private float gunRangeMultiplier = 1;
    [SerializeField] private int maxAmmoMultiplier = 0;

    public float GunDamage { get { return shootDamage * gunDmgMultiplier; } }
    public float GunReloadTime { get { return (gunReloadMultiplier - 1) * reloadTime; } }
    public float GunShootRange { get { return range * gunRangeMultiplier; } }
    public int MaxAmmo { get { return maxAmmo + maxAmmoMultiplier; } }



    [Header("----- Melee Stats -----")]

    [SerializeField] int meleeLevel;
    [SerializeField] public int meleeDamage;
    [SerializeField] int meleeReach;
    [SerializeField] float knockbackForce;
    [SerializeField] float swingSpeed;
    private bool canMeleeAttack = true;
    [SerializeField] private LayerMask meleeMask;

    [Header("----- Explosive Stats -----")]
    [SerializeField] public GameObject explosive;
    public bool hasExplosive;

    //public GameObject viewModel;

    [Header("----- Player Info -----")]
    [Header("----- Weapon Slots -----")]
    //[SerializeField] public List<MasterWeapon> weaponList = new List<MasterWeapon>();
    public List<Weapon> weaponList = new List<Weapon>();

    [SerializeField] public int currentWeapon;

    //[SerializeField] List<ProjectileWeaponScriptableObjects> gunList = new List<ProjectileWeaponScriptableObjects>();

    //[SerializeField] List<MeleeWeaponScriptableObjects> meleeList = new List<MeleeWeaponScriptableObjects>();



    [SerializeField] private List<GameObject> WeaponSlots;

    public Vector3 pushBack;
    [SerializeField] int pushBackTime;


    bool isShooting;
    bool isReloading;
    bool staminaLeft;
    bool isAttacking;
    public bool inActionRange;
    bool isPlayingSteps;
    bool isSprinting;
    bool isExplosiveAttacking;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = PlayerMaxHealth;
        currentStamina = playerBaseStamina;
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
        if (!gameManager.instance.isPaused)
        {
            if (move.normalized.magnitude > 0.3f && !isPlayingSteps)
                StartCoroutine(playSteps());
            movement();

            if (weaponList.Count > 0)
            {
                if (!isAttacking && Input.GetButtonDown("Shoot"))
                {
                    Attack();
                }

                if (!isReloading && Input.GetButtonDown("Reload"))
                {
                    StartCoroutine(reload((ProjectileWeaponScriptableObjects)weaponList[currentWeapon].weapon));
                }
            }

            if (hasExplosive)
            {
                if (!isExplosiveAttacking && Input.GetButtonDown("Grenade"))
                {
                    StartCoroutine(ExplosiveAttack());
                }
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
        if (Input.GetButton("Sprint") && currentStamina > 0)
        {
            isSprinting = true;
            controller.Move(move * Time.deltaTime * PlayerSpeed * 2);
            useStamina(0.5f);
        }
        else
        {
            isSprinting = false;
            controller.Move(move * Time.deltaTime * PlayerSpeed);
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
        animator.SetFloat("Speed", move.magnitude);
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
            weaponList[currentWeapon].currentClip--;
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
                        buckShotRigidbody.GetComponent<bullet>().bulletDamage = GunDamage;
                    }
                }
            } // regular single bullet weapon
            else
            {
                GameObject bulletClone = Instantiate(bullet, muzzle.position, bullet.transform.rotation);
                bulletClone.GetComponent<Rigidbody>().AddForce(mainCamera.transform.forward * bulletSpeed, ForceMode.Impulse);
                bulletClone.GetComponent<bullet>().bulletDamage = GunDamage;
            }



            if (Physics.Raycast(mainCamera.GetComponent<Camera>().ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, GunShootRange, enemyMask))
            {

                if (hit.collider.GetComponent<IDamage>() != null)
                {
                    hit.collider.GetComponent<IDamage>().TakeDamage(GunDamage);
                }

                if (hit.rigidbody != null)
                {
                    hit.rigidbody.AddForceAtPosition(
                        transform.forward * projectileWeaponScriptableObjects.shootForce,
                        hit.point);
                }
            }

            if (!isReloading)
            {
                StartCoroutine(reload((ProjectileWeaponScriptableObjects)weaponList[currentWeapon].weapon));
            }
            yield return new WaitForSeconds(projectileWeaponScriptableObjects.shootRate);
        }
        //else if(weaponList[currentWeapon].currentClip <= 0 && !isReloading)
        //{
        //    StartCoroutine(reload((ProjectileWeaponScriptableObjects)weaponList[currentWeapon].weapon));
        //}
        isAttacking = false;
    }
    IEnumerator reload(ProjectileWeaponScriptableObjects projectileWeaponScriptableObjects)
    {
        isReloading = true;
        animator.SetBool("Reloading", true);

        if (ammoRemaining >= projectileWeaponScriptableObjects.magMax && weaponList[currentWeapon].currentClip != projectileWeaponScriptableObjects.magMax)
        {
            aud.PlayOneShot(audReload, audReloadVol);
            yield return new WaitForSeconds(GunReloadTime);
            //projectileWeaponScriptableObjects.ammoRemaining = projectileWeaponScriptableObjects.ammoCapacity;
            weaponList[currentWeapon].currentClip = projectileWeaponScriptableObjects.magMax;
            ammoRemaining -= projectileWeaponScriptableObjects.magMax;
            gameManager.instance.updateAmmoUI();
        }
        animator.SetBool("Reloading", false);
        isReloading = false;
    }


    public void takeDamage(float damage)
    {
        currentHealth -= damage;
        updatePlayerHP();
        StartCoroutine(gameManager.instance.flash());
        aud.PlayOneShot(audPlayerDamage[Random.Range(0, audPlayerDamage.Length)], audPlayerDamageVol);
        if (currentHealth <= 0)
        {
            gameManager.instance.youLose();
        }
    }

    public void useStamina(float energy)
    {
        if (currentStamina - energy >= 0)
        {
            currentStamina -= energy;
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

        while (currentStamina < PlayerMaxStamina && !Input.GetButtonDown("Sprint"))
        {
            currentStamina += PlayerMaxStamina / 100;
            updatePlayerStamina();
            yield return new WaitForSeconds(.1f);
        }

        staminaRegen = null;
    }

    public int GetCoins()
    {
        return coins;
    }

    public float getHP()
    {
        return currentHealth;
    }

    public float getStamina()
    {
        return PlayerMaxStamina;
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

    public bool UpgradeStat(UpgradeTypes type, float amount = 0.1f)
    {
        if (coins < upgradeCost) return false;
        coins -= upgradeCost;
        switch (type)
        {
            case UpgradeTypes.PlayerSpeed:
                playerSpeedMultiplier += amount;
                break;
            case UpgradeTypes.PlayerHealth:
                playerHealthMultiplier += amount;
                break;
            case UpgradeTypes.PlayerStamina:
                playerStaminaMultiplier += amount;
                break;
            case UpgradeTypes.GunDmg:
                gunDmgMultiplier += amount;
                break;
            case UpgradeTypes.GunReload:
                gunReloadMultiplier += amount;
                break;
            case UpgradeTypes.GunRange:
                gunRangeMultiplier += amount;
                break;
            case UpgradeTypes.GunMaxAmmo:
                maxAmmoMultiplier += (int)amount;
                break;
        }
        upgradeCost += 50;
        
        return true;
    }

    public void updatePlayerHP()
    {
        gameManager.instance.playerHPBar.fillAmount = (float)currentHealth / (float)PlayerMaxHealth;
    }

    public void updatePlayerStamina()
    {
        gameManager.instance.playerStaminaBar.fillAmount = (float)currentStamina / (float)PlayerMaxStamina;
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
        else if (weaponList[currentWeapon].isExplosive)
        {
            StartCoroutine(ExplosiveAttack());
        }
        // call explosive logic

        //if current weapon is a heal
        // call heal logic
    }

    public void powerPickup(PowerStat power)
    {
        if (power.speedBonus != 0)
        {
            playerBaseSpeed += power.speedBonus;
            gameManager.instance.speedBoostIcon.SetActive(true);
            speedPart.gameObject.SetActive(true);
            speedPart.Play();
        }
        if (power.staminaBonus != 0)
        {
            currentStamina += power.staminaBonus;
        }
        if (power.shootDmgBonus != 0)
        {
            shootDamage += power.shootDmgBonus;
            killPart.gameObject.SetActive(true);
            killPart.Play();
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
            healthPart.gameObject.SetActive(true);
            healthPart.Play();
            gameManager.instance.healingIcon.SetActive(true);
        }
        if (power.ammoBonus != 0)
        {
            if (ammoRemaining + power.ammoBonus <= MaxAmmo)
            {
                AddAmmo(power.ammoBonus);
            }
            else
            {
                ammoRemaining = MaxAmmo;
            }
            if (weaponList.Count > 0 && weaponList[currentWeapon].isGun)
            {
                gameManager.instance.updateAmmoUI();
            }
        }
        if(power.weapon != null)
        {
            AddWeaponToInventory(power.weapon);
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
                enemy.TakeDamage(meleeDamage);
            }
        }
        yield return new WaitForSeconds(0.5f);
        anim.Stop();
        canMeleeAttack = true;
        yield break;
    }

    IEnumerator ExplosiveAttack()
    {
        isExplosiveAttacking = true;
        GameObject explosiveClone = Instantiate(explosive, muzzle.position, Camera.main.transform.rotation);
        explosiveClone.GetComponent<Rigidbody>().velocity = (Camera.main.transform.forward + new Vector3(0, 1, 0)) * 7;
        yield return new WaitForSeconds(3f);
        isExplosiveAttacking = false;
    }

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
        //WeaponSlots[index].transform.rotation = projectileWeaponScriptableObjects.Model.transform.rotation;
        WeaponSlots[index].transform.localRotation = Quaternion.Euler(projectileWeaponScriptableObjects.rotationOffset);
        WeaponSlots[index].transform.localPosition = projectileWeaponScriptableObjects.positionOffset;

        muzzle.transform.localPosition = projectileWeaponScriptableObjects.GetMuzzleLocation().localPosition;

        ammo = projectileWeaponScriptableObjects.magMax;

        animator.SetInteger("WeaponType", 2);
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

        animator.SetInteger("WeaponType", 1);
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
            //Debug.Log("Too Many Weapons in AddWeaponToInventory");
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
        if (currentHealth + 1 > PlayerMaxHealth)
        {
            currentHealth = PlayerMaxHealth;
            updatePlayerHP();

        }
        if (currentHealth < PlayerMaxHealth)
        {
            for (int i = 0; i < effectDuration; i++)
            {
                if (currentHealth + 1 > PlayerMaxHealth)
                {
                    currentHealth = PlayerMaxHealth;
                    break;
                }
                yield return new WaitForSeconds(1.0f);
                currentHealth += healStep;
                updatePlayerHP();
            }
        }
    }
}

public class Weapon
{
    public MasterWeapon weapon;
    public bool isGun;
    public bool isMelee;
    public bool isExplosive;
    public int currentClip;

    public Weapon(MasterWeapon weapon, bool isGun, int currentClip)
    {
        this.weapon = weapon;
        this.isGun = isGun;
        this.currentClip = currentClip;
    }
}