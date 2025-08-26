using System;
using UnityEngine.InputSystem;

public class UserInput : Singleton<UserInput> {
  private InputSystemActions inputActions;

  private void OnEnable() {
    inputActions = new InputSystemActions();
    inputActions.Enable();
    inputActions.UI.Enable();
    inputActions.Player.Enable();
  }

  public void RegisterPointerUpListener(Action<InputAction.CallbackContext> action) => inputActions.Player.Attack.performed += action;
  public void UnregisterPointerUpListener(Action<InputAction.CallbackContext> action) => inputActions.Player.Attack.performed -= action;
}