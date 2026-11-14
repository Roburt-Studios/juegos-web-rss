using UnityEngine;

[CreateAssetMenu(fileName = "InventoryItem", menuName = "Inventory/Item")]
public class InventoryItem : ScriptableObject
{
    public string displayName;
    public Sprite icon;

    [Min(1)]
    public int maxStack = 1;
}
