using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Security.Cryptography;
using System.Linq;

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
            WriteToBinaryFile(InventorySaveLocation, GetSerializeGameData());
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

            // Close the file
            s.Close();

            // Close the file stream
            fs.Close();

            // Dispose of the crypto service provider
            des.Dispose();

            // Dispose of the crypto stream
            s.Dispose();

            // Dispose of the file stream
            fs.Dispose();



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
            SerialSaveDataObject ssdo = data;

            SerialInventory[] sInventories = data.Inventories;
            SerialChest[] sChests = data.Chests;
            SerialRect[] sPanels = data.PanelLocations;
            SerialDroppedItem[] sWorldItems = data.WorldItems;

            Debug.Log("Loading Inventory Data");
            // Load all inventories

            if (sInventories[0] != null)
            {
                Debug.Log("ok 1");
                if (sInventories[0].SerialSlots != null)
                {
                    Debug.Log("ok 2");
                    InventoryController.PlayerInventoryCapacity = sInventories[0].SerialSlots.Length;
                }
                else
                {
                    Debug.Log("ok 3");
                    InventoryController.PlayerInventoryCapacity = 24;
                }
            }

            foreach (SerialInventory sInv in sInventories)
            {
                InventoryController.InventoryList[sInv.Index] = new Inventory(sInv);
            }

            Debug.Log("Inventories Loaded");

            Debug.Log("Loading Chest Data");
            // Load saved chests
            foreach (SerialChest sc in sChests)
            {
                _ = InventoryController.SpawnChest(sc.ChestID, sc.ItemCatalogID, sc.Transform.Position, Quaternion.Euler(sc.Transform.Rotation), sc.Transform.Scale);
            }
            Debug.Log("Chests Loaded");

            Debug.Log("Loading Panel Data");
            // Load panel locations

            for (int i = 0; i < panels.Length; i++)
            {
                if (panels[i].TryGetComponent<RectTransform>(out RectTransform rt))
                {
                    rt.sizeDelta = sPanels[i].Size;
                    rt.position = sPanels[i].Position;
                }
            }
            Debug.Log("Panels Loaded");

            Debug.Log("Loading World Item Data");
            // Load and spawn dropped items
            foreach (SerialDroppedItem sWi in sWorldItems)
            {
                if (sWi.ItemID > 0)
                {
                    InventoryController.Instance.SpawnDroppedItem(sWi.ItemID, sWi.Position, sWi.StackCount, sWi.TimeToLive);
                }
            }
          //  Debug.Log("World Items Loaded");
         //   Debug.Log("Inventory Data Loaded *************************************************************");
        }

        // Helper method to get the serialized inventory data
        private static SerialSaveDataObject GetSerializeGameData()
        {
         //   Debug.Log("Collecting Data to Save");
            Inventory[] inventories = InventoryController.InventoryList.Values.ToArray();
            SerialInventory[] sInventories = new SerialInventory[inventories.Length];

            // Convert each inventory to SerialInventory
         //   Debug.Log("Collecting Inventories");
            foreach (Inventory inventory in inventories)
            {
                sInventories[inventory.Index] = new SerialInventory(inventory);
            }

            // Convert chests to SerialChest
            //  Debug.Log("Converting Chests to SerialChests");
            SerialChest[] sChests = new SerialChest[InventoryController.ChestMap.Count];
            int ii = 0;
            foreach (GameObject chestObject in InventoryController.ChestMap.Values)
            {
                //  Debug.Log("Converting Chest " + ii + " to SerialChest");
                ChestController cc = chestObject.GetComponent<ChestController>();
                SerialTransform st = new(cc.transform);
                SerialInventory si = new(cc.Inventory);

                sChests[ii] = new SerialChest(cc.ChestID, cc.ItemCatalogID, st, si);
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
            // Debug.Log("Collecting Dropped Items");
            SerialDroppedItem[] sWorldItems = new SerialDroppedItem[InventoryController.Instance.DroppedItems.Count];
            int x = 0;
            foreach (DroppedItem di in InventoryController.Instance.DroppedItems)
            {
                //  Debug.Log("Converting Dropped Item " + di.name + " to SerialDroppedItem");
                sWorldItems[x] = new SerialDroppedItem(di.ItemID, di.StackCount, di.Durability, di.TimeToLive - di.Timer, di.transform.position);
                x++;
            }

            return new SerialSaveDataObject(sInventories, sChests, sPanelLocations, sWorldItems);
        }

    }
}