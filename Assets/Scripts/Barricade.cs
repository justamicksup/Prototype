using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Barricade : MonoBehaviour, IDamage, actionObject
{
    [Header("----- Gate Stats -----")]
    [SerializeField] int HP;
    private int HPOrig;
    [SerializeField] int gateCost;
    [SerializeField] int repairCost;
    [SerializeField] float cleanUpTimer;
    [SerializeField] bool canTakeDamage;

    [Header("----- Components -----")]
    [SerializeField] GameObject pillars;
    [SerializeField] GameObject brokenGate1;
    [SerializeField] GameObject brokenGate2;
    [SerializeField] Transform _brokenGate1;
    [SerializeField] Transform _brokenGate2;
    [SerializeField] GameObject Gate1;
    [SerializeField] GameObject Gate2;

    private bool hasCoin = false;
    private bool playerInRange;
    private Transform target = null;

    // Start is called before the first frame update
    void Start()
    {
        brokenGate1.SetActive(false);
        brokenGate2.SetActive(false);
        _brokenGate1 = brokenGate1.transform;
        _brokenGate2 = brokenGate2.transform;
        HPOrig = HP;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Action") && CheckPlayerCoins(repairCost) && !Gate1.activeSelf)
        {
            secondaryAction();
        }
        if (Input.GetButtonDown("Submit") && CheckPlayerCoins(gateCost) && Gate1.activeSelf)
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
            gameManager.instance.alertText.text = $"F: Open Gates ({gateCost})\n E: Close Gates ({repairCost})";
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

    public void takeDamage(int damage)
    {
        if (Gate1.activeSelf && Gate2.activeSelf && canTakeDamage)
        {
            if (HP > 0)
            {
                HP -= damage;
            }
            if (HP <= 0)
            {
                brokenGate1.SetActive(true); brokenGate2.SetActive(true);
                pillars.transform.parent.GetComponent<BoxCollider>().enabled = false;
                //Destroy(Gate1);
                Gate1.SetActive(false);
                //Destroy(Gate2);
                Gate2.SetActive(false);

                Invoke(nameof(cleanUpDebris), cleanUpTimer);
            }
        }
    }

    public void primaryAction()
    {
        gameManager.instance.playerScript.addCoins(-gateCost);
        Gate1.SetActive(false); Gate2.SetActive(false);
        pillars.transform.parent.GetComponent<BoxCollider>().enabled = false;
        pillars.transform.parent.GetComponent<NavMeshObstacle>().enabled = false;
    }
    public void secondaryAction()
    {
        Debug.Log("REPAIRING");
        gameManager.instance.playerScript.addCoins(-repairCost);
        pillars.transform.parent.GetComponent<NavMeshObstacle>().enabled = true;
        Gate1.SetActive(true); Gate2.SetActive(true);
        pillars.transform.parent.GetComponent<BoxCollider>().enabled = true;
        pillars.transform.parent.GetComponent<NavMeshObstacle>().enabled = true;
        HP = HPOrig;
    }
    void cleanUpDebris()
    {
        pillars.transform.parent.GetComponent<NavMeshObstacle>().enabled = false;
        brokenGate1.SetActive(false);
        brokenGate2.SetActive(false);
        
        //Destroy(brokenGate1);
        
        //Destroy(brokenGate2);
    }
    GameObject buildGate(GameObject gate, Transform gateTran)
    {
        GameObject bg = Instantiate(gate, gateTran.position, gateTran.rotation, pillars.transform);
        bg.SetActive(false);
        return bg;
    }
    private bool CheckPlayerCoins(int cost)
    {
        if(playerInRange && gameManager.instance.playerScript.GetCoins() >= cost) 
        {
            hasCoin = true;
        }
        return hasCoin;
    }
    public int GetHP() { return HP; }
}
