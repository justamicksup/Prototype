using UnityEngine;

[CreateAssetMenu(fileName = "Enemy (Explosive)", menuName = "Enemy Factory/Enemy (Explosive)")]
public class EnemyExplosiveScriptableObjects : MasterEnemy
{
    public int throwingDistance;
    public GameObject bomb;
    public int throwAngle = 45;
    public int viewAngle = 60;
    public int forceForward= 500;
    public int forceUpward= 250;
    public int torque= 500;
    public int rotationSpeed = 50;
   
}