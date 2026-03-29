using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreenUI : MonoBehaviour
{
    [SerializeField] private string mainMenuScene = "MainMenu";

    public void GoToMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }
}
