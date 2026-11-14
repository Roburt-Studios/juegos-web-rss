using Patterns.ServiceLocator;
using UnityEngine;
using UnityEngine.Events;

public class InteractableWithItem : MonoBehaviour, IInteractable
{
    [SerializeField]
    private InventoryItem neededItem;

    [SerializeField]
    private string failedDialogID;

    [SerializeField]
    private UnityEvent callbackAction;

    void Start()
    {
        if (callbackAction == null)
            Debug.LogError("No hay callback para le interacción", this);
    }

    public void Enable()
    {
        this.gameObject.tag = "Interactable";
    }

    public void Disable()
    {
        this.gameObject.tag = "Untagged";
    }

    public void Interacted()
    {
        if (ServiceLocator.Instance.GetService<InventoryService>().Remove(neededItem))
        {
            callbackAction?.Invoke();
        }
        else
        {
            ServiceLocator.Instance.GetService<DialogManager>().StartDialog(failedDialogID);
        }
    }
}
