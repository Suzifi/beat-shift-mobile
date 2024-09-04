using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResetScrollPosition : MonoBehaviour
{
    public float resetPosLeft;
    public float resetPosRight;

    public float cellWidth = 400f;

    float scrollDuration = 0.2f;
    bool scrolling;

    public Transform resetPosTrLeft;

    private void Awake()
    {
        cellWidth = transform.GetChild(1).localPosition.x - transform.GetChild(0).localPosition.x;

        //resetPosLeft = transform.GetChild(0).position.x - cellWidth * 2f;
        resetPosLeft = resetPosTrLeft.position.x;
        resetPosRight = transform.GetChild(transform.childCount - 1).position.x;
        
    }


    IEnumerator ScrollOneStep(int direction) // 1 for right, -1 for left
    {
        scrolling = true;

        Vector3 startPos = transform.localPosition;
        Vector3 endPos = transform.localPosition + direction * new Vector3(cellWidth, 0f, 0f);

        float timeElapsed = 0;

        while (timeElapsed < scrollDuration)
        {
            transform.localPosition = Vector3.Lerp(startPos, endPos, timeElapsed / scrollDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = endPos;
        scrolling = false;
    }

    public void UpdateChildPositions()
    {
        if (transform.GetChild(0).position.x < resetPosLeft)
        {
            transform.GetChild(0).localPosition = transform.GetChild(transform.childCount - 1).localPosition + new Vector3(cellWidth, 0f, 0f);
            transform.GetChild(0).SetAsLastSibling();
        }
        else if (transform.GetChild(transform.childCount - 1).position.x > resetPosRight)
        {
            transform.GetChild(transform.childCount - 1).localPosition = transform.GetChild(0).localPosition - new Vector3(cellWidth, 0f, 0f);
            transform.GetChild(transform.childCount - 1).SetAsFirstSibling();
        }
    }
}