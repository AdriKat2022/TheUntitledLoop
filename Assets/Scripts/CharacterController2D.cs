using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController2D : MonoBehaviour
{
    [HideInInspector]
    public int SpeedMultiplier = 100; // Modified by something external

    #region Variables
    [Header("Animations")]
    [SerializeField] private GameObject _sprite;

    [Header("Speed & forces")]
    [Range(0f, 10f)]
    [Tooltip("Units per seconds")]
    [SerializeField] private float _topSpeed = 2f;
    [Space]
    [Tooltip("Using this will control the velocity overriding it directly, which might not work well if you're working with other forces.")]
    [SerializeField] private bool _instantAcceleration = false;
    [SerializeField] private float _acceleration = 50f;
    [Tooltip("Using this will control the velocity overriding it directly, which might not work well if you're working with other forces.")]
    [SerializeField] private bool _instantDeceleration = false;
    [SerializeField] private float _deceleration = 50f;

    [Header("Input References")]
    [SerializeField] private InputActionReference _movementInputReference;

    private Rigidbody2D _rigidbody;
    private float _currentHorizontalInput;
    #endregion

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    #region Input Management

    private void OnEnable()
    {
        _movementInputReference.action.Enable();
        _movementInputReference.action.performed += OnInput;
        _movementInputReference.action.canceled += OnInput;
    }

    private void OnDisable()
    {
        _movementInputReference.action.Disable();
        _movementInputReference.action.performed -= OnInput;
        _movementInputReference.action.canceled -= OnInput;
    }

    private void OnInput(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Performed)
        {
            _currentHorizontalInput = ctx.ReadValue<float>();
        }
        else
        {
            _currentHorizontalInput = 0;
        }
    }

    #endregion

    private void FixedUpdate()
    {
        ManageMovement();
    }

    private void ManageMovement()
    {
        if (_currentHorizontalInput != 0)
        {
            // Normalize the input if needed
            float targetVelocity = (SpeedMultiplier / 100f) * _topSpeed * _currentHorizontalInput;

            if (_instantAcceleration)
            {
                // Compute needed acceleration to reach the target velocity instantly as an impulse force
                float impulse = (targetVelocity - _rigidbody.velocity.x) * _rigidbody.mass;
                _rigidbody.AddForce(impulse * Vector2.right, ForceMode2D.Impulse);
            }
            else
            {
                float acceleration = (targetVelocity - _rigidbody.velocity.x) * _acceleration;
                _rigidbody.AddForce(acceleration * Vector2.right, ForceMode2D.Force);
            }
        }
        else
        {
            if (_instantDeceleration)
            {
                // Compute needed deceleration to reach 0 velocity instantly as an impulse force
                Vector2 impulse = -_rigidbody.velocity * _rigidbody.mass;
                _rigidbody.AddForce(impulse, ForceMode2D.Impulse);
            }
            else
            {
                // Decelerate the player
                Vector2 deceleration = -_rigidbody.velocity * _deceleration;
                _rigidbody.AddForce(deceleration, ForceMode2D.Force);
            }
        }
    }
}
