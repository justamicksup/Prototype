using UnityEngine;

public class rotatingItem : MonoBehaviour
{

    [SerializeField] int rotationSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, rotationSpeed, 0));
    }
}
