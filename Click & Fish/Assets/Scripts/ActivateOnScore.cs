using UnityEngine;

public class ActivateOnScore : MonoBehaviour
{
    [SerializeField] private GameObject _targetToActivate;
    private void Start()
    {
        if (UpdateScore.Instance != null)
        {
            UpdateScore.Instance.OnScoreReached.AddListener(OnScoreConditionMet);
        }
    }

    private void OnScoreConditionMet()
    {
        _targetToActivate.SetActive(true);
    }

    private void OnDestroy()
    {
        if (UpdateScore.Instance != null)
        {
            UpdateScore.Instance.OnScoreReached.RemoveListener(OnScoreConditionMet);
        }
    }
}
