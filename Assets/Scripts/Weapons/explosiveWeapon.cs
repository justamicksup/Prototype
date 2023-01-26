using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class explosiveWeapon : MonoBehaviour
{
    public int damage;
    public int timer;
    public int range;
    public int force;
    public LayerMask layerMask;
    [SerializeField] GameObject explosion;
    public Renderer renderer;
    public Color originalColor;
    public float blinkTime;
    public Color color;
    public AudioSource aud;
    public AudioClip[] audExplosion;
    [Range(0, 1)] [SerializeField] float audExplosionVol;
    //Start is called before the first frame update
    void Start()
    {
        originalColor = renderer.material.color;
        StartCoroutine(explode());
        StartCoroutine(BlinkEffect());
    }

    IEnumerator explode()
    {
        yield return new WaitForSeconds(timer);
        GameObject explosionClone = Instantiate(explosion, transform.position, transform.rotation);
        Collider[] colliders = Physics.OverlapSphere(transform.position, range, layerMask);


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

            if (colliders[i].CompareTag("Player"))
            {
                gameManager.instance.playerScript.takeDamage(damage);
            }
        }
        aud.PlayOneShot(audExplosion[Random.Range(0, audExplosion.Length)], audExplosionVol);
        Destroy(gameObject);
        Destroy(explosionClone, 0.5f);
    }

    IEnumerator BlinkEffect()
    {
        while (true)
        {
            renderer.material.color = color;

            yield return new WaitForSeconds(blinkTime);

            renderer.material.color = originalColor;
            yield return new WaitForSeconds(blinkTime);
        }
    }

}