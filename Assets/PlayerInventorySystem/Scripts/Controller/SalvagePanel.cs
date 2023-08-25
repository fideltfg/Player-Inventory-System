using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace PlayerInventorySystem
{

    /// <summary>
    /// Controller for the Crafting panel
    /// 
    /// This class generates the crafting panels slot list and cleans up when the panel is closed.
    /// </summary>
    public class SalvagePanel : InventorySystemPanel
    {
        public SlotController inputSlotController;

        //   public CraftingTableController CraftingTable;

        public override void OnDisable()
        {
            // move remaining items back to the inventory
            // if there is no room for it then drop it on the ground

            if (inputSlotController.Slot.Item != null)
            {
                if (InventoryController.GiveItem(inputSlotController.Slot.Item.Data.id, inputSlotController.Slot.Item.StackCount) == false)
                {
                    InventoryController.Instance.PlayerIC.DropItem(inputSlotController.Slot.Item, inputSlotController.Slot.Item.StackCount);
                }
                inputSlotController.Slot.SetItem(null);
            }

            base.OnDisable();
        }

        public override void Build(int InventoryIndex)
        {
            Index = InventoryIndex;
            // set up th input slot
            Transform InputArray = transform.Find("InputArray");
            GameObject go = Instantiate(SlotPrefab, Vector3.zero, Quaternion.identity, InputArray);
            inputSlotController = go.GetComponent<SlotController>();
            inputSlotController.Index = Index;
            inputSlotController.SetSlot(InventoryController.SalvageInputInventory[0]);
            inputSlotController.Slot.RegisterSlotChangedCallback(SlotChangeCallback);
        }

        public void SlotChangeCallback(Slot slot)
        {
            if (slot != null)
            {
                if (slot.Item != null)
                {

                    // get the itme recipe
                    int[] salvagableItemIDs = slot.Item.Data.recipe.GetRequiredItemIds();

                    foreach (int id in salvagableItemIDs)
                    {
                        Debug.Log("Salvage item id " + id);

                    }



                }
            }
        }


    }
}
