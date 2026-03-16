using System.Collections.Generic;
using UnityEngine;
using System.Linq; // Required for finding interfaces easily

/// <summary>
/// Singleton manager that coordinates the saving and loading of all game data.
/// </summary>
public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Configuration")]
    [Tooltip("The exact name of the save file (e.g., savegame.json).")]
    [SerializeField] private string fileName = "data.json";
    
    [Tooltip("Enable to encrypt the save file, preventing players from easily reading or modifying their data outside the game.")]
    [SerializeField] private bool useEncryption = false; 

    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;

    public static DataPersistenceManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Found more than one Data Persistence Manager in the scene. Destroying the newest one.");
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // Initialize the data handler with the system's persistent path, file name, and the chosen encryption setting
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
        
        // Scan the active scene to find all scripts that implement the IDataPersistence interface
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        
        // Automatically load existing data or initialize default values if no save file exists
        LoadGame();
    }

    /// <summary> Starts a fresh game by generating default GameData. </summary>
    public void NewGame()
    {
        this.gameData = new GameData();
    }

    /// <summary> Loads the game data from the file and distributes it to all listeners. </summary>
    public void LoadGame()
    {
        // 1. Load any saved data from a file using the data handler
        this.gameData = dataHandler.Load();

        // 2. If no data exists (first time playing), initialize to defaults
        if (this.gameData == null)
        {
            Debug.Log("No save data found. Initializing to defaults.");
            NewGame();
        }

        // 3. Push the loaded data to all scripts that need it
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }
    }

    /// <summary> Collects data from all listeners and saves it to the file. </summary>
    public void SaveGame()
    {
        if (this.gameData == null)
        {
            Debug.LogWarning("No data was found. A New Game needs to be started before data can be saved.");
            return;
        }

        // 1. Pull data from all scripts so they can update the GameData object
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }

        // 2. Save that updated data to the physical file
        dataHandler.Save(gameData);
    }

    /// <summary> Automatically save when the game is closed. </summary>
    private void OnApplicationQuit()
    {
        SaveGame();
    }

    /// <summary>
    /// Searches the entire scene for any MonoBehaviour that implements the IDataPersistence interface.
    /// </summary>
    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        // FindObjectsOfType gets all scripts, OfType filters only the ones with our contract
        IEnumerable<IDataPersistence> persistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();
        return new List<IDataPersistence>(persistenceObjects);
    }
}