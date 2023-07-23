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
        public int stackCount = 1;

        /// <summary>
        /// The current durabilty value for this item.
        /// Does not apply if the stack count > 1
        /// </summary>
        public float durabilty = 0;


        /// <summary>
        /// The amount of time this object will remain after being dropped befor despawning.
        /// </summary>
        public float TTL = 30;

        public float timer = 0;



        /// <summary>
        /// holder for position data.
        /// used only when game loads saved inventory data. should be null otherwise.
        /// </summary>
        public Vector3 loadSpawnPoint;


        private void OnEnable ()
        {
            gameObject.tag = "Item";
            timer = 0;
        }

        private void Update ()
        {
            if (timer >= TTL)
            {
                InventoryController.DroppedItems.Remove(this);
                Destroy(gameObject);
            }
            else
            {
                timer += Time.deltaTime;
            }
            loadSpawnPoint = transform.position;
        }



    }
}
