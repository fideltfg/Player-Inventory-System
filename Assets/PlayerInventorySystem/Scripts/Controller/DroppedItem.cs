using UnityEngine;
using System.Collections;

namespace PlayerInventorySystem
{

    /// <summary>
    /// Component to add to items that are dropped in world to allow player to pick them up.
    /// This component will change the objects tag to "Item" when used.
    /// </summary>
    public class DroppedItem : MonoBehaviour
    {
        /// <summary>
        /// The ID the of the item the gameObject represents
        /// </summary>
        public int ItemID = 0;
        /// <summary>
        /// The stack count for the item
        /// </summary>
        public int StackCount = 1;

        /// <summary>
        /// The current durabilty value for this item.
        /// Does not apply if the stack count > 1
        /// </summary>
        public float Durability = 0;

        /// <summary>
        /// The amount of time this object will remain after being dropped befor despawning.
        /// </summary>
        /// 
        public float TimeToLive = 30;

        public float Timer = 0;

        /// <summary>
        /// holder for position data.
        /// used only when game loads saved inventory data. should be null otherwise.
        /// </summary>
        public Vector3 Position;


        private void OnEnable ()
        {
            gameObject.tag = "Item";
            Timer = 0;
        }

        private void Update ()
        {
            if (Timer >= TimeToLive)
            {
                InventoryController.Instance.DroppedItems.Remove(this);
                Destroy(gameObject);
            }
            else
            {
                Timer += Time.deltaTime;
            }
            Position = transform.position;
        }

        private void OnDestroy()
        {
            InventoryController.Instance.DroppedItems.Remove(this);
        }
    }
}
