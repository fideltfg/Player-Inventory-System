using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace PlayerInventorySystem
{

    /// <summary>
    /// Controller for Chest panel.
    /// </summary>
    public class ChestPanel : InventorySystemPanel
    {

        ChestController chest;

        public ChestController Chest
        {
            get { return chest; }
            set
            {
                chest = value; // save the chest locally
                if (chest != null)
                {
                    Populate(); // populate the chest panel
                }
            }
        }

        public override void OnEnable ()
        {

            GridLayoutGroup.cellSize = SlotPrefab.GetComponent<RectTransform>().sizeDelta;
            GetComponent<ContentSizeFitter>().enabled = true;

            base.OnEnable();

        }

        public override void Build (int InventoryIndex = 0)
        {
            this.Index = InventoryIndex;


            // add 24 (the max slot count for chests ) slots to the panel
            // these will be reused  for all chests displayed
            for (int i = 0; i < 24; i++)
            {
                GameObject go = GameObject.Instantiate(SlotPrefab, Vector3.zero, Quaternion.identity, transform);
                SlotController sc = go.GetComponent<SlotController>();
                sc.Index = this.Index;
                sc.slotLocation = SLOTLOCATION.CHEST;
                SlotList.Add(sc);
                sc.gameObject.SetActive(false);
            }
        }

        private void Populate ()
        {
            this.Index = chest.ChestID;

            if (InventoryController.ChestInventories.ContainsKey(this.Index) == false)
            {
                InventoryController.ChestInventories[this.Index] = new Inventory(this.Index, chest.Capacity);
            }

            for (int i = 0; i < chest.Capacity; i++)
            {
                SlotList[i].Index = this.Index; // set the SlotControllers new index
                SlotList[i].SetSlot(InventoryController.GetChestInventory(Index)[i]); // get the slot from the chest inventory
                SlotList[i].gameObject.SetActive(true); // display the slot game object in the panel
            }
        }

        public override void OnDisable ()
        {
            // disable all slots in prep for next time the panel is displayed
            foreach (SlotController sc in SlotList)
            {
                sc.outline.enabled = false;
                sc.gameObject.SetActive(false);
            }
            OpenChest(false);

        }

        public void OpenChest (bool v)
        {
            if (chest != null)
            {
                chest.transform.Find("Lid").GetComponent<Animator>().SetBool("Open", v);
            }
        }
    }
}