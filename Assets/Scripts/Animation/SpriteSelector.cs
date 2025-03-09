using UnityEngine;

public class SpriteSelector : MonoBehaviour
{
    [SerializeField] private SerializableDictionary<string, GameObject> _sprites;

    internal void SelectSprite(string currentCharacter)
    {
        bool found = false;
        foreach (var sprite in _sprites)
        {
            if (sprite.Key == currentCharacter)
            {
                found = true;
                sprite.Value.SetActive(true);
                break;
            }
            else
            {
                sprite.Value.SetActive(false);
            }
        }

        if (!found)
        {
            Debug.LogError($"Sprite not found for character {currentCharacter}.");
        }
    }
}
