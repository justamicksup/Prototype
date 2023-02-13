using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class Barricade : MonoBehaviour, IDamage, actionObject
{
    [Header("----- Gate Stats -----")] 
    [SerializeField] int HP;
    private int HPOrig;
    [SerializeField] int repairCost;
    [SerializeField] int damage;
    [SerializeField] int pushBackForce;

    [Header("----- Components -----")]
    [SerializeField] GameObject barricade;
    [SerializeField] Transform _barricade;
    [SerializeField] GameObject brokenBarricade;
    [SerializeField] Transform _brokenBarricade;
    [SerializeField] BoxCollider _boxCollider;

    private bool hasCoin = false;
    private bool playerInRange;
    private bool barricadeActive;
    private Transform target = null;

    // Start is called before the first frame update
    void Start()
    {
        brokenBarricade.SetActive(false);
        _brokenBarricade = _barricade;
        HPOrig = HP;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Action") && CheckPlayerCoins(repairCost))
        {
            primaryAction();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            target = other.transform;
            playerInRange = true;
            if (!barricadeActive)
            {
                gameManager.instance.alertText.text = $"E: Rebuild: ({repairCost})";
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if  (collision.gameObject.CompareTag("Range")
            || collision.gameObject.CompareTag("Melee")
            || collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<IDamage>().takeDamage(damage);
            collision.gameObject.GetComponent<Rigidbody>().AddForce(-transform.forward * pushBackForce);
            takeDamage(damage / 2);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerInRange = false;
            hasCoin = false;
            gameManager.instance.alertText.text = "";
        }
    }

    public void takeDamage(float damage)
    {
        if (HP > 0)
        {
            HP -= (int)damage;
        }
        if (HP <= 0)
        {
            barricade.SetActive(false);
            barricadeActive = false;
            brokenBarricade.SetActive(true);
            _boxCollider.enabled = false;
        }        
    }

    public void primaryAction()
    {
        gameManager.instance.playerScript.addCoins(-repairCost);
        brokenBarricade.SetActive(false);
        barricadeActive = true;
        barricade.SetActive(true);
        _boxCollider.enabled = true;
        HP = HPOrig;
        gameManager.instance.alertText.text = "";
    }

    public void secondaryAction()
    {
        primaryAction();
    }

    private bool CheckPlayerCoins(int cost)
    {
        if (playerInRange && gameManager.instance.playerScript.GetCoins() >= cost)
        {
            hasCoin = true;
        }

        return hasCoin;
    }

    public int GetHP()
    {
        return HP;
    }
}