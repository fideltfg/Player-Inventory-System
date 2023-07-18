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
        /*       public int selectedSlotID;

               /// <summary>
               /// ID of the slot the player hass currently selected.
               /// </summary>
               public int SelectedSlotID
               {
                   get
                   {
                       return selectedSlotID;
                   }
                   set
                   {
                       SlotList[selectedSlotID].Selected = false;
                       selectedSlotID = (int)Mathf.Clamp(value, 0, 9);
                       SlotList[selectedSlotID].Selected = true;
                       InventoryController.Instance.OnSelectedItemChangeCallBack?.Invoke(SelectedSlotController.Slot.Item);
                   }
               }

               /// <summary>
               /// The Slotcontroller of the currently selected slot.
               /// </summary>
               public SlotController SelectedSlotController
               {
                   get { return SlotList[SelectedSlotID]; }
               }*/

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
            else // if the wheel is not scrolling check for keyboard input
            {

                if (Input.GetKeyDown(KeyCode.Alpha1)) { SelectedSlotID = 0; }
                else if (Input.GetKeyDown(KeyCode.Alpha2)) { SelectedSlotID = 1; }
                else if (Input.GetKeyDown(KeyCode.Alpha3)) { SelectedSlotID = 2; }
                else if (Input.GetKeyDown(KeyCode.Alpha4)) { SelectedSlotID = 3; }
                else if (Input.GetKeyDown(KeyCode.Alpha5)) { SelectedSlotID = 4; }
                else if (Input.GetKeyDown(KeyCode.Alpha6)) { SelectedSlotID = 5; }
                else if (Input.GetKeyDown(KeyCode.Alpha7)) { SelectedSlotID = 6; }
                else if (Input.GetKeyDown(KeyCode.Alpha8)) { SelectedSlotID = 7; }
                else if (Input.GetKeyDown(KeyCode.Alpha9)) { SelectedSlotID = 8; }
                else if (Input.GetKeyDown(KeyCode.Alpha0)) { SelectedSlotID = 9; }
                else
                {
                    //  SelectedSlotID = selectedSlotID;
                    return;
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
