//using PlayerInventorySystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using static UnityEngine.RuleTile.TilingRuleOutput;

namespace PlayerInventorySystem
{
    /// <summary>
    /// Controller for the player inventory panel.
    /// </summary>
    public class CatalogPanel : InventorySystemPanel
    {
        private Inventory inventory;
        private List<ItemData> itemCatalog
        {
            get
            {
                return InventoryController.Instance.ItemCatalog.list;
            }
        }
        private int itemCount = 0;


        public override void Update()
        {
            if (itemCatalog.Count != itemCount)
            {
                itemCount = itemCatalog.Count;

                InventoryController.ResizeInventory(InventoryController.GetInventory(this.Index), itemCount);
                InventoryController.GetInventory(this.Index).EmptyAllSlots();
                foreach (ItemData itemData in itemCatalog)
                {
                    if (itemData.id == 0)
                    {
                        continue;
                    }
                    int m = itemData.maxStackSize;
                    InventoryController.GetInventory(this.Index).AddItem(Item.New(itemData.id, m));
                }

                // remove existing slots
                foreach (SlotController sc in SlotList)
                {
                    sc.Slot.UnregisterSlotChangeCallback(sc.OnSlotChanged);
                    GameObject.Destroy(sc.gameObject);
                }

                // rebuild the slots
                SlotList = new List<SlotController>();
                Build(Index);

            }
        }

        public override void Build(int InventoryIndex)
        {

            base.Build(InventoryIndex);

            GridLayoutGroup.cellSize = SlotPrefab.GetComponent<RectTransform>().sizeDelta;
            GridLayoutGroup.constraintCount = 8;
            GetComponent<ContentSizeFitter>().enabled = true;
            // add the slot objects for the players inventory

            foreach (Slot slot in InventoryController.GetInventory(this.Index))
            {
                GameObject go = GameObject.Instantiate(SlotPrefab, Vector3.zero, Quaternion.identity, transform);
                SlotController sc = go.GetComponent<SlotController>();
                sc.Index = this.Index;
                sc.slotLocation = SLOTLOCATION.INVENTORY;
                sc.SetSlot(slot);
                SlotList.Add(sc);
            }

            SlotList[0].Selected = true;

        }
    }
}