using Patterns.ServiceLocator;
using UnityEngine;

public class PassChest : MonoBehaviour, IInteractable
{
    [SerializeField] private InventoryItem chestKey;
    [SerializeField] private InventoryItem pass;

    public void Interacted()
    {
        if (chestKey == null || pass == null)
        {
            Debug.LogError("PassChest: faltan la llave o el pase.", this);
            return;
        }

        InventoryService inventory = ServiceLocator.Instance.GetService<InventoryService>();
        DialogManager dialog = ServiceLocator.Instance.GetService<DialogManager>();

        if (!inventory.Remove(chestKey))
        {
            dialog.StartDialog("DCofre");
            return;
        }

        if (!inventory.Add(pass))
        {
            inventory.Add(chestKey);
            Debug.LogWarning("PassChest: no hay espacio en el inventario para el pase.", this);
            return;
        }

        gameObject.tag = "Untagged";
        dialog.StartDialog("DCofreConPase");
    }
}
