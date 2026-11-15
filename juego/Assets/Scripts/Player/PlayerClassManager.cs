using UnityEngine;

public class PlayerClassManager : MonoBehaviour
{
    public static PlayerClassManager instance;

    public Profile characterClass;

    public int selectedSkill = -1;

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
