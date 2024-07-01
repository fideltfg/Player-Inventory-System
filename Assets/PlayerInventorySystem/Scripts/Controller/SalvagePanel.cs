
using System.Linq;
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
        public SalvageSlotController inputSlotController; // Controller for the input slot
        public Transform OutputArray; // Transform for the output array
        private bool validSalvage = false; // Indicates if the current recipe is valid for salvage

        /// <summary>
        /// Called when the panel is disabled
        /// </summary>
        public override void OnDisable()
        {
            // Move remaining items back to the inventory or drop them on the ground
            if (inputSlotController.Slot.Item != null)
            {
                var item = inputSlotController.Slot.Item;
                if (!InventoryController.GiveItem(item.Data.id, item.StackCount, item.Durability))
                {
                    InventoryController.Instance.PlayerInventoryControler.DropItem(item, item.StackCount, item.Durability);
                }
                inputSlotController.Slot.SetItem(null);
            }

            // Clear the output slots
            foreach (SlotController sc in SlotList)
            {
                sc.Slot.SetItem(null);
            }

            base.OnDisable();
        }

        /// <summary>
        /// Builds the salvage panel
        /// </summary>
        /// <param name="inventoryIndex">Index of the inventory</param>
        public override void Build(int inventoryIndex)
        {
            Index = inventoryIndex;
            inputSlotController.Index = Index;
            inputSlotController.SetSlot(InventoryController.SalvageInputInventory[0]);
            inputSlotController.Slot.RegisterSlotChangedCallback(SlotChangeCallback);

            // Initialize the output slots
            foreach (Slot slot in InventoryController.SalvageOutputInventory)
            {
                var slotGO = Instantiate(SlotPrefab, Vector3.zero, Quaternion.identity, OutputArray);
                slotGO.SetActive(true);
                var slotController = slotGO.GetComponent<SlotController>();
                slotController.interactable = false;
                slotController.Index = this.Index + 1; // Assuming the inventory for salvage panel is the next index
                slotController.SetSlot(slot);
                SlotList.Add(slotController);
            }
        }

        private int[] salvagableItemIDs;

        /// <summary>
        /// Callback for when the slot changes
        /// </summary>
        /// <param name="slot">The changed slot</param>
        public void SlotChangeCallback(Slot slot)
        {
            // Reset all output slots
            foreach (var sc in SlotList)
            {
                sc.outline.enabled = false;
                sc.Slot.SetItem(null);
            }

            // If slot or item is null, mark as invalid salvage
            if (slot?.Item == null)
            {
                validSalvage = false;
                return;
            }

            // If item requires a furnace, mark as invalid salvage as the item cannot be salvaged
            if (slot.Item.Data.requiresFurnace)
            {
                validSalvage = false;
                return;
            }

            // Get the required item IDs for salvage
            salvagableItemIDs = slot.Item.Data.recipe.GetRequiredItemIds();
            //Debug.Log("Salvage item ids: " + string.Join(" ", salvagableItemIDs));

            // Check if there are valid items for salvage
            validSalvage = salvagableItemIDs.Length >= 1 && salvagableItemIDs.Any(id => id != 0);

            // Process each slot
            for (int i = 0; i < SlotList.Count; i++)
            {
                var sc = SlotList[i];
                int itemId = salvagableItemIDs[i];

                // Skip if item ID is 0
                if (itemId == 0) continue;

                var item = Item.New(itemId);
                sc.Slot.SetItem(item);

                // Skip if item is null
                if (item == null) continue;

                // Set outline color based on whether the item is salvageable
                sc.SetOutLineColor(item.Data.salvageable ? sc.ValidColor : sc.ErrorColor);
            }
        }

        /// <summary>
        /// Handles the salvage button click
        /// </summary>
        public void SalvageButton()
        {
            // If salvage is valid, process the output slots
            if (!validSalvage) return;


            // loop through the salvagableItemIDs for each item in the input slot and give the items to the player that can me salvaged
            for (int i = 0; i < inputSlotController.Slot.Item.StackCount; i++) {
                foreach (var id in salvagableItemIDs)
                {
                    var item = Item.New(id);
                    if (item != null && item.Data.salvageable)
                    {
                        if (InventoryController.Instance.UsePlayerInventory == true)
                        {
                             InventoryController.PlayerInventory.AddItem(item);
                        }
                    }
                }
            }

            // Clear the input slot
            if (inputSlotController.Slot.Item != null)
            {
                inputSlotController.Slot.SetItem(null);
            }
        }
    }
}
