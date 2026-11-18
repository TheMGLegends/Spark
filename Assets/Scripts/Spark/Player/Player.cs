using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

namespace Spark
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private List<GameObject> hotbarItems = new();

        private int selectedHotbarSlotIndex = 0;

        public event Action<string> OnHotbarSlotSelected;

        public GameObject HeldItem { get; set; }
        public GameObject HeldItemType => hotbarItems[selectedHotbarSlotIndex];
        public string HeldItemName => hotbarItems[selectedHotbarSlotIndex].name;

        private void Start()
        {
            SelectHotbarSlot(0);
        }

        private void LateUpdate()
        {
            if (TryGetHeldItem(out Wire wire))
            {
                Spline spline = wire.SplineContainer.Spline;
                int lastIndex = spline.Count - 1;

                BezierKnot playerKnot = spline[lastIndex];
                playerKnot.Position = transform.position;
                spline.SetKnot(lastIndex, playerKnot);

                wire.SplineExtrude.Rebuild();
            }
        }

        public void SelectHotbarSlot(int slotIndex)
        {
            selectedHotbarSlotIndex = Mathf.Clamp(slotIndex, 0, hotbarItems.Count - 1);
            OnHotbarSlotSelected?.Invoke(HeldItemName);
        }

        public bool IsHoldingItemType<T>()
        {
            return HeldItemType.TryGetComponent<T>(out _);
        }

        public bool TryGetHeldItem<T>(out T item) where T : MonoBehaviour
        {
            if (HeldItem && HeldItem.TryGetComponent(out item))
            {
                return true;
            }

            item = null;
            return false;
        }

        public bool TryInstantiateHeldItem<T>(out T item) where T : MonoBehaviour
        {
            HeldItem = Instantiate(HeldItemType);
            return TryGetHeldItem(out item);
        }
    }
}
