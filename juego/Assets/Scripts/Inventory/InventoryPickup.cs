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

    public void Interacted()
    {
        PickUpAction();
    }

    public void PickUpAction()
    {
        if (item == null || ServiceLocator.Instance == null)
        {
            Debug.LogWarning("InventoryPickup: falta el item o el ServiceLocator.", this);
            return;
        }

        if (!ServiceLocator.Instance.GetService<InventoryService>().Add(item, quantity))
        {
            Debug.LogWarning("InventoryPickup: no hay espacio suficiente en el inventario.", this);
            return;
        }

        Debug.Log($"Recogido: {item.displayName} x{quantity}.", this);
        if (destroyOnPickup)
            Destroy(gameObject);
    }
}
