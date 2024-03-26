using Meta.WitAi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxManager: MonoBehaviour
{
    [SerializeField] Material skyboxImg360Mono;
    [SerializeField] Material skyboxImg180Mono;
    [SerializeField] Material skyboxBase;
    public FadeScreen fadeScreen;

    private void Start()
    {
        RenderSettings.skybox = skyboxBase;
    }
    public void OnButton1Pressed()
    {       
        if(RenderSettings.skybox == skyboxImg360Mono)
        {
            RenderSettings.skybox = skyboxBase;
        }
        else
        {   
            RenderSettings.skybox = skyboxImg360Mono;
        }
    }
        
}
