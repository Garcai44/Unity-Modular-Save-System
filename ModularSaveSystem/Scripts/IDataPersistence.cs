using UnityEngine;

/// <summary>
/// Interface implemented by any script that needs to save or load data.
/// Allows the SaveManager to interact with objects without knowing their specific class.
/// </summary>
public interface IDataPersistence
{
    /// <summary>
    /// Called when the game loads. Injects the loaded data into the script.
    /// </summary>
    /// <param name="data">The global game data container.</param>
    void LoadData(GameData data);

    /// <summary>
    /// Called right before saving the game. The script should update the data container with its current state.
    /// </summary>
    /// <param name="data">The global game data container to be modified.</param>
    void SaveData(ref GameData data);
}