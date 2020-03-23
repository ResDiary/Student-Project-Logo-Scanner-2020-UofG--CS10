using System.Net;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
///     Executes when the camera view is loaded and handles the pop-up events on the screen
///     in accordance to the state the user is in. It essentially triggers the error message and 
///     instructive pop-ups depending on the success of the image capture.
/// </summary>
public class CameraViewUIPopupsHandler : UIBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
  
    [Tooltip("How long must pointer be down on this object to trigger a long press")]
    public float durationThreshold = 1.0f; /// Threshold Duration

    private bool isPointerDown;


    public bool hideLocationPopup { get; set; }
    public UnityEvent onLongPress = new UnityEvent();
    public GameObject StateInfoPopup;
    public GameObject LocationPopup; /// Location Permission Popup
    private float timePressStarted;
    public GameObject WarningPopup; /// By default the warning popup says "Image not recognised"

    /// <summary>
    ///         Event handler for when a button is being held down.
    ///         Indicates that the camera is scanning when the button is held.
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        StateInfoPopup.GetComponentInChildren<Text>().text = "Scanning";
        timePressStarted = Time.time;
        isPointerDown = true;
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerDown = false;
    }

    /// <summary>
    ///      Event handler for when a button is released.
    ///      Clear warnings after the user stops scanning and enables the instructional popup again  
    /// </summary>
    public void OnPointerUp(PointerEventData eventData)
    {
        isPointerDown = false;

        StateInfoPopup.GetComponentInChildren<Text>().text = "Press and hold the button below to scan!";
        WarningPopup.SetActive(false); // 
    }

    private new void Start()
    {
        ResdiaryClient.DataReceived += HandleDataReceived;
        WarningPopup.SetActive(false);
    }

    private void Update()
    {
        WarningTimer();
        ShowLocationPopup();
    }

    /// <summary>
    ///     Enables and disables the location popup
    /// </summary>
    private void ShowLocationPopup()
    {
        if (!Input.location.isEnabledByUser && !hideLocationPopup)
        {
            LocationPopup.SetActive(true);
        }
        else
        {
            LocationPopup.SetActive(false);
        }
    }

    /// <summary>
    ///     Launches warning popup after set amount of time
    /// </summary>
    private void WarningTimer()
    {
        if (isPointerDown)
            if (Time.time - timePressStarted > durationThreshold)
            {
                WarningPopup.GetComponentInChildren<Text>().text = "Logo not recognised";
                WarningPopup.SetActive(true);
            }
    }

    /// <summary>
    ///     Listen to ResDataReceived event
    /// </summary>
    private void HandleDataReceived(object sender, ResdiaryClient.ResDataReceivedEventArgs args)
    {
        if (args.StatusCode != 200f)
        {
            var WarningText = WarningPopup.GetComponentInChildren<Text>();
            WarningText.text = "Had trouble retrieving restaurant's data, please try again later";
            WarningPopup.SetActive(true);
        }
    }
}