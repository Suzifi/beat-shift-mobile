using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundGroup : MonoBehaviour
{
    public BeatClip[] clips;
    private int clipsPerGroup = 4;

    public Color mutedColor;
    public Color playingColor;
    public Color fontColor;

    public GameObject variationsGroup;
    public bool variationsOpen;

    public bool isVariationObject;

    private Vector3 orgPosition;

    private float sideMargin = 50f;

    private PadControl padControl;
    Animator animator;

    Slider volumeSlider;

    void Start()
    {
        //ScaleGroupItems();
        animator = GetComponent<Animator>();
        volumeSlider = GetComponentInChildren<Slider>();

        //clips = GetComponentsInChildren<BeatClip>();
        if(!isVariationObject && variationsGroup != null)
        {
            orgPosition = variationsGroup.GetComponent<RectTransform>().localPosition;
        }        

        padControl = GetComponentInParent<PadControl>();
        padControl.OnRandomPressed.AddListener(PlayRandom);
        padControl.OnVolumeControlToggle.AddListener(ToggleVolumeSliderVisible);

        ToggleVolumeSliderVisible();
    }

    
    void Update()
    {
        
    }

    public void MuteOthers(BeatClip clipToPlaySolo)
    {
        foreach (BeatClip clip in clips)
        {
            if(clip != clipToPlaySolo)
            {
                if(clip.isActive)
                {
                    clip.MuteClip();
                }
            }
        }
    }

    public void AdjustGroupVolume(float newVolume)
    {
        foreach (BeatClip clip in clips)
        {
            clip.SetVolume(newVolume);
        }
    }

    public void ToggleVolumeSliderVisible()
    {
        if(isVariationObject || volumeSlider == null)
        {
            return;
        }

        volumeSlider.gameObject.SetActive(padControl.volumeControlsVisible);
    }

    public BeatClip GetactiveClip()
    {
        foreach (BeatClip clip in clips)
        {
            if(clip.isActive)
            {
                return clip;
            }            
        }
        return clips[0];
    }

    public void PlayRandom()
    {
        int randomClip = Random.Range(0, clips.Length);

        clips[randomClip].StartPlaying();
    }

    public void ToggleVariations()
    {
        if(isVariationObject || variationsGroup == null)
        {
            return;
        }

        else if(!variationsOpen)
        {
            variationsGroup.GetComponent<RectTransform>().localPosition = Vector3.zero;
        }
        else
        {
            variationsGroup.GetComponent<RectTransform>().localPosition = orgPosition;
        }
        animator.SetTrigger("VarClicked");
        variationsOpen = !variationsOpen;
    }

    void ScaleGroupItems()
    {
        float screenWidth = Camera.main.pixelRect.width;
        float screenHeight = Camera.main.pixelRect.height;
        float groupSideLength = (screenWidth - 2 * sideMargin) / 2f;

        RectTransform rect = GetComponent<RectTransform>();
        float currentWidth = rect.sizeDelta.x;

        

        rect.sizeDelta = new Vector2(groupSideLength, groupSideLength);
        //GetComponent<GridLayoutGroup>().cellSize = new Vector2(groupSideLength / 2 - 25f, groupSideLength / 2 - 25f);
    }
}
