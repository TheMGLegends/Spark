using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Terminal : MonoBehaviour
{
    [SerializeField, ReadOnly] private MonoBehaviour owner;
    [SerializeField, ReadOnly] private Wire connectedWire;
    [SerializeField] private CircuitRole terminalType;
    [SerializeField] private Transform connectionPoint; // TODO: Figure out how we use this connection point to set the wires location (Either set it in here [Terminal] or on wires end using connection point getter)

    public IElectricalDevice Owner { get { return owner as IElectricalDevice; } }
    public Wire ConnectedWire { get { return connectedWire; } }
    public bool IsWireConnected => connectedWire;
    public CircuitRole TerminalType { get { return terminalType; } }

    private void Awake()
    {
        TrySetOwner();
        TrySetConnectionPoint();
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
        // TODO: Wire Disconnect Function
        //return connectedWire.Disconnect(this);

        return true; // TEMP
    }

    private void TrySetOwner()
    {
        IElectricalDevice electricalDevice = GetComponentInParent<IElectricalDevice>();
        if (electricalDevice != null)
        {
            owner = electricalDevice as MonoBehaviour;
        }
        else
        {
            Message.LogError($"{gameObject.name} is a {typeof(Terminal)}, therefore it needs to be attached to a parent MonoBehaviour object that implements the {typeof(IElectricalDevice)} interface.");
        }
    }

    private void TrySetConnectionPoint()
    {
        if (!owner) { return; }

        if (!connectionPoint)
        {
            connectionPoint = owner.transform;
            Message.LogWarning($"{gameObject.name}'s connectionPoint was not set in the editor, defaulting to {owner.name}'s transform as the connectionPoint.");
        }
    }
}
