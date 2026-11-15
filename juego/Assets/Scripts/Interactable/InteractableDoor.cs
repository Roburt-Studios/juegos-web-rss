using System.Collections;
using Patterns.ServiceLocator;
using UnityEngine;

public class InteractableDoor : MonoBehaviour, IInteractable
{
    [SerializeField]
    private float openRotationY = 90f;

    [SerializeField]
    private float closeRotationY = 0f;

    [SerializeField]
    private float duration = 1.0f;

    [SerializeField]
    private InventoryItem neededItem;

    [SerializeField]
    private string failedDialogID;

    private bool isOpen = false;
    private bool isMoving = false;
    private bool unlocked = false;

    public void Interacted()
    {
        if (isMoving)
            return;

        if (neededItem != null && !unlocked)
        {
            InventoryService inventory = ServiceLocator.Instance.GetService<InventoryService>();
            if (!inventory.Remove(neededItem))
            {
                ServiceLocator.Instance.GetService<DialogManager>().StartDialog(failedDialogID);
                return;
            }

            unlocked = true;
        }

        DoorAction();
    }

    public void DoorAction()
    {
        if (isMoving)
            return;

        StartCoroutine(DoorRoutine());
    }

    public void Enable()
    {
        this.gameObject.tag = "Interactable";
    }

    public void Disable()
    {
        this.gameObject.tag = "Untagged";
    }

    IEnumerator DoorRoutine()
    {
        isMoving = true;
        float elapsed = 0;
        Quaternion startRotation = transform.localRotation;
        float targetY = isOpen ? closeRotationY : openRotationY;

        while (elapsed < duration)
        {
            float y = Mathf.Lerp(startRotation.eulerAngles.y, targetY, elapsed / duration);
            transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        isOpen = !isOpen;
        isMoving = false;
    }
}
