using System;
using Patterns.ServiceLocator.Interfaces;
using UnityEngine;

public class InventoryService : MonoBehaviour, IService
{
    public const int Capacity = 10;

    public struct Slot
    {
        public InventoryItem Item { get; }
        public int Quantity { get; }

        public Slot(InventoryItem item, int quantity)
        {
            Item = item;
            Quantity = quantity;
        }
    }

    private readonly Slot[] slots = new Slot[Capacity];
    public event Action Changed;

    public void Initialize()
    {
        Debug.Log("Starting inventory service");
    }

    public Slot GetSlot(int index)
    {
        if (index < 0 || index >= Capacity)
            Debug.LogError("Indice del inventario no valido");
        return slots[index];
    }

    // Devuelve false si no cabe la cantidad completa
    public bool Add(InventoryItem item, int quantity = 1)
    {
        if (item == null || quantity <= 0)
            return false;

        // El espacio disponible en el inventario
        int space = 0;
        int maxStack = Mathf.Max(1, item.maxStack);
        for (int i = 0; i < Capacity; i++)
        {
            if (slots[i].Item == item)
                space += maxStack - slots[i].Quantity;
            else if (slots[i].Item == null)
                space += maxStack;
        }
        if (space < quantity)
            return false;

        int remaining = quantity;
        for (int i = 0; i < Capacity && remaining > 0; i++)
        {
            if (slots[i].Item != item || slots[i].Quantity >= maxStack)
                continue;
            int added = Mathf.Min(remaining, maxStack - slots[i].Quantity);
            slots[i] = new Slot(item, slots[i].Quantity + added);
            remaining -= added;
        }
        for (int i = 0; i < Capacity && remaining > 0; i++)
        {
            if (slots[i].Item != null)
                continue;
            int added = Mathf.Min(remaining, maxStack);
            slots[i] = new Slot(item, added);
            remaining -= added;
        }
        Changed?.Invoke();
        return true;
    }

    public bool Remove(InventoryItem item, int quantity = 1)
    {
        if (item == null || quantity <= 0)
            return false;

        int available = 0;
        for (int i = 0; i < Capacity; i++)
            if (slots[i].Item == item)
                available += slots[i].Quantity;
        if (available < quantity)
            return false;

        int remaining = quantity;
        for (int i = Capacity - 1; i >= 0 && remaining > 0; i--)
        {
            if (slots[i].Item != item)
                continue;
            int removed = Mathf.Min(remaining, slots[i].Quantity);
            int newQuantity = slots[i].Quantity - removed;
            slots[i] = newQuantity == 0 ? default : new Slot(item, newQuantity);
            remaining -= removed;
        }
        Changed?.Invoke();
        return true;
    }
}
