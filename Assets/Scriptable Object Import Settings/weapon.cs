using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weapon : MonoBehaviour
{
    [Range(0.1f, 2)][SerializeField] public float shootRate;
    [Range(1, 15)][SerializeField] public int shootDist;
    [Range(1, 10)][SerializeField] public int shootDamage;
    [SerializeField] public float shootForce;
    [SerializeField] public int ammoCapacity;
    [SerializeField] public int ammoRemaining;
    [SerializeField] public int reloadTime;
    [SerializeField] public Mesh viewModel;

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
