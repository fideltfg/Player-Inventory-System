namespace PlayerInventorySystem.Serial
{
    using System;
    using UnityEngine;

    [Serializable]
    internal class SerialChest : SerialPlacedItem
    {
        [SerializeField] private int chestID;
        [SerializeField] private SerialInventory inventory;

        public SerialChest(int itemID, SerialTransform serialTransform) : base(itemID, serialTransform) { }

        /// <summary>
        /// The ID of the chest.
        /// </summary>
        internal int ChestID
        {
            get { return chestID; }
            set { chestID = value; }
        }

        /// <summary>
        /// The inventory data associated with the chest.
        /// </summary>
        internal SerialInventory Inventory
        {
            get { return inventory; }
            set { inventory = value; }
        }
    }
}