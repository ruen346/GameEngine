using UnityEngine;

namespace Complete
{
    public class CameraControl : MonoBehaviour
    {
        public GameObject main_tank;

        public float offsetX = 0f;
        public float offsetY = 10f;
        public float offsetZ = -10f;

        public float follow_speed = 0;
        
        Vector3 camera_position;

        void LateUpdate()
        {
            camera_position.x = main_tank.transform.position.x + offsetX;
            camera_position.y = main_tank.transform.position.y + offsetY;
            camera_position.z = main_tank.transform.position.z + offsetZ;
            transform.position = camera_position;
        }
    }
}