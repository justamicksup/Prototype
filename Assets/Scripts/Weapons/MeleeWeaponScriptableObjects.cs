using UnityEngine;

[CreateAssetMenu(fileName = "Melee Weapon", menuName = "Weapon Factory/Melee Weapon")]
public class MeleeWeaponScriptableObjects : MasterWeapon
{
    // all weapons share
    public int meleeLevel = 1;
    public int damage = 5;
    public int meleeReach = 5;
    public float knockbackForce = 1;
    public float swingSpeed = 0.1f;
    
    public void Randomize()
    {
        damage = Random.Range(1, 10);
        meleeReach = Random.Range(10, 100);
        knockbackForce = Random.Range(10, 100);
        swingSpeed = Random.Range(0.5f, 3);
    }
}