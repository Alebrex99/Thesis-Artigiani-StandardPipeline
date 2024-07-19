using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class cRecorder : MonoBehaviour
{
    public static cRecorder instance;

    public string userName = "";
    public string activity = "";
    public string selectedMicrophone;
    private AudioClip _audioClip;
    public bool _isRecording = false;
    private string _filePath;
    private int _minFrequency;
    private int _maxFrequency;
    public float volumeMultiplier = 2.0f; // Factor de amplificación del volumen
    public float voiceThreshold = 0.02f;  // Umbral para detección de voz
    public bool isVoiceDetected = false;  // Indica si se está detectando voz

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (Input.GetKeyDown(KeyCode.F8))
            StartRecording();
        if (Input.GetKeyDown(KeyCode.F9))
            StopRecording();

        DontDestroyOnLoad(gameObject);

        if (Microphone.devices.Length > 0)
        {
            selectedMicrophone = Microphone.devices[0];
        }

        for (int i = 0; i < Microphone.devices.Length; i++)
        {
            if (Microphone.devices[i].Contains("Oculus"))
            {
                selectedMicrophone = Microphone.devices[i];
                break;
            }
        }
    }

    public void StartRecording()
    {
        if (_isRecording) return;
        StartCoroutine(StartRecordingCoroutine());
    }

    private IEnumerator StartRecordingCoroutine()
    {
        if (Microphone.devices.Length > 0)
        {
            Microphone.GetDeviceCaps(selectedMicrophone, out _minFrequency, out _maxFrequency);
            //_audioClip = Microphone.Start(selectedMicrophone, true, 60, _maxFrequency > 0 ? _maxFrequency : 44100);
            _audioClip = Microphone.Start(selectedMicrophone, false, 130, AudioSettings.outputSampleRate);

            while (!Microphone.IsRecording(selectedMicrophone))
                yield return new WaitForEndOfFrame(); // Dejar que se procese un frame

            yield return new WaitForEndOfFrame();

            _isRecording = true;
            StartCoroutine(DetectVoice());
        }
    }

    public void StopRecording()
    {
        if (!_isRecording) return;
        StartCoroutine(StopRecordingCoroutine());
    }

    private IEnumerator StopRecordingCoroutine()
    {
        yield return new WaitForEndOfFrame(); // BORRAR

        Debug.Log("StopRecordingCoroutine started");
        int recordingLength = Microphone.GetPosition(selectedMicrophone);
        Debug.Log("Got recording length: " + recordingLength);
        Microphone.End(selectedMicrophone);
        Debug.Log("Microphone ended");


        if (recordingLength > 0)
        {
            float[] samples = new float[recordingLength * _audioClip.channels];
            Debug.Log("Created samples array with length: " + samples.Length);
            _audioClip.GetData(samples, 0);
            Debug.Log("Got audio data");

            // Aplicar amplificación de volumen
            for (int i = 0; i < samples.Length; i++)
            {
                samples[i] *= volumeMultiplier;
            }
            Debug.Log("Amplified audio data");

            AudioClip recordedClip = AudioClip.Create(_audioClip.name, recordingLength, _audioClip.channels, _audioClip.frequency, false);
            recordedClip.SetData(samples, 0);
            Debug.Log("Created recordedClip and set data");
            yield return SaveRecording(recordedClip);
            Debug.Log("Finished SaveRecording");
        }


        _isRecording = false;
        Debug.Log("StopRecordingCoroutine finished");
    }

    private IEnumerator SaveRecording(AudioClip clip)
    {
        Debug.Log("SaveRecording started");
        string folder = "_recordFiles";
        string directoryPath = Path.Combine(Application.persistentDataPath, folder, userName, activity);

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
            Debug.Log("Created directory: " + directoryPath);
        }

        _filePath = Path.Combine(directoryPath, "recordedAudio" + "_" + DateTime.Now.ToString("ddMMyyyy_hhmmss") + ".wav");

        var samples = new float[clip.samples];
        clip.GetData(samples, 0);
        Debug.Log("Got clip data");

        byte[] wavFile = ConvertToWav(samples, clip.channels, clip.frequency);
        Debug.Log("Converted to wav");

        File.WriteAllBytes(_filePath, wavFile);
        Debug.Log("Wrote wav file to disk: " + _filePath);

        yield return null; // Dejar que se procese un frame
        Debug.Log("SaveRecording finished");
    }

    private byte[] ConvertToWav(float[] samples, int channels, int frequency)
    {
        MemoryStream stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);

        int sampleCount = samples.Length;
        int byteRate = frequency * channels * 2;
        int blockAlign = channels * 2;

        writer.Write(new char[4] { 'R', 'I', 'F', 'F' });
        writer.Write(36 + sampleCount * 2);
        writer.Write(new char[4] { 'W', 'A', 'V', 'E' });
        writer.Write(new char[4] { 'f', 'm', 't', ' ' });
        writer.Write(16);
        writer.Write((short)1);
        writer.Write((short)channels);
        writer.Write(frequency);
        writer.Write(byteRate);
        writer.Write((short)blockAlign);
        writer.Write((short)16);
        writer.Write(new char[4] { 'd', 'a', 't', 'a' });
        writer.Write(sampleCount * 2);

        foreach (var sample in samples)
        {
            short intSample = (short)Mathf.Clamp(sample * 32767, short.MinValue, short.MaxValue);
            writer.Write(intSample);
        }

        writer.Seek(4, SeekOrigin.Begin);
        writer.Write((int)writer.BaseStream.Length - 8);

        writer.Close();
        return stream.ToArray();
    }

    private IEnumerator DetectVoice()
    {
        float[] samples = new float[256];
        while (_isRecording)
        {
            int position = Microphone.GetPosition(selectedMicrophone);
            if (position < samples.Length) continue;

            _audioClip.GetData(samples, position - samples.Length);
            isVoiceDetected = IsVoiceDetected(samples);

            if (isVoiceDetected)
                yield return new WaitForSeconds(0.35f);
            else
                yield return new WaitForEndOfFrame();
        }
    }

    private bool IsVoiceDetected(float[] samples)
    {
        float sum = 0;
        foreach (var sample in samples)
        {
            sum += Mathf.Abs(sample);
        }
        float average = sum / samples.Length;

        // Detectar voz si el promedio de amplitud supera el umbral
        return average > voiceThreshold;
    }
}
