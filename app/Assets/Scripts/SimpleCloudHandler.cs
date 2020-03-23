using System;
using UnityEngine;
using Vuforia;

/// <summary>
///   Handles communications between the app and Vuforia’s cloud service.
///   It is used to convey the metadata of the scanned logo which includes the microsite and device location (from DeviceLocation).
/// </summary>
public class SimpleCloudHandler : MonoBehaviour
{

    private bool _isInitialised; /// True when cloud reco finishes initalisation
    private CloudRecoBehaviour _mCloudRecoBehaviour;
    public bool defaultState; /// Start state of cloud image tracking (on/off)
    public bool mIsScanning;/// Is vuforia currently scanning the cloud database?



    /// <summary>
    ///          Event raised whenever a logo gets recognised
    /// </summary>
    public static event EventHandler<LogoRecognisedEventArgs> LogoRecognised;

  
    /// <summary>
    ///          Send out a message that a logo has been recognised
    /// </summary>
    protected virtual void OnLogoRecognised(LogoRecognisedEventArgs e)
    {
        LogoRecognised?.Invoke(this, e);
    }

    private void Start()
    {
        // register this event handler at the cloud reco behaviour 
        _mCloudRecoBehaviour = GetComponent<CloudRecoBehaviour>();

        RegisterEventHandlers();
    }
    
    private void RegisterEventHandlers()
    {
        if (_mCloudRecoBehaviour)
        {
            // Since Vuforia version 8.6 all event handlers have to be registered one by one
            _mCloudRecoBehaviour.RegisterOnInitializedEventHandler(OnInitialized);
            _mCloudRecoBehaviour.RegisterOnInitErrorEventHandler(OnInitError);
            _mCloudRecoBehaviour.RegisterOnUpdateErrorEventHandler(OnUpdateError);
            _mCloudRecoBehaviour.RegisterOnStateChangedEventHandler(OnStateChanged);
            _mCloudRecoBehaviour.RegisterOnNewSearchResultEventHandler(OnNewSearchResult);
        }
    }

    public void OnInitialized(TargetFinder targetFinder)
    {
        Debug.Log("Cloud Reco initialized");
    }

    public void OnInitError(TargetFinder.InitState initError)
    {
        Debug.Log("Cloud Reco init error " + initError);
    }

    public void OnUpdateError(TargetFinder.UpdateState updateError)
    {
        Debug.Log("Cloud Reco update error " + updateError);
    }

    public void OnStateChanged(bool scanning)
    {
        // update isScanning to indicate if the cloud database is still being scanned in new state
        mIsScanning = scanning;
        if (mIsScanning)
        {
            // clear all known trackables
            var tracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
            tracker.GetTargetFinder<ImageTargetFinder>().ClearTrackables(false);

            if (_isInitialised == false)
            {
                _mCloudRecoBehaviour.CloudRecoEnabled = defaultState;
                mIsScanning = defaultState;
                _isInitialised = true;
            }
        }
    }

    /// <summary>
    ///         Here we handle a cloud target recognition event
    /// </summary>
    public void OnNewSearchResult(TargetFinder.TargetSearchResult targetSearchResult)
    {
        var cloudRecoSearchResult =
            (TargetFinder.CloudRecoSearchResult) targetSearchResult;

        // Get target metadata
        var mTargetName = cloudRecoSearchResult.TargetName;
        var mTargetMetadata = cloudRecoSearchResult.MetaData;

        // stop the target finder (i.e. stop scanning the cloud)
        _mCloudRecoBehaviour.CloudRecoEnabled = false;

        // Logo has been recognised so send out event
        SendOutLogoRecognisedEvent(mTargetName, mTargetMetadata);
    }

    private void SendOutLogoRecognisedEvent(string mTargetName, string mTargetMetadata)
    {
        var location = DeviceLocation.GetInstance().GetLocation();
        var args = new LogoRecognisedEventArgs
        {
            microsite = mTargetName,
            metadata = mTargetMetadata,
            latitude = location.latitude,
            longitude = location.longitude
        };

        OnLogoRecognised(args);
    }


    public void EnableScan()
    {
        // Enable TargetFinder
        _mCloudRecoBehaviour.CloudRecoEnabled = true;
        mIsScanning = true;
    }


    public void DisableScan()
    {
        // Disable TargetFinder
        _mCloudRecoBehaviour.CloudRecoEnabled = false;
        mIsScanning = false;
    }

    public class LogoRecognisedEventArgs : EventArgs
    {
        public string microsite { get; set; }
        public string metadata { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }
    }
}