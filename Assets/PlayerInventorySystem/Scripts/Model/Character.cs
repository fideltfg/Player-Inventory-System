using PlayerInventorySystem;
using System;
using UnityEngine;
using System.Diagnostics;

namespace PlayerInventorySystem
{
    /// <summary>
    /// Struct that outlines the base character stats
    /// </summary>
    [Serializable]
    public struct Character
    {
        public string characterName;
        public int ID;
        public GENDER GENDER;
        public int Level;
        public int Experience;
        public float Health;
        public float Mana;
        public float Stamina;
        public float Strength;
        public float Dexterity;
        public float IQ;
        public float Armor;
        public float Speed;
        public float Luck;

        // damage is calculated using strength, speed, dexterity, intelligence, and luck
        public float Damage
        {
            get
            {
                return (Strength + IQ + Dexterity + Speed) * Luck + 1;
            }
        }
    }
}