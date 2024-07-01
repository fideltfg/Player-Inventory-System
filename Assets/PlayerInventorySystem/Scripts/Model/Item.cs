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


        public Slot lastSlot;

        /// <summary>
        /// Method to return a new Item of the given ID with the given stack count
        /// </summary>
        /// <param name="itemID">The ID of the Item to return</param>
        /// <param name="count">The stack count</param>
        /// <returns>An Item with a stack count as given or null if invalid item ID</returns>
        public static Item New(int itemID, int count = 1)
        {
            ItemData data = InventoryController.Instance.ItemCatalog.list.Find(item => item.id == itemID);
            if (data != null)
            {
                return new Item(data, count);
            }
            Debug.LogWarning("Item ID not found in catalog: " + itemID);
            return null;

        }

        /// <summary>
        /// use Item.New to create items
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="count"></param>
        private Item(ItemData data, int count = 1)
        {
            this.Data = data;
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
        /// method to set the value of stackCount.
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
            //Debug.LogWarning("SetStackCount value exceeds MaxStackSize!");
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

        public int ItemID
        {
            get
            {
                return Data.id;
            }
            private set { }
        }

        /// <summary>
        /// Method to place AN item in the world and remove it from the inventory
        /// 
        /// dont use this for loading from serial data
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
                    InventoryController.OnPlaceItem(item);
                    break;

                case "craftingtable":
                    CraftingTableController cTc = InventoryController.SpawnCraftingTable(item.Data.id, position, rotation, scale);
                    InventoryController.OnPlaceItem(item, cTc);
                    break;

                default:
                    GameObject go = GameObject.Instantiate(item.Data.worldPrefab, position, rotation);
                    PlacedItem pi = go.AddComponent<PlacedItem>();
                    InventoryController.OnPlaceItem(item, pi);
                    break;
            }
        }

        internal static Item New(object itemID, int v)
        {
            throw new NotImplementedException();
        }
    }
}