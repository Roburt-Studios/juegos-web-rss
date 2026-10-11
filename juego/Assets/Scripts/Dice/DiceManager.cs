using Patterns.ServiceLocator.Interfaces;
using TMPro;
using UnityEngine;

public class DiceManager : MonoBehaviour, IService
{
    [SerializeField]
    private TMP_Text dice1;

    [SerializeField]
    private TMP_Text dice2;

    [SerializeField]
    private TMP_Text scoreText;

    private int currentDiceScore;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    public void Initialize() { }

    public void RollDices()
    {
        currentDiceScore = 0;
        int firstNum = Random.Range(1, 6);
        int secondNum = Random.Range(1, 6);

        currentDiceScore = firstNum + secondNum;

        dice1.text = firstNum.ToString();
        dice2.text = secondNum.ToString();

        scoreText.text = "Score: " + currentDiceScore;
    }
}
