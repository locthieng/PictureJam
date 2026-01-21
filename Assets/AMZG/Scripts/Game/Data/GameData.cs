using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData
{
    public int ChestKey;
    //public int Coin;
    public int BestScore;
    public int LevelCycleCount;
    public int LevelIndex;
    public List<int> Levels;
    public int SkinID;
    public int WeaponID;
    public int LastUnlockItemByLevel;
    public List<int> SkinIDs;
    public List<int> SkinUnlockProgress;
    public List<int> WeaponIDs;
    public List<int> WeaponUnlockProgress;
    public List<int> DecoItemInUseIDs;
    public List<string> DecoItemIDs;
}

[Serializable]
public class GameStats
{
    public bool NoAds;
    public int GamePlayCount;
    public int TotalScore;
}
