using UnityEngine;

public class WaterMovement : MonoBehaviour
{
    [SerializeField] private float _speed = 1f;
    [SerializeField] private float _maxMovement = 1f;

    private Vector3 _startPosition;

    void Start()
    {
        _startPosition = transform.position;
    }

    void Update()
    {
        float offset = Mathf.PingPong(Time.time * _speed, _maxMovement);
        transform.position = _startPosition + new Vector3(0f, offset, 0f);
    }
}
