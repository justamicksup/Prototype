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
        Barricade b = other.GetComponent<Barricade>();
        if (other.GetComponent<Barricade>() && b.GetHP() > 0)
        {
            other.GetComponent<Odamage>().TakeDamage(bulletDamage);
        }
        Destroy(gameObject);
    }
}
