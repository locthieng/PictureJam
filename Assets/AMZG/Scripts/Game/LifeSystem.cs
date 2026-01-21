using System;
using UnityEngine;

public class LifeSystem : MonoBehaviour
{
    public static LifeSystem Instance { get; private set; }

    public event Action<int> OnLifeChanged;

    private const string LIFE_KEY = "PLAYER_LIFE";
    private const string LIFE_TIME_KEY = "PLAYER_LIFE_TIME";

    // Key để lưu vào máy
    private const string INFINITE_LIFE_EXPIRE_KEY = "PLAYER_INFINITE_LIFE_EXPIRE";

    // Biến lưu thời điểm hết hạn mạng vô hạn
    private DateTime infiniteExpireTime;

    // Thuộc tính để kiểm tra xem hiện tại có đang vô hạn mạng không
    public bool IsInfinite => DateTime.Now < infiniteExpireTime;

    [SerializeField] private int maxLife = 5;
    [SerializeField] private int refillSeconds = 300; // 5 phút

    private int currentLife;
    private DateTime lastLifeTime;

    public int CurrentLife => currentLife;
    public int MaxLife => maxLife;
    public int RefillSeconds => refillSeconds;
    public bool IsFull => currentLife >= maxLife;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Load();
    }

    #region Load / Save

    private void Load()
    {
        currentLife = PlayerPrefs.GetInt(LIFE_KEY, maxLife);

        string timeStr = PlayerPrefs.GetString(LIFE_TIME_KEY, string.Empty);
        if (!string.IsNullOrEmpty(timeStr))
            lastLifeTime = DateTime.Parse(timeStr);
        else
            lastLifeTime = DateTime.Now;

        string expireTimeStr = PlayerPrefs.GetString(INFINITE_LIFE_EXPIRE_KEY, string.Empty);
        if (!string.IsNullOrEmpty(expireTimeStr))
            infiniteExpireTime = DateTime.Parse(expireTimeStr);
        else
            infiniteExpireTime = DateTime.MinValue; // Mặc định là không có vô hạn

        Notify();
    }

    private void Save()
    {
        PlayerPrefs.SetInt(LIFE_KEY, currentLife);
        PlayerPrefs.SetString(LIFE_TIME_KEY, lastLifeTime.ToString());
        PlayerPrefs.SetString(INFINITE_LIFE_EXPIRE_KEY, infiniteExpireTime.ToString());
        PlayerPrefs.Save();
    }

    #endregion

    #region Public API (CH? G?I QUA ?ÂY)

    /// <summary>
    /// Tr? life (ví d?: vào level)
    /// </summary>
    public bool ConsumeLife(int amount = 1)
    {
        if (IsInfinite) return true; // Đang vô hạn thì không trừ mạng, cho đi tiếp luôn!
        if (currentLife < amount)
            return false;

        currentLife -= amount;

        if (currentLife < maxLife)
            lastLifeTime = DateTime.Now;

        Save();
        Notify();
        return true;
    }

    /// <summary>
    /// C?ng life (t? timer / ads / reward)
    /// </summary>
    public void AddLife(int amount = 1)
    {
        int before = currentLife;
        currentLife = Mathf.Min(currentLife + amount, maxLife);

        if (before < maxLife && currentLife >= maxLife)
        {
            // full ? reset time
            lastLifeTime = DateTime.Now;
        }

        Save();
        Notify();
    }

    #endregion

    #region Time Logic (?? UI H?I)

    public TimeSpan GetRemainTime()
    {
        if (IsFull)
            return TimeSpan.Zero;

        TimeSpan passed = DateTime.Now - lastLifeTime;
        double remain = refillSeconds - passed.TotalSeconds;
        return TimeSpan.FromSeconds(Mathf.Max(0, (float)remain));
    }

    #endregion

    private void Notify()
    {
        OnLifeChanged?.Invoke(currentLife);
    }

    public void ActivateInfiniteLife(int durationSeconds)
    {
        if (IsInfinite)
        {
            // Nếu đang vô hạn rồi thì cộng dồn thêm thời gian
            infiniteExpireTime = infiniteExpireTime.AddSeconds(durationSeconds);
        }
        else
        {
            // Nếu chưa có thì bắt đầu tính từ bây giờ
            infiniteExpireTime = DateTime.Now.AddSeconds(durationSeconds);
        }
        Save();
        Notify(); // Thông báo cho UI cập nhật (hiện icon vô cực)
    }
}