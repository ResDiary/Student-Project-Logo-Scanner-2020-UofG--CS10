using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecoSwitch : MonoBehaviour
{
    private bool isLocal;
    public Text textbox;
    // Start is called before the first frame update
    void Start()
    {
        isLocal = true;
    }

    public void SwapMode()
    {
        var foundObjects = Resources.FindObjectsOfTypeAll<Vuforia.ImageTargetBehaviour>();
        foreach (var obj in foundObjects)
        {
            GameObject parent = obj.gameObject;
            parent.SetActive(!parent.activeSelf);
            obj.enabled = !obj.enabled;
        }
        var cloud = Resources.FindObjectsOfTypeAll<Vuforia.CloudRecoBehaviour>();
        foreach (var obj in cloud)
        {
            GameObject parent = obj.gameObject;
            parent.SetActive(!parent.activeSelf);
            obj.enabled = !obj.enabled;
        }
        isLocal = !isLocal;
        if (isLocal)
        {
            textbox.text = "Local Reco";
        } else
        {
            textbox.text = "Cloud Reco";
        }
    }
}
