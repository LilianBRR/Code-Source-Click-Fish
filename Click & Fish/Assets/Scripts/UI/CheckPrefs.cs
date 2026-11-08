using System;
using UnityEngine;

public class CheckPrefs : MonoBehaviour
{
    [SerializeField] string[] _tags;
    [SerializeField] GameObject[] _images;

    private void Start()
    {
        CheckPlayerPrefs();
    }

    public void CheckPlayerPrefs()
    {
        for (int i = 0; i < 9; i++)
        {
            if (PlayerPrefs.GetInt(_tags[i]) == 1)
            {
                _images[i].SetActive(true);
            }
        } 
    }
}
