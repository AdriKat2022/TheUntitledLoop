using AdriKat.Toolkit.Attributes;
using UnityEngine;

public class AnimationSpriteController : MonoBehaviour
{
    [Required("A CharacterController2D is required to animate the sprite.")]
    [SerializeField] private CharacterController2D _characterController;
    [Space]
    [SerializeField] private Sprite[] _idleSprites;
    [Space]
    [SerializeField] private float _walkThreshold = 0.2f;
    [Space]
    [SerializeField] private bool _overrideLeftWalkingSprites;
    [SerializeField] private Sprite[] _rightWalkingSprites;
    [ShowIf(nameof(_overrideLeftWalkingSprites), disableInsteadOfHidding: true)]
    [SerializeField] private Sprite[] _leftWalkingSprites;
    [Space]
    [Tooltip("Speed in sprites per second")]
    [SerializeField] private float _animationSpeed = 1f;
    [Tooltip("Going from idle to walking for example will always use the first sprite fully and then follow the _animationSpeed.")]
    [SerializeField] private bool _resetTimingOnStateChange;
    [ShowIf(nameof(_resetTimingOnStateChange))]
    [Tooltip("Going from left to right or the opposite won't reset the timing.")]
    [SerializeField] private bool _ignoreDirectionChangeAsState;
    [Space]
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private float _timer = 0;
    private AnimationState _currentState;

    private void LateUpdate()
    {
        if (_characterController == null)
        {
            return;
        }

        float horizontalSpeed = _characterController.HorizontalSpeed;

        if (Mathf.Abs(horizontalSpeed) <= _walkThreshold)
        {
            CheckTiming(AnimationState.Idle);
            Animate(_idleSprites);
        }
        else if (horizontalSpeed > 0)
        {
            CheckTiming(AnimationState.WalkingRight);
            _spriteRenderer.flipX = false;
            Animate(_rightWalkingSprites);
        }
        else
        {
            CheckTiming(AnimationState.WalkingLeft);

            if (_overrideLeftWalkingSprites)
            {
                _spriteRenderer.flipX = false;
                Animate(_leftWalkingSprites);
            }
            else
            {
                _spriteRenderer.flipX = true;
                Animate(_rightWalkingSprites);
            }
        }

        _timer += Time.deltaTime * _animationSpeed;
    }

    private void Animate(Sprite[] idleSprites)
    {
        int index = (int)(_timer) % idleSprites.Length;
        _spriteRenderer.sprite = idleSprites[index];
    }

    private void CheckTiming(AnimationState state)
    {
        if (!_resetTimingOnStateChange || _currentState == state) return;

        if (_ignoreDirectionChangeAsState)
        {
            if (_currentState == AnimationState.WalkingLeft && state == AnimationState.WalkingRight)
            {
                return;
            }
            if (_currentState == AnimationState.WalkingRight && state == AnimationState.WalkingLeft)
            {
                return;
            }
        }

        _timer = 0;
        _currentState = state;
    }

    public enum AnimationState
    {
        Idle,
        WalkingRight,
        WalkingLeft
    }
}
