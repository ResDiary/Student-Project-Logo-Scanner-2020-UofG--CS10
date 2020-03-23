using System.Collections;
using UnityEngine;

/// <summary>
///     https://docs.unity3d.com/ScriptReference/LocationService.Start.html
///     When attached to a GameObject will attempt to start the device's location service on load.
///     If run inside the Editor the initialization will be delayed by a few seconds to allow the Editor
///     to connect to the device.
/// </summary>
public class DeviceLocation : MonoBehaviour
{
    private static DeviceLocation _instance;

    public static DeviceLocation GetInstance()
    {
        return _instance;
    }

    private void Awake()
    {
        _instance = this;
    }

    /// <summary>
    ///     Start location service when script is loaded.
    ///     Start is a run as a coroutine so that coroutines are run serial rather than parallel,
    ///     i.e. it should wait while initialization happens before starting the location service.
    /// </summary>
    private IEnumerator Start()
    {
        yield return StartCoroutine(EditorInitialization());
        yield return StartCoroutine(StartLocationService());
    }

    /// <summary>
    ///     If running in Editor it takes a while for it to connect to an external device,
    ///     so a delay is needed before attempting to start location services
    /// </summary>
    private IEnumerator EditorInitialization()
    {
        if (Application.isEditor) yield return new WaitForSeconds(5);
    }

    /// <summary>
    ///     Try to start the location service on user's device
    /// </summary>
    private IEnumerator StartLocationService()
    {
        // First, check if user has location service enabled
        if (Input.location.isEnabledByUser)
            // Start service before querying location
            Input.location.Start(10, 0.1f); // 10m accuracy, update every 0.1m

        yield return null;
    }

    /// <summary>
    ///     Returns Device latitude and longitude in a Location struct,
    ///     will return 0 0 if location service has not been started
    /// </summary>
    public (float latitude, float longitude) GetLocation()
    {
        return Input.location.status == LocationServiceStatus.Running
            ? (Input.location.lastData.latitude, Input.location.lastData.longitude)
            : (0f, 0f);
    }
}