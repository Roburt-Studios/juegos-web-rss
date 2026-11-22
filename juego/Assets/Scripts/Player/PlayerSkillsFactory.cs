using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillsFactory : MonoBehaviour
{
    private static readonly Dictionary<Profile, PlayerSkillsData> cache = new();

    public static PlayerSkillsData GetData(Profile characterClass)
    {
        var key = characterClass;

        if (cache.ContainsKey(key)) return cache[key];

        PlayerSkillsData data = CreateSkills(characterClass);
        cache.Add(key, data);

        return data;
    }

    private static PlayerSkillsData CreateSkills(Profile characterClass)
    {
        switch (characterClass)
        {
            case (Profile.Strength):
                return new PlayerSkillsData(
                    listOfSkills: new List<PlayerSkillsData.Skill> {

                        new PlayerSkillsData.Skill(
                            skillName: "Stab",
                            skillDescription: "Cuts apart the flesh of one foe, deals great damage with low chance of critic",
                            attackPower: 2f,
                            critic: 25,
                            manaCost: 30
                        ),

                        new PlayerSkillsData.Skill(
                            skillName: "Tear",
                            skillDescription: "Tears apart the body of one foe, deals medium damage and medium chance of critic",
                            attackPower: 1.3f,
                            critic: 50,
                            manaCost: 20
                        ),

                        new PlayerSkillsData.Skill(
                            skillName: "Crush",
                            skillDescription: "Crushes the bones of one foe, deals high damage and low chance of critic",
                            attackPower: 1.8f,
                            critic: 30,
                            manaCost: 25)
                    });

            case (Profile.Defense):
                return new PlayerSkillsData(
                    listOfSkills: new List<PlayerSkillsData.Skill> {
                        new PlayerSkillsData.Skill(
                            skillName: "Stomp",
                            skillDescription: "Destroys the limps of one foe, deals low damage and medium chance of critic",
                            attackPower: 1.2f,
                            critic: 30,
                            manaCost: 20),

                        new PlayerSkillsData.Skill(
                            skillName: "Pound",
                            skillDescription: "Pulverizes one foe, deals medium damage and low chance of critic",
                            attackPower: 1.4f,
                            critic: 15,
                            manaCost: 25),

                        new PlayerSkillsData.Skill(
                            skillName: "Charge",
                            skillDescription: "Acts like a bull and attack one foe, deals medium damage and low chance of critic",
                            attackPower: 1.6f,
                            critic: 10,
                            manaCost: 30)
                    });

            case (Profile.Intelligence):
                return new PlayerSkillsData(
                    listOfSkills: new List<PlayerSkillsData.Skill> {
                        new PlayerSkillsData.Skill(
                            skillName: "Puncture",
                            skillDescription: "Punctures through weak parts of one foe, deals low damage and high chance of critic",
                            attackPower: 1.1f,
                            critic: 70,
                            manaCost: 20),

                        new PlayerSkillsData.Skill(
                            skillName: "Pierce",
                            skillDescription: "Pierces through the body of one foe, deals low damage and moderate chance of critic",
                            attackPower: 1.2f,
                            critic: 60,
                            manaCost: 25),

                        new PlayerSkillsData.Skill(
                            skillName: "Shoot",
                            skillDescription: "Shoots at one foe, deals medium damage and medium chance of critic",
                            attackPower: 1.4f,
                            critic: 45,
                            manaCost: 30)
                    });
            default:
                throw new System.ArgumentOutOfRangeException();
        }
    }
}
