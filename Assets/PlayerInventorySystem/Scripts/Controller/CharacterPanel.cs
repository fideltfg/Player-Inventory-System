using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
namespace PlayerInventorySystem
{
    /// <summary>
    /// Controller for Character Panel
    /// </summary>
    public class CharacterPanel : InventorySystemPanel
    {
        /// <summary>
        /// The starting health of your character
        /// </summary>
        public float baseHealth = 0;

        /// <summary>
        /// The starting stamina of your character
        /// </summary>
        public float baseStamina = 0;

        /// <summary>
        /// The starting dexterity of your character
        /// </summary>
        public float baseDexterity = 0;

        /// <summary>
        /// The starting armor of your character
        /// </summary>
        public float baseArmor = 0;

        /// <summary>
        /// The starting mana of your character
        /// </summary>
        public float baseMana = 0;

        /// <summary>
        /// The starting intelligence of your character
        /// </summary>
        public float baseIntelligence = 0;

        /// <summary>
        /// The starting speed of your character
        /// </summary>
        public float baseSpeed = 0;

        /// <summary>
        /// The starting lucj of your character
        /// </summary>
        public float baseLuck = 0;

        public SlotController HeadSlot;
        public SlotController LeftHandSlot;
        public SlotController RightHandSlot;
        public SlotController BodySlot;
        public SlotController LegsSlot;
        public SlotController FeetSlot;

        public Text HealthText;
        public Text StaminaText;
        public Text DexterityText;
        public Text ArmorText;
        public Text ManaText;
        public Text IntelligenceText;
        public Text SpeedText;
        public Text LuckText;

        public Dictionary<string, float> buffValues = new Dictionary<string, float>();

        /// <summary>
        /// method to update both the UI stats and the buffValues list
        /// </summary>
        /// <param name="slot">Can be null required only because this is used as a call backon the slots to update the stats</param>
        public void UpdateStats(Slot slot)
        {
            // get default values
            float health = baseHealth;
            float stamina = baseStamina;
            float dexterity = baseDexterity;
            float armor = baseArmor;
            float mana = baseMana;
            float intelligence = baseIntelligence;
            float speed = baseSpeed;
            float luck = baseLuck;

            // update default buffs from items
            foreach (SlotController SlotController in SlotList)
            {
                if (SlotController.Slot.Item != null)
                {
                    health += SlotController.Slot.Item.Data.health;
                    stamina += SlotController.Slot.Item.Data.stamina;
                    dexterity += SlotController.Slot.Item.Data.dexterity;
                    armor += SlotController.Slot.Item.Data.armor;
                    mana += SlotController.Slot.Item.Data.mana;
                    intelligence += SlotController.Slot.Item.Data.intelligence;
                    speed += SlotController.Slot.Item.Data.speed;
                    luck += SlotController.Slot.Item.Data.luck;
                }
            }

            // set UI values
            HealthText.text = health.ToString();
            StaminaText.text = stamina.ToString();
            DexterityText.text = dexterity.ToString();
            ArmorText.text = armor.ToString();
            ManaText.text = mana.ToString();
            IntelligenceText.text = intelligence.ToString();
            SpeedText.text = speed.ToString();
            LuckText.text = luck.ToString();

            // save values to buffValues dict
            buffValues["Health"] = baseHealth;
            buffValues["Stamina"] = baseStamina;
            buffValues["Dexterity"] = baseDexterity;
            buffValues["Armor"] = baseArmor;
            buffValues["Mana"] = baseArmor;
            buffValues["Intelligance"] = baseIntelligence;
            buffValues["Speed"] = baseSpeed;
            buffValues["Luck"] = baseLuck;

            // invloke callback to let other scripts know the stats have changed
            InventoryController.Instance.OnCharacterItemChangeCallBack?.Invoke();


        }

        public override void Build(int InventoryIndex)
        {
            base.Build(InventoryIndex);

            Inventory CharacterInventory = InventoryController.CharacterInventory;

            // check if the health value is already in the buffValues dict if not add it
            if (!buffValues.ContainsKey("Health"))
            {
                buffValues.Add("Health", baseHealth);
            }

            // check if the stamina value is already in the buffValues dict if not add it
            if (!buffValues.ContainsKey("Stamina"))
            {
                buffValues.Add("Stamina", baseStamina);
            }

            // check if the dexterity value is already in the buffValues dict if not add it
            if (!buffValues.ContainsKey("Dexterity"))
            {
                buffValues.Add("Dexterity", baseDexterity);
            }

            // check if the armor value is already in the buffValues dict if not add it
            if (!buffValues.ContainsKey("Armor"))
            {
                buffValues.Add("Armor", baseArmor);
            }

            // check if the mana value is already in the buffValues dict if not add it
            if (!buffValues.ContainsKey("Mana"))
            {
                buffValues.Add("Mana", baseMana);
            }

            // check if the intelligence value is already in the buffValues dict if not add it
            if (!buffValues.ContainsKey("Intelligance"))
            {
                buffValues.Add("Intelligance", baseIntelligence);
            }

            // check if the speed value is already in the buffValues dict if not add it
            if (!buffValues.ContainsKey("Speed"))
            {
                buffValues.Add("Speed", baseSpeed);
            }

            // check if the luck value is already in the buffValues dict if not add it
            if (!buffValues.ContainsKey("Luck"))
            {
                buffValues.Add("Luck", baseLuck);
            }


            if (HeadSlot == null || LeftHandSlot == null || RightHandSlot == null || BodySlot == null || LegsSlot == null || FeetSlot == null)
            {
                Debug.LogError("One or more slots are not assigned to the character panel");
                return;
            }

            HeadSlot.Index = this.Index;
            HeadSlot.SetSlot(CharacterInventory[0]);
            HeadSlot.Slot.SlotType = SLOTTYPE.HEAD;
            HeadSlot.slotLocation = SLOTLOCATION.CHARACTER;
            HeadSlot.Slot.RegisterSlotChangedCallback(UpdateStats);

            LeftHandSlot.Index = this.Index;
            LeftHandSlot.SetSlot(CharacterInventory[1]);
            LeftHandSlot.Slot.SlotType = SLOTTYPE.HANDS;
            LeftHandSlot.slotLocation = SLOTLOCATION.CHARACTER;
            LeftHandSlot.Slot.RegisterSlotChangedCallback(UpdateStats);

            RightHandSlot.Index = this.Index;
            RightHandSlot.SetSlot(CharacterInventory[2]);
            RightHandSlot.Slot.SlotType = SLOTTYPE.HANDS;
            RightHandSlot.slotLocation = SLOTLOCATION.CHARACTER;
            RightHandSlot.Slot.RegisterSlotChangedCallback(UpdateStats);

            BodySlot.Index = this.Index;
            BodySlot.SetSlot(CharacterInventory[3]);
            BodySlot.Slot.SlotType = SLOTTYPE.BODY;
            BodySlot.slotLocation = SLOTLOCATION.CHARACTER;
            BodySlot.Slot.RegisterSlotChangedCallback(UpdateStats);

            LegsSlot.Index = this.Index;
            LegsSlot.SetSlot(CharacterInventory[4]);
            LegsSlot.Slot.SlotType = SLOTTYPE.LEGS;
            LegsSlot.slotLocation = SLOTLOCATION.CHARACTER;
            LegsSlot.Slot.RegisterSlotChangedCallback(UpdateStats);

            FeetSlot.Index = this.Index;
            FeetSlot.SetSlot(CharacterInventory[5]);
            FeetSlot.Slot.SlotType = SLOTTYPE.FEET;
            FeetSlot.slotLocation = SLOTLOCATION.CHARACTER;
            FeetSlot.Slot.RegisterSlotChangedCallback(UpdateStats);

            SlotList.Add(HeadSlot);
            SlotList.Add(LeftHandSlot);
            SlotList.Add(RightHandSlot);
            SlotList.Add(BodySlot);
            SlotList.Add(LegsSlot);
            SlotList.Add(FeetSlot);
            UpdateStats(null);
        }

    }
}