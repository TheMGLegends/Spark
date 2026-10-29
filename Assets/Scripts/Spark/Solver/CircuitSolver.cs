using System.Collections.Generic;
using UnityEngine;

namespace Spark
{
    public class CircuitSolver : MonoBehaviour
    {
        [SerializeField, ReadOnly] private List<Object> devices = new();
        [SerializeField, ReadOnly] private List<Wire> wires = new();
        [SerializeField, ReadOnly] private bool isDirty = true;

        private readonly List<IElectricalDevice> electricalDevices = new();

        public static CircuitSolver Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        private void LateUpdate()
        {
            if (!isDirty) { return; }

            EvaluateCircuits();
        }

        public bool RegisterElectricalDevice(IElectricalDevice electricalDevice)
        {
            if (electricalDevice == null || electricalDevices.Contains(electricalDevice)) { return false; }

            isDirty = true;
            electricalDevice.OnInternalConnectivityChanged += ElectricalDevice_EventRaised;
            electricalDevices.Add(electricalDevice);

#if UNITY_EDITOR
            Object device = electricalDevice as Object;
            if (device) { devices.Add(device); }
#endif

            return true;
        }

        public bool UnregisterElectricalDevice(IElectricalDevice electricalDevice)
        {
            if (electricalDevice == null || !electricalDevices.Contains(electricalDevice)) { return false; }

            isDirty = true;
            electricalDevice.OnInternalConnectivityChanged -= ElectricalDevice_EventRaised;

#if UNITY_EDITOR
            Object device = electricalDevice as Object;
            if (device) { devices.Remove(device); }
#endif

            return electricalDevices.Remove(electricalDevice);
        }

        public bool RegisterWire(Wire wire)
        {
            if (wire == null || wires.Contains(wire)) { return false; }

            isDirty = true;
            wire.OnConnected += Wire_EventRaised;
            wire.OnDisconnected += Wire_EventRaised;
            wires.Add(wire);

            return true;
        }

        public bool UnregisterWire(Wire wire)
        {
            if (wire == null || !wires.Contains(wire)) { return false; }

            isDirty = true;
            wire.OnDisconnected -= Wire_EventRaised;
            wire.OnConnected -= Wire_EventRaised;
            return wires.Remove(wire);
        }

        private void EvaluateCircuits()
        {
            isDirty = false;

            foreach (IElectricalDevice electricalDevice in electricalDevices)
            {
                if (electricalDevice is IPowerSource powerSourceDevice)
                {
                    HashSet<IElectricalDevice> positiveTerminalVisitedDevices = TraverseTerminal(powerSourceDevice.PositiveTerminal);
                    HashSet<IElectricalDevice> negativeTerminalVisitedDevices = TraverseTerminal(powerSourceDevice.NegativeTerminal);

                    foreach (IElectricalDevice visitedDevice in positiveTerminalVisitedDevices)
                    {
                        bool powered = negativeTerminalVisitedDevices.Contains(visitedDevice);
                        visitedDevice.SetPowered(powered);
                    }

                    foreach (IElectricalDevice visitedDevice in negativeTerminalVisitedDevices)
                    {
                        bool powered = positiveTerminalVisitedDevices.Contains(visitedDevice);
                        visitedDevice.SetPowered(powered);
                    }
                }
            }
        }

        private HashSet<IElectricalDevice> TraverseTerminal(Terminal startingTerminal)
        {
            if (!startingTerminal) { return new HashSet<IElectricalDevice>(); }

            HashSet<IElectricalDevice> reachedDevices = new();
            HashSet<Terminal> visitedTerminals = new();
            Stack<Terminal> pendingTerminals = new();
            pendingTerminals.Push(startingTerminal);

            while (pendingTerminals.Count > 0)
            {
                Terminal leavingTerminal = pendingTerminals.Pop();

                Wire wire = leavingTerminal.ConnectedWire;
                if (!wire) { continue; }

                Terminal arrivedTerminal = wire.GetOtherTerminal(leavingTerminal);
                if (!arrivedTerminal || !visitedTerminals.Add(arrivedTerminal)) { continue; }

                IElectricalDevice electricalDevice = arrivedTerminal.Owner;
                if (electricalDevice == null)
                {
                    Message.LogError($"{arrivedTerminal.name} is a {typeof(Terminal).Name}, but it's not associated with a {typeof(IElectricalDevice).Name}, make sure it's attached to a valid device.");
                    continue;
                }

                reachedDevices.Add(electricalDevice);

                foreach (Terminal otherTerminal in electricalDevice.Terminals)
                {
                    if (otherTerminal != arrivedTerminal && electricalDevice.CanCurrentPass(arrivedTerminal, otherTerminal))
                    {
                        pendingTerminals.Push(otherTerminal);
                    }
                }
            }

            return reachedDevices;
        }

        private void ElectricalDevice_EventRaised(IElectricalDevice electricalDevice)
        {
            isDirty = true;
        }

        private void Wire_EventRaised(Wire wire)
        {
            isDirty = true;
        }
    }
}
