using UnityEngine;
using Vuforia;

/// <summary>
///     Delay the initialisation of image targets.
///     If the image target is enabled during Vuforia's initialisation it spawns
///     ImageTargets for local database which is unwanted and Vuforia provides no
///     functionality as of now to remove a local ImageTarget database from the project
/// </summary>
[RequireComponent(typeof(ImageTargetBehaviour))]
public class DelayImageTargetEnabling : MonoBehaviour
{
    // Start is called after Awake in the order of execution so ImageTargetBehaviour is enabled after Vuforia
    private void Start()
    {
        GetComponent<ImageTargetBehaviour>().enabled = true;
    }
}