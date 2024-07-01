using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

namespace PlayerInventorySystem
{

    /// <summary>
    /// The Base slot controller contains all methods for dealing with slots and provides access to items within the slot.
    /// </summary>
    public class CatalogSlotController : SlotController, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {

        protected override void PickUpHalfStack()
        {
            if (this.Slot.Item.StackCount == 1)
            {
                HeldItem = this.Slot.Item.Clone();
               // Slot.SetItem(null);
            }
            else
            {
                HeldItem = this.Slot.Item.Clone();

                Vector2Int v = SplitIntVectorInt(Slot.Item.StackCount);
                Slot.SetStackCount(v.x);
                HeldItem.SetStackCount(v.y);

                // if the stack is now empty  clear the slot
                if (Slot.Item.StackCount <= 0)
                {
                    //Slot.SetItem(null);
                }
            }
        }

        protected override void PickUpStack()
        {
            HeldItem = this.Slot.Item;
            //Slot.SetItem(null);
        }

    }
}