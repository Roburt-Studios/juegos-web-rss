using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovment : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField]
    private float moveSpeed = 10f;

    [Tooltip("Velocidad a la que el personaje se orienta hacia su dirección de movimiento.")]
    [SerializeField]
    private float rotationSpeed = 720f;

    [Header("Referencia")]
    [Tooltip("La cámara cuya orientación horizontal determina W/A/S/D. Si se deja vacía, se usa Main Camera.")]
    [SerializeField]
    private Camera movementCamera;

    [Header("Input")]
    [SerializeField]
    private InputReader input;
    private Vector3 inputVector;

    [Header("Gravedad")]
    [SerializeField]
    private float groundedPullForce = -9.8f;

    private CharacterController characterController;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Start()
    {
        if (movementCamera == null)
            movementCamera = Camera.main;

        if (movementCamera == null)
            Debug.LogWarning("PlayerMovment: No se encontró una Main Camera; el movimiento usará la orientación del jugador.");
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void HandleInput(Vector2 input)
    {
        inputVector = input;
    }

    void Move()
    {
        Vector3 rawInput = new Vector3(inputVector.x, 0f, inputVector.y);

        // Se elimina la altura/inclinación de la cámara: solo importa hacia dónde mira sobre el suelo.
        Transform reference = movementCamera != null ? movementCamera.transform : transform;
        Vector3 cameraForward = Vector3.ProjectOnPlane(reference.forward, Vector3.up).normalized;
        Vector3 cameraRight = Vector3.ProjectOnPlane(reference.right, Vector3.up).normalized;

        // W avanza en la dirección que mira la cámara; por tanto, si la cámara está detrás,
        // el jugador se aleja de ella. A/S/D conservan la misma relación.
        Vector3 moveDirection = Vector3.ClampMagnitude(
            (cameraRight * rawInput.x) + (cameraForward * rawInput.z),
            1f);

        // El personaje mira hacia donde se desplaza, sin afectar la inclinación vertical.
        if (moveDirection.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime);
        }

        Vector3 motion = (moveDirection * moveSpeed) + (Vector3.up * groundedPullForce);
        characterController.Move(motion * Time.deltaTime);
    }

    void OnEnable()
    {
        input.MoveEvent += HandleInput;
    }

    void OnDisable()
    {
        input.MoveEvent -= HandleInput;
    }
}
