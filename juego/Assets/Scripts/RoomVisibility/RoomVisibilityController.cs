using UnityEngine;

[RequireComponent(typeof(Camera))]
public class RoomVisibilityController : MonoBehaviour
{
    [SerializeField]
    private LayerMask roomLayers;

    [SerializeField]
    private LayerMask initialRoom;

    private Camera roomCamera;
    private int commonLayers;

    private void Awake()
    {
        roomCamera = GetComponent<Camera>();
        commonLayers = roomCamera.cullingMask & ~roomLayers.value;

        ShowRoom(initialRoom);
    }

    public void ShowRoom(LayerMask room)
    {
        int selected = room.value;
        if (selected == 0 || (selected & roomLayers.value) != selected)
        {
            Debug.LogWarning(
                "RoomVisibilityController: Select only layers included in Room Layers.",
                this
            );
            return;
        }

        roomCamera.cullingMask = commonLayers | selected;
    }
}
