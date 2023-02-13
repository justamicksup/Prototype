using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Barricade : MonoBehaviour, IDamage, actionObject
{
    [Header("----- Gate Stats -----")] [SerializeField]
    int HP;

    private int HPOrig;

    [Header("----- Components -----")]
    [SerializeField] GameObject barricade;
    [SerializeField] Transform _barricade;
    [SerializeField] GameObject brokenBarricade;
    [SerializeField] Transform _brokenBarricade;

    private bool hasCoin = false;
    private bool playerInRange;
    private Transform target = null;
    [SerializeField] bool isBossDoor;

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
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            gameManager.instance.key = 6;
        }
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
            if (isBossDoor)
            {
                target = other.transform;
                playerInRange = true;
                gameManager.instance.alertText.text = $"F: 6 Keys are needed to Open Gate)";
            }
            else
            {
                target = other.transform;
                playerInRange = true;
                gameManager.instance.alertText.text = $"F: Open Gates ({gateCost})\n E: Close Gates ({repairCost})";
            }
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
        if (Gate1.activeSelf && Gate2.activeSelf && canTakeDamage)
        {
            if (HP > 0)
            {
                HP -= (int)damage;
            }

            if (HP <= 0)
            {
                brokenGate1.SetActive(true);
                brokenGate2.SetActive(true);
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
        if (isBossDoor)
        {
            if (gameManager.instance.key == 6)
            {
                Gate1.SetActive(false);
                Gate2.SetActive(false);
                pillars.transform.parent.GetComponent<BoxCollider>().enabled = false;
                pillars.transform.parent.GetComponent<NavMeshObstacle>().enabled = false;
            }
        }
        else
        {
            gameManager.instance.playerScript.addCoins(-gateCost);
            Gate1.SetActive(false);
            Gate2.SetActive(false);
            pillars.transform.parent.GetComponent<BoxCollider>().enabled = false;
            pillars.transform.parent.GetComponent<NavMeshObstacle>().enabled = false;
        }
    }

    public void secondaryAction()
    {
        Debug.Log("REPAIRING");
        gameManager.instance.playerScript.addCoins(-repairCost);
        pillars.transform.parent.GetComponent<NavMeshObstacle>().enabled = true;
        Gate1.SetActive(true);
        Gate2.SetActive(true);
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