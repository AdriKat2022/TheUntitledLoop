using AdriKat.Toolkit.Attributes;
using System;
using UnityEngine;

public class DialogueStarter : MonoBehaviour
{
    #region Variables
    [SerializeField] private bool _lockPlayerMovmement = true;

    [ShowIf(nameof(_lockPlayerMovmement), disableInsteadOfHidding: false)]
    [SerializeField] private bool _useTag;

    [ShowIf(nameof(_lockPlayerMovmement), disableInsteadOfHidding: false)]
    [ShowIf(nameof(_useTag), disableInsteadOfHidding: true, invert: true)]
    [SerializeField] private CharacterController2D _playerMouvementScript;

    [ShowIf(nameof(_lockPlayerMovmement), disableInsteadOfHidding: false)]
    [ShowIf(nameof(_useTag), disableInsteadOfHidding: false, invert: false)]
    [SerializeField] private string _playerTag = "Player";

    [SerializeField] string name = "Ce";

    [Space]

    [SerializeField] public Story _story;
    #endregion

    private void OnEnable()
    {
        if (_useTag)
        {
            _playerMouvementScript = FindCharacterControllerWithTag(_playerTag);
        }

        if (_lockPlayerMovmement && _playerMouvementScript == null)
        {
            Debug.LogWarning("Player movement script is null but you want to lock it during dialogue, please assign it in the inspector or use a tag.");
        }
        _story.SetVariable("isHappy", "false");
        _story.SetVariable("isOpen", "false");
        _story.SetVariable("oneTime", "false");
        _story.SetVariable("isInside", "false");
    }

    public void StartDialogue(Action callback)
    {
        StartDialogueAction(callback);
    }

    public void StartDialogue()
    {
        StartDialogueAction(null);
    }

    private void StartDialogueAction(Action callback)
    {
        if (_story == null)
        {
            Debug.LogError("There is no referenced story!");
            return;
        }

        bool lockMovement = _lockPlayerMovmement;

        if (lockMovement)
        {
            _playerMouvementScript.DisableMovement();
        }

        DialogueManager.Instance.StartDialogue(_story, () =>
        {
            if (lockMovement)
            {
                _playerMouvementScript.EnableMovement();
            }
            callback?.Invoke();
        }, _playerMouvementScript.name + "_" + name + "_" + _playerMouvementScript.zone);
    }

    #region Helper Methods
    [ButtonAction("Assign Player and use it")]
    private void AssignTagImmediately()
    {
        if (!_useTag)
        {
            Debug.LogWarning("Use Tag to assign it.");
            return;
        }

        if (string.IsNullOrEmpty(_playerTag))
        {
            Debug.LogWarning("Player tag is empty, please assign a tag.");
            return;
        }

        _playerMouvementScript = FindCharacterControllerWithTag(_playerTag);

        _useTag = false;
    }

    private CharacterController2D FindCharacterControllerWithTag(string playerTag)
    {
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);

        if (player == null)
        {
            Debug.LogWarning("Player not found with tag: " + playerTag);
            return null;
        }

        if (player.TryGetComponent(out CharacterController2D playerMouvementScript))
        {
            return playerMouvementScript;
        }

        Debug.LogWarning($"Player movement script not found on player object {player}.");
        return null;
    }
    #endregion
}
