using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Security.Cryptography;

namespace PlayerInventorySystem
{
    public class Serializer
    {
        public static string SavePath = "Data";
        public static string SaveFile = "data.dat";

        private static readonly byte[] kk = { 0x93, 0x17, 0x01, 0x9C, 0xE6, 0xB7, 0x11, 0xD1, 0xE0, 0xE7, 0x1E, 0x5E, 0xF8, 0x17, 0xA5, 0xCC, 0xB3, 0x05, 0x88, 0x26, 0xA5, 0x25, 0x15, 0xBD };
        private static readonly byte[] ii = { 0xD5, 0x65, 0x05, 0xF9, 0xD4, 0x06, 0xDC, 0x42 };

        private static InventorySystemPanel[] panels = { InventoryController.Instance.InventoryPanel, InventoryController.Instance.CharacterPanel, InventoryController.Instance.CraftingPanel, InventoryController.Instance.ChestPanel };

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

        private static void WriteToBinaryFile<T>(string filePath, T objectToWrite, bool append = false)
        {
            CheckGenFolder(SavePath);
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            using var fs = new FileStream(filePath, append ? FileMode.Append : FileMode.Create);
            using CryptoStream s = new(fs, des.CreateEncryptor(kk, ii), CryptoStreamMode.Write);
            BinaryFormatter formatter = new();
            formatter.Serialize(s, objectToWrite);
        }

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

        internal static void Save()
        {
            WriteToBinaryFile<SerialSaveDataObject>(InventorySaveLocation, GetInventoryData());
        }

        internal static void Load()
        {
            if (File.Exists(InventorySaveLocation))
            {
                LoadSerialInventoryData(ReadFromBinaryFile<SerialSaveDataObject>(InventorySaveLocation));
            }
            else
            {
                Debug.Log("Inventory File Not Found");
            }
        }

        private static void LoadSerialInventoryData(SerialSaveDataObject saveData)
        {
            SerialInventory[] sInventories = saveData.Inventories;
            SerialChest[] sChests = saveData.Chests;
            SerialRect[] sPanels = saveData.PanelLocations;
            SerialDroppedItem[] sWorldItems = saveData.WorldItems;

            InventoryController.Instance.PlayerInventoryCapacity = sInventories[0].Slots.Length;
            for (int i = 0; i < sInventories.Length; i++)
            {
                Inventory inventory = ConvertFromSerialInventory(sInventories[i]);
                InventoryController.InventoryList[inventory.Index] = inventory;
            }

            foreach (SerialChest sc in sChests)
            {
                InventoryController.SpawnSavedChest(sc.ChestID, sc.ItemCatalogID, ConvertFromSerialInventory(sc.Inventory), sc.Transform);
            }

            for (int i = 0; i < panels.Length; i++)
            {
                if (panels[i].TryGetComponent<RectTransform>(out var rt))
                {
                    rt.sizeDelta = sPanels[i].Size;
                    rt.position = sPanels[i].Position;
                }
            }

            foreach (SerialDroppedItem sWi in sWorldItems)
            {
                if (sWi.ItemID > 0)
                {
                    InventoryController.Instance.SpawnDroppedItem(sWi.ItemID, sWi.Position, sWi.StackCount, sWi.TimeToLive);
                }
            }
        }

        private static SerialSaveDataObject GetInventoryData()
        {
            Inventory[] inventories = { InventoryController.PlayerInventory, InventoryController.ItemBarInventory, InventoryController.CraftingInventory, InventoryController.CharacterInventory };
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
                SerialTransform st = new(cc.transform);
                sChests[cID] = new SerialChest(cc.ChestID, cc.ItemCatalogID, st, ConvertToSerialInventory(cc.Inventory));
                cID++;
            }

            SerialRect[] sPanelLocations = new SerialRect[panels.Length];
            for (int i = 0; i < panels.Length; i++)
            {
                if (panels[i].TryGetComponent<RectTransform>(out var rt))
                {
                    sPanelLocations[i] = new SerialRect(rt);
                }
                else
                {
                    sPanelLocations[i] = null;
                }
            }

            SerialDroppedItem[] sWorldItems = new SerialDroppedItem[InventoryController.Instance.DroppedItems.Count];
            int x = 0;
            foreach (DroppedItem di in InventoryController.Instance.DroppedItems)
            {
                sWorldItems[x] = new SerialDroppedItem(di.ItemID, di.StackCount, di.Durability, di.TimeToLive - di.Timer, di.Position);
                x++;
            }

            return new SerialSaveDataObject(sInventories, sChests, sPanelLocations, sWorldItems);
        }

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
                    serialSlot = new SerialSlot(slot.SlotID, slot.Item.Data.ID, slot.StackCount, slot.Item.Durability);
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