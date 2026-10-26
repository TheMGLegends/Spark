using System.Collections.Generic;
using UnityEngine;

public abstract class ElectricalDeviceBase : MonoBehaviour, IElectricalDevice
{
    [SerializeField, ReadOnly] protected bool isPowered;
    [SerializeField, ReadOnly] protected List<Terminal> terminals = new();

    // TODO: Potential need to announce a connectivity changed event

    public bool IsPowered => isPowered;

    public IReadOnlyList<Terminal> Terminals => terminals;

    protected virtual void Awake()
    {
        SetTerminals();
    }

    protected virtual void Start()
    {
        Initialise();
    }

    protected virtual void OnDestroy()
    {
        Cleanup();
    }

    public void SetPowered(bool powered)
    {
        if (IsPowered == powered) { return; }

        isPowered = powered;

        if (IsPowered) { PowerOn(); }
        else { PowerOff(); }
    }

    public virtual bool CanCurrentPass(Terminal terminalA, Terminal terminalB)
    {
        if (terminals.Contains(terminalA) && terminals.Contains(terminalB) && terminalA != terminalB)
        {
            return true;
        }

        return false;
    }

    protected virtual void PowerOn() { }
    protected virtual void PowerOff() { }

    protected virtual void SetTerminals()
    {
        GetComponentsInChildren(true, terminals);

        if (terminals.Count == 0)
        {
            Message.LogWarning($"{gameObject.name} has no terminals attached to it, even though it is a {typeof(IElectricalDevice)}, was this intentional?");
        }
    }

    private void Initialise()
    {
        // TODO: Register IElectricalDevice Instance with the Circuit Solver Singleton Here
    }

    private void Cleanup()
    {
        // TODO: Cleanup logic (Unregistering itself again from the Circuit Solver Singleton)
    }
}
