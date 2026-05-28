using NUnit.Framework.Internal;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class IteractStelle : MonoBehaviour, IInteractable
{

    [Header("Text")]
    [SerializeField] private string text;

    [Header("UIPanel")]
    [SerializeField] private Canvas stelleTextPanel;
    [SerializeField] private TextMeshProUGUI _text;

    private void Start()
    {
        stelleTextPanel.enabled = false;
    }

    private void Reset()
    {
        _text = stelleTextPanel.GetComponent<TextMeshProUGUI>();
    }

    public void Interact()
    {
        _text.text = text;
        stelleTextPanel.enabled = !stelleTextPanel.enabled;
        Cursor.visible = stelleTextPanel.enabled;
        Cursor.lockState = stelleTextPanel.enabled ? CursorLockMode.None : CursorLockMode.Locked;
    }
    public void CloseInteract()
    {
        stelleTextPanel.enabled = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
