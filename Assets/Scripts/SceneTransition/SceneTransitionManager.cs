using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    //Fade screen
    public FadeScreen fadeScreen;

    //Scene transition
    [SerializeField] private LoadSceneMode _loadSceneMode;
    private AsyncOperation asyncLoadOperation; //restituito dalle operazioni asincrone per determinare se op è completata

    
  
    

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
