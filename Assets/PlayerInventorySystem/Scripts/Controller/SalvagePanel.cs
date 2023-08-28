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
        public SlotController inputSlotController;
        public Text output;
        public Button salvageButton;
        //   public CraftingTableController CraftingTable;

        private bool validSalvage = false;

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
            // clear the output slots
            foreach (SlotController sc in SlotList)
            {
                sc.Slot.SetItem(null);
            }

            base.OnDisable();
        }

        public override void Build(int InventoryIndex)
        {
            Index = InventoryIndex;
            inputSlotController.Index = Index;
            inputSlotController.SetSlot(InventoryController.SalvageInputInventory[0]);
            inputSlotController.Slot.RegisterSlotChangedCallback(SlotChangeCallback);

            Transform InputArry = transform.Find("OutputArray");
            foreach (Slot slot in InventoryController.SalvageOutputInventory)
            {
                GameObject go = Instantiate(SlotPrefab, Vector3.zero, Quaternion.identity, InputArry);
                SlotController sc = go.GetComponent<SlotController>();
                sc.interactable = false;
                sc.Index = this.Index + 1; //assuming the inventory for salvage panel is the next index, not a goot thing to do but it works for now
                sc.SetSlot(slot);
                SlotList.Add(sc);
            }


        }

        public void SlotChangeCallback(Slot slot)
        {
            Debug.Log("Salvage slot changed");
            if (slot != null)
            {
                if (slot.Item != null)
                {
                    Debug.Log("Salvage item " + slot.Item.Data.name);
                    // get the itme recipe
                    int[] salvagableItemIDs = slot.Item.Data.recipe.GetRequiredItemIds();

                    // check the recipe is valid
                    if (salvagableItemIDs.Length < 1 || salvagableItemIDs.Any(num => num != 0) == false)
                    {
                        validSalvage = false;
                        return;
                    }

                    validSalvage = true;

                    int i = 0;
                    foreach (SlotController sc in SlotList)
                    {
                        Item item = InventoryController.Instance.ItemCatalog.GetItemByID(salvagableItemIDs[i]);
                        sc.Slot.SetItem(item);
                        i++;
                    }
                }
                else
                {
                    validSalvage = false;
                    // clear the output slots
                    foreach (SlotController sc in SlotList)
                    {
                        sc.Slot.SetItem(null);
                    }
                }
            }
        }

        public void SalvageButton()
        {
            // spawn the salvage output items
            if (validSalvage)
            {
                foreach (SlotController sc in SlotList)
                {
                    if (sc.Slot.Item != null)
                    {
                        InventoryController.Instance.PlayerIC.DropItem(sc.Slot.Item, sc.Slot.Item.StackCount);
                        sc.Slot.SetItem(null);
                    }
                }

                // clear the input slot
                if (inputSlotController.Slot.Item != null)
                {
                    inputSlotController.Slot.SetItem(null);
                }
            }

        }



    }
}
