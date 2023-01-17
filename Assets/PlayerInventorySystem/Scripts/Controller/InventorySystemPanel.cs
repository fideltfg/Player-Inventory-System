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
        /// The index of the invnetory for this panel
        /// </summary>
        public int Index;

        public virtual void Update () { }

        public virtual void OnEnable ()
        {
            InventoryController.Instance.OnWindowOpenCallback(this);
            transform.SetAsLastSibling();
        }

        public virtual void OnDisable ()
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
        public virtual void Build (int InventoryIndex = 0)
        {
            this.Index = InventoryIndex;
        }

    }

}