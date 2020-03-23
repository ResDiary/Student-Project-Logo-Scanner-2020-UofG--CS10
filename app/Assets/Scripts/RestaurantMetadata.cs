using System;
using System.Collections.Generic;

/// <summary>
///     Holds all the data of the last scanned restaurant. Is used by the ResDiaryClient when deserializing
///     the JSON data received from the middleware. It is implemented using a singleton pattern so that there
///     is only ever a single instance of a restaurant’s data in memory and so that any class can easily read 
///     the data and process if whenever they need to, for example when handling the DataReceived event.
/// </summary>
[Serializable]
public class RestaurantMetadata
{

    /// <summary>
    ///     set default values to blank or 0 if data wasn't parsed
    ///     otherwise NULL exceptions are thrown when checking for data
    /// </summary>
    /// 
    /// <param name="address">Location Address</param>
    /// <param name="cuisineTypes">Cuisine types array</param>
    /// <param name="microsite">Microsite</param>
    /// <param name="reviews">Amount of reviews</param>
    /// <param name="pricePoint">Price point</param>
    /// <param name="name">Restaurant Name</param>
    /// <param name="slots">Time Slots</param>
    /// <param name="logoUrl">Logo URL</param>
    /// <param name="menuUrl">Menu link URL</param>
    private static RestaurantMetadata _instance;
    public Address address = new Address();
    public string bookingUrl = "";
    public List<string> cuisineTypes = new List<string>();
    public string logoUrl = "";
    public string menuUrl = "";
    public string microsite = "";
    public string name = "";
    public int pricePoint;
    public Reviews reviews = new Reviews();
    public List<Slot> slots = new List<Slot>();

    private RestaurantMetadata()
    {
    }

    /// <summary>
    ///     RestaurantMetadata is implemented as a singleton pattern so that all other classes can access the data
    ///     from one place.
    /// </summary>
    public static RestaurantMetadata GetData
    {
        get
        {
            if (_instance == null)
                _instance = new RestaurantMetadata();
            return _instance;
        }
    }

    [Serializable]
    /// <param name = "average" > Average reviews </param>
    /// <param name="count"> Number of reviews </param>
    public class Reviews
    {
        public int average;
        public int count;
    }

    [Serializable]
    /// <param name = "latitude" > Latitude of restaurant location </param>
    /// <param name="longitude"> Longitude of restaurant location</param>
    public class Address
    {
        public float latitude = -1;
        public float longitude = -1;
    }

    [Serializable]
    /// <param name = "promotions" > List of promotions </param>
    /// <param name="longitude"> Time of promotions </param>
    public class Slot
    {
        public List<string> promotions = new List<string>();
        public string time = "";
    }
}