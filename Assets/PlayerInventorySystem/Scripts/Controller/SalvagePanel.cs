using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerInventorySystem
{
    /// <summary>
    /// Controller for the Crafting panel
    /// 
    /// This class generates the crafting panels slot list and cleans up when the panel is closed.
    /// </summary>
    public class SalvagePanel : InventorySystemPanel
    {
        public SalvageSlotController inputSlotController;
        public Transform OutputArray;
        private bool validSalvage = false;

        public override void OnDisable()
        {
            if (inputSlotController.Slot.Item != null)
            {
                var item = inputSlotController.Slot.Item;
                if (!InventoryController.GiveItem(item.Data.id, item.StackCount))
                {
                    InventoryController.Instance.PlayerInventoryControler.DropItem(item, item.StackCount);
                }
                inputSlotController.Slot.SetItem(null);
            }

            foreach (SlotController sc in SlotList)
            {
                sc.Slot.SetItem(null);
            }

            base.OnDisable();
        }

        public override void Build(int inventoryIndex)
        {
            Index = inventoryIndex;
            inputSlotController.Index = Index;
            inputSlotController.SetSlot(InventoryController.SalvageInputInventory[0]);
            inputSlotController.Slot.RegisterSlotChangedCallback(SlotChangeCallback);

            foreach (Slot slot in InventoryController.SalvageOutputInventory)
            {
                var slotGO = Instantiate(SlotPrefab, Vector3.zero, Quaternion.identity, OutputArray);
                slotGO.SetActive(true);
                var slotController = slotGO.GetComponent<SlotController>();
                slotController.interactable = false;
                slotController.Index = this.Index + 1;
                slotController.SetSlot(slot);
                SlotList.Add(slotController);
            }
        }

        public void SlotChangeCallback(Slot slot)
        {
            foreach (var sc in SlotList)
            {
                sc.outline.enabled = false;
                sc.Slot.SetItem(null);
            }

            if (slot?.Item == null)
            {
                validSalvage = false;
                return;
            }

            var salvagableItemIDs = slot.Item.Data.recipe.GetRequiredItemIds();
            Debug.Log("Salvage item ids: " + string.Join(" ", salvagableItemIDs));

            validSalvage = salvagableItemIDs.Length >= 1 && salvagableItemIDs.Any(id => id != 0);

            for (int i = 0; i < SlotList.Count; i++)
            {
                var sc = SlotList[i];
                int itemId = salvagableItemIDs[i];

                if (itemId == 0) continue;

                var item = Item.New(itemId);
                sc.Slot.SetItem(item);

                if (item == null) continue;

                sc.SetOutLineColor(item.Data.salvageable ? sc.ValidColor : sc.ErrorColor);
            }
        }

        public void SalvageButton()
        {
            if (!validSalvage) return;

            foreach (var sc in SlotList)
            {
                var item = sc.Slot.Item;
                if (item != null && item.Data.salvageable)
                {
                    InventoryController.Instance.PlayerInventoryControler.GiveItem(item);
                    sc.Slot.SetItem(null);
                }
            }

            if (inputSlotController.Slot.Item != null)
            {
                inputSlotController.Slot.SetItem(null);
            }
        }
    }
}
