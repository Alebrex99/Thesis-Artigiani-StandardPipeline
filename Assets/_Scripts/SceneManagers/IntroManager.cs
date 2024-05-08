using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class IntroManager : MonoBehaviour
{
    public static IntroManager instance;

    //VIDEO (cSceneInfo)
    public VideoPlayer videoPlayer;
    [SerializeField] private GameObject goVideoPlayer;
    int loopVideo = 0;
    [Range(0.1f, 10)]
    [SerializeField] private float rotationVideoSpeed = 1;
    private bool bShownVideo = false;

    //cSceneInfo : video + animation logo
    public Transform userInitPos;
    private float timeLastClick = 0;
    [SerializeField] Animator animLogo;

    //AUDIO
    public AudioSource voiceAudio;

  
    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (bShownVideo)
        {
            //Vector3 euler = Quaternion.LookRotation(goVideoPlayer.transform.position - cXRManager.GetTrCenterEye().position).eulerAngles;
            //goVideoPlayer.transform.eulerAngles = new Vector3(0, euler.y, 0);
            
            //Method lectures:
            Vector3 targetDirection = goVideoPlayer.transform.position - cXRManager.GetTrCenterEye().position;
            targetDirection.y = 0;
            targetDirection.Normalize();
            float rotationStep = rotationVideoSpeed * Time.deltaTime;
            
            Vector3 newDirection = Vector3.RotateTowards(goVideoPlayer.transform.forward, targetDirection, rotationStep, 0.0f);
            goVideoPlayer.transform.rotation = Quaternion.LookRotation(newDirection, goVideoPlayer.transform.up);
        }
        else
        {
            Vector3 euler = Quaternion.LookRotation(goVideoPlayer.transform.position - cXRManager.GetTrCenterEye().position).eulerAngles;
            //SOSTITUIRE : goVideoPlayer se voglio un altro oggetto quando il video si spegne; es) goLogoCentral
            goVideoPlayer.transform.eulerAngles = new Vector3(0, euler.y, 0);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        ResetUserPosition();
        //START VOICE AUDIO
        if (voiceAudio != null && videoPlayer !=null)
        {
            videoPlayer.Play();
            bShownVideo = true;
            voiceAudio.Play();
        }
        videoPlayer.loopPointReached += EndVideo;
        Invoke(nameof(EndAudio), voiceAudio.clip.length);
    }

    private void EndVideo(VideoPlayer source)
    {
        loopVideo++;
        if(loopVideo>= 2)
        {
            source.loopPointReached -= EndVideo;
            source.Stop();

            //ANIMAZIONI POSSIBILI : LOGO -> VIDEO
            /*animLogo.ResetTrigger("ShowVideo");
            animLogo.SetTrigger("HideVideo");*/
            bShownVideo = false; //smetterà di seguire l'utente
        }
    }

    private void EndAudio()
    {
        cAppManager.LoadScene(Scenes.HOME);
        //ANIMAZIONI POSSIBILI : LOGO -> VIDEO
    }

    public Transform GetUserInitTr()
    {
        return userInitPos;
    }

    public void ResetUserPosition()
    {
        cXRManager.SetUserPosition(GetUserInitTr().position, GetUserInitTr().rotation);
    }

    public void SetVideo(VideoClip vc)
    {
        //setting video clip run time
        videoPlayer.clip = vc;
    }




    //SOLO SE SI VUOLE AGGIUNGERE ANIMAZIONE LOGO -> VIDEO
    public void ClickLogo()
    {
        Debug.Log("Tocado logo");
        animLogo.ResetTrigger("HideVideo");
        animLogo.SetTrigger("ShowVideo");
        videoPlayer.loopPointReached += EndVideo;
        videoPlayer.isLooping = true;
        videoPlayer.Play();
        bShownVideo = true;
    }

    public void ClickButtonVideo()
    {
        if (Time.realtimeSinceStartup - timeLastClick < 1)
        {
            return;
        }
        if (bShownVideo)
        {
            videoPlayer.loopPointReached -= EndVideo;
            videoPlayer.Stop();
            animLogo.ResetTrigger("ShowVideo");
            animLogo.SetTrigger("HideVideo");
            bShownVideo = false;
            timeLastClick = Time.realtimeSinceStartup;
        }
        else
        {
            Debug.Log("Tocado logo");
            animLogo.ResetTrigger("HideVideo");
            animLogo.SetTrigger("ShowVideo");
            videoPlayer.loopPointReached += EndVideo;
            videoPlayer.isLooping = true;
            videoPlayer.Play();
            bShownVideo = true;
            timeLastClick = Time.realtimeSinceStartup;
            //cDataManager.AddResponse(eDataSesionAction.VIDEO, "");
        }
    }


}
