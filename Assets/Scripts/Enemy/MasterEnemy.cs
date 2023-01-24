using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public abstract class MasterEnemy : ScriptableObject
{
    public new string name = "Input Name";
    public int health = 10;
    public int defense = 0;
    public bool isBoss = false;
    public GameObject Model;
    public NavMeshScriptableObject navMesh;
    public NavMeshAgent agent;

}
