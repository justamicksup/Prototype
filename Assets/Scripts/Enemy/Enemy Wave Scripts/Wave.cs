using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Enemy_Wave_Scripts
{
   [CreateAssetMenu(fileName = "Enemies In Wave", menuName = "Scriptable Objects/Wave/Create Enemy List In Wave")]
   public class Wave : ScriptableObject
   {
      [field: SerializeField] public List<GameObject> EnemiesInWave { get; private set; }
   }
}
