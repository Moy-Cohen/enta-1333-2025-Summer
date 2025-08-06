using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
public class GameSaveData
{
    public string SaveTimeStamp;

    /* --- Progress --- */
    public int CurrentWave;
    public float WaveTimer;
    public int PlayerResources;
    public int Score;

    /* --- Audio ---*/
    public float MusicVolume = 1f;
    public float SFXVolume = 1f;

    /*--- Active Units ---*/
    public List<UnitSnapshot> Units = new();

    [Serializable]
    public struct UnitSnapshot
    {
        public string UnitTypeName;
        public Team Team;
        public Vector3 Position;
        public int CurrentHP;
    }

    public enum Team
    {
        Player,
        Enemy
    }
}