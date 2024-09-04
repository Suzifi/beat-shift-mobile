using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuControl : MonoBehaviour
{
    bool showLoadingBar;
    public GameObject loadingBar;

    public Image loadingBarFiller;

    public bool adInProgress;

    Button[] allButtons;

    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        allButtons = FindObjectsOfType<Button>();

        for (int i = 0; i < allButtons.Length; i++)
        {
            allButtons[i].interactable = true;
        }
    }


    void Update()
    {

    }

    public void ChoosePackage(string packageName)
    {
        StartCoroutine(LoadScene(packageName));
    }

    public void ShowLoadingBar(GameObject loadingBarToShow)
    {
        loadingBar = loadingBarToShow;
        loadingBarFiller = loadingBar.transform.GetChild(0).GetComponent<Image>();
    }

    public void DisableOtherButtons()
    {
        for (int i = 0; i < allButtons.Length; i++)
        {
            allButtons[i].interactable = false;
        }
    }

    IEnumerator LoadScene(string packageName)
    {
        DisableOtherButtons();
        loadingBar.SetActive(true);

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(packageName);
        loadOperation.allowSceneActivation = false; // Don't let the scene load too fast

        GameManager.manager.timesPressedPlay++;
        GameManager.manager.OnGameStarted?.Invoke();

        while (!loadOperation.isDone)
        {
            //loadingBarFiller.fillAmount = loadOperation.progress;
            float fill = loadOperation.progress;

            if (fill < loadingBarFiller.fillAmount + Time.deltaTime * 0.3f)
            {
                fill = loadingBarFiller.fillAmount + Time.deltaTime * 0.3f;
            }

            loadingBarFiller.fillAmount = fill;

            if (loadOperation.progress >= 0.9f)
            {
                loadingBarFiller.fillAmount = 1f;

                // Allow loading after minimum time has passed
                loadOperation.allowSceneActivation = true;
            }

            yield return null;
        }

    }

    public void OpenPrivacyPolicy()
    {
        Application.OpenURL("https://sites.google.com/view/2bluemittens/privacy-policy");
    }

    public void Rate()
    {
        Application.OpenURL("market://details?id=" + Application.identifier);
    }
}
