using Cinemachine;
using UnityEngine;

public class CameraSineDolly : MonoBehaviour
{
    [SerializeField] private CinemachineDollyCart _dolly;
    [Space]
    [SerializeField] private AnimationCurve _pathOverTime;
    [Tooltip("This curve should be the same length as the path. It should start at 0 and end at 1.")]
    [SerializeField] private float _fullPathTime;

    private float _time = 0f;
    private float _direction = 1f;

    private void Update()
    {
        AnimateDolly();
    }

    public void AnimateDolly()
    {
        _dolly.m_Position = _pathOverTime.Evaluate(_time / _fullPathTime);

        if (_time > _fullPathTime || _time < 0)
        {
            _time = Mathf.Clamp(_time, 0, _fullPathTime);
            _direction *= -1;
        }

        _time += Time.deltaTime * _direction;
    }
}
