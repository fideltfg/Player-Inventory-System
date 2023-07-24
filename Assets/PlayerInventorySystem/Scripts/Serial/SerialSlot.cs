namespace PlayerInventorySystem.Serial
{
    using System;
    using UnityEngine;

    [Serializable]
    internal class SerialSlot
    {
        [SerializeField] private int slotID;
        [SerializeField] private int itemID;
        [SerializeField] private int stackCount;
        [SerializeField] private float durability;

        /// <summary>
        /// The ID of the slot.
        /// </summary>
        public int SlotID
        {
            get { return slotID; }
            set { slotID = value; }
        }

        /// <summary>
        /// The ID of the item in the slot.
        /// </summary>
        public int ItemID
        {
            get { return itemID; }
            set { itemID = value; }
        }

        /// <summary>
        /// The number of items in the slot's stack.
        /// </summary>
        public int StackCount
        {
            get { return stackCount; }
            set { stackCount = value; }
        }

        /// <summary>
        /// The durability of the item in the slot.
        /// </summary>
        public float Durability
        {
            get { return durability; }
            set { durability = value; }
        }

        /// <summary>
        /// Constructor to create a new SerialSlot with the provided slot data.
        /// </summary>
        /// <param name="slotID">The ID of the slot.</param>
        /// <param name="itemID">The ID of the item in the slot.</param>
        /// <param name="stackCount">The number of items in the slot's stack.</param>
        /// <param name="durability">The durability of the item in the slot.</param>
        public SerialSlot(int slotID, int itemID, int stackCount, float durability)
        {
            SlotID = slotID;
            ItemID = itemID;
            StackCount = stackCount;
            Durability = durability;
        }
 
    }

}