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
        public ItemData Data;

        /// <summary>
        /// How many copies of this item are currently stacked here
        /// </summary>
        public int StackCount { get; protected set; }

        /// <summary>
        /// The current durability value of this item
        /// </summary>
        public float Durability = 0;

        /// <summary>
        /// indicates if the item can be stacked. 
        /// </summary>
        public bool Stackable
        {
            get { return Data.maxStackSize > 1; }
        }

        /// <summary>
        /// Method to return a new stack of an item created from the given data
        /// </summary>
        /// <param name="data">The Items data</param>
        /// <param name="count">the stack count</param>
        /// <returns>An Item or null</returns>
        public static Item New(ItemData data, int count = 1)
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
        public static Item New(int itemID, int count = 1)
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
        public Item(ItemData data, int count)
        {
            this.Data = data;
            StackCount = Mathf.Clamp(count, 1, data.maxStackSize);
            this.Durability = this.Data.maxDurability;
        }

        /// <summary>
        /// use Item.New to create items
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="count"></param>
        public Item(int itemID, int count)
        {
            this.Data = InventoryController.Instance.ItemCatalog.list[itemID];
            StackCount = Mathf.Clamp(count, 1, Data.maxStackSize);
            this.Durability = this.Data.maxDurability;
        }

        /// <summary>
        /// method to add a quantity to this items stack
        /// </summary>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public bool AddToStack(int quantity)
        {
            if (quantity <= (Data.maxStackSize - StackCount))
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
        public bool SetStackCount(int val)
        {
            if (Data.maxStackSize >= val)
            {
                StackCount = Mathf.Clamp(val, 0, Data.maxStackSize);
                return true;
            }
            Debug.LogWarning("SetStackCount value exceeds MaxStackSize!");
            return false;
        }

        /// <summary>
        /// method to clone this Item
        /// </summary>
        /// <returns>Memberwise clone of this Item</returns>
        public Item Clone()
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
                    if (data.id != this.Data.id)
                    {
                        if (data.recipe.InRecipe(this.Data.id))
                        {
                            used.Add(data.name);
                        }
                    }
                }
                return used.ToArray();
            }
        }

        /// <summary>
        /// Method to place AN item in the world
        /// </summary>
        /// <param name="item"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="scale"></param>
        /// <exception cref="NotImplementedException"></exception>
        internal static void Place(Item item, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            switch (item.Data.worldPrefab.tag.ToLower())
            {
                case "chest":
                    _ = InventoryController.SpawnChest(InventoryController.GetNewChestID(), item.Data.id, position, rotation, scale);
                    return;
                default:
                    GameObject go = GameObject.Instantiate(item.Data.worldPrefab, position, rotation);
                    PlacedItem pi = go.AddComponent<PlacedItem>();
                    InventoryController.OnPlaceItem(item, pi);
                    return;
            }
        }
    }
}