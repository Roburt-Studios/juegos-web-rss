using Patterns.ServiceLocator;
using UnityEngine;

public class TalkToEnemy : MonoBehaviour
{
    [SerializeField]
    private string dialogId;

    private DialogManager dialogManager;
    private bool talked = false;

    public void Talk()
    {
        string dialog = !talked ? dialogId : "DHablado";
        dialogManager = ServiceLocator.Instance.GetService<DialogManager>();

        dialogManager.StartDialog(dialog, GetComponent<CombatEncounter>());
        talked = true;
    }
}
