using System;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerInventorySystem
{

    /// <summary>
    /// Controller for the Crafting panel
    /// 
    /// This class generates the crafting panels slot list and cleans up when the panel is closed.
    /// </summary>
    public class CraftingPanel : InventorySystemPanel
    {
        public SlotController outputSlot;

        public override void OnDisable ()
        {
            // move remaining items back to the inventory
            // if there is no room for it then drop it on the ground
            foreach (SlotController slotController in SlotList)
            {
                if (slotController.Slot.Item != null)
                {
                    if (InventoryController.PlayerInventory.AddItem(slotController.Slot.Item) == false)
                    {
                        InventoryController.DropItem(slotController.Slot.Item, slotController.Slot.Item.StackCount);
                    }
                    slotController.Slot.SetItem(null);
                }
            }
            base.OnDisable();
        }

        public override void Build (int InventoryIndex)
        {
            this.Index = InventoryIndex;
            Transform InputArry = transform.Find("InputArray");
            foreach (Slot slot in InventoryController.CraftingInventory)
            {
                GameObject go = GameObject.Instantiate(SlotPrefab, Vector3.zero, Quaternion.identity, InputArry);
                SlotController sc = go.GetComponent<SlotController>();
                sc.Index = this.Index;
                sc.SetSlot(slot);
                SlotList.Add(sc);
            }
            outputSlot.Index = 5;
            outputSlot.SetSlot(InventoryController.CraftingOutputInventory[0]);
        }


    }
}
