using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour
{
    [SerializeField] private Transform[] armTransform = new Transform[2];
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

    private void LateUpdate()
    {
        //transform.localRotation = Quaternion.Euler(125, 0, 90);
        armTransform[0].transform.localRotation = Quaternion.Euler(xRotation, armTransform[0].transform.localRotation.eulerAngles.y, armTransform[0].transform.localRotation.eulerAngles.z);
        armTransform[1].transform.localRotation = Quaternion.Euler(-xRotation, armTransform[1].transform.localRotation.eulerAngles.y, armTransform[1].transform.localRotation.eulerAngles.z);
    }
}
