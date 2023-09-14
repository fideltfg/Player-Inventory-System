using UnityEngine;
using System.Collections.Generic;

namespace PlayerInventorySystem
{
    /// <summary>
    /// Scripable object used to create the Item Catalog
    /// </summary>
    [CreateAssetMenu()]
    public class SO_ItemList : ScriptableObject
    {
        public List<ItemData> list = new List<ItemData>();


        public Item GetItemByID(int itemID)
        {
            return Item.New(itemID);
        }
    }
}
