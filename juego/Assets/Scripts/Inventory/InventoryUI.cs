using Patterns.ServiceLocator;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class InventoryUI : MonoBehaviour
{
    [SerializeField]
    private InputReader input;

    [SerializeField]
    private RectTransform[] slots;

    private struct SlotView
    {
        public Image icon;
        public TextMeshProUGUI itemName;
        public TextMeshProUGUI quantity;
    }

    private InventoryService inventory;
    private Canvas inventoryCanvas;
    private SlotView[] views;
    private bool slotsReady;

    private void Awake()
    {
        inventoryCanvas = GetComponent<Canvas>();
        slotsReady = CacheSlotReferences();
    }

    private void OnEnable()
    {
        if (input != null)
            input.InventoryEvent += ToggleInventory;
    }

    private void Start()
    {
        inventoryCanvas.enabled = false;
        if (ServiceLocator.Instance == null)
        {
            Debug.LogError("InventoryUI: falta ServiceLocator en la escena.", this);
            return;
        }

        inventory = ServiceLocator.Instance.GetService<InventoryService>();
        inventory.Changed += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        if (input != null)
            input.InventoryEvent -= ToggleInventory;
    }

    private void OnDestroy()
    {
        if (inventory != null)
            inventory.Changed -= Refresh;
    }

    private void ToggleInventory()
    {
        inventoryCanvas.enabled = !inventoryCanvas.enabled;
    }

    private bool CacheSlotReferences()
    {
        if (slots == null || slots.Length != InventoryService.Capacity)
        {
            Debug.LogError("InventoryUI: asigna las 10 casillas en Slots.", this);
            return false;
        }

        views = new SlotView[slots.Length];
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
            {
                Debug.LogError($"InventoryUI: falta la casilla " + i, this);
                return false;
            }

            Transform icon = slots[i].Find("ItemIcon");
            Transform itemName = slots[i].Find("ItemName");
            Transform quantity = slots[i].Find("Quantity");
            views[i].icon = icon != null ? icon.GetComponent<Image>() : null;
            views[i].itemName = itemName != null ? itemName.GetComponent<TextMeshProUGUI>() : null;
            views[i].quantity = quantity != null ? quantity.GetComponent<TextMeshProUGUI>() : null;

            if (views[i].icon == null || views[i].itemName == null || views[i].quantity == null)
            {
                Debug.LogError(
                    $"InventoryUI: en {slots[i].name} faltan ItemIcon (Image), ItemName o Quantity (TextMeshProUGUI).",
                    slots[i]
                );
                return false;
            }
        }
        return true;
    }

    private void Refresh()
    {
        if (!slotsReady)
            return;

        for (int i = 0; i < views.Length; i++)
        {
            InventoryService.Slot slot = inventory.GetSlot(i);
            bool occupied = slot.Item != null;
            views[i].icon.sprite = occupied ? slot.Item.icon : null;
            views[i].icon.enabled = occupied && slot.Item.icon != null;
            views[i].itemName.text = occupied ? slot.Item.displayName : "";
            views[i].quantity.text = occupied && slot.Quantity > 1 ? slot.Quantity.ToString() : "";
        }
    }
}
