using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField]
    private InputReader input;

    [Header("Collider de proximidad")]
    [SerializeField]
    private SphereCollider proximityTrigger;
    private Collider[] nearby;

    [SerializeField]
    private LayerMask mask;

    [Header("Prefab con la imagen de la letra <e> para el aviso de interacción")]
    [SerializeField]
    private GameObject interactWarning;

    [SerializeField]
    private Transform cameraTransform;

    // Parametros para el calculo de los candidatosVector3 origin
    private Vector3 origin;
    private Vector3 scale;
    private float radius;

    void Start()
    {
        if (proximityTrigger == null)
            Debug.LogError("PlayerInteract: Proximity collider not set");
    }

    void Update()
    {
        // Busqueda de los candidatos a interacción
        origin = proximityTrigger.transform.TransformPoint(proximityTrigger.center);
        scale = proximityTrigger.transform.lossyScale;
        radius =
            proximityTrigger.radius
            * Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z));

        nearby = Physics.OverlapSphere(origin, radius, mask, QueryTriggerInteraction.Collide);

        if (nearby.Length > 0)
        {
            interactWarning.SetActive(true);
            interactWarning.transform.LookAt(cameraTransform, Vector3.up);
        }
        else
        {
            interactWarning.SetActive(false);
        }
    }

    void Interact()
    {
        if (proximityTrigger == null)
            return;

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

            if (
                distance > 0.01f
                && Physics.Raycast(
                    origin,
                    direction,
                    out RaycastHit hit,
                    distance + 0.05f,
                    Physics.DefaultRaycastLayers,
                    QueryTriggerInteraction.Ignore
                )
                && hit.collider != candidate
            )
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
