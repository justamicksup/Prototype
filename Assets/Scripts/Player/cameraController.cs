using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour
{
    //camera rotation limit
    [SerializeField] int lockVerMin;
    [SerializeField] int lockVerMax;

    //inversion bool
    [SerializeField] bool invertX;

    float xRotation;


    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible= false;
        Cursor.lockState= CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //get input
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * gameManager.instance.sensitivity;
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * gameManager.instance.sensitivity;

        if(invertX)
            xRotation += mouseY;
        else
            xRotation -= mouseY;

        //clamp camera rotation
        xRotation = Mathf.Clamp(xRotation, lockVerMin, lockVerMax);

        //rotate cameraX
        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        //rotate playerY
        transform.parent.Rotate(Vector3.up * mouseX);
    }
}
