using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillsFactory : MonoBehaviour
{
    private static readonly Dictionary<Profile, PlayerSkillsData> cache = new();

    public static PlayerSkillsData GetData(Profile characterClass)
    {
        var key = characterClass;

        if (cache.ContainsKey(key))
            return cache[key];

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
                    listOfSkills: new List<PlayerSkillsData.Skill>
                    {
                        new PlayerSkillsData.Skill(
                            skillName: "Puñalada",
                            skillDescription: "Corta la carne de un enemigo. Inflige mucho daño y tiene pocas probabilidades de causar un golpe crítico.",
                            attackPower: 2f,
                            critic: 25,
                            manaCost: 30
                        ),
                        new PlayerSkillsData.Skill(
                            skillName: "Desgarro",
                            skillDescription: "Desgarra el cuerpo de un enemigo. Inflige daño medio y tiene una probabilidad media de causar un golpe crítico.",
                            attackPower: 1.3f,
                            critic: 50,
                            manaCost: 20
                        ),
                        new PlayerSkillsData.Skill(
                            skillName: "Aplastamiento",
                            skillDescription: "Aplasta los huesos de un enemigo. Inflige mucho daño y tiene pocas probabilidades de causar un golpe crítico.",
                            attackPower: 1.8f,
                            critic: 30,
                            manaCost: 25
                        ),
                    }
                );

            case (Profile.Defense):
                return new PlayerSkillsData(
                    listOfSkills: new List<PlayerSkillsData.Skill>
                    {
                        new PlayerSkillsData.Skill(
                            skillName: "Pisotón",
                            skillDescription: "Destroza las extremidades de un enemigo. Inflige poco daño y tiene una probabilidad media de causar un golpe crítico.",
                            attackPower: 1.2f,
                            critic: 30,
                            manaCost: 20
                        ),
                        new PlayerSkillsData.Skill(
                            skillName: "Machacar",
                            skillDescription: "Pulveriza a un enemigo. Inflige daño medio y tiene pocas probabilidades de causar un golpe crítico.",
                            attackPower: 1.4f,
                            critic: 15,
                            manaCost: 25
                        ),
                        new PlayerSkillsData.Skill(
                            skillName: "Embestida",
                            skillDescription: "Embiste a un enemigo como un toro. Inflige daño medio y tiene pocas probabilidades de causar un golpe crítico.",
                            attackPower: 1.6f,
                            critic: 10,
                            manaCost: 30
                        ),
                    }
                );

            case (Profile.Intelligence):
                return new PlayerSkillsData(
                    listOfSkills: new List<PlayerSkillsData.Skill>
                    {
                        new PlayerSkillsData.Skill(
                            skillName: "Punción",
                            skillDescription: "Ataca los puntos débiles de un enemigo. Inflige poco daño y tiene muchas probabilidades de causar un golpe crítico.",
                            attackPower: 1.1f,
                            critic: 70,
                            manaCost: 20
                        ),
                        new PlayerSkillsData.Skill(
                            skillName: "Perforación",
                            skillDescription: "Perfora el cuerpo de un enemigo. Inflige poco daño y tiene una probabilidad moderada de causar un golpe crítico.",
                            attackPower: 1.2f,
                            critic: 60,
                            manaCost: 25
                        ),
                        new PlayerSkillsData.Skill(
                            skillName: "Disparo",
                            skillDescription: "Dispara a un enemigo. Inflige daño medio y tiene una probabilidad media de causar un golpe crítico.",
                            attackPower: 1.4f,
                            critic: 45,
                            manaCost: 30
                        ),
                    }
                );
            default:
                throw new System.ArgumentOutOfRangeException();
        }
    }
}
