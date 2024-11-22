using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour, PlayerInputActions.IGameplayActions
{
    PlayerInputActions inputs;

    public event Action<Vector2> OnMovementEvent;
    public event Action<Vector2> OnLookEvent;

    private void Awake()
    {
        inputs = new PlayerInputActions();
        inputs.Gameplay.SetCallbacks(this);
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            OnMovementEvent?.Invoke(context.ReadValue<Vector2>());
        } else if(context.canceled)
        {
            OnMovementEvent?.Invoke(Vector2.zero);
        }
    }
    public void OnMousePos(InputAction.CallbackContext context)
    {
        if(context.performed || context.started)
        {
            OnLookEvent?.Invoke(context.ReadValue<Vector2>());
        }
    }

    private void OnEnable()
    {
        inputs.Gameplay.Enable();
    }
    private void OnDisable()
    {
        inputs.Gameplay.Disable();
    }

}
