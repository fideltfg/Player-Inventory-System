using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlayerInventorySystem
{
    /// <summary>
    /// Component added to player object to allow items to be picked up.
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class InventoryPlayerController : MonoBehaviour
    {
        /// <summary>
        /// Ceter offset for collider
        /// </summary>
        Vector3 center = new Vector3(0, -.5f, 0);
        /// <summary>
        /// Size of the collider
        /// </summary>
        Vector3 size = new Vector3(2, 1, 2);

        /// <summary>
        /// Provides access to the values added from equipping items on the charater panel
        /// </summary>
        public Dictionary<string, float> BuffValues
        {
            get { return InventoryController.Instance.CharacterPanel.buffValues; }
        }

        public AudioClip pickupSound;

        void OnEnable ()
        {
            BoxCollider bc = GetComponent<BoxCollider>();
            bc.isTrigger = true;
            bc.center = center;
            bc.size = size;
        }

        private void OnTriggerEnter (Collider other)
        {
            InventoryController.PickUpItem(other.gameObject);
        }

        /// <summary>
        /// method to place an item in to your game world.
        /// </summary>
        /// <param name="item">The item that is to be placed.</param>
        /// <param name="hit">The hit point representing where the player clicked int he game world</param>
        /// <returns></returns>
        public bool PlaceItemInWorld (Item item, RaycastHit hit)
        {
            GameObject go = GameObject.Instantiate(item.data.worldPrefab, hit.point, Quaternion.identity);
            InventoryController.ItemPlaced(item, go);
            return true;
        }
    }
}
