using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BattleUnit))]
public class PlayerSetup : MonoBehaviour
{
    Profile debugProfile = Profile.Strength;
    int debugSkill = 0;

    void Awake()
    {
        Profile profile = PlayerClassManager.instance != null ? PlayerClassManager.instance.characterClass : debugProfile;

        int idx = PlayerClassManager.instance != null ? PlayerClassManager.instance.selectedSkill : debugSkill;
        
        PlayerData data = PlayerDataFactory.GetData(profile);

        GetComponent<BattleUnit>().Initialize
            (
            name: data.CharacterClass.ToString(),
            hp: data.InitialHealth,
            damage: data.Strength,
            skill: data.SkillPower
            );

        List<PlayerSkillsData.Skill> skills = PlayerSkillsFactory.GetData(profile).ListOfSkills;

        if (idx >= 0 && idx < skills.Count) GetComponent<BattleUnit>().SetSkill(skills[idx]);
    }
}
