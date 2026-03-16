using UnityEngine;

/// <summary>
/// A simple test script to demonstrate the data persistence system.
/// Implements the IDataPersistence interface to save and load its position.
/// </summary>
public class PlayerSaveTest : MonoBehaviour, IDataPersistence
{
    /// <summary>
    /// Called when the game starts. Reads the position from the save file.
    /// </summary>
    public void LoadData(GameData data)
    {
        // Apply the saved position to this object's transform
        this.transform.position = data.playerPosition;
        Debug.Log("Loaded player position from Save File.");
    }

    /// <summary>
    /// Called when the game is closing. Writes the current position to the save file.
    /// </summary>
    public void SaveData(ref GameData data)
    {
        // Store this object's current position into the save file container
        data.playerPosition = this.transform.position;
        Debug.Log("Saved player position to Save File.");
    }
}