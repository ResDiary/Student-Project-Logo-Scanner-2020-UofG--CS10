using System.Collections;
using System.Linq;
using States;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
///     Executes when history view scene is loaded. It queries the history from the HistoryHandler and then
///     for each RestaurantMetadata object in the history array, it instantiates a history panel prefab,
///     which it updates with all the data for that history element.
/// </summary>
public class HistoryViewUpdate : MonoBehaviour
{
    public Transform contentTransform;
    public GameObject historyPanelPrefab;

    // Instantiates a history prefab in the content transform
    private GameObject InstantiateBlankHistoryItem()
    {
        var historyItem = Instantiate(historyPanelPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        historyItem.transform.SetParent(contentTransform);
        historyItem.transform.localScale = new Vector3(1, 1, 1); // Reset scale of instantiated item
        return historyItem;
    }

    // Adds all the existing history entries on the history view
    private void InstantiateHistory()
    {
        var historyHandler = FindObjectOfType<HistoryHandler>();
        if (!historyHandler)
        {
            Debug.Log("No History Handler in scene");
            return;
        }

        var history = historyHandler.GetHistory().pastData;

        foreach (var data in history)
        {
            if (data is null)
                continue;

            var item = InstantiateBlankHistoryItem();
            item.FindComponentInChildWithTag<Text>("Microsite").text = data.microsite;
            item.FindComponentInChildWithTag<Text>("ResName").text = data.name;
            item.FindComponentInChildWithTag<Text>("PricePoint").text =
                string.Concat(Enumerable.Repeat("£", data.pricePoint));
            item.FindComponentInChildWithTag<Text>("Cuisine").text = string.Join(", ", data.cuisineTypes);
            var logo = item.FindComponentInChildWithTag<RawImage>("Logo");
            StartCoroutine(DownloadImage(data.logoUrl, logo));
            item.GetComponent<Button>().onClick.AddListener(delegate { Buttons.DisplayRestaurant(this); });
        }
    }

    private static IEnumerator DownloadImage(string mediaUrl, RawImage destination)
    {
        var request = UnityWebRequestTexture.GetTexture(mediaUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
            destination.texture = ((DownloadHandlerTexture) request.downloadHandler).texture;
    }

    private void Start()
    {
        InstantiateHistory();
    }
}