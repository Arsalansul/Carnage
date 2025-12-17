using MonoBehaviourBridges;
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
    }
    
    private void OnDisable()
    {
        inputActions.GameMap.MousePosition.performed -= OnMousePositionPerformed;
        inputActions.GameMap.MouseLeftButtonClick.performed -= OnMouseLeftButtonClickPerformed;
        inputActions.GameMap.MouseLeftButtonClick.canceled -= OnMouseLeftButtonClickCanceled;
        inputActions.GameMap.MouseRightButtonClick.performed -= OnMouseRightButtonClickPerformed;
    }
    
    private void OnMouseRightButtonClickPerformed(InputAction.CallbackContext context)
    {
        hybridHandler.SetInputDataField(InputDataActionType.MouseRightButton, context);
    }
    
    private void OnMouseLeftButtonClickPerformed(InputAction.CallbackContext context)
    {
        hybridHandler.SetInputDataField(InputDataActionType.MouseLeftButton, context);
    }
    
    private void OnMousePositionPerformed(InputAction.CallbackContext context)
    {
        hybridHandler.SetInputDataField(InputDataActionType.MousePos, context);
    }
    
    private void OnMouseLeftButtonClickCanceled(InputAction.CallbackContext context)
    {
        hybridHandler.SetInputDataField(InputDataActionType.MouseLeftButtonCancel, context);
    }
}