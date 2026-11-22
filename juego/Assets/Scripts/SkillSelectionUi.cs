using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SkillSelectionUI : MonoBehaviour
{
    [SerializeField] private Button[] skillButtons;
    [SerializeField] private TMP_Text[] skillTexts;

    private Profile debugProfile = Profile.Strength;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Profile profile = PlayerClassManager.instance != null ? PlayerClassManager.instance.characterClass : debugProfile;

        List<PlayerSkillsData.Skill> skills = PlayerSkillsFactory.GetData(profile).ListOfSkills;

        for (int i = 0; i < skillButtons.Length; i++)
        {
            bool hasSkill = i < skills.Count;
            skillButtons[i].gameObject.SetActive(hasSkill);

            skillTexts[i].gameObject.SetActive(hasSkill);

            if (!hasSkill) continue;

            TextMeshProUGUI label = skillButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if(label != null) label.text = skills[i].SkillName;

            skillTexts[i].text = skills[i].SkillDescription + " (" + skills[i].ManaCost + " MP)";

            int idx = i;

            skillButtons[i].onClick.AddListener(() => ChooseSkill(idx));
        }
    }

    void ChooseSkill(int idx)
    {
        if(PlayerClassManager.instance != null)
        {
            PlayerClassManager.instance.selectedSkill = idx;
            SceneManager.LoadScene(7);
        }
    }


}
