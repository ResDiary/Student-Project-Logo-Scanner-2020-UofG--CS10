using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[Obsolete("New version of Vuforia implemented editable OnTrackingFound() through the inspector, so use that")]
[RequireComponent(typeof(RestaurantMetadata))]
public class MyDefaultTrackableEventHandler : DefaultTrackableEventHandler
{
    public GameObject restaurantInfoPanel;
    public GameObject[] toHide;
    public GameObject mCamera;

    protected override void OnTrackingFound()
    {
        showRestaurantInfo();
        base.OnTrackingFound();
    }

    protected override void OnTrackingLost()
    {
        base.OnTrackingLost();
    }

    /// <summary>
    /// Called from SimpleCloudHandler, Displays info canvas upon tracking a logo
    /// </summary>
    /// <param name="targetName">name of tracked logo</param>
    /// <param name="targetMetadata">metadata of tracked logo</param>
    public void cloudTargetFound(string targetName, string targetMetadata)
    {
        RestaurantMetadata restarurantData = GameObject.FindObjectOfType<RestaurantMetadata>();
        targetName = targetMetadata; // TODO: metadata hold name for demo purpose remove after
                                     //        restarurantData.updateUI(targetName);
        showRestaurantInfo();
    }
    public void showRestaurantInfo()
    {
        restaurantInfoPanel.SetActive(true);

        foreach (GameObject obj in toHide)
        {
            obj.SetActive(false);
        }
    }


}
