using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChangeQuestText : MonoBehaviour
{
    [Header("Dependances")]
    [SerializeField] Dialogue _dialogueOne;
    [SerializeField] TextMeshProUGUI _text;
    
    public void SetText(string newText)
    {
        _text.text = newText;
    }
}
