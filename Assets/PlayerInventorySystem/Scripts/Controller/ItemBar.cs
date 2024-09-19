using System;
using UnityEditor.Rendering.BuiltIn.ShaderGraph;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerInventorySystem
{

    /// <summary>
    /// ItemBar Component Class
    /// </summary>
    public class ItemBar : InventorySystemPanel
    {
        public bool built = false;

        public override void OnEnable() { }

        public override void Update()
        {
            // TODO: Implement the Update method in the new input system
            // check if the user is scrolling the mouse wheel
            float s = Input.GetAxisRaw("Mouse ScrollWheel");
            if (s != 0)
            {
                // move the highlight left or right based on which way the wheel is scrolling
                if (s > 0)
                {
                    SelectedSlotID--;
                }
                else
                {
                    SelectedSlotID++;
                }
            }

        }

        public override void Build(int InventoryIndex)
        {
            base.Build(InventoryIndex);

            GridLayoutGroup.cellSize = SlotPrefab.GetComponent<RectTransform>().sizeDelta;

            // add the slot objects for the players inventory
            if (!built)
            {
                foreach (Slot slot in InventoryController.GetInventory(this.Index))
                {
                    GameObject go = GameObject.Instantiate(SlotPrefab, Vector3.zero, Quaternion.identity, transform);
                    SlotController sc = go.GetComponent<SlotController>();
                    sc.Index = this.Index;
                    sc.slotLocation = SLOTLOCATION.ITEMBAR;
                    sc.SetSlot(slot);
                    SlotList.Add(sc);

                }
                SlotList[0].Selected = true;
            }
            // TEST
            built = true;
        }

        /// <summary>
        /// method to drop the item in the currently selected slot
        /// </summary>
        public void DropSelectedItem()
        {
            if (SelectedSlotController.Slot.Item != null)
            {

                InventoryController.Instance.PlayerInventoryControler.DropItem(SelectedSlotController.Slot.Item, 1, SelectedSlotController.Slot.Item.Durability);

                SelectedSlotController.Slot.IncermentStackCount(-1);

                if (SelectedSlotController.Slot.Item.StackCount <= 0)
                {
                    SelectedSlotController.Slot.SetItem(null);
                }

            }
        }

        public void SelectNextSlot()
        {
            SelectedSlotID++;
        }

        public void SelectPreviousSlot()
        {
            SelectedSlotID--;
        }

        /// <summary>
        /// method to get the currently selected item (or stack of items) from the item bar.
        /// This methid will not remove the item from the item bar slot
        /// </summary>
        /// <returns>Item or null</returns>
        public Item GetSelectedItem()
        {
            return SelectedSlotController.Slot.Item;
        }

        /// <summary>
        /// method to consume the currently selected item (or stack of items) from the item bar.
        /// </summary>
        public void ConsumeSelectedItem()
        {
            Slot slot = SelectedSlotController.Slot;
            if (slot.Item != null)
            {
                slot.IncermentStackCount(-1);
                if (slot.Item.StackCount <= 0)
                {
                    slot.SetItem(null);
                }
            }
        }
    }
}
