using System;
using System.Collections.Generic;
using UnityEngine;

namespace Spark
{
    public abstract class ElectricalDeviceBase : MonoBehaviour, IElectricalDevice
    {
        [SerializeField, ReadOnly] protected bool isPowered;
        [SerializeField, ReadOnly] protected List<Terminal> terminals = new();

        public bool IsPowered => isPowered;
        public IReadOnlyList<Terminal> Terminals => terminals;

        public event Action<IElectricalDevice> OnInternalConnectivityChanged;

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

            if (IsPowered) { OnPowerGained(); }
            else { OnPowerLost(); }
        }

        public int GetTerminalCount(CircuitRole terminalType)
        {
            int count = 0;
            foreach (Terminal terminal in terminals)
            {
                if (terminal && terminal.TerminalType == terminalType)
                {
                    count++;
                }
            }

            return count;
        }

        public virtual bool CanCurrentPass(Terminal terminalA, Terminal terminalB)
        {
            if (!terminals.Contains(terminalA) || !terminals.Contains(terminalB)) { return false; }

            CircuitRole terminalTypeA = terminalA.TerminalType;
            CircuitRole terminalTypeB = terminalB.TerminalType;

            return (terminalTypeA == CircuitRole.Live && terminalTypeB == CircuitRole.Neutral) ||
                   (terminalTypeA == CircuitRole.Neutral && terminalTypeB == CircuitRole.Live);
        }

        protected virtual void OnPowerGained() { Message.Log($"{gameObject.name} is powered on!"); }
        protected virtual void OnPowerLost() { Message.Log($"{gameObject.name} is powered off!"); }

        protected virtual void SetTerminals()
        {
            GetComponentsInChildren(true, terminals);

            if (terminals.Count == 0)
            {
                Message.LogWarning($"{gameObject.name} has no terminals attached to it, even though it is a {typeof(IElectricalDevice).Name}, was this intentional?");
            }
        }

        protected void NotifyInternalConnectivityChanged()
        {
            OnInternalConnectivityChanged?.Invoke(this);
        }

        private void Initialise()
        {
            CircuitSolver.Instance.RegisterElectricalDevice(this);
        }

        private void Cleanup()
        {
            CircuitSolver.Instance.UnregisterElectricalDevice(this);
        }
    }
}
