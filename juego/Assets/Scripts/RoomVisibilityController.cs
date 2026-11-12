using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class RoomVisibilityController : MonoBehaviour
{
    [SerializeField]
    private LayerMask roomLayers;

    [SerializeField]
    private LayerMask initialRoom;

    [SerializeField]
    private CanvasFader transitionFader;

    [SerializeField, Min(0f)]
    private float fadeDuration = 0.35f;

    private Camera roomCamera;
    private int commonLayers;
    private int visibleRoom;
    private LayerMask requestedRoom;
    private Coroutine transitionCoroutine;

    private void Awake()
    {
        roomCamera = GetComponent<Camera>();
        commonLayers = roomCamera.cullingMask & ~roomLayers.value;

        ShowRoom(initialRoom);
    }

    public void ShowRoom(LayerMask room)
    {
        if (!IsValidRoom(room))
        {
            Debug.LogWarning(
                "RoomVisibilityController: Select only layers included in Room Layers.",
                this
            );
            return;
        }

        visibleRoom = room.value;
        roomCamera.cullingMask = commonLayers | visibleRoom;
    }

    public void TransitionToRoom(LayerMask room)
    {
        if (!IsValidRoom(room))
        {
            Debug.LogWarning("RoomVisibilityController: Invalid room layers for transition.", this);
            return;
        }

        requestedRoom = room;
        if (transitionCoroutine == null && visibleRoom != room.value)
        {
            if (transitionFader == null)
                ShowRoom(room);
            else
                transitionCoroutine = StartCoroutine(TransitionRoutine());
        }
    }

    private bool IsValidRoom(LayerMask room)
    {
        return room.value != 0 && (room.value & roomLayers.value) == room.value;
    }

    private IEnumerator TransitionRoutine()
    {
        while (requestedRoom.value != visibleRoom)
        {
            yield return transitionFader.FadeToAndWait(1f, fadeDuration);
            ShowRoom(requestedRoom);
            yield return transitionFader.FadeToAndWait(0f, fadeDuration);
        }

        transitionCoroutine = null;
    }
}
