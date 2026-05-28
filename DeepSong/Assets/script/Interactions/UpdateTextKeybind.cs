using UnityEngine;
using UnityEngine.InputSystem;

public class UpdateTextKeybind : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI promptText;
    [SerializeField] private InputActionReference interactAction;

    private void Awake()
    {
        UpdateText();
    }
    public void UpdateText()
    {
        string keyDisplay = interactAction.action.GetBindingDisplayString(0);

        promptText.text = $"**{keyDisplay}** to continue";
    }
}
