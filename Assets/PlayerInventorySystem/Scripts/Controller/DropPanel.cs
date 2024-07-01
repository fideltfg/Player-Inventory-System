using UnityEngine;
using UnityEngine.EventSystems;

namespace PlayerInventorySystem
{

    /// <summary>
    /// Controller for allowing player to click past the inventory panels to drop an item
    /// </summary>
    public class DropPanel : MonoBehaviour, IPointerDownHandler
    {
        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("DropPanel.OnPointerDown");
            if (InventoryController.HeldItem != null)
            {
                if (eventData.button == PointerEventData.InputButton.Left)
                {
                    InventoryController.Instance.PlayerInventoryControler.DropItem(InventoryController.HeldItem, InventoryController.HeldItem.StackCount, InventoryController.HeldItem.Durability);
                    InventoryController.HeldItem = null;
                }
                // if no shift on right click
                else if (eventData.button == PointerEventData.InputButton.Right)
                {
                    InventoryController.Instance.PlayerInventoryControler.DropItem(InventoryController.HeldItem, 1, InventoryController.HeldItem.Durability);
                    // simulate throwing the first item from the stack and the next has full durability
                    InventoryController.HeldItem.Durability = InventoryController.HeldItem.Data.maxDurability;

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