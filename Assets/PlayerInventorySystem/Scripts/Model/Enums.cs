using UnityEngine;
using System.Collections;

namespace PlayerInventorySystem
{

    public enum ITEMTYPE
    {
        CONSUMABLE, // item usch as food, cloth, rock or other consumable items
        USABLE, // items such as tools,  weapons and other none consumables.
        WEARABLE, //  clothing
        QUESTITEM, // items that are required for a quest, non consumable or usable.
        CHEST
    }

    public enum SLOTTYPE
    {
        INVENTORY, // items that can only be carried in the inventory
        HEAD, // items that can be used on the head or carried in the inventory
        BODY, // items that can be used on the body or carried in the inventory
        HANDS, // items that can be used in either hand or carried in the inventory
        LEFTHAND, // items that can only be used on the left hand or carried in the inventory
        RIGHTHAND,  // items that can only be used on the right hand or carried in the inventory
        LEGS, // items that can only be used on the legs for caiired in the inventory
        FEET // items that can onlt be used ont he feet or carried in the inventory
    }

    public enum SLOTLOCATION
    {
        CHEST,
        INVENTORY,
        ITEMBAR,
        CHARACTER
    }
}
