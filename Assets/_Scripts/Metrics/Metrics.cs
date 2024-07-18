using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class Metrics : MonoBehaviour
{
/*

    //public static Metrics instance;
    private cSlot[] _shoeses;
    private bool isGrabbed = false;

    [SerializeField] private cSceneInfo _sceneInfo;

    //METRICHE APPLICAZIONE
    private float endAppTimeEXP = 0; //istante di fine esperienza dalla fine del caricamentoo scBase
    [SerializeField] private AudioSource _audioSource;

    //METRICHE SCARPA:
    //Event + time + product + rotation + position
    private int shoesNumber;
    //accesso a ProductoGrabPinch: per prendere rotation
    private Transform trShoes;
    private Vector3 trShoesRotation;
    private Vector3 trShoesPosition;

    //METRICHE BUTTONS
    private bool isVideoButtonPressed = false;

    //METRICHE POSIZIONE TESTA
    private Transform trCenterEye;

    //CREAZIONE CSV:
    private int frameCounter = 1;
    private int frameInterval = 10; //ogni 10 frame
    private string header;
    private List<string> data = new List<string>();

    //OTTIMIZZAZIONE LISTA E SALVATAGGIO CSV
    private const int MAXLISTSIZE = 5000; //dati 70fps, svuoto la lista ogni 0.15sec * 2500 = 6.25 minuti
    private string fileCheckpointName = "";
    private bool continueCheckpoint = false;



    void Start()
    {
        //startAppTimeALL = Time.realtimeSinceStartup; //compreso il tempo in cui stai nel menu (not affected by pausing); Time.time //affected by pausing   
        //startAppTimeALL = Time.realtimeSinceStartup;
        continueCheckpoint = true;
        header = "Event; Time; Product; Rot:X ; Rot:Y ; Rot:Z; Pos:X; Pos:Y; Pos:Z";
        data.Insert(0, header);

        trCenterEye = cXRManager.GetTrCenterEye(); //non necessita di LateStart (persistente)
        _audioSource.Play();


        StartCoroutine(LateStart());
       

    }


 */

 /*
    void Update()
    {
        frameCounter++;
        if(frameCounter % frameInterval >= 0) //cambio ==
        {
            string eventHEADname = "HEAD";
            float headTime = Time.timeSinceLevelLoad;
            Vector3 trCenterEyeRotation = trCenterEye.rotation.eulerAngles;
            Vector3 trCenterEyePosition = trCenterEye.position;
            string HEADdata = $"{eventHEADname}; {headTime} ; ;{trCenterEyeRotation.x}; {trCenterEyeRotation.y}; {trCenterEyeRotation.z}; {trCenterEyePosition.x}; {trCenterEyePosition.y}; {trCenterEyePosition.z}";
            data.Add(HEADdata);
            if (isGrabbed)
            {
                string eventName = "GrabWhile";
                
                //LOCAL:
                //trShoesRotation = trShoes.localRotation.eulerAngles;
                //trShoesPosition = trShoes.localPosition;
                //GLOBAL:
                trShoesRotation = trShoes.rotation.eulerAngles;
                trShoesPosition = trShoes.position;

                string rotationData = $"{eventName}; {headTime}; {shoesNumber};{trShoesRotation.x}; " +
                    $"{trShoesRotation.y}; {trShoesRotation.z}; {trShoesPosition.x}; {trShoesPosition.y}; {trShoesPosition.z}";
                data.Add(rotationData);
            }
            frameCounter = 0;

            //OTTIMIZZAZIONE LISTA (TOGLIERE PER SALVATAGGIO STANDARD): la svuoto ogni 6.25 minuti
            if (data.Count >= MAXLISTSIZE && continueCheckpoint)
            {
                fileCheckpointName = "Checkpoint_Metrics.csv";
                StartCoroutine(SavePartialCSV(Path.Combine(Application.persistentDataPath, fileCheckpointName)));
            }
        }        


        //SALVATAGGIO CSV CON PRESSIONE DEL CONTROLLER
        if (OVRInput.GetDown(OVRInput.RawButton.B))
        {
            continueCheckpoint = false;
            if (endAppTimeEXP == 0) //togliere l'IF se si vuole prendere il tempo finale N volte
            {
                endAppTimeEXP = Time.timeSinceLevelLoad; //fine esperienza (dal caricamento scbase)
                string eventName = "EndEXP";
                string endData = $"{eventName}; {endAppTimeEXP}; ; ; ; ; ; ;";
                data.Add(endData);
                Debug.Log("Tempo trascorso dall'avvio scBase: " + endAppTimeEXP.ToString("F2") + " secondi");
            }
            //salvare i dati in un file csv            
            string fileName = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss_") + "Button_Metrics.csv";
            StartCoroutine(SaveCSV(Path.Combine(Application.persistentDataPath, fileName))); //funziona
        }
    }


    private IEnumerator LateStart()
    {
        yield return new WaitForSeconds(5f);
        _shoeses = FindObjectsOfType<cSlot>(); //devo, non posso dall'inspector in real time
        
        _sceneInfo = FindObjectOfType<cSceneInfo>();
        if(_sceneInfo!= null)
            _sceneInfo.OnButtonVideoPressed += OnMetricVideoPressed; //CAPIRE PERCHE DA NULL POINTER EXCEPTION FORSE PERCHE FIGLIO DI ALTRO OBJ

        //ALTERNATIVA 1 ACTIONS :
        foreach (cSlot _shoes in _shoeses)
        {
            //inscrivo tutte le scarpe agli eventi (cSlot)
            _shoes.OnShoesGrabbed += OnMetricShoesGrabbed;
            _shoes.OnShoesReleased += OnMetricShoesReleased;

            _shoes.OnInfoPressed += OnMetricInfoPressed;
            _shoes.OnXInfoPressed += OnMetricXInfoPressed;
            _shoes.OnFilterPressed += OnMetricFilterPressed;
            _shoes.OnXFilterPressed += OnMetricXFilterPressed;

            Debug.Log("Shoes: " + _shoes.name);
        }
        //ALTERNATIVA 2 USIAMO UN FIND DI TUTTI QUESTI OGGETTI E ONCLICK()
    }


    private void OnMetricShoesGrabbed(cSlot shoes, bool isGrabbed)
    {
        this.isGrabbed = isGrabbed; //true
        string eventName = "GrabIN";
        float grabTimeIN = Time.timeSinceLevelLoad; //dall'inizio di scBase; se mettessi in pausa il gioco, questo viene messo in pausa (come time.time)

        //SALVA TRANSFORM SCARPA presa, sevirà in update
        shoesNumber = shoes.getSlotID(); //numero scarpa (prodotto)
        trShoes = shoes.transform.GetChild(0); //transform figlio scarpa iniziale

        //LOCAL:
        //Vector3 trShoesRotationIN = trShoes.localRotation.eulerAngles;
        //Vector3 trShoesPositionIN = trShoes.localPosition;

        //GLOBAL:
        Vector3 trShoesRotationIN = trShoes.rotation.eulerAngles;
        Vector3 trShoesPositionIN = trShoes.position;

        if (isGrabbed)
        {
            //NomeEvento :: istanteTempo :: Product :: valoreAngolo
            string grabData = $"{eventName}; {grabTimeIN}; {shoesNumber}; {trShoesRotationIN.x}; " +
                $"{trShoesRotationIN.y}; {trShoesRotationIN.z}; {trShoesPositionIN.x}; {trShoesPositionIN.y}; {trShoesPositionIN.z}";
            data.Add(grabData);
        }
        Debug.Log("EVENTO= " + eventName + " TEMPO: " + grabTimeIN + " Shoes + " + shoesNumber + " ROTAZIONE: " + trShoesRotationIN + "POSIZIONE: " + trShoesPositionIN);
    }
    private void OnMetricShoesReleased(cSlot shoes, bool isGrabbed)
    {
        this.isGrabbed = isGrabbed; //false
        string eventName = "GrabEND";
        float grabTimeEND = Time.timeSinceLevelLoad;
        //trShoes viene presa dalla funzione OnMetricShoesGrabbed 1 volta : non rilasci una scarpa non grabbata prima
        
        //LOCAL:
        //Vector3 trShoesRotationEND = trShoes.localRotation.eulerAngles;
        //Vector3 trShoesPositionEND = trShoes.localPosition;

        //GLOBAL:
        Vector3 trShoesRotationEND = trShoes.rotation.eulerAngles;
        Vector3 trShoesPositionEND = trShoes.position;

        if (!isGrabbed)
        {
            string releaseData = $"{eventName}; {grabTimeEND}; {shoesNumber}; {trShoesRotationEND.x}; " +
                $"{trShoesRotationEND.y}; {trShoesRotationEND.z}; {trShoesPositionEND.x}; {trShoesPositionEND.y}; {trShoesPositionEND.z}";
            data.Add(releaseData);
        }
    }
    private void OnMetricInfoPressed(cSlot shoes, bool isInfoPressed)
    {
        string eventName = "InfoPressed";
        float pressedTime = Time.timeSinceLevelLoad;
        int shoesNumber = shoes.getSlotID();
        if (isInfoPressed)
        {
            string infoData = $"{eventName}; {pressedTime}; {shoesNumber}; ; ; ; ; ; ";
            data.Add(infoData);
        }
    }
    private void OnMetricFilterPressed(cSlot shoes, bool isFilterPressed)
    {
        string eventName = "FilterPressed";
        float pressedTime = Time.timeSinceLevelLoad;
        int shoesNumber = shoes.getSlotID();
        if(isFilterPressed)
        {
            string filterData = $"{eventName}; {pressedTime}; {shoesNumber}; ; ; ; ; ; ";
            data.Add(filterData);
        }
    }
    private void OnMetricXInfoPressed(cSlot shoes, bool isXInfoPressed)
    {
        string eventName = "XInfoPressed";
        float pressedTime = Time.timeSinceLevelLoad;
        int shoesNumber = shoes.getSlotID();
        if (isXInfoPressed)
        {
            string xInfoData = $"{eventName}; {pressedTime}; {shoesNumber}; ; ; ; ; ; ";
            data.Add(xInfoData);
        }
    }
    private void OnMetricXFilterPressed(cSlot shoes, bool isXFilterPressed)
    {
        string eventName = "XFilterPressed";
        float pressedTime = Time.timeSinceLevelLoad;
        int shoesNumber = shoes.getSlotID();
        if (isXFilterPressed)
        {
            string xFilterData = $"{eventName}; {pressedTime}; {shoesNumber}; ; ; ; ; ; ";
            data.Add(xFilterData);
        }
    }
    private void OnMetricVideoPressed()
    {
        //Se non si vuole acquisire anche la seconda volta che viene premuto il bottone video (per spegnere video)
        //bottone false -> premo -> true + salvo -> ripremo -> risetto a false e non faccio nulla
        if(!isVideoButtonPressed)
        {
            string eventName = "VideoButtonPressed";
            float pressedTime = Time.timeSinceLevelLoad;
            string videoData = $"{eventName}; {pressedTime}; ; ; ; ; ; ; ";
            data.Add(videoData);

            isVideoButtonPressed = true;
        }
        else
        {
            string eventName = "VideoButtonReleased";
            float pressedTime = Time.timeSinceLevelLoad;
            string videoData = $"{eventName}; {pressedTime}; ; ; ; ; ; ; ";
            data.Add(videoData);
            isVideoButtonPressed = false;
        }
        
        


    }



    private void OnDestroy()
    {
        //sfruttando XRManager : puoi usare il tempo onHMDMounted() 
        //per raccogliere metriche togliere visore, per il momento facciamola semplice
        if (endAppTimeEXP == 0) //togliere l'IF se si vuole prendere il tempo finale N volte
        {
            endAppTimeEXP = Time.timeSinceLevelLoad; //fine esperienza (dal caricamento scbase)
            string eventName = "EndEXP";
            string endData = $"{eventName}; {endAppTimeEXP}; ; ; ; ; ; ; ";
            data.Add(endData);
            Debug.Log("Tempo trascorso dall'avvio scBase: " + endAppTimeEXP.ToString("F2") + " secondi");
        }
        string fileName = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss_") + "AppQuit_Metrics.csv";
        SaveCSVQuit(Path.Combine(Application.persistentDataPath, fileName)); //funziona
                                                                            
        if (File.Exists(Path.Combine(Application.persistentDataPath, fileCheckpointName)))
        {
            string newFileName = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss_") + "Checkpoint_Metrics.csv";
            File.Move(Path.Combine(Application.persistentDataPath, fileCheckpointName), Path.Combine(Application.persistentDataPath, newFileName));
            Debug.Log("Checkpoint file renamed to: " + newFileName);
        }
    }



    private IEnumerator SaveCSV(string fullPath)
    {
        if (!data.Contains(header))
        {
            data.Insert(0, header);
        }
       
        fullPath = fullPath.Replace('\\', '/');
        if (data.Count == 0)
        {
            Debug.LogWarning("No data to save");
            //return;

            yield return new WaitForEndOfFrame();
        }
        //string header = "Event; Time; Product; Rot:X ; Rot:Y ; Rot:Z; Pos:X; Pos:Y; Pos:Z";
        //data.Insert(0, header);

        yield return new WaitForEndOfFrame();

        //scrivi i dati in un file csv
        if (!File.Exists(fullPath))
        {
            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(fullPath))
            {
                foreach (string line in data)
                {
                    sw.WriteLine(line);
                }
            }
        }

        Debug.Log("Data saved to: " + fullPath);

        if (File.Exists(Path.Combine(Application.persistentDataPath, fileCheckpointName)))
        {
            string newFileName = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss_") + "Checkpoint_Metrics.csv";
            File.Move(Path.Combine(Application.persistentDataPath, fileCheckpointName), Path.Combine(Application.persistentDataPath, newFileName));
            Debug.Log("Checkpoint file renamed to: " + newFileName);
        }
    }

    private IEnumerator SavePartialCSV(string fullPath)
    {
        fullPath = fullPath.Replace('\\', '/');
        if(data.Count == 0)
        {
            Debug.LogWarning("No data to save");
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForEndOfFrame();

        //scrivi i dati in un file csv -> Checkpoint
        if (!File.Exists(fullPath))
        {
            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(fullPath))
            {
                foreach (string line in data)
                {
                    sw.WriteLine(line);
                }
            }
        }
        //SE IL FILE ESISTE GIA' (checkpoint): se vuoi appenderli al file usa questo
        else
        {
            using (StreamWriter writer = File.AppendText(fullPath))
            {
                foreach (string line in data)
                {
                    writer.WriteLine(line);
                }
            }
        }
        Debug.Log("Data saved to: " + fullPath);
        data.Clear();
    }

    private void SaveCSVQuit(string fullPath)
    {
        fullPath = fullPath.Replace('\\', '/');
        if (data.Count == 0)
        {
            Debug.LogWarning("No data to save");
            return;
        }

        //scrivi i dati in un file csv
        if (!File.Exists(fullPath))
        {
            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(fullPath))
            {
                foreach (string line in data)
                {
                    sw.WriteLine(line);
                }
            }
        }
        Debug.Log("Data saved to: " + fullPath);
    }

*/
}

