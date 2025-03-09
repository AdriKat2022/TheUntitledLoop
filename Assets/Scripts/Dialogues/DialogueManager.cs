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

    private RectTransform _rectTransform;
    private int _optionSelected = 0;
    private bool _isOptionSelected = false;
    private bool _continueFlag = false;
    private bool _isInDialogue = false;

    private string isHappy = "false";
    private string isOpen = "false";
    private string isInside = "false";

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
        gameObject.SetActive(false);
        _isInDialogue = false;

        onDialogueEndCallback?.Invoke();
    }

    private IEnumerator GoThroughStory(Story story, bool forceRestart, Action onDialogueEndCallback = null, string title = null)
    {
        _isInDialogue = true;

        if (forceRestart || title != null)
        {
            Debug.Log(title);
            story.SetNextNode(title != null ? title : story.GetStart());
        }

        Debug.Log("Going through story : " + story);
        int nextNodesCount = 0;
        StoryNode currentNode;

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

            _titleText.text = currentNode.GetTitle();
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

                Debug.Log("Option count: " + nextNodesCount);

                // Wait for player to choose an option
                yield return new WaitUntil(() => _isOptionSelected);

                Debug.Log("Option selected: " + _optionSelected);

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

    private void UpdateVariables(Story story)
    {
        if(isHappy!=story.GetVariable("isHappy"))
        {
            isHappy = story.GetVariable("isHappy");
            UpdateTeacherHappyness();
        }
        if (isOpen != story.GetVariable("isOpen"))
        {
            isOpen = story.GetVariable("isOpen");
            UpdateCelebrityOpeness();
        }
        if(isInside != story.GetVariable("isInside"))
        {
            isInside = story.GetVariable("isInside");
            UpdateDog();

        }
    }

    private void UpdateDog()
    {
        throw new NotImplementedException();
    }

    private void UpdateCelebrityOpeness()
    {
        throw new NotImplementedException();
    }

    private void UpdateTeacherHappyness()
    {
        throw new NotImplementedException();
    }

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

    public void OptionSelected(int optionIndex)
    {
        //Debug.Log("Option selected : " + optionIndex);
        _optionSelected = optionIndex;
        _isOptionSelected = true;

        _optionsPanel.gameObject.SetActive(false);
    }
}
