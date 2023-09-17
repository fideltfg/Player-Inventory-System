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
        float health = 0;
        float stamina = 0;
        float dexterity = 0;
        float armor = 0;
        float mana = 0;
        float intelligence = 0;
        float speed = 0;
        float luck = 0;

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

        public Dictionary<string, float> buffValues = new();

        /// <summary>
        /// method to update both the UI stats and the buffValues list
        /// </summary>
        /// <param name="slot">Can be null required only because this is used as a call backon the slots to update the stats</param>
        public void UpdateStats(Slot slot)
        {
            // get default values
            health = InventoryController.Character.Health;
            mana = InventoryController.Character.Mana;
            stamina = InventoryController.Character.Stamina;
            dexterity = InventoryController.Character.Dexterity;
            intelligence = InventoryController.Character.Intelligence;
            armor = InventoryController.Character.Armor;
            speed = InventoryController.Character.Speed;
            luck = InventoryController.Character.Luck;

            // update default buffs values
            foreach (SlotController SlotController in SlotList)
            {
                if (SlotController.Slot.Item != null)
                {
                    buffValues["Health"] += SlotController.Slot.Item.Data.health;
                    buffValues["Mana"] += SlotController.Slot.Item.Data.mana;
                    buffValues["Stamina"] += SlotController.Slot.Item.Data.stamina;
                    buffValues["Dexterity"] += SlotController.Slot.Item.Data.dexterity;
                    buffValues["Intelligance"] += SlotController.Slot.Item.Data.intelligence;
                    buffValues["Armor"] += SlotController.Slot.Item.Data.armor;
                    buffValues["Speed"] += SlotController.Slot.Item.Data.speed;
                    buffValues["Luck"] += SlotController.Slot.Item.Data.luck;
                }
            }

            // set UI values
            HealthText.text = "+" + buffValues["Health"].ToString() + " " +  health.ToString();
            ManaText.text = mana + buffValues["Mana"].ToString();
            StaminaText.text = (stamina + buffValues["Stamina"]).ToString();
            DexterityText.text = (dexterity + buffValues["Dexterity"]).ToString();
            IntelligenceText.text = (intelligence + buffValues["Intelligance"]).ToString();
            ArmorText.text = (armor + buffValues["Armor"]).ToString();
            SpeedText.text = (speed + buffValues["Speed"]).ToString();
            LuckText.text = (luck + buffValues["Luck"]).ToString();

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
                buffValues.Add("Health", 0);
            }

            // check if the stamina value is already in the buffValues dict if not add it
            if (!buffValues.ContainsKey("Stamina"))
            {
                buffValues.Add("Stamina", 0);
            }

            // check if the dexterity value is already in the buffValues dict if not add it
            if (!buffValues.ContainsKey("Dexterity"))
            {
                buffValues.Add("Dexterity", 0);
            }

            // check if the armor value is already in the buffValues dict if not add it
            if (!buffValues.ContainsKey("Armor"))
            {
                buffValues.Add("Armor", 0);
            }

            // check if the mana value is already in the buffValues dict if not add it
            if (!buffValues.ContainsKey("Mana"))
            {
                buffValues.Add("Mana", 0);
            }

            // check if the intelligence value is already in the buffValues dict if not add it
            if (!buffValues.ContainsKey("Intelligance"))
            {
                buffValues.Add("Intelligance", 0);
            }

            // check if the speed value is already in the buffValues dict if not add it
            if (!buffValues.ContainsKey("Speed"))
            {
                buffValues.Add("Speed", 0);
            }

            // check if the luck value is already in the buffValues dict if not add it
            if (!buffValues.ContainsKey("Luck"))
            {
                buffValues.Add("Luck", 0);
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