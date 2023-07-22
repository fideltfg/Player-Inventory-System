using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UIElements;
using Unity.VisualScripting;

namespace PlayerInventorySystem
{
    /// <summary>
    /// Component added to player object to allow items to be picked up.
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class PlayerInventoryController : MonoBehaviour
    {
        /// <summary>
        /// Ceter offset for collider
        /// </summary>
        Vector3 center = new Vector3(0, -.5f, 0);
        /// <summary>
        /// Size of the collider
        /// </summary>
        Vector3 size = new Vector3(2, 1, 2);

        public LayerMask layermask;

        /// <summary>
        /// Provides access to the values added from equipping items on the charater panel
        /// </summary>
        public Dictionary<string, float> BuffValues
        {
            get { return InventoryController.Instance.CharacterPanel.buffValues; }
        }

        public AudioClip pickupSound;

        public RaycastHit hit;

        Ray ray;

        public bool CanPlaceItem
        {
            get
            {
                Camera mainCamera = Camera.main;
                Vector3 cameraCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f); // Center of the screen

                ray = mainCamera.ScreenPointToRay(cameraCenter);

                // draw a debug ray
                Debug.DrawRay(ray.origin, ray.direction * 10, Color.red);

                bool b = Physics.Raycast(ray, out hit, Mathf.Infinity, layermask);

                // add a debug sphere at the hit point (if hit something)
                if (b)
                {
                    GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    go.transform.position = hit.point;
                    go.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                }

                return b;
            }
        }




        public bool CanPlaceItemX
        {
            get
            {

                ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                // draw a debug ray
                Debug.DrawRay(ray.origin, ray.direction * 10, Color.red);
                // add a debug sphere at the hit point
                GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);

                go.transform.position = hit.point;

                go.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);




                bool b = Physics.Raycast(ray, out hit, Mathf.Infinity, layermask);

                return b;

            }
        }

        void OnEnable()
        {
            BoxCollider bc = GetComponent<BoxCollider>();
            bc.isTrigger = true;
            bc.center = center;
            bc.size = size;
        }

        private void OnTriggerEnter(Collider other)
        {
            PickUpItem(other.gameObject);
        }

        /// <summary>
        /// method to allow player to pick up an Item and place it in the inventory
        /// This method will try and place the item in the  ItemBar if there is space
        /// if no space there it will try the players inventory. Either way will return true
        /// if both fail it will return false
        /// </summary>
        /// <param name="collectedObject">The GameObject that is to be picked up</param>
        /// <returns></returns>
        internal bool PickUpItem(GameObject collectedObject)
        {

            if (!collectedObject.CompareTag("Item"))
            {
                return false;
            }

            if (!collectedObject.TryGetComponent<DroppedItem>(out var ip))
            {
                return false;
            }

            if (ip.ItemID <= 0)
            {
                return false;
            }

            Item newItem = new Item(ip.ItemID, ip.stackCount);

            if (InventoryController.ItemBarInventory.AddItem(newItem) == false)
            {
                if (InventoryController.PlayerInventory.AddItem(newItem) == false)
                {
                    return false;
                }
            }

            if (InventoryController.DroppedItems.Contains(ip))
            {
                InventoryController.DroppedItems.Remove(ip);
            }

            GetComponent<AudioSource>().PlayOneShot(pickupSound);

            Destroy(collectedObject);

            return true;

        }

        /// <summary>
        /// method for player to drop an item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="quantity"></param>
        internal void DropItem(Item item, int quantity = 1)
        {
            GameObject prefab;

            if (quantity > 1)
            {
                prefab = item.data.worldPrefabMultiple; // get the multiple prefab
            }
            else
            {
                prefab = item.data.worldPrefabSingle; // get the default drop prefab for the selected item
            }

            if (prefab != null) //  make sure we have a prefabe
            {

                // instatiate teh prefabe and 'toss' it our infront of the player
                Vector3 dropPoint = InventoryController.Instance.Player.transform.position + InventoryController.Instance.Player.transform.forward + InventoryController.Instance.Player.transform.up; // TODO make this more acurate, and show setting on the editor
                GameObject g = Instantiate(prefab, dropPoint, Quaternion.identity);
                g.GetComponent<Rigidbody>().AddForceAtPosition(InventoryController.Instance.Player.transform.forward * 2 + InventoryController.Instance.Player.transform.up, g.transform.position, ForceMode.Impulse);
                if (g.TryGetComponent<DroppedItem>(out var ip))
                {
                    ip.ItemID = item.data.id;
                    ip.stackCount = quantity;
                    ip.TTL = InventoryController.Instance.DroppedItemTTL;
                    InventoryController.DroppedItems.Add(ip);
                }
                else
                {
                    Debug.LogWarning("ItemPickup component missing from dropped item prefab. Dropped item can not be picked up without it.");
                }
            }
            else
            {
                Debug.LogWarning("Missing drop prefab on item " + item.data.name);
            }
        }

    }
}
