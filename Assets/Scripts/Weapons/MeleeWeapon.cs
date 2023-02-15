
    using UnityEngine;

    public class MeleeWeapon: MonoBehaviour
    {
        public int damage;


        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                gameManager.instance.playerScript.takeDamage(damage);
            }
        }
    }
