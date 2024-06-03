using UnityEngine;
using System.Collections.Generic;
using System;
using PlayerInventorySystem.Serial;
using static UnityEditor.Experimental.GraphView.Port;

namespace PlayerInventorySystem
{
    /// <summary>
    /// Class provides methods to store, modify and search items. 
    /// </summary>
    [Serializable]
    public class Inventory : List<Slot>, IList<Slot>
    {
        /// <summary>
        /// The index of this inventory in the InventoryController InventoryList, set at object creation
        /// </summary>
        public int Index { get; protected set; }

        /// <summary>
        /// New Inventory Constructor.
        /// </summary>
        /// <param name="index">The index that this inventory will reside in the InventoryList of InventoryController</param>
        /// <param name="capacity">The number of slots availabile in this inventory.</param>
        internal Inventory(int index, int capacity)
        {
            Index = index;
            for (int i = 0; i < capacity; i++)
            {
                Add(new Slot(i) { index = this.Index });
            }
        }

        /// <summary>
        /// Inventory Constructor from Serial Inventory
        /// </summary>
        /// <param name="sInventory"></param>
        internal Inventory(SerialInventory sInventory)
        {
            // Debug.Log("Creating Inventory from SerialInventory " + sInventory.Index);
            if (sInventory == null)
            {
                //  Debug.Log("SerialInventory was null, creating new SerialInventory");
                sInventory = new SerialInventory(InventoryController.InventoryList.Count, new SerialSlot[InventoryController.PlayerInventoryCapacity]);
            }

            // Set the index of this inventory to the index of the SerialInventory
            Index = sInventory.Index;

            // Create a new slot for each slot in the SerialInventory
            for (int i = 0; i < sInventory.SerialSlots.Length; i++)
            {
                // Create a new slot for each slot in the SerialInventory
                Add(new Slot(i) { index = this.Index });

                // Set the items in the slots of this inventory to the items in the SerialInventory
                SerialSlot sSlot = sInventory.SerialSlots[i];
                if (sSlot.ItemID > 0)
                {
                    Item newItem = Item.New(sSlot.ItemID, sSlot.StackCount);
                    newItem.Durability = sSlot.Durability;
                    this[sSlot.SlotID].SetItem(newItem);
                }
                else
                {
                    this[sSlot.SlotID].SetItem(null);
                }
            }
        }

        // Deligate to find stack of searchItem
        bool FindItemStack(Slot slot)
        {
            if (slot.Item == null)
            {
                return false;
            }
            if (searchItem.Data.id == slot.Item.Data.id)
            {
                return true;
            }
            return false;
        }
        // Deligate to check if slot item's stack has space

        bool SlotIsnotFull(Slot slot)
        {
            return !slot.Isfull;
        }
        // Deligate to check if given slot is empty

        bool SlotIsEmpty(Slot slot)
        {
            return slot.IsEmpty;
        }

        public bool InventoryContains(Item item)
        {
            // find all the current stacks of the item
            List<Slot> foundStacks = GetItemSlots(item);
            return foundStacks != null && foundStacks.Count > 0;
        }

        /// <summary>
        /// method to check if this inventory contains an item with the given ID
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public bool InventoryContains(int itemID)
        {
            return InventoryContains(Item.New(itemID));
        }

        /// <summary>
        /// method to return all the slots in the inventory that contain the given item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public List<Slot> GetItemSlots(Item item)
        {
            searchItem = item;

            // find all the current stacks of the item
            return FindAll(FindItemStack);
        }

        /// <summary>
        /// Method to place an item in to an empty slot of this inventory.
        /// If there are no empty slots this method will throw an exception.
        /// </summary>
        /// <param name="item">The Item to be placed in the inventory.</param>
        public bool PlaceItemInNewSlot(Item item)
        {
            Slot slot = Find(SlotIsEmpty);
            if (slot != null)
            {
                if (slot.SetItem(item) != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private Item searchItem;

        /// <summary>
        /// Method to add an item to this inventory. The item will be added to the the first existing slot that already contains an instance of the given item (stacked)
        /// Or if none exist it will be placed in an empty slot. If there are no empty slots this method will throw an exception.
        /// </summary>
        /// <param name="item">The item to be added.</param>
        public bool AddItem(Item item)
        {

            // if null item given return false
            if (item == null)
            {
                return true;
            }

            // if the item can not be stacked add it to a new slot
            if (!item.Stackable)
            {
                item.SetStackCount(1);
                return PlaceItemInNewSlot(item);
            }
            else
            {
                // if Item can be stacked
                // store the item for use in 'FindItemStack' Delegate
                searchItem = item;

                // find all the current stacks of the item
                List<Slot> foundStacks = FindAll(FindItemStack);

                // if there is a stack with some free space in it
                Slot slot = foundStacks.Find(SlotIsnotFull);
                if (slot != null)
                {

                    // move one item from the item stack in to the found stack with space until the stack is full or new item stack is empty
                    while (slot.StackCount < slot.Item.Data.maxStackSize && item.StackCount > 0)
                    {
                        slot.IncermentStackCount(1);
                        item.AddToStack(-1);

                        if (slot.selected)
                        {
                            InventoryController.Instance.OnSelectedItemChangeCallBack?.Invoke(slot.Item);
                        }

                    }

                    // if there are still items in the stack the repeat the process
                    if (item.StackCount > 0)
                    {
                        // repeat the process until the stack is empty
                        return AddItem(item);
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    // if no stack with free space place item in new slot
                    return PlaceItemInNewSlot(item);
                }
            }
        }

        /// <summary>
        /// method to return the number of empty slots availiable in this inventory.
        /// </summary>
        public int EmptySlotCount
        {
            get
            {
                int c = 0;
                foreach (Slot slot in this)
                {
                    if (slot.IsEmpty)
                    {
                        c++;
                    }
                }
                return c;
            }
        }

        /// <summary>
        /// Method to return an array of all the slots in this inventory that contain the item held in the given slot.
        /// If the given slot is from this invnetory it will be excluded from the array.
        /// </summary>
        /// <param name="slot">The slot containing the item to seach for.</param>
        /// <returns>Slot[]</returns>
        public Slot[] GetSlotsWithItem(Slot slot)
        {
            if (slot.Item != null)
            {
                List<Slot> slots = new List<Slot>();
                foreach (Slot s in this)
                {
                    if (!s.Equals(slot))
                    {
                        if (s.Item != null)
                        {
                            if (s.Item.Data.id == slot.Item.Data.id)
                            {
                                slots.Add(s);
                            }
                        }
                    }
                }

                if (slots.Count > 0)
                {
                    Slot[] sA = slots.ToArray();
                    Array.Sort(sA, delegate (Slot x, Slot y) { return x.StackCount.CompareTo(y.StackCount); });
                    return sA;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// Returns the count of the given item in this inventory.
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        internal int GetItemCount(int itemID)
        {

            // find all the current stacks of the item
            List<Slot> foundStacks = FindAll(delegate (Slot slot) { return slot.Item != null && slot.Item.Data.id == itemID; });
            if (foundStacks.Count > 0)
            {
                int count = 0;
                foreach (Slot slot in foundStacks)
                {
                    count += slot.StackCount;
                }
                return count;
            }
            return 0;
        }


    }
}
