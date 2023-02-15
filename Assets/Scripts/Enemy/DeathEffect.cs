using System;
using System.Collections;
using UnityEngine;

public class DeathEffect : MonoBehaviour
{
    public GameObject deathEffect;
    public Transform deathEffectLocation;
    public float delay;
    // Start is called before the first frame update

    public void SetDeathEffect(GameObject _deathEffect)
    {
        deathEffect = _deathEffect;
    }

    
   public void DeathByEffects()
    {
        if (deathEffect != null)
        {
            if (deathEffectLocation != null)
            {
                Instantiate(deathEffect, deathEffectLocation.position, Quaternion.identity);
            }
            else
            {
                 Instantiate(deathEffect, transform.position, Quaternion.identity);
            }
           
        }
        StartCoroutine(DestroyAfterDelay(deathEffect, delay));
    }
    
    IEnumerator DestroyAfterDelay(GameObject deathEffect, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(deathEffect);
    }
    
    
}
