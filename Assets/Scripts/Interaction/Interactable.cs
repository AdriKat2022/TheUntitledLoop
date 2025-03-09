using AdriKat.Toolkit.Attributes;
using System;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [SerializeField] private bool _highlightOnInteract = true;
    [ShowIf(nameof(_highlightOnInteract))]
    [SerializeField] private SpriteRenderer _sprite;
    [ShowIf(nameof(_highlightOnInteract))]
    [SerializeField] private Color _highlightColor = Color.yellow;

    [Space]

    [SerializeField] private bool _isInteractable = true;
    [SerializeField] private bool _isReusable = true;

    [SerializeField] private UnityEvent<Action> _onInteract;

    private bool _isUsed = false;
    private bool _isHighlighted = false;
    private Color _lastColor;
    private CharacterInteraction _currentCharacterInteraction;

    private void Start()
    {
        if (_highlightOnInteract && _sprite == null)
        {
            _sprite = GetComponent<SpriteRenderer>();
        }

        if (_highlightOnInteract && _sprite == null)
        {
            Debug.LogWarning("Highlight on interact is enabled but no sprite renderer was found nor assigned.", gameObject);
        }
    }

    private void OnDisable()
    {
        if (_currentCharacterInteraction != null)
        {
            _currentCharacterInteraction.RemovePossibleInteraction(this);
            _currentCharacterInteraction = null;
        }
    }

    public void Interact(Action endInteractionCallback)
    {
        Debug.Log("Interacted with " + name, gameObject);
        if (_isInteractable && (!_isUsed || _isReusable))
        {
            _onInteract.Invoke(endInteractionCallback);
            _isUsed = !_isReusable;
        }
    }

    public void Highlight(bool toogle)
    {
        if (!_highlightOnInteract || _sprite == null) return;

        if (toogle)
        {
            if (!_isHighlighted) _lastColor = _sprite.color;

            Debug.Log("Saving " + _sprite.color + " from " + _sprite.gameObject, gameObject);

            _sprite.color = _highlightColor;
            _isHighlighted = true;
        }
        else
        {
            if (_isHighlighted) _sprite.color = _lastColor;

            _isHighlighted = false;
        }
    }

    #region Trigger Management
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out CharacterInteraction interaction))
        {
            _currentCharacterInteraction = interaction;
            interaction.AddPossibleInteraction(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out CharacterInteraction interaction))
        {
            interaction.RemovePossibleInteraction(this);
            _currentCharacterInteraction = null;
        }
    }
    #endregion
}
