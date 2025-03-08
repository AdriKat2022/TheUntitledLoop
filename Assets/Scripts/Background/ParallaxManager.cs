using AdriKat.Toolkit.Attributes;
using System;
using UnityEngine;

public class ParallaxManager : MonoBehaviour
{
    [Header("Parallax Settings")]
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
        public bool overrideMultiplier;
        [ShowIf(nameof(overrideMultiplier))]
        public float multiplier;

        public ParallaxLayer(Transform transform, bool overrideMultiplier, float multiplier)
        {
            this.transform = transform;
            this.overrideMultiplier = overrideMultiplier;
            this.multiplier = multiplier;
        }
    }
}
