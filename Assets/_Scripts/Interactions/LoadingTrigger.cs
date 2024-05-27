using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LoadingTrigger : MonoBehaviour
{
    //OGGETTI DA IGNORARE:
    [SerializeField] private GameObject[] goToIgnore;
    [SerializeField] private cMenuLoad cMenuLoad;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("ENTRATO: " +  other.gameObject.name); //es. entra il button load + entra il collider (game object nel bottone che si chiama collider)
        if (cMenuLoad.IsShowing())
        {
            if (!goToIgnore.Contains(other.gameObject))
            {
                other.gameObject.SetActive(false);
                Debug.Log("DEVE ESSERCI SOLO IL MENU : " + other.gameObject.name);
            }
            //se in questo trigger si trovano i goToIgnore, non devono essere disattivati
        }
        else //alora è solo loading cambio scena
        {
            //se il pannello attio è quello di loading allora disattiva tutti gli oggetti col trigger eccetto la OVR camera rig
            if (other.gameObject != GetComponent<OVRCameraRig>())
            {
                other.gameObject.SetActive(false);
                Debug.Log("DEVE ESSERCI SOLO IL LOADING" + other.gameObject.name);
            }
        }
         
    }
}
