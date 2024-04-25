using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using JetBrains.Annotations;

public class Button3D : MonoBehaviour
{
    //Notifiche cambio stato a FSM: scegliere Azione/Variabile
    public Action<Button3D, bool> OnButtonPressed;
    private bool isButtonPressed = false;
    public string ButtonName;

    //Elementi da spegnere e da accendere
    [SerializeField] Material _skyboxOn;
    [SerializeField] Material _skyboxMain;
    //[SerializeField] GameObject[] _envsOn;
    [SerializeField] GameObject _environmentOn;
    [SerializeField] GameObject _environmentMain;
    [SerializeField] private static GameObject _currentEnvironment;

    //mettere la logica direttamente nel bottone
    private Button3D _currentButton;


    void Start ()
    {
        _currentEnvironment = _environmentMain;
    }

    //Funzione specifica Buttone1
    public void Press()
    {
        if (isButtonPressed)
            return;

        isButtonPressed = true;

        //ACTION CAMBIO STATO:
        if (OnButtonPressed != null)
            OnButtonPressed(this, isButtonPressed);
        

        //logica nel bottone:


        //ChangeEvnironment();

        isButtonPressed = false;

    }


    public void ChangeEnvironment()
    {
        Debug.Log(_currentEnvironment.name);

        if (_currentEnvironment != _environmentOn)
        {
            _currentEnvironment.SetActive(false);
            _currentEnvironment = _environmentOn;
            _currentEnvironment.SetActive(true);
        }
        else
        {
            _currentEnvironment.SetActive(false);
            _currentEnvironment = _environmentMain;
            _currentEnvironment.SetActive(true);
  

        }


        /*
        if (RenderSettings.skybox == _skyboxOn)
        {
            RenderSettings.skybox = _skyboxOff;
        }
        else
        {
            RenderSettings.skybox = _skyboxOn;
        }*/

    }

    public String getButtonName()
    {
        return ButtonName;
    }

    public GameObject GetAssociatedEnvironment()
    {
        return _environmentOn;
    }

    public void ChangeEvnironment()
    {
        if (_environmentOn != null && _environmentOn)
        {
            _environmentOn.SetActive(true);
        }
    }
}
