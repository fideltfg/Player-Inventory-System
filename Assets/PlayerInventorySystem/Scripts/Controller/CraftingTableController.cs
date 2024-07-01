using PlayerInventorySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerInventorySystem
{
    public class CraftingTableController : Interactive
    {
        private void OnEnable()
        {
            Panel = InventoryController.Instance.CraftingPanel;
        }

        internal override void ClosePanel()
        {
            if (Panel != null)
            {
                Panel.gameObject.SetActive(false);
            }
        }

        public override void Interact(PlayerInventoryController playerInventoryController)
        {
            Debug.Log("Interacting with Crafting Table");
            InventoryController.Instance.CraftingPanel.CraftingTable = this; // pass the selected chest to the chest panel
            InventoryController.Instance.CraftingPanel.gameObject.SetActive(true);
        }
    }
}

