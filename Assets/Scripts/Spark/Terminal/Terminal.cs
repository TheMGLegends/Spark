using UnityEngine;

namespace Spark
{
    [RequireComponent(typeof(BoxCollider))]
    public class Terminal : MonoBehaviour
    {
        [SerializeField, DisplayName("Owner"), ReadOnly] private MonoBehaviour owningDevice;
        [SerializeField, ReadOnly] private Wire connectedWire;
        [SerializeField] private CircuitRole terminalType;
        [SerializeField] private Transform connectionPoint;

        private IElectricalDevice owner;

        public IElectricalDevice Owner => owner;
        public Wire ConnectedWire => connectedWire;
        public bool IsWireConnected => connectedWire;
        public CircuitRole TerminalType => terminalType;

        private void Awake()
        {
            SetOwner();
            SetConnectionPoint();
        }

        private void OnDestroy()
        {
            Cleanup();
        }

        public bool Attach(Wire wire)
        {
            if (!wire || IsWireConnected) { return false; }

            connectedWire = wire;
            return true;
        }

        public bool Detach(Wire wire)
        {
            if (!wire || !IsWireConnected || connectedWire != wire) { return false; }

            connectedWire = null;
            return true;
        }

        public bool DisconnectWire()
        {
            if (!IsWireConnected) { return false; }

            return connectedWire.Disconnect(this);
        }

        private void SetOwner()
        {
            IElectricalDevice electricalDevice = GetComponentInParent<IElectricalDevice>();
            if (electricalDevice != null)
            {
                owningDevice = electricalDevice as MonoBehaviour;
                owner = electricalDevice;
            }
            else
            {
                Message.LogError($"{gameObject.name} is a {typeof(Terminal).Name}, therefore it needs to be attached to a parent MonoBehaviour object that implements the {typeof(IElectricalDevice).Name} interface.");
            }
        }

        private void SetConnectionPoint()
        {
            if (!owningDevice) { return; }

            if (!connectionPoint)
            {
                connectionPoint = owningDevice.transform;
                Message.LogWarning($"{gameObject.name}'s connectionPoint was not set in the editor, defaulting to {owningDevice.name}'s transform as the connectionPoint.");
            }
        }

        private void Cleanup()
        {
            DisconnectWire();
        }
    }
}
