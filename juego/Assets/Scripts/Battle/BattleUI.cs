
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

    [Header("Botones")]
    public Button attackButton;
    public Button defendButton;
    public Button potionButton;
    public Button specialButton;


    private BattleManager manager;

    public void Initialize(BattleManager battleManager)
    {
        manager = battleManager;

        attackButton.onClick.AddListener(
            () => manager.Attack()
        );

        defendButton.onClick.AddListener(
            () => manager.Defend()
        );

        potionButton.onClick.AddListener(
            () => manager.UsePotion()
        );

        specialButton.onClick.AddListener(
            () => manager.UseSpecial()
        );


    }

    private void Update()
    {
        if (manager == null)
            return;

        if (!manager.IsSelectingTarget)
            return;

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            manager.SelectNextTarget();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            manager.SelectPreviousTarget();
        }

        if (Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            manager.ConfirmAction();
        }
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

        attackButton.interactable = playerTurn;

        specialButton.interactable = playerTurn;

        potionButton.interactable =
            playerTurn && potionAvailable;

        defendButton.interactable = playerTurn;

        foreach (BattleUnit enemy in enemies)
        {
            if (enemy == null)
                continue;

            enemy.SetSelected(
                playerTurn &&
                target != null &&
                enemy == target &&
                manager.IsSelectingTarget
            );
        }
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
