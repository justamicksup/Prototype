using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Wave", menuName = "Scriptable Objects/Wave/Enemy List")]
public class Wave : ScriptableObject
{
   [field: SerializeField] public List<GameObject> EnemiesInWave { get; private set; }
   
}
