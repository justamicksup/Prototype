using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
#endif
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Scriptable Objects/Create An Enemy")]
public class EnemyStats : ScriptableObject
{
    public new string name = "Input Name";
    public int health = 10;
    public int attack = 1;
    public int defense = 0;
    
    public float baseOffset = .5f;
    public float speed = 3.5f;
    public float angularSpeed = 120f;
    public float acceleration = 8f;
    public float stoppingDistance = 0f;
    public bool autoBraking = true;

    public float radius = .5f;
    public float height = 1f;

    
    
    public GameObject weapon;
}