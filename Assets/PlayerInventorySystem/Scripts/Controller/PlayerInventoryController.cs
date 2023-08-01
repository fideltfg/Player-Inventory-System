using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using static UnityEditor.Progress;

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
        /// If the player is looking at an interactable object or at the ground where an item can be placed.
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
        /// method to allow player to pick up an item and place it in the inventory
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

            if (!collectedObject.TryGetComponent(out DroppedItem droppedItem))
            {
                return false;
            }

            if (droppedItem.ItemID <= 0)
            {
                return false;
            }


            Item newItem = new Item(droppedItem.ItemID, droppedItem.StackCount);

            if (newItem.Data.consumeOnPickup)
            {
                ConsumeItem(newItem);
                // TODO change this to play conumptioon sound
                GetComponent<AudioSource>().PlayOneShot(pickupSound);
            }
            else
            {
                GiveItem(newItem);
                GetComponent<AudioSource>().PlayOneShot(pickupSound);
            }
            Destroy(collectedObject);

            return true;

        }

        /// <summary>
        /// method to add a single item directly in to the players itemBar or inventory
        /// </summary>
        /// <param name="itemID">The ID of the item to be added</param>
        /// <returns>Returns true on success else false</returns>
        public static bool GiveItem(Item item)
        {
            if (InventoryController.ItemBarInventory.AddItem(item) == false)
            {
                return InventoryController.PlayerInventory.AddItem(item);

                // TODO: If both inventories are full then drop the item on the ground


            }
            return true;
        }

        /// <summary>
        /// method for player to drop an item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="quantity"></param>
        internal void DropItem(Item item, int quantity = 1)
        {
            InventoryController.Instance.SpawnDroppedItem(item.Data.id, transform.position + new Vector3(0, 2, 2), quantity);
        }

        /// <summary>
        /// Method for the player to interact with the world and items in it
        /// </summary>
        internal void Interact()
        {
            if (CanInteract == true && hit.transform != null)
            {
                switch (hit.transform.tag.ToLower())
                {
                    case "chest":
                        ChestController cc = hit.transform.GetComponent<ChestController>();
                        InventoryController.Instance.OpenChest(cc);
                        break;
                    case "craftingtable":
                        Debug.Log("Crafting Table");
                        CraftingTableController cTc = hit.transform.GetComponent<CraftingTableController>();
                        cTc.Panel = InventoryController.Instance.CraftingPanel;
                        InventoryController.Instance.OpenCraftingTable(cTc);
                        break;
                    default:
                        // Debug.Log("Place Item");
                        InventoryController.Instance.PlaceItem(hit.point, Quaternion.identity, Vector3.one);
                        break;
                }
            }
            else
            {
                Debug.Log("Can Not Interact Here!");
                return;
            }
        }

        private void ConsumeItem(Item item)
        {
            if(item.Data.itemType == ITEMTYPE.CONSUMABLE)
            {
                if(item.Data != null)
                {
                }
            }
        }
    }
}