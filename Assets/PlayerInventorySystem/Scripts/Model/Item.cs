using UnityEngine;
using System;
using System.Collections.Generic;

namespace PlayerInventorySystem
{
    /// <summary>
    /// This class defines the basic item's value sand methods
    /// </summary>
    [Serializable]
    public class Item
    {
        /// <summary>
        /// This items catalog entry is stored here
        /// </summary>
        public ItemData data;

        /// <summary>
        /// How many copies of this item are currently stacked here
        /// </summary>
        public int StackCount { get; protected set; }

        /// <summary>
        /// The current durability value of this item
        /// </summary>
        public float durability = 0;

        /// <summary>
        /// indicates if the item can be stacked. 
        /// </summary>
        public bool Stackable
        {
            get { return data.maxStackSize > 1; }
        }

        /// <summary>
        /// Method to return a new stack of an item created from the given data
        /// </summary>
        /// <param name="data">The Items data</param>
        /// <param name="count">the stack count</param>
        /// <returns>An Item or null</returns>
        public static Item New (ItemData data, int count = 1)
        {
            if (data != null)
            {
                return new Item(data, count);
            }
            return null;
        }

        /// <summary>
        /// Method to return a new Item of the given ID with the givne stack count
        /// </summary>
        /// <param name="itemID">The ID of the Item to return</param>
        /// <param name="count">The stack count</param>
        /// <returns>An Item or null</returns>
        public static Item New (int itemID, int count = 1)
        {
            if (itemID > 0 && itemID < InventoryController.Instance.ItemCatalog.list.Count)
            {
                return new Item(itemID, count);
            }
            return null;
        }

        /// <summary>
        /// use Item.New to create items
        /// </summary>
        /// <param name="data"></param>
        /// <param name="count"></param>
        public Item (ItemData data, int count)
        {
            this.data = data;
            StackCount = Mathf.Clamp(count, 1, data.maxStackSize);
            this.durability = this.data.maxDurability;
        }

        /// <summary>
        /// use Item.New to create items
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="count"></param>
        public Item (int itemID, int count)
        {
            this.data = InventoryController.Instance.ItemCatalog.list[itemID];
            StackCount = Mathf.Clamp(count, 1, data.maxStackSize);
            this.durability = this.data.maxDurability;
        }

        /// <summary>
        /// method to add a quantity to this items stack
        /// </summary>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public bool AddToStack (int quantity)
        {
            if (quantity <= (data.maxStackSize - StackCount))
            {
                StackCount += quantity;
                return true;
            }
            return false;
        }

        /// <summary>
        /// methof to set the value of stackCount.
        /// </summary>
        /// <param name="val">The value to set</param>
        /// <returns>True if the value was succesfuly set. Else false</returns>
        public bool SetStackCount (int val)
        {
            if (data.maxStackSize >= val)
            {
                StackCount = Mathf.Clamp(val, 0, data.maxStackSize);
                return true;
            }
            Debug.LogWarning("SetStackCount value exceeds MaxStackSize!");
            return false;
        }

        /// <summary>
        /// method to clone this Item
        /// </summary>
        /// <returns>Memberwise clone of this Item</returns>
        public Item Clone ()
        {
            return (Item)this.MemberwiseClone();
        }

        /// <summary>
        /// Contains a list of items that need this item to be crafted
        /// </summary>
        public string[] Uses
        {
            get
            {
                List<string> used = new List<string>();
                foreach (ItemData data in InventoryController.Instance.ItemCatalog.list)
                {
                    if (data.id != this.data.id)
                    {
                        if (data.recipe.InRecipe(this.data.id))
                        {
                            used.Add(data.name);
                        }
                    }
                }
                return used.ToArray();
            }
        }
    }
}