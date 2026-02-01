using UnityEngine;

namespace TDLN.CameraControllers
{
    public class CameraOrbit : MonoBehaviour
    {
        public GameObject target;
        public float distance = 10.0f;
        public float xSpeed = 250.0f;
        public float ySpeed = 120.0f;
        public float yMinLimit = -20;
        public float yMaxLimit = 80;

        private float x = 0.0f;
        private float y = 0.0f;

        [SerializeField] Rigidbody busBody;

        private void Start()
        {
            var angles = transform.eulerAngles;
            x = angles.y;
            y = angles.x;
        }

        private void LateUpdate()
        {
            distance -= Input.GetAxis("Mouse ScrollWheel") * 2.5f;
            if (distance < 3) distance = 3;
            if (distance > 11) distance = 11;

            if (target && (Input.GetMouseButton(0) || Input.GetMouseButton(1)))
            {
                x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
                y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
                y = ClampAngle(y, yMinLimit, yMaxLimit);
            }

            // Calculate camera rotation (independent of bus rotation)
            var rotation = Quaternion.Euler(y, x, 0);
            var position = target.transform.position - (rotation * Vector3.forward * distance);

            transform.rotation = rotation;
            transform.position = position;

            // Restoring cursor visibility and lock state
            Cursor.visible = !(Input.GetMouseButton(0) || Input.GetMouseButton(1));
            Cursor.lockState = Cursor.visible ? CursorLockMode.None : CursorLockMode.Locked;
            
                    
        }

        private static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360) angle += 360;
            if (angle > 360) angle -= 360;
            return Mathf.Clamp(angle, min, max);
        }
    }
}
