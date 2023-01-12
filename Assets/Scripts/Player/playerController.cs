using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    [Header("----- Components -----")]
    [SerializeField] CharacterController controller;

    [Header("----- Player Stats -----")]
    [Range(1, 100)] [SerializeField] int HP;
    [Range(0,1)] [SerializeField] float stamina;
    [SerializeField] int playerSpeed;
    [SerializeField] int jumpVelocity;
    [SerializeField] int gravity;
    [SerializeField] int jumpMax;
    [SerializeField] int coins;
    [Range(0.01f,5)] [SerializeField] float actionRange;

    [Header("----- Shooting -----")]
    public weapon[] weapons;
    int currentWeapon;
    public GameObject viewModel;

    int jumpTimes;
    Vector3 move;
    Vector3 playerVelocity;

    bool isShooting;
    bool isReloading;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        movement();

        if(!isShooting && Input.GetButton("Shoot") && weapons[currentWeapon] != null)
        {
            if (!isReloading && weapons[currentWeapon].ammoRemaining > 0)
            {
                StartCoroutine(shoot());
            }   
            else if (!isReloading && weapons[currentWeapon].ammoRemaining <= 0)
            {
                StartCoroutine(reload());
            }
        }
        if(!isReloading && Input.GetButtonDown("Reload"))
        {
            StartCoroutine(reload());
        }
        if (Input.GetButtonDown("Action"))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, actionRange))
            {
                if (hit.collider.GetComponent<actionObject>() != null)
                {
                    hit.collider.GetComponent<actionObject>().primaryAction();
                }
            }
        }
        if (Input.GetButtonDown("Submit"))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, actionRange))
            {
                if (hit.collider.GetComponent<actionObject>() != null)
                {
                    hit.collider.GetComponent<actionObject>().secondaryAction();
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
        move =  (transform.right * Input.GetAxis("Horizontal")) + 
                (transform.forward * Input.GetAxis("Vertical"));
        //move character
        if(Input.GetButton("Sprint") && stamina >  0)
        {
            controller.Move(move * Time.deltaTime * playerSpeed * 2);
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

        if(Input.GetButtonDown("Weapon1"))
        {
            changeWeapon(0);
        }
        if (Input.GetButtonDown("Weapon2"))
        {
            changeWeapon(1);
        }
        if (Input.GetButtonDown("Weapon3"))
        {
            changeWeapon(2);
        }
        //add gravity
        playerVelocity.y -= gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    IEnumerator shoot()
    {
        isShooting = true;
        weapons[currentWeapon].ammoRemaining--;
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, weapons[currentWeapon].shootDist))
        {
            if(hit.collider.GetComponent<IDamage>() != null) 
            {
                hit.collider.GetComponent<IDamage>().takeDamage(weapons[currentWeapon].shootDamage);
            }
            if(hit.rigidbody != null)
            {
                hit.rigidbody.AddForceAtPosition(transform.forward * weapons[currentWeapon].shootForce, hit.point);
            }
        }
        yield return new WaitForSeconds(weapons[currentWeapon].shootRate);
        isShooting = false;
    }

    IEnumerator reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(weapons[currentWeapon].reloadTime);
        weapons[currentWeapon].ammoRemaining = weapons[currentWeapon].ammoCapacity;
        isReloading = false;
    }

    public void takeDamage(int damage)
    {
        HP -= damage;
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
        if (weapons[weapon] != null)
        {
            currentWeapon = weapon;
            viewModel.GetComponent<MeshFilter>().mesh = weapons[weapon].viewModel;
        }       
    }
    
    public void respawnPlayer()
    {
        controller.enabled = false;
        transform.position = gameManager.instance.playerSpawnPos.transform.position;
        controller.enabled = true;
    }
}
