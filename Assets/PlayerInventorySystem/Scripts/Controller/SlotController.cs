using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

namespace PlayerInventorySystem
{

    /// <summary>
    /// The Base slot controller contains all methods for dealing with slots and provides access to items within the slot.
    /// </summary>
    public class SlotController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        public int slotID;
        public float counter = 0;
        public Color unselectedColor;
        public bool mouseOver;
        public bool showInfoPanel;
        public bool interactable = true;
        public Transform infoPanel;

        /// <summary>
        /// The index of this slots inventory
        /// </summary>
        [HideInInspector]
        public int Index = 0;

        /// <summary>
        /// The UI image that this slotwill display an Item's sprite
        /// </summary>
        public Image image;

        /// <summary>
        /// The textobject that his controller will use to display the stack count.
        /// </summary>
        public Text text;

        /// <summary>
        /// The slot durability slider
        /// </summary>
        public Slider dSlider;

        /// <summary>
        /// The ouline component of the slot. Used to highlight the slot.
        /// </summary>
        public Outline outline;

        /// <summary>
        /// The location of this controllers slot.
        /// </summary>
        public SLOTLOCATION slotLocation = SLOTLOCATION.INVENTORY;

        /// <summary>
        /// The default color for highlighting this slot
        /// </summary>
        public Color HighlightColor;

        /// <summary>
        /// Highlight color when item placment if valid
        /// </summary>
        public Color ValidColor;

        /// <summary>
        /// The highlight color when item placement is invalid.
        /// </summary>
        public Color ErrorColor;

        /// <summary>
        /// The slot this controller controlls
        /// </summary>
        public Slot Slot
        {
            get
            {
                if (this.slotLocation == SLOTLOCATION.CHEST)
                {
                    Inventory Inventory = InventoryController.GetChestInventory(Index);
                    if (Inventory != null)
                    {
                        return InventoryController.GetChestInventory(Index)[slotID];
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    try
                    {
                        return GetInventorySlot(Index, slotID);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.Message + " " + e.StackTrace);
                        return null;

                    }
                }
            }
        }

        /// <summary>
        /// method to return a slot from an inventory
        /// </summary>
        /// <param name="index">the index of the inventory in the inventory list</param>
        /// <param name="slotID">the ID of the slot in the inventory</param>
        /// <returns></returns>
        private Slot GetInventorySlot(int index, int slotID)
        {

            if ((InventoryController.InventoryList[index].Count - 1) >= slotID)
            {
                try
                {
                    return InventoryController.GetInventory(index)[slotID];
                }
                catch (Exception e)
                {

                    throw e;
                }
            }
            else
            {
                throw new Exception("Inventory slot does not exist.");
            }

        }

        /// <summary>
        /// Indicates true if this slot is selected else false
        /// </summary>
        internal bool Selected
        {
            get { return Slot.selected; }
            set
            {
                if (interactable)
                {
                    Slot.selected = value;
                    GetComponent<Image>().color = value ? HighlightColor : unselectedColor;
                }
            }
        }

        /// <summary>
        /// Accessor for the inventory system's Held Item
        /// </summary>
        public Item HeldItem { get { return InventoryController.HeldItem; } set { InventoryController.HeldItem = value; } }

        /// <summary>
        /// The delay between mouse over and the Item Informaion Box being diaplayed.
        /// </summary>
        public float ItemInfoBoxDelay = .66f;

        internal Animation ImageAnimator;

        void OnEnable()
        {
            if (ImageAnimator == null)
            {
                ImageAnimator = transform.Find("Image").GetComponent<Animation>();
            }

            infoPanel = transform.Find("Info Panel");

        }

        void Awake()
        {
            unselectedColor = GetComponent<Image>().color;
        }

        public virtual void Update()
        {
            if (infoPanel != null)
            {
                infoPanel.gameObject.SetActive(showInfoPanel);
            }
            // what to do if the mouse pointer is over this slot
            if (mouseOver)
            {
                if (slotLocation != SLOTLOCATION.ITEMBAR)
                {
                    if (counter > ItemInfoBoxDelay)
                    {
                        InventoryController.Instance.ItemHolder.itemInfoBox.Show(this.Slot.Item);
                    }
                    else
                    {
                        counter += Time.deltaTime;
                    }
                }
            }
        }

        void OnDisable()
        {
            mouseOver = false;
        }

        /// <summary>
        /// Method to set the slot that this controller will control.
        /// </summary>
        /// <param name="slot"></param>
        internal void SetSlot(Slot slot)
        {
            this.slotID = slot.SlotID;
            slot.RegisterSlotChangedCallback(OnSlotChanged);
            UpdateSlotUI();
        }

        /// <summary>
        /// called whenever this slot or its item have changed in any way
        /// </summary>
        /// <param name="slot"></param>
        internal void OnSlotChanged(Slot slot)
        {
            UpdateSlotUI();
        }

        /// <summary>
        /// method called to update the slots UI
        /// </summary>
        internal virtual void UpdateSlotUI()
        {
            counter = 0;
            InventoryController.Instance.ItemHolder.itemInfoBox.Show(null);
            if (Slot != null && Slot.Item != null && Slot.Item.Data.name != "NULL")
            {
                image.sprite = Slot.Item.Data.sprite;
                image.enabled = true;

                if (Slot.Item.Data.maxStackSize > 1)
                {
                    // stackable items to NOT have durability so hide the dSlider

                    if (dSlider != null)
                    {
                        dSlider.gameObject.SetActive(false);
                    }
                    // enable the stack text
                    text.text = Slot.Item.StackCount.ToString();
                    if (text != null)
                    {
                        text.enabled = true;
                    }
                }
                else
                {
                    if (dSlider != null)
                    {
                        if (Slot.Item.Data.maxDurability > 0)
                        {
                            dSlider.maxValue = Slot.Item.Data.maxDurability;
                            dSlider.value = Slot.Item.Durability;
                            dSlider.gameObject.SetActive(true);
                        }
                        else
                        {
                            dSlider.gameObject.SetActive(false);
                        }
                    }
                    if (text != null)
                    {
                        text.enabled = false;
                    }
                }

                // play pop animation
                if (ImageAnimator != null)
                {
                    ImageAnimator.Play();
                }

            }
            else
            {
                if (dSlider != null)
                {
                    dSlider.gameObject.SetActive(false);
                }
                image.enabled = false;
                text.enabled = false;
                InventoryController.Instance.ItemHolder.itemInfoBox.gameObject.SetActive(false);
                outline.enabled = false;
            }
        }

        /// <summary>
        /// method to handel the moue pointer entering the slot
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (interactable)
            {
                SetOutLineColor();
            }

            if (Slot.Item != null)
            {
                mouseOver = true;
            }
            counter = 0;
        }

        /// <summary>
        /// method to handel the pointer exiting the slot
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (interactable)
            {
                outline.effectColor = HighlightColor;
                outline.enabled = false;
            }

            counter = 0;
            mouseOver = false;
        }

        /// <summary>
        /// Method to handle mouse pointer down on this slot
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (!interactable)
            {
                return;
            }

            counter = 0;
            mouseOver = false;
            // if NOT holding an item and there is an item in this slot
            if (HeldItem == null && Slot.Item != null)
            {
                // if the player is holding down shift place the item( or stack of items) in the slot
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    MoveStack(); // to the next available slot in the open window.
                }
                else // if no shift on left click
                {
                    if (eventData.button == PointerEventData.InputButton.Left)
                    {
                        //just pick up whats here
                        PickUpStack();
                    }
                    // if no shift on right click
                    else if (eventData.button == PointerEventData.InputButton.Right)
                    {
                        PickUpHalfStack();
                    }
                }
                // return;
            }

            // if we are holding an item (on the mouse)
            else if (HeldItem != null)
            {
                // and this slot is not empty
                if (Slot.Item != null)
                {
                    // and the held Item is the same as the item in this slot
                    if (HeldItem.Data.id == Slot.Item.Data.id)
                    {
                        // if this stack is full
                        if (Slot.Item.StackCount >= Slot.Item.Data.maxStackSize)
                        {
                            // swap Items
                            Item i = HeldItem;
                            HeldItem = Slot.Item;
                            Slot.SetItem(i);
                        }
                        else
                        {

                            if (eventData.button == PointerEventData.InputButton.Left)
                            {
                                while (HeldItem.StackCount > 0 && Slot.Item.StackCount < Slot.Item.Data.maxStackSize)
                                {
                                    HeldItem.AddToStack(-1);
                                    Slot.IncermentStackCount(1);
                                }

                            }
                            else if (eventData.button == PointerEventData.InputButton.Right)
                            {
                                HeldItem.AddToStack(-1);
                                Slot.IncermentStackCount(1);
                            }

                            if (HeldItem.StackCount <= 0)
                            {
                                HeldItem = null;
                            }
                        }
                    }
                    else
                    {
                        if (HeldItem.Data.slotType == Slot.SlotType || Slot.SlotType == SLOTTYPE.INVENTORY)
                        {
                            // swap Items
                            Item i = HeldItem;
                            HeldItem = Slot.Item;
                            Slot.SetItem(i);
                        }
                    }
                }
                else if (Slot.Item == null) //  this slot has no item (empty)
                {
                    if (Slot.SlotType == HeldItem.Data.slotType || Slot.SlotType == SLOTTYPE.INVENTORY)
                    {

                        if (eventData.button == PointerEventData.InputButton.Left)
                        {
                            // place the stack
                            Slot.SetItem(HeldItem);
                            HeldItem = null;
                        }
                        if (eventData.button == PointerEventData.InputButton.Right)
                        {
                            Debug.Log("Right Click");
                            // place one copy of the item in to this slot
                            Item CloneItem = HeldItem.Clone();
                            CloneItem.SetStackCount(1);
                            Slot.SetItem(CloneItem);

                            // FIX STACKING BUG
                            if (HeldItem.StackCount > 1)
                            {
                                HeldItem.AddToStack(-1);
                            }
                            else
                            {
                                HeldItem = null;
                            }
                        }
                    }
                }
            }
            SetOutLineColor();
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            if (interactable)
            {

            }
        }

        internal virtual void SetOutLineColor()
        {
            if (HeldItem != null)
            {
                if (HeldItem.Data.slotType == Slot.SlotType || Slot.SlotType == SLOTTYPE.INVENTORY)
                {
                    outline.effectColor = ValidColor;
                }
                else
                {
                    outline.effectColor = ErrorColor;
                }
            }
            else
            {
                outline.effectColor = HighlightColor;
            }
            outline.enabled = true;
        }

        public void SetOutLineColor(Color color)
        {
            outline.effectColor = color;
            outline.enabled = true;
        }


        protected bool MoveStack()
        {
            if (!InventoryController.Instance.AnyWindowOpen)
            {
                return false;
            }

            bool isSlotMoved = false;
            switch (this.slotLocation)
            {
                case SLOTLOCATION.INVENTORY:
                    isSlotMoved = TryMoveToCharacterPanel() || TryMoveToChestPanel() || TryMoveToItemBar();
                    break;

                case SLOTLOCATION.ITEMBAR:
                    isSlotMoved = TryMoveToCharacterPanel() || TryMoveToInventoryPanel() || TryMoveToChestPanel();
                    break;

                case SLOTLOCATION.CHARACTER:
                    isSlotMoved = TryMoveToInventoryPanel() || TryMoveToChestPanel() || TryMoveToItemBar();
                    break;

                case SLOTLOCATION.CHEST:
                    isSlotMoved = TryMoveToCharacterPanel() || TryMoveToInventoryPanel() || TryMoveToItemBar();
                    break;
            }

            return isSlotMoved;
        }

        private bool TryMoveToCharacterPanel()
        {
            if (InventoryController.Instance.CharacterPanel.gameObject.activeInHierarchy && MoveStackToCharacter())
            {
                return true;
            }
            return false;
        }

        private bool TryMoveToChestPanel()
        {
            if (InventoryController.Instance.ChestPanel.gameObject.activeInHierarchy && MoveStackToChest())
            {
                return true;
            }
            return false;
        }

        private bool TryMoveToInventoryPanel()
        {
            if ((InventoryController.Instance.InventoryPanel.gameObject.activeInHierarchy ||
                 InventoryController.Instance.AdvancedInventoryPanel.gameObject.activeInHierarchy) &&
                InventoryController.PlayerInventory.AddItem(this.Slot.Item))
            {
                Slot.SetItem(null);
                return true;
            }
            return false;
        }

        private bool TryMoveToItemBar()
        {
            if (InventoryController.ItemBarInventory.AddItem(this.Slot.Item))
            {
                Slot.SetItem(null);
                return true;
            }
            return false;
        }

        bool MoveStackToCharacter()
        {
            // is there a slot of the correct type for this item free on the character panel?
            if (this.Slot.Item.Data.itemType == ITEMTYPE.WEARABLE)
            {
                switch (this.Slot.Item.Data.slotType)
                {
                    case SLOTTYPE.BODY:
                        if (InventoryController.Instance.CharacterPanel.BodySlot.Slot.Item == null)
                        {
                            // move item
                            InventoryController.Instance.CharacterPanel.BodySlot.Slot.SetItem(Slot.Item);
                            Slot.SetItem(null);
                        }
                        else
                        {
                            // swap Items
                            Item i = InventoryController.Instance.CharacterPanel.BodySlot.Slot.Item;
                            InventoryController.Instance.CharacterPanel.BodySlot.Slot.SetItem(Slot.Item);
                            Slot.SetItem(i);
                        }
                        break;
                    case SLOTTYPE.FEET:
                        if (InventoryController.Instance.CharacterPanel.FeetSlot.Slot.Item == null)
                        {
                            // move item
                            InventoryController.Instance.CharacterPanel.FeetSlot.Slot.SetItem(Slot.Item);
                            Slot.SetItem(null);
                        }
                        else
                        {
                            // swap Items
                            Item i = InventoryController.Instance.CharacterPanel.FeetSlot.Slot.Item;
                            InventoryController.Instance.CharacterPanel.FeetSlot.Slot.SetItem(Slot.Item);
                            Slot.SetItem(i);
                        }
                        break;
                    case SLOTTYPE.HEAD:
                        if (InventoryController.Instance.CharacterPanel.HeadSlot.Slot.Item == null)
                        {
                            // move item
                            InventoryController.Instance.CharacterPanel.HeadSlot.Slot.SetItem(Slot.Item);
                            Slot.SetItem(null);
                        }
                        else
                        {
                            // swap Items
                            Item i = InventoryController.Instance.CharacterPanel.HeadSlot.Slot.Item;
                            InventoryController.Instance.CharacterPanel.HeadSlot.Slot.SetItem(Slot.Item);
                            Slot.SetItem(i);
                        }
                        break;
                    case SLOTTYPE.LEGS:
                        if (InventoryController.Instance.CharacterPanel.LegsSlot.Slot.Item == null)
                        {
                            // move item
                            InventoryController.Instance.CharacterPanel.LegsSlot.Slot.SetItem(Slot.Item);
                            Slot.SetItem(null);
                        }
                        else
                        {
                            // swap Items
                            Item i = InventoryController.Instance.CharacterPanel.LegsSlot.Slot.Item;
                            InventoryController.Instance.CharacterPanel.LegsSlot.Slot.SetItem(Slot.Item);
                            Slot.SetItem(i);
                        }
                        break;
                    case SLOTTYPE.LEFTHAND:
                        if (InventoryController.Instance.CharacterPanel.LeftHandSlot.Slot.Item == null)
                        {
                            // move item
                            InventoryController.Instance.CharacterPanel.LeftHandSlot.Slot.SetItem(Slot.Item);
                            Slot.SetItem(null);
                        }
                        else
                        {
                            // swap Items
                            Item i = InventoryController.Instance.CharacterPanel.LeftHandSlot.Slot.Item;
                            InventoryController.Instance.CharacterPanel.LeftHandSlot.Slot.SetItem(Slot.Item);
                            Slot.SetItem(i);
                        }
                        break;
                    case SLOTTYPE.RIGHTHAND:
                        if (InventoryController.Instance.CharacterPanel.RightHandSlot.Slot.Item == null)
                        {
                            // move item
                            InventoryController.Instance.CharacterPanel.RightHandSlot.Slot.SetItem(Slot.Item);
                            Slot.SetItem(null);
                        }
                        else
                        {
                            // swap Items
                            Item i = InventoryController.Instance.CharacterPanel.RightHandSlot.Slot.Item;
                            InventoryController.Instance.CharacterPanel.RightHandSlot.Slot.SetItem(Slot.Item);
                            Slot.SetItem(i);
                        }
                        break;
                    case SLOTTYPE.HANDS:
                        if (InventoryController.Instance.CharacterPanel.RightHandSlot.Slot.Item == null)
                        {
                            // move item
                            InventoryController.Instance.CharacterPanel.RightHandSlot.Slot.SetItem(Slot.Item);
                            Slot.SetItem(null);
                        }
                        else if (InventoryController.Instance.CharacterPanel.LeftHandSlot.Slot.Item == null)
                        {
                            // move item
                            InventoryController.Instance.CharacterPanel.LeftHandSlot.Slot.SetItem(Slot.Item);
                            Slot.SetItem(null);
                        }
                        else if (InventoryController.Instance.CharacterPanel.LeftHandSlot.Slot.Item != null && InventoryController.Instance.CharacterPanel.RightHandSlot.Slot.Item != null)
                        {
                            // swap Item with righ hand item
                            Item i = InventoryController.Instance.CharacterPanel.RightHandSlot.Slot.Item;
                            InventoryController.Instance.CharacterPanel.RightHandSlot.Slot.SetItem(Slot.Item);
                            Slot.SetItem(i);
                        }
                        break;
                    default:
                        return false;
                }
                return true;
            }
            return false;
        }

        bool MoveStackToChest()
        {
            ChestController Chest = InventoryController.Instance.ChestPanel.Chest;
            if (Chest != null)
            {
                if (Chest.Inventory.AddItem(this.Slot.Item))
                {
                    Slot.SetItem(null);
                    return true;
                }
            }
            return false;
        }

        protected Vector2Int SplitIntVectorInt(int number)
        {
            int firstNumber = number / 2;
            int secondNumber = number - firstNumber;

            if (firstNumber < secondNumber)
            {
                (secondNumber, firstNumber) = (firstNumber, secondNumber);

                /*int temp = firstNumber;
                firstNumber = secondNumber;
                secondNumber = temp;*/
            }

            return new Vector2Int(firstNumber, secondNumber);
        }
        
        protected virtual void PickUpHalfStack()
        {
            if (this.Slot.Item.StackCount == 1)
            {
                HeldItem = this.Slot.Item.Clone();
                Slot.SetItem(null);
            }
            else
            {
                HeldItem = this.Slot.Item.Clone();

                Vector2Int v = SplitIntVectorInt(Slot.Item.StackCount);
                Slot.SetStackCount(v.x);
                HeldItem.SetStackCount(v.y);

                // if the stack is now empty  clear the slot
                if (Slot.Item.StackCount <= 0)
                {
                    Slot.SetItem(null);
                }
            }
        }

        protected virtual void PickUpStack()
        {
            HeldItem = this.Slot.Item;
            Slot.SetItem(null);
        }


        /// <summary>
        /// swaps the Held item with the item in the given slot
        /// </summary>
        public void SwapItems()
        {
            Item i = HeldItem;
            HeldItem = this.Slot.Item;
            Slot.SetItem(i);
        }

        /// <summary>
        /// method to swap the item in this slot with the item in the item bar slot with the given ID
        /// </summary>
        /// <param name="ItemBarSlotID"></param>
        public virtual void SwapWithItemBarSlot(int ItemBarSlotID)
        {
            Item i = InventoryController.ItemBarInventory[ItemBarSlotID].Item;
            InventoryController.ItemBarInventory[ItemBarSlotID].SetItem(null);
            InventoryController.ItemBarInventory[ItemBarSlotID].SetItem(Slot.Item);
            InventoryController.OnSelectedItemChangeCallBack?.Invoke(InventoryController.Instance.ItemBar.SelectedSlotController.Slot.Item);
            Slot.SetItem(i);
        }

    }
}