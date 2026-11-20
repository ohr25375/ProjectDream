using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private InputActionReference inventoryInput;
    [SerializeField] private Transform inventoryUI;
    [SerializeField] private float openY =0;
    [SerializeField] private float closeY =0;
    [SerializeField] private float moveSpeed=0;

    private Vector3 targetPosition;
    private bool isOpen = false;
    private bool isMoving = false;
    private bool canOpenInventory = true;
    private void Start()
    {
        targetPosition = inventoryUI.localPosition;
    }
    private void Update()
    {
            inventoryUI.localPosition = Vector3.MoveTowards(inventoryUI.localPosition,targetPosition,moveSpeed * Time.deltaTime);
           
        if (inventoryUI.localPosition == targetPosition)
        {
            isMoving = false;
        }
    }

    private void OnInventoryInput(InputAction.CallbackContext context)
    {
        if (!canOpenInventory) return;
        if (isMoving) return;
        if (isOpen) CloseInventory();
        else OpenInventory();

        isOpen = !isOpen;
    }

    private void OnEnable()
    {
        inventoryInput.action.started += OnInventoryInput;
    }

    private void OnDisable()
    {
        inventoryInput.action.started -= OnInventoryInput;
    }

    void OpenInventory()
    {
        isMoving = true;
        targetPosition = new Vector3(inventoryUI.localPosition.x,openY,inventoryUI.localPosition.z);
    }

    void CloseInventory()
    {
        isMoving = true;
        targetPosition = new Vector3(inventoryUI.localPosition.x,closeY,inventoryUI.localPosition.z);
    }
}