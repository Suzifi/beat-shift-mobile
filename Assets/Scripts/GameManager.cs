using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager manager;

    public UnityEvent OnGameStarted;
    public UnityEvent OnMenuOpened;
    public UnityEvent OnAdsInitialized;

    public int timesPressedPlay; // To show ads only every second time

    private void Awake()
    {
        CreateSingleton();
    }

    void CreateSingleton()
    {
        if (manager == null)
        {
            DontDestroyOnLoad(gameObject); // If there is no manager yet - make this a manager
            manager = this;
        }
        else // Destroy if there already is a manager and another one is trying to instantiate
        {
            Destroy(gameObject);
        }
    }

    public void StartPackage(string package)
    {
        SceneManager.LoadScene(package);

        timesPressedPlay++;

        OnGameStarted?.Invoke();
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("Menu");

        OnMenuOpened?.Invoke();
    }
}
