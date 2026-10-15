using UnityEngine;

public class Terminal : MonoBehaviour
{
    [SerializeField, ReadOnly] private InterfaceReference<IElectricalDevice> owner;
    [SerializeField, ReadOnly] private Wire connectedWire;
    [SerializeField] private CircuitRole terminalType;

    public IElectricalDevice Owner { get { return owner.Value; } }
    public CircuitRole TerminalType { get { return terminalType; } }
    public Wire ConnectedWire { get { return connectedWire; } set { connectedWire = ConnectedWire; } }
    public bool IsConnected => connectedWire;
}
