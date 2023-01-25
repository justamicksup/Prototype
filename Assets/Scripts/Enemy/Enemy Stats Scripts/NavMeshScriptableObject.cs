using UnityEngine;

[CreateAssetMenu(fileName = "Enemy (NavMesh)", menuName = "Enemy Factory/Enemy (NavMesh)")]
    public class NavMeshScriptableObject: ScriptableObject
    {
        // Need to Finish getting all the NavMesh info
        //This will attach to the enemy SO to be able to fine tune Navmesh in realtime
        public float baseOffset = .5f;
        
        [Header("----- Steering -----")]
        public float speed = 3.5f;
        public float angularSpeed = 120f;
        public float acceleration = 8f;
        public float stoppingDistance = 0f;
        public bool autoBraking = false;
        
        [Header("----- Obstacle Avoidance -----")]
        public float radius = .5f;
        public float height = 1f;
        public int AvoidancePriority = 50;

        [Header("----- Path Finding -----")] 
        public bool AutoTraverseOffMeshLink = true;
        public bool AutoRepath = true;

    }