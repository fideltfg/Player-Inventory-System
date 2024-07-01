using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;

namespace PlayerInventorySystem
{
    /// <summary>
    /// Component added to player object to allow items to be picked up.
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class PlayerInventoryController : MonoBehaviour
    {
        public Transform Head;
        public Transform Body;
        public Transform LeftHand;
        public Transform RightHand;

        [Range(0.1f, 10)]
        public float interactDistance = 8f;

        public Animator animator;

        /// <summary>
        /// Ceter offset for collider
        /// </summary>
        public Vector3 colliderCenter = new Vector3(0, .75f, 0);
        /// <summary>
        /// Size of the collider
        /// </summary>
        public Vector3 colliderSize = new Vector3(1, 1.5f, 1);

        public LayerMask layermask;

        /// <summary>
        /// Provides access to the values added from equipping items on the charater panel
        /// </summary>
        public Dictionary<string, float> BuffValues
        {
            get { return InventoryController.Instance.CharacterPanel.buffValues; }
        }

        internal Character Character
        {
            get { return InventoryController.Character; }
        }

        public AudioClip pickupSound;

        RaycastHit hit;

        /// <summary>
        /// If the player is looking at an interactable object or at the ground where an item can be placed.
        /// </summary>
        internal bool CanInteract
        {
            get
            {
                Vector3 cameraCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f); // Center of the screen

                Ray ray = Camera.main.ScreenPointToRay(cameraCenter);

                return Physics.Raycast(ray, out hit, interactDistance, layermask);
            }
            private set { }
        }

        void OnEnable()
        {
            BoxCollider bc = GetComponent<BoxCollider>();

            bc.isTrigger = true;

            bc.center = colliderCenter;

            bc.size = colliderSize;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (PickUpItem(other.gameObject))
            {
                GetComponent<AudioSource>().PlayOneShot(pickupSound);
            }
        }

        /// <summary>
        /// method to allow player to pick up an item and place it in the inventory
        /// This method will try and place the item in the  ItemBar if there is space
        /// if no space there it will try the players inventory. Either way will return true
        /// if both fail it will return false
        /// 
        /// do not use this method to give the player an item.
        /// 
        /// </summary>
        /// <param name="collectedObject">The GameObject that is to be picked up</param>
        /// <returns></returns>
        internal bool PickUpItem(GameObject collectedObject)
        {
            // Debug.Log("PickUpItem");
            if (!collectedObject.CompareTag("Item"))
            {
                //  Debug.Log("Not an item");
                return false;
            }

            if (!collectedObject.TryGetComponent(out DroppedItem droppedItem))
            {
                // Debug.Log("No DroppedItem component");
                return false;
            }

            if (droppedItem.ItemID <= 0)
            {
                // Debug.Log("ItemID is 0");
                return false;
            }

            Item newItem = Item.New(droppedItem.ItemID, droppedItem.StackCount);

            if (newItem.Data.ConsumeOnPickup)
            {
                ConsumeItem(newItem);
                Destroy(collectedObject);
                return true;
            }
            else
            {
                if (GiveItem(newItem))
                {
                    Destroy(collectedObject);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// method to add a single item directly in to the players itemBar or inventory
        /// </summary>
        /// <param name="itemID">The ID of the item to be added</param>
        /// <returns>Returns true on success else false</returns>
        internal bool GiveItem(Item item)
        {
            if (InventoryController.ItemBarInventory.AddItem(item) == false)
            {
                if (InventoryController.Instance.UsePlayerInventory == true)
                {
                    return InventoryController.PlayerInventory.AddItem(item);
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// method for player to drop an item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="quantity"></param>
        internal void DropItem(Item item, int quantity = 1, float durabiltiy = 100)
        {
            // get the items default ttl value
            float ttl = item.Data.prefabSingle.GetComponent<DroppedItem>().TimeToLive;
            // spawn the item in the world
            InventoryController.Instance.SpawnItem(item.Data.id, transform.position + (transform.forward * 1.5f) + (transform.up * 1.5f), quantity, ttl, durabiltiy);
        }

        /// <summary>
        /// Method for the player to interact with the world and items in it
        /// </summary>
        internal void Interact()
        {
            if (CanInteract == true && hit.transform != null)
            {
                Interactive interactiveObject = hit.transform.gameObject.GetComponent<Interactive>();
                if (interactiveObject != null)
                {
                    interactiveObject.Interact(this);
                }
                else
                {
                    // check if there are any windows open. need to do this so the player can not place items in the world
                    if (!InventoryController.Instance.AnyWindowOpen)
                    {
                        // place the item in the world
                        InventoryController.Instance.PlaceItem(hit.point, Quaternion.identity, Vector3.one);
                    }
                }
            }
            else
            {
                //Debug.Log("Can Not Interact Here!");
                return;
            }
        }

        private void Mine(Mineable mineable)
        {
            if (!mining)
            {
                StartCoroutine(Mining(mineable));
            }
            else
            {
                Debug.Log("Already Mining");
            }
        }

        internal bool mining = false;

        private IEnumerator Mining(Mineable mineable)
        {
            if (mineable == null || mining)
            {
                Debug.Log("Not Mineable");
                yield return null;
            }
            // set the mining flag to true
            mining = true;
            // get the tool item from the item bars selected slot
            Item toolItem = InventoryController.Instance.ItemBar.SelectedSlotController.Slot.Item;

            if (toolItem == null)
            {
                Debug.Log("No Tool Selected");
                yield return null;
            }

            // check if the tool item is a usable item. Some items can not be used to mine
            if (toolItem.Data.itemType == ITEMTYPE.USABLE)
            {
                animator.SetBool("Mine", true);

                while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.6)
                {
                    yield return null;
                }
                animator.SetBool("Mine", false);
                Item minedItem = mineable.Mine(toolItem);

                if (minedItem != null)
                {
                    if (mineable.mineToPlayerInventory)
                    {
                        // give the player the mined item or spawn it in the world if inventory is full
                        if (GiveItem(minedItem) == false)
                        {
                            InventoryController.Instance.SpawnItem(minedItem.Data.id, mineable.transform.position + (transform.up * 1.5f), 1);
                        }

                    }
                    else
                    {
                        // spawn the item in the world
                        InventoryController.Instance.SpawnItem(minedItem.Data.id, mineable.transform.position + (transform.up * 1.5f), 1);
                    }
                }

            }
            mining = false;
            yield return null;

        }

        internal void ConsumeItem(Item item)
        {
            // check that the item is a consumable item
            if (item.Data.itemType == ITEMTYPE.CONSUMABLE)
            {



            }
        }

        private void UseItem(Item itemBeingUsed)
        {
            if (itemBeingUsed == null || InventoryController.Instance.AnyWindowOpen)
            {
                return;
            }

            switch (itemBeingUsed.Data.itemType)
            {
                case ITEMTYPE.USABLE:


                    // test if there is an interactive item in front of the player
                    if (CanInteract == true && hit.transform != null)
                    {
                        Interactive interactiveObject = hit.transform.gameObject.GetComponent<Interactive>();

                        if (interactiveObject != null)
                        {

                            Type type = interactiveObject.GetType();

                            if (type == typeof(Mineable))
                            {
                                Mine(interactiveObject as Mineable);
                            }
                            else
                            {
                                float damage = (itemBeingUsed.Data.damage + Character.Damage) * 10;

                                // play the attack animation
                                animator.SetTrigger("Attack");
                                Debug.Log("Attacking");

                                // get the placable component from the hit object
                                PlacedItem placedItem = hit.transform.gameObject.GetComponent<PlacedItem>();
                                if (placedItem != null)
                                {
                                    placedItem.TakeDamage(damage);
                                }
                            }

                            if (itemBeingUsed.Durability <= 0)
                            {
                                itemBeingUsed.Durability = itemBeingUsed.Data.maxDurability;
                                itemBeingUsed.lastSlot.IncermentStackCount(-1);
                                if (itemBeingUsed.StackCount <= 0)
                                {
                                    itemBeingUsed.lastSlot.SetItem(null);
                                }
                                Debug.Log("Item is broken");
                                return;
                            }

                            itemBeingUsed.Durability--;
                            itemBeingUsed.lastSlot.SlotChanged(itemBeingUsed.lastSlot);
                        }
                    }

                    break;
                case ITEMTYPE.CONSUMABLE:
                    // consume the item
                    ConsumeItem(itemBeingUsed);
                    break;
            }
        }

        internal void UseCurrentItem()
        {
            // get the selected item from the item bar
            Item itemBeingUsed = InventoryController.Instance.ItemBar.SelectedSlotController.Slot.Item;
            UseItem(itemBeingUsed);
        }
    }
}