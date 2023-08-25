using UnityEngine;
using System;
using System.Collections.Generic;
using PlayerInventorySystem.Serial;

///*********************************************************************************
/// Player Inventorty System
/// version: 0.0.6 alpha
///*********************************************************************************

namespace PlayerInventorySystem
{

    /// <summary>
    /// The Primary Component and controller of the player inventory system.
    /// </summary>
    public class InventoryController : MonoBehaviour
    {
        /// <summary>
        /// Static instance of the inventory controller.
        /// </summary>
        public static InventoryController Instance;

        /// <summary>
        /// The list of items used in game. List inclues all needed data to generate and control items.
        /// </summary>
        public SO_ItemList ItemCatalog;

        /// <summary>
        /// The default capacity of the players inventory.
        /// must  have a  multiple of four slots (4, 8, 12, 16, 20 24....)
        /// </summary>
        public static int PlayerInventoryCapacity = 24;

        /// <summary>
        /// Set true to load saved inventory data on start
        /// </summary>
        public bool LoadInventory = false;

        /// <summary>
        /// Set true to save data to Application.persistentDataPath + "/Data/data.dat"
        /// Only use this when you have configured your Unity Player settings for publication
        /// keep false for testing in the editor.
        /// </summary>
        public bool UsePersistentDataPath = false;

        /// <summary>
        /// Holder for all inventories in the system eccept for chests.
        /// </summary>
        internal static Dictionary<int, Inventory> InventoryList = new Dictionary<int, Inventory>();

        /// <summary>
        /// Holder for all chest inventories in the system.
        /// </summary>
        internal static Dictionary<int, Inventory> ChestInventories = new Dictionary<int, Inventory>();

        /// <summary>
        /// map of chest game objects to chest ID's
        /// </summary>
        internal static Dictionary<int, GameObject> ChestMap = new Dictionary<int, GameObject>();

        /// <summary>
        /// List of items dropped by the player or spawned from mob/destroyed object ect..
        /// that currently exist in the game world.
        /// Items remove themselves from this list when they despawn or are picked up.
        /// </summary>
        internal List<DroppedItem> DroppedItems = new List<DroppedItem>();

        /// <summary>
        /// List of items that the player has placed in the game world.
        /// </summary>
        internal static List<PlacedItem> PlacedItems = new List<PlacedItem>();

        /// <summary>
        /// Static accessor for the players inventory
        /// </summary>
        internal static Inventory PlayerInventory { get { return InventoryList[0]; } } // index 0

        /// <summary>
        /// Static accessor for the Item Bar inventory
        /// </summary>
        internal static Inventory ItemBarInventory { get { return InventoryList[1]; } } // index 1

        /// <summary>
        /// Static accessor for the crafting panel inventory
        /// </summary>
        internal static Inventory CraftingInventory { get { return InventoryList[2]; } } // index 2

        /// <summary>
        /// Static accessor for the character panel inventory
        /// </summary>
        internal static Inventory CharacterInventory { get { return InventoryList[3]; } } // index 3.... slot order// 0: Head//1: Left Hand// 2: Right Hand//3: Body// 4: Legs// 5: feet

        /// <summary>
        /// Static accessor for the crafting panel output slot
        /// </summary>
        internal static Inventory CraftingOutputInventory { get { return InventoryList[5]; } } // index 5

        internal static Inventory SalvageInputInventory { get { return InventoryList[6]; } } // index 6

        internal static Inventory SalvageOutputInventory { get { return InventoryList[7]; } } // index 7


        /// <summary>
        /// Accessor for the held item
        /// 
        /// Inventory index 4
        /// </summary>
        public static Item HeldItem
        {
            get { return InventoryList[4][0].Item; }
            set
            {
                if (value != null)
                {
                    Cursor.visible = false;
                    Instance.dropPanel.gameObject.SetActive(true);
                }
                else
                {
                    Cursor.visible = true;
                    Instance.dropPanel.gameObject.SetActive(false);
                }
                InventoryList[4][0].SetItem(value);

            }
        }

        /// <summary>
        /// The player game object that this inventory is connected to.
        /// </summary>
        public GameObject Player;

        /// <summary>
        /// this is the controller for the player to interface with the inventory system
        /// </summary>
        public PlayerInventoryController PlayerIC;

        #region UI Elements

        /// <summary>
        /// The controller for the inventory panel
        /// </summary>
        public InventoryPanel InventoryPanel;

        /// <summary>
        /// The controller for the item bar
        /// </summary>
        public ItemBar ItemBar;

        /// <summary>
        /// the controller for the drop panel
        /// </summary>
        public DropPanel dropPanel;

        /// <summary>
        /// the controller for the item holder
        /// </summary>
        public ItemHolder ItemHolder;

        /// <summary>
        /// The controller for the crafting panel
        /// </summary>
        public CraftingPanel CraftingPanel;

        public SalvagePanel SalvagePanel;

        /// <summary>
        /// The controller for the character panel
        /// </summary>
        public CharacterPanel CharacterPanel;

        /// <summary>
        /// The controller of the chest panel
        /// </summary>
        public ChestPanel ChestPanel;

        #endregion UI Elements
        #region callbacks

        /// <summary>
        /// Action called whenever an inventory System panel is opened
        /// Register for this callback to trigger actions outside the inventory system when a panel is opened.
        /// This callback passes the panel that was opened.
        /// </summary>
        internal Action<InventorySystemPanel> OnWindowOpenCallback;

        /// <summary>
        /// Action called whenever the player selects a new item on the item bar
        /// Register for this callback to trigger actions outside the inventory system when the player changes their selected item.
        /// This callback passes the item that was selected.
        /// </summary>
        internal Action<Item> OnSelectedItemChangeCallBack;

        /// <summary>
        /// Action called whenever the player drops an item into the game world
        /// This callback passes the item that was dropped.
        /// </summary>
        public Action<Item> OnItemDroppedCallBack;

        /// <summary>
        /// callback for when an item on the character panel is changed
        public Action OnCharacterItemChangeCallBack;

        #endregion callbacks

        /// <summary>
        /// Indicates if any of the inventory system Panels are currently being displayed.
        /// </summary>
        public bool AnyWindowOpen
        {
            get
            {
                return InventoryPanel.gameObject.activeSelf ||
                    CraftingPanel.gameObject.activeSelf ||
                    CharacterPanel.gameObject.activeSelf ||
                    ItemBar.gameObject.activeSelf ||
                    ChestPanel.gameObject.activeSelf;
            }
        }

        /// <summary>
        /// Default time to live of items dropped by the player into the game world in seconds
        /// </summary>
        public float DroppedItemTTL = 300;

        void OnEnable()
        {
            if (Player == null)
            {
                Player = GameObject.FindGameObjectWithTag("Player");
                if (Player == null)
                {
                    Debug.LogError("No Game Object Tagged as Player was found. Either drag your player object on to the Inventoy Controller component player value or Tag it as Player. ");
                    return;
                }
            }

            if (!Player.TryGetComponent(out PlayerIC))
            {
                PlayerIC = Player.AddComponent<PlayerInventoryController>();
            }

            if (PlayerIC == null)
            {
                Debug.LogError("No PlayerInventoryController component was found on the player object. Either drag your player object on to the Inventoy Controller component player value or add a PlayerInventoryController component to your player object. ");
                return;
            }
            Application.targetFrameRate = -1;
            Instance = this; // create controller instance

            CreatePlayerInventory();// create the players inventory FIRST!!!! (0)
            AddNewInventory(10); // add the item bar inventory (1)
            AddNewInventory(9); // add crafting table inventory(2)
            AddNewInventory(6); // add character panel inventory(3)
            AddNewInventory(1); // Inventory for the current held Item (4)
            AddNewInventory(1); // inventory for crafting output item (5)
            AddNewInventory(1); // inventory for salvage input item (6)
            AddNewInventory(9); // inventory for salvage output inventory (7)


            if (LoadInventory)
            {
                Load();
            }

            // set up and config the inventory panel
            InventoryPanel.gameObject.SetActive(false);
            InventoryPanel.Build(0);

            // setup and config the item bar
            ItemBar.Build(1);

            // set up and config crafting panel
            CraftingPanel.gameObject.SetActive(false);
            CraftingPanel.Build(2);

            // setup and config salvage panel
            SalvagePanel.gameObject.SetActive(false);
            SalvagePanel.Build(6);

            //setup and config Character panel
            CharacterPanel.gameObject.SetActive(false);
            CharacterPanel.Build(3);

            ChestPanel.gameObject.SetActive(false);
            ChestPanel.Build(); //chest panel get build when chest is opened!!!

            // register callbacks for when a window opens
            OnWindowOpenCallback += WindowOpenCallback;

        }

        void Update()
        {
            // close all windows
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (AnyWindowOpen)
                {
                    dropPanel.gameObject.SetActive(false);
                }

                InventoryPanel.gameObject.SetActive(false);
                CharacterPanel.gameObject.SetActive(false);
                CraftingPanel.gameObject.SetActive(false);
                ChestPanel.gameObject.SetActive(false);


            }


            // this bit test to see if the player is holding an item and if they are it will drop it
            // if there are no windows open
            if (AnyWindowOpen == false)
            {

                if (HeldItem != null)
                {
                    if (ItemBarInventory.AddItem(HeldItem) == false)
                    {
                        if (PlayerInventory.AddItem(HeldItem) == false)
                        {
                            PlayerIC.DropItem(HeldItem, HeldItem.StackCount);
                        }
                    }
                    HeldItem = null;
                }
            }

            //EnablePlayerMovent(true); // enable the player
            dropPanel.gameObject.SetActive(false); // disable the drop panel

        }

        /// <summary>
        /// Method used to create an empty player inventory
        /// </summary>
        private void CreatePlayerInventory()
        {
            if (InventoryList.ContainsKey(0) == false)
            {
                AddNewInventory(PlayerInventoryCapacity);
            }
            else
            {
                if (PlayerInventory.Count != PlayerInventoryCapacity)
                {
                    // change inventroy size
                    InventoryList[0] = ResizeInventory(PlayerInventory, PlayerInventoryCapacity);
                }
            }
        }

        // add register callbacks for oncharacter item change
        public static void RegisterOnCharacterItemChangeCallback(Action callback)
        {
            Instance.OnCharacterItemChangeCallBack += callback;
        }

        public static void UnregisterOnCharacterItemChangeCallback(Action callback)
        {
            if (Instance.OnCharacterItemChangeCallBack != null)
            {
                Instance.OnCharacterItemChangeCallBack -= callback;
            }
        }

        /// <summary>
        /// Method to register a callback for when the selected item changes
        /// </summary>
        /// <param name="callbacK"></param>
        public static void RegisterOnSlectedItemChangeCallback(Action<Item> callbacK)
        {
            Instance.OnSelectedItemChangeCallBack += callbacK;
        }

        /// <summary>
        /// Method to unregister a callback for when the selected item changes
        /// </summary>
        /// <param name="callbacK"></param>
        public static void UnregisterOnSelectedItemChangeCallback(Action<Item> callbacK)
        {
            if (Instance.OnSelectedItemChangeCallBack != null)
            {
                Instance.OnSelectedItemChangeCallBack -= callbacK;
            }
        }

        /// <summary>
        /// Method to resize a given inventory to the given size.
        /// If the new size is smaller then the origonal, the inventory will be repacked.
        /// meaning aitems will be moved to the begining of the new inventory.
        /// Items that do not fit in to the new inventory will be discarded.
        /// If the new inventory size is greater then the old repacking will not take place
        /// and items will remain in there origonal slots.
        /// </summary>
        /// <param name="oldInventory">Inventory to resize</param>
        /// <param name="size"> Size of new inventory</param>
        /// <returns></returns>
        internal static Inventory ResizeInventory(Inventory oldInventory, int size)
        {
            Inventory newInv = new Inventory(oldInventory.Index, size);
            if (oldInventory.Count > size)
            {
                foreach (Slot s in oldInventory)
                {
                    if (!newInv.AddItem(s.Item))
                    {
                        // drop it
                        Instance.PlayerIC.DropItem(s.Item, s.Item.StackCount);
                    }
                }
            }
            else
            {
                foreach (Slot s in oldInventory)
                {
                    newInv[s.SlotID] = s;
                }
            }
            return newInv;
        }

        /// <summary>
        /// method called whenever an inventory panel is opened.
        /// </summary>
        /// <param name="window"></param>
        private void WindowOpenCallback(InventorySystemPanel window)
        {
            //  EnablePlayerMovent(false);// disable player movement while windows are open

            dropPanel.gameObject.SetActive(true); // turn on the drop panel

            UnityEngine.Cursor.lockState = CursorLockMode.None; // unlock the mouse

            if (HeldItem == null)
            {
                UnityEngine.Cursor.visible = true; // show the mouse
            }
        }

        /// <summary>
        /// Method to spawn a dropped item in to the world.
        /// Use this when you want an item to be dropped by a NPC or spawned when the player destroys an object but not from the player.
        /// </summary>
        /// <param name="itemID">The id of the item to spawn</param>
        /// <param name="position">the location to spawn the item</param>
        /// <param name="quantity">the size of the stack to spawn</param>
        /// <param name="TTL">The time the item will reamin in the world</param>
        /// <returns>Returns true on success else false</returns>
        /// 
        internal bool SpawnDroppedItem(int itemID, Vector3 position, int stackCount, float ttl = 30)
        {

            if (itemID <= 0)
            {
                return false;
            }

            ItemData itemData = Instance.ItemCatalog.list[itemID];

            GameObject prefab = itemData.worldPrefabSingle;

            if (stackCount > 1)
            {
                prefab = itemData.worldPrefabMultiple;
            }

            if (prefab == null)
            {
                return false;
            }

            // Calculate a random direction for popping up
            Vector3 popDirection = new Vector3(UnityEngine.Random.Range(-1f, 1f), 1f, UnityEngine.Random.Range(-1f, 1f)).normalized;

            GameObject g = Instantiate(prefab, position, Quaternion.identity);

            if (g.TryGetComponent<DroppedItem>(out var di))
            {
                di.ItemID = itemData.id;
                di.StackCount = stackCount;
                di.TimeToLive = ttl;

                // Apply the pop direction to the spawned item's rigidbody
                if (di.TryGetComponent<Rigidbody>(out var rb))
                {
                    rb.AddForce(popDirection * 3, ForceMode.Impulse);
                }

                DroppedItems.Add(di);
            }
            else
            {
                Debug.LogWarning("ItemPickup component missing from spawned item prefab. Item cannot be picked up without it.");
            }

            return true;
        }

        public bool SpawnDroppedItem(SerialDroppedItem sdi)
        {
            return SpawnDroppedItem(sdi.ItemID, sdi.Transform.Position, sdi.StackCount, sdi.TimeToLive);
        }


        /*        public bool SpawnDroppedItem(int itemID, Vector3 position, int quantity = 1, float TTL = 30, float Durability = 0)
                {
                    if (itemID <= 0)
                    {
                        return false;
                    }

                    ItemData itemData = Instance.ItemCatalog.list[itemID];

                    GameObject prefab = itemData.worldPrefabSingle;

                    if (quantity > 1)
                    {
                        prefab = itemData.worldPrefabMultiple;
                    }

                    if (prefab == null)
                    {
                        return false;
                    }

                    GameObject g = Instantiate(prefab, position, Quaternion.identity);

                    if (g.TryGetComponent<DroppedItem>(out var di))
                    {
                        di.ItemID = itemData.id;

                        di.StackCount = quantity;

                        di.TimeToLive = TTL;

                        DroppedItems.Add(di);
                    }
                    else
                    {
                        Debug.LogWarning("ItemPickup component missing from spawned item prefab. Item can not be picked up without it.");
                    }

                    return true;
                }*/

        /// <summary>
        /// Method to spawn a chest that was previously saved.
        /// </summary>
        /// <param name="chestID"></param>
        /// <param name="itemCatalogID"></param>
        /// <param name="inventory"></param>
        /// <param name="sTransform"></param>
        internal static ChestController SpawnChest(int chestID, int itemCatalogID, Vector3 position, Quaternion rotation, Vector3 scale, Inventory inventory = null)
        {
            //  Debug.Log("Spawning chest");

            GameObject go = Instantiate(Instance.ItemCatalog.list[itemCatalogID].worldPrefab, position, rotation);
            go.transform.localScale = scale;

            ChestController cc = go.AddComponent<ChestController>();
            cc.ChestID = chestID;
            cc.ItemID = itemCatalogID;
            //   cc.Panel = Instance.ChestPanel;
            if (inventory != null)
            {
                cc.Inventory = inventory;
            }
            else
            {
                cc.Inventory = new Inventory(0, 24);
            }

            MapChest(cc);
            return cc;

        }

        /// <summary>
        /// method to spawn a crafting table
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="scale"></param>
        internal static CraftingTableController SpawnCraftingTable(int itemID, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            GameObject go = Instantiate(Instance.ItemCatalog.list[itemID].worldPrefab, position, rotation);
            go.transform.localScale = scale;
            CraftingTableController cTc = go.AddComponent<CraftingTableController>();
            cTc.ItemID = itemID;
            //   cTc.Panel = Instance.CraftingPanel;
            return cTc;
        }

        /// <summary>
        /// method to store a chest for saving
        /// </summary>
        /// <param name="cc"></param>
        internal static void MapChest(ChestController cc)
        {
            if (ChestInventories.ContainsKey(cc.ChestID) == false)
            {
                ChestInventories.Add(cc.ChestID, cc.Inventory);
            }

            if (ChestMap.ContainsKey(cc.ChestID) == false)
            {
                ChestMap.Add(cc.ChestID, cc.gameObject);
            }

        }

        /// <summary>
        /// Method called when player places an item to register item to be saved. 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="placedItem"></param>
        internal static void OnPlaceItem(Item item, PlacedItem placedItem = null, bool consume = true)
        {
            // Debug.Log("OnPlaceItem");
            if (consume)
            {
                //   Debug.Log("Consume");
                // remove the item from the players inventory
                Instance.ItemBar.SelectedSlotController.Slot.IncermentStackCount(-1);

                // if the item stack count is 0 then remove the item from the slot
                if (Instance.ItemBar.SelectedSlotController.Slot.Item.StackCount <= 0)
                {
                    // remove the item from the slot
                    Instance.ItemBar.SelectedSlotController.Slot.SetItem(null);
                }
            }

            // if the placed item is not null then add it to the placed items list
            if (placedItem != null)
            {
                placedItem.ItemID = item.Data.id;

                // if the plased item is craftingtable then set its pan


                PlacedItems.Add(placedItem);
            }
        }

        /// <summary>
        /// Method to add an item (or stack of) directly in to the players itemBar or inventory
        /// 
        /// Use this method when you want to add an item to the players inventory but not from the world.
        /// 
        /// </summary>
        /// <param name="itemID">The ID of the item to be added</param>
        /// <returns>Returns true on success else false</returns>
        public static bool GiveItem(int itemID, int stackCount = 1)
        {
            if (itemID <= 0)
            {
                return false;
            }

            Item newItem = new Item(itemID, stackCount);

            if (ItemBarInventory.AddItem(newItem) == false)
            {
                if (PlayerInventory.AddItem(newItem) == false)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// method to toggle the inventory panel
        /// </summary>
        public void ToggleInventoryPanel()
        {
            Debug.Log("Toggling Inventory Panel");
            InventoryPanel.gameObject.SetActive(!InventoryPanel.gameObject.activeInHierarchy);
        }

        /// <summary>
        /// method to toggle the character panel
        /// </summary> 
        public void ToggleCharacterPanel()
        {
            CharacterPanel.gameObject.SetActive(!CharacterPanel.gameObject.activeInHierarchy);
        }

        /// <summary>
        /// method to toggle the chest panel
        /// </summary>
        public void ToggleChestPanel()
        {
            ChestPanel.gameObject.SetActive(!ChestPanel.gameObject.activeInHierarchy);
        }

        /// <summary>
        /// method to toggle the crafting panel
        /// </summary>
        public void ToggleCraftingPanel()
        {
            CraftingPanel.gameObject.SetActive(!CraftingPanel.gameObject.activeInHierarchy);
        }

        /// <summary>
        /// method to toggle the item bar
        /// </summary>
        public void ToggleItemBar()
        {
            ItemBar.gameObject.SetActive(!CraftingPanel.gameObject.activeInHierarchy);
        }

        /// <summary>
        /// method to add an inventory to the controller
        /// </summary>
        /// <param name="size">The capacity of the new inventory</param>
        /// <returns>int, the ID for the new inventory</returns>
        private int AddNewInventory(int size)
        {
            int index = InventoryList.Count;
            InventoryList.Add(index, new Inventory(index, size));
            return index;
        }

        /// <summary>
        /// method to return the inventory at the given index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        internal static Inventory GetInventory(int index)
        {
            if (InventoryList.ContainsKey(index))
            {
                return InventoryList[index];
            }
            else
            {
                throw new Exception("Inventory ID out of range.");
            }
        }

        /// <summary>
        /// method to return the inventory of the chest with the given id
        /// </summary>
        /// <param name="chestID"></param>
        /// <returns></returns>
        internal static Inventory GetChestInventory(int chestID)
        {
            if (ChestInventories.ContainsKey(chestID))
            {
                return ChestInventories[chestID];
            }
            return null;
        }

        /// <summary>
        /// method to display the chest Panel and the selected chest inventory
        /// </summary>
        internal void OpenChest(ChestController chestController)
        {
            if (chestController != null)
            {
                ChestPanel.Chest = chestController; // pass the selected chest to the chest panel
                ChestPanel.OpenCloseChestLid(true);
                ChestPanel.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogError("ChestController is null");
            }
        }

        /// <summary>
        /// method to toggle the crafting panel
        /// </summary>
        public void OpenCraftingTable(CraftingTableController cTc)
        {
            if (cTc != null)
            {
                CraftingPanel.CraftingTable = cTc; // pass the selected chest to the chest panel
                CraftingPanel.gameObject.SetActive(true);


            }
            else
            {
                Debug.LogError("CraftingTableController is null");
            }
        }

        /// <summary>
        /// Method to place the currently selected item from the itembar in the world
        /// </summary>
        internal void PlaceItem(Vector3 pos, Quaternion rot, Vector3 scale)
        {
            // Check if the selected slot has an item
            if (ItemBar.SelectedSlotController.Slot.Item != null)
            {
                // Get the selected item from the item bar
                Item item = ItemBar.SelectedSlotController.Slot.Item;
                if (item.StackCount >= 1)
                {
                    // If the item has a world prefab then place it in the world
                    if (item.Data.worldPrefab != null)
                    {
                        Item.Place(item, pos, rot, scale);
                    }
                }
            }
        }

        /// <summary>
        /// Method to generate a new id for chests.
        /// </summary>
        /// <returns></returns>
        internal static int GetNewChestID()
        {
            int newID = 0;
            foreach (int k in ChestInventories.Keys)
            {
                if (k >= newID)
                {
                    newID = k + 1;
                }
            }
            return newID;
        }

        /// <summary>
        /// Method to save the inventory data.
        /// </summary>
        public static void Save()
        {
            Serial.Serializer.Save();
        }

        /// <summary>
        /// Method to load the current saved data
        /// </summary>
        public static void Load()
        {
            Serial.Serializer.Load();
        }

        private void OnApplicationQuit()
        {
            Save();
        }


    }
}