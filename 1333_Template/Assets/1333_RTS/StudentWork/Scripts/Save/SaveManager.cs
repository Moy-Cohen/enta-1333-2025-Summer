using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class SaveManager
{
    private const string FILE_NAME = "save.json";
    private static GameSaveData pendingLoad;

    /* ----- Public API ----- */

    public static void SaveGame(GameContext ctx)
    {
        var data = BuildData(ctx);

        string json = JsonUtility.ToJson(data, prettyPrint: true);
        string path = Path.Combine(Application.persistentDataPath, FILE_NAME);
        File.WriteAllText(path, json);

        Debug.Log($"Game saved --> {path}");
    }

    public static bool SaveExists()
    {
        string path = Path.Combine(Application.persistentDataPath,FILE_NAME);
        return File.Exists(path);
    }

    public static void DeleteSave()
    {
        string path = Path.Combine(Application.persistentDataPath, FILE_NAME);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    public static void LoadGame()
    {
        string path = Path.Combine(Application.persistentDataPath, FILE_NAME);
        if (!File.Exists(path))
        {
            Debug.LogWarning("No save file found."); 
            return;
        }

        string json = File.ReadAllText(path);
        pendingLoad = JsonUtility.FromJson<GameSaveData>(json);
    }

    public static bool TryGetPending(out GameSaveData data)
    {
        data = pendingLoad;
        bool ok = pendingLoad != null;
        pendingLoad = null;
        return ok;
    }

    private static GameSaveData BuildData(GameContext c)
    {
        if(c.LaneManager == null)
        {
            Debug.LogError("SaveManager: LaneManager reference is NULL!");
        }
        var d = new GameSaveData
        {
            SaveTimeStamp = System.DateTime.Now.ToString("o"),
            CurrentWave = c.WaveManager.CurrentWave,
            WaveTimer = c.WaveManager.CurrentWaveTimer,
            PlayerResources = c.ResourceManager.ResourceAmmount,
            Score = c.ScoreManager.Score,
            MusicVolume = c.AudioUI.MusicSlider.value,
            SFXVolume = c.AudioUI.SFXSlider.value
        }; 

        foreach(UnitInstance u in c.LaneManager.GetAllUnits())
        {
            
            d.Units.Add(new GameSaveData.UnitSnapshot
            {
                UnitTypeName = u.UnitType.UnitName,
                Team = u.Team == UnitTeam.Player ? GameSaveData.Team.Player : GameSaveData.Team.Enemy,
                Position = u.transform.position,
            });
        }

        return d;
    }
}
