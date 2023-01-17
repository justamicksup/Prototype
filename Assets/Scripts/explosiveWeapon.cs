using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explosiveWeapon : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] int timer;
    [SerializeField] int range;
    [SerializeField] int force;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, timer);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Melee") || other.CompareTag("Range"))
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, other.transform.position, out hit, range))
            {
                if (hit.collider.GetComponent<IDamage>() != null)
                {
                    hit.collider.GetComponent<IDamage>().takeDamage(damage);
                }

                if (hit.rigidbody != null)
                {
                    hit.rigidbody.AddForceAtPosition(transform.forward * force, hit.point);
                }
            }
        }
        Destroy(gameObject);
    }
}
