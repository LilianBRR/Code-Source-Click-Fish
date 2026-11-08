using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Dependances")]
    [SerializeField] Camera _camera;

    [Header("Variable")]
    [SerializeField] float _speed;
    [SerializeField] float _coefSprint;
    [SerializeField] float _initalSpeed;
    
    [Space(10)]
    [SerializeField] Animator _animator;

    Vector3 _moveInputDirection;

    void Start()
    {
        _speed = _initalSpeed;
        _initalSpeed = _speed;
    }

    void Update()
    {
        Vector3 f = _camera.transform.forward;
        f.y = 0f;
        var r = _camera.transform.right;
        var dir = (f * _moveInputDirection.z) + (r * _moveInputDirection.x);
        transform.Translate(dir * (_speed * Time.deltaTime), Space.World);

        if (dir.sqrMagnitude > 0.01f)
        {
            
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 15f);
        }

        if (_moveInputDirection != Vector3.zero)
        {
            _animator.SetBool("IsMoving", true);
        }
        else
        {
            _animator.SetBool("IsMoving", false);
        }
    }
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<OceanTag>()!= null)
        {
            _animator.SetTrigger("IsSwimming");
        } 
    }

    void OnCollisionExit(Collision other)
    {
        if (other.gameObject.GetComponent<OceanTag>() != null)
        {
            _animator.SetTrigger("Moving");
        }
    }
     

    public void SetMoveInput(Vector2 dir)
    {
        _moveInputDirection = new Vector3(dir.x, 0f, dir.y);
        
    }

    public void SetIsSprinting(bool value)
    {
        if (value)
        {
            _speed *= _coefSprint;
        }
        else
        {
            _speed = _initalSpeed;
        }
    }
}