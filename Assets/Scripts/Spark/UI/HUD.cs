using TMPro;
using UnityEngine;

namespace Spark
{
    public class HUD : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI selectedItemText;

        private Player player;

        private void Start()
        {
            player = FindFirstObjectByType<Player>();
            if (player)
            {
                player.OnHotbarSlotSelected += Player_OnHotbarSlotSelected;
            }
        }

        private void OnDestroy()
        {
            if (player)
            {
                player.OnHotbarSlotSelected -= Player_OnHotbarSlotSelected;
            }
        }

        private void Player_OnHotbarSlotSelected(string selectedItemName)
        {
            if (!selectedItemText) { return; }

            selectedItemText.text = selectedItemName;
        }
    }
}
