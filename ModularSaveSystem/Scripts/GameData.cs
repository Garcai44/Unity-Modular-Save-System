using UnityEngine;

/// <summary>
/// Represents the global save file structure. 
/// Any data that needs to be serialized into the JSON file must be defined here.
/// </summary>
[System.Serializable]
public class GameData
{
    public int deathCount;
    public Vector3 playerPosition;

    /// <summary>
    /// Default constructor used when no save file exists.
    /// Initializes the game with default starting values.
    /// </summary>
    public GameData()
    {
        deathCount = 0;
        playerPosition = Vector3.zero; 
    }
}