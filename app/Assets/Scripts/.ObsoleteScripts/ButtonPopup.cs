using UnityEngine;
using System.Collections;
using Vuforia;

public class ButtonPopup : MonoBehaviour, ITrackableEventHandler
{

    private TrackableBehaviour mTrackableBehaviour;

    //public GameObject restaurantInfo;
    //public GameObject[] toHide;
    //public GameObject mCamera;

    void Start()
    {
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
        {
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
        }
    }

    public void OnTrackableStateChanged(
                                    TrackableBehaviour.Status previousStatus,
                                    TrackableBehaviour.Status newStatus)
    {
        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            //Debug.Log("DETECTED");
            //showRestaurantInfo();
        }
    }

    //public void showRestaurantInfo()
    //{
    //    restaurantInfo.SetActive(true);

    //    foreach (GameObject obj in toHide)
    //    {
    //        obj.SetActive(false);
    //    }
    //}

}