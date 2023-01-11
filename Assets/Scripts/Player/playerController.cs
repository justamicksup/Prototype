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

    // Start is called before the first frame update
    void Start()
    {
        weapons = new weapon[3];
    }

    // Update is called once per frame
    void Update()
    {
        movement();
        if(!isShooting && Input.GetButton("Shoot") && weapons[currentWeapon] != null)
        {
            if (weapons[currentWeapon].ammo > 0)
            {
                StartCoroutine(shoot());
            }   
        }
        if(Input.GetButtonDown("Pause"))
        {
            gameManager.instance.pauseGame();
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
        weapons[currentWeapon].ammo--;
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
}
