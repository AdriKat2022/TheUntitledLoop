using System;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [SerializeField] private bool _isInteractable = true;
    [SerializeField] private bool _isReusable = true;

    [SerializeField] private UnityEvent<Action> _onInteract;

    private bool _isUsed = false;

    public void Interact(Action endInteractionCallback)
    {
        if (_isInteractable && (!_isUsed || _isReusable))
        {
            _onInteract.Invoke(endInteractionCallback);
            _isUsed = !_isReusable;
        }
    }

    #region Trigger Management
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out CharacterInteraction interaction))
        {
            interaction.SetInteractable(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("aaaaaaaaaaaaaaaaaaaaaaaaaa");
        if (collision.TryGetComponent(out CharacterInteraction interaction))
        {
            interaction.SetInteractable(null);
        }
    }
    #endregion
}
