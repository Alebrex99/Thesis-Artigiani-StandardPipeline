using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class Button3D : MonoBehaviour
{
    //Notifiche cambio stato a FSM: scegliere Azione/Variabile
    public Action<Button3D, bool> OnButtonPressed;
    private bool isButtonPressed = false;
    public string ButtonName;

    //Elementi da spegnere e da accendere
    [SerializeField] Material _skyboxOn;
    [SerializeField] Material _skyboxOff;
    [SerializeField] GameObject[] _envsOn;
    [SerializeField] GameObject[] _EnvsOff;
    [SerializeField] GameObject _environmentOn;
    [SerializeField] GameObject _environmentOff;
    [SerializeField] GameObject _environmentMain;
    [SerializeField] Material _skyboxMain;

    void Start ()
    {
        
    }

    //Funzione specifica Buttone1
    public void Press()
    {
        if (isButtonPressed)
            return;
       isButtonPressed = true;
        //ACTION:
        if (OnButtonPressed != null)
            OnButtonPressed(this, isButtonPressed);
        
        isButtonPressed = !isButtonPressed;


        //PUOI UNIRE GLI IF PER SKYBOX E CAMBIO SCENA
        if (_environmentOn != null && _environmentOff != null)
        {
            if (_environmentOn.activeSelf)
            {
                _environmentOn.SetActive(false);
                _environmentOff.SetActive(true);
            }
            else
            {
                _environmentOff.SetActive(false);
                _environmentOn.SetActive(true);
            }

        }
        if (RenderSettings.skybox == _skyboxOn)
        {
            RenderSettings.skybox = _skyboxOff;
        }
        else
        {
            RenderSettings.skybox = _skyboxOn;
        }

    }

    //Ogni volta che viene cliccato un altro bottone, il corrente va resettato (stato main)
    public void Reset()
    {
        RenderSettings.skybox = _skyboxMain;
        _environmentMain.SetActive(true);
        _environmentOn.SetActive(false);
        isButtonPressed = false;
    }

    public String getButtonName()
    {
        return ButtonName;
    }

}
