using System.Collections;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private int beginSceneIndex = 1;
    [SerializeField] private Animator[] menus;
    [SerializeField] private float animationDuration = 0.5f;

    private int currentMenuIndex = 0;
    private bool isAnimating = false;

    public void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(beginSceneIndex);
    }

    public void ShowMenu(int index)
    {
        if (isAnimating) return;

        isAnimating = true;

        StartCoroutine(ShowMenuAnimation(index));
    }

    #region Animations
    private IEnumerator ShowMenuAnimation(int index)
    {
        Animator menu = menus[currentMenuIndex];
        menu.SetBool("IsVisible", false);

        currentMenuIndex = index;
        yield return new WaitForSeconds(animationDuration);

        Animator newMenu = menus[index];
        newMenu.SetBool("IsVisible", true);

        isAnimating = false;
    }
    #endregion

    public void QuitGame()
    {
        Application.Quit();
    }
}
