using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using PlayerInventorySystem;
using System;

namespace PlayerInventorySystem
{
    public class CharacterPanel : InventorySystemPanel
    {
        // Dictionary to store buff values for each stat
        internal Dictionary<string, float> buffValues = new Dictionary<string, float>();

        // UI slot controllers for different equipment slots
        public SlotController HeadSlot, LeftHandSlot, RightHandSlot, BodySlot, LegsSlot, FeetSlot;

        // UI text elements for displaying stats
        public Text HealthText, StaminaText, DexterityText, ArmorText, ManaText, IQText, SpeedText, LuckText, StrengthText, DamageText;

        // Base stats
        private float health = 0;
        private float stamina = 0;
        private float strength = 0;
        private float dexterity = 0;
        private float armor = 0;
        private float mana = 0;
        private float IQ = 0;
        private float speed = 0;
        private float luck = 0;

        // Property to calculate damage based on buffs
        private float damage
        {
            get
            {
                return 1 + ((buffValues["Stamina"] * buffValues["Speed"]) + (buffValues["Dexterity"] * buffValues["IQ"])) * buffValues["Luck"];
            }
        }

        // Method to update stats and UI
        public void UpdateStats(Slot slot)
        {

            ResetBuffValues();

            // Update buff values based on equipped items
            foreach (SlotController slotController in SlotList)
            {
                if (slotController.Slot.Item != null)
                {
                    UpdateBuffValues(slotController.Slot.Item.Data);
                }
            }

            // Get base stats from the character
            health = InventoryController.Character.Health + buffValues["Health"];
            mana = InventoryController.Character.Mana + buffValues["Mana"];
            stamina = InventoryController.Character.Stamina + buffValues["Stamina"];
            strength = InventoryController.Character.Strength + buffValues["Strength"];
            dexterity = InventoryController.Character.Dexterity + buffValues["Dexterity"];
            IQ = InventoryController.Character.IQ + buffValues["IQ"];
            armor = InventoryController.Character.Armor + buffValues["Armor"];
            speed = InventoryController.Character.Speed + buffValues["Speed"];
            luck = InventoryController.Character.Luck + buffValues["Luck"];

            // Update UI text for each stat
            UpdateUIText(HealthText, "Health", health);
            UpdateUIText(ManaText, "Mana", mana);
            UpdateUIText(StaminaText, "Stamina", stamina);
            UpdateUIText(StrengthText, "Strength", strength);
            UpdateUIText(DexterityText, "Dexterity", dexterity);
            UpdateUIText(IQText, "IQ", IQ);
            UpdateUIText(ArmorText, "Armor", armor);
            UpdateUIText(SpeedText, "Speed", speed);
            UpdateUIText(LuckText, "Luck", luck);
            UpdateUIText(DamageText, "Damage", damage + InventoryController.Character.Damage);

            // Update UI color for each stat
            UpdateUIColor(HealthText, "Health");
            UpdateUIColor(ManaText, "Mana");
            UpdateUIColor(StaminaText, "Stamina");
            UpdateUIColor(StrengthText, "Strength");
            UpdateUIColor(DexterityText, "Dexterity");
            UpdateUIColor(IQText, "IQ");
            UpdateUIColor(ArmorText, "Armor");
            UpdateUIColor(SpeedText, "Speed");
            UpdateUIColor(LuckText, "Luck");
            UpdateUIColor(DamageText, "Damage");

            // Invoke callback for character item change
            InventoryController.OnCharacterItemChangeCallBack?.Invoke();
        }

        // Reset all buff values to zero
        private void ResetBuffValues()
        {
            buffValues = new Dictionary<string, float>
            {
                { "Health", 0 },
                { "Stamina", 0 },
                { "Strength", 0 },
                { "Dexterity", 0 },
                { "Mana", 0 },
                { "IQ", 0 },
                { "Speed", 0 },
                { "Armor", 0 },
                { "Luck", 0 },
                { "Damage", 0 }
            };
        }

        // Update buff values based on item data
        private void UpdateBuffValues(ItemData itemData)
        {
            buffValues["Health"] += itemData.health;
            buffValues["Mana"] += itemData.mana;
            buffValues["Stamina"] += itemData.stamina;
            buffValues["Strength"] += itemData.strength;
            buffValues["Dexterity"] += itemData.dexterity;
            buffValues["IQ"] += itemData.IQ;
            buffValues["Speed"] += itemData.speed;
            buffValues["Luck"] += itemData.Luck;
            buffValues["Armor"] += itemData.armor;
        }

        // Update UI text for a given stat
        private void UpdateUIText(Text uiText, string statName, float baseValue)
        {
            float result = baseValue + buffValues[statName];
            uiText.text = result.ToString(result < 1 ? "0.##" : "###.##");
        }

        // Update UI color based on buff value
        private void UpdateUIColor(Text uiText, string statName)
        {
            if (buffValues[statName] > 0)
            {
                uiText.color = Color.green;
            }
            else if (buffValues[statName] < 0)
            {
                uiText.color = Color.red;
            }
            else
            {
                uiText.color = Color.white;
            }
        }

        // Build method to initialize slots and stats
        public override void Build(int InventoryIndex)
        {
            base.Build(InventoryIndex);
            ResetBuffValues();

            // Initialize buff values
            InitializeBuffValue("Health");
            InitializeBuffValue("Stamina");
            InitializeBuffValue("Dexterity");
            InitializeBuffValue("Strength");
            InitializeBuffValue("Armor");
            InitializeBuffValue("Mana");
            InitializeBuffValue("IQ");
            InitializeBuffValue("Speed");
            InitializeBuffValue("Luck");
            InitializeBuffValue("Damage");

            // Initialize equipment slots
            InitializeSlot(HeadSlot, 0, SLOTTYPE.HEAD);
            InitializeSlot(LeftHandSlot, 1, SLOTTYPE.LEFTHAND);
            InitializeSlot(RightHandSlot, 2, SLOTTYPE.RIGHTHAND);
            InitializeSlot(BodySlot, 3, SLOTTYPE.BODY);
            InitializeSlot(LegsSlot, 4, SLOTTYPE.LEGS);
            InitializeSlot(FeetSlot, 5, SLOTTYPE.FEET);

            SlotList.AddRange(new SlotController[] { HeadSlot, LeftHandSlot, RightHandSlot, BodySlot, LegsSlot, FeetSlot });
            UpdateStats(null);
        }

        // Initialize a specific buff value
        private void InitializeBuffValue(string statName)
        {
            if (!buffValues.ContainsKey(statName))
            {
                buffValues.Add(statName, 0);
            }
        }

        // Initialize a specific slot with given parameters
        private void InitializeSlot(SlotController slotController, int index, SLOTTYPE slotType)
        {
            slotController.Index = this.Index;
            slotController.SetSlot(InventoryController.CharacterInventory[index]);
            slotController.Slot.SlotType = slotType;
            slotController.slotLocation = SLOTLOCATION.CHARACTER;
            slotController.Slot.RegisterSlotChangedCallback(UpdateStats);
        }
    }
}
