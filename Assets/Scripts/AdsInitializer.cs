using UnityEngine;
using UnityEngine.Advertisements;

public class AdsInitializer : MonoBehaviour, IUnityAdsInitializationListener
{
    public static AdsInitializer adManager;

    [SerializeField] string _androidGameId;
    [SerializeField] string _iOSGameId;
    [SerializeField] bool _testMode = true;
    private string _gameId;

    [SerializeField] BannerAdManager bannerAdManager;
    [SerializeField] InterstitialAdManager interstitialAdManager;

    void Awake()
    {
        CreateSingleton();
        InitializeAds();
    }

    void CreateSingleton()
    {
        if (adManager == null)
        {
            DontDestroyOnLoad(gameObject); // If there is no manager yet - make this a manager
            adManager = this;
        }
        else // Destroy if there already is a manager and another one is trying to instantiate
        {
            Destroy(gameObject);
        }
    }

    public void InitializeAds()
    {
        _gameId = (Application.platform == RuntimePlatform.IPhonePlayer)
            ? _iOSGameId
            : _androidGameId;
        Advertisement.Initialize(_gameId, _testMode, this);
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
        bannerAdManager.LoadBanner();
        interstitialAdManager.LoadAd();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
        InitializeAds();
    }
}