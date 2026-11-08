using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    private bool _isInteracting;
    private bool _isFishing = false;

    [Header("Cameras")]
    [SerializeField] private GameObject _secondCamera;
    [SerializeField] private GameObject _mainCamera;

    [Header("Grid Object")]
    [SerializeField] private GameObject[] _fish;

    [Header("Movement Control")]
    [SerializeField] private MonoBehaviour _playerMovementScript;

    private void Start()
    {
        foreach (GameObject fish in _fish)
        {
            if (fish != null)
            {
                if (fish.TryGetComponent<CameraGridMover>(out var gridMover))
                {
                    gridMover.OnFinish.AddListener(ExitFishing);
                }
                fish.SetActive(false);
            }
        }
        
    }

    public void SetInteractInput(bool value)
    {
        _isInteracting = value;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<InteractTag>(out var interactTag))
        {
            interactTag.SetInputGraphicActive(true);
            if (_isInteracting)
            {
                interactTag.ActivateGameObject();
            }
        }

        if (other.CompareTag("FishingZone") && _isInteracting)
        {
            if (!_isFishing)
            {
                StartFishing();
            }
            _isInteracting = false;
        }
    }

    public void StartFishing()
    {
        _isFishing = true;
        ApplyFishingState();
    }

    public void ExitFishing()
    {
        _isFishing = false;
        ApplyFishingState();
    }

    private void ApplyFishingState()
    {
        if (_secondCamera != null) _secondCamera.SetActive(_isFishing);
        if (_mainCamera != null) _mainCamera.SetActive(!_isFishing);

        SetMovementEnabled(!_isFishing);
    }

    private void SetMovementEnabled(bool isEnabled)
    {
        if (_playerMovementScript != null)
            _playerMovementScript.enabled = isEnabled;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<InteractTag>(out var interactTag))
        {
            interactTag.SetInputGraphicActive(false);
        }
    }
}