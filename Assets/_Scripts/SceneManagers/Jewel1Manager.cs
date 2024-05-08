using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Jewel1Manager : MonoBehaviour
{
    public static Jewel1Manager instance;

    //FEATURES DA cStBase
    public Transform userInitPos;
    public Transform trLightJewel;

    //GESTIONE AUDIO + IMMERSIONE
    public AudioSource envExplaination;
    [Range(0, 60)]
    [SerializeField] private float _immersionDelay = 1f;

    //VIDEO SOROLLA
    public VideoPlayer videoPlayer;
    [SerializeField] private GameObject goVideoPlayer;
    int loopVideo = 0;
    private bool bShownVideo = false;
    private bool bShowJewel = false;
    [Range(0.1f, 10)]
    [SerializeField] private float rotationVideoSpeed = 1;


    // Start is called before the first frame update
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        ResetUserPosition();
        Invoke(nameof(PlayEnvExplaination), _immersionDelay);
    }

    // Update is called once per frame
    void Update()
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

    public Transform GetUserInitTr()
    {
        return userInitPos;
    }

    public void ResetUserPosition()
    {
        cXRManager.SetUserPosition(GetUserInitTr().position, GetUserInitTr().rotation);
    }

    public void PlayEnvExplaination()
    {
        if(envExplaination != null)
        {
            envExplaination.Play();
        }
    }

}
