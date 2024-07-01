using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerInventorySystem
{
    public class Interactive : PlacedItem
    {

        // the panel the player will see when they interact with this object
        [HideInInspector]
        public InventorySystemPanel Panel;


        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                ClosePanel();
            }
        }

        private void OnEnable()
        {
            Collider sc = GetComponent<Collider>();
            sc.isTrigger = true;
        }

        internal virtual void ClosePanel()
        {
            if (Panel != null)
            {
                Panel.gameObject.SetActive(false);
            }
        }

        internal virtual void OpenPanel()
        {
            if (Panel != null)
            {
                Panel.gameObject.SetActive(true);
            }
        }

        public virtual void Interact(PlayerInventoryController playerInventoryController)
        {
           // throw new NotImplementedException();
        }
    }
}