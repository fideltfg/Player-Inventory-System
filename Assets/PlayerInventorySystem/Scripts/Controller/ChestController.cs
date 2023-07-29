using System;
using UnityEngine;
namespace PlayerInventorySystem
{

    /// <summary>
    /// Controller for chest objects. Place this compentent of object you wish to set up as a chest.
    /// </summary>
    public class ChestController : PlacedItem
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

        public int ChestRange = 3;

        public bool Open = false;

        /// <summary>
        /// points to InventoryController.ChestList entry for this chest.
        /// </summary>
        public Inventory Inventory
        {
            get { return InventoryController.GetChestInventory(ChestID); }
            set { InventoryController.ChestInventories[ChestID] = value; }
        }



        private void Start()
        {
            gameObject.tag = "Chest";
        }


        private void Update()
        {
            // if the chest is open test if the player is winthin range and if not close the chest
            if (Open && Vector3.Distance(transform.position, InventoryController.Instance.Player.transform.position) > ChestRange)
            {
                transform.Find("Lid").GetComponent<Animator>().SetBool("Open", false);
                InventoryController.Instance.ChestPanel.gameObject.SetActive(false);
            }
        }



        private void OnEnable()
        {
            // initialize chest inventory
            if (!InventoryController.ChestInventories.ContainsKey(ChestID))
            {
                Debug.Log("ChestController: ChestID " + ChestID + " not found in ChestInventories, creating new inventory");
                InventoryController.ChestInventories.Add(ChestID, new Inventory(ChestID, 24));
                InventoryController.ChestMap.Add(ChestID, gameObject);
            }
        }

        public void DestroyChest()
        {
            foreach (Slot s in Inventory)
            {
                if (s.Item != null)
                {
                    InventoryController.Instance.SpawnDroppedItem(s.Item.Data.id, transform.position, s.StackCount);
                    s.SetItem(null);
                }
            }
            InventoryController.ChestInventories.Remove(ChestID);
            InventoryController.ChestMap.Remove(ChestID);
        }

    }
}