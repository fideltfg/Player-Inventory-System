using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using System;

namespace PlayerInventorySystem
{

    /// <summary>
    /// Controller for the Item holder. 
    /// Provides methods for items to be held.
    /// As well as to display item information when the cursor is hovered over a slot
    /// </summary>
    public class ItemHolder : MonoBehaviour
    {
        public ItemInfoBox itemInfoBox;
        public Image image;
        public Text text;

        public void Update ()
        {
            Item heldItem = InventoryController.HeldItem;

            if (heldItem != null && heldItem.StackCount > 0)
            {
                if (heldItem.StackCount <= 0)
                {
                    heldItem = null;
                }
                else
                {
                    image.sprite = heldItem.Data.sprite;
                    text.text = heldItem.StackCount.ToString();
                    image.enabled = true;
                    if (heldItem.StackCount < 2)
                    {
                        text.enabled = false;
                    }
                    else
                    {
                        text.enabled = true;
                    }
                }
            }
            else if (heldItem == null || heldItem.StackCount <= 0)
            {
                Cursor.visible = true;
                text.enabled = false;
                image.enabled = false;
                itemInfoBox.Show(null);
            }
            transform.position = Input.mousePosition;

        }

        private void FixedUpdate ()
        {
            transform.SetAsLastSibling();
        }
    }
}
