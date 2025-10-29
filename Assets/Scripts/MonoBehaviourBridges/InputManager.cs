using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public enum InputDataActionType
{
    Move,
    MousePos,
    MouseLeftButton,
    MouseRightButton,
    MouseLeftButtonCancel
}

public class InputManager : MonoBehaviour
{
    [Inject] private NewInputActions inputActions;

    private void Update()
    {
        SetInputDataField(InputDataActionType.Move);
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
        SetInputDataField(InputDataActionType.MouseRightButton, context);
    }

    private void OnMouseLeftButtonClickPerformed(InputAction.CallbackContext context)
    {
        SetInputDataField(InputDataActionType.MouseLeftButton, context);
    }

    private void OnMousePositionPerformed(InputAction.CallbackContext context)
    {
        SetInputDataField(InputDataActionType.MousePos, context);
    }

    private void OnMouseLeftButtonClickCanceled(InputAction.CallbackContext context)
    {
        SetInputDataField(InputDataActionType.MouseLeftButtonCancel, context);
    }

    private void SetInputDataField(InputDataActionType inputAction, InputAction.CallbackContext context = default)
    {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        var entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<InputData>().Build(entityManager);
        var entityArray = entityQuery.ToEntityArray(Allocator.Temp);

        var inputDataArray = entityQuery.ToComponentDataArray<InputData>(Allocator.Temp);

        for (var i = 0; i < entityArray.Length; i++)
        {
            var inputData = inputDataArray[i];
            switch (inputAction)
            {
                case InputDataActionType.Move:
                    inputData.Movement = inputActions.GameMap.Move.ReadValue<Vector2>();
                    break;
                case InputDataActionType.MousePos:
                    var inputMousePosition = context.ReadValue<Vector2>();
                    inputData.MousePos =
                        Camera.main.ScreenToWorldPoint(new Vector3(inputMousePosition.x, inputMousePosition.y, 10));
                    break;
                case InputDataActionType.MouseLeftButton:
                    inputData.MouseLeft = true;
                    break;
                case InputDataActionType.MouseRightButton:
                    inputData.MouseRight = true;
                    break;
                case InputDataActionType.MouseLeftButtonCancel:
                    inputData.MouseLeft = false;
                    break;
            }

            entityManager.SetComponentData(entityArray[i], inputData);
        }
    }
}