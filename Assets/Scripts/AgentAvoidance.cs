using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentAvoidance : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float separationDistance = 1f;
    [SerializeField] private float separationStrength = .1f;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }


    // Update is called once per frame
    void Update()
    {
        Vector3 separationForce = Vector3.zero;

        NavMeshAgent[] agents = FindObjectsOfType<NavMeshAgent>();
        
        foreach (var otherAgent in agents)
        {
            if (otherAgent != null && otherAgent != GetComponent<NavMeshAgent>())
            {
                float distance = Vector3.Distance(transform.position, otherAgent.transform.position);
                if (distance < separationDistance)
                {
                    separationForce = (transform.position - otherAgent.transform.position).normalized *
                                      separationStrength;
                    GetComponent<NavMeshAgent>().velocity +=
                        Vector3.ClampMagnitude(separationForce, separationStrength);
                }
            }
        }
    }
}