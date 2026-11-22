using JetBrains.Annotations;
using Patterns.ServiceLocator.Interfaces;
using UnityEngine;

public class PlayerClassManager : MonoBehaviour, IService
{
    public static PlayerClassManager instance;

    public Profile characterClass = Profile.None;

    public int selectedSkill = -1;

    public int savedHP = 100;
    public int savedMP = -1;

    public void Initialize() 
    {
        if(characterClass == Profile.None)
        {
            characterClass = Profile.Intelligence;
        }
    }
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
