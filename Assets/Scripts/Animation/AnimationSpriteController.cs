using AdriKat.Toolkit.Attributes;
using UnityEngine;

public class AnimationSpriteController : MonoBehaviour
{
    [Required("A CharacterController2D is required to animate the sprite.")]
    [SerializeField] private Rigidbody2D _rigidbody;
    [Space]
    [Required("To control a character, make a CharacterSpriteData object.\n(Right Click->New->Character->CharacterSpritesData)")]
    [SerializeField] private CharacterSpritesDataSO _characterSpriteData;
    [Space]
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private float _timer = 0;
    private AnimationState _currentState;

    private void LateUpdate()
    {
        if (_rigidbody == null)
        {
            return;
        }

        float horizontalSpeed = _rigidbody.velocity.x;

        if (Mathf.Abs(horizontalSpeed) <= _characterSpriteData.WalkThreshold)
        {
            CheckTiming(AnimationState.Idle);
            Animate(_characterSpriteData.IdleSprites);
        }
        else if (horizontalSpeed > 0)
        {
            CheckTiming(AnimationState.WalkingRight);
            _spriteRenderer.flipX = false;
            Animate(_characterSpriteData.RightWalkingSprites);
        }
        else
        {
            CheckTiming(AnimationState.WalkingLeft);

            if (_characterSpriteData.OverrideLeftWalkingSprites)
            {
                _spriteRenderer.flipX = false;
                Animate(_characterSpriteData.LeftWalkingSprites);
            }
            else
            {
                _spriteRenderer.flipX = true;
                Animate(_characterSpriteData.RightWalkingSprites);
            }
        }

        _timer += Time.deltaTime * _characterSpriteData.AnimationSpeed;
    }

    private void Animate(Sprite[] idleSprites)
    {
        int index = (int)(_timer) % idleSprites.Length;
        _spriteRenderer.sprite = idleSprites[index];
    }

    private void CheckTiming(AnimationState state)
    {
        if (!_characterSpriteData.ResetTimingOnStateChange || _currentState == state) return;

        if (_characterSpriteData.IgnoreDirectionChangeAsState)
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
