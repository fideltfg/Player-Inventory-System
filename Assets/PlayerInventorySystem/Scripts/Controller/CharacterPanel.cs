using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using PlayerInventorySystem;
using System;

namespace PlayerInventorySystem
{
    public class CharacterPanel : InventorySystemPanel
    {
        internal Dictionary<string, float> buffValues = new Dictionary<string, float>();

        public SlotController HeadSlot, LeftHandSlot, RightHandSlot, BodySlot, LegsSlot, FeetSlot;
        public Text HealthText, StaminaText, DexterityText, ArmorText, ManaText, IQText, SpeedText, LuckText, StrengthText, DamageText;

        private float health = 0;
        private float stamina = 0;
        private float strength = 0;
        private float dexterity = 0;
        private float armor = 0;
        private float mana = 0;
        private float IQ = 0;
        private float speed = 0;
        private float luck = 0;
        
        private float damage
        {

            get
            {
                return 1 + ((buffValues["Stamina"] * buffValues["Speed"]) + (buffValues["Dexterity"] * buffValues["IQ"])) * buffValues["Luck"];
            }
        }



        public void UpdateStats(Slot slot)
        {

            health = InventoryController.Character.Health;
            mana = InventoryController.Character.Mana;
            stamina = InventoryController.Character.Stamina;
            strength = InventoryController.Character.Strength;
            dexterity = InventoryController.Character.Dexterity;
            IQ = InventoryController.Character.IQ;
            armor = InventoryController.Character.Armor;
            speed = InventoryController.Character.Speed;
            luck = InventoryController.Character.Luck;
            ResetBuffValues();

            foreach (SlotController SlotController in SlotList)
            {
                if (SlotController.Slot.Item != null)
                {
                    UpdateBuffValues(SlotController.Slot.Item.Data);
                }
            }

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

            InventoryController.Instance.OnCharacterItemChangeCallBack?.Invoke();
        }

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

        private void UpdateBuffValues(ItemData itemData)
        {
            buffValues["Health"] += itemData.health;
            buffValues["Mana"] += itemData.mana;
            buffValues["Stamina"] += itemData.stamina;
            buffValues["Strength"] += itemData.Strength;
            buffValues["Dexterity"] += itemData.Dexterity;
            buffValues["IQ"] += itemData.IQ;
            buffValues["Speed"] += itemData.Speed;
            buffValues["Luck"] += itemData.Luck;
            buffValues["Armor"] += itemData.armor;
        }

        private void UpdateUIText(Text uiText, string statName, float baseValue)
        {
            float result = baseValue + buffValues[statName];
            uiText.text = result.ToString(result < 1 ? "0.##" : "###.##");

            // uiText.text = (baseValue + buffValues[statName]).ToString(result < 1 ? "0.##" : "###.##");
        }

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

        public override void Build(int InventoryIndex)
        {
            base.Build(InventoryIndex);
            ResetBuffValues();

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

            InitializeSlot(HeadSlot, 0, SLOTTYPE.HEAD);
            InitializeSlot(LeftHandSlot, 1, SLOTTYPE.HANDS);
            InitializeSlot(RightHandSlot, 2, SLOTTYPE.HANDS);
            InitializeSlot(BodySlot, 3, SLOTTYPE.BODY);
            InitializeSlot(LegsSlot, 4, SLOTTYPE.LEGS);
            InitializeSlot(FeetSlot, 5, SLOTTYPE.FEET);

            SlotList.AddRange(new SlotController[] { HeadSlot, LeftHandSlot, RightHandSlot, BodySlot, LegsSlot, FeetSlot });
            UpdateStats(null);
        }

        private void InitializeBuffValue(string statName)
        {
            if (!buffValues.ContainsKey(statName))
            {
                buffValues.Add(statName, 0);
            }
        }

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