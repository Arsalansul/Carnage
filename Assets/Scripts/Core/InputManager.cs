using System;
using Hybrid;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class InputManager : MonoBehaviour
{
    [Inject] private InputSystem_Actions inputActions;
    [Inject] private HybridHandler hybridHandler;

    private void Update()
    {
        hybridHandler.SetInputDataField(InputDataActionType.Move);
    }
    
    private void OnEnable()
    {
        inputActions.Player.Look.performed += OnMousePositionPerformed;
        inputActions.Player.Attack.performed += OnMouseLeftButtonClickPerformed;
        inputActions.Player.Attack.canceled += OnMouseLeftButtonClickCanceled;
        // inputActions.GameMap.MouseRightButtonClick.performed += OnMouseRightButtonClickPerformed;
        
        // ..digit1Key.isPressed += OnOneButtonPerformed;
        // inputActions.GameMap.Two.performed += OnTwoButtonPerformed;
        // inputActions.GameMap.Three.performed += OnThreeButtonPerformed;
        // inputActions.GameMap.Four.performed += OnFourButtonPerformed;
        // inputActions.GameMap.Five.performed += OnFiveButtonPerformed;
    }
    
    private void OnDisable()
    {
        inputActions.Player.Look.performed -= OnMousePositionPerformed;
        inputActions.Player.Attack.performed -= OnMouseLeftButtonClickPerformed;
        inputActions.Player.Attack.canceled -= OnMouseLeftButtonClickCanceled;
        // inputActions.GameMap.MouseRightButtonClick.performed -= OnMouseRightButtonClickPerformed;
        //
        // inputActions.GameMap.One.performed -= OnOneButtonPerformed;
        // inputActions.GameMap.Two.performed -= OnTwoButtonPerformed;
        // inputActions.GameMap.Three.performed -= OnThreeButtonPerformed;
        // inputActions.GameMap.Four.performed -= OnFourButtonPerformed;
        // inputActions.GameMap.Five.performed -= OnFiveButtonPerformed;
    }
    
    // private void OnMouseRightButtonClickPerformed(InputAction.CallbackContext context) => hybridHandler.SetInputDataField(InputDataActionType.MouseRightButton);
    private void OnMouseLeftButtonClickPerformed(InputAction.CallbackContext context) => hybridHandler.SetInputDataField(InputDataActionType.MouseLeftButton);
    
    private void OnMousePositionPerformed(InputAction.CallbackContext context) => hybridHandler.SetInputDataField(InputDataActionType.MousePos, context);
    private void OnMouseLeftButtonClickCanceled(InputAction.CallbackContext context) => hybridHandler.SetInputDataField(InputDataActionType.MouseLeftButtonCancel);
    
    // private void OnOneButtonPerformed(InputAction.CallbackContext context) => hybridHandler.SetInputDataField(InputDataActionType.One);
    // private void OnTwoButtonPerformed(InputAction.CallbackContext context) => hybridHandler.SetInputDataField(InputDataActionType.Two);
    // private void OnThreeButtonPerformed(InputAction.CallbackContext context) => hybridHandler.SetInputDataField(InputDataActionType.Three);
    // private void OnFourButtonPerformed(InputAction.CallbackContext context) => hybridHandler.SetInputDataField(InputDataActionType.Four);
    // private void OnFiveButtonPerformed(InputAction.CallbackContext context) => hybridHandler.SetInputDataField(InputDataActionType.Five);
}