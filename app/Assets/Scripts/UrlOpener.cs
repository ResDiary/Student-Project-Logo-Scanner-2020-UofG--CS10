using UnityEngine;
using UnityEngine.Networking;

public class UrlOpener : MonoBehaviour /// Responsible for handling all the URLs received and redirecting the user to the relevant data 
{

    
    public static int timeslotIndex { get; set; }/// Index of selected dropdown item

    public void OpenMenuUrl()
    {
        Application.OpenURL(RestaurantMetadata.GetData.menuUrl);
    }

    
    
   
    public void OpenMapUrl()
    {
        // Query for google maps API
        Application.OpenURL("https://www.google.com/maps/search/?api=1&query="+ RestaurantMetadata.GetData.address.latitude+","+RestaurantMetadata.GetData.address.longitude); 
    }

  
    public void OpenBookingURL()
    {
        var bookUrl = UnityWebRequest.EscapeURL(RestaurantMetadata.GetData.slots[timeslotIndex].time);

        Application.OpenURL(RestaurantMetadata.GetData.bookingUrl + bookUrl);
    }
}