using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OneShotClip : MonoBehaviour, IPointerDownHandler
{
    public AudioClip audioClip;
    AudioSource audioSource;

    public Image fillerImage;

    public float clipLength;
    float fillAmount;

    Animator anim;
    private PadControl padControl;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        clipLength = audioClip.length;
        anim = GetComponent<Animator>();
        padControl = FindObjectOfType<PadControl>();
        padControl.OnStopPressed.AddListener(CancelEffect);
    }

    
    void Update()
    {
        
    }

    public void PlayEffectOnce()
    {
        // If already playing, stop and restart
        if(audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        anim.SetBool("Playing", true);
        audioSource.PlayOneShot(audioClip);
        StartCoroutine(UpdateFillerImage());
    }

    public void CancelEffect()
    {
        audioSource.Stop();
        anim.SetBool("Playing", false);
    }

    IEnumerator UpdateFillerImage()
    {
        float timeElapsed = 0f;
        while (timeElapsed < clipLength)
        {
            fillerImage.fillAmount = 1 - (timeElapsed / clipLength);
            //Debug.Log("Setting fill " + timeElapsed);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        anim.SetBool("Playing", false);
        fillerImage.fillAmount = 1f;
        yield return null;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        PlayEffectOnce();
    }
}
