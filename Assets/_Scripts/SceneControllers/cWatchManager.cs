using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cWatchManager : MonoBehaviour{
    public GameObject goPulsera;
    public GameObject[] goSubBotones;
    public Animator[] animaciones;
    //ALE public cCestaManager scrCesta;
    public Jewel1Manager jewel1Manager;//cStEVAlcudia
    //public cStEVAlcudia jewel1Manager;//cStEVAlcudia

    private bool bShowing = false;

    private void Update() {
        if (cAppManager.IsZurdo) {
            transform.position = cXRManager.GetTrMunyecaRight().position;
            transform.rotation = cXRManager.GetTrMunyecaRight().rotation;
        }
        else {
            transform.position = cXRManager.GetTrMunyecaLeft().position;
            transform.rotation = cXRManager.GetTrMunyecaLeft().rotation;
        }
    }
    public void Show(bool enable) {
        goPulsera.SetActive(enable);
    }

    public void ClickMainBt() {
        bShowing = !bShowing;
        StartCoroutine(ToggleMenu());
    }
    private IEnumerator ToggleMenu() {
        if (bShowing) {
            foreach (GameObject but in goSubBotones) {
                but.SetActive(true);
            }
            yield return null;
            foreach (Animator anim in animaciones) {
                anim.ResetTrigger("Show");
                anim.ResetTrigger("Hide");
                anim.SetTrigger("Show");
            }
        }
        else {
            foreach (Animator anim in animaciones) {
                anim.ResetTrigger("Show");
                anim.ResetTrigger("Hide");
                anim.SetTrigger("Hide");
            }
            yield return new WaitForSeconds(0.6f);
            foreach (GameObject but in goSubBotones) {
                but.SetActive(false);
            }
        }
    }
    public void ClickHomeBt() {
        bShowing = false;
        StartCoroutine(ToggleMenu());
        jewel1Manager.ResetUserPosition();
        //ALE cDataManager.AddResponse(eDataSesionAction.HOME, "");
    }
    public void ClickCestaBt() {
        bShowing = false;
        StartCoroutine(ToggleMenu());
        //ALE scrCesta.Show();
        //ALE cDataManager.AddResponse(eDataSesionAction.CESTA_ABRE, "");
    }
    public void ClickExitBt() {
        bShowing = false;
        StartCoroutine(ToggleMenu());
        //ALE cDataManager.AddResponse(eDataSesionAction.EXIT, "");
        cAppManager.QuitApp();
    }
}
