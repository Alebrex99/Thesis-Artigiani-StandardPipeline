using System;
using System.Collections.Generic;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;

using UnityEditor.PackageManager;
using System.Collections;
using System.Buffers.Text;
using static SocketIOUnity;
using TMPro.Examples;


public class cSocketManager : MonoBehaviour
{
    public SocketIOUnity socket;
    private bool isConnected = false;
    [SerializeField] private GameObject objectToSpin;
    //MESSAGGIO DA INVIARE DA VOICE -> TO TEXT
    private string message = "Hello from Unity!";

    // Start is called before the first frame update
    void Start()
    {
        //TODO: check the Uri if Valid.
        //var uri = new Uri("http://192.168.1.107:11100"); //DEFAULT: IP non corretto
        //var uri = new Uri("http://localhost:11100"); //Funziona con SERVER DI PROVA
        var uri = new Uri("http://localhost:5000"); //Funziona con server MIKEL; bisognerà poi modificare l'indirizzo con uno internet
        socket = new SocketIOUnity(uri, new SocketIOOptions
        {
            Query = new Dictionary<string, string>
                {
                    {"token", "UNITY" }
                }
            ,
            EIO = 4
            ,
            Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
        });
        socket.JsonSerializer = new NewtonsoftJsonSerializer();

        //-------------- reserved socketio events-----------------------
        /*Un "ping" è un messaggio inviato dal client al server per verificare se 
         * il server è ancora raggiungibile e per misurare il tempo di latenza 
         * tra il client e il server. Il server risponderà quindi con un "pong" 
         * per confermare la ricezione del messaggio e includerà informazioni 
         * sulla latenza, come ad esempio il tempo impiegato per ricevere 
         * il messaggio di ping.*/
        socket.OnConnected += (sender, e) =>
        {
            Debug.Log("socket.OnConnected");
            socket.Emit("chat_message", "hola"); //message to INIT, chiama internamente EmitAsync
            Debug.Log("Initial message sent from connect");
        };
        socket.OnPing += (sender, e) =>
        {
            Debug.Log("Ping");
        };
        socket.OnPong += (sender, e) =>
        {
            Debug.Log("Pong: " + e.TotalMilliseconds);
        };
        socket.OnDisconnected += (sender, e) =>
        {
            Debug.Log("Disconnected from server: " + e);
            isConnected = false;
        };
        socket.OnReconnectAttempt += (sender, e) =>
        {
            Debug.Log($"{DateTime.Now} Reconnecting: attempt = {e}");
        };
        //--------------------------------------------------------------
    
        Debug.Log("Connecting...");
        socket.Connect();

        //------------------- RECEIVING = REACTIONS TO SERVER (Unity wrapper) ------------------------
        //ON UNITY THREAD = SOLO CON PLAYER PREFS SYSTEM, PER EFFETTO SU OGGETTI
        /* Set (unityThreadScope) the thread scope function where the code should run.
        Options are: .Update, .LateUpdate or .FixedUpdate, default: UnityThreadScope.Update */
        socket.unityThreadScope = UnityThreadScope.Update; //dove tale thread sta andando
        socket.OnUnityThread("audio_response_end", (response) =>
        {
            objectToSpin.transform.Rotate(0, 45, 0);
        });
        //in teoria non usarlo per i chunk AUDIO
        socket.OnUnityThread("audio_response_chunk", (response) =>
        {
            Debug.Log("OnUnityThread: " + response.ToString());
            /*var base64String = response.GetValue<string>(); //prima: GetValue<string>("audio_chunk")
            var chunk = Convert.FromBase64String(base64String);
            Debug.Log($"Received chunk of length {chunk.Length}");*/
        });
        //ReceivedText.text = "";
        socket.OnAnyInUnityThread((name, response) =>
        {
            //ReceivedText.text += "Received On " + name + " : " + response.GetValue<string>() + "\n"; //response.GetValue().GetRawText()
        });


        //----------------RECEIVING = REACTIONS TO SERVER (SocketIO)------------------------
        //receives CHUNKS FROM SERVER : Equivalente delle funzioni sopra, ma prese dirette da SOcketIO (tanto SocketIO Unity estende socketIO)
        socket.On("audio_response_chunk", response =>
        {
            Debug.Log("Audio response chunk: " + response.ToString());
            //NON FUNZIONANO I SEGUENTI: non stampano nulla (guardare la GetValue()
            var base64String = response.GetValue<string>(); //prima: GetValue<string>("audio_chunk"); possible: data[index]
            var chunk = Convert.FromBase64String(base64String);
            Debug.Log($"Received chunk of length {chunk.Length}");

        });

        socket.On("audio_response_end", response =>
        {
            Debug.Log("Audio response end: " + response.ToString());
            objectToSpin.transform.Rotate(0, 45, 0);
        });
    }

    void Update()
    {
        //
        if (OVRInput.GetDown(OVRInput.RawButton.B))
        {
            socket.Emit("chat_message", 123);
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            socket.Emit("chat_message", 123); //Possibilità 1
            StartCoroutine(SendMessages(message)); //send message epresso da USER
        }
    }



    IEnumerator SendMessages(string message) //params -> ENTERO MESSAGE TO SEND
    {
        //TESTING
        /*
        while (true)
        {
            if (isConnected)
            {
                //TRASCRITION THE AUDIO
                Debug.Log("Enter message to send: ");
                //string message = Console.ReadLine(); //TO READ FROM terminal: passare message (TEXT) como parametro
                if (message.ToLower() == "exit")
                {
                    StartCoroutine(Disconnect());
                    yield break;
                }
                client.EmitAsync("chat_message", message).Wait();
                Debug.Log("Message sent");
            }
            yield return null;
        }*/

        //TO SEND MESSAGES FROM USER REALMEMENT, NON CON CONSOLE
        if (isConnected)
        {
            //TRASCRITION THE AUDIO
            Debug.Log("Enter message to send: ");
            //string message = Console.ReadLine(); //TO READ FROM temrminal: passare message (TEXT) como parametro
            if (message.ToLower() == "exit")
            {
                StartCoroutine(Disconnect());
                yield break;
            }
            socket.EmitAsync("chat_message", message).Wait();
            Debug.Log("Message sent");
        }
        yield return null;

    }

    IEnumerator Disconnect()
    {
        if (socket != null)
        {
            yield return socket.DisconnectAsync();
            isConnected = false;
        }
    }

    void OnApplicationQuit()
    {
        StartCoroutine(Disconnect());
    }







    public void EmitTest()
    {
        //string eventName = EventNameTxt.text.Trim().Length < 1 ? "hello" : EventNameTxt.text;
        //string txt = DataTxt.text;
        /*
        if (!IsJSON(txt))
        {
            socket.Emit(eventName, txt);
            //socket.Emit(txt); //il server da errore perchè vuole un JSON= json.decoder.JSONDecodeError: Expecting ',' delimiter: line 1 column 4 (char 3)

        }
        else
        {
            socket.EmitStringAsJSON(eventName, txt);
        }*/
    }

    public static bool IsJSON(string str)
    {
        if (string.IsNullOrWhiteSpace(str)) { return false; }
        str = str.Trim();
        if ((str.StartsWith("{") && str.EndsWith("}")) || //For object
            (str.StartsWith("[") && str.EndsWith("]"))) //For array
        {
            try
            {
                var obj = JToken.Parse(str);
                return true;
            }catch (Exception ex) //some other exception
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public void EmitSpin()
    {
        socket.Emit("spin");
    }

    public void EmitClass()
    {
        TestClass testClass = new TestClass(new string[] { "foo", "bar", "baz", "qux" });
        TestClass2 testClass2 = new TestClass2("lorem ipsum");
        socket.Emit("class", testClass2);
    }

    // our test class
    [System.Serializable]
    class TestClass
    {
        public string[] arr;

        public TestClass(string[] arr)
        {
            this.arr = arr;
        }
    }

    [System.Serializable]
    class TestClass2
    {
        public string text;

        public TestClass2(string text)
        {
            this.text = text;
        }
    }
    //
}