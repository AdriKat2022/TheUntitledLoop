using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterInteraction : MonoBehaviour
{
    [SerializeField] private InputActionReference _interactionInputReference;
    [SerializeField] private Animator _interactionHint;

    private Interactable _currentInteractable;

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
        if (_currentInteractable != null)
        {
            _currentInteractable.Interact();
        }
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
