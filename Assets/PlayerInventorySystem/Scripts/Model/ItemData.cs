using UnityEngine;

namespace PlayerInventorySystem
{

    /// <summary>
    /// Class to define an item's stats in the Item catalog
    /// 
    /// Becarful as changing these values will affect all items in the catalog and may cause data loss!
    /// Unity or Visual Studio do not pass refacrtoring changes to the contents of the ItemCatalog.asset file.
    /// You will need to update the contents of Assets/PlayerInventorySystem/ItemCatalog.asset manualy to reflect the change.
    /// </summary>
    [System.Serializable]
    public class ItemData
    {
        /// <summary>
        /// The human readable name of this object
        /// </summary>
        [Tooltip("The human readable name of this object")]
        public string name;

        /// <summary>
        /// The catalog ID of this item
        /// </summary>
        [Tooltip("The catalog ID of this item")]
        public int id;

        [Tooltip("Indicates if this item will be consumed/used when picked up. Item will not be placed in inventory. Use this for things like food or health packs that automaticaly aply when picked up.")]
        public bool consumeOnPickup = false;

        /// <summary>
        /// A brife description of this item
        /// </summary>
        [Tooltip("A brife description of this item. This will be displayed when the player hovers the mouse over slot with an item in it.")]
        public string description = "";

        /// <summary>
        /// The inventory sprite used to display this item in slots
        /// </summary>
        [Tooltip("The inventory sprite used to display this item in slots")]
        [PreviewSprite]
        public Sprite sprite;

        /// <summary>
        /// The prefab that will represent a single instance of the item in your game world.
        /// </summary>
        [Tooltip("The prefab that will represent a single instance of the item dropped in the game world.")]
        public GameObject worldPrefabSingle;

        /// <summary>
        /// The prefab the will represent a stack of the item in your game world.
        /// </summary>
        [Tooltip("The prefab the will represent a stack of the item dropped in the game world.")]
        public GameObject worldPrefabMultiple;

        /// <summary>
        /// The game object that represents this item when its equiped on the player
        /// </summary>
        [Tooltip("The game object that represents this item when its equiped on the player or placed in the game world.")]
        public GameObject worldPrefab;

        /// <summary>
        /// Defines the items type.
        /// Consumable, Usable, Wearable or Quest
        /// </summary>
        [Tooltip("Defines the items type. Consumable, Usable, Wearable or Quest")]
        public ITEMTYPE itemType;

        /// <summary>
        /// Defines the type of slot this item requires
        /// </summary>
        [Tooltip("Defines the type of slot this item requires")]
        public SLOTTYPE slotType;

        /// <summary>
        /// The max number of this item that can be stacked in one slot
        /// </summary>
        [Tooltip("The max number of this item that can be stacked in one slot")]
        public int maxStackSize = 5; // default and min = 1

        /// <summary>
        /// The recipe to craft this item
        /// </summary>
        [Tooltip("The recipe to craft this item")]
        public Recipe recipe = new Recipe();

        /// <summary>
        /// The number of this item that the crafting recipe will yeild
        /// </summary>
        [Tooltip("The number of this item that the crafting recipe will yeild")]
        public int craftCount = 1; // the number of items that are crafted with the recipe

        /// <summary>
        /// The durability of this item when new
        /// </summary>
        [Tooltip("The durability of this item when new")]
        public float maxDurability = 0;

        /// <summary>
        /// The damage bonus this item will bestow upon player when equiped 
        /// </summary>
        [Tooltip("The damage bonus this item will bestow upon player when equiped")]
        public float damage;

        /// <summary>
        /// The speed bonus this item will bestow upon player when equiped 
        /// </summary>
        [Tooltip("The speed bonus this item will bestow upon player when equiped")]
        public float speed;

        /// <summary>
        /// The health bonus this item will bestow upon player when equiped 
        /// </summary>
        [Tooltip("The health bonus this item will bestow upon player when equiped")]
        public float health;

        /// <summary>
        /// The stamina bonus this item will bestow upon player when equiped 
        /// </summary>
        [Tooltip("The stamina bonus this item will bestow upon player when equiped")]
        public float stamina;

        /// <summary>
        /// The mana bonus this item will bestow upon player when equiped 
        /// </summary>
        [Tooltip("The mana bonus this item will bestow upon player when equiped")]
        public float mana;

        /// <summary>
        /// The armor bonus this item will bestow upon player when equiped 
        /// </summary>
        [Tooltip("The armor bonus this item will bestow upon player when equiped")]
        public float armor;

        /// <summary>
        /// The intelligence bonus this item will bestow upon player when equiped 
        /// </summary>
        [Tooltip("The intelligence bonus this item will bestow upon player when equiped")]
        public float intelligence;

        /// <summary>
        /// The dexterity bonus this item will bestow upon player when equiped 
        /// </summary>
        [Tooltip("The dexterity bonus this item will bestow upon player when equiped")]
        public float dexterity;

        /// <summary>
        /// The luck bonus this item will bestow upon player when equiped 
        /// </summary>
        [Tooltip("The luck bonus this item will bestow upon player when equiped")]
        public float luck;
    }

    /// <summary>
    /// Helper class for PreviewSpriteDrawer
    /// </summary>
    public class PreviewSpriteAttribute : PropertyAttribute
    {
        public PreviewSpriteAttribute() { }
    }
}