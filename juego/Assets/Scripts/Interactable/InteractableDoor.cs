using System.Collections;
using UnityEngine;

public class InteractableDoor : MonoBehaviour, IInteractable
{
    [SerializeField]
    private float openRotationY = 90f;

    [SerializeField]
    private float closeRotationY = 0f;

    [SerializeField]
    private float duration = 1.0f;

    private bool isOpen = false;
    private bool isMoving = false;

    public void Interacted()
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
