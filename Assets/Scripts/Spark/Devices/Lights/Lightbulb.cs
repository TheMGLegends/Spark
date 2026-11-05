using UnityEngine;

namespace Spark
{
    [RequireComponent(typeof(Light))]
    public class Lightbulb : MonoBehaviour
    {
        [SerializeField, ReadOnly] private LightSocket connectedLightSocket;

        private new Light light;

        public LightSocket ConnectedLightSocket => connectedLightSocket;
        public bool IsLightSocketConnected => connectedLightSocket;
        public Light Light => light;

        private void Awake()
        {
            light = GetComponent<Light>();
        }

        public bool Connect(LightSocket lightSocket)
        {
            if (!lightSocket || IsLightSocketConnected) { return false; }

            if (lightSocket.Attach(this))
            {
                connectedLightSocket = lightSocket;
                return true;
            }

            return false;
        }

        public bool Disconnect(LightSocket lightSocket)
        {
            if (!lightSocket || !IsLightSocketConnected || connectedLightSocket != lightSocket) { return false; }

            if (lightSocket.Detach(this))
            {
                connectedLightSocket = null;
                return true;
            }

            return false;
        }
    }
}
