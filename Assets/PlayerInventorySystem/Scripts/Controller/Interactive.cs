using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerInventorySystem
{
    [RequireComponent(typeof(SphereCollider))]
    public class Interactive : PlacedItem
    {

        // the panel the player will see when they interact with this object
        public InventorySystemPanel Panel;

        internal int Radius = 2;

        public virtual void Update()
        {
            if (Vector3.Distance(transform.position, InventoryController.Instance.Player.transform.position) > Radius)
            {
                ClosePanel();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                //  Panel.gameObject.SetActive(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                ClosePanel();
            }
        }

        private void OnEnable()
        {
            SphereCollider sc = GetComponent<SphereCollider>();
            sc.isTrigger = true;
            sc.radius = Radius;
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
    }
}