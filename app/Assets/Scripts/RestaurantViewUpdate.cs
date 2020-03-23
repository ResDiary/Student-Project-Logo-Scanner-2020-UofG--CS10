using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
///     Executes when the Restaurant View scene is loaded and handles updating all the UI elements to display the data
///     of the most recently scanned restaurant. It gets references to the UI elements through specific tags assigned to each element.
///     This avoids the need of having to manually set the references in the editor, which break if there’s any change to those elements.
///     It fills the promos scroll box by instantiating a promo prefab which is an empty placeholder promo object and it updates its text
///     with the correct promo. A list of promo objects is stored so that they can all be destroyed and replace with new ones when a user
///     selects a new option on the timeslot dropdown, which calls TimeSlotCheck.
/// </summary>
public class RestaurantViewUpdate : MonoBehaviour

{
    
    public Transform contentTransform;/// Transform of viewport
    public GameObject prefabButton; /// Prefab object for promotion
    public List<GameObject> promolist;
    public Dropdown timeslot;

    /// <summary>
    ///     Add a new blank promo to the UI from prefab
    /// </summary>
    /// <returns>new promo UI object</returns>
    private GameObject AddPromos()
    {
        var promo = Instantiate(prefabButton, new Vector3(0, 0, 0), Quaternion.identity);
        promo.transform.SetParent(contentTransform);
        promo.transform.localScale =
            new Vector3(1, 1, 1); // Scales all the promo items correctly to fit the scroll viewport
        return promo;
    }

    /// <summary>
    ///     Gets a reference to the logo UI component and starts the coroutine which will download the logo
    /// </summary>
    private void GetImages()
    {
        var logo = GameObject.FindGameObjectWithTag("Logo").GetComponent<RawImage>();

        StartCoroutine(DownloadImage(RestaurantMetadata.GetData.logoUrl, logo));
    }

    /// <summary>
    ///     Download an image from a url and assign it to a RawImage component
    /// </summary>
    /// <param name="mediaUrl">Url to download from</param>
    /// <param name="destination">RawImage to assign the downloaded logo to</param>
    /// <returns></returns>
    private static IEnumerator DownloadImage(string mediaUrl, RawImage destination)
    {
        var request = UnityWebRequestTexture.GetTexture(mediaUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
            destination.texture = ((DownloadHandlerTexture) request.downloadHandler).texture;
    }

    /// <summary>
    ///     Display most up to data restaurant data on the UI
    /// </summary>
    private static void UpdateUi()
    {
        GetUiComponents(out var resName, out var cuisine, out var rating, out var price, out var reviews);

        SetUiText(resName, cuisine, rating, reviews, price);
    }

    /// <summary>
    ///     Get references to UI Components, UI components are returned as out parameters
    /// </summary>
    private static void GetUiComponents(out Text resName, out Text cuisine, out Text rating, out Text price,
        out Text reviews)
    {
        resName = GameObject.FindGameObjectWithTag("ResName").GetComponent<Text>();
        cuisine = GameObject.FindGameObjectWithTag("Cuisine").GetComponent<Text>();
        rating = GameObject.FindGameObjectWithTag("AvgRating").GetComponent<Text>();
        price = GameObject.FindGameObjectWithTag("PricePoint").GetComponent<Text>();
        reviews = GameObject.FindGameObjectWithTag("ReviewCount").GetComponent<Text>();
    }

    /// <summary>
    ///     Change text boxes on restaurant view
    /// </summary>
    /// <param name="resName">Restaurant name</param>
    /// <param name="cuisine">Cuisine types array</param>
    /// <param name="rating">Average rating</param>
    /// <param name="reviews">Amount of reviews</param>
    /// <param name="price">Price point</param>
    private static void SetUiText(Text resName, Text cuisine, Text rating, Text reviews, Text price)
    {
        var data = RestaurantMetadata.GetData;

        resName.text = data.name;
        cuisine.text = string.Join(", ", data.cuisineTypes);
        rating.text = "Avg " + data.reviews.average;
        reviews.text = data.reviews.count + " reviews";
        Debug.Log(data.address.latitude+","+ data.address.longitude);
        price.text = string.Concat(Enumerable.Repeat("£", data.pricePoint));
    }


    /// <summary>
    ///     Called when a time slot is picked on the dropdown
    /// </summary>
    /// <param name="change">Chosen dropdown option</param>
    public void TimeSlotCheck(Dropdown change)
    {
        DestroyPromos();
        InstantiatePromos(change.value);
        UrlOpener.timeslotIndex = change.value;
    }

    /// <summary>
    ///     Displays promos for the first available time slot,
    ///     should be called before user picks a time slot to by default show first
    /// </summary>
    private void PopulateInitialPromos()
    {
        DestroyPromos();
        if (RestaurantMetadata.GetData.slots.Count > 0) InstantiatePromos(0);
    }

    /// <summary>
    ///     Destroy all promo buttons so that new promos can be shown
    /// </summary>
    private void DestroyPromos()
    {
        foreach (var obj in promolist) Destroy(obj);

        promolist.Clear();
    }

    /// <summary>
    ///     Instantiate buttons for all promos in chosen time slot,
    ///     also add all promos to a promolist array to keep track of them
    /// </summary>
    /// <param name="slotIndex">index of time slot to show promos for</param>
    private void InstantiatePromos(int slotIndex)
    {
        var promos = RestaurantMetadata.GetData.slots[slotIndex].promotions;
        for (var i = 0; i < promos.Count; i++)
        {
            var promo = AddPromos();
            promo.GetComponentInChildren<Text>().text = promos[i];
            promolist.Add(promo);
        }
    }

    /// <summary>
    ///     Add time slots to the time slots dropdown
    /// </summary>
    private void TimeSlotPopulation()
    {
        if (RestaurantMetadata.GetData.slots.Count > 0) timeslot.options.Clear();

        foreach (var slot in RestaurantMetadata.GetData.slots)
            timeslot.options.Add(new Dropdown.OptionData {text = slot.time.Substring(11, 5)});

        timeslot.RefreshShownValue();
    }

    private void Start()
    {
        GetImages();
        UpdateUi();
        TimeSlotPopulation();
        PopulateInitialPromos();
    }
}