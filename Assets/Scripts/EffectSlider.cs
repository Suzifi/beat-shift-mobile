using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EffectSlider : MonoBehaviour, IDragHandler
{
    public Image effectPad;
    public Transform handleImg;
    float padWidth;
    float padHeight;
    float handlePosOffset;

    AudioListener audioListener;

    [Header("Settings")]
    public float minLowPass = 900f;
    public float maxLowPass = 7000f;
    public float minHighPass = 0f;
    public float maxHighPass = 5000f;

    float highPassDefault;
    float lowPassDefault;

    private AudioHighPassFilter highPassFilter;
    private AudioLowPassFilter lowPassFilter;

    void Start()
    {
        audioListener = FindObjectOfType<AudioListener>();
        padWidth = effectPad.rectTransform.rect.width;
        padHeight = effectPad.rectTransform.rect.height;
        handlePosOffset = handleImg.GetComponent<RectTransform>().rect.width / 2f;

        highPassFilter = audioListener.GetComponent<AudioHighPassFilter>();
        lowPassFilter = audioListener.GetComponent<AudioLowPassFilter>();

        highPassDefault = highPassFilter.cutoffFrequency;
        lowPassDefault = lowPassFilter.cutoffFrequency;

        Debug.Log("Rect: " + effectPad.rectTransform.rect.xMin + " " + effectPad.rectTransform.rect.xMax + " width " + padWidth);
    }

    
    void Update()
    {
        
    }


    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Dragging");
        AddEffect(eventData);
        //
        //
        handleImg.position = eventData.position;

        if (handleImg.localPosition.x < 0f + handlePosOffset)
        {
            Debug.Log("Out of bouds");
            handleImg.localPosition = new Vector2(0f + handlePosOffset, handleImg.localPosition.y);
        }
        if (handleImg.localPosition.x > padWidth - handlePosOffset)
        {
            handleImg.localPosition = new Vector2(padWidth - handlePosOffset, handleImg.localPosition.y);
        }
        if (handleImg.localPosition.y < handlePosOffset + padHeight / -2f)
        {
            handleImg.localPosition = new Vector2(handleImg.localPosition.x, handlePosOffset + padHeight / -2f);
        }
        if (handleImg.localPosition.y > padHeight / 2f - handlePosOffset)
        {
            handleImg.localPosition = new Vector2(handleImg.localPosition.x, padHeight / 2 - handlePosOffset);
        }
    }

    public void AddEffect(PointerEventData eventData)
    {

        // Is the pointer inside of the image
        //if(RectTransformUtility.ScreenPointToLocalPointInRectangle(effectPad.rectTransform, eventData.position, Camera.main, out localPosition))
        if(eventData != null)
        {            
            // Pointer position related to the progress bar rect transform ends
            //float newFxPercent = Mathf.InverseLerp(effectPad.rectTransform.rect.xMin, effectPad.rectTransform.rect.xMax, localPosition.x);
            float newFxPercent = Mathf.InverseLerp(effectPad.rectTransform.position.x, effectPad.rectTransform.position.x + padWidth, handleImg.localPosition.x);
            Debug.Log("Should change " + effectPad.rectTransform.position.x + " " + effectPad.rectTransform.position.x + padWidth + " local x " + handleImg.localPosition.x + " " + newFxPercent);

            // Move the handle image to the pointer position
            //handleImg.localPosition = new Vector2(padWidth * newFxPercent, 0f);

            if (newFxPercent < 0.5f)
            {
                //float midPoint = (minLowPass + maxLowPass) / 2f;
                float newFrequency = Mathf.InverseLerp(minLowPass, maxLowPass, (newFxPercent * maxLowPass * 2f));
                newFrequency *= maxLowPass;

                if(newFrequency < minLowPass)
                {
                    newFrequency = minLowPass;
                }

                lowPassFilter.cutoffFrequency = newFrequency;
                //highPassFilter.cutoffFrequency = highPassDefault;
            }
            else
            {
                float percentage = newFxPercent - 0.5f;
                float newFrequency = Mathf.InverseLerp(minHighPass, maxHighPass, (percentage * maxHighPass * 2f));
                newFrequency *= maxHighPass;
                highPassFilter.cutoffFrequency = newFrequency;
                //lowPassFilter.cutoffFrequency = lowPassDefault;
            }
        }
    }

    public void ResetHandlePosition()
    {
        StartCoroutine(MoveHandleToOrigin());
    }

    IEnumerator MoveHandleToOrigin()
    {
        float moveTime = 0.2f;
        float timeElapsed = 0.03f;
        Vector2 startPos = handleImg.localPosition;
        Vector2 targetPos = new Vector2(padWidth / 2f, 0f);

        while (timeElapsed < moveTime)
        {
            handleImg.localPosition = Vector2.Lerp(startPos, targetPos, timeElapsed / moveTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        handleImg.localPosition = targetPos;

    }
}
