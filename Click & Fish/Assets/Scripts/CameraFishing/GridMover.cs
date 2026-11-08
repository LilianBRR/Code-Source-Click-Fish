using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public enum GameMode
{
    ClickJump, // Mode 1: Direct click on target
    Tracking   // Mode 2: Follow target with cursor
}

public class CameraGridMover : MonoBehaviour
{
    [System.Serializable]
    public struct FishModelBinding
    {
        public FishingCollection.FISH fishType;
        public GameObject modelObject; // Child object present in the Hierarchy under "Model"
    }

    [Header("Game Mode")]
    [SerializeField] private GameMode _gameMode = GameMode.ClickJump;

    [Header("Debug Settings")]
    [Tooltip("Check to force a specific fish instead of random selection")]
    [SerializeField] private bool _overrideFishSelection = false;
    [SerializeField] private FishingCollection.FISH _debugFishToSpawn = FishingCollection.FISH.COMMON_GREY;

    [Header("Session Settings")]
    [SerializeField] private int _maxAttempts = 5; // Number of attempts per session

    [Header("Zone Reference")]
    [Tooltip("Drag the 'Fishing Zone 1' GameObject here. If empty, the script automatically picks the parent.")]
    [SerializeField] private GameObject _fishingZone;

    [Header("Collection & Models")]
    [SerializeField] private FishingCollection _fishingCollection;
    [SerializeField] private FishModelBinding[] _fishModels;

    [Header("Rotation Smoothness")]
    [Tooltip("Progressive fish rotation speed")]
    [SerializeField] private float _rotationSpeed = 15f;

    [Header("Camera Settings")]
    [SerializeField] private Camera _gridCamera;
    [SerializeField] private float _distanceFromCamera = 5f;

    [Header("Grid Settings")]
    [SerializeField] private int _columns = 4;
    [SerializeField] private int _rows = 3;

    [Header("Jump & Water Settings")]
    [SerializeField] private float _underwaterDepth = 2f;
    [SerializeField] private float _jumpHeight = 3f;
    [Tooltip("Vertical water surface offset relative to grid cells")]
    [SerializeField] private float _waterSurfaceOffset = 0f;

    [Header("Splash Effects")]
    [Tooltip("Visual effect when fish exits the water")]
    [SerializeField] private GameObject _exitWaterSplash;
    [Tooltip("Visual effect when fish re-enters the water")]
    [SerializeField] private GameObject _enterWaterSplash;
    [SerializeField] private float _splashDestroyDelay = 2f;

    [Header("Timer Settings")]
    [SerializeField] private float _timeLimit = 3f;

    [Header("Tracking Mode Settings")]
    [Range(0.1f, 1f)]
    [SerializeField] private float _requiredTrackingRatio = 0.5f;
    [Tooltip("Particle system triggered when cursor hovers over the fish in Tracking mode")]
    [SerializeField] private ParticleSystem _trackingParticles;

    [Header("Controller Settings")]
    [SerializeField] private RectTransform _cursorUI;
    [SerializeField] private float _cursorSpeed = 900f;

    [Header("Events")]
    public UnityEvent OnSuccessfulAction; // Triggered on each successful attempt (score)
    public UnityEvent OnFinish;           // Triggered when session ends

    private Vector2 _virtualCursorPos;
    private bool _isUsingGamepad = false;

    private Vector3[] _gridPositions;

    private int _currentIndex = -1;
    private Vector3 _baseCellPosition;

    private Vector3 _startCellPosition;
    private Vector3 _endCellPosition;
    private float _trackedTime;

    private float _currentTimer;
    private bool _isGameActive;

    private int _currentAttemptCount;

    private Vector3 _lastPosition;

    private bool _wasUnderwater = true;
    private FishingCollection.FISH _currentFish = FishingCollection.FISH.NONE;

    private void Awake()
    {
        if (_fishingZone == null && transform.parent != null)
        {
            _fishingZone = transform.parent.gameObject;
        }

        if (_fishingCollection == null)
        {
            _fishingCollection = GetComponent<FishingCollection>();
        }
    }

    private void OnEnable()
    {
        _columns = Mathf.Max(1, _columns);
        _rows = Mathf.Max(1, _rows);

        _gridPositions = new Vector3[_columns * _rows];

        if (_gridCamera == null) _gridCamera = Camera.main;

        if (_gridCamera != null && Application.isPlaying)
        {
            _virtualCursorPos = new Vector2(Screen.width / 2f, Screen.height / 2f);

            if (_cursorUI != null)
            {
                _cursorUI.position = _virtualCursorPos;
                _cursorUI.gameObject.SetActive(false);
            }

            RecalculateGrid();
            RestartGame();
        }
    }

    private void OnDisable()
    {
        _isGameActive = false;
        StopTrackingParticles();

        Cursor.visible = true;
        if (_cursorUI != null)
        {
            _cursorUI.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        if (UpdateScore.Instance != null)
        {
            OnSuccessfulAction?.AddListener(UpdateScore.Instance.AddScore);
        }
    }

    private void Update()
    {
        if (_gridCamera == null || !_isGameActive || !Application.isPlaying) return;

        HandleInputs();
        UpdateTargetPosition();

        if (_gameMode == GameMode.Tracking)
        {
            CheckCursorTracking();
        }

        HandleTimer();
    }

    private void HandleInputs()
    {
        if (Mouse.current != null)
        {
            bool mouseMoved = Mouse.current.delta.ReadValue().sqrMagnitude > 0.1f;
            bool mouseClicked = Mouse.current.leftButton.wasPressedThisFrame;

            if (mouseMoved || mouseClicked) SwitchToMouse();

            if (mouseClicked && _gameMode == GameMode.ClickJump)
            {
                TryClickAtScreenPoint(Mouse.current.position.ReadValue());
            }
        }

        if (Gamepad.current != null)
        {
            Vector2 stickInput = Gamepad.current.leftStick.ReadValue();
            bool stickMoved = stickInput.sqrMagnitude > 0.01f;
            bool buttonPressed = Gamepad.current.buttonSouth.wasPressedThisFrame;

            if (stickMoved || buttonPressed) SwitchToGamepad();

            if (_isUsingGamepad)
            {
                if (stickMoved)
                {
                    _virtualCursorPos += stickInput * _cursorSpeed * Time.deltaTime;
                    _virtualCursorPos.x = Mathf.Clamp(_virtualCursorPos.x, 0, Screen.width);
                    _virtualCursorPos.y = Mathf.Clamp(_virtualCursorPos.y, 0, Screen.height);

                    if (_cursorUI != null)
                        _cursorUI.position = _virtualCursorPos;
                }

                if (buttonPressed && _gameMode == GameMode.ClickJump)
                {
                    TryClickAtScreenPoint(_virtualCursorPos);
                }
            }
        }
    }

    private void UpdateTargetPosition()
    {
        float progress = 1f - (_currentTimer / _timeLimit);
        float verticalOffset = Mathf.Sin(progress * Mathf.PI) * (_jumpHeight + _underwaterDepth) - _underwaterDepth;

        Vector3 surfacePos = Vector3.zero;
        Vector3 targetPos = Vector3.zero;

        if (_gameMode == GameMode.ClickJump)
        {
            surfacePos = _baseCellPosition;
            targetPos = _baseCellPosition + (Vector3.up * verticalOffset);
        }
        else if (_gameMode == GameMode.Tracking)
        {
            surfacePos = Vector3.Lerp(_startCellPosition, _endCellPosition, progress);
            targetPos = surfacePos + (Vector3.up * verticalOffset);
        }

        transform.position = targetPos;

        Vector3 moveDirection = targetPos - _lastPosition;
        if (moveDirection.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);
        }
        _lastPosition = targetPos;

        // --- Water surface threshold detection ---
        Vector3 splashWorldPos = surfacePos + Vector3.up * _waterSurfaceOffset;

        if (_wasUnderwater && verticalOffset >= _waterSurfaceOffset)
        {
            _wasUnderwater = false;
            SpawnSplash(_exitWaterSplash, splashWorldPos);
        }
        else if (!_wasUnderwater && verticalOffset < _waterSurfaceOffset)
        {
            _wasUnderwater = true;
            SpawnSplash(_enterWaterSplash, splashWorldPos);
        }
    }

    private void SpawnSplash(GameObject splashPrefab, Vector3 position)
    {
        if (splashPrefab == null) return;

        GameObject splash = Instantiate(splashPrefab, position, splashPrefab.transform.rotation);
        Destroy(splash, _splashDestroyDelay);
    }

    private void CheckCursorTracking()
    {
        Vector2 activeCursorPos = _isUsingGamepad
            ? _virtualCursorPos
            : (Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero);

        Ray ray = _gridCamera.ScreenPointToRay(activeCursorPos);

        bool isHovered = Physics.Raycast(ray, out RaycastHit hit) && (hit.transform == transform || hit.transform.IsChildOf(transform));

        if (isHovered)
        {
            _trackedTime += Time.deltaTime;

            if (_trackingParticles != null && !_trackingParticles.isPlaying)
            {
                _trackingParticles.Play();
            }
        }
        else
        {
            StopTrackingParticles();
        }
    }

    private void StopTrackingParticles()
    {
        if (_trackingParticles != null && _trackingParticles.isPlaying)
        {
            _trackingParticles.Stop();
        }
    }

    private void HandleTimer()
    {
        _currentTimer -= Time.deltaTime;

        if (_currentTimer <= 0f)
        {
            _currentTimer = 0f;

            if (_gameMode == GameMode.Tracking)
            {
                float ratio = _trackedTime / _timeLimit;
                if (ratio >= _requiredTrackingRatio)
                {
                    RegisterSuccess();
                }
            }

            AdvanceToNextAttempt();
        }
    }

    private void TryClickAtScreenPoint(Vector2 screenPos)
    {
        Ray ray = _gridCamera.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform == transform || hit.transform.IsChildOf(transform))
            {
                RegisterSuccess();
                AdvanceToNextAttempt();
            }
        }
    }

    private void RegisterSuccess()
    {
        if (_fishingCollection != null && _currentFish != FishingCollection.FISH.NONE)
        {
            _fishingCollection.Reward();
        }

        OnSuccessfulAction?.Invoke();
    }

    private void AdvanceToNextAttempt()
    {
        _currentAttemptCount++;

        if (_currentAttemptCount < _maxAttempts)
        {
            MoveToNextTarget();
        }
        else
        {
            FinishGame();
        }
    }

    private void FinishGame()
    {
        _isGameActive = false;
        StopTrackingParticles();

        OnFinish?.Invoke();

        if (_fishingZone != null)
        {
            _fishingZone.SetActive(false);
            _gridCamera.gameObject.SetActive(false);   
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void ApplyFishModel(FishingCollection.FISH fish)
    {
        if (_fishModels == null) return;

        foreach (var binding in _fishModels)
        {
            if (binding.modelObject != null)
            {
                bool isTarget = (binding.fishType == fish);
                binding.modelObject.SetActive(isTarget);
            }
        }
    }

    private void SwitchToMouse()
    {
        if (!_isUsingGamepad) return;

        _isUsingGamepad = false;
        Cursor.visible = true;

        if (_cursorUI != null)
            _cursorUI.gameObject.SetActive(false);
    }

    private void SwitchToGamepad()
    {
        if (_isUsingGamepad && _cursorUI != null && _cursorUI.gameObject.activeSelf) return;

        _isUsingGamepad = true;
        Cursor.visible = false;

        if (_cursorUI != null)
        {
            _cursorUI.gameObject.SetActive(true);
            _cursorUI.position = _virtualCursorPos;
        }
    }

    public void RecalculateGrid()
    {
        float cellWidth = 1f / _columns;
        float cellHeight = 1f / _rows;

        for (int row = 0; row < _rows; row++)
        {
            for (int col = 0; col < _columns; col++)
            {
                int index = row * _columns + col;
                float viewportX = (col + 0.5f) * cellWidth;
                float viewportY = (row + 0.5f) * cellHeight;

                Vector3 viewportPoint = new Vector3(viewportX, viewportY, _distanceFromCamera);
                _gridPositions[index] = _gridCamera.ViewportToWorldPoint(viewportPoint);
            }
        }
    }

    public void MoveToNextTarget()
    {
        if (_gridPositions == null || _gridPositions.Length == 0) return;

        StopTrackingParticles();

        _trackedTime = 0f;
        _currentTimer = _timeLimit;
        _wasUnderwater = true;

        if (_gameMode == GameMode.ClickJump)
        {
            int nextIndex = _currentIndex;
            if (_gridPositions.Length > 1)
            {
                while (nextIndex == _currentIndex)
                    nextIndex = Random.Range(0, _gridPositions.Length);
            }
            else nextIndex = 0;

            _currentIndex = nextIndex;
            _baseCellPosition = _gridPositions[_currentIndex];

            Vector3 startPos = _baseCellPosition - (Vector3.up * _underwaterDepth);
            transform.position = startPos;
            _lastPosition = startPos - (Vector3.up * 0.05f);
        }
        else if (_gameMode == GameMode.Tracking)
        {
            int startIndex = Random.Range(0, _gridPositions.Length);
            int endIndex = startIndex;

            if (_gridPositions.Length > 1)
            {
                while (endIndex == startIndex)
                    endIndex = Random.Range(0, _gridPositions.Length);
            }

            _startCellPosition = _gridPositions[startIndex];
            _endCellPosition = _gridPositions[endIndex];

            Vector3 startPos = _startCellPosition - (Vector3.up * _underwaterDepth);
            transform.position = startPos;
            _lastPosition = startPos - (Vector3.up * 0.05f);
        }

        Vector3 initialDir = transform.position - _lastPosition;
        if (initialDir.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(initialDir);
        }

        UpdateTargetPosition();
    }

    public void RestartGame()
    {
        _isGameActive = true;
        _currentAttemptCount = 0;

        if (_overrideFishSelection)
        {
            _currentFish = _debugFishToSpawn;
            Debug.Log($"[Fishing Debug] Forced fish: {_currentFish}");
            ApplyFishModel(_currentFish);
        }
        else if (_fishingCollection != null)
        {
            _currentFish = _fishingCollection.GetFish();
            Debug.Log($"[Fishing] Selected fish: {_currentFish}");
            ApplyFishModel(_currentFish);
        }

        MoveToNextTarget();
    }

    private void OnValidate()
    {
        if (!Application.isPlaying && _overrideFishSelection)
        {
            ApplyFishModel(_debugFishToSpawn);
        }
    }

    private void OnDrawGizmos()
    {
        Camera cam = _gridCamera != null ? _gridCamera : Camera.main;
        if (cam == null) return;

        float cellWidth = 1f / _columns;
        float cellHeight = 1f / _rows;

        for (int row = 0; row < _rows; row++)
        {
            for (int col = 0; col < _columns; col++)
            {
                float viewportX = (col + 0.5f) * cellWidth;
                float viewportY = (row + 0.5f) * cellHeight;
                Vector3 surfacePos = cam.ViewportToWorldPoint(new Vector3(viewportX, viewportY, _distanceFromCamera));

                Vector3 waterSurfacePos = surfacePos + Vector3.up * _waterSurfaceOffset;

                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(surfacePos - Vector3.up * _underwaterDepth, 0.08f);
                Gizmos.DrawLine(surfacePos - Vector3.up * _underwaterDepth, surfacePos + Vector3.up * _jumpHeight);
                Gizmos.DrawWireSphere(surfacePos + Vector3.up * _jumpHeight, 0.12f);

                Gizmos.color = Color.blue;
                Gizmos.DrawWireCube(waterSurfacePos, new Vector3(0.3f, 0.01f, 0.3f));
            }
        }
    }
}