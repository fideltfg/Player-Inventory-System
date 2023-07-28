using UnityEngine;
using UnityEngine.UI;


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
                if (chest != value)
                {
                    chest = value; // save the chest locally
                    if (chest != null)
                    {
                        Populate(chest); // populate the chest panel
                    }
                    else
                    {
                        Debug.LogError("Chest is null");
                    }
                }
            }
        }

        public override void OnEnable()
        {

            GridLayoutGroup.cellSize = SlotPrefab.GetComponent<RectTransform>().sizeDelta;
            GetComponent<ContentSizeFitter>().enabled = true;

            base.OnEnable();

        }

        public override void Build(int InventoryIndex = 0)
        {
            Index = InventoryIndex;


            // add 24 (the max slot count for chests ) slots to the panel
            // these will be reused  for all chests displayed
            for (int i = 0; i < 24; i++)
            {
                GameObject go = GameObject.Instantiate(SlotPrefab, Vector3.zero, Quaternion.identity, transform);
                SlotController sc = go.GetComponent<SlotController>();
                sc.Index = Index;
                sc.slotLocation = SLOTLOCATION.CHEST;
                SlotList.Add(sc);
                sc.gameObject.SetActive(false);
            }
        }

        private void Populate(ChestController chest)
        {
            Index = chest.ChestID;

            Inventory inv = chest.Inventory;

            for (int i = 0; i < chest.Capacity; i++)
            {
                SlotList[i].Index = Index; // set the SlotControllers new index
                SlotList[i].SetSlot(inv[i]); // set the slot from the chest inventory
                SlotList[i].gameObject.SetActive(true); // display the slot game object in the panel
            }
        }

        public override void OnDisable()
        {
            // base.OnDisable();

            // disable all slots in prep for next time the panel is displayed
            foreach (SlotController sc in SlotList)
            {
                sc.outline.enabled = false;
                sc.gameObject.SetActive(false);
            }
            // close the chest lid
            OpenCloseChestLid(false);

            // discard the chest
            chest = null;

        }

        /// <summary>
        /// method to open or close the chest lid
        /// </summary>
        /// <param name="v"></param>
        public void OpenCloseChestLid(bool v)
        {
            if (chest != null)
            {
                chest.transform.Find("Lid").GetComponent<Animator>().SetBool("Open", v);
            }
            else
            {
                Debug.LogError("Chest is null");
            }
        }
    }
}