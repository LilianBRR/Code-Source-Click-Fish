using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class UpdateScore : MonoBehaviour
{
    public static UpdateScore Instance { get; private set; }
    [Header("Scoring")]
    [SerializeField] private int _score;
    [SerializeField] private int _scoreToReach;
    [SerializeField] private int _defaultScoreToReach;
    [SerializeField] private int _maxScore;
    
    [Header("Dependances")]
    public int SCORE => _score;
    public int SCORE_TO_REACH => _scoreToReach;
    public UnityEvent OnScoreReached;

    private void Start()
    {
        _defaultScoreToReach = 5;
        _scoreToReach =  _defaultScoreToReach;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
    }

    public void AddScore()
    {
        _score++;
        if (_score >= _scoreToReach)
        {
            OnScoreReached?.Invoke();
        }
    }

    public void AddScoreToReach(int value)
    { 
        _scoreToReach += value;
        if (_scoreToReach >= _maxScore)
        {
            _scoreToReach = _maxScore;   
        }
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _score = 0;
    }

    public void ResetFullState()
    {
        _score = 0;
        _scoreToReach = _defaultScoreToReach;
    }
}
