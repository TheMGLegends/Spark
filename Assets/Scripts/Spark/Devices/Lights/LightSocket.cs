using UnityEngine;

namespace Spark
{
    public class LightSocket : ElectricalDeviceBase
    {
        [SerializeField, ReadOnly] private Lightbulb connectedLightbulb;

        public Lightbulb ConnectedLightbulb => connectedLightbulb;
        public bool IsLightbulbConnected => connectedLightbulb;

        public bool Attach(Lightbulb lightbulb)
        {
            if (!lightbulb || IsLightbulbConnected) { return false; }

            connectedLightbulb = lightbulb;
            NotifyInternalConnectivityChanged();
            return true;
        }

        public bool Detach(Lightbulb lightbulb)
        {
            if (!lightbulb || !IsLightbulbConnected || connectedLightbulb != lightbulb) { return false; }

            connectedLightbulb = null;
            NotifyInternalConnectivityChanged();
            return true;
        }

        public bool DisconnectLightbulb()
        {
            if (!IsLightbulbConnected) { return false; }

            return connectedLightbulb.Disconnect(this);
        }

        public override bool CanCurrentPass(Terminal terminalA, Terminal terminalB)
        {
            return IsLightbulbConnected && base.CanCurrentPass(terminalA, terminalB);
        }

        protected override void OnPowerGained()
        {
            if (!connectedLightbulb || !connectedLightbulb.Light) { return; }

            connectedLightbulb.Light.enabled = true;
        }

        protected override void OnPowerLost()
        {
            if (!connectedLightbulb || !connectedLightbulb.Light) { return; }

            connectedLightbulb.Light.enabled = false;
        }
    }
}
