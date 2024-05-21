using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.IO;

public class ReadText : MonoBehaviour
{
    public static ReadText instance;
    [SerializeField] private TextMeshProUGUI myText;
    private string filePath;
    private int currentLineIndex = 0;

    //CSV
    private List<string> configData = new List<string>();

    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        filePath = Application.persistentDataPath + "/append.txt";
        //myText.text = GetLineAtIndex(currentLineIndex);

        //CSV
        filePath = Application.persistentDataPath + "/config.csv";
        StartCoroutine(ReadCSVFile(filePath));
    }

    private IEnumerator ReadCSVFile(string filePath)
    {
        filePath = filePath.Replace("\\", "/");
        yield return new WaitForEndOfFrame();

        if(!File.Exists(filePath))
        {
            Debug.LogError("File not found: " + filePath);
            yield break;
        }
        else
        {
            using (StreamReader sr = new StreamReader(filePath))
            {
                bool endOfFile = false;
                while (!endOfFile)
                {
                    string line = sr.ReadLine();
                    if (line == null)
                    {
                        endOfFile = true;
                        break;
                    }
                    //usa Array
                    /*var data_values = line.Split(';');
                    Debug.Log("Data: " + data_values[0].ToString() + " " + data_values[1].ToString());*/
                    
                    //Usa Lista
                    configData.Add(line);
                    Debug.Log("Line: " + line);
                }
            }
        }

        /*StreamReader streamReader = new StreamReader(filePath);
        bool endOfFile = false;
        while (!endOfFile)
        {
           
            string data_String = streamReader.ReadLine();
            if (data_String == null)
            {
                endOfFile = true;
                break;
            }
            var data_values = data_String.Split(',');
            Debug.Log("Data: " + data_values[0].ToString() + " " + data_values[1].ToString() + " " + data_values[2].ToString());
        }*/
    }





    //TXT
    private string GetLineAtIndex(int index)
    {
        string[] lines = File.ReadAllLines(filePath);
        if(index < lines.Length)
        {
            return lines[index];
        }
        else
        {
            return "NO MORE LINES.";
        }
    }

    public void NextLine()
    {
        currentLineIndex++;
        myText.text = GetLineAtIndex(currentLineIndex);
    }


}
