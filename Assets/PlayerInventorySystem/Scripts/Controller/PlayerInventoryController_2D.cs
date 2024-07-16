using System;
using UnityEngine;

namespace PlayerInventorySystem
{
    /// <summary>
    /// 2D component to allow the player to interact with items
    /// </summary>
    public class PlayerInventoryController_2D : PlayerInventoryController
    {
        private bool _canInteract = false;

        internal override bool CanInteract
        {
            get
            {
                return _canInteract;
            }
        }

        public override void OnEnable()
        {
            // we override the base method with an empty one as we are not using the default
            // collider here
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // if this is a dropped item
            if (PickUpItem(other.gameObject))
            {
               // Debug.Log("Picked up item");
                return;
            }

            if (other.GetComponent<Interactive>() != null)
            {
              //  Debug.Log("Can interact");
                _canInteract = true;
                interactiveObject = other.GetComponent<Interactive>();
              //  Debug.Log(interactiveObject);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            Interactive i = other.GetComponent<Interactive>();
            if (i != null)
            {
                _canInteract = true;
                interactiveObject = i;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (interactiveObject == collision.GetComponent<Interactive>())
            {
                interactiveObject = null;
                _canInteract = false;
            }
        }

        internal override void Interact()
        {
            if (_canInteract && interactiveObject != null)
            {
                interactiveObject.Interact(this);
            }
            else
            {
                int l = PlayerController_2D_Demo.lastLookedLeft ? -1 : 1;
                Vector3 pos = transform.position;
                pos.x += l;
                pos.z = 1;
                InventoryController.Instance.PlaceItem(pos, Quaternion.identity, Vector3.one);
            }
        }

        internal override GameObject DropItem(Item item, int quantity = 1, float durabiltiy = 100)
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

        internal override void UseItem(Item itemBeingUsed)
        {

            if (itemBeingUsed == null || InventoryController.Instance.AnyWindowOpen)
            {
               // Debug.Log("No Item Selected");
                return;
            }

            switch (itemBeingUsed.Data.itemType)
            {
                case ITEMTYPE.USABLE:


                    // test if there is an interactive item in front of the player
                    if (CanInteract == true && interactiveObject != null)
                    {
                        Type type = interactiveObject.GetType();

                        if (type == typeof(Mineable))
                        {
                          //  Debug.Log("Type Minable");
                            Mine(interactiveObject as Mineable);
                        }
                        else
                        {

                            OnUseItemCallback?.Invoke(itemBeingUsed);


                            float damage = (itemBeingUsed.Data.damage + Character.Damage) * 10;

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
                            return;
                        }

                        itemBeingUsed.Durability--;
                        itemBeingUsed.lastSlot.SlotChanged(itemBeingUsed.lastSlot);




                    }
                    else
                    {
                        //Debug.Log("No Interactive Object");
                    }

                    break;
                case ITEMTYPE.CONSUMABLE:
                    // consume the item
                    ConsumeItem(itemBeingUsed);
                    break;
            }
        }

    }
}