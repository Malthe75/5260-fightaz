using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuItem : MenuItemBase
{
    [Header("Action")]
    [SerializeField] private string sceneToLoad;
    [SerializeField] private bool quitGame;

    public override void Confirm(int playerIndex = 0)
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else if (quitGame)
        {
            Application.Quit();
        }
    }
}
