using UnityEngine;
using System;

namespace PlayerInventorySystem
{

    /// <summary>
    /// Class to represent the slot and all its methods.
    /// </summary>
    [Serializable]
    public class Slot
    {
        /// <summary>
        /// The id or index of this slot in the inventory for the panel inwhic it exits.
        /// </summary>
        public int SlotID;

        /// <summary>
        /// the index of the panel inventory
        /// </summary>
        public int index = 0;

        /// <summary>
        /// indicates if this the currently selected slot
        /// </summary>
        public bool selected = false;


        private Action<Slot> SlotChanged;

        /// <summary>
        /// The type of slot this is
        /// </summary>
        public SLOTTYPE SlotType = SLOTTYPE.INVENTORY;

        /// <summary>
        /// The the item contained in this slot.
        /// </summary>
        public Item Item { get; protected set; }

        /// <summary>
        /// contains the stack count of the item in this slot. 0 if slot is empty
        /// </summary>
        public int StackCount
        {
            get
            {
                if (Item != null)
                {
                    return Item.StackCount;
                }
                return 0;
            }
            protected set { }
        }

        /// <summary>
        /// True if the slot des not contain an item or the item stack is 0
        /// </summary>
        public bool IsEmpty { get { return Item == null || Item.StackCount <= 0; } }

        /// <summary>
        /// True if the stack count of the item in this slot id >= the max stack count of the item contained in this slot.
        /// </summary>
        public bool Isfull { get { return Item.StackCount >= Item.Data.maxStackSize; } }

        /// <summary>
        /// construct take the id of the slot.
        /// </summary>
        /// <param name="slotID"></param>
        public Slot (int slotID)
        {
            this.SlotID = slotID;
            SlotType = SLOTTYPE.INVENTORY;
        }

        /// <summary>
        /// method to set the item for this slot
        /// </summary>
        /// <param name="newItem">The item to be placed in this slot</param>
        /// <returns>The item placed in the slot</returns>
        public virtual Item SetItem (Item newItem)
        {
            if (newItem == null)
            {
                this.Item = null;
            }
            else if (newItem.Data.slotType == this.SlotType || this.SlotType == SLOTTYPE.INVENTORY)
            {
                this.Item = newItem;
            }

            if (SlotChanged != null)
            {
                SlotChanged(this);
            }

            if (this.selected)
            {
                if (InventoryController.Instance.OnSelectedItemChangeCallBack != null)
                {
                    InventoryController.Instance.OnSelectedItemChangeCallBack(this.Item);
                }
                
            }

            return this.Item;
        }

        /// <summary>
        /// method to set the stack count of the item in this slot.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool SetItemStackCount (int val)
        {
            if (Item != null)
            {
                if (Item.SetStackCount(val))
                {
                    SlotChanged(this);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// method to incerment the stack count of this slots item
        /// </summary>
        /// <param name="val"></param>
        /// <returns>true on success else false</returns>
        public bool IncermentStackCount (int val)
        {
            return SetItemStackCount(StackCount + val);
        }

        /// <summary>
        /// method to register a callback for when this slot or the item in it changes in anyway
        /// </summary>
        /// <param name="callback"></param>
        public void RegisterSlotChangedCallback (Action<Slot> callback)
        {
            SlotChanged += callback;
        }

        /// <summary>
        /// method to un register a slot change callback
        /// </summary>
        /// <param name="callback"></param>
        public void UnregisterSlotChangeCallback (Action<Slot> callback)
        {
            SlotChanged -= callback;
        }
    }
}
