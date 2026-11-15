using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillsData
{
    public struct Skill
    {
        public string SkillName { get; private set;  }
        public string SkillDescription { get; private set;  }
        public float AttackPower { get; private set;  }
        public int Critic { get; private set;  }

        public Skill(string skillName, string skillDescription, float attackPower, int critic)
        {
            SkillName = skillName; //Nombre de la habilidad
            SkillDescription = skillDescription;//Descripción de la habilidad
            AttackPower = attackPower; //Multiplicación al daño base
            Critic = critic; //Probabilidad de efectuar un impacto crítico
        }
    }

    public List<Skill> ListOfSkills { get; private set; }

    public PlayerSkillsData(
        List<Skill> listOfSkills
        )
    {
        ListOfSkills = listOfSkills;
    }
}
