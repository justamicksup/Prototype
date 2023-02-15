using System.Collections;
using UnityEngine;

namespace Enemy
{
    public class DestroyPirateShip : MonoBehaviour
    {
    
        public  DeathEffect deathEffect;
        public float delay;
        internal void SinkTheShip()
        {
            deathEffect.DeathByEffects();
            StartCoroutine(DestroyDelay());
        
        }

        IEnumerator DestroyDelay()
        {
            yield return new WaitForSeconds(delay);
            Destroy(gameObject);
        }

   
    }
}
