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

        public CraftingTableController CraftingTable;

        public override void Update()
        {
            Item craftedItem = GetItemByIngredients(GivenItemsAsRecipe(InventoryController.Instance.CraftingPanel.SlotList));
            if (outputSlot != null)
            {
                outputSlot.Slot.SetItem(craftedItem);
            }
        }

        public override void OnDisable()
        {
            // move remaining items back to the inventory
            // if there is no room for it then drop it on the ground
            foreach (SlotController slotController in SlotList)
            {
                if (slotController.Slot.Item != null)
                {
                    if(InventoryController.GiveItem(slotController.Slot.Item.Data.id, slotController.Slot.Item.StackCount)  == false)
                    {
                        InventoryController.Instance.PlayerIC.DropItem(slotController.Slot.Item, slotController.Slot.Item.StackCount);
                    }
                    slotController.Slot.SetItem(null);
                }
            }
            base.OnDisable();
        }

        public override void Build(int InventoryIndex)
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

        private string GivenItemsAsRecipe(List<SlotController> ingredients)
        {
            string recipe = "";

            foreach (SlotController sc in ingredients)
            {
                if (sc.Slot.Item == null)
                {
                    recipe += "X";
                }
                else
                {
                    string x = sc.Slot.Item.Data.id.ToString();
                    if ((x.Length & 1) != 0)
                    {
                        string c = x.PadLeft(2, '0');
                        recipe += c;
                    }
                    else
                    {
                        recipe += x;
                    }
                }
            }
            char[] t = { 'X' };
            return recipe.Trim(t);
        }

        private Item GetItemByIngredients(string ingredients)
        {
            if (ingredients.Length <= 0)
            {
                return null;
            }

            foreach (ItemData itemData in InventoryController.Instance.ItemCatalog.list)
            {
                if (itemData.recipe.Ingredients.Equals(ingredients))
                {
                    return Item.New(itemData, itemData.craftCount);
                }
            }
            return null;
        }

    }
}
