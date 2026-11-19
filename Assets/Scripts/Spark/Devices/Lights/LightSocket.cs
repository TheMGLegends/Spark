using UnityEngine;

namespace Spark
{
    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(Animator))]
    public class LightSocket : ElectricalDeviceBase, IInteractable
    {
        [SerializeField, ReadOnly] private Lightbulb connectedLightbulb;
        [SerializeField] private Transform connectionPoint;

        private Animator animator;
        private bool canInteract;
        private static readonly int IsAttachingHash = Animator.StringToHash("isAttaching");

        public Lightbulb ConnectedLightbulb => connectedLightbulb;
        public bool IsLightbulbConnected => connectedLightbulb;

        protected override void Awake()
        {
            base.Awake();

            animator = GetComponent<Animator>();
            canInteract = true;
        }

        public void OnInteract(Player player)
        {
            if (!player || !canInteract || connectedLightbulb || !player.IsHoldingItemType<Lightbulb>()) { return; }

            if (player.TryInstantiateHeldItem(out Lightbulb lightbulb))
            {
                lightbulb.gameObject.SetActive(false);
                if (lightbulb.Connect(this))
                {
                    canInteract = false;
                    animator.SetBool(IsAttachingHash, true);
                }
            }
        }

        public void OnSecondaryInteract(Player player)
        {
            if (!player || !canInteract || !connectedLightbulb) { return; }

            Destroy(connectedLightbulb.gameObject);
            if (connectedLightbulb.Disconnect(this))
            {
                canInteract = false;
                animator.SetBool(IsAttachingHash, false);
            }
        }

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

        protected override void Cleanup()
        {
            base.Cleanup();

            DisconnectLightbulb();
        }

        private void OnAnim_LightbulbAttached()
        {
            canInteract = true;
            connectedLightbulb.transform.SetPositionAndRotation(connectionPoint.position, connectionPoint.rotation);
            connectedLightbulb.gameObject.SetActive(true);

            if (IsPowered)
            {
                connectedLightbulb.Light.enabled = true;
            }
        }

        private void OnAnim_LightbulbDetached()
        {
            canInteract = true;
        }
    }
}
