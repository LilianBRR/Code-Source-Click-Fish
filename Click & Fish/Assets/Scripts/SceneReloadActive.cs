using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneReloadActive : MonoBehaviour
{
    [SerializeField] private GameObject[] _gameobjects;
    
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (PlayerPrefs.GetInt("can_active") == 1)
        {
            foreach (GameObject g in _gameobjects)
            {
                g.SetActive(true);
            }
        }
    }
}
