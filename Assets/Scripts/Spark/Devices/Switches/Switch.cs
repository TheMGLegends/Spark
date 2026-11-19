using UnityEngine;

namespace Spark
{
    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(Animator))]
    public class Switch : ElectricalDeviceBase, IInteractable
    {
        [SerializeField, ReadOnly] private bool isClosed;

        private Animator animator;
        private static readonly int IsSwitchingHash = Animator.StringToHash("isSwitching");

        public bool IsClosed => isClosed;

        protected override void Awake()
        {
            base.Awake();

            animator = GetComponent<Animator>();
        }

        public void OnInteract(Player player)
        {
            animator.SetTrigger(IsSwitchingHash);
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
                Message.LogError($"{gameObject.name} is a {typeof(Switch).Name}, but has {neutralTerminalsCount} {CircuitRole.Neutral} terminals. This type of device requires two {CircuitRole.Live} terminals and optionally a {CircuitRole.Earth} terminal.");
                return;
            }
        }
    }
}
