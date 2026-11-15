using Patterns.ServiceLocator;
using UnityEngine;

public class InventoryPickup : MonoBehaviour, IInteractable
{
    [SerializeField]
    private InventoryItem item;

    [SerializeField, Min(1)]
    private int quantity = 1;

    [SerializeField]
    private bool destroyOnPickup = true;

    [SerializeField]
    private InventoryItem neededItem;

    [SerializeField]
    private string failedDialogID;

    private bool unlocked;

    public void Interacted()
    {
        if (neededItem == null || unlocked)
        {
            PickUpAction();
            return;
        }

        if (item == null || ServiceLocator.Instance == null)
        {
            Debug.LogWarning("InventoryPickup: falta el item o el ServiceLocator.", this);
            return;
        }

        InventoryService inventory = ServiceLocator.Instance.GetService<InventoryService>();
        if (!inventory.Remove(neededItem))
        {
            ServiceLocator.Instance.GetService<DialogManager>().StartDialog(failedDialogID);
            return;
        }

        if (TryPickUpAction())
        {
            unlocked = true;
            if (!destroyOnPickup)
                gameObject.tag = "Untagged";
        }
        else
            inventory.Add(neededItem);
    }

    public void PickUpAction()
    {
        TryPickUpAction();
    }

    private bool TryPickUpAction()
    {
        if (item == null || ServiceLocator.Instance == null)
        {
            Debug.LogWarning("InventoryPickup: falta el item o el ServiceLocator.", this);
            return false;
        }

        if (!ServiceLocator.Instance.GetService<InventoryService>().Add(item, quantity))
        {
            Debug.LogWarning("InventoryPickup: no hay espacio suficiente en el inventario.", this);
            return false;
        }

        Debug.Log($"Recogido: {item.displayName} x{quantity}.", this);
        if (destroyOnPickup)
            Destroy(gameObject);

        return true;
    }
}
