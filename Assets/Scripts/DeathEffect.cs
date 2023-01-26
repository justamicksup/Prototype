using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathEffect : MonoBehaviour
{
    public GameObject deathEffect;
    
    // Start is called before the first frame update

    public void SetDeathEffect(GameObject _deathEffect)
    {
        deathEffect = _deathEffect;
    }

    
   public void DeathByEffects()
    {
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }
        StartCoroutine(DestroyAfterDelay(deathEffect, .5f));
    }
    
    IEnumerator DestroyAfterDelay(GameObject deathEffect, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(deathEffect);
    }
    
    
}
