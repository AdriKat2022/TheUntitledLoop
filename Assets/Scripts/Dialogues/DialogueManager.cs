using AdriKat.Toolkit.Attributes;
using AdriKat.Toolkit.CodePatterns;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueManager : Singleton<DialogueManager>
{
    [Header("Default Behaviour")]
    [SerializeField] private bool _resetStoryOnDialogueStart = true;

    [Header("References")]
    [SerializeField] private GameObject _titleTextContainer;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _mainText;
    [Space]
    [SerializeField] private InputActionReference _continueAction;
    [Space]
    [SerializeField] private CanvasGroup _optionsPanel;
    [SerializeField] private DialogueOptionKnitting _optionPrefab;

    [Space]

    [Header("Animations")]
    [SerializeField] private float _timeBeforeFirstText;
    [SerializeField] private bool _useAnimations;
    [ShowIf(nameof(_useAnimations))]
    [SerializeField] private Animator _dialogueBoxAnimator;
    [ShowIf(nameof(_useAnimations))]
    [SerializeField] private string _visibleBoolDialogueBoxKey;

    private RectTransform _rectTransform;
    private int _optionSelected = 0;
    private bool _isOptionSelected = false;
    private bool _continueFlag = false;
    private bool _isInDialogue = false;

    private string isHappy = "false";
    private string isOpen = "false";
    private string isInside = "false";

    [Header("Other vars")]
    [SerializeField] GameObject door;
    [SerializeField] GameObject guard;
    [SerializeField] GameObject[] eyes;
    [SerializeField] AudioSource abouament;

    #region Input Management
    private void OnEnable()
    {
        _continueAction.action.Enable();
        _continueAction.action.performed += ContinueAction_performed;
    }

    private void OnDisable()
    {
        _continueAction.action.Disable();
        _continueAction.action.performed -= ContinueAction_performed;
    }

    private void ContinueAction_performed(InputAction.CallbackContext obj)
    {
        if (!_isInDialogue) return;

        _continueFlag = true;
    }
    #endregion

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        gameObject.SetActive(false);
        _optionsPanel.gameObject.SetActive(false);

        if (_useAnimations && _dialogueBoxAnimator == null)
        {
            Debug.LogError("Dialogue box animator is not assigned. Animation disabled.", gameObject);
        }

        if (_useAnimations)
        {
            _dialogueBoxAnimator.SetBool(_visibleBoolDialogueBoxKey, false);
        }
    }

    #region Start Dialogue
    public void StartDialogue(Story story, string title = null)
    {
        gameObject.SetActive(true);
        Debug.Log("Start of dialogue");
        StartCoroutine(GoThroughStory(story, _resetStoryOnDialogueStart, title: title));
    }
    public void StartDialogue(Story story, Action onDialogueEndCallback, string title = null)
    {
        gameObject.SetActive(true);
        Debug.Log("Start of dialogue");
        StartCoroutine(GoThroughStory(story, _resetStoryOnDialogueStart, onDialogueEndCallback, title));
    }

    public void StartDialogue(Story story, bool forceRestart, string title = null)
    {
        gameObject.SetActive(true);
        Debug.Log("Start of dialogue");
        StartCoroutine(GoThroughStory(story, forceRestart, title: title));
    }
    #endregion

    private void EndDialogue(Action onDialogueEndCallback)
    {
        Debug.Log("End of dialogue");
        ClearDialogueBox();
        _isInDialogue = false;

        if (_useAnimations)
        {
            _dialogueBoxAnimator.SetBool(_visibleBoolDialogueBoxKey, false);
        }
        else
        {
            gameObject.SetActive(false);
        }

        onDialogueEndCallback?.Invoke();
    }

    private IEnumerator GoThroughStory(Story story, bool forceRestart, Action onDialogueEndCallback = null, string title = null)
    {
        ClearDialogueBox();

        if (_useAnimations)
        {
            _dialogueBoxAnimator.SetBool(_visibleBoolDialogueBoxKey, true);
        }

        _isInDialogue = true;

        if (forceRestart || title != null)
        {
            Debug.Log(title);
            story.SetNextNode(title != null ? title : story.GetStart());
        }

        //Debug.Log("Going through story : " + story);
        int nextNodesCount = 0;
        StoryNode currentNode;

        yield return new WaitForSeconds(_timeBeforeFirstText);

        do
        {
            _continueFlag = false;

            currentNode = story.GetCurrentNode();
            if (currentNode == null)
            {
                Debug.LogError("Current node is null while reading story.");
                break;
            }
            nextNodesCount = currentNode.GetNextNodes().Count;

            if (title != null)
            {
                if (title == "Chien")
                {
                    _titleText.text = "Chien";
                    if(!abouament.isPlaying) abouament.Play();
                }
                else _titleText.text = GetFullName(title.Split('_')[currentNode.HasTag("Reponse") ? 1 : 0]);
            }
            else _titleText.text = currentNode.GetTitle();

            _mainText.text = currentNode.getText();

            // If the title text begins with // then hide the title text container
            _titleTextContainer.SetActive(!_titleText.text.StartsWith("//"));

            LayoutRebuilder.ForceRebuildLayoutImmediate(_rectTransform);

            if (nextNodesCount > 1)
            {
                // There are multiple options to choose from
                _isOptionSelected = false;

                // Display options to the player
                yield return DisplayOptions(currentNode);

                //Debug.Log("Option count: " + nextNodesCount);

                // Wait for player to choose an option
                yield return new WaitUntil(() => _isOptionSelected);

                //Debug.Log("Option selected: " + _optionSelected);

                story.ChooseNextNode(_optionSelected % nextNodesCount);
            }
            else
            {
                // Simple dialogue, just continue to the next node when the player presses the continue button
                yield return new WaitUntil(() => _continueFlag);

                // Break from the loop if there are no more nodes
                if (nextNodesCount > 0)
                {
                    story.NextNode();
                }
                else
                {
                    break;
                }
            }

            yield return null;
        }
        while (currentNode != null);

        UpdateVariables(story);

        EndDialogue(onDialogueEndCallback);
    }

    private string GetFullName(string nameShort)
    {
        switch (nameShort)
        {
            case "Ce": return "Mangaka";
            case "Ch": return "Chien";
            case "P": return "Professeur";
            case "E": return "Enfant";
        }

        return "// ???";
    }

    private void UpdateVariables(Story story)
    {
        if (isHappy != story.GetVariable("isHappy"))
        {
            isHappy = story.GetVariable("isHappy");
            UpdateTeacherHappyness();
        }
        if (isOpen != story.GetVariable("isOpen"))
        {
            isOpen = story.GetVariable("isOpen");
            UpdateCelebrityOpeness();
        }
        if (isInside != story.GetVariable("isInside"))
        {
            isInside = story.GetVariable("isInside");
            UpdateDog();

        }
    }

    private void UpdateDog()
    {
        guard.SetActive(false);
    }

    private void UpdateCelebrityOpeness()
    {
        door.SetActive(false);

        // TODO: Show eyes on all npcs
        ShowNpcEyes(true);
    }

    private void UpdateTeacherHappyness()
    {
        throw new NotImplementedException();
    }

    #region Helper Methods
    private IEnumerator DisplayOptions(StoryNode currentNode)
    {
        _optionsPanel.alpha = 0;
        _optionsPanel.gameObject.SetActive(true);
        foreach (Transform child in _optionsPanel.transform)
        {
            Destroy(child.gameObject);
        }

        int optionIndex = 0;
        foreach (var nextNode in currentNode.GetNextNodes())
        {
            DialogueOptionKnitting option = Instantiate(_optionPrefab, _optionsPanel.transform);
            option.MakeOption(nextNode.display, optionIndex, this);
            optionIndex++;
        }

        yield return null;

        // Force to recalculate the layout
        LayoutRebuilder.ForceRebuildLayoutImmediate(_rectTransform);
        _optionsPanel.alpha = 1;
    }

    private void ClearDialogueBox()
    {
        _titleTextContainer.SetActive(false);
        _titleText.text = "";
        _mainText.text = "";
    }

    private void ShowNpcEyes(bool show)
    {
        foreach (var eye in eyes)
        {
            eye.SetActive(show);
        }
    }

    #endregion

    #region Callbacks
    public void OptionSelected(int optionIndex)
    {
        //Debug.Log("Option selected : " + optionIndex);
        _optionSelected = optionIndex;
        _isOptionSelected = true;

        _optionsPanel.gameObject.SetActive(false);
    }
    #endregion
}
