using UnityEngine;

namespace Enemy.Enemy_Stats_Scripts
{
    public abstract class MasterEnemy : ScriptableObject
    {
        public new string name = "Input Name"; 
        public int health = 10;
        public int defense;
        public bool isBoss;
        public GameObject model;
        public NavMeshScriptableObject navMesh;
        public int coinValue = 10;
    


    }
}
