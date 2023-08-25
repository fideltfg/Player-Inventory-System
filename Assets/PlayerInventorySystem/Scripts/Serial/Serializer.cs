using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Security.Cryptography;
using System.Linq;
using static UnityEditor.Progress;
using UnityEngine.UIElements;


namespace PlayerInventorySystem.Serial
{
    public class Serializer
    {
        // File paths and encryption keys
        public static string SavePath = "Data";
        public static string SaveFile = "data.dat";

        private static readonly byte[] kk = { 0x93, 0x17, 0x01, 0x9C, 0xE6, 0xB7, 0x11, 0xD1, 0xE0, 0xE7, 0x1E, 0x5E, 0xF8, 0x17, 0xA5, 0xCC, 0xB3, 0x05, 0x88, 0x26, 0xA5, 0x25, 0x15, 0xBD };
        private static readonly byte[] ii = { 0xD5, 0x65, 0x05, 0xF9, 0xD4, 0x06, 0xDC, 0x42 };

        /// private static readonly byte[] kk = { *//* Encryption Key - 24 bytes *//* };
        /// private static readonly byte[] ii = { *//* Initialization Vector - 8 bytes *//* };

        // Array of panels for serialization
        private static InventorySystemPanel[] panels = { InventoryController.Instance.InventoryPanel, InventoryController.Instance.CharacterPanel, InventoryController.Instance.CraftingPanel, InventoryController.Instance.ChestPanel };

        // Helper method to get the complete inventory save location
        private static string InventorySaveLocation
        {
            get
            {
                if (InventoryController.Instance.UsePersistentDataPath)
                {
                    string s = Application.persistentDataPath + "/" + SavePath + "/" + SaveFile;
                    // Debug.Log(s);
                    return s;
                }
                else
                {
                    //Debug.Log(SavePath + "/" + SaveFile);
                    return SavePath + "/" + SaveFile;
                }
            }
        }

        // Method used by InventoryController to save the inventory data
        internal static void Save()
        {
            SerialSaveDataObject ssdo = GetSerializeGameData();
            WriteToBinaryFile(InventorySaveLocation, ssdo);
        }

        // Method used by InventoryController to load the saved data file
        internal static void Load()
        {
            if (File.Exists(InventorySaveLocation))
            {

                SerialSaveDataObject ssdo = ReadFromBinaryFile<SerialSaveDataObject>(InventorySaveLocation);

                LoadSerialInventoryData(ssdo);
            }
            else
            {
                //if you need to do something if the file does not exist do it here
                Debug.Log("Save Data File Not Found!");
            }
        }

        // Helper method to check and create a folder if it does not exist
        private static bool CheckGenFolder(string path)
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

        // Helper method to write an object instance to a binary file
        private static void WriteToBinaryFile<T>(string filePath, T objectToWrite, bool append = false)
        {
            // Create the folder if it does not exist
            CheckGenFolder(SavePath);


            // Create the file if it does not exist
            using var fs = new FileStream(filePath, append ? FileMode.Append : FileMode.Create);

            // Encrypt the file
            TripleDESCryptoServiceProvider des = new();


            // Write the object to the file
            using CryptoStream s = new(fs, des.CreateEncryptor(kk, ii), CryptoStreamMode.Write);

            // Serialize the object
            BinaryFormatter formatter = new();
            formatter.Serialize(s, objectToWrite);


            // Serialize the object to JSON
            string json = JsonUtility.ToJson(objectToWrite, true);

            // Write the JSON to the file
            File.WriteAllText(SavePath + "/data.json", json);

            s.Close();

        }

        // Helper method to read an object instance from a binary file
        private static T ReadFromBinaryFile<T>(string filePath) where T : class
        {
            TripleDESCryptoServiceProvider des = new();
            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var s = new CryptoStream(fs, des.CreateDecryptor(kk, ii), CryptoStreamMode.Read);
            BinaryFormatter f = new();
            object o = f.Deserialize(s) as T;
            s.Close();
            return (T)o;
        }

        // Helper method to load the serialized inventory data
        private static void LoadSerialInventoryData(SerialSaveDataObject data)
        {
            SerialInventory[] sInventories = data.Inventories;
            SerialChest[] sChests = data.Chests;
            SerialRect[] sPanels = data.PanelLocations;
            SerialDroppedItem[] sDroppedItems = data.DroppedItems;
            SerialPlacedItem[] sPlacedItems = data.PlacedItems;

            // Debug.Log("Loading Inventory Data");
            // Load all inventories

            if (sInventories[0] != null)
            {
                if (sInventories[0].SerialSlots != null)
                {
                    InventoryController.PlayerInventoryCapacity = sInventories[0].SerialSlots.Length;
                }
                else
                {
                    // REDUNDANT??
                    InventoryController.PlayerInventoryCapacity = 24;
                }
            }

            foreach (SerialInventory sInv in sInventories)
            {
                InventoryController.InventoryList[sInv.Index] = new Inventory(sInv);
            }

            // Load saved chests
            foreach (SerialChest sc in sChests)
            {
                _ = InventoryController.SpawnChest(sc.ChestID, sc.ItemID, sc.Transform.Position, Quaternion.Euler(sc.Transform.Rotation), sc.Transform.Scale, new Inventory(sc.Inventory));
            }

            // Load panel locations
            for (int i = 0; i < panels.Length; i++)
            {
                if (panels[i].TryGetComponent<RectTransform>(out RectTransform rt))
                {
                    // DISABLED FOR NOW
                    // rt.sizeDelta = sPanels[i].Size;
                    // rt.position = sPanels[i].Position;
                }
            }

            // Load and spawn dropped items
            foreach (SerialDroppedItem sdi in sDroppedItems)
            {
                if (sdi != null && sdi.ItemID > 0)
                {
                    InventoryController.Instance.SpawnDroppedItem(sdi);
                }
            }

            // load and spawn placed items
            foreach (SerialPlacedItem spi in sPlacedItems)
            {
                Item item = Item.New(spi.ItemID);
                if (item.Data.worldPrefab != null)
                {
                    switch (item.Data.worldPrefab.tag.ToLower())
                    {
                        case "chest":
                            _ = InventoryController.SpawnChest(InventoryController.GetNewChestID(), spi.ItemID, spi.Transform.Position, Quaternion.Euler(spi.Transform.Rotation), spi.Transform.Scale);
                            InventoryController.OnPlaceItem(item, null, false);
                            break;
                        case "craftingtable":
                            CraftingTableController cTc = InventoryController.SpawnCraftingTable(spi.ItemID, spi.Transform.Position, Quaternion.Euler(spi.Transform.Rotation), spi.Transform.Scale);
                            InventoryController.OnPlaceItem(item, cTc, false);
                            break;
                        default:
                            Item.Place(item, spi.Transform.Position, Quaternion.Euler(spi.Transform.Rotation), spi.Transform.Scale);
                            break;
                    }

                }

            }
        }

        // Helper method to get the serialized inventory data
        private static SerialSaveDataObject GetSerializeGameData()
        {
            //   Debug.Log("Collecting Data to Save");

            //  Debug.Log("Collecting Inventories");
            Inventory[] inventories = InventoryController.InventoryList.Values.ToArray();
            int playerInventoryCapacity = InventoryController.PlayerInventoryCapacity;
            SerialInventory[] sInventories = new SerialInventory[inventories.Length];


            // Convert each inventory to SerialInventory

            foreach (Inventory inventory in inventories)
            {
                sInventories[inventory.Index] = new SerialInventory(inventory);
            }

            // Convert chests to SerialChest
            // Debug.Log("Converting Chests to SerialChests");
            SerialChest[] sChests = new SerialChest[InventoryController.ChestMap.Count];
            int ii = 0;
            foreach (GameObject chestObject in InventoryController.ChestMap.Values)
            {
                ChestController cc = chestObject.GetComponent<ChestController>();
                sChests[ii] = new SerialChest(cc.ItemID, new(cc.transform))
                {
                    Inventory = new(cc.Inventory),
                    ChestID = cc.ChestID
                };
                ii++;
            }

            // Collect panel locations
            // Debug.Log("Collecting Panel Locations");
            SerialRect[] sPanelLocations = new SerialRect[panels.Length];
            for (int i = 0; i < panels.Length; i++)
            {
                //  Debug.Log("Converting Panel " + i + " to SerialRect");
                if (panels[i].TryGetComponent<RectTransform>(out var rt))
                {
                    sPanelLocations[i] = new SerialRect(rt);
                }
                else
                {
                    sPanelLocations[i] = null;
                }
            }

            // Collect data for dropped and spawned items
             Debug.Log("Collecting Dropped Items");
              SerialDroppedItem[] sDroppedItems = new SerialDroppedItem[InventoryController.Instance.DroppedItems.Count];
                int x = 0;
               foreach (DroppedItem di in InventoryController.Instance.DroppedItems)
               {
                   //  Debug.Log("Converting Dropped Item " + di.name + " to SerialDroppedItem");
                   sDroppedItems[x] = new SerialDroppedItem(di.ItemID, new SerialTransform(di.transform), di.StackCount, di.TimeToLive - di.Timer);
                   x++;
               }


            // Collect data for placed items
            Debug.Log("Collecting Placed Items");
            SerialPlacedItem[] sPlacedItems = new SerialPlacedItem[InventoryController.PlacedItems.Count];
            int p = 0;
            foreach (PlacedItem pi in InventoryController.PlacedItems)
            {
                //  Debug.Log("Converting Placed Item " + pi.name + " to SerialPlacedItem");
                sPlacedItems[p] = new SerialPlacedItem(pi.ItemID, new SerialTransform(pi.transform));
                p++;
            }

            SerialSaveDataObject ssdo = new SerialSaveDataObject(sInventories, sChests, sPanelLocations, sDroppedItems, sPlacedItems);

            ssdo.PlayerInventoryCapacity = playerInventoryCapacity;
            return ssdo;
        }

    }
}