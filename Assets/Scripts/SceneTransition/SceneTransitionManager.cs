using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public FadeScreen fadeScreen;

    [SerializeField] private string _firstSceneToLoad;
    [SerializeField] private GameObject _playerCharacter;
    // Start is called before the first frame update
    void Start()
    {
        Scene firstScene = SceneManager.GetSceneByName(_firstSceneToLoad);
        if (firstScene.IsValid() && firstScene.isLoaded)
            return;

        if (_playerCharacter != null)
            _playerCharacter.gameObject.SetActive(false);
        StartCoroutine(LoadFirstScene(_firstSceneToLoad));

    }
    private IEnumerator LoadFirstScene(string sceneName)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while (!loadOperation.isDone)
            yield return null;

        if (_playerCharacter != null)
            _playerCharacter.gameObject.SetActive(true);
    }

    public void GoToScene(int sceneIndex)
    {
        StartCoroutine(GoToSceneRoutine(sceneIndex));
    }
    
    IEnumerator GoToSceneRoutine(int sceneIndex)
    {
        fadeScreen.FadeOut();
        yield return new WaitForSeconds(fadeScreen.fadeDuration);

        SceneManager.LoadScene(sceneIndex);
    }
    
    public void GoToSceneAsync(int sceneIndex)
    {
        StartCoroutine(GoToSceneAsyncRoutine(sceneIndex));
    }
    IEnumerator GoToSceneAsyncRoutine(int sceneIndex)
    {
        fadeScreen.FadeOut();
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        operation.allowSceneActivation = false;
        
        float timer = 0;
        while (timer <= fadeScreen.fadeDuration && !operation.isDone)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        operation.allowSceneActivation = true;
    }
    public void GoToAsyncDelay(int sceneIndex, float delay)
    {
        StartCoroutine(GoToAsyncDelayRoutine(sceneIndex, delay));
    }
    IEnumerator GoToAsyncDelayRoutine(int sceneIndex, float delay)
    {
        float timer = 0;
        while (timer <= delay)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        GoToSceneAsync(sceneIndex);
    }
}
