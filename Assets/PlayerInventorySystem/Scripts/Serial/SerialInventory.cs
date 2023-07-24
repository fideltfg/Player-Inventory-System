namespace PlayerInventorySystem
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
        public SerialSlot[] Slots
        {
            get { return slots; }
            set { slots = value; }
        }

        /// <summary>
        /// Constructor to create a new SerialInventory with the provided index and slots.
        /// </summary>
        /// <param name="index">The index/ID of the inventory.</param>
        /// <param name="slots">An array of SerialSlot objects representing the slots in the inventory.</param>
        public SerialInventory(int index, SerialSlot[] slots)
        {
            Index = index;
            Slots = slots;
        }

        public SerialInventory()
        {
        }
    }

}