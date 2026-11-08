using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class SwitchImageController : MonoBehaviour
{
    
    [SerializeField] private Image _image;
    [SerializeField] private Sprite _keyboardSprite;
    [SerializeField] private Sprite _gamepadSprite;
    
    private void OnEnable()
    {
        InputSystem.onActionChange += OnActionChange;
    }

    private void OnDisable()
    {
        InputSystem.onActionChange -= OnActionChange;
    }

    private void OnActionChange(object obj, InputActionChange change)
    {
        if (change == InputActionChange.ActionPerformed)
        {
            InputAction action = (InputAction)obj;
            InputControl lastControl = action.activeControl;

            if (lastControl == null) return;
            
            if (lastControl.device is Gamepad)
            {
                SetSprite(_gamepadSprite);
            }
            else if (lastControl.device is Keyboard || lastControl.device is Mouse)
            {
                SetSprite(_keyboardSprite);
            }
        }
    }
    
    void SetSprite(Sprite sprite)
    {
        _image.sprite = sprite;
    }
}
