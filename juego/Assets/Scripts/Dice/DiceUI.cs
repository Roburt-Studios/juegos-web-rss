using TMPro;
using UnityEngine;

/// <summary>
/// Presentador de la tirada. Puede sustituirse sin cambiar la lógica de dados.
/// </summary>
public class DiceUI : MonoBehaviour
{
    [SerializeField]
    private DiceManager diceManager;

    [SerializeField]
    private TMP_Text dice1;

    [SerializeField]
    private TMP_Text dice2;

    [SerializeField]
    private TMP_Text scoreText;

    private void OnEnable()
    {
        if (diceManager != null)
            diceManager.Rolled += ShowRoll;
    }

    private void Start()
    {
        if (diceManager != null && diceManager.LastRoll.Total > 0)
            RenderRoll(diceManager.LastRoll);
    }

    private void OnDisable()
    {
        if (diceManager != null)
            diceManager.Rolled -= ShowRoll;
    }

    private void ShowRoll(object sender, DiceRoll roll)
    {
        RenderRoll(roll);
    }

    private void RenderRoll(DiceRoll roll)
    {
        if (dice1 != null)
            dice1.text = roll.FirstDie.ToString();
        if (dice2 != null)
            dice2.text = roll.SecondDie.ToString();
        if (scoreText != null)
            scoreText.text = "Score: " + roll.Total;
    }
}
