using UnityEngine;

namespace Spark
{
    public class Battery : ElectricalDeviceBase, IPowerSource, IPlaceable
    {
        private Terminal positiveTerminal;
        private Terminal negativeTerminal;

        public Terminal PositiveTerminal => positiveTerminal;

        public Terminal NegativeTerminal => negativeTerminal;

        public void OnPlace(RaycastHit hit, Transform playerTransform)
        {
            if (hit.normal != Vector3.up) { Destroy(gameObject); return; }

            Vector3 forward = -playerTransform.forward;
            Vector3 upward = hit.normal;

            transform.SetPositionAndRotation(hit.point, Quaternion.LookRotation(forward, upward));
        }

        public override bool CanCurrentPass(Terminal terminalA, Terminal terminalB)
        {
            return false;
        }

        protected override void SetTerminals()
        {
            base.SetTerminals();

            if (terminals.Count != 2)
            {
                string terminalCount = terminals.Count > 1 ? $"are {terminals.Count} terminals" : $"is {terminals.Count} terminal";
                Message.LogError($"{gameObject.name} is a {typeof(Battery).Name}, meaning it requires a positive and negative terminal only. Currently there {terminalCount} on the {typeof(Battery).Name}.");
                return;
            }

            Terminal terminalA = terminals[0];
            CircuitRole terminalAType = terminalA.TerminalType;

            Terminal terminalB = terminals[1];
            CircuitRole terminalBType = terminalB.TerminalType;
            if (terminalAType == terminalBType)
            {
                Message.LogError($"{gameObject.name} is a {typeof(Battery).Name}, but has two {terminalAType} terminals. This type of device requires a {CircuitRole.Live} and {CircuitRole.Neutral} terminal.");
                return;
            }

            if (terminalAType == CircuitRole.Earth || terminalBType == CircuitRole.Earth)
            {
                Message.LogError($"{gameObject.name} is a {typeof(Battery).Name}, but one of its terminals is a {CircuitRole.Earth} terminal. This type of device requires a {CircuitRole.Live} and {CircuitRole.Neutral} terminal.");
                return;
            }

            positiveTerminal = terminalAType == CircuitRole.Live ? terminalA : terminalB;
            negativeTerminal = terminalBType == CircuitRole.Neutral ? terminalB : terminalA;
        }
    }
}
