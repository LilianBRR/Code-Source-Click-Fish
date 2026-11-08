using UnityEngine;
using UnityEngine.Events;
public class DisableOnce : MonoBehaviour
{
    [SerializeField] private string _key = "";
    
    private void Awake()
    {
        if (PlayerPrefs.GetInt(_key) == 1)
        {
            gameObject.SetActive(false);
        }
    }
    public void CompleteDisable()
    {
        PlayerPrefs.SetInt(_key, 1);
        PlayerPrefs.SetInt("can_active", 1);
        gameObject.SetActive(false);
    }
}
