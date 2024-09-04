using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class BeatClip : MonoBehaviour, IPointerDownHandler
{
    public bool loopingClip;
    public bool isActive;
    public AudioSource audioSource;

    private PadControl padControl;
    public SoundGroup soundGroup;

    Animator animator;
    int siblingIndex;

    float maxVol;

    // Set clip max volume

    void Start()
    {
        padControl = FindObjectOfType<PadControl>();
        soundGroup = GetComponentInParent<SoundGroup>();
        audioSource = GetComponentInChildren<AudioSource>();

        maxVol = audioSource.volume;

        siblingIndex = transform.GetSiblingIndex();
        animator = soundGroup.gameObject.GetComponent<Animator>();

        GetComponentInChildren<TextMeshProUGUI>().color = soundGroup.fontColor;

        GetComponent<Image>().color = soundGroup.mutedColor;

        audioSource.mute = true;

        padControl.FirstBeatStarted.AddListener(audioSource.Play);
        padControl.AllSoundsMuted.AddListener(audioSource.Stop);
        padControl.OnStopPressed.AddListener(MuteClip);

        padControl.effect3DSlider.onValueChanged.AddListener(AdjustSpatialEffect);
        padControl.OnStopEffects.AddListener(StopSpatialEffect);

        padControl.effectPitchSlider.onValueChanged.AddListener(AdjustPitchEffect);
        padControl.OnStopPitchEffects.AddListener(StopPitchEffect);

        padControl.RangeLoopPointReached.AddListener(LoopStepBack);
        padControl.RangeLoopStopped.AddListener(LoopEffectStopped);
        //audioSource.Play();

    }

    /*public void StartClip()
    {
        if(!isActive)
        {
            return;
        }
        audioSource.Play();

        if(!loopingClip)
        {
            isActive = false;
        }
    }*/

    public void MuteClip()
    {
        audioSource.mute = true;
        isActive = false;
        GetComponent<Image>().color = soundGroup.mutedColor;
        padControl.soundsActive--;
    }

    public void SetVolume(float newVolume)
    {
        audioSource.volume = newVolume * maxVol;
    }

    public void ToggleActive()
    {
        
        isActive = !isActive;
        audioSource.mute = !isActive;

        if(isActive)
        {
            padControl.soundsActive++;
            soundGroup.MuteOthers(this);
            GetComponent<Image>().color = soundGroup.playingColor;
        }
        else
        {
            GetComponent<Image>().color = soundGroup.mutedColor;
            padControl.soundsActive--;
        }

        if (!padControl.startedPlaying)
        {
            padControl.startedPlaying = true;
            padControl.FirstBeatStarted?.Invoke();
        }
    }

    public void StartPlaying()
    {
        isActive = true;
        audioSource.mute = false;

        padControl.soundsActive++;
        soundGroup.MuteOthers(this);
        GetComponent<Image>().color = soundGroup.playingColor;

        if (!padControl.startedPlaying)
        {
            padControl.startedPlaying = true;
            padControl.FirstBeatStarted?.Invoke();
        }
    }

    public void AdjustSpatialEffect(float newValue)
    {
        audioSource.spatialBlend = newValue;
    }

    public void AdjustPitchEffect(float newValue)
    {
        audioSource.pitch = newValue;
    }

    public void StopPitchEffect()
    {
        audioSource.pitch = 1f;
    }

    public void StopSpatialEffect()
    {
        audioSource.spatialBlend = 0f;
    }

    public void LoopStepBack()
    {
        audioSource.time = padControl.loopRangeStartTime;
    }

    public void LoopEffectStopped()
    {
        audioSource.time = padControl.loopButtonHoldTimeCounter % audioSource.clip.length;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ToggleActive();
        animator.SetTrigger(siblingIndex + "Clicked");
        //StartCoroutine(ShowPressedEffect());
    }

    // Animates the button when pressed
    IEnumerator ShowPressedEffect()
    {
        GetComponent<RectTransform>().localScale = new Vector2(1.1f, 1.1f);
        float normalScale = 1f;
        float biggerScale = 1.1f;

        float speed = 0.2f;
        float time = 0f;

        while (time < speed)
        {
            float newScale = Mathf.Lerp(biggerScale, normalScale, time / speed);
            GetComponent<RectTransform>().localScale = new Vector2(newScale, newScale);
            time += Time.deltaTime;
            yield return null;
        }

        GetComponent<RectTransform>().localScale = new Vector2(1f, 1f);
    }
}
