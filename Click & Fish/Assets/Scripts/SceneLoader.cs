using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneLoader : MonoBehaviour
{
    public void ReloadCurrentScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
        UpdateScore.Instance.AddScoreToReach(5);
    }

    public void ResetScene()
    {
        PlayerPrefs.DeleteKey("first_interact_dialogue");
        PlayerPrefs.DeleteKey("can_active");
        
        if (UpdateScore.Instance != null)
        {
            UpdateScore.Instance.ResetFullState();
        }
        SceneManager.LoadScene("Game");
    }
}
