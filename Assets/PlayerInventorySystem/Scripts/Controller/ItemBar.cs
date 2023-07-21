using System;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerInventorySystem
{

    /// <summary>
    /// ItemBar Component Class
    /// </summary>
    public class ItemBar : InventorySystemPanel
    {

        public override void OnEnable() { }

        public override void Update()
        {

            // check if the user is scrolling the mouse wheel
            float s = Input.GetAxisRaw("Mouse ScrollWheel");
            if (s != 0)
            {
                // move the high light left or right based on which way the wheel is scrolling
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

        /// <summary>
        /// method to drop the item in the currently selected slot
        /// </summary>
        public void DropSelectedItem()
        {
            if (SelectedSlotController.Slot.Item != null)
            {
                InventoryController.DropItem(SelectedSlotController.Slot.Item, 1);
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



        public void UseSelectedItem()
        {
            if (SelectedSlotController.Slot.Item != null)
            {
                switch (SelectedSlotController.Slot.Item.data.itemType)
                {
                    case ITEMTYPE.CONSUMABLE: // food water healt packs ect
                        Slot sS = SelectedSlotController.Slot;

                        if (sS.ItemStackCount <= 0)
                        {
                            sS.SetItem(null);
                        }
                        break;

                    case ITEMTYPE.WEARABLE: // aka equipable... armor, tools and weapons
                        break;

                    case ITEMTYPE.USABLE: // keys and other items that can be used in predefind ways

                        break;
                }


            }
        }

    }
}
