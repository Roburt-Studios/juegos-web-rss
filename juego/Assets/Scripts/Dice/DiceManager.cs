using System;
using Patterns.ServiceLocator.Interfaces;
using UnityEngine;

public class DiceManager : MonoBehaviour, IService
{
    public event EventHandler<DiceRoll> Rolled;

    public void Initialize() { }

    public DiceRoll LastRoll { get; private set; }

    /// <summary>
    /// Lanza dos dados de seis caras y notifica a quien quiera mostrar o usar
    /// el resultado. Esta clase no conoce ningún elemento de interfaz.
    /// </summary>
    public DiceRoll Roll()
    {
        int firstNum = UnityEngine.Random.Range(1, 7);
        int secondNum = UnityEngine.Random.Range(1, 7);

        LastRoll = new DiceRoll(firstNum, secondNum);
        Rolled?.Invoke(this, LastRoll);

        return LastRoll;
    }

    // Se mantiene para los botones ya configurados en escenas antiguas.
    public void RollDices()
    {
        Roll();
    }
}

public readonly struct DiceRoll
{
    public int FirstDie { get; }
    public int SecondDie { get; }
    public int Total => FirstDie + SecondDie;

    public DiceRoll(int firstDie, int secondDie)
    {
        FirstDie = firstDie;
        SecondDie = secondDie;
    }
}
