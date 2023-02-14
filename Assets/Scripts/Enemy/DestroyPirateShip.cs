using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyPirateShip : MonoBehaviour
{
    
    public  DeathEffect _deathEffect;
    public float delay;
   internal void sinkTheShip()
    {
        _deathEffect.DeathByEffects();
        StartCoroutine(destroyDelay());
        
    }

   IEnumerator destroyDelay()
   {
       yield return new WaitForSeconds(delay);
       Destroy(gameObject);
   }

   
}
