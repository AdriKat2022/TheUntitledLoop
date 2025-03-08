using AdriKat.Toolkit.Attributes;
using System;
using UnityEngine;

public class ParallaxManager : MonoBehaviour
{
    #region Variables
    [Header("Parallax Settings")]
    [SerializeField] private bool _useStartPositionAsReference = false;
    [ShowIf(nameof(_useStartPositionAsReference), disableInsteadOfHidding: false, invert: true)]
    [SerializeField] private Vector2 _basePosition = Vector2.zero;
    [Space]
    [SerializeField] private float _depthMultiplier = 1f;
    [Space]
    [SerializeField] private bool _parallaxOnX = true;
    [ShowIf(nameof(_parallaxOnX))]
    [SerializeField] private float _parallaxXMultiplier = 1f;

    [SerializeField] private bool _parallaxOnY = false;
    [ShowIf(nameof(_parallaxOnY))]
    [SerializeField] private float _parallaxYMultiplier = 1f;

    [Header("References")]
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private ParallaxLayer[] _backgrounds;

    [Serializable]
    private struct ParallaxLayer
    {
        public Transform transform;
        [Range(0f, 1f)]
        public float multiplier;
        public bool isForeground;

        public float xOffset;
        public float yOffset;
    }
    #endregion

    #region Gizmos

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_basePosition, new Vector3(1, 1, 1));
    }

    #endregion

    private void Awake()
    {
        if (_useStartPositionAsReference)
        {
            _basePosition = _cameraTransform.position;
        }
    }

    private void Update()
    {
        ManageParallax();
    }

    private void ManageParallax()
    {
        for (int i = 0; i < _backgrounds.Length; i++)
        {
            // Between 0 and 1, if greater than 1, it's a foreground and it moves against the camera
            float depthMultiplier = _backgrounds[i].multiplier;

            if (_backgrounds[i].isForeground)
            {
                depthMultiplier = -depthMultiplier;
            }

            Vector3 backgroundTargetPos = _backgrounds[i].transform.position;

            // Get the offsets
            float xOffset = _basePosition.x + _backgrounds[i].xOffset;
            float yOffset = _basePosition.y + _backgrounds[i].yOffset;

            if (_parallaxOnX)
            {
                // Follow camera on X axis, the greater the depth, the greater it follows, but never goes faster than the camera
                backgroundTargetPos.x = (_cameraTransform.position.x - xOffset) * depthMultiplier;
            }
            if (_parallaxOnY)
            {
                // Follow camera on Y axis, the greater the depth, the greater it follows, but never goes faster than the camera
                backgroundTargetPos.y = (_cameraTransform.position.y - yOffset) * depthMultiplier;
            }

            _backgrounds[i].transform.position = backgroundTargetPos;
        }
    }

    [ButtonAction("Set Base Position To Camera Position")]
    private void SetBasePositionToCamera()
    {
        _basePosition = _cameraTransform.position;
        _useStartPositionAsReference = false;
    }
}
