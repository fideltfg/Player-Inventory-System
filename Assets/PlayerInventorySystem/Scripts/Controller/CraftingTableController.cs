using PlayerInventorySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerInventorySystem
{
    public class CraftingTableController : Interactive
    {

        internal override void ClosePanel()
        {
            if (InventoryController.Instance.CraftingPanel != null)
            {
                InventoryController.Instance.CraftingPanel.gameObject.SetActive(false);
            }
        }

        internal override void OpenPanel()
        {
            if (InventoryController.Instance.CraftingPanel != null)
            {
                InventoryController.Instance.CraftingPanel.gameObject.SetActive(true);
            }
        }
    }
}

