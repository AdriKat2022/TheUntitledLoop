using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterInteraction : MonoBehaviour
{
    [SerializeField] private InputActionReference _interactionInputReference;
    [SerializeField] private Animator _interactionHint;

    private Interactable _currentInteractable;
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

        if (_currentInteractable != null)
        {
            _currentInteractable.Interact(EndInteractionCallback);
            _isInteracting = true;
            _interactionHint.SetBool("InInteraction", true);
        }
    }

    private void EndInteractionCallback()
    {
        _isInteracting = false;
        _interactionHint.SetBool("InInteraction", false);
    }
    #endregion

    public void SetInteractable(Interactable interactable)
    {
        _currentInteractable = interactable;
        if (_interactionHint != null)
        {
            _interactionHint.SetBool("IsVisible", interactable != null);
        }
    }
}
