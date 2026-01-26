using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class InputManager : MonoBehaviour
{
    [Inject] private NewInputActions inputActions;
    [Inject] private HybridHandler hybridHandler;
    
    private void Update()
    {
        hybridHandler.SetInputDataField(InputDataActionType.Move);
    }
    
    private void OnEnable()
    {
        inputActions.GameMap.MousePosition.performed += OnMousePositionPerformed;
        inputActions.GameMap.MouseLeftButtonClick.performed += OnMouseLeftButtonClickPerformed;
        inputActions.GameMap.MouseLeftButtonClick.canceled += OnMouseLeftButtonClickCanceled;
        inputActions.GameMap.MouseRightButtonClick.performed += OnMouseRightButtonClickPerformed;
        
        inputActions.GameMap.One.performed += OnOneButtonPerformed;
        inputActions.GameMap.Two.performed += OnTwoButtonPerformed;
    }
    
    private void OnDisable()
    {
        inputActions.GameMap.MousePosition.performed -= OnMousePositionPerformed;
        inputActions.GameMap.MouseLeftButtonClick.performed -= OnMouseLeftButtonClickPerformed;
        inputActions.GameMap.MouseLeftButtonClick.canceled -= OnMouseLeftButtonClickCanceled;
        inputActions.GameMap.MouseRightButtonClick.performed -= OnMouseRightButtonClickPerformed;
        
        inputActions.GameMap.One.performed -= OnOneButtonPerformed;
        inputActions.GameMap.Two.performed -= OnTwoButtonPerformed;
    }
    
    private void OnMouseRightButtonClickPerformed(InputAction.CallbackContext context) => hybridHandler.SetInputDataField(InputDataActionType.MouseRightButton);
    private void OnMouseLeftButtonClickPerformed(InputAction.CallbackContext context) => hybridHandler.SetInputDataField(InputDataActionType.MouseLeftButton);
    
    private void OnMousePositionPerformed(InputAction.CallbackContext context) => hybridHandler.SetInputDataField(InputDataActionType.MousePos, context);
    private void OnMouseLeftButtonClickCanceled(InputAction.CallbackContext context) => hybridHandler.SetInputDataField(InputDataActionType.MouseLeftButtonCancel);
    
    private void OnOneButtonPerformed(InputAction.CallbackContext context) => hybridHandler.SetInputDataField(InputDataActionType.One);
    private void OnTwoButtonPerformed(InputAction.CallbackContext context) => hybridHandler.SetInputDataField(InputDataActionType.Two);
}