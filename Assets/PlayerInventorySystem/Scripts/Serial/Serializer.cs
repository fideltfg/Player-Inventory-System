using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Security.Cryptography;

namespace PlayerInventorySystem.Serial
{
    public class Serializer
    {
        // File paths and encryption keys
        public static string SavePath = "Data";
        public static string SaveFile = "data.dat";

        private static readonly byte[] kk = { 0x93, 0x17, 0x01, 0x9C, 0xE6, 0xB7, 0x11, 0xD1, 0xE0, 0xE7, 0x1E, 0x5E, 0xF8, 0x17, 0xA5, 0xCC, 0xB3, 0x05, 0x88, 0x26, 0xA5, 0x25, 0x15, 0xBD };
        private static readonly byte[] ii = { 0xD5, 0x65, 0x05, 0xF9, 0xD4, 0x06, 0xDC, 0x42 };

       /* private static readonly byte[] kk = { *//* Encryption Key - 24 bytes *//* };
        private static readonly byte[] ii = { *//* Initialization Vector - 8 bytes *//* };*/

        // Array of panels for serialization
        private static InventorySystemPanel[] panels = { InventoryController.Instance.InventoryPanel, InventoryController.Instance.CharacterPanel, InventoryController.Instance.CraftingPanel, InventoryController.Instance.ChestPanel };

        // Helper method to get the complete inventory save location
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
            CheckGenFolder(SavePath);
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            using var fs = new FileStream(filePath, append ? FileMode.Append : FileMode.Create);
            using CryptoStream s = new(fs, des.CreateEncryptor(kk, ii), CryptoStreamMode.Write);
            BinaryFormatter formatter = new();
            formatter.Serialize(s, objectToWrite);
        }

        // Helper method to read an object instance from a binary file
        private static T ReadFromBinaryFile<T>(string filePath) where T : class
        {
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var s = new CryptoStream(fs, des.CreateDecryptor(kk, ii), CryptoStreamMode.Read);
            BinaryFormatter f = new();
            object o = f.Deserialize(s) as T;
            s.Close();
            return (T)o;
        }

        // Method used by InventoryController to save the inventory data
        internal static void Save()
        {
            WriteToBinaryFile<SerialSaveDataObject>(InventorySaveLocation, GetDataToSave());
        }

        // Method used by InventoryController to load the saved data file
        internal static void Load()
        {
            if (File.Exists(InventorySaveLocation))
            {
                LoadSerialInventoryData(ReadFromBinaryFile<SerialSaveDataObject>(InventorySaveLocation));
            }
            else
            {
                Debug.Log("Save Data File Not Found!");
            }
        }

        // Helper method to load the serialized inventory data
        private static void LoadSerialInventoryData(SerialSaveDataObject saveData)
        {
            SerialInventory[] sInventories = saveData.Inventories;
            SerialChest[] sChests = saveData.Chests;
            SerialRect[] sPanels = saveData.PanelLocations;
            SerialDroppedItem[] sWorldItems = saveData.WorldItems;

            // Load all inventories
            InventoryController.Instance.PlayerInventoryCapacity = sInventories[0].Slots.Length;
            for (int i = 0; i < sInventories.Length; i++)
            {
                Inventory inventory = ConvertFromSerialInventory(sInventories[i]);
                InventoryController.InventoryList[inventory.Index] = inventory;
            }

            // Load saved chests
            foreach (SerialChest sc in sChests)
            {
                InventoryController.SpawnSavedChest(sc.ChestID, sc.ItemCatalogID, ConvertFromSerialInventory(sc.Inventory), sc.Transform);
            }

            // Load panel locations
            for (int i = 0; i < panels.Length; i++)
            {
                if (panels[i].TryGetComponent<RectTransform>(out RectTransform rt))
                {
                    rt.sizeDelta = sPanels[i].Size;
                    rt.position = sPanels[i].Position;
                }
            }

            // Load and spawn dropped items
            foreach (SerialDroppedItem sWi in sWorldItems)
            {
                if (sWi.ItemID > 0)
                {
                    InventoryController.Instance.SpawnDroppedItem(sWi.ItemID, sWi.Position, sWi.StackCount, sWi.TimeToLive);
                }
            }
        }

        // Helper method to get the serialized inventory data
        private static SerialSaveDataObject GetDataToSave()
        {
            Debug.Log("Collecting Data to Save");
            Inventory[] inventories = { InventoryController.PlayerInventory, InventoryController.ItemBarInventory, InventoryController.CraftingInventory, InventoryController.CharacterInventory };
            SerialInventory[] sInventories = new SerialInventory[inventories.Length];

            // Convert each inventory to SerialInventory
            Debug.Log("Collecting Inventories");
            for (int i = 0; i < inventories.Length; i++)
            {
                Debug.Log("Converting Inventory " + i + " to SerialInventory");
                sInventories[i] = ConvertToSerialInventory(inventories[i]);
            }

            // Convert chests to SerialChest
            Debug.Log("Converting Chests to SerialChests");
            SerialChest[] sChests = new SerialChest[InventoryController.ChestMap.Count];
            int ii = 0;
            foreach (GameObject chestObject in InventoryController.ChestMap.Values)
            {
                Debug.Log("Converting Chest " + ii + " to SerialChest");
                ChestController cc = chestObject.GetComponent<ChestController>();
                SerialTransform st = new(cc.transform);
                sChests[ii] = new SerialChest(cc.ChestID, cc.ItemCatalogID, st, ConvertToSerialInventory(cc.Inventory));
                ii++;
            }

            // Collect panel locations
            Debug.Log("Collecting Panel Locations");
            SerialRect[] sPanelLocations = new SerialRect[panels.Length];
            for (int i = 0; i < panels.Length; i++)
            {
                Debug.Log("Converting Panel " + i + " to SerialRect");
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
            SerialDroppedItem[] sWorldItems = new SerialDroppedItem[InventoryController.Instance.DroppedItems.Count];
            int x = 0;
            foreach (DroppedItem di in InventoryController.Instance.DroppedItems)
            {
                Debug.Log("Converting Dropped Item " + di.name + " to SerialDroppedItem");
                sWorldItems[x] = new SerialDroppedItem(di.ItemID, di.StackCount, di.Durability, di.TimeToLive - di.Timer, di.transform.position);
                x++;
            }

            return new SerialSaveDataObject(sInventories, sChests, sPanelLocations, sWorldItems);
        }

        // Helper method to convert SerialInventory to Inventory
        private static Inventory ConvertFromSerialInventory(SerialInventory sInventory)
        {
            Inventory newInventory = new Inventory(sInventory.Index, sInventory.Slots.Length);

            for (int i = 0; i < sInventory.Slots.Length; i++)
            {
                SerialSlot sSlot = sInventory.Slots[i];
                if (sSlot.ItemID > 0)
                {
                    newInventory[sSlot.SlotID].SetItem(new Item(sSlot.ItemID, sSlot.StackCount) { Durability = sSlot.Durability });
                }
                else
                {
                    newInventory[sSlot.SlotID].SetItem(null);
                }
            }

            return newInventory;
        }

        // Helper method to convert Inventory to SerialInventory
        private static SerialInventory ConvertToSerialInventory(Inventory inventory)
        {
            if (inventory == null)
            {
                return new SerialInventory()
                {
                    Index = -1,
                    Slots = null
                };
            }

            SerialInventory serialInventory = new SerialInventory()
            {
                Index = inventory.Index,
                Slots = new SerialSlot[inventory.Count]
            };

            for (int s = 0; s < inventory.Count; s++)
            {
                Slot slot = inventory[s];
                SerialSlot serialSlot;

                if (slot.Item != null)
                {
                    serialSlot = new SerialSlot(slot.SlotID, slot.Item.Data.id, slot.StackCount, slot.Item.Durability);
                }
                else
                {
                    serialSlot = new SerialSlot(slot.SlotID, 0, 0, 0);
                }
                serialInventory.Slots[s] = serialSlot;
            }

            return serialInventory;
        }
    }
}
