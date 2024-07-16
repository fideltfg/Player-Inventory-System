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
        [Tooltip("The ID of this chest and also the Index of its InventoryController.ChestList entry.")]
        public int ID = 0;

        /// <summary>
        /// The number of slots in this chest
        /// </summary>
        internal int Capacity = 24;

        /// <summary>
        /// Indicates if open or not.
        /// </summary>
        internal bool Open = false;

        /// <summary>
        /// points to InventoryController.ChestList entry for this chest.
        /// </summary>
        public Inventory Inventory
        {
            get { return InventoryController.GetChestInventory(ID); }
            set { InventoryController.ChestInventories[ID] = value; }
        }

        public virtual void Start()
        {
            gameObject.tag = "Chest";
        }

        public virtual void Update()
        {
            // if the chest is open test if the player is winthin range and if not close the chest
            if (Open && Vector3.Distance(transform.position, InventoryController.Instance.Player.transform.position) > 2)
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
            Panel = InventoryController.Instance.ChestPanel;

            // initialize chest inventory
            if (!InventoryController.ChestInventories.ContainsKey(ID))
            {
                InventoryController.ChestInventories.Add(ID, new Inventory(ID, 24));
                InventoryController.ChestMap.Add(ID, gameObject);
            }
        }

        internal void EmptyChest()
        {
            foreach (Slot s in Inventory)
            {
                if (s.Item != null)
                {
                    InventoryController.Instance.SpawnItem(s.Item.Data.id, transform.position + transform.up, s.StackCount, s.Item.Durability);
                    s.SetItem(null);
                }
            }
        }

        public override void OnDestroy()
        {
            InventoryController.ChestInventories.Remove(ID);
            InventoryController.ChestMap.Remove(ID);
        }

        public override void Interact(PlayerInventoryController playerInventoryController)
        {
            Debug.Log("Interacting with Chest");
            InventoryController.Instance.ChestPanel.Chest = this; // pass the selected chest to the chest panel
            InventoryController.Instance.ChestPanel.OpenCloseChestLid(true);
            InventoryController.Instance.ChestPanel.gameObject.SetActive(true);
        }

        internal override void TakeDamage(float damage)
        {
            Debug.Log("Chest ID " + ID + " took " + damage + " damage");

            durability -= damage;

            if (durability <= 0)
            {
                EmptyChest();

                InventoryController.Instance.SpawnItem(ItemID, transform.position + transform.up, 1);

                Destroy(gameObject);
            }
        }
    }
}