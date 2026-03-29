using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LoadingManager : MonoBehaviour
{
    [SerializeField] private Image loadingBarFill;
    [SerializeField] private string sceneToLoad = "MainScene";

    private void Start()
    {
        loadingBarFill.transform.localScale = new Vector3(0f, 1f, 1f);
        StartCoroutine(LoadSceneAsync());
    }

    private IEnumerator LoadSceneAsync()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            loadingBarFill.transform.localScale = new Vector3(progress, 1f, 1f);

            yield return null;
        }
    }
}