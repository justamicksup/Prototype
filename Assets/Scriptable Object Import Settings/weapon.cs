using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weapon : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] public int ammo;
    [Range(0.1f, 2)][SerializeField] public float shootRate;
    [Range(1, 15)][SerializeField] public int shootDist;
    [Range(1, 10)][SerializeField] public int shootDamage;
    [SerializeField] public float shootForce;
    [SerializeField] public Mesh viewModel;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
