using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStatSheet : MonoBehaviour
{
    [SerializeField] EnemyStats skeleton;
    [SerializeField] NavMeshAgent agent;
    private void Start()
    {
        agent.baseOffset = skeleton.baseOffset;
        agent.speed = skeleton.speed;
        agent.angularSpeed = skeleton.angularSpeed;
        agent.acceleration = skeleton.acceleration;
        agent.stoppingDistance = skeleton.stoppingDistance;
        agent.autoBraking = skeleton.autoBraking;
        agent.radius = skeleton.radius;
        agent.height = skeleton.height;
        
    }
}
