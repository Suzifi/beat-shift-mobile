using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;

public class PadControl : MonoBehaviour
{
    public float packageMaxVol = 1f;

    [Header("Clip info")]
    public float bpm;

    [SerializeField] private AudioClip sampleLengthClip;

    public float completeLoopFrequency;
    public float completeLoopTimeCounter;

    public bool startedPlaying;

    public int soundsActive;

    public bool volumeControlsVisible;

    [Header("Objects")]
    public Image beatFillerImage;

    public Slider effect3DSlider;
    public Slider effectPitchSlider;

    [Header("Loop effect settings")]
    public bool loopSmallRange;
    public float loopRangeStartTime;
    float loopRangeEndTime;
    public float loopButtonHoldTimeCounter;

    public float loopRangeDur;
    public int loopRangeDurDivider = 2; // To control the length
    public float loopRangeDurCounter;

    public UnityEvent FirstBeatStarted;
    public UnityEvent AllSoundsMuted;
    public UnityEvent RangeLoopPointReached;
    public UnityEvent RangeLoopStopped;

    public UnityEvent OnStopPressed;
    public UnityEvent OnRandomPressed;
    public UnityEvent OnStopEffects;
    public UnityEvent OnStopPitchEffects;
    public UnityEvent OnVolumeControlToggle;

    public Image recButton;
    public Color recButtonOriginal;
    public Color recButtonRecording;

    AudioRecorder recorder;

    void Start()
    {
        AudioListener.volume = packageMaxVol;

        completeLoopFrequency = sampleLengthClip.length;
        loopRangeDur = 60 / bpm / loopRangeDurDivider;

        recorder = Camera.main.GetComponent<AudioRecorder>();
        recorder.OnRecStarted.AddListener(SetRecording);
        recorder.OnRecEnded.AddListener(SetNotRecording);

        AllowScreenSleeping(false);
    }


    void Update()
    {
        if (startedPlaying && soundsActive == 0)
        {
            startedPlaying = false;
            completeLoopTimeCounter = 0f;
            beatFillerImage.fillAmount = 0f;
            AllSoundsMuted?.Invoke();
        }
        if (startedPlaying)
        {
            if (completeLoopTimeCounter >= completeLoopFrequency)
            {
                beatFillerImage.fillAmount = 0f;
                completeLoopTimeCounter = 0f;
            }
            else
            {
                beatFillerImage.fillAmount = completeLoopTimeCounter / completeLoopFrequency;
                completeLoopTimeCounter += Time.deltaTime;
            }

        }
        if (loopSmallRange)
        {
            if (loopRangeDurCounter >= loopRangeDur)
            {
                RangeLoopPointReached?.Invoke();
                loopRangeDurCounter = 0f;
            }
            else
            {
                loopRangeDurCounter += Time.deltaTime;
            }
            loopButtonHoldTimeCounter += Time.deltaTime;
        }
    }

    void SetRecording()
    {
        AudioListener.volume = packageMaxVol * 0.6f;
        recButton.color = recButtonRecording;
    }

    void SetNotRecording()
    {
        AudioListener.volume = packageMaxVol;
        recButton.color = recButtonOriginal;
    }

    public void StartLoopingRange()
    {
        loopRangeStartTime = GetComponentInChildren<AudioSource>().time;

        float bufferTime = loopRangeDur - (loopRangeStartTime % loopRangeDur);

        loopRangeStartTime += bufferTime;

        loopButtonHoldTimeCounter = loopRangeStartTime;
        loopRangeEndTime = loopRangeStartTime + loopRangeDur;

        loopRangeDurCounter = loopRangeDur;

        loopSmallRange = true;
    }


    public void StopLoopingRange()
    {
        loopSmallRange = false;

        RangeLoopStopped?.Invoke();
    }

    public void StopAllSounds()
    {
        Debug.Log("Stopping");
        OnStopPressed?.Invoke();
        soundsActive = 0;
        startedPlaying = false;
        completeLoopTimeCounter = 0f;
        beatFillerImage.fillAmount = 0f;
        AllSoundsMuted?.Invoke();
    }

    public void PlayRandom()
    {
        OnRandomPressed?.Invoke();
    }

    public void StopSpatialEffect()
    {
        OnStopEffects?.Invoke();
    }
    public void StopPitchEffect()
    {
        OnStopPitchEffects?.Invoke();
    }

    public void AdjustVolume(float newVolume)
    {
        AudioListener.volume = newVolume;
    }

    public void ToggleVolumeControls()
    {
        volumeControlsVisible = !volumeControlsVisible;
        OnVolumeControlToggle?.Invoke();
    }

    public void AllowScreenSleeping(bool allow)
    {
        if (allow)
        {
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
        }
        else
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
    }

    public void GoToMenu()
    {
        GameManager.manager.GoToMenu();
    }

    public void Rate()
    {
        Application.OpenURL("market://details?id=" + Application.identifier);
    }
}
