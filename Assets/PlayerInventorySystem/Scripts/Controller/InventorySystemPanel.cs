using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerInventorySystem
{

    /// <summary>
    /// Base class for all inventory panels
    /// </summary>
    public class InventorySystemPanel : MonoBehaviour
    {
        internal int selectedSlotID;// the slot that is currently selected in this panel

        /// <summary>
        /// ID of the slot the player has currently selected.
        /// </summary>
        public int SelectedSlotID
        {
            get
            {
                return selectedSlotID;
            }
            set
            {
                SlotList[selectedSlotID].Selected = false;
                selectedSlotID = (int)Mathf.Clamp(value, 0, 9);
                SlotList[selectedSlotID].Selected = true;
                InventoryController.Instance.OnSelectedItemChangeCallBack?.Invoke(SelectedSlotController.Slot.Item);
            }
        }

        /// <summary>
        /// The Slotcontroller of the currently selected slot.
        /// </summary>
        public SlotController SelectedSlotController
        {
            get { return SlotList[SelectedSlotID]; }
        }



        /// <summary>
        /// Holds the GridLayoutGroup component, read only 
        /// </summary>
        public GridLayoutGroup GridLayoutGroup { get { return GetComponent<GridLayoutGroup>(); } }

        /// <summary>
        /// The prefab that will be used to generate the slots for this panel
        /// </summary>
        public GameObject SlotPrefab;

        /// <summary>
        /// Provides a list of all SlotController components of the slots on this panel
        /// </summary>
        public List<SlotController> SlotList = new List<SlotController>();

        /// <summary>
        /// The index of the inventory this panel is displaying
        /// </summary>
        public virtual int Index
        {
            get { return index; }
            set
            {
                if (index != value)
                {
                    index = value;
                }
            }
        }
        private int index;

        public virtual void Update() { }

        public virtual void OnEnable()
        {
            InventoryController.Instance.OnWindowOpenCallback(this);
            transform.SetAsLastSibling();
        }

        public virtual void OnDisable()
        {
            // make sure all highlighting is turned off
            foreach (SlotController sc in SlotList)
            {
                sc.outline.enabled = false;
            }
        }

        /// <summary>
        /// Called to generate the panels slots and arrnge them on start.
        /// </summary>
        /// <param name="InventoryIndex"></param>
        public virtual void Build(int InventoryIndex = 0)
        {
            Index = InventoryIndex;
        }

    }

}