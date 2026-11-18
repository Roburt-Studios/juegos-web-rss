using System.Collections;
using System.Collections.Generic;
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

    [SerializeField]
    private Image inventoryPanel;

    [SerializeField]
    private TMP_Text collectedItemText;
    private SlotView[] views;
    private bool slotsReady;
    private GameObject inventoryContent;
    private readonly Queue<ItemNotification> pendingNotifications = new Queue<ItemNotification>();
    private Coroutine notificationRoutine;

    private struct ItemNotification
    {
        public string Name;
        public int Quantity;

        public ItemNotification(string name, int quantity)
        {
            Name = name;
            Quantity = quantity;
        }
    }

    private void Awake()
    {
        slotsReady = CacheSlotReferences();
        inventoryContent =
            inventoryPanel != null ? inventoryPanel.gameObject
            : slotsReady ? slots[0].parent.gameObject
            : null;

        if (collectedItemText == null && slotsReady)
        {
            TextMeshProUGUI text = Instantiate(views[0].itemName, transform);
            text.name = "CollectedItemText";
            RectTransform rect = text.rectTransform;
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = new Vector2(0f, -40f);
            rect.sizeDelta = new Vector2(600f, 70f);
            text.alignment = TextAlignmentOptions.Center;
            text.raycastTarget = false;
            collectedItemText = text;
        }

        if (collectedItemText != null)
            collectedItemText.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (input != null)
            input.InventoryEvent += ToggleInventory;
        if (inventory != null)
            inventory.ItemAdded += OnItemAdded;
    }

    private void Start()
    {
        if (inventoryContent != null)
            inventoryContent.SetActive(false);
        if (ServiceLocator.Instance == null)
        {
            Debug.LogError("InventoryUI: falta ServiceLocator en la escena.", this);
            return;
        }

        inventory = ServiceLocator.Instance.GetService<InventoryService>();
        inventory.Changed += Refresh;
        inventory.ItemAdded += OnItemAdded;
        Refresh();
    }

    private void OnDisable()
    {
        if (input != null)
            input.InventoryEvent -= ToggleInventory;
        if (inventory != null)
            inventory.ItemAdded -= OnItemAdded;
        if (notificationRoutine != null)
        {
            StopCoroutine(notificationRoutine);
            notificationRoutine = null;
        }
        pendingNotifications.Clear();
        if (collectedItemText != null)
            collectedItemText.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (inventory != null)
            inventory.Changed -= Refresh;
    }

    private void ToggleInventory()
    {
        if (inventoryContent != null)
            inventoryContent.SetActive(!inventoryContent.activeSelf);
    }

    private void OnItemAdded(InventoryItem item, int quantity)
    {
        if (collectedItemText == null)
            return;

        pendingNotifications.Enqueue(new ItemNotification(item.displayName, quantity));
        if (notificationRoutine == null)
            notificationRoutine = StartCoroutine(ShowPendingNotifications());
    }

    private IEnumerator ShowPendingNotifications()
    {
        while (pendingNotifications.Count > 0)
        {
            ItemNotification notification = pendingNotifications.Dequeue();
            yield return ShowNewItemCollectedCorrutine(notification.Name, notification.Quantity);
        }

        notificationRoutine = null;
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

    public IEnumerator ShowNewItemCollectedCorrutine(string itemName, int quantity)
    {
        if (collectedItemText == null)
            yield break;

        collectedItemText.text = quantity > 1 ? "+{itemName} x{quantity}" : $"+: {itemName}";
        collectedItemText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        collectedItemText.gameObject.SetActive(false);
    }
}
