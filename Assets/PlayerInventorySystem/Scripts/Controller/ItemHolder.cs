using UnityEngine;
using UnityEngine.UI;

namespace PlayerInventorySystem
{

    /// <summary>
    /// Controller for the Item holder. 
    /// Provides methods for items to be held.
    /// As well as to display item information when the cursor is hovered over a slot
    /// </summary>
    public class ItemHolder : MonoBehaviour
    {
        public ItemInfoBox itemInfoBox;  // Reference to the ItemInfoBox component
        public Image image;              // Reference to the Image component
        public Text text;                // Reference to the Text component

        // Update is called once per frame
        public void Update()
        {
            // Get the currently held item from the InventoryController
            Item heldItem = InventoryController.HeldItem;

            // Check if there is a held item and its stack count is greater than 0
            if (heldItem != null && heldItem.StackCount > 0)
            {
                UpdateItemDisplay(heldItem);
            }
            else
            {
                ClearItemDisplay();
            }

            // Update the position of the ItemHolder to the mouse position
            transform.position = Input.mousePosition;
        }

        // FixedUpdate is called at a fixed interval
        private void FixedUpdate()
        {
            // Ensure the ItemHolder is rendered last (on top of other UI elements)
            transform.SetAsLastSibling();
        }

        // Update the item display with the held item's details
        private void UpdateItemDisplay(Item heldItem)
        {
            image.sprite = heldItem.Data.sprite;  // Set the image sprite to the held item's sprite
            text.text = heldItem.StackCount.ToString();  // Set the text to display the stack count

            image.enabled = true;  // Enable the image component
            text.enabled = heldItem.StackCount > 1;  // Enable or disable the text component based on stack count
        }

        // Clear the item display when no item is held
        private void ClearItemDisplay()
        {
            Cursor.visible = InventoryController.Instance.AnyWindowOpen;  // Show/hide the cursor
            text.enabled = false;  // Disable the text component
            image.enabled = false;  // Disable the image component
            itemInfoBox.Show(null);  // Show the item info box with no item
        }
    }
}
