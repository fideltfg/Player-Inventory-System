using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PlayerInventorySystem
{

    /// <summary>
    /// Controller for allowing player to click past the inventory panels to drop an item
    /// </summary>
    public class DropPanel : MonoBehaviour, IPointerDownHandler
    {
        public void OnPointerDown (PointerEventData eventData)
        {
          
            if (InventoryController.HeldItem != null)
            {
                if (eventData.button == PointerEventData.InputButton.Left)
                {
                    InventoryController.DropItem(InventoryController.HeldItem, InventoryController.HeldItem.StackCount);
                    InventoryController.HeldItem = null;
                }
                // if no shift on right click
                else if (eventData.button == PointerEventData.InputButton.Right)
                {
                    InventoryController.DropItem(InventoryController.HeldItem);
                    if (InventoryController.HeldItem.StackCount == 1)
                    {
                        InventoryController.HeldItem = null;
                    }
                    else
                    {
                        InventoryController.HeldItem.AddToStack(-1);
                    }
                }
            }

        }
    }
}