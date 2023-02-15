using System.Collections;
using UnityEngine;

namespace Enemy
{
    public class DeathEffect : MonoBehaviour
    {
        public GameObject deathEffect;
        public Transform deathEffectLocation;
        public float delay;
        // Start is called before the first frame update

        public void SetDeathEffect(GameObject effect)
        {
            deathEffect = effect;
        }

    
        public void DeathByEffects()
        {
            if (deathEffect != null)
            {
                Instantiate(deathEffect,
                    deathEffectLocation != null ? deathEffectLocation.position : transform.position,
                    Quaternion.identity);
            }
            StartCoroutine(DestroyAfterDelay(deathEffect, delay));
        }
    
        IEnumerator DestroyAfterDelay(GameObject effect, float delayDuration)
        {
            yield return new WaitForSeconds(delayDuration);
            Destroy(effect);
        }
    
    
    }
}
