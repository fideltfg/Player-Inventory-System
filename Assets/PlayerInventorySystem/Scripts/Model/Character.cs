using PlayerInventorySystem;
using System;

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
    public float Dexterity;
    public float Intelligence;
    public float Armor;
    public float Speed;
    public float Luck;
}
