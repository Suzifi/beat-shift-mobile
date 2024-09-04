using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.IO;
using System;
#if UNITY_ANDROID || UNITY_IOS
using NativeShareNamespace;
#endif

public class AudioRecorder : MonoBehaviour
{
    int bufferSize;
    int bufferCount;

    int headerSize = 44; // standard wav
    int outputRate = 44100;

    public string fileName = "BeatClip1.wav";
    public string filePath;

    public bool recording;
    bool recStarted;
    bool headersStarted;

    bool recButtonPressed;

    bool readyToWrite;

    FileStream fs;

    float maxRecordLength = 300f;
    float recordStartedTime;

    public UnityEvent OnRecStarted;
    public UnityEvent OnRecEnded;

    private void Awake()
    {
        AudioConfiguration audioConfiguration = AudioSettings.GetConfiguration();

        outputRate = audioConfiguration.sampleRate;
        Debug.Log("Samplerate: " + outputRate);
        //AudioSettings.Reset(audioConfiguration);
        //outputRate = AudioSettings.outputSampleRate;
    }

    void Start()
    {
        // Create /videos subdirectory if it doesn't exist
        if (!Directory.Exists(Application.persistentDataPath + "/music"))
        {
            DirectoryInfo dInfo = Directory.CreateDirectory(Application.persistentDataPath + "/music");
        }

        AudioSettings.GetDSPBufferSize(out bufferSize, out bufferCount);

        // Save with the scene name
        fileName = SceneManager.GetActiveScene().name + ".wav";
    }

    public void ToggleRecord()
    {
        recording = !recording;

        if (recording)
        {
            OnRecStarted?.Invoke();
            recordStartedTime = Time.timeSinceLevelLoad;
            string dateTimeString = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");

            fileName = "BeatShift_" + dateTimeString + "_" + fileName;
            filePath = Application.persistentDataPath + "/music/" + fileName;

            //AudioListener.volume = 0.5f;
            StartWriting(fileName);
            recStarted = true;
        }
        else
        {
            OnRecEnded?.Invoke();
            readyToWrite = false;
            //AudioListener.volume = 1f;
            WriteHeader();
        }
        recButtonPressed = true;
    }

    void Update()
    {
        if (recording)
        {
            // Automatically stop rec when the file is max length
            if (Time.timeSinceLevelLoad - recordStartedTime > maxRecordLength)
            {
                Debug.Log("5 min recorded!");
                ToggleRecord();
            }
            //ToggleRecord();

            /*if (recording && !recStarted)
            {
                StartWriting(fileName);
                recStarted = true;
            }

            else if (!headersStarted)
            {
                readyToWrite = false;
                //recStarted = false;
                WriteHeader();

            }*/

        }

    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        // HERE this never happens on the phone
        //Debug.Log("Filter read");
        if (readyToWrite)
        {
            ConvertAndWrite(data);
            Debug.Log("Filter read and ready to write");
        }
    }

    void StartWriting(string fileName)
    {
        // Create the file
        if (!File.Exists(filePath))
        {

            var emptyByte = new byte();

            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
            {
                for (int i = 0; i < headerSize; i++)
                {
                    fs.WriteByte(emptyByte);
                }

                fs.Flush();
            }
            Debug.Log("File made in " + filePath);

            readyToWrite = true;
            //WriteHeader();
        }

    }

    void ConvertAndWrite(float[] dataRange)
    {
        Int16[] intData = new Int16[dataRange.Length];
        Byte[] bytesData = new Byte[dataRange.Length * 2];

        float rescaleFactor = 32767; // to convert float to Int16

        for (int i = 0; i < dataRange.Length; i++)
        {
            intData[i] = (short)(dataRange[i] * rescaleFactor);
            Byte[] byteArr = new Byte[2];
            byteArr = BitConverter.GetBytes(intData[i]);
            byteArr.CopyTo(bytesData, i * 2);
        }

        using (var stream = new FileStream(filePath, FileMode.Append, FileAccess.Write))
        {
            stream.Write(bytesData, 0, bytesData.Length);
            Debug.Log("Appended");
        }
    }

    // Write the header of the wav file
    void WriteHeader()
    {
        if (headersStarted)
        {
            return;
        }

        headersStarted = true;
        Debug.Log("Making headers");

        FileStream fs = new FileStream(filePath, FileMode.Open);

        fs.Seek(0, SeekOrigin.Begin);

        Byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
        fs.Write(riff, 0, 4);

        Byte[] chunkSize = BitConverter.GetBytes(fs.Length - 8);
        fs.Write(chunkSize, 0, 4);

        Byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
        fs.Write(wave, 0, 4);

        Byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
        fs.Write(fmt, 0, 4);

        Byte[] subChunk = BitConverter.GetBytes(16);
        fs.Write(subChunk, 0, 4);

        UInt16 two = 2;
        UInt16 one = 1;

        Byte[] audioFormat = BitConverter.GetBytes(one);
        fs.Write(audioFormat, 0, audioFormat.Length);

        Byte[] numChannels = BitConverter.GetBytes(two);
        fs.Write(numChannels, 0, numChannels.Length);

        Byte[] sampleRate = BitConverter.GetBytes(outputRate);
        fs.Write(sampleRate, 0, sampleRate.Length);

        Byte[] byteRate = BitConverter.GetBytes(outputRate * 4); // sampleRate * bytesPerSample*number of channels, here 44100*2*2
        fs.Write(byteRate, 0, 4);

        UInt16 four = 4;
        Byte[] blockAlign = BitConverter.GetBytes(four);
        fs.Write(blockAlign, 0, 2);

        UInt16 sixteen = 16;
        Byte[] bitsPerSample = BitConverter.GetBytes(sixteen);
        fs.Write(bitsPerSample, 0, 2);

        Byte[] dataString = System.Text.Encoding.UTF8.GetBytes("data");
        fs.Write(dataString, 0, 4);

        Byte[] subChunk1 = BitConverter.GetBytes(fs.Length - headerSize);
        fs.Write(subChunk1, 0, 4);

        fs.Close();

        recButtonPressed = false;
        ShareFile();
        Debug.Log("Headers done");
    }

    // Use NativeShare to save or share the file
    public void ShareFile()
    {
        new NativeShare().AddFile(filePath)
        .SetCallback((result, shareTarget) =>
        {
            Debug.Log("Share result: " + result + ", selected app: " + shareTarget + ". Deleting file!");
            DeleteFile();
        })
        .Share();
    }

    public void DeleteFile()
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
}
