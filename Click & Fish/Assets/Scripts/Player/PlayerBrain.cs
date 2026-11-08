using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBrain : MonoBehaviour
{
    [Header("Dependances")]
    [SerializeField] InputActionReference _moveInput;
    [SerializeField] InputActionReference _sprintInput;
    [SerializeField] InputActionReference _interactInput;
    
    [Space(5)]
    
    [SerializeField] PlayerMovement _playerMovement;
    [SerializeField] PlayerInteractor _playerInteractor;

    Vector3 _joystickDirection;


    void Start()
    {
        _moveInput.action.started += OnMove;
        _moveInput.action.performed += OnMove;
        _moveInput.action.canceled += OnMoveCancel;
        
        _sprintInput.action.performed += OnSprint;
        _sprintInput.action.canceled += OnSprintCancel;
        
        _interactInput.action.started += OnInteract;
        _interactInput.action.canceled += OnInteractCancel;
    }

    void OnDestroy()
    {
        _moveInput.action.started -= OnMove;
        _moveInput.action.performed -= OnMove;
        _moveInput.action.canceled -= OnMoveCancel;

        _sprintInput.action.performed -= OnSprint;
        _sprintInput.action.canceled -= OnSprintCancel;
        
        _interactInput.action.started -= OnInteract;  
        _interactInput.action.canceled -= OnInteractCancel;
    }

    void OnMove(InputAction.CallbackContext obj)
    {
        Vector2 inputDir = obj.ReadValue<Vector2>();
        _playerMovement.SetMoveInput(inputDir);
    }
    void OnMoveCancel(InputAction.CallbackContext obj)
    {
        _playerMovement.SetMoveInput(new Vector2(0, 0));
    }

    void OnSprint(InputAction.CallbackContext obj)
    {
        _playerMovement.SetIsSprinting(true);
    }

    void OnSprintCancel(InputAction.CallbackContext obj)
    {
        _playerMovement.SetIsSprinting(false);
    }

    void OnInteract(InputAction.CallbackContext obj)
    {
        _playerInteractor.SetInteractInput(true);
    }

    void OnInteractCancel(InputAction.CallbackContext obj)
    {
        _playerInteractor.SetInteractInput(false);
    }
    
}
