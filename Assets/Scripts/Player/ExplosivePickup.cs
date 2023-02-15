using UnityEngine;

public class ExplosivePickup : MonoBehaviour
{
    [SerializeField] GameObject explosive;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.instance.playerScript.explosive = explosive;
            gameManager.instance.playerScript.hasExplosive = true;
            Destroy(gameObject);
        }
    }
}
