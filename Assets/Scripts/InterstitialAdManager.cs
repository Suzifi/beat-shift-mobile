using UnityEngine;
using UnityEngine.Advertisements;

public class InterstitialAdManager : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] string _androidAdUnitId = "Interstitial_Android";
    [SerializeField] string _iOsAdUnitId = "Interstitial_iOS";
    string _adUnitId;

    public bool needNewAd = true;

    void Awake()
    {

        // Get the Ad Unit ID for the current platform:
        _adUnitId = (Application.platform == RuntimePlatform.IPhonePlayer)
            ? _iOsAdUnitId
            : _androidAdUnitId;
    }

    private void Start()
    {
        //LoadAd();
        GameManager.manager.OnGameStarted.AddListener(ShowAd);
        GameManager.manager.OnMenuOpened.AddListener(LoadAd);
        GameManager.manager.OnAdsInitialized.AddListener(LoadAd);
    }

    // Load content to the Ad Unit:
    public void LoadAd()
    {
        if (needNewAd)
        {
            // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
            Debug.Log("Loading Ad: " + _adUnitId);
            Advertisement.Load(_adUnitId, this);
        }

    }

    // Show the loaded content in the Ad Unit:
    public void ShowAd()
    {
        if (!Advertisement.isInitialized)
        {
            AdsInitializer.adManager.InitializeAds();
        }

        // Don't show ad the first time
        if (GameManager.manager.timesPressedPlay == 1)
        {
            return;
        }

        // Note that if the ad content wasn't previously loaded, this method will fail
        Debug.Log("Showing Ad: " + _adUnitId);
        Advertisement.Show(_adUnitId, this);
        needNewAd = true;
    }

    // Implement Load Listener and Show Listener interface methods: 
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        needNewAd = false;
    }

    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        needNewAd = true;
        Debug.Log($"Error loading Ad Unit: {adUnitId} - {error.ToString()} - {message}");
        LoadAd();
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        needNewAd = true;
        Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
        LoadAd();
    }

    public void OnUnityAdsShowStart(string adUnitId) { }
    public void OnUnityAdsShowClick(string adUnitId) { }
    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        Debug.Log("Show complete, load new ad");
        LoadAd();
    }
}