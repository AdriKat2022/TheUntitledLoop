using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSpritesData", menuName = "Character/CharacterSpritesDataSO", order = 1)]
public class CharacterSpritesDataSO : ScriptableObject
{
    [field: SerializeField] public Sprite[] IdleSprites { get; private set; }

    [field: Space]

    [field: SerializeField] public float WalkThreshold { get; private set; } = 0.2f;
    [field: SerializeField] public bool OverrideLeftWalkingSprites { get; private set; }
    [field: SerializeField] public Sprite[] RightWalkingSprites { get; private set; }
    [field: SerializeField] public Sprite[] LeftWalkingSprites { get; private set; }

    [field: Space(10)]

    [Tooltip("Speed in sprites per second")]
    [field: SerializeField] public float AnimationSpeed { get; private set; } = 1f;

    [Tooltip("Going from idle to walking for example will always use the first sprite fully and then follow the _animationSpeed.")]
    [field: SerializeField] public bool ResetTimingOnStateChange { get; private set; }

    [Tooltip("Going from left to right or the opposite won't reset the timing.")]
    [field: SerializeField] public bool IgnoreDirectionChangeAsState { get; private set; }
}
