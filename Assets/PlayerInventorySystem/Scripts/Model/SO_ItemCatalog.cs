using UnityEngine;
using System.Collections.Generic;
using System;

namespace PlayerInventorySystem
{
    /// <summary>
    /// Scripable object used to create the Item Catalog
    /// </summary>
    [Serializable]
    [CreateAssetMenu(menuName = "Inventory/Item Catalog")]
    public class SO_ItemCatalog : ScriptableObject
    {
        public int catalogID = 0;
        public List<ItemData> list = new List<ItemData>();

        public Item GetItemByID(int itemID)
        {
            return Item.New(itemID);
        }


        public Item GetRandomItem()
        {
            return Item.New(UnityEngine.Random.Range(1, list.Count), 1); 
        }

    }
}
