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
            if(Panel != null)
            {
                Panel.gameObject.SetActive(false);
            }
        }

        internal override void OpenPanel()
        {
            if(Panel != null)
            {
                  Panel.gameObject.SetActive(true);
            }
        }
    }
}

