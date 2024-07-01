using UnityEngine;
using UnityEngine.UI;

namespace PlayerInventorySystem
{
    /// <summary>
    /// Controller for the player inventory panel.
    /// </summary>
    public class InventoryPanel : InventorySystemPanel
    {

        public override void Update()
        {
            if (InventoryController.PlayerInventoryCapacity != InventoryController.PlayerInventory.Count)
            {
                foreach (SlotController sc in SlotList)
                {
                    sc.Slot.UnregisterSlotChangeCallback(sc.OnSlotChanged);
                    GameObject.Destroy(sc.gameObject);
                }
                InventoryController.InventoryList[InventoryController.PlayerInventory.Index] = InventoryController.ResizeInventory(InventoryController.PlayerInventory, InventoryController.PlayerInventoryCapacity);
                SlotList = new System.Collections.Generic.List<SlotController>();
                Build(Index);
            }
        }

        public override void Build(int InventoryIndex)
        {

            base.Build(InventoryIndex);

            GridLayoutGroup.cellSize = SlotPrefab.GetComponent<RectTransform>().sizeDelta;
            GridLayoutGroup.constraintCount = Mathf.FloorToInt(Mathf.Sqrt(InventoryController.PlayerInventoryCapacity));
            GetComponent<ContentSizeFitter>().enabled = true;
            // add the slot objects for the players inventory

            foreach (Slot slot in InventoryController.GetInventory(this.Index))
            {
                GameObject go = GameObject.Instantiate(SlotPrefab, Vector3.zero, Quaternion.identity, transform);
                SlotController sc = go.GetComponent<SlotController>();
                sc.Index = this.Index;
                sc.slotLocation = SLOTLOCATION.INVENTORY;
                sc.SetSlot(slot);
                SlotList.Add(sc);
            }

            //SlotList[0].Selected = true;

        }
    }
}