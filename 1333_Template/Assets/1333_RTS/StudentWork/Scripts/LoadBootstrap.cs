using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadBootstrap : MonoBehaviour
{
    [SerializeField] private AudioSource Music;
    [SerializeField] private AudioClip BackgroundMusic;
    private void Start()
    {
        if (!SaveManager.TryGetPending(out GameSaveData d)) return;

        var waveMgr = FindObjectOfType<EnemyWaveManager>();
        var resMgr = FindObjectOfType<ResourceManager>();
        var scoreMgr = FindObjectOfType<ScoreManager>();
        var laneMgr = FindObjectOfType<LaneManager>();
        var audioUI = FindObjectOfType<AudioSettingUI>();

        waveMgr.LoadWave(d.CurrentWave, d.WaveTimer);
        resMgr.SetResources(d.PlayerResources);
        scoreMgr.SetScore(d.Score);

        audioUI.SetSliders(d.MusicVolume, d.SFXVolume);

        foreach(var s in d.Units)
        {
            UnitType type = 
                System.Array.Find(UnitDatabase.Instance.PlayerUnits, t => t.UnitName == s.UnitTypeName)
                ?? System.Array.Find(UnitDatabase.Instance.EnemyUnits, t => t.UnitName == s.UnitTypeName);


            if(type == null)
            {
                Debug.LogWarning($"UnitType {s.UnitTypeName} missing - skipped");
                continue;
            }


            float yRot = (s.Team == GameSaveData.Team.Player) ? 90f : -90f;
            Quaternion facing = Quaternion.Euler(0, yRot, 0);
            UnitInstance u = Instantiate(type.Prefab, s.Position, facing);
            UnitTeam teamEnum = (s.Team == GameSaveData.Team.Player) ? UnitTeam.Player : UnitTeam.Enemy;
            u.Initialize(type, teamEnum);

            int laneIndex = Mathf.Clamp(Mathf.RoundToInt(s.Position.z), 0, 8);
            laneMgr.RegisterUnit(laneIndex, u);
        }
        Debug.Log("Save file loaded and applied");


        /*Music.loop = true;
        Music.Play();
        Debug.Log("Music is Playing");*/
        AudioManager.Instance.PlayMusic(BackgroundMusic);



    }
}
