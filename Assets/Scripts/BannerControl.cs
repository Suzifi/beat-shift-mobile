using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BannerControl : MonoBehaviour
{
    public Image[] dots;
    public Color originalColor;
    public Color dimmedColor;

    void Start()
    {
        //SetActiveDot(1);
    }

    public void Rate()
    {
        Application.OpenURL("market://details?id=" + Application.identifier);
    }

    public void ToRockQuiz()
    {
        Application.OpenURL("market://details?id=com.TwoBlueMittens.RockMetal");
    }

    /*public void SetActiveDot(int activeChild)
    {
        for (int i = 0; i < dots.Length; i++)
        {
            if (i == activeChild)
            {
                dots[i].color = originalColor;
            }
            else
            {
                dots[i].color = dimmedColor;
            }

        }
    }*/
}
