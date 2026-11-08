using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    [SerializeField] private RectTransform _cursorUI;
    [SerializeField] private float _cursorSpeed = 900f;
    
    public void Play()
    {
        PlayerPrefs.DeleteKey("first_interact_dialogue");
        PlayerPrefs.DeleteKey("can_active");
        
        if (UpdateScore.Instance != null)
        {
            UpdateScore.Instance.ResetFullState();
        }
        SceneManager.LoadScene("Game");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
