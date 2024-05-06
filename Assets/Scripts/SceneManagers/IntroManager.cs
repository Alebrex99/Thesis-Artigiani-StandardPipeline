using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class IntroManager : MonoBehaviour
{
    public static IntroManager instance;

    [SerializeField] private Transform trInitPos;
    public VideoPlayer videoPlayer;
    public AudioSource voiceAudio;

    private bool bShowVideo = false;
    int loopVideo = 0;
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        //QUANDO IL VIDEO FINISCE
        
        //START VOICE AUDIO
        if (voiceAudio != null && videoPlayer !=null)
        {
            videoPlayer.Play();
            voiceAudio.Play();
        }
        videoPlayer.loopPointReached += EndVideo;
        Invoke("EndAudio", voiceAudio.clip.length);
        ResetUserPosition();

    }

    private void EndVideo(VideoPlayer source)
    {
        //cAppManager.GoToSceneAsync(Scenes.HOME);
        //ANIMAZIONI POSSIBILI
        loopVideo++;
        if(loopVideo>= 2)
        {
            source.Stop();
            source.loopPointReached -= EndVideo;
        }
    }

    private void EndAudio()
    {
        cAppManager.GoToSceneAsync(Scenes.HOME);
        //ANIMAZIONI POSSIBILI
    }


    public Transform GetUserInitTr()
    {
        return trInitPos;
    }

    public void ResetUserPosition()
    {
        cXRManager.SetUserPosition(GetUserInitTr().position, GetUserInitTr().rotation);
    }



}
