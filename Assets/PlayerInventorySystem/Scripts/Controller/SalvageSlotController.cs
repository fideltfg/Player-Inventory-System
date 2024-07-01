using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PlayerInventorySystem
{
    public class SalvageSlotController : SlotController
    {

        internal override void SetOutLineColor()
        {
            if (HeldItem != null)
            {
                // if the item is recyclable its valid in this slot
                if (HeldItem.Data.recyclable)
                {
                    outline.effectColor = ValidColor;
                }
                else
                {
                    outline.effectColor = ErrorColor;
                }
                outline.enabled = true;
            }
            else
            {
                outline.effectColor = HighlightColor;
                outline.enabled = false;
            }

        }


        /// <summary>
        /// Method to handle mouse pointer down on this slot
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnPointerDown(PointerEventData eventData)
        {
            if (!interactable)
            {
                return;
            }

            counter = 0;
            mouseOver = false;
            // if NOT holding an item and there is an item in this slot
            if (HeldItem == null && Slot.Item != null)
            {
                // if the player is holding down shift place the item( or stack of items) in the slot
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    MoveStack(); // to the next available slot in the open window.
                }
                else // if no shift on left click
                {
                    if (eventData.button == PointerEventData.InputButton.Left)
                    {
                        //just pick up whats here
                        PickUpStack();
                    }
                    // if no shift on right click
                    else if (eventData.button == PointerEventData.InputButton.Right)
                    {
                        PickUpHalfStack();
                    }
                }
                // return;
            }

            // if we are holding an item (on the mouse)
            else if (HeldItem != null)
            {
                // check if the item can be recycled
                if (HeldItem.Data.recyclable)
                { 
                    // and this slot is not empty
                    if (Slot.Item != null)
                    {
                        // and the held Item is the same as the item in this slot
                        if (HeldItem.Data.id == Slot.Item.Data.id)
                        {
                            // if this stack is full
                            if (Slot.Item.StackCount >= Slot.Item.Data.maxStackSize)
                            {
                                // swap Items
                                Item i = HeldItem;
                                HeldItem = Slot.Item;
                                Slot.SetItem(i);
                            }
                            else
                            {

                                if (eventData.button == PointerEventData.InputButton.Left)
                                {
                                    while (HeldItem.StackCount > 0 && Slot.Item.StackCount < Slot.Item.Data.maxStackSize)
                                    {
                                        HeldItem.AddToStack(-1);
                                        Slot.IncermentStackCount(1);
                                    }

                                }
                                else if (eventData.button == PointerEventData.InputButton.Right)
                                {
                                    HeldItem.AddToStack(-1);
                                    Slot.IncermentStackCount(1);
                                }

                                if (HeldItem.StackCount <= 0)
                                {
                                    HeldItem = null;
                                }
                            }
                        }
                        else
                        {
                            if (HeldItem.Data.slotType == Slot.SlotType || Slot.SlotType == SLOTTYPE.INVENTORY)
                            {
                                // swap Items
                                Item i = HeldItem;
                                HeldItem = Slot.Item;
                                Slot.SetItem(i);
                            }
                        }
                    }
                    else if (Slot.Item == null) //  this slot has no item (empty)
                    {
                        if (Slot.SlotType == HeldItem.Data.slotType || Slot.SlotType == SLOTTYPE.INVENTORY)
                        {

                            if (eventData.button == PointerEventData.InputButton.Left)
                            {
                                // place the stack
                                Slot.SetItem(HeldItem);
                                HeldItem = null;
                            }
                            if (eventData.button == PointerEventData.InputButton.Right)
                            {
                                Debug.Log("Right Click");
                                // place one copy of the item in to this slot
                                Item CloneItem = HeldItem.Clone();
                                CloneItem.SetStackCount(1);
                                Slot.SetItem(CloneItem);

                                // FIX STACKING BUG
                                if (HeldItem.StackCount > 1)
                                {
                                    HeldItem.AddToStack(-1);
                                }
                                else
                                {
                                    HeldItem = null;
                                }
                            }
                        }
                    }
                }
            }
            SetOutLineColor();
        }


    }
}
