using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputReader", menuName = "Input/Input Reader")]
public class InputReader : ScriptableObject, PlayerControls.IPlayerActions
{
    private PlayerControls controls;

    // Evento a los que se suscriben los componentes
    public event Action<Vector2> MoveEvent;
    public event Action InteractEvent;
    public event Action InventoryEvent;

    private void OnEnable()
    {
        if (controls == null)
        {
            controls = new PlayerControls();
            controls.Player.SetCallbacks(this); // Implementa la interfaz generada
        }
        EnableGameplayInput();
    }

    private void OnDisable()
    {
        DisableAllInput();
    }

    public void EnableGameplayInput()
    {
        controls.Player.Enable();
    }

    public void DisableAllInput()
    {
        controls.Player.Disable();
    }

    // Callbacks
    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
            MoveEvent?.Invoke(context.ReadValue<Vector2>());
        else if (context.canceled)
            MoveEvent?.Invoke(Vector2.zero);
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
            InteractEvent?.Invoke();
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.performed)
            InventoryEvent?.Invoke();
    }
}
