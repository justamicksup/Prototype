using UnityEngine;

namespace Player
{
    public class cameraController : MonoBehaviour
    {
        [SerializeField] private Transform[] armTransform = new Transform[2];
        //camera rotation limit
        [SerializeField] int lockVerMin;
        [SerializeField] int lockVerMax;

        //inversion bool
        [SerializeField] bool invertX;

        float _xRotation;


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
                _xRotation += mouseY;
            else
                _xRotation -= mouseY;

            //clamp camera rotation
            _xRotation = Mathf.Clamp(_xRotation, lockVerMin, lockVerMax);

            //rotate cameraX
            transform.localRotation = Quaternion.Euler(_xRotation, 0, 0);

            //rotate playerY
            transform.parent.Rotate(Vector3.up * mouseX);
        }

        private void LateUpdate()
        {
            //transform.localRotation = Quaternion.Euler(125, 0, 90);
            armTransform[0].transform.localRotation = Quaternion.Euler(_xRotation, armTransform[0].transform.localRotation.eulerAngles.y, armTransform[0].transform.localRotation.eulerAngles.z);
            armTransform[1].transform.localRotation = Quaternion.Euler(-_xRotation, armTransform[1].transform.localRotation.eulerAngles.y, armTransform[1].transform.localRotation.eulerAngles.z);
        }
    }
}
