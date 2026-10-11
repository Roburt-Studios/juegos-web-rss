using Patterns.ServiceLocator;
using UnityEngine;

public class TalkToEnemy : MonoBehaviour
{
    [SerializeField]
    private string dialogId;

    public void Talk()
    {
        ServiceLocator.Instance.GetService<DialogManager>().StartDialog(dialogId);
        gameObject.tag = "Untagged";
    }
}
