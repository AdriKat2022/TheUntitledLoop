using TMPro;
using UnityEngine;

public class WobblyTextEffect : MonoBehaviour
{
    [SerializeField] private TMP_Text textMeshPro;
    [SerializeField] private float amplitude = 5f; // Strength of wobble
    [SerializeField] private float frequency = 2f; // Speed of wobble
    [SerializeField] private float waveOffset = 0.5f; // Offset per character

    private TMP_TextInfo textInfo;
    private Vector3[] vertices;

    private void Start()
    {
        if (textMeshPro == null)
        {
            textMeshPro = GetComponent<TMP_Text>();
        }
    }

    private void Update()
    {
        ApplyWobbleEffect();
    }

    private void ApplyWobbleEffect()
    {
        if (textMeshPro == null) return;

        textMeshPro.ForceMeshUpdate();
        textInfo = textMeshPro.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;

            int vertexIndex = textInfo.characterInfo[i].vertexIndex;
            int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
            vertices = textInfo.meshInfo[materialIndex].vertices;

            Vector3 offset = GetWobbleOffset(i, Time.time);
            ApplyOffsetToCharacter(vertexIndex, offset);
        }

        // Update mesh
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            textMeshPro.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }

    private Vector3 GetWobbleOffset(int characterIndex, float time)
    {
        float wobbleX = Mathf.Sin(time * frequency + characterIndex * waveOffset) * amplitude * 0.5f;
        float wobbleY = Mathf.Cos(time * frequency + characterIndex * waveOffset) * amplitude;
        return new Vector3(wobbleX, wobbleY, 0);
    }

    private void ApplyOffsetToCharacter(int vertexIndex, Vector3 offset)
    {
        vertices[vertexIndex + 0] += offset;
        vertices[vertexIndex + 1] += offset;
        vertices[vertexIndex + 2] += offset;
        vertices[vertexIndex + 3] += offset;
    }
}
