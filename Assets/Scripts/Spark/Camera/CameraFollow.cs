using UnityEngine;

namespace Spark
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform target;

        private void LateUpdate()
        {
            if (!target) { return; }

            transform.position = target.position;
        }
    }
}
