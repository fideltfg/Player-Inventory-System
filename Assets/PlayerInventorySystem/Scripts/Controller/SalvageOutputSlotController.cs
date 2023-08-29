using PlayerInventorySystem;
using UnityEngine;


public class SalvageOutputSlotController : SlotController
{

    /// <summary>
    /// method called to update the slots UI
    /// </summary>
    internal override void UpdateSlotUI()
    {
        base.UpdateSlotUI();

        if (Slot != null && Slot.Item != null)
        {
            Item item = Slot.Item;

            if (item.Data.salvageable)
            {
                SetOutLineColor(ValidColor);
            }
            else
            {
                SetOutLineColor(ErrorColor);
            }
        }
    }
}
