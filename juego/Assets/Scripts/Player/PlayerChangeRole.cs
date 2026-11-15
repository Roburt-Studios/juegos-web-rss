using UnityEngine;

public class PlayerChangeRole : MonoBehaviour
{
    [SerializeField] private Profile role;

    public void ChangeRole()
    {
        PlayerClassManager.instance.characterClass = role;
    }
}
