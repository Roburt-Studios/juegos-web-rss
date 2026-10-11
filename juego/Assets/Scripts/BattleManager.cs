
using System.Collections;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [Header("Combatientes")]
    public BattleUnit[] party;
    public BattleUnit[] enemies;

    [Header("Interfaz")]
    public BattleUI battleUI;

    [Header("Dados")]
    [SerializeField] private DiceManager diceManager;

    private int partyIndex;
    private int targetIndex;

    private bool playerTurn;
    private bool battleEnded;
    private bool potionAvailable = true;

    private BattleUnit CurrentActor =>
        partyIndex >= 0 && partyIndex < party.Length
            ? party[partyIndex]
            : null;

    private BattleUnit CurrentTarget =>
        targetIndex >= 0 && targetIndex < enemies.Length
            ? enemies[targetIndex]
            : null;

    private void Start()
    {
        if (diceManager == null)
            diceManager = GetComponent<DiceManager>();

        if (battleUI == null || diceManager == null ||
            party == null || party.Length == 0 ||
            enemies == null || enemies.Length == 0)
        {
            Debug.LogError(
                "UI, dados, aliados y enemigos sin asignar"
            );
            enabled = false;
            return;
        }

        battleUI.Initialize(this);

        partyIndex = FindAlivePartyMember(0);
        targetIndex = FindAliveEnemy(0);

        if (partyIndex < 0 || targetIndex < 0)
        {
            EndBattle(partyIndex >= 0);
            return;
        }

        playerTurn = true;
        RefreshUI("¡Comienza el combate!");
    }

    public void Attack()
    {
        if (!CanPlayerAct()) return;

        BattleUnit actor = CurrentActor;
        BattleUnit target = CurrentTarget;

        string message = DealPlayerDamage(actor, target, actor.attackPower, "ataca a");

        AfterPlayerAction(message);
    }

    public void UseSkill()
    {
        if (!CanPlayerAct()) return;

        BattleUnit actor = CurrentActor;
        BattleUnit target = CurrentTarget;

        string message = DealPlayerDamage(actor, target, actor.skillPower, "usa una habilidad contra");

        AfterPlayerAction(message);
    }

    public void UsePotion()
    {
        if (!CanPlayerAct() || !potionAvailable) return;

        BattleUnit actor = CurrentActor;

        actor.Heal(35);
        potionAvailable = false;

        AfterPlayerAction(
            actor.unitName + " usa una poción y recupera vida."
        );
    }

    public void SelectNextTarget()
    {
        if (!playerTurn || battleEnded) return;

        int next = FindAliveEnemy(targetIndex + 1);

        if (next >= 0)
            targetIndex = next;

        RefreshUI("Selecciona tu acción.");
    }

    private bool CanPlayerAct()
    {
        return playerTurn &&
               !battleEnded &&
               CurrentActor != null &&
               CurrentActor.IsAlive &&
               CurrentTarget != null &&
               CurrentTarget.IsAlive;
    }

    private string DealPlayerDamage(
        BattleUnit actor,
        BattleUnit target,
        int baseDamage,
        string action)
    {
        DiceRoll roll = diceManager.Roll();
        int totalDamage = baseDamage + roll.Total;
        target.TakeDamage(totalDamage);

        return actor.unitName + " " + action + " " + target.unitName +
               " (2d6: " + roll.FirstDie + "+" + roll.SecondDie +
               ", +" + roll.Total + ", " + totalDamage + " de daño).";
    }

    private void AfterPlayerAction(string message)
    {
        if (AllDefeated(enemies))
        {
            EndBattle(true);
            return;
        }

        if (CurrentTarget == null || !CurrentTarget.IsAlive)
            targetIndex = FindAliveEnemy(targetIndex + 1);

        partyIndex = FindAlivePartyMember(partyIndex + 1);

        if (partyIndex >= 0)
        {
            playerTurn = true;
            RefreshUI(message);
        }
        else
        {
            playerTurn = false;
            RefreshUI(message);
            StartCoroutine(EnemyTurn());
        }
    }

    private IEnumerator EnemyTurn()
    {
        playerTurn = false;

        for (int i = 0; i < enemies.Length; i++)
        {
            BattleUnit enemy = enemies[i];

            if (enemy == null || !enemy.IsAlive)
                continue;

            if (AllDefeated(party))
            {
                EndBattle(false);
                yield break;
            }

            yield return new WaitForSeconds(0.5f);

            int aliveIndex = FindAlivePartyMember(0);

            if (aliveIndex < 0)
            {
                EndBattle(false);
                yield break;
            }

            BattleUnit target = party[aliveIndex];
            target.TakeDamage(enemy.attackPower);

            RefreshUI(
                enemy.unitName + " ataca a " +
                target.unitName + " (" +
                enemy.attackPower + " de daño)."
            );

            yield return new WaitForSeconds(0.5f);
        }

        if (AllDefeated(party))
        {
            EndBattle(false);
            yield break;
        }

        partyIndex = FindAlivePartyMember(0);
        targetIndex = FindAliveEnemy(0);

        playerTurn = true;
        RefreshUI("¡Turno del equipo!");
    }

    private int FindAlivePartyMember(int start)
    {
        for (int i = Mathf.Max(0, start); i < party.Length; i++)
        {
            if (party[i] != null && party[i].IsAlive)
                return i;
        }

        return -1;
    }

    private int FindAliveEnemy(int start)
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            int index = (start + i) % enemies.Length;

            if (enemies[index] != null &&
                enemies[index].IsAlive)
                return index;
        }

        return -1;
    }

    private bool AllDefeated(BattleUnit[] units)
    {
        foreach (BattleUnit unit in units)
        {
            if (unit != null && unit.IsAlive)
                return false;
        }

        return true;
    }

    private void RefreshUI(string message)
    {
        foreach (BattleUnit unit in party)
        {
            if (unit != null)
                unit.SetSelected(
                    playerTurn && unit == CurrentActor
                );
        }

        foreach (BattleUnit unit in enemies)
        {
            if (unit != null)
                unit.SetSelected(
                    playerTurn && unit == CurrentTarget
                );
        }

        battleUI.Refresh(
            party,
            enemies,
            CurrentTarget,
            message,
            playerTurn && !battleEnded,
            potionAvailable
        );
    }

    private void EndBattle(bool victory)
    {
        battleEnded = true;
        playerTurn = false;

        RefreshUI(
            victory ? "¡Victoria!" : "Derrota..."
        );

        Debug.Log(
            victory ? "Combate ganado." : "Combate perdido."
        );
    }
}
