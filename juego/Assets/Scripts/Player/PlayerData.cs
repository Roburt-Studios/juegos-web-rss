using UnityEngine;
public enum Profile
{
    Strength,
    Defense,
    Intelligence,
    None
}
public class PlayerData
{
    public Profile CharacterClass {  get; private set; }

    public int InitialHealth { get; private set; }
    public int InitialMana { get; private set; }

    public int Strength { get; private set; }

    public int Defense { get; private set; }

    public int SkillPower { get; private set; }

    public int Charisma { get; private set; }

    public PlayerData(
        Profile characterClass,
        int initialHealth,
        int initialMana,
        int strength,
        int charisma,
        int defense,
        int skillPower
        )
    {
        CharacterClass = characterClass;
        InitialHealth = initialHealth;
        InitialMana = initialMana;
        Strength = strength;
        Charisma = charisma;
        Defense = defense;
        SkillPower = skillPower;
    }
}
