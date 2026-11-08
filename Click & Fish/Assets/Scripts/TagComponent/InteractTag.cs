using UnityEngine;
using UnityEngine.Events;

public class InteractTag : MonoBehaviour
{
    [Header("Dependency")]
    [SerializeField] private GameObject _inputGraphic;
    [SerializeField] private Transform _cameraTransform;

    public UnityEvent OnInteraction;
    void Start()
    {
        _inputGraphic.SetActive(false);
    }

    public void SetInputGraphicActive(bool isActive)
    {
        _inputGraphic.SetActive(isActive);
    }

    public void ActivateGameObject()
    {
        OnInteraction?.Invoke();
    }

    void Update()
    {
        if (_inputGraphic.activeSelf)
        {
            _inputGraphic.transform.LookAt(_cameraTransform);
        }
    }
}
