using UnityEngine;

namespace PlayerInventorySystem
{
    public class SalvageSlotController : SlotController
    {

        internal override void SetOutLineColor()
        {
            if (HeldItem != null)
            {
                // if the item is recyclable its valid in this slot
                if (HeldItem.Data.recycable)
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


    }
}
