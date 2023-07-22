using System;
using UnityEngine;
namespace PlayerInventorySystem
{

    /// <summary>
    /// Controller for chest objects. Place this compentent of object you wish to set up as a chest.
    /// </summary>
    public class ChestController : MonoBehaviour
    {

        /// <summary>
        /// The ID of this chest and also the Index of its InventoryController.ChestList entry.
        /// </summary>
        public int ChestID = 0;

        /// <summary>
        /// this is the item ID for this chest in the item catalog.
        /// </summary>
        public int ItemCatalogID = 0;


        /// <summary>
        /// The number of slots in this chest
        /// </summary>
        public int Capacity = 24;



        /// <summary>
        /// points to InventoryController.ChestList entry for this chest.
        /// </summary>
        public Inventory Inventory
        {
            get { return InventoryController.GetChestInventory(ChestID); }
            set
            {
                if (InventoryController.ChestInventories.ContainsKey(ChestID))
                {
                    InventoryController.ChestInventories[ChestID] = value;
                }
                else
                {
                    InventoryController.ChestInventories.Add(ChestID, value);
                }

            }
        }

        private void OnEnable()
        {
            gameObject.tag = "Chest";
        }


        public void DestroyChest()
        {
            foreach (Slot s in Inventory)
            {
                if (s.Item != null)
                {
                    InventoryController.SpawnDroppedItem(s.Item.data.id, transform.position, s.ItemStackCount);
                    s.SetItem(null);
                }
            }
            InventoryController.ChestInventories.Remove(ChestID);
            InventoryController.ChestMap.Remove(ChestID);
        }

    }
}