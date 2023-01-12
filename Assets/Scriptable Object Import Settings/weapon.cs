using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weapon : MonoBehaviour
{
    [Range(0.1f, 10)] [SerializeField] public float shootRate;
    [Range(1, 100)] [SerializeField] public int shootDist;
    [Range(1, 10)] [SerializeField] public int shootDamage;
    [Range(1, 10)] [SerializeField] public float shootForce;
    [Range(1, 100)] [SerializeField] public int ammoCapacity;
    [Range(1, 100)] [SerializeField] public int ammoRemaining;
    [Range(0.1f, 10)] [SerializeField] public float reloadTime;
    [SerializeField] public Mesh viewModel;
}
