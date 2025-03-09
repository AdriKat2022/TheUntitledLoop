using AdriKat.Toolkit.CodePatterns;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Header("Main Menu")]
    [SerializeField] private int _mainMenuSceneIndex = 0;

    public void ReturnToMainMenu()
    {
        Debug.Log("Returning to main menu...");

        // Load main menu scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(_mainMenuSceneIndex);
    }
}
