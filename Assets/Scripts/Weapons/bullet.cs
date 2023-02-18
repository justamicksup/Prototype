using Enemy;
using UnityEngine;

public class bullet : MonoBehaviour
{
    public float bulletDamage;
    [SerializeField] int timer;


    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, timer);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            gameManager.instance.playerScript.takeDamage(bulletDamage);
        }
        Destroy(gameObject);
    }
}
