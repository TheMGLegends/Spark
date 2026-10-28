using UnityEngine;

namespace Spark
{
    [RequireComponent(typeof(BoxCollider))]
    public class Terminal : MonoBehaviour
    {
        [SerializeField, ReadOnly] private MonoBehaviour owner; // TODO: Hacky way because it doesnt display interface solve this
        [SerializeField, ReadOnly] private Wire connectedWire;
        [SerializeField] private CircuitRole terminalType;
        [SerializeField] private Transform connectionPoint; // TODO: Figure out how we use this connection point to set the wires location (Either set it in here [Terminal] or on wires end using connection point getter)

        public IElectricalDevice Owner => owner as IElectricalDevice;
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
                owner = electricalDevice as MonoBehaviour;
            }
            else
            {
                Message.LogError($"{gameObject.name} is a {typeof(Terminal).Name}, therefore it needs to be attached to a parent MonoBehaviour object that implements the {typeof(IElectricalDevice).Name} interface.");
            }
        }

        private void SetConnectionPoint()
        {
            if (!owner) { return; }

            if (!connectionPoint)
            {
                connectionPoint = owner.transform;
                Message.LogWarning($"{gameObject.name}'s connectionPoint was not set in the editor, defaulting to {owner.name}'s transform as the connectionPoint.");
            }
        }

        private void Cleanup()
        {
            DisconnectWire();
        }
    }
}
