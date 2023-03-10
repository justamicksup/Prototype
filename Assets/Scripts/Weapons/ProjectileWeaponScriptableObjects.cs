using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "Projectile Weapon", menuName = "Weapon Factory/Projectile Weapon")]
public class ProjectileWeaponScriptableObjects : MasterWeapon
{
    // all weapons share
    public int gunLevel = 1;
    public int damage = 5;
    public int range = 5;
    public float shootRate = 1f;
    public float shootForce = 1;
    public int magMax;
    public int carryAmount;
    public float reloadTime = 0.1f;
    public AudioClip audGunShot;
    [Range(0, 1)] public float audGunShotVol;
    public ParticleSystem muzzleFlash;
    public GameObject bullet;
    public int bulletSpeed;

    //We need clip size, current clip count, max carry amount, and current carry count

    public void Randomize()
    {
        
        gunLevel = Random.Range(1, 10);
        damage =Random.Range(10, 100);
        range = Random.Range(10, 100);
        shootRate = Random.Range(0.1f, 1);
        shootForce =Random.Range(1, 5);
        magMax =Random.Range(5, 30);
        carryAmount = Random.Range(10, 100);
        reloadTime = Random.Range(0.1f, 2f);
    }

    public Transform GetMuzzleLocation()
    {
        return Model.transform.Find("Muzzle");
    }
}