using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    public float bulletDamage;
    [SerializeField] int timer;


    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, timer);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            gameManager.instance.playerScript.takeDamage(bulletDamage);
        }
        if(other.CompareTag("Destructible"))
        {
            other.GetComponentInChildren<IDamage>().takeDamage(bulletDamage);
        }
        else
        {
            Physics.IgnoreCollision(this.GetComponent<Collider>(), other.GetComponent<Collider>());
        }
        Destroy(gameObject);
    }
}
