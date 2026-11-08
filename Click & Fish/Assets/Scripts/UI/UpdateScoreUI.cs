using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UpdateScoreUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _text;
    void Update()
    {
        UpdateUI();
    }
    void UpdateUI()
    {
        _text.text = UpdateScore.Instance.SCORE.ToString() + " / " + UpdateScore.Instance.SCORE_TO_REACH.ToString();
    }
}
