using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [Tooltip("If true, the first interaction is the one that will be interacted with.\nIf false, it's the last newer one that was added.")]
    [SerializeField] private bool _useFirstInteraction = true; // If true, the interaction will be done with the first interaction found
    [Space]
    [SerializeField] private InputActionReference _interactionInputReference;
    [SerializeField] private Animator _interactionHint;

    private readonly List<Interactable> _interactables = new();
    private bool _isInteracting = false;

    #region Input Management
    private void OnEnable()
    {
        _interactionInputReference.action.Enable();
        _interactionInputReference.action.performed += OnInteraction;
    }

    private void OnDisable()
    {
        _interactionInputReference.action.Disable();
        _interactionInputReference.action.performed -= OnInteraction;
    }

    private void OnInteraction(InputAction.CallbackContext ctx)
    {
        if (_isInteracting) return;

        if (_interactables != null && _interactables.Count > 1)
        {
            PerformInteraction();
        }
    }
    #endregion

    private void PerformInteraction()
    {
        _isInteracting = true;
        _interactionHint.SetBool("InInteraction", true);

        if (_useFirstInteraction)
        {
            _interactables[0].Interact(EndInteractionCallback);
        }
        else
        {
            _interactables[^1].Interact(EndInteractionCallback);
        }
    }

    private void EndInteractionCallback()
    {
        _isInteracting = false;
        _interactionHint.SetBool("InInteraction", false);
    }

    public void AddPossibleInteraction(Interactable interactable)
    {
        if (interactable == null)
        {
            Debug.LogError("Tried to add a null interactable.");
            return;
        }

        _interactables.Add(interactable);
        if (_interactionHint != null)
        {
            _interactionHint.SetBool("IsVisible", true);
        }

        CheckNextInteractable();
    }

    public void RemovePossibleInteraction(Interactable interactable)
    {
        if (interactable == null)
        {
            Debug.LogError("Tried to remove a null interactable.");
            return;
        }

        // Find the interactable and remove it
        for (int i = 0; i < _interactables.Count; i++)
        {
            if (_interactables[i] == interactable)
            {
                _interactables[i].Highlight(false);
                _interactables.RemoveAt(i);
                break;
            }
        }

        if (_interactionHint != null)
        {
            _interactionHint.SetBool("IsVisible", _interactables.Count > 0);
        }

        CheckNextInteractable();
    }

    private void CheckNextInteractable()
    {
        if (_interactables.Count == 0) return;

        foreach (Interactable interactable in _interactables)
        {
            interactable.Highlight(false);
        }

        if (_useFirstInteraction)
        {
            _interactables[0].Highlight(true);
        }
        else
        {
            _interactables[^1].Highlight(true);
        }
    }
}
