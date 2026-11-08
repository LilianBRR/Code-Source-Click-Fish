
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
public class InputGamepadButton : MonoBehaviour
{
    [SerializeField] private InputActionReference _bookInput;
    [SerializeField] private Button _interactionButton;
    
    private void OnEnable()
    {
        _bookInput.action.performed += OnBookInput;
        _bookInput.action.Enable();
    }

    private void OnDisable()
    {
        _bookInput.action.performed -= OnBookInput;
    }

    private void OnBookInput(InputAction.CallbackContext ctx)
    {
        if (_interactionButton != null && _interactionButton.interactable)
        {
            _interactionButton.onClick.Invoke();
        }
    }
}
