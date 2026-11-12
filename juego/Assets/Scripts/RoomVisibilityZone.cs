using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class RoomVisibilityZone : MonoBehaviour
{
    [SerializeField]
    private RoomVisibilityController visibilityController;

    [SerializeField]
    private LayerMask roomLayer;

    private void Reset()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    private void Awake()
    {
        if (visibilityController == null && Camera.main != null)
            visibilityController = Camera.main.GetComponent<RoomVisibilityController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (visibilityController != null && other.GetComponentInParent<PlayerMovment>() != null)
            visibilityController.TransitionToRoom(roomLayer);
    }
}
