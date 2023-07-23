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

        RaycastHit hit;

        /// <summary>
        /// method to test if the player is looking at an interactable object
        /// or at the ground where an item can be placed.
        /// </summary>
        public bool CanInteract
        {
            get
            {
                Vector3 cameraCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f); // Center of the screen

                Ray ray = Camera.main.ScreenPointToRay(cameraCenter);

                return Physics.Raycast(ray, out hit, Mathf.Infinity, layermask);
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
        /// method to add a single item directly in to the players itemBar or inventory
        /// </summary>
        /// <param name="itemID">The ID of the item to be added</param>
        /// <returns>Returns true on success else false</returns>
        public static bool GiveItem(int itemID, int stackCount = 1)
        {
            if (itemID <= 0)
            {
                return false;
            }

            Item newItem = new Item(itemID, stackCount);

            if (InventoryController.ItemBarInventory.AddItem(newItem) == false)
            {
                if (InventoryController.PlayerInventory.AddItem(newItem) == false)
                {
                    return false;
                }
            }
            return true;
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

            if (!collectedObject.TryGetComponent(out DroppedItem dorppedItem))
            {
                return false;
            }

            if (dorppedItem.ItemID <= 0)
            {
                return false;
            }

            Item newItem = new Item(dorppedItem.ItemID, dorppedItem.stackCount);

            if (InventoryController.ItemBarInventory.AddItem(newItem) == false)
            {
                if (InventoryController.PlayerInventory.AddItem(newItem) == false)
                {
                    return false;
                }
            }

            if (InventoryController.DroppedItems.Contains(dorppedItem))
            {
                InventoryController.DroppedItems.Remove(dorppedItem);
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
        public void DropItem(Item item, int quantity = 1)
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
                // instatiate the prefabe and 'toss' it our in front of the player
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

        /// <summary>
        /// method to place the currently selected item from the itembar in the world
        /// </summary>
        private void PlaceItem(Vector3 pos, Quaternion rot, Vector3 scale)
        {
            // if the player right clicks on the ground then place the selected item in the world
            if (InventoryController.Instance.ItemBar.SelectedSlotController.Slot.Item != null)
            {
                // Get the selected item from the item bar
                Item item = InventoryController.Instance.ItemBar.SelectedSlotController.Slot.Item;

                // If the item has a world prefab then place it in the world
                if (item.data.worldPrefab != null)
                {
                    Item.Spawn(item, pos, rot, scale);
                }
            }
        }

        /// <summary>
        /// Method for the player to interact with the world and items in it
        /// </summary>
        internal void Interact()
        {

            if (CanInteract == true && hit.transform != null)
            {
                Debug.Log("Interact");
                switch (hit.transform.tag.ToLower())
                {
                    case "chest":
                        Debug.Log("Open Chest");
                        InventoryController.Instance.OpenChest(hit.transform.gameObject.GetComponent<ChestController>());
                        break;
                    case "craftingtable":
                        break;
                    default:
                        Debug.Log("Place Item");
                        PlaceItem(hit.point, Quaternion.identity, Vector3.one);
                        break;

                }
            }
            else
            {
                Debug.Log("Can Not Interact Here!");
                return;
            }
        }
    }
}