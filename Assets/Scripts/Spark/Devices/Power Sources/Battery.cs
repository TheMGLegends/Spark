using UnityEngine;

public class Battery : ElectricalDeviceBase, IPowerSource
{
    [SerializeField, ReadOnly] private Terminal positiveTerminal;
    [SerializeField, ReadOnly] private Terminal negativeTerminal;

    public Terminal PositiveTerminal => positiveTerminal;

    public Terminal NegativeTerminal => negativeTerminal;

    public override bool CanCurrentPass(Terminal terminalA, Terminal terminalB)
    {
        return false;
    }

    protected override void SetTerminals()
    {
        base.SetTerminals();

        if (terminals.Count != 2)
        {       
            Message.LogError($"{gameObject.name} is a {typeof(Battery)}, meaning it requires a positive and negative terminal only. Currently there are {terminals.Count} terminals on this {typeof(Battery)}.");
            return;
        }

        Terminal terminalA = terminals[0];
        CircuitRole terminalAType = terminalA.TerminalType;

        Terminal terminalB = terminals[1];
        CircuitRole terminalBType = terminalB.TerminalType;
        if (terminalAType == terminalBType)
        {
            Message.LogError($"{gameObject.name} is a {typeof(Battery)}, but has two {terminalAType} terminals. This type of device requires a {CircuitRole.Live} and {CircuitRole.Neutral} terminal.");
            return;
        }

        if (terminalAType == CircuitRole.Ground || terminalBType == CircuitRole.Ground)
        {
            Message.LogError($"{gameObject.name} is a {typeof(Battery)}, but one of its terminals is a {CircuitRole.Ground} terminal. This type of device requires a {CircuitRole.Live} and {CircuitRole.Neutral} terminal.");
            return;
        }

        positiveTerminal = terminalAType == CircuitRole.Live ? terminalA : terminalB;
        negativeTerminal = terminalBType == CircuitRole.Neutral ? terminalB : terminalA;
    }
}
