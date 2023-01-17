using UnityEngine;

namespace PlayerInventorySystem
{

    /// <summary>
    /// Class to define an item's stats in the Item catalog
    /// </summary>
    [System.Serializable]
    public class ItemData
    {
        /// <summary>
        /// The human readable name of this object
        /// </summary>
        public string name;

        /// <summary>
        /// The catalog ID of this item
        /// </summary>
        public int id;

        /// <summary>
        /// A brife description of this item
        /// </summary>
        public string description = "";

        /// <summary>
        /// The inventory sprite used to display this item in slots
        /// </summary>
        [PreviewSprite]
        public Sprite sprite;

        /// <summary>
        /// The prefab that will represent a single instance of the item in your game world.
        /// </summary>
        public GameObject worldPrefabSingle;

        /// <summary>
        /// The prefab the will represent a stack of the item in your game world.
        /// </summary>
        public GameObject worldPrefabMultiple; 

        /// <summary>
        /// The gmae object that represents this item when its equiped on the player
        /// </summary>
        public GameObject worldPrefab;  

        /// <summary>
        /// Defines the items type.
        /// Consumable, Usable, Wearable or Quest
        /// </summary>
        public ITEMTYPE itemType;

        /// <summary>
        /// Defines the type of slot this item requires
        /// </summary>
        public SLOTTYPE slotType;

        /// <summary>
        /// Teh max number of this item that can be stacked in one slot
        /// </summary>
        public int maxStackSize = 5; // default and min = 1

        /// <summary>
        /// The recipe to craft this item
        /// </summary>
        public Recipe recipe = new Recipe();

        /// <summary>
        /// The number of this item that the crafting recipr will yeild
        /// </summary>
        public int craftCount = 1; // the number of items that are crafted with the recipe

        /// <summary>
        /// The durability of this item when new
        /// </summary>
        public float maxDurability = 0;

        /// <summary>
        /// The damage bonus this item will bestow upon player when equiped 
        /// </summary>
        public float damage;

        /// <summary>
        /// The speed bonus this item will bestow upon player when equiped 
        /// </summary>
        public float speed;
        /// <summary>
        /// The health bonus this item will bestow upon player when equiped 
        /// </summary>
        public float health;

        /// <summary>
        /// The stamina bonus this item will bestow upon player when equiped 
        /// </summary>
        public float stamina;

        /// <summary>
        /// The mana bonus this item will bestow upon player when equiped 
        /// </summary>
        public float mana;

        /// <summary>
        /// The armor bonus this item will bestow upon player when equiped 
        /// </summary>
        public float armor;

        /// <summary>
        /// The intelligence bonus this item will bestow upon player when equiped 
        /// </summary>
        public float intelligence;

        /// <summary>
        /// The dexterity bonus this item will bestow upon player when equiped 
        /// </summary>
        public float dexterity;

        /// <summary>
        /// The luck bonus this item will bestow upon player when equiped 
        /// </summary>
        public float luck;
    }

    /// <summary>
    /// Helper class for PreviewSpriteDrawer
    /// </summary>
    public class PreviewSpriteAttribute : PropertyAttribute
    {
        public PreviewSpriteAttribute () { }
    }
}