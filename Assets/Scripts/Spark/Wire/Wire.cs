using System;
using UnityEngine;

public class Wire : MonoBehaviour
{
    [SerializeField, ReadOnly] private Terminal terminalA;
    [SerializeField, ReadOnly] private Terminal terminalB;
    [SerializeField] private CircuitRole wireType;

    // TODO: Do we need to pass what was previously connected and what is now connected?
    public static event Action OnConnected;
    public static event Action OnDisconnected;

    public Terminal TerminalA 
    { 
        get { return terminalA; } 

        set 
        {
            if (!terminalA && TerminalA) { OnConnected?.Invoke(); }
            else if (terminalA && !TerminalA) { OnDisconnected?.Invoke(); }

            terminalA = TerminalA; 
        } 
    }
    public Terminal TerminalB 
    { 
        get { return terminalB; } 

        set 
        {
            if (!terminalB && TerminalB) { OnConnected?.Invoke(); }
            else if (terminalB && !TerminalB) { OnDisconnected?.Invoke();  }
            terminalB = TerminalB; 
        } 
    }
    public CircuitRole WireType { get { return wireType; } }
}
