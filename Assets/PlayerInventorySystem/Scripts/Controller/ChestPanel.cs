using UnityEngine;
using UnityEngine.UI;

namespace PlayerInventorySystem
{
    // This class is responsible for controlling the chest panel in the game
    public class ChestPanel : InventorySystemPanel
    {
        // Variable to store the reference to the chest
        ChestController chest;

        // Property to get or set the Chest object
        public ChestController Chest
        {
            get { return chest; }
            set
            {
                // If the chest is a new one
                if (chest != value)
                {
                    // If we previously had a chest, close its lid
                    if (chest != null)
                    {
                        OpenCloseChestLid(false);
                    }

                    // Reference the new chest
                    chest = value;

                    // If the new chest is valid
                    if (chest != null)
                    {
                        // Populate the panel with the chest's items
                        Populate(chest);
                        // Open the chest's lid
                        OpenCloseChestLid(true);
                    }
                    else
                    {
                        // Log an error if the chest is null
                        Debug.LogError("Chest is null");
                    }
                }
            }
        }

        // Method that runs when this object is enabled
        public override void OnEnable()
        {
            // Setup the UI
            GridLayoutGroup.cellSize = SlotPrefab.GetComponent<RectTransform>().sizeDelta;
            GetComponent<ContentSizeFitter>().enabled = true;
            base.OnEnable();
        }

        // Method to build the panel UI
        public override void Build(int InventoryIndex = 0)
        {
            Index = InventoryIndex;

            // Create slots for the items in the chest
            for (int i = 0; i < 24; i++)
            {
                GameObject go = GameObject.Instantiate(SlotPrefab, Vector3.zero, Quaternion.identity, transform);
                SlotController sc = go.GetComponent<SlotController>();
                sc.Index = Index;
                sc.slotLocation = SLOTLOCATION.CHEST;
                SlotList.Add(sc);
                sc.gameObject.SetActive(false);
            }
        }

        // Method to populate the chest with items
        private void Populate(ChestController chest)
        {
            Index = chest.ID;

            Inventory inv = chest.Inventory;

            // Iterate over each item in the chest and add it to a slot
            for (int i = 0; i < chest.Capacity; i++)
            {
                SlotList[i].Index = Index;
                SlotList[i].SetSlot(inv[i]);
                SlotList[i].gameObject.SetActive(true);
            }
        }

        // Method that runs when this object is disabled
        public override void OnDisable()
        {
            // Disable all slots in preparation for the next time the panel is displayed
            foreach (SlotController sc in SlotList)
            {
                sc.outline.enabled = false;
                sc.gameObject.SetActive(false);
            }

            // Close the chest lid
            OpenCloseChestLid(false);

            // Discard the chest
            chest = null;
        }

        // Method to open or close the chest lid
        // Todo: should trigger a call to the chest controller to open or close the chest
        public void OpenCloseChestLid(bool v)
        {
            // If we have a valid chest
            if (chest != null)
            {
                // Find the "Lid" child of the chest and set its "Open" state
                chest.transform.Find("Lid").GetComponent<Animator>().SetBool("Open", v);
                chest.Open = v;
            }
        }
    }
}
