using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.XR;
using static System.TimeZoneInfo;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    [Header("Target")]
    public Transform target;

    [Header("Follow")]
    public float followSpeed = 5f;
    public float focusHeight = 1f;

    [Header("Zoom")]
    public Camera cam;
    public float zoomSpeed = 5f;
    public float initialZoom = 10f;
    public float minZoom = 4f;
    public float maxZoom = 15f;

    [Header("Rotation")]
    public Transform pivot;
    public float pitch = 45f;
    public float rotationSpeed = 0.25f;
    public float rotationSmoothness = 5f;
    //public float maxRotationAngle = 80f;

    private float currentYaw;
    private float targetYaw;
    private float initialYaw;

    [Header("Look Ahead")]
    public float lookAheadDistance = 2f;
    public float lookAheadSpeed = 4f;

    private Vector3 currentLookAhead;
    private Vector3 rootVelocity;

    [SerializeField] private float SmoothTime = 0.7f;
    private float gameplayZoom;
    private float currentZoom;
    private float targetZoom;

    [SerializeField]
    private float zoomVelocity;

    private Quaternion targetRotation;
    private Vector3 targetPosition;

    [Header("Collision")]
    public LayerMask collisionLayers;
    public float collisionRadius = 0.3f;
    public float collisionOffSet = 0.2f;
    public float minCollisionDistance = 0.5f;
    public float recoverSpeed = 5f;
    float collisionDistance;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("CameraController: Target is not assigned.");
            enabled = false;
            return;
        }
        if (cam == null)
        {
            Debug.LogError("CameraController: Camera is not assigned.");
            enabled = false;
            return;
        }
        if (pivot == null)
        {
            Debug.LogError("CameraController: Pivot is not assigned.");
            enabled = false;
            return;
        }

        transform.position = target.position + Vector3.up * focusHeight + target.forward * lookAheadDistance;

        currentYaw = pivot.localEulerAngles.y;
        targetYaw = currentYaw;

        initialYaw = currentYaw;

        gameplayZoom = Mathf.Clamp(initialZoom, minZoom, maxZoom);

        currentZoom = gameplayZoom;
        collisionDistance = currentZoom;
        targetZoom = gameplayZoom;

        Vector3 camPos = cam.transform.localPosition;
        camPos.x = 0f;
        camPos.y = 0f;
        camPos.z = -currentZoom;
        cam.transform.localPosition = camPos;
        SetSmoothTime(SmoothTime);

        pivot.localRotation = Quaternion.Euler(pitch, currentYaw, 0f);
    }

    void Update()
    {
        UpdateGameplayRotation();
        UpdateGameplayZoomInput();
    }

    void LateUpdate()
    {
        UpdateLookAhead();
        UpdateGameplayPosition();
        UpdateGameplayZoomTarget();

        UpdateCamera();
    }

    public void UpdateLookAhead()
    {
        Vector3 movementDirection = target.forward;

        Vector3 desiredLookAhead = movementDirection * lookAheadDistance;

        currentLookAhead = Vector3.Lerp(
            currentLookAhead,
            desiredLookAhead,
            lookAheadSpeed * Time.deltaTime
        );
    }

    public void UpdateGameplayPosition()
    {
        SetTargetPosition(target.position + Vector3.up * focusHeight + currentLookAhead);
        //SetTargetPosition(target.position + currentLookAhead);
    }

    public void UpdateGameplayRotation()
    {
        if (Mouse.current == null)
            return;

        if (Mouse.current.rightButton.isPressed)
        {
            targetYaw += Mouse.current.delta.ReadValue().x * rotationSpeed;

            float relativeYaw = Mathf.DeltaAngle(initialYaw, targetYaw);

            //relativeYaw = Mathf.Clamp(relativeYaw, -maxRotationAngle, maxRotationAngle);

            targetYaw = initialYaw + relativeYaw;
        }

        //currentYaw = Mathf.LerpAngle(currentYaw, targetYaw, rotationSmoothness * Time.deltaTime);

        SetTargetRotation(Quaternion.Euler(pitch, targetYaw, 0f));
    }

    public void UpdateGameplayZoomInput()
    {
        float scroll = Mouse.current.scroll.ReadValue().y;

        if (Mathf.Abs(scroll) < 0.01f)
            return;

        gameplayZoom -= scroll * zoomSpeed * 0.1f;

        gameplayZoom = Mathf.Clamp(gameplayZoom, minZoom, maxZoom);
    }

    public void UpdateGameplayZoomTarget()
    {
        SetTargetZoom(gameplayZoom);
    }

    public void SetSmoothTime(float value)
    {
        SmoothTime = value;
    }

    public void SyncGameplayYaw()
    {
        currentYaw = pivot.localEulerAngles.y;

        targetYaw = currentYaw;
    }

    private void UpdateCamera()
    {
        if (SmoothTime <= 0f)
        {
            transform.position = targetPosition;
        }
        else
        {
            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPosition,
                ref rootVelocity,
                SmoothTime
            );
        }

        pivot.localRotation = Quaternion.Slerp(
            pivot.localRotation,
            targetRotation,
            rotationSmoothness * Time.deltaTime
        );

        currentZoom = Mathf.SmoothDamp(currentZoom, targetZoom, ref zoomVelocity, SmoothTime);

        float allowedDistance = GetAllowedDistance(currentZoom);

        if(allowedDistance < collisionDistance)
        {
            collisionDistance = allowedDistance;
        }
        else
        {
            collisionDistance = Mathf.Lerp(collisionDistance, allowedDistance, recoverSpeed * Time.deltaTime);
        }

        Vector3 pos = cam.transform.localPosition;
        pos.z = -collisionDistance;
        cam.transform.localPosition = pos;
    }

    private float GetAllowedDistance(float currentZoom)
    {
        Vector3 origin = pivot.position;
        Vector3 direction = -pivot.forward;

        if(Physics.SphereCast(origin, collisionRadius, direction, out RaycastHit hit, currentZoom, collisionLayers, QueryTriggerInteraction.Ignore))
        {
            return Mathf.Max(minCollisionDistance, hit.distance - collisionOffSet);
        }

        return currentZoom;
    }

    public void SetTargetPosition(Vector3 position)
    {
        targetPosition = position;
    }

    public void SetTargetRotation(Quaternion rotation)
    {
        targetRotation = rotation;
    }

    public void SetTargetZoom(float zoom)
    {
        targetZoom = zoom;
    }
}
