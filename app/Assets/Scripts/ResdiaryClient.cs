using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
///     Manages communication with the middleware. It responds to the LogoRecognised event by constructing a post request
///     with the data extracted from the event, making the post request to the middleware, asynchronously waiting for a response,
///     when a good response is received parse the JSON and deserialize it into the RestaurantMetadata class, overwriting any existing data.
///     Then whether the request was successful or not trigger an event, DataReceived, with the response code received from the middleware. 
///     This allows any listener to act appropriately depending on the response code, for example for a 200 display the data to the user
///     otherwise display a warning message.
/// </summary>
public class ResdiaryClient : MonoBehaviour
{
    public static event EventHandler<ResDataReceivedEventArgs> DataReceived;

    protected virtual void OnDataReceived(object source, ResDataReceivedEventArgs e)
    {
        DataReceived?.Invoke(this, e);
    }

    private void Start()
    {
        // Subscribe event listener
        SimpleCloudHandler.LogoRecognised += HandleLogoRecognised;
    }

    // Called when logo gets scanned
    private void HandleLogoRecognised(object sender, SimpleCloudHandler.LogoRecognisedEventArgs a) 
    {
        var microsite = ParseMetadata(a.metadata);
        var lat = a.latitude;
        var lon = a.longitude;

        StartCoroutine(RequestData(microsite, lat, lon));
    }

    private string[] ParseMetadata(string metadata)
    {
        return metadata.Split(',');
    }
    /// <summary>
    ///         Method for communicating with the middleware and exchanging relevant metadata
    /// </summary>
    /// <param name="lat">Latitude of user</param>
    /// <param name="lon">Longitude of user</param>
    /// <param name="micrositeArr">Array of possible microsites</param>
  
    public IEnumerator RequestData(string[] micrositeArr, float lat, float lon)
    {
        var valObj = new Values { microsites = micrositeArr, latitude = lat, longitude = lon }; // data sent as part of POST request

        var json = JsonUtility.ToJson(valObj);
        var postContent = Encoding.UTF8.GetBytes(json);
        
        using (UnityWebRequest www = new UnityWebRequest("http://138.68.119.131/request/", "POST"))
        {
            www.uploadHandler = (UploadHandler)new UploadHandlerRaw(postContent);
            www.SetRequestHeader("Content-Type", "application/json");
            www.downloadHandler = new DownloadHandlerBuffer();
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                // Overwrite data in RestaurantMetadata
                JsonUtility.FromJsonOverwrite(www.downloadHandler.text, RestaurantMetadata.GetData);
                var args = new ResDataReceivedEventArgs(www.responseCode);
                OnDataReceived(this, args); // Trigger event
            }
        }
    }

    /// <summary>
    ///         Event arguments for OnDataReceived event contains a status code for the event
    /// </summary>

    public class ResDataReceivedEventArgs : EventArgs
    {
        public float StatusCode;

        public ResDataReceivedEventArgs(float statusCode)
        {
            StatusCode = statusCode;
        }
    }


    private class Values
    {
        public float latitude;
        public float longitude;
        public string[] microsites;
    }
}