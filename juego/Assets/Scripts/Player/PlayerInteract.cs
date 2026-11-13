using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField]
    private InputReader input;

    [Header("Collider de proximidad")]
    [SerializeField]
    private SphereCollider proximityTrigger;

    [SerializeField]
    private LayerMask mask;

    void Start()
    {
        if (proximityTrigger == null)
            Debug.LogError("PlayerInteract: Proximity collider not set");
    }

    void Interact()
    {
        if (proximityTrigger == null)
            return;

        Vector3 origin = proximityTrigger.transform.TransformPoint(proximityTrigger.center);
        Vector3 scale = proximityTrigger.transform.lossyScale;
        float radius = proximityTrigger.radius * Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z));
        Collider[] nearby = Physics.OverlapSphere(origin, radius, mask, QueryTriggerInteraction.Collide);

        IInteractable closest = null;
        float closestDistance = float.MaxValue;
        foreach (Collider candidate in nearby)
        {
            if (!candidate.CompareTag("Interactable"))
                continue;

            IInteractable interactable = candidate.GetComponent<IInteractable>();
            if (interactable == null)
                continue;

            Vector3 point = candidate.ClosestPoint(origin);
            Vector3 direction = point - origin;
            float distance = direction.magnitude;
            if (distance >= closestDistance)
                continue;

            if (distance > 0.01f &&
                Physics.Raycast(origin, direction, out RaycastHit hit, distance + 0.05f,
                    Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore) &&
                hit.collider != candidate)
                continue;

            closest = interactable;
            closestDistance = distance;
        }

        closest?.Interacted();
    }

    void OnEnable()
    {
        input.InteractEvent += Interact;
    }

    void OnDisable()
    {
        input.InteractEvent -= Interact;
    }

}
