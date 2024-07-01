using UnityEngine;
using System.Collections;
using System;

namespace PlayerInventorySystem
{
    /// <summary>
    /// Component added to item prefabs so the inventroy system can identify them for saving and respawning
    /// </summary>
    public class PlacedItem : MonoBehaviour
    {
        public float durability = 100f;
        public int ItemID = 0;

        internal virtual void TakeDamage(float damage)
        {
            Debug.Log("Taking damage");
            durability -= damage;
            if (durability <= 0)
            {
                float dur = InventoryController.Instance.ItemCatalog.GetItemByID(ItemID).Data.maxDurability;

                InventoryController.Instance.SpawnItem(ItemID, transform.position + transform.up, 1, 30, dur);

                Destroy(gameObject);
            }
        }

        public virtual void OnDestroy()
        {
            if (InventoryController.Instance != null)
            {
                if (InventoryController.PlacedItems.Exists(x => x == this))
                {
                    InventoryController.PlacedItems.Remove(this);
                }
            }
        }
    }
}