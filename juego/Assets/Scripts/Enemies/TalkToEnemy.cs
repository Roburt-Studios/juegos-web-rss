using Patterns.ServiceLocator;
using UnityEngine;

public class TalkToEnemy : MonoBehaviour
{
    [SerializeField]
    private string dialogId;

    private DialogManager dialogManager;
    private bool talked = false;

    public bool Talk()
    {
        string dialog = !talked ? dialogId : "DHablado";
        dialogManager = ServiceLocator.Instance.GetService<DialogManager>();

        if (!dialogManager.StartDialog(dialog, GetComponent<CombatEncounter>()))
            return false;

        talked = true;
        return true;
    }
}
