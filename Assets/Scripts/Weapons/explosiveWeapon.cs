using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class explosiveWeapon : MonoBehaviour
{
    [Header("----- Bomb Stats -----")] 
    public int damage;
    public int timer;
    public int range;
    public int force;
   
    [Header("----- Bomb Can Interact With -----")] 
    public LayerMask layerMask;
    
    [Header("----- Bomb Visuals -----")] 
    [SerializeField] GameObject explosion;
    public Renderer renderer;
    public Color originalColor;
    public float blinkTime;
    public Color color;
    
    [Header("----- Bomb Audio -----")] 
    public AudioSource aud;
    public AudioClip[] audExplosion;
    [Range(0, 1)] [SerializeField] float audExplosionVol;
    public float bombSoundDuration;
    
    //Start is called before the first frame update
    void Start()
    {
        aud.clip = audExplosion[Random.Range(0, audExplosion.Length)];
        aud.volume = audExplosionVol;
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

        // Make bomb Invisible 
        GetComponent<MeshRenderer>().enabled = false;
        
        //Play explosion Sound
        aud.Play();
        
        // Wait until sound plays for a bit
        yield return StartCoroutine(explosionSoundDuration());
        
        // Destroy the gameObject and the clone
        Destroy(gameObject);
        Destroy(explosionClone);
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

    IEnumerator explosionSoundDuration()
    {
      
        yield return new WaitForSeconds(bombSoundDuration);
       
    }
}