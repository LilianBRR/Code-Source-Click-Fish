using UnityEngine;
using UnityEngine.UI;
public class SwitchImage : MonoBehaviour
{
    [Header("Dependances")]
    [SerializeField] Dialogue _dialogue;
    [SerializeField] Image _image;
    [SerializeField] Sprite _bear;
    [SerializeField] Sprite _penguin;

    private int _currentIndex;
    private int _lastIndex;

    private void Start()
    {
        _image.sprite = _bear;
        _lastIndex = _dialogue.INDEX;
        _currentIndex = _dialogue.INDEX;
    }

    private void Update()
    {
        _currentIndex = _dialogue.INDEX;
        if( _currentIndex > _lastIndex)
        {
            if (_image.sprite != null && _image.sprite == _bear)
            {
                _image.sprite = _penguin;
            }
            else if (_image.sprite == _penguin)
            {
                _image.sprite = _bear;
            }
            _lastIndex = _currentIndex;
        }
    }

}
