
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    [Header("Textos")]
    public TMP_Text statusText;
    public TMP_Text partyStatusText;
    public TMP_Text enemyStatusText;
    public TMP_Text targetButtonText;

    [Header("Botones")]
    public Button attackButton;
    public Button skillButton;
    public Button potionButton;
    public Button targetButton;

    private BattleManager manager;

    public void Initialize(BattleManager battleManager)
    {
        manager = battleManager;

        attackButton.onClick.AddListener(
            () => manager.Attack()
        );

        skillButton.onClick.AddListener(
            () => manager.UseSkill()
        );

        potionButton.onClick.AddListener(
            () => manager.UsePotion()
        );

        targetButton.onClick.AddListener(
            () => manager.SelectNextTarget()
        );
    }

    public void Refresh(
        BattleUnit[] party,
        BattleUnit[] enemies,
        BattleUnit target,
        string message,
        bool playerTurn,
        bool potionAvailable
    )
    {
        statusText.text = message;

        partyStatusText.text = BuildStatus(party);
        enemyStatusText.text = BuildStatus(enemies);

        targetButtonText.text = target != null
            ? "Objetivo: " + target.unitName
            : "Sin objetivos";

        attackButton.interactable =
            playerTurn && target != null;

        skillButton.interactable =
            playerTurn && target != null;

        potionButton.interactable =
            playerTurn && potionAvailable;

        targetButton.interactable =
            playerTurn && target != null;
    }

    private string BuildStatus(BattleUnit[] units)
    {
        StringBuilder builder = new StringBuilder();

        foreach (BattleUnit unit in units)
        {
            if (unit == null) continue;

            builder.Append(unit.unitName);
            builder.Append("  ");
            builder.Append(unit.currentHP);
            builder.Append("/");
            builder.Append(unit.maxHP);
            builder.Append(" HP");

            if (!unit.IsAlive)
                builder.Append("  KO");

            builder.AppendLine();
        }

        return builder.ToString();
    }
}
