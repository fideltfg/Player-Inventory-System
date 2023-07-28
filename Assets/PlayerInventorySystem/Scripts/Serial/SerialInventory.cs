namespace PlayerInventorySystem.Serial
{
    using System;
    using UnityEngine;

    [Serializable]
    internal class SerialInventory
    {
        [SerializeField] private int index;
        [SerializeField] private SerialSlot[] slots;

        /// <summary>
        /// The inventory's index/ID.
        /// </summary>
        public int Index
        {
            get { return index; }
            set { index = value; }
        }

        /// <summary>
        /// An array of the slots that make up this inventory.
        /// </summary>
        public SerialSlot[] SerialSlots
        {
            get { return slots; }
            set { slots = value; }
        }

        /// <summary>
        /// Constructor to create a new SerialInventory with the provided index and slots.
        /// </summary>
        /// <param name="index">The index/ID of the inventory.</param>
        /// <param name="slots">An array of SerialSlot objects representing the slots in the inventory.</param>
        public SerialInventory(int index, SerialSlot[] slots = null)
        {
            Index = index;
            if (slots != null)
            {
                SerialSlots = slots;
            }
            else
            {
                SerialSlots = new SerialSlot[InventoryController.PlayerInventoryCapacity];
            }
        }

        public SerialInventory(Inventory inventory)
        {
            if (inventory == null)
            {
                inventory = new Inventory(InventoryController.InventoryList.Count, InventoryController.PlayerInventoryCapacity);
            }
            
            SerialSlots = new SerialSlot[inventory.Count];
            for (int s = 0; s < SerialSlots.Length; s++)
            {
                if (inventory[s].Item != null)
                {
                    SerialSlots[s] = new SerialSlot(inventory[s].SlotID, inventory[s].Item.Data.id, inventory[s].StackCount, inventory[s].Item.Durability);
                }
                else
                {
                    SerialSlots[s] = new SerialSlot(inventory[s].SlotID, 0, 0, 0);
                }
            }
        }
    }
}