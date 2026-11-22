using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BattleUnit))]
public class PlayerSetup : MonoBehaviour
{

    private BattleUnit unit;
    Profile debugProfile = Profile.Strength;
    int debugSkill = 0;

    void Awake()
    {
        unit = GetComponent<BattleUnit>();

        Profile profile = PlayerClassManager.instance != null ? PlayerClassManager.instance.characterClass : debugProfile;

        int idx = PlayerClassManager.instance != null ? PlayerClassManager.instance.selectedSkill : debugSkill;
        
        PlayerData data = PlayerDataFactory.GetData(profile);

        GetComponent<BattleUnit>().Initialize
            (
            name: data.ClassDisplayName,
            hp: data.InitialHealth,
            mana: data.InitialMana,
            def: data.Defense,
            damage: data.Strength,
            skill: data.SkillPower
            );

        List<PlayerSkillsData.Skill> skills = PlayerSkillsFactory.GetData(profile).ListOfSkills;

        if (idx >= 0 && idx < skills.Count) unit.SetSkill(skills[idx]);

        if (PlayerClassManager.instance != null && PlayerClassManager.instance.savedHP > 0) unit.currentHP = Mathf.Min(PlayerClassManager.instance.savedHP, unit.maxHP);
        if (PlayerClassManager.instance != null && PlayerClassManager.instance.savedMP >= 0)
            unit.currentMP = Mathf.Min(PlayerClassManager.instance.savedMP, unit.maxMP);
    }

    private void OnDestroy()
    {
        if(PlayerClassManager.instance != null && unit != null)
        {
            PlayerClassManager.instance.savedHP = unit.currentHP;
            PlayerClassManager.instance.savedMP = unit.currentMP;
        }
    }
}
