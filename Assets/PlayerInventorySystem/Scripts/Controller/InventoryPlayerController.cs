using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

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

        Camera cam;


        void OnEnable()
        {
            cam = GetComponentInChildren<Camera>();
            BoxCollider bc = GetComponent<BoxCollider>();
            bc.isTrigger = true;
            bc.center = center;
            bc.size = size;
        }

        private void OnTriggerEnter(Collider other)
        {
            InventoryController.PickUpItem(other.gameObject);
        }




        /// <summary>
        /// method to place an item in to your game world.
        /// </summary>
        /// <param name="item">The item that is to be placed.</param>
        /// <param name="hit">The hit point representing where the player clicked int he game world</param>
        /// <returns></returns>
        public bool PlaceItemInWorld(Item item, RaycastHit hit)
        {
            /*            GameObject go = GameObject.Instantiate(item.data.worldPrefab, hit.point, Quaternion.identity);
                        InventoryController.ItemPlaced(item, go);*/
            return true;
        }


        public LayerMask layermask;
        /// <summary>
        /// method to return the position to place an item relative to the player
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        internal RaycastHit GetPlacePos()
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(0.5f, 0.5f, 2f));

            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.up * 20f, Color.red);
            RaycastHit[] hits = Physics.RaycastAll(cam.transform.position, cam.transform.forward, 10f, layermask);
            if (hits.Length > 0)
            {
                foreach (RaycastHit hit in hits)
                { 
                    
                  /*  if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Terrain"))
                    {*/
                        Debug.Log(hit.transform.gameObject.name);
                        return hit;
                   // }
                }
            }
            // return an empty hit
            return new RaycastHit();
        }
    }
}
