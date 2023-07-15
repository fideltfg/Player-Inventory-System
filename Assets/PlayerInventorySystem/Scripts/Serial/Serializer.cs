using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Security.Cryptography;

/// <summary>
/// Classes and methods to serialize and save invnetories and other data
/// </summary>
namespace PlayerInventorySystem.Serial
{
    public class Serializer
    {
        public static string SavePath = "Data";
        public static string SaveFile = "data.dat";


        /// How to change keys to encrypt saved data
        /// Before you publish your game you should change the values of kk, and ii byte arrays below.
        /// run the three lines of code below to output new keys. You can copy them to a new components' Start method 
        /// and the new values will be output to your console
        /// You will then have to format the output in to a byte array. check the existing values to see how this should be done.
        /**
           TripleDESCryptoServiceProvider TDES = new TripleDESCryptoServiceProvider();
           Debug.Log("new kk = " + BitConverter.ToString(TDES.Key));
           Debug.Log("new ii = " + BitConverter.ToString(TDES.IV));
         **/
        private static readonly byte[] kk = { 0x93, 0x17, 0x01, 0x9C, 0xE6, 0xB7, 0x11, 0xD1, 0xE0, 0xE7, 0x1E, 0x5E, 0xF8, 0x17, 0xA5, 0xCC, 0xB3, 0x05, 0x88, 0x26, 0xA5, 0x25, 0x15, 0xBD };
        private static readonly byte[] ii = { 0xD5, 0x65, 0x05, 0xF9, 0xD4, 0x06, 0xDC, 0x42 };

        private static InventorySystemPanel[] panels = { InventoryController.Instance.InventoryPanel, InventoryController.Instance.CharacterPanel, InventoryController.Instance.CraftingPanel, InventoryController.Instance.ChestPanel };

        /// <summary>
        /// The location of the inventory save file.
        /// </summary>
        private static string InventorySaveLocation
        {
            get
            {
                if (InventoryController.Instance.UsePersistentDataPath)
                {
                    return Application.persistentDataPath + "/" + SavePath + "/" + SaveFile;
                }
                else
                {
                    return SavePath + "/" + SaveFile;
                }
            }
        }

        private static bool CheckGenFolder (string path)
        {
            if (Directory.Exists(path))
            {
                return true;
            }
            else
            {
                try
                {
                    Directory.CreateDirectory(path);
                    return true;
                }
                catch (System.Exception e)
                {
                    Debug.Log(e.StackTrace.ToString());
                }
            }
            return false;
        }

        /// <summary>
        /// Writes the given object instance to a binary file.
        /// <para>Object type (and all child types) must be decorated with the [Serializable] attribute.</para>
        /// <para>To prevent a variable from being serialized, decorate it with the [NonSerialized] attribute; cannot be applied to properties.</para>
        /// </summary>
        /// <typeparam name="T">The type of object being written to the XML file.</typeparam>
        /// <param name="filePath">The file path to write the object instance to.</param>
        /// <param name="objectToWrite">The object instance to write to the XML file.</param>
        /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
        private static void WriteToBinaryFile<T> (string filePath, T objectToWrite, bool append = false)
        {
            CheckGenFolder(SavePath);
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            using (var fs = new FileStream(filePath, append ? FileMode.Append : FileMode.Create))
            using (CryptoStream s = new CryptoStream(fs, des.CreateEncryptor(kk, ii), CryptoStreamMode.Write))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(s, objectToWrite);
            }
        }

        /// <summary>
        /// Reads an object instance from a binary file.
        /// </summary>
        /// <typeparam name="T">The type of object to read from the XML.</typeparam>
        /// <param name="filePath">The file path to read the object instance from.</param>
        /// <returns>Returns a new instance of the object read from the binary file.</returns>
        private static T ReadFromBinaryFile<T> (string filePath) where T : class
        {
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (var s = new CryptoStream(fs, des.CreateDecryptor(kk, ii), CryptoStreamMode.Read))
            {
                BinaryFormatter f = new BinaryFormatter();
                object o = f.Deserialize(s) as T;
                s.Close();
                return (T)o;
            }
        }

        /// <summary>
        /// method used by inventory controller ti save the inventory data
        /// </summary>
        internal static void Save ()
        {
            Serializer.WriteToBinaryFile<SerialSaveDataObject>(InventorySaveLocation, GetInventoryData());
        }

        /// <summary>
        /// method used by inventory controller to load the save data file
        /// </summary>
        internal static void Load ()
        {
            if (File.Exists(InventorySaveLocation))
            {
                LoadSerialInventoryData(Serializer.ReadFromBinaryFile<SerialSaveDataObject>(InventorySaveLocation));
            }
            else
            {
                Debug.Log("Inventory File Not Found");
            }
        }

        private static void LoadSerialInventoryData (SerialSaveDataObject saveData)
        {
            // load all inventories
            SerialInventory[] sInventories = saveData.inventories;
            SerialChest[] sChests = saveData.chests;
            SerialRect[] sPanels = saveData.panelLocations;
            SerialDroppedItem[] sWorldItems = saveData.worldItems;

            // unpack players inventory
            InventoryController.Instance.PlayerInventoryCapacity = sInventories[0].slots.Length;
            for (int i = 0; i < sInventories.Length; i++)
            {
                Inventory inventory = Serializer.ConvertFromSerialInventory(sInventories[i]);
                InventoryController.InventoryList[inventory.Index] = inventory;
            }

            // unpack chests
            foreach (SerialChest sc in sChests)
            {
                InventoryController.SpawnChest(sc.ChestID, sc.itemCatalogID, ConvertFromSerialInventory(sc.inventory), sc.transform);
            }


            // unpack panels
            for (int i = 0; i < panels.Length; i++)
            {
                RectTransform rt = panels[i].GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(sPanels[i].width, sPanels[i].height);
                rt.position = new Vector3(sPanels[i].positionX, sPanels[i].positionY, sPanels[i].positionZ);
            }

            // unpack and spawn world items
            foreach (SerialDroppedItem sWi in sWorldItems)
            {
                if (sWi.itemID > 0)
                {
                    InventoryController.SpawnDroppedItem(sWi.itemID, new Vector3(sWi.x, sWi.y + .25f, sWi.z), sWi.stackCount, sWi.TTL);
                }
            }
        }

        private static SerialSaveDataObject GetInventoryData ()
        {
            // get an array of all the inventories to be saved
            Inventory[] inventories = { InventoryController.PlayerInventory, InventoryController.ItemBarInventory, InventoryController.CraftingInventory, InventoryController.CharacterInventory };

            // convert each inventory in to a serialInventory objects
            SerialInventory[] sInventories = new SerialInventory[inventories.Length];

            for (int i = 0; i < inventories.Length; i++)
            {
                sInventories[i] = ConvertToSerialInventory(inventories[i]);
            }

            SerialChest[] sChests = new SerialChest[InventoryController.ChestMap.Count];
            int cID = 0;
            foreach (GameObject chestObject in InventoryController.ChestMap.Values)
            {
                ChestController cc = chestObject.GetComponent<ChestController>();
                SerialTransform st = new SerialTransform(cc.transform);
                sChests[cID] = new SerialChest()
                {
                    ChestID = cc.ChestID,
                    transform = st,
                    inventory = ConvertToSerialInventory(cc.Inventory),
                    itemCatalogID = cc.ItemCatalogID
                };
                cID++;
            }


            // collect all panel locations
            SerialRect[] sPanelLocations = new SerialRect[panels.Length];
            for (int i = 0; i < panels.Length; i++)
            {
                RectTransform rt = panels[i].GetComponent<RectTransform>();
                if (rt != null)
                {
                    sPanelLocations[i] = new SerialRect(rt);

                }
                else
                {
                    sPanelLocations[i] = null;
                }
            }

            // collect data for dropped and spawned items
            SerialDroppedItem[] sWorldItems = new SerialDroppedItem[InventoryController.DroppedItems.Count];
            int x = 0;
            foreach (DroppedItem di in InventoryController.DroppedItems)
            {
                sWorldItems[x] = new SerialDroppedItem()
                {
                    itemID = di.ItemID,
                    stackCount = di.stackCount,
                    TTL = di.TTL - di.timer,
                    x = di.loadSpawnPoint.x,
                    y = di.loadSpawnPoint.y,
                    z = di.loadSpawnPoint.z
                };
                x++;
            }

            return new SerialSaveDataObject()
            {
                // inventories
                inventories = sInventories,
                chests = sChests,
                panelLocations = sPanelLocations,
                worldItems = sWorldItems
            };
        }

        private static Inventory ConvertFromSerialInventory (SerialInventory sInventory)
        {

            Inventory newInventory = new Inventory(sInventory.Index, sInventory.slots.Length);


            for (int i = 0; i < sInventory.slots.Length; i++)
            {
                SerialSlot sSlot = sInventory.slots[i];
                if (sSlot.itemID > 0)
                {
                    newInventory[sSlot.slotID].SetItem(new Item(sSlot.itemID, sSlot.stackCount) { durability = sSlot.durability });
                }
                else
                {
                    newInventory[sSlot.slotID].SetItem(null);
                }
            }


            return newInventory;
        }

        private static SerialInventory ConvertToSerialInventory (Inventory inventory)
        {
            if (inventory == null)
            {
                return new SerialInventory()
                {
                    Index = -1,
                    slots = null
                };
            }

            SerialInventory serialInventory = new SerialInventory()
            {
                Index = inventory.Index,
                slots = new SerialSlot[inventory.Count]
            };

            for (int s = 0; s < inventory.Count; s++)
            {
                Slot slot = inventory[s];
                SerialSlot serialSlot;

                if (slot.Item != null)
                {
                    serialSlot = new SerialSlot()
                    {
                        slotID = slot.slotID,
                        itemID = slot.Item.data.id,
                        stackCount = slot.ItemStackCount,
                        durability = slot.Item.durability
                    };
                }
                else
                {
                    serialSlot = new SerialSlot()
                    {
                        slotID = slot.slotID,
                        itemID = 0,
                        stackCount = 0,
                        durability = 0
                    };
                }
                serialInventory.slots[s] = serialSlot;
            }


            return serialInventory;
        }
    }

    /// <summary>
    /// The main Serial Data Class used when saving inventory system data
    /// </summary>
    [Serializable]
    internal class SerialSaveDataObject
    {
        /// <summary>
        /// An array of all the inventorys int he system serialzed
        /// </summary>
        public SerialInventory[] inventories;

        /// <summary>
        /// An array of all the chests in the system serialzed
        /// </summary>
        public SerialChest[] chests;

        /// <summary>
        /// an array of all the panel locations in the system serialzed
        /// </summary>
        public SerialRect[] panelLocations;

        /// <summary>
        /// an array of all the dropped or spawned items inthe game world at save time serialzed
        /// </summary>
        public SerialDroppedItem[] worldItems;
    }

    /// <summary>
    /// Class to hold details of inventories when saving
    /// </summary>
    [Serializable]
    internal class SerialInventory
    {
        /// <summary>
        ///  The inventories index/ID
        /// </summary>
        public int Index;

        /// <summary>
        /// An array of the slots that make up this inventory
        /// </summary>
        public SerialSlot[] slots;

    }

    /// <summary>
    /// Clas to hold details of slots when saving
    /// </summary>
    [Serializable]
    internal class SerialSlot
    {
        public int slotID;
        public int itemID;
        public int stackCount;
        public float durability;
    }

    /// <summary>
    /// Class to hold chest details when saving
    /// </summary>
    [Serializable]
    internal class SerialChest
    {
        public int ChestID;
        public int itemCatalogID;
        public SerialTransform transform;
        public SerialInventory inventory;
    }
  
    /// <summary>
    /// Class to hold data about dropped or spawned items when saving
    /// </summary>
    [Serializable]
    internal class SerialDroppedItem
    {
        public int itemID;
        public int stackCount;
        public int durablity;
        public float TTL;
        public float x;
        public float y;
        public float z;
    }

    /// <summary>
    /// Class to hold details of a rectTrasform for saving
    /// </summary>
    [Serializable]
    internal class SerialRect
    {
        public float height;
        public float width;

        public float positionX;
        public float positionY;
        public float positionZ;

        public float rotationX;
        public float rotationY;
        public float rotationZ;

        public float scaleX;
        public float scaleY;
        public float scaleZ;

        public Vector3 Position
        {
            get { return new Vector3(positionX, positionY, positionZ); }
        }

        public Vector3 Rotation
        {
            get { return new Vector3(rotationX, rotationY, rotationZ); }
        }

        public Vector3 Scale
        {
            get { return new Vector3(scaleX, scaleY, scaleZ); }
        }


        /// <summary>
        /// Pass in the RectTransform you wish to serialize
        /// </summary>
        /// <param name="rt"></param>
        public SerialRect (RectTransform rt)
        {
            width = rt.sizeDelta.x;
            height = rt.sizeDelta.y;
            positionX = rt.position.x;
            positionY = rt.position.y;
            positionZ = rt.position.z;
            rotationX = rt.localRotation.eulerAngles.x;
            rotationY = rt.localRotation.eulerAngles.y;
            rotationZ = rt.localRotation.eulerAngles.z;
            scaleX = rt.localScale.x;
            scaleY = rt.localScale.y;
            scaleZ = rt.localScale.z;
        }
    }
 
    /// <summary>
    /// Class to hold the details of a transform when saving.
    /// </summary>
    [Serializable]
    internal class SerialTransform
    {
        [SerializeField]
        public float positionX;
        public float positionY;
        public float positionZ;
        public float rotationX;
        public float rotationY;
        public float rotationZ;
        public float scaleX;
        public float scaleY;
        public float scaleZ;

        public Vector3 Position
        {
            get { return new Vector3(positionX, positionY, positionZ); }
        }

        public Vector3 Rotation
        {
            get { return new Vector3(rotationX, rotationY, rotationZ); }
        }

        public Vector3 Scale
        {
            get { return new Vector3(scaleX, scaleY, scaleZ); }
        }

        /// <summary>
        /// Construst. Pass in the transform you wish to serailize
        /// </summary>
        /// <param name="t"></param>
        public SerialTransform (Transform t)
        {
            positionX = t.position.x;
            positionY = t.position.y;
            positionZ = t.position.z;

            rotationX = t.localRotation.eulerAngles.x;
            rotationY = t.localRotation.eulerAngles.y;
            rotationZ = t.localRotation.eulerAngles.z;

            scaleX = t.localScale.x;
            scaleY = t.localScale.y;
            scaleZ = t.localScale.z;
        }

    }
}
