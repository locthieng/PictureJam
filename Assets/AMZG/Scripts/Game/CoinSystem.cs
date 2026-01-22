using System;
using UnityEngine;

public class CoinSystem : MonoBehaviour
{
    public static CoinSystem Instance { get; set; }

    public event Action<int> OnCoinChanged;

    private const string COIN_KEY = "PLAYER_COIN";

    private int _coin;
    public int Coin
    {
        get => _coin;
        set => _coin = value;
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadCoin();
    }

    #region Load / Save

    private void LoadCoin()
    {
        _coin = PlayerPrefs.GetInt(COIN_KEY, 0);
        Notify();
    }

    private void SaveCoin()
    {
        PlayerPrefs.SetInt(COIN_KEY, _coin);
        PlayerPrefs.Save();
    }

    #endregion

    #region Public API

    public void AddCoin(int amount)
    {
        if (amount <= 0) return;

        _coin += amount;
        SaveCoin();
        Notify();
    }

    public bool SpendCoin(int amount)
    {
        if (amount <= 0) return true;
        if (_coin < amount) return false;

        _coin -= amount;
        SaveCoin();
        Notify();
        return true;
    }

    public void SetCoin(int value)
    {
        _coin = Mathf.Max(0, value);
        SaveCoin();
        Notify();
    }

    public bool HasEnough(int amount)
    {
        return _coin >= amount;
    }

    #endregion

    private void Notify()
    {
        OnCoinChanged?.Invoke(_coin);
    }
}