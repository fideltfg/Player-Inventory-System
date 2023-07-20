using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using PlayerInventorySystem.Serial;
using Unity.VisualScripting;

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
        /// Static Accessor for the item catalog list.
        /// </summary>
        public static List<ItemData> Catalog { get { return Instance.ItemCatalog.list; } }

        /// <summary>
        /// Holder for all inventories in the system.
        /// Does not include chest inventories.
        /// </summary>
        public static Dictionary<int, Inventory> InventoryList = new Dictionary<int, Inventory>();

        /// <summary>
        /// Holder for all chest inventories in the system.
        /// </summary>
        public static Dictionary<int, Inventory> ChestInventories = new Dictionary<int, Inventory>();

        /// <summary>
        /// map of chest game objects to chest ID's
        /// </summary>
        public static Dictionary<int, GameObject> ChestMap = new Dictionary<int, GameObject>();

        /// <summary>
        /// List of items dropped by the player or spawned from mob/destroyed object ect..
        /// that currently exist in the game world.
        /// Items remove themselves from this list when they despawn or are picked up.
        /// </summary>
        public static List<DroppedItem> DroppedItems = new List<DroppedItem>();

        /// <summary>
        /// List of items that the player has placed in the game world.
        /// </summary>
        public static List<PlacedItem> PlacedItems = new List<PlacedItem>();

        /// <summary>
        /// The default capacity of the players inventory.
        /// must  have a  multiple of four slots (4, 8, 12, 16, 20 24....)
        /// </summary>
        public int PlayerInventoryCapacity = 24;

        /// <summary>
        /// Set true to load saved inventory data on start
        /// </summary>
        public bool LoadInventory = false;

        /// <summary>
        /// Method to generate a new id for chests
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
        /// Set true to save data to Application.persistentDataPath + "/Data/data.dat"
        /// Only use this when you have configured your Unity Player settings  for publication
        /// </summary>
        public bool UsePersistentDataPath = false;

        /// <summary>
        /// Static accessor for the players inventory
        /// </summary>
        public static Inventory PlayerInventory { get { return InventoryList[0]; } } // index 0

        /// <summary>
        /// Static accessor for the Item Bar inventory
        /// </summary>
        public static Inventory ItemBarInventory { get { return InventoryList[1]; } } // index 1

        /// <summary>
        /// Static accessor for the crafting panel inventory
        /// </summary>
        public static Inventory CraftingInventory { get { return InventoryList[2]; } } // index 2

        /// <summary>
        /// Static accessor for the character panel inventory
        /// </summary>
        public static Inventory CharacterInventory { get { return InventoryList[3]; } } // index 3.... slot order// 0: Head//1: Left Hand// 2: Right Hand//3: Body// 4: Legs// 5: feet

        /// <summary>
        /// Static accessor for the crafting panel output slot
        /// </summary>
        public static Inventory CraftingOutputInventory { get { return InventoryList[5]; } } // index 5

        /// <summary>
        /// Accessor for the held item
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
        public LayerMask dropPanelLayerask;

        /// <summary>
        /// the controller for the item holder
        /// </summary>
        public ItemHolder ItemHolder;

        /// <summary>
        /// The controller for the crafting panel
        /// </summary>
        public CraftingPanel CraftingPanel;

        /// <summary>
        /// The controller for the character panel
        /// </summary>
        public CharacterPanel CharacterPanel;

        /// <summary>
        /// The controller of the chest panel
        /// </summary>
        public ChestPanel ChestPanel;

        /// <summary>
        /// Action called whenever an inventory System panel is opened
        /// </summary>
        public Action<InventorySystemPanel> OnWindowOpenCallback;

        /// <summary>
        /// action called Whenever the player slected a new item on the item bar
        /// </summary>
        public Action<Item> OnSelectedItemChangeCallBack;

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

/*        bool throwAway;

        /// <summary>
        /// when called the currently selectedc slots' item will be thrown away
        /// if the player is holding an item it will be thrown away
        /// </summary>
        internal void DroppedItem()
        {
            throwAway = true;
        }*/

        /// <summary>
        /// Default time to live of items dropped by the player into the game world in seconds
        /// </summary>
        public float DroppedItemTTL = 30;

        void Start()
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

            if (!Player.TryGetComponent<InventoryPlayerController>(out var inventoryPlayerController))
            {
                Player.AddComponent<InventoryPlayerController>();
            }

            Application.targetFrameRate = -1;
            Instance = this; // create controller instance

            CreatePlayerInventory();// create the players inventory FIRST!!!! (0)
            AddNewInventory(10); // add the item bar inventory (1)
            AddNewInventory(9); // add crafting table inventory(2)
            AddNewInventory(6); // add character panel inventory(3)
            AddNewInventory(1); // Inventory for the current held Item (4)
            AddNewInventory(1); // inventory for crafting output item (5)

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
                    EnablePlayerMovent(true);
                    dropPanel.gameObject.SetActive(false);
                }

                InventoryPanel.gameObject.SetActive(false);
                CharacterPanel.gameObject.SetActive(false);
                CraftingPanel.gameObject.SetActive(false);
                ChestPanel.gameObject.SetActive(false);


            }

            // if there are no windows open
            if (AnyWindowOpen == false)
            {
                // if the player hits the throwAway button (q) while no windows are open then throw away one of the selected Item
                //if (Input.GetKeyDown(throwawayKey))
                /*  if (throwAway)
                  {
                      ItemBar.DropSelectedItem();
                      throwAway = false;
                  }*/

                if (HeldItem != null)
                {
                    if (ItemBarInventory.AddItem(HeldItem) == false)
                    {
                        if (PlayerInventory.AddItem(HeldItem) == false)
                        {
                            DropItem(HeldItem, HeldItem.StackCount);
                        }
                    }
                    HeldItem = null;
                }
            }

            EnablePlayerMovent(true); // enable the player
            dropPanel.gameObject.SetActive(false); // disable the drop panel


            // TODO change this to use the new input system

            // watch for player interactions with chests and items in the world
            if (Input.GetMouseButtonDown(1))
            {
                /* Ray ray = Camera.main.ViewportPointToRay(new Vector3(.5f, .5f, 0));
                 RaycastHit hit;
                 if (Physics.Raycast(ray, out hit, 5))
                 {*/


                // Cast a ray from the mouse position into the scene
                Ray ray = Camera.main.ScreenPointToRay(new Vector3(0.5f, 0.5f, 0f));

                // Perform a raycast with the layer mask ignoring the drop panel layer and the player layer
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, dropPanelLayerask))
                {
                    Debug.Log(hit.transform.name);
                    // if the player right clicks on a chest then open the chest
                    if (hit.transform.CompareTag("Chest"))
                    {
                        OpenChest(hit.transform.gameObject.GetComponent<ChestController>());
                    }

                    else if (!hit.transform.CompareTag("Item"))
                    {
                        // if the player right clicks on the ground then place the selected item in the world
                        if (ItemBar.SelectedSlotController.Slot.Item != null)
                        {
                            // get the selected item from the item bar
                            Item item = ItemBar.SelectedSlotController.Slot.Item;

                            // place the selected item from the item bar in the world and the hit point.
                            PlaceItem(item, hit);
                        }
                    }
                }
            }
            else if (Input.GetMouseButtonDown(0))
            {
                // destroy item
            }
            //  }
        }

        private void PlaceItem(Item item, RaycastHit hit)
        {
            // if the item has a world prefab then place it in the world
            if (item.data.worldPrefab != null)
            {
                // place the item in the world

                // TODO Add pooling
                GameObject go = Instantiate(item.data.worldPrefab, hit.point, Quaternion.identity);

                // register the item with the inventory controller
                ItemPlaced(item, go);

                // remove the item from the players inventory
                ItemBar.SelectedSlotController.Slot.IncermentStackCount(-1);

                // if the item stack count is 0 then remove the item from the slot
                if (ItemBar.SelectedSlotController.Slot.Item.StackCount <= 0)
                {
                    // remove the item from the slot
                    ItemBar.SelectedSlotController.Slot.SetItem(null);
                }
            }
        }

        /// <summary>
        /// method used to create an empty player inventory
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

        /// <summary>
        /// method to register a callback for when the selected item changes
        /// </summary>
        /// <param name="callbacK"></param>
        public static void RegisterOnSlectedItemChangeCallback(Action<Item> callbacK)
        {
            Instance.OnSelectedItemChangeCallBack += callbacK;
        }

        /// <summary>
        /// methdo to unregister a callback for when the selected item changes
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
        /// method to resize a given inventory to the given size.
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
                        DropItem(s.Item, s.Item.StackCount);
                    }
                }
            }
            else
            {
                foreach (Slot s in oldInventory)
                {
                    newInv[s.slotID] = s;
                }
            }
            return newInv;
        }

        /// <summary>
        /// method to enable or diable the players ablity to move.
        /// </summary>
        /// <param name="b"></param>
        private void EnablePlayerMovent(bool b)
        {

        }

        /// <summary>
        /// method called whenever an inventory panel is opened.
        /// </summary>
        /// <param name="window"></param>
        private void WindowOpenCallback(InventorySystemPanel window)
        {
            //  EnablePlayerMovent(false);// disable player movement while windows are open

            dropPanel.gameObject.SetActive(true); // turn on the drop panel

            Cursor.lockState = CursorLockMode.None; // unlock the mouse

            if (HeldItem == null)
            {
                Cursor.visible = true; // show the mouse
            }
        }

        /// <summary>
        /// method to display the chest Panel and the selected chest inventory
        /// </summary>
        private void OpenChest(ChestController chestController)
        {
            if (chestController != null)
            {
                ChestPanel.Chest = chestController; // pass the selected chest to the chest panel
                ChestPanel.OpenChest(true);
                ChestPanel.gameObject.SetActive(true); // display the chest panel
            }
        }

        /// <summary>
        /// Method for the game to spawn a dropped item in to the world.
        /// use this when you want an item to be dropped by a NPC or spawned when the player destroys an object
        /// </summary>
        /// <param name="itemID">The id of the item to spawn</param>
        /// <param name="position">the location to spawn the item</param>
        /// <param name="stackCount">the size of the stack to spawn</param>
        /// <param name="TTL">The time the item will reamin in the world</param>
        /// <returns>Returns true on success else false</returns>
        public static bool SpawnDroppedItem(int itemID, Vector3 position, int stackCount = 1, float TTL = 30, float Durability = 0)
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
            GameObject g = Instantiate(prefab, position, Quaternion.identity);
            if (g.TryGetComponent<DroppedItem>(out var di))
            {
                di.ItemID = itemData.id;
                di.stackCount = stackCount;
                di.TTL = TTL;
                DroppedItems.Add(di);
            }
            else
            {
                Debug.LogWarning("ItemPickup component missing from spawned item prefab. Item can not be picked up without it.");
            }

            return true;
        }

        /// <summary>
        /// Method to spawn a chest item that was previously saved.
        /// </summary>
        /// <param name="chestID"></param>
        /// <param name="itemCatalogID"></param>
        /// <param name="inventory"></param>
        /// <param name="sTransform"></param>
        internal static void SpawnChest(int chestID, int itemCatalogID, Inventory inventory, SerialTransform sTransform)
        {
            Quaternion q = Quaternion.Euler(sTransform.Rotation);
            GameObject go = Instantiate(Instance.ItemCatalog.list[itemCatalogID].worldPrefab, sTransform.Position, q);
            go.transform.localScale = sTransform.Scale;
            ChestController cc = go.GetComponent<ChestController>();
            cc.ChestID = chestID;
            cc.ItemCatalogID = itemCatalogID;
            MapChest(cc);
            cc.Inventory = inventory;
        }

        /// <summary>
        /// method to store a chest for saving
        /// </summary>
        /// <param name="cc"></param>
        internal static void MapChest(ChestController cc)
        {
            ChestInventories.Add(cc.ChestID, cc.Inventory ?? new Inventory(cc.ChestID, cc.Capacity));
            ChestMap.Add(cc.ChestID, cc.gameObject);
        }

        /// <summary>
        /// method called when player places an item to register item to be saved. This method also set the objects world position and other values
        /// </summary>
        /// <param name="item"></param>
        /// <param name="go"></param>
        internal static void ItemPlaced(Item item, GameObject go)
        {
            /// if this item is a chest then we need to map it corretly to be able to save it
            if (item.data.itemType == ITEMTYPE.CHEST)
            {
                int id = GetNewChestID();
                // fix Null-Coalescing issue
                //ChestController cc = go.GetComponent<ChestController>() ?? go.AddComponent<ChestController>();
                if (go.TryGetComponent(out ChestController cc) == false)
                {
                    cc = go.AddComponent<ChestController>();
                }

                cc.ChestID = id;

                cc.ItemCatalogID = item.data.id;

                MapChest(cc);

                // adjust chest position so it aligns to closest unit
                Vector3 cPos = go.transform.position;

                go.transform.position = new Vector3(Mathf.Round(cPos.x), Mathf.Round(cPos.y), Mathf.Round(cPos.z));

                // rotate chest to point the towards the player (nearest 90)
                Vector3 cRot = go.transform.rotation.eulerAngles;
                go.transform.rotation = Quaternion.Euler(cRot.x, Mathf.Round(InventoryController.Instance.Player.transform.localRotation.eulerAngles.y / 90) * 90, cRot.z);

            }
            else
            {
                // register item in the world items list

                if (go.TryGetComponent(out PlacedItem pi) == false)
                {
                   pi = go.AddComponent<PlacedItem>();
                }
                
                // PlacedItem pi = go.GetComponent<PlacedItem>() ?? go.AddComponent<PlacedItem>();
                pi.ItemID = item.data.id;
                PlacedItems.Add(pi);
            }
        }

        /// <summary>
        /// method for player to drop an item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="quantity"></param>
        internal static void DropItem(Item item, int quantity = 1)
        {
            GameObject prefab;

            if (quantity > 1)
            {
                prefab = item.data.worldPrefabMultiple; // get the multiple prefab
            }
            else
            {
                prefab = item.data.worldPrefabSingle; // get the default drop prefab for the selected item
            }

            if (prefab != null) //  make sure we have a prefabe
            {

                // instatiate teh prefabe and 'toss' it our infront of the player
                Vector3 dropPoint = Instance.Player.transform.position + Instance.Player.transform.forward + Instance.Player.transform.up; // TODO make this more acurate, and show setting on the editor
                GameObject g = Instantiate(prefab, dropPoint, Quaternion.identity);
                g.GetComponent<Rigidbody>().AddForceAtPosition(Instance.Player.transform.forward * 2 + Instance.Player.transform.up, g.transform.position, ForceMode.Impulse);
                if (g.TryGetComponent<DroppedItem>(out var ip))
                {
                    ip.ItemID = item.data.id;
                    ip.stackCount = quantity;
                    ip.TTL = Instance.DroppedItemTTL;
                    DroppedItems.Add(ip);
                }
                else
                {
                    Debug.LogWarning("ItemPickup component missing from dropped item prefab. Dropped item can not be picked up without it.");
                }
            }
            else
            {
                Debug.LogWarning("Missing drop prefab on item " + item.data.name);
            }
        }

        /// <summary>
        /// method to add a single item directly in to the players itemBar or inventory
        /// </summary>
        /// <param name="itemID">The ID of the item to be added</param>
        /// <returns>Returns true on success else false</returns>
        public static bool GivePlayerItem(int itemID, int stackCount = 1)
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
        /// method to allow player to pick up an Item and place it in the inventory
        /// This method will try and place the item in the  ItemBar if there is space
        /// if no space there it will try the players inventory. Either way will return true
        /// if both fail it will return false
        /// </summary>
        /// <param name="collectedObject">The GameObject that is to be picked up</param>
        /// <returns></returns>
        internal static bool PickUpItem(GameObject collectedObject)
        {

            if (!collectedObject.CompareTag("Item"))
            {
                return false;
            }
            if (!collectedObject.TryGetComponent<DroppedItem>(out var ip))
            {
                return false;
            }
            Item newItem = new Item(ip.ItemID, ip.stackCount);

            if (ItemBarInventory.AddItem(newItem) == false)
            {
                if (PlayerInventory.AddItem(newItem) == false)
                {
                    return false;
                }
            }

            if (DroppedItems.Contains(ip))
            {
                DroppedItems.Remove(ip);
            }
            Instance.Player.GetComponent<AudioSource>().PlayOneShot(Instance.Player.GetComponent<InventoryPlayerController>().pickupSound);
            GameObject.Destroy(collectedObject);
            return true;

        }

        /// <summary>
        /// method to toggle the inventory panel
        /// </summary>
        public void ToggleInventoryPanel()
        {
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
        /// method to return a slot from an inventory
        /// </summary>
        /// <param name="index">the index of the inventory in the inventory list</param>
        /// <param name="slotID">the ID of the slot in the inventory</param>
        /// <returns></returns>
        internal static Slot GetInventorySlot(int index, int slotID)
        {

            if ((InventoryList[index].Count - 1) >= slotID)
            {
                try
                {
                    return GetInventory(index)[slotID];
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
        /// Method to save the inventory data.
        /// </summary>
        public static void Save()
        {
            Serializer.Save();
        }

        /// <summary>
        /// Method to load the current saved data
        /// </summary>
        public static void Load()
        {
            Serializer.Load();
        }

        private void OnApplicationQuit()
        {
            Save();
        }

    }
}


