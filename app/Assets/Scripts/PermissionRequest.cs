
using UnityEngine;
using UnityEngine.Android;


/// <summary>
///     Class designated to request for location service permissions if it hasn't already been granted by the user.
/// </summary>
public class PermissionRequest : MonoBehaviour
{
    void Start()
    {
        AskForPopupPermission();
    }

    // Recursively requests for permission to enable location services
    private void AskForPopupPermission()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            AskForPopupPermission();
        }
    }

  
}
