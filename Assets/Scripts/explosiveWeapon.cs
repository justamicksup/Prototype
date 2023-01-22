using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explosiveWeapon : MonoBehaviour
{
    
    public int damage;
    [SerializeField] int timer;
    [SerializeField] int range;
    [SerializeField] int force;
    [SerializeField] GameObject explosion;
    
    //Start is called before the first frame update
    void Start()
    {
        StartCoroutine(explode());
        
    }

    IEnumerator explode()
    {
        yield return new WaitForSeconds(timer);
        GameObject explosionClone = Instantiate(explosion, transform.position, transform.rotation);
        Collider[] colliders = Physics.OverlapSphere(transform.position, range);
        
       
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].GetComponent<IDamage>() != null)
            {
                colliders[i].GetComponent<IDamage>().takeDamage(damage);
            }
    
            if (colliders[i].GetComponent<Rigidbody>() != null)
            {
                colliders[i].GetComponent<Rigidbody>()
                    .AddForceAtPosition(transform.forward * force, colliders[i].transform.position);
            }
        }
    
        Destroy(gameObject);
        Destroy(explosionClone, 0.5f);
    }
    
    
    
    }
