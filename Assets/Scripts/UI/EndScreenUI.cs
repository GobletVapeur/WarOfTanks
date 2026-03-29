using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreenUI : MonoBehaviour
{
    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
