using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

/// <summary>
///     Handles keeping track of scanned restaurants and saving the data to a file. It uses a History class, 
///     which is just an array of RestaurantMetadatas and the last added index to keep track of what has been
///     scanned by the user and the position in the array to overwrite when the user next scans a logo. 
///     It listens to the DataReceived event and when a successful response has been received it saves the new data to a file.
/// </summary>
public class HistoryHandler : MonoBehaviour
{
   
  
    private History _currentHistory;
    public int historyLimit = 5; /// Maximum number of restaurants stored, set in editor

    /// <summary>
    ///     History is implemented as a singleton pattern so that all other classes can access the data from one place.  
    /// </summary>
    public History GetHistory()
    {
        return _currentHistory ?? new History(historyLimit);
    }

    private void Start()
    {
        _currentHistory = LoadHistoryFromFile();
        ResdiaryClient.DataReceived += HandleDataReceived;
    }

    /// <summary>
    ///     Add restaurant data to history upon successful status response
    /// </summary>
    public void HandleDataReceived(object sender, ResdiaryClient.ResDataReceivedEventArgs args)
    {
        if (args.StatusCode == 200) AddToHistory(RestaurantMetadata.GetData);
    }

    /// <summary>
    ///     Adds restaurant data to the history file
    /// </summary>
    public void AddToHistory(RestaurantMetadata data)
    {
        var newHistory = LoadHistoryFromFile();

        var index = _currentHistory.lastAddedIndex % historyLimit;
        newHistory.pastData[index] = data;
        newHistory.lastAddedIndex = ++index;
        _currentHistory = newHistory;
        SaveFile();
    }

    /// <summary>
    ///     Save history to /save.dat
    /// </summary>
    public void SaveFile()
    {
        var destination = Application.persistentDataPath + "/save.dat";
        FileStream file;

        file = File.Exists(destination) ? File.OpenWrite(destination) : File.Create(destination);

        var data = _currentHistory;
        var bf = new BinaryFormatter();
        bf.Serialize(file, data);
        file.Close();
    }

    /// <summary>
    ///     Loads history from the /save.dat file
    /// </summary>
    public History LoadHistoryFromFile()
    {
        var destination = Application.persistentDataPath + "/save.dat";
        FileStream file;

        if (File.Exists(destination))
        {
            file = File.OpenRead(destination);
        }
        else
        {
            Debug.Log("History file not found");
            return new History(historyLimit);
        }

        var bf = new BinaryFormatter();
        var history = (History) bf.Deserialize(file);
        // TO DO: Check for invalid cast exception
        file.Close();

        // If the history limit has changed since the last time history was loaded create a new larger array
        if (history.pastData.Length != historyLimit)
            return new History(historyLimit);

        return history;
    }

    [Serializable]
    public class History
    {
        public int lastAddedIndex;
        public RestaurantMetadata[] pastData;

        public History(int size)
        {
            pastData = new RestaurantMetadata[size];
        }
    }
}