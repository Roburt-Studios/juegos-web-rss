using System;
using Patterns.ServiceLocator;
using Patterns.ServiceLocator.Interfaces;
using UnityEngine;

public class DiceManager : MonoBehaviour, IService
{
    public event EventHandler<DiceRoll> Rolled;

    PlayerClassManager manager;

    void Awake()
    {
        manager = ServiceLocator.Instance.GetService<PlayerClassManager>();
    }

    public void Initialize() { }

    public DiceRoll LastRoll { get; private set; }

    /// <summary>
    /// Lanza dos dados de seis caras y notifica a quien quiera mostrar o usar
    /// el resultado. Esta clase no conoce ningún elemento de interfaz.
    /// </summary>
    public DiceRoll Roll(bool applyCharisma)
    {
        int charisma = applyCharisma
            ? PlayerDataFactory.GetData(manager.characterClass).Charisma
            : 0;

        int firstNum = MayIncreaseWithCharisma(charisma);
        int secondNum = MayIncreaseWithCharisma(charisma);

        LastRoll = new DiceRoll(firstNum, secondNum);
        Rolled?.Invoke(this, LastRoll);

        return LastRoll;
    }

    //Hay reroll según lo alto que sea el carisma, a más alto más probabilidad de hacer reroll, pero ello no implica sacar
    //siempre un mejor resultado, solo uno que no sea peor
    public int MayIncreaseWithCharisma(int charisma)
    {
        int result = UnityEngine.Random.Range(1, 7);

        float prob = Mathf.Clamp01(charisma / 100f);

        if(UnityEngine.Random.value < prob)
        {
            result = Mathf.Max(result, UnityEngine.Random.Range(1, 7));
        }

        return result;
    }

    // Se mantiene para los botones ya configurados en escenas antiguas.
    public void RollDices(bool applyCharisma)
    {
        Roll(applyCharisma);
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
