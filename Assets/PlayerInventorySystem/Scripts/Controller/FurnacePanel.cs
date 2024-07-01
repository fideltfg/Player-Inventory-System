using UnityEngine;
using UnityEngine.UI;

namespace PlayerInventorySystem
{
    // This class is responsible for controlling the furnace panel in the game
    public class FurnacePanel : InventorySystemPanel
    {
        public SlotController FuelSlot;
        public SlotController MaterialSlot;
        public SlotController OutputSlot;


        // Variable to store the reference to the furnace
        FurnaceController furnace;

        // Property to get or set the Chest object
        public FurnaceController Furnace
        {
            get { return furnace; }
            set
            {
                // If the furnace is a new one
                if (furnace != value)
                {
                    // If we previously had a furnace clean that one up
                    if (furnace != null)
                    {
                    }

                    // Reference the new furnace
                    furnace = value;

                    // If the new furnace is valid
                    if (furnace != null)
                    {
                        // Populate the panel with the furnace's items
                        Populate(furnace);
                        // Open the furnace's lid
                    }
                    else
                    {
                        // Log an error if the furnace is null
                        Debug.LogError("Furnace is null");
                    }
                }
            }
        }

        // Method that runs when this object is enabled
        public override void OnEnable()
        {
            // Setup the UI
            base.OnEnable();
        }

        public override void Update()
        {
            // get the dslider from the output slot
            if (OutputSlot.dSlider != null)
            {
                if (furnace != null)
                {
                    if (furnace.isProcessing)
                    {
                        OutputSlot.dSlider.value = furnace.processTimer * (MaterialSlot.Slot.Item.Data.smeltTime / 100);
                    }
                    else
                    {
                        OutputSlot.dSlider.value = 0;
                    }
                }
            }

        }


        // Method to build the panel UI
        public override void Build(int InventoryIndex = 0)
        {


            FuelSlot.gameObject.SetActive(false);
            MaterialSlot.gameObject.SetActive(false);
            OutputSlot.gameObject.SetActive(false);


        }

        // Method to populate the furnace panel with items
        private void Populate(FurnaceController furnace)
        {

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


            // Discard the furnace
            furnace = null;
        }

    }
}
