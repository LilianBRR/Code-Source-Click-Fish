using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    [SerializeField] private InputActionReference _restartInput;
    [SerializeField] private InputActionReference _pauseInput;
    [SerializeField] private SceneLoader _sceneLoader;
    [SerializeField] private GameObject _pauseMenuUI;
    [SerializeField] private GameObject _gameplayUI;
    [SerializeField] private GameObject _firstSelectedButton;
    
    bool _isPaused = false;
    void Start()
    {
        _restartInput.action.started += OnRestart;
        _restartInput.action.canceled += OnRestart;
        _pauseInput.action.started += TogglePause;
    }

    void OnDestroy()
    {
        _restartInput.action.started -= OnRestart;
        _restartInput.action.canceled -= OnRestart;
        _pauseInput.action.started -= TogglePause;
    }

    void OnRestart(InputAction.CallbackContext obj)
    {
        _sceneLoader.ReloadCurrentScene();
    }
    
    void TogglePause(InputAction.CallbackContext obj)
    {
        if (_isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }
    
    public void PauseGame()
    {
        _gameplayUI.SetActive(false);
        _pauseMenuUI.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(_firstSelectedButton);
        Time.timeScale = 0f;
        _isPaused = true;
        
    }

    public void ResumeGame()
    {
        _pauseMenuUI.SetActive(false);
        _gameplayUI.SetActive(true);
        Time.timeScale = 1f;      
        _isPaused = false;
    }
    
    private void OnApplicationQuit()
    {
        PlayerPrefs.DeleteKey("first_interact_dialogue");
        PlayerPrefs.DeleteKey("can_active");
    }
    
}
