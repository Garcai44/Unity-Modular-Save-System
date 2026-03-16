using System;
using System.IO;
using UnityEngine;

/// <summary>
/// Handles reading and writing GameData to a physical local file using JSON.
/// Includes an optional XOR encryption layer to prevent save file tampering.
/// </summary>
public class FileDataHandler
{
    private string dataDirPath = "";
    private string dataFileName = "";
    
    // Security toggle and encryption key
    private bool useEncryption = false;
    private readonly string encryptionCodeWord = "supersecretkey123";

    /// <summary>
    /// Constructor to initialize the file paths and encryption settings.
    /// </summary>
    public FileDataHandler(string dataDirPath, string dataFileName, bool useEncryption)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
        this.useEncryption = useEncryption;
    }

    /// <summary>
    /// Reads the JSON file from the hard drive, decrypts it if necessary, and converts it back into GameData.
    /// </summary>
    public GameData Load()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        GameData loadedData = null;

        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";

                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                // Decrypt the data before converting it back to a C# object
                if (useEncryption)
                {
                    dataToLoad = EncryptDecrypt(dataToLoad);
                }

                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error occurred when trying to load data from file: {fullPath}\n{e}");
            }
        }
        
        return loadedData;
    }

    /// <summary>
    /// Converts GameData into JSON, encrypts it if necessary, and writes it to the hard drive.
    /// </summary>
    public void Save(GameData data)
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string dataToStore = JsonUtility.ToJson(data, true);

            // Encrypt the JSON string before writing it to the file
            if (useEncryption)
            {
                dataToStore = EncryptDecrypt(dataToStore);
            }

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error occurred when trying to save data to file: {fullPath}\n{e}");
        }
    }

    /// <summary>
    /// A simple XOR encryption algorithm.
    /// It modifies the string using a secret codeword. Running it twice reverses the effect.
    /// </summary>
    private string EncryptDecrypt(string data)
    {
        string modifiedData = "";
        for (int i = 0; i < data.Length; i++)
        {
            modifiedData += (char)(data[i] ^ encryptionCodeWord[i % encryptionCodeWord.Length]);
        }
        return modifiedData;
    }
}