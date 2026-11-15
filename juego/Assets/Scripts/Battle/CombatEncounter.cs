using System;
using UnityEngine;

// Cada NPC puede iniciar un encuentro distinto. Un encuentro también puede incluir varios enemigos.
public class CombatEncounter : MonoBehaviour
{
    [Serializable]
    public class Opponent
    {
        public GameObject worldObject;
        public string unitName = "Enemigo";
        [Min(1)] public int maxHP = 40;
        [Min(0)] public int defense;
        [Min(0)] public int attackPower = 10;
        [Min(0)] public int skillPower = 15;
    }

    [SerializeField] private Opponent[] opponents = { new Opponent() };
    private bool defeated;

    public Opponent[] Opponents => opponents;
    public bool IsAvailable => !defeated && isActiveAndEnabled;

    public void CompleteVictory()
    {
        if (defeated) return;
        defeated = true;

        foreach (Opponent opponent in opponents)
        {
            if (opponent != null && opponent.worldObject != null)
                opponent.worldObject.SetActive(false);
        }

        // El NPC que inició el encuentro también desaparece si no está en la lista.
        gameObject.SetActive(false);
    }
}
