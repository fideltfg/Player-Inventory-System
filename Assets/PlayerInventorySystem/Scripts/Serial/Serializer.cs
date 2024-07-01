using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Linq;

namespace PlayerInventorySystem.Serial
{
    public class Serializer
    {
        // File paths and encryption keys
        private const string SavePath = "Data";
        private const string SaveFile = "data.dat";

        private static readonly byte[] kk = { 0x93, 0x17, 0x01, 0x9C, 0xE6, 0xB7, 0x11, 0xD1, 0xE0, 0xE7, 0x1E, 0x5E, 0xF8, 0x17, 0xA5, 0xCC, 0xB3, 0x05, 0x88, 0x26, 0xA5, 0x25, 0x15, 0xBD };
        private static readonly byte[] ii = { 0xD5, 0x65, 0x05, 0xF9, 0xD4, 0x06, 0xDC, 0x42 };

        /// private static readonly byte[] kk = { *//* Encryption Key - 24 bytes *//* };
        /// private static readonly byte[] ii = { *//* Initialization Vector - 8 bytes *//* };

        // Array of panels for serialization
        private static readonly InventorySystemPanel[] panels =
        {
            InventoryController.Instance.InventoryPanel,
            InventoryController.Instance.CharacterPanel,
            InventoryController.Instance.CraftingPanel,
            InventoryController.Instance.ChestPanel,
            InventoryController.Instance.SalvagePanel,
           // InventoryController.Instance.FurnacePanel,
            InventoryController.Instance.AdvancedInventoryPanel

        };

        // Helper method to get the complete inventory save location
        private static string InventorySaveLocation =>
            InventoryController.Instance.UsePersistentDataPath
                ? Path.Combine(Application.persistentDataPath, SavePath, SaveFile)
                : Path.Combine(SavePath, SaveFile);

        // Method used by InventoryController to save the inventory data
        internal static void Save()
        {
            var ssdo = GetSerializeGameData();
            WriteToBinaryFile(InventorySaveLocation, ssdo);
        }

        // Method used by InventoryController to load the saved data file
        internal static bool Load()
        {
            if (File.Exists(InventorySaveLocation))
            {
                var ssdo = ReadFromBinaryFile<SerialSaveDataObject>(InventorySaveLocation);
                LoadSerialInventoryData(ssdo);
                return true;
            }

            Debug.Log("Save Data File Not Found!");
            return false;
        }

        // Helper method to check and create a folder if it does not exist
        private static bool CheckGenFolder(string path)
        {
            if (Directory.Exists(path)) return true;
            try
            {
                Directory.CreateDirectory(path);
                return true;
            }
            catch (System.Exception e)
            {
                Debug.Log(e.StackTrace);
                return false;
            }
        }

        // Helper method to write an object instance to a binary file
        private static void WriteToBinaryFile<T>(string filePath, T objectToWrite, bool append = false)
        {
            CheckGenFolder(SavePath);

            using var fs = new FileStream(filePath, append ? FileMode.Append : FileMode.Create);
            using var des = new TripleDESCryptoServiceProvider();
            using var s = new CryptoStream(fs, des.CreateEncryptor(kk, ii), CryptoStreamMode.Write);

            var formatter = new BinaryFormatter();
            formatter.Serialize(s, objectToWrite);

            var json = JsonUtility.ToJson(objectToWrite, true);
            File.WriteAllText(Path.Combine(SavePath, "data.json"), json);
        }

        // Helper method to read an object instance from a binary file
        private static T ReadFromBinaryFile<T>(string filePath) where T : class
        {
            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var des = new TripleDESCryptoServiceProvider();
            using var s = new CryptoStream(fs, des.CreateDecryptor(kk, ii), CryptoStreamMode.Read);

            var formatter = new BinaryFormatter();
            return formatter.Deserialize(s) as T;
        }

        // Helper method to load the serialized inventory data
        private static void LoadSerialInventoryData(SerialSaveDataObject data)
        {
            var sInventories = data.Inventories;
            InventoryController.PlayerInventoryCapacity = sInventories[0]?.SerialSlots?.Length ?? 24;

            foreach (var sInv in sInventories)
            {
                InventoryController.InventoryList[sInv.Index] = new Inventory(sInv);
            }

            foreach (var sc in data.Chests)
            {
                InventoryController.SpawnChest(sc.ChestID, sc.ItemID, sc.Transform.Position, Quaternion.Euler(sc.Transform.Rotation), sc.Transform.Scale, new Inventory(sc.Inventory));
            }

            for (int i = 0; i < panels.Length; i++)
            {
                if (panels[i].TryGetComponent<RectTransform>(out var rt))
                {
                    // DISABLED FOR NOW
                    rt.sizeDelta = data.PanelLocations[i].Size;
                    rt.position = data.PanelLocations[i].Position;
                }
            }

            foreach (var sdi in data.DroppedItems)
            {
                if (sdi?.ItemID > 0)
                {
                    InventoryController.Instance.SpawnDroppedItem(sdi);
                }
            }

            foreach (var spi in data.PlacedItems)
            {
                var item = Item.New(spi.ItemID);
                if (item.Data.worldPrefab != null)
                {
                    switch (item.Data.worldPrefab.tag.ToLower())
                    {
                        case "chest":
                            InventoryController.SpawnChest(InventoryController.GetNewChestID(), spi.ItemID, spi.Transform.Position, Quaternion.Euler(spi.Transform.Rotation), spi.Transform.Scale);
                            InventoryController.OnPlaceItem(item, null, false);
                            break;
                        case "craftingtable":
                            var cTc = InventoryController.SpawnCraftingTable(spi.ItemID, spi.Transform.Position, Quaternion.Euler(spi.Transform.Rotation), spi.Transform.Scale);
                            InventoryController.OnPlaceItem(item, cTc, false);
                            break;
                        default:
                            var go = GameObject.Instantiate(item.Data.worldPrefab, spi.Transform.Position, Quaternion.Euler(spi.Transform.Rotation));
                            var pi = go.AddComponent<PlacedItem>();
                            InventoryController.OnPlaceItem(item, pi, false);
                            break;
                    }
                }
            }
        }

        // Helper method to get the serialized inventory data
        private static SerialSaveDataObject GetSerializeGameData()
        {
            var inventories = InventoryController.InventoryList.Values.ToArray();
            var sInventories = inventories.Select(inventory => new SerialInventory(inventory)).ToArray();

            var sChests = InventoryController.ChestMap.Values
                .Select(chestObject => chestObject.GetComponent<ChestController>())
                .Select(cc => new SerialChest(cc.ItemID, new SerialTransform(cc.transform)) { Inventory = new SerialInventory(cc.Inventory), ChestID = cc.ID })
                .ToArray();

            var sPanelLocations = panels.Select(panel =>
            {
                panel.TryGetComponent<RectTransform>(out var rt);
                return rt != null ? new SerialRect(rt) : null;
            }).ToArray();

            var sDroppedItems = InventoryController.Instance.DroppedItems
                .Select(di => new SerialDroppedItem(di.ItemID, new SerialTransform(di.transform), di.StackCount, di.TimeToLive - di.Timer))
                .ToArray();

            var sPlacedItems = InventoryController.PlacedItems
                .Select(pi => new SerialPlacedItem(pi.ItemID, new SerialTransform(pi.transform)))
                .ToArray();

            return new SerialSaveDataObject(sInventories, sChests, sPanelLocations, sDroppedItems, sPlacedItems, InventoryController.Character)
            {
                PlayerInventoryCapacity = InventoryController.PlayerInventoryCapacity
            };
        }
    }
}
