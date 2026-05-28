using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class OpenInventory : MonoBehaviour
{
    [SerializeField] private Canvas inventoryUI;
    [SerializeField] private InputActionReference inventoryAction;
    [SerializeField] private GameObject firstSelected;

    private void Start()
    {
        inventoryUI.enabled = false;
    }

    private void OnEnable()
    {
        inventoryAction.action.Enable();
        inventoryAction.action.performed += OnOpen;
    }

    private void OnDisable()
    {
        inventoryAction.action.performed -= OnOpen;
        inventoryAction.action.Disable();
    }

    private void OnOpen(InputAction.CallbackContext context)
    {

        inventoryUI.enabled = !inventoryUI.enabled;
        Cursor.visible = inventoryUI.enabled;
        Cursor.lockState = inventoryUI.enabled ? CursorLockMode.None : CursorLockMode.Locked;

        FindAnyObjectByType<EventSystem>().SetSelectedGameObject(firstSelected);
    }

}
