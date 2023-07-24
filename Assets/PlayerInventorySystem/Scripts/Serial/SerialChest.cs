namespace PlayerInventorySystem
{
    using System;
    using UnityEngine;

    [Serializable]
    internal class SerialChest
    {
        [SerializeField] private int chestID;
        [SerializeField] private int itemCatalogID;
        [SerializeField] private SerialTransform transform;
        [SerializeField] private SerialInventory inventory;

        /// <summary>
        /// The ID of the chest.
        /// </summary>
        public int ChestID
        {
            get { return chestID; }
            private set { chestID = value; }
        }

        /// <summary>
        /// The ID of the item catalog associated with the chest.
        /// </summary>
        public int ItemCatalogID
        {
            get { return itemCatalogID; }
            private set { itemCatalogID = value; }
        }

        /// <summary>
        /// The transform data (position, rotation, scale) of the chest.
        /// </summary>
        public SerialTransform Transform
        {
            get { return transform; }
            private set { transform = value; }
        }

        /// <summary>
        /// The inventory data associated with the chest.
        /// </summary>
        public SerialInventory Inventory
        {
            get { return inventory; }
            private set { inventory = value; }
        }

        /// <summary>
        /// Constructor to create a new SerialChest with the provided data.
        /// </summary>
        /// <param name="chestID">The ID of the chest.</param>
        /// <param name="itemCatalogID">The ID of the item catalog associated with the chest.</param>
        /// <param name="transform">The transform data (position, rotation, scale) of the chest.</param>
        /// <param name="inventory">The inventory data associated with the chest.</param>
        public SerialChest(int chestID, int itemCatalogID, SerialTransform transform, SerialInventory inventory)
        {
            ChestID = chestID;
            ItemCatalogID = itemCatalogID;
            Transform = transform;
            Inventory = inventory;
        }
    }

}