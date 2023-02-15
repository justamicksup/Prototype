using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [SerializeField] MasterWeapon weapon;
   
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.instance.playerScript.AddWeaponToInventory(weapon);
            Destroy(gameObject);
        }
    }
}
