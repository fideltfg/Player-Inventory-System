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
        /// <summary>
        /// indicates if the current recipe is valid for salvage
        /// </summary>
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

           // Transform InputArry = transform.Find("OutputArray");
            foreach (Slot slot in InventoryController.SalvageOutputInventory)
            {
                GameObject go = Instantiate(SlotPrefab, Vector3.zero, Quaternion.identity, OutputArray);
                go.SetActive(true);
                SlotController sc = go.GetComponent<SlotController>();
                sc.interactable = false;
                sc.Index = this.Index + 1; //assuming the inventory for salvage panel is the next index, not a goot thing to do but it works for now
                sc.SetSlot(slot);
                SlotList.Add(sc);
            }


        }

        public void SlotChangeCallback(Slot slot)
        {
            foreach (SlotController sc in SlotList)
            {
                sc.outline.enabled = false;
                sc.Slot.SetItem(null);
            }
            if (slot != null)
            {
                if (slot.Item != null)
                {
                    int[] salvagableItemIDs = slot.Item.Data.recipe.GetRequiredItemIds();
                  /*  string t = "";
                    foreach (int y in salvagableItemIDs)
                    {
                        t += y.ToString() + " ";
                    }
                    Debug.Log("Salvage item ids: " + t);
                  */
                    validSalvage = salvagableItemIDs.Length >= 1 && !salvagableItemIDs.All(num => num == 0);

                    int i = 0;
                    foreach (SlotController sc in SlotList)
                    {
                        Item item = Item.New(salvagableItemIDs[i]);
                        i++;

                        // set the put slot item
                        sc.Slot.SetItem(item);

                        // check if the item is not null
                        if (item == null)
                        {
                            continue;
                        }



                        // check if the item is salvageable and set the outline color accordingly
                        if (sc.Slot.Item.Data.salvageable)
                        {
                            sc.SetOutLineColor(sc.ValidColor);
                        }
                        else
                        {
                            sc.SetOutLineColor(sc.ErrorColor);
                        }

                    }
                }
                else
                {
                    validSalvage = false;
                    // clear the output slots
                   
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
                        if (sc.Slot.Item.Data.salvageable)
                        {
                            InventoryController.Instance.PlayerIC.DropItem(sc.Slot.Item, sc.Slot.Item.StackCount);
                        }
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
