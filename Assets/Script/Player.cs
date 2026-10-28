using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private InputActionReference leftInput;
    [SerializeField] private InputActionReference rightInput;
    [SerializeField] private InputActionReference upInput;
    [SerializeField] private InputActionReference downInput;

    [SerializeField] private float speed = 1.0f;

    private Vector2 direction;

    private void FixedUpdate()
    {
        if (IsMoveInputActive())
        {
            transform.position += (Vector3)direction * Time.deltaTime * speed;
        }
    }


    private bool IsMoveInputActive()
    {
        bool isLeftInput = leftInput.action.ReadValue<float>() > 0;
        bool isRightInput = rightInput.action.ReadValue<float>() > 0;
        bool isUpInput = upInput.action.ReadValue<float>() > 0;
        bool isDownInput = downInput.action.ReadValue<float>() > 0;
        return isLeftInput || isRightInput || isUpInput || isDownInput;
    }

    private void OnLeftInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            direction = Vector2.left;
        }
    }

    private void OnRightInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            direction = Vector2.right;
        }
    }

    private void OnUpInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            direction = Vector2.up;
        }
    }

    private void OnDownInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            direction = Vector2.down;
        }
    }

    private void OnEnable()
    {
        leftInput.action.started += OnLeftInput;
        rightInput.action.started += OnRightInput;
        upInput.action.started += OnUpInput;
        downInput.action.started += OnDownInput;
    }

    private void OnDisable()
    {
        leftInput.action.started -= OnLeftInput;
        rightInput.action.started -= OnRightInput;
        upInput.action.started -= OnUpInput;
        downInput.action.started -= OnDownInput;
    }



}
