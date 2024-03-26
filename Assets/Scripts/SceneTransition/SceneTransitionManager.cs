using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    //Fade screen
    public FadeScreen fadeScreen;

    //Multiple scenes scenario
    [SerializeField] private string _firstSceneToLoad;
    [SerializeField] private GameObject _playerCharacter; //OVRCameraRIG

    //Scene transition
    [SerializeField] private LoadSceneMode _loadSceneMode;
    private AsyncOperation asyncLoadOperation; //restituito dalle operazioni asincrone per determinare se op è completata

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
        asyncLoadOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while (!asyncLoadOperation.isDone)
            yield return null;

        if (_playerCharacter != null)
            _playerCharacter.gameObject.SetActive(true);
    }

    

    //FUNZIONI PER CAMBIO SCENA ESPOSTE:
    //unloading scene environment per eliminare l'ambiente base quando premo pulsante
    public void UnloadScene(int sceneIndex)
    {
        StartCoroutine(UnloadSceneRoutine(sceneIndex));
    }

    private IEnumerator UnloadSceneRoutine(int sceneIndex)
    {
        while (asyncLoadOperation != null && !asyncLoadOperation.isDone)
        {
            yield return null;
        }

        SceneManager.UnloadSceneAsync(sceneIndex);
    }

    //FUNZIONI GENERICHE PER CAMBIO SCENA:
    //Meglio usare le Async, perchè le base frizzano il gioco, caricano e fanno ripartire tutto
    public void GoToSceneAsync(int sceneIndex)
    {
        StartCoroutine(GoToSceneAsyncRoutine(sceneIndex));
    }

    IEnumerator GoToSceneAsyncRoutine(int sceneIndex)
    {
        fadeScreen.FadeOut();
        asyncLoadOperation.allowSceneActivation = false;

        float timer = 0;
        while (timer <= fadeScreen.fadeDuration && !asyncLoadOperation.isDone)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        asyncLoadOperation.allowSceneActivation = true;
    }

    //COROUTINES: eseguite (di default) una volta per frame (yield return null) quando lanciate 1 volta per es. con il bottone
  

}
