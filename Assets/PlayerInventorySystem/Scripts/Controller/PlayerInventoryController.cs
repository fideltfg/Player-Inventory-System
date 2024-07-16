using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace PlayerInventorySystem
{
    /// <summary>
    /// Component added to player object to allow items to be picked up.
    /// </summary>
    public class PlayerInventoryController : MonoBehaviour
    {
        public Transform Head;
        public Transform Body;
        public Transform LeftHand;
        public Transform RightHand;

        [Range(0.1f, 10)]
        public float interactDistance = 8f;

        [Tooltip("The object that the player is currently able to interact with.")]
        public Interactive interactiveObject;

        public Animator animator;

        /// <summary>
        /// Callback for when the player attempts to mines an item
        /// </summary>
        internal Action<Mineable> OnMineCallback;

        /// <summary>
        /// Callback for when the player attempts to attack something
        /// </summary>
        internal Action<Item> OnUseItemCallback;

        /// <summary>
        /// Callback for when the player picks up an item
        /// </summary>
        internal Action<Item> OnPickupCallback;


        /// <summary>
        /// Callback for when the player drops an item
        /// </summary>
        internal Action<Item> OnDropItemCallback;

        /// <summary>
        /// Ceter offset for collider
        /// </summary>
        public Vector3 colliderCenter = new Vector3(0, .75f, 0);
        /// <summary>
        /// Size of the collider
        /// </summary>
        public Vector3 colliderSize = new Vector3(1, 1.5f, 1);

        public LayerMask InteractionLayerMask;

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
        /// Indicates if the player can interact at this time
        /// </summary>
        internal virtual bool CanInteract
        {
            get
            {
                Vector3 cameraCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f); // Center of the screen

                Ray ray = Camera.main.ScreenPointToRay(cameraCenter);

                return Physics.Raycast(ray, out hit, interactDistance, InteractionLayerMask);
            }
            private set { }
        }

        public virtual void OnEnable()
        {
            BoxCollider bc = GetComponent<BoxCollider>();

            bc.isTrigger = true;

            bc.center = colliderCenter;

            bc.size = colliderSize;
        }

        private void OnTriggerEnter(Collider other)
        {
            // if this is a dropped item
            if (PickUpItem(other.gameObject))
            {
                Debug.Log("Picked up item");
                return;
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

            // invoke on pickup callback
            OnPickupCallback?.Invoke(newItem);

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
        private bool GiveItem(Item item)
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
        internal virtual GameObject DropItem(Item item, int quantity = 1, float durabiltiy = 100)
        {
            if (item == null)
            {
                return null;
            }
            // get the items default ttl value
            float ttl = item.Data.prefabSingle.GetComponent<DroppedItem>().TimeToLive;
            // spawn the item in the world
            return InventoryController.Instance.SpawnItem(item.Data.id, transform.position + (transform.forward * 1.5f) + (transform.up * 1.5f), quantity, ttl, durabiltiy);
        }

        /// <summary>
        /// Method for the player to interact with the world and items in it
        /// </summary>
        internal virtual void Interact()
        {

            Debug.Log("Interact 2");
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

        /// <summary>
        /// method to mine a mineable object
        /// </summary>
        /// <param name="mineable"></param>
        internal void Mine(Mineable mineable)
        {
            if (!isMining)
            {
                Debug.Log("Mining");
                OnMineCallback?.Invoke(mineable);

                // comment out this line if you want to handel mining in a different way via the OnMineCallback in your own code
                StartCoroutine(Mining(mineable));
            }
            else
            {
                Debug.Log("Already Mining");
            }
        }

        private bool isMining = false;

        private IEnumerator Mining(Mineable mineable)
        {
            if (mineable == null || isMining)
            {
                Debug.Log("Not Mineable");
                yield return null;
            }
            // set the mining flag to true
            isMining = true;
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
                while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.6)
                {
                    yield return null;
                }
                // animator.SetBool("Mine", false);
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
            isMining = false;
            yield return null;

        }

        internal void ConsumeItem(Item item)
        {
            // check that the item is a consumable item
            if (item.Data.itemType == ITEMTYPE.CONSUMABLE)
            {

                // TODO!

            }
        }

        internal virtual void UseItem(Item itemBeingUsed)
        {
            //Debug.Log("Using Item");
            if (itemBeingUsed == null || InventoryController.Instance.AnyWindowOpen)
            {
                //  Debug.Log("No Item Selected");
                return;
            }

            switch (itemBeingUsed.Data.itemType)
            {
                case ITEMTYPE.USABLE:


                    // test if there is an interactive item in front of the player
                    if (CanInteract == true && hit.transform != null)
                    {
                        interactiveObject = hit.transform.gameObject.GetComponent<Interactive>();

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
                                // Debug.Log("Attacking");

                                // get the placable component from the hit object
                                if (interactiveObject.TryGetComponent<PlacedItem>(out PlacedItem placedItem))
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
                                // Debug.Log("Item is broken");
                                return;
                            }

                            itemBeingUsed.Durability--;
                            itemBeingUsed.lastSlot.SlotChanged(itemBeingUsed.lastSlot);
                        }
                    }
                    else
                    {
                        //  Debug.Log("No Interactive Object");
                    }

                    break;
                case ITEMTYPE.CONSUMABLE:
                    // consume the item
                    ConsumeItem(itemBeingUsed);
                    break;
            }
        }

        public void UseCurrentItem()
        {
            // get the selected item from the item bar
            Item itemBeingUsed = InventoryController.Instance.ItemBar.SelectedSlotController.Slot.Item;
            UseItem(itemBeingUsed);
        }

        public void RegisterOnMineCallback(Action<Mineable> callback)
        {
            OnMineCallback += callback;
        }

        public void UnRegisterOnMineCallback(Action<Mineable> callback)
        {
            OnMineCallback -= callback;
        }

        public void RegisterOnAttackCallback(Action<Item> callback)
        {
            OnUseItemCallback += callback;
        }

        public void UnRegisterOnUseItemCallback(Action<Item> callback)
        {
            OnUseItemCallback -= callback;
        }

        public void RegisterOnPickupCallback(Action<Item> callback)
        {
            OnPickupCallback += callback;
        }

        public void UnRegisterOnPickupCallback(Action<Item> callback)
        {
            OnPickupCallback -= callback;
        }

        public void RegisterOnDropItemCallback(Action<Item> callback)
        {
            OnDropItemCallback += callback;
        }

        public void UnRegisterOnDropItemCallback(Action<Item> callback)
        {
            OnDropItemCallback -= callback;
        }
    }
}