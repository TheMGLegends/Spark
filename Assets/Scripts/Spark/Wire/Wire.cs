using System;
using UnityEngine;

public class Wire : MonoBehaviour
{
    [SerializeField, ReadOnly] private Terminal terminalA;
    [SerializeField, ReadOnly] private Terminal terminalB;
    [SerializeField] private CircuitRole wireType;

    public event Action<Wire> OnConnected;
    public event Action<Wire> OnDisconnected;

    public Terminal TerminalA { get { return terminalA; } }
    public Terminal TerminalB { get { return terminalB; } }
    public bool IsFullyConnected => terminalA && terminalB;
    public CircuitRole WireType { get { return wireType; } }

    private void Start()
    {
        Initialise();
    }

    private void OnDestroy()
    {
        Cleanup();
    }

    public bool Connect(WireEnd wireEnd, Terminal terminal)
    {
        Terminal currentTerminal = GetTerminal(wireEnd);
        Terminal otherTerminal = GetOtherTerminal(wireEnd);
        if (!terminal || currentTerminal || otherTerminal == terminal) { return false; }

        if (terminal.Attach(this))
        {
            SetTerminal(wireEnd, terminal);
            if (IsFullyConnected) { OnConnected?.Invoke(this); }

            return true;
        }

        return false;
    }

    public bool Disconnect(Terminal terminal)
    {
        if (TryGetEnd(terminal, out WireEnd wireEnd))
        {
            return Disconnect(wireEnd);
        }

        return false;
    }

    public bool Disconnect(WireEnd wireEnd)
    {
        Terminal terminal = GetTerminal(wireEnd);
        if (!terminal) { return false; }

        if (terminal.Detach(this))
        {
            bool wasFullyConnected = IsFullyConnected;
            SetTerminal(wireEnd, null);
            if (wasFullyConnected) { OnDisconnected?.Invoke(this); }

            return true;
        }

        return false;
    }

    public bool TryGetEnd(Terminal terminal, out WireEnd wireEnd)
    {
        if (terminal)
        {
            if (terminal == terminalA)
            {
                wireEnd = WireEnd.A;
                return true;
            }
            else if (terminal == terminalB)
            {
                wireEnd = WireEnd.B;
                return true;
            }
        }

        wireEnd = default;
        return false;
    }

    public Terminal GetOtherTerminal(Terminal terminal)
    {
        if (TryGetEnd(terminal, out WireEnd wireEnd))
        {
            return GetOtherTerminal(wireEnd);
        }

        return null;
    }

    public Terminal GetOtherTerminal(WireEnd wireEnd)
    {
        return wireEnd == WireEnd.A ? terminalB : terminalA;
    }

    private Terminal GetTerminal(WireEnd wireEnd)
    {
        return wireEnd == WireEnd.A ? terminalA : terminalB;
    }

    private void SetTerminal(WireEnd wireEnd, Terminal terminal)
    {
        switch (wireEnd)
        {
            case WireEnd.A:
                terminalA = terminal;
                break;
            case WireEnd.B:
                terminalB = terminal;
                break;
            default:
                break;
        }
    }

    private void Initialise()
    {
        // TODO: Register Wire Instance and it's events with the Circuit Solver Singleton Here
    }

    private void Cleanup()
    {
        Disconnect(WireEnd.A);
        Disconnect(WireEnd.B);

        // TODO: Cleanup logic (Unregistering itself again from the Circuit Solver Singleton)
    }
}
