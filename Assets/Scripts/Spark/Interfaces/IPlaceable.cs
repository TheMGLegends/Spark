using UnityEngine;

namespace Spark
{
    public interface IPlaceable
    {
#pragma warning disable IDE1006 // Naming Styles
        Transform transform { get; }
#pragma warning restore IDE1006 // Naming Styles

        public void OnPlace(RaycastHit hit, Transform playerTransform)
        {
            if (!transform) { return; }

            Vector3 forward = hit.normal;
            Vector3 upward = hit.normal == Vector3.up ? playerTransform.forward :
                             hit.normal == Vector3.down ? -playerTransform.forward : Vector3.up;

            transform.SetPositionAndRotation(hit.point, Quaternion.LookRotation(forward, upward));
        }
    }
}
