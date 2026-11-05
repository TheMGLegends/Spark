using UnityEngine;

namespace Spark
{
    public class Switch : ElectricalDeviceBase
    {
        [SerializeField, ReadOnly] private bool isClosed;

        public bool IsClosed => isClosed;

        public void ToggleClosed()
        {
            isClosed = !isClosed;
            NotifyInternalConnectivityChanged();
        }

        public override bool CanCurrentPass(Terminal terminalA, Terminal terminalB)
        {
            return IsClosed && terminalA != terminalB &&
                   terminals.Contains(terminalA) && terminals.Contains(terminalB) &&
                   terminalA.TerminalType == CircuitRole.Live && terminalB.TerminalType == CircuitRole.Live;
        }

        protected override void SetTerminals()
        {
            base.SetTerminals();

            int liveTerminalsCount = GetTerminalCount(CircuitRole.Live);
            if (liveTerminalsCount != 2)
            {
                Message.LogError($"{gameObject.name} is a {typeof(Switch).Name}, meaning it requires two {CircuitRole.Live} terminals. Currently there {liveTerminalsCount} on the {typeof(Switch).Name}.");
                return;
            }

            int neutralTerminalsCount = GetTerminalCount(CircuitRole.Neutral);
            if (neutralTerminalsCount != 0)
            {
                Message.LogError($"{gameObject.name} is a {typeof(Switch).Name}, but has {neutralTerminalsCount} {CircuitRole.Neutral} terminals. This type of device requires two {CircuitRole.Live} terminals and optionally a {CircuitRole.Ground} terminal.");
                return;
            }
        }
    }
}
