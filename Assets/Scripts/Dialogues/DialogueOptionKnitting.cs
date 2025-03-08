using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialogueOptionKnitting : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI _optionText;

    private DialogueManager _parentDialogueManager;
    private int _id = 0;

    public void MakeOption(string optionText, int id, DialogueManager parentDialogueManager)
    {
        _parentDialogueManager = parentDialogueManager;
        _optionText.text = optionText;
        _id = id;
    }

    private void OnClick()
    {
        _parentDialogueManager.OptionSelected(_id);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick();
    }
}
