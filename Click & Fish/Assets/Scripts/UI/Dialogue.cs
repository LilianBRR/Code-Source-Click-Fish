using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.Events;
public class Dialogue : MonoBehaviour
{
    [Header("Dependency")]
    [SerializeField] private TextMeshProUGUI _dialogueText;
    [SerializeField] private string[] _lines;
    [SerializeField] private float _textSpeed;
    [SerializeField] private InputActionReference _leftClickInput;

    [SerializeField] private int _index;

    public UnityEvent OnDialogueEnded;

    private bool _canClick;
    
    bool _isFinishing;
    
    public int INDEX => _index;

    void Start()
    {
        _canClick = true;
        _leftClickInput.action.started += OnClick;
        _dialogueText.text = string.Empty;
        StartDialogue();
    }
    

    void OnDestroy()
    {
        _leftClickInput.action.started -= OnClick;
    }

    void OnDisable()
    {
        _canClick = false;
    }

    void OnClick(InputAction.CallbackContext obj)
    {
        if (_canClick == false)
            return;
        if (_dialogueText.text == _lines[_index])
        {
            NextLine();
        }
        else
        {
            StopAllCoroutines();
            _dialogueText.text = _lines[_index];
        }
    }

    void StartDialogue()
    {
        _index = 0;
        StartCoroutine(TypeLine());

    }

    IEnumerator TypeLine()
    {
        foreach (char c in _lines[_index].ToCharArray())
        {
            _dialogueText.text += c;
            yield return new WaitForSeconds(_textSpeed);
        }
            
    }
    
    void NextLine()
    {
        if (_index < _lines.Length - 1)
        {
            _index++;
            _dialogueText.text = String.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            _isFinishing = true;
            OnDialogueEnded?.Invoke();
            gameObject.SetActive(false);
        }
    }
   
}
