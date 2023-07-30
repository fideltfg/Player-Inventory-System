using System;
using UnityEngine;
namespace PlayerInventorySystem
{

    /// <summary>
    /// Controller for chest objects. Place this compentent of object you wish to set up as a chest.
    /// </summary>
    public class ChestController : Interactive
    {

        /// <summary>
        /// The ID of this chest and also the Index of its InventoryController.ChestList entry.
        /// </summary>
        public int ChestID = 0;

        /// <summary>
        /// The number of slots in this chest
        /// </summary>
        public int Capacity = 24;

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


        public override void Update()
        {
            // if the chest is open test if the player is winthin range and if not close the chest
            if (Open && Vector3.Distance(transform.position, InventoryController.Instance.Player.transform.position) > Radius)
            {
                // trigger the lid closing animation
                transform.Find("Lid").GetComponent<Animator>().SetBool("Open", false);

                // close the chest panel
                ClosePanel();
            }
        }

        internal override void ClosePanel()
        {
            if (InventoryController.Instance.ChestPanel != null)
            {
                InventoryController.Instance.ChestPanel.gameObject.SetActive(false);
            }
        }


        private void OnEnable()
        {
            // initialize chest inventory
            if (!InventoryController.ChestInventories.ContainsKey(ChestID))
            {
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