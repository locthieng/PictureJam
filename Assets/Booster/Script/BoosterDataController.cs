using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BoosterData : ISerializationCallbackReceiver //
{
    // List chỉ dùng để Save/Load, lúc code logic thì kệ xác nó
    [SerializeField] private List<BoosterType> _keys = new List<BoosterType>();
    [SerializeField] private List<int> _values = new List<int>();

    // Runtime dùng cái này cho nhanh
    // NonSerialized để JsonUtility không cố lưu cái này (gây lỗi)
    [NonSerialized]
    public Dictionary<BoosterType, int> BoosterDic = new Dictionary<BoosterType, int>();

    // --- LOGIC GAMEPLAY (Chỉ quan tâm Dictionary) ---

    public int GetCount(BoosterType type)
    {
        return BoosterDic.TryGetValue(type, out int count) ? count : 0;
    }

    public void AddBooster(BoosterType type, int amount = 1)
    {
        if (BoosterDic.ContainsKey(type))
            BoosterDic[type] += amount;
        else
            BoosterDic[type] = amount;

        // KHÔNG CẦN update List ở đây. Tự động làm lúc Save.
    }

    public void RemoveBooster(BoosterType type, int amount = 1)
    {
        if (BoosterDic.ContainsKey(type))
        {
            BoosterDic[type] = Mathf.Max(BoosterDic[type] - amount, 0);
        }
    }

    // --- LOGIC SAVE/LOAD RIÊNG ---

    // PlayerPrefs logic nên tách ra, nhưng mày để đây cũng tạm được
    const string IntroducedKeyPrefix = "Booster_Intro_";
    public bool IsIntroduced(BoosterType type) => PlayerPrefs.GetInt(IntroducedKeyPrefix + (int)type, 0) == 1;

    public void SetIntroduced(BoosterType type)
    {
        PlayerPrefs.SetInt(IntroducedKeyPrefix + (int)type, 1);
        // Bỏ PlayerPrefs.Save() đi, đừng spam ổ cứng
    }

    // --- MAGIC CỦA UNITY (ISerializationCallbackReceiver) ---

    // Hàm này Unity tự gọi TRƯỚC khi ToJson
    public void OnBeforeSerialize()
    {
        _keys.Clear();
        _values.Clear();
        foreach (var kvp in BoosterDic)
        {
            _keys.Add(kvp.Key);
            _values.Add(kvp.Value);
        }
    }

    // Hàm này Unity tự gọi SAU khi FromJson
    public void OnAfterDeserialize()
    {
        BoosterDic = new Dictionary<BoosterType, int>();
        for (int i = 0; i != Math.Min(_keys.Count, _values.Count); i++)
        {
            // Tránh lỗi duplicate key nếu file save bị ngu
            if (!BoosterDic.ContainsKey(_keys[i]))
                BoosterDic.Add(_keys[i], _values[i]);
        }
    }
}

public class BoosterDataController : MonoBehaviour
{
    public static BoosterDataController Instance;
    private const string BoosterDataKey = "BoosterData";
    public BoosterData Data; // Đặt tên ngắn gọn thôi

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        LoadData();
    }

    private void LoadData()
    {
        string json = PlayerPrefs.GetString(BoosterDataKey, "");

        if (string.IsNullOrEmpty(json))
        {
            // New User: Tạo data mặc định
            Data = new BoosterData();
            foreach (BoosterType type in Enum.GetValues(typeof(BoosterType)))
            {
                Data.BoosterDic.Add(type, 0);
            }
        }
        else
        {
            // Old User: Load từ Json
            // Lúc này hàm OnAfterDeserialize tự chạy để fill Dictionary
            Data = JsonUtility.FromJson<BoosterData>(json);
        }
    }

    public void SaveData()
    {
        // Lúc này hàm OnBeforeSerialize tự chạy để fill List từ Dictionary
        string json = JsonUtility.ToJson(Data);
        PlayerPrefs.SetString(BoosterDataKey, json);
        PlayerPrefs.Save(); // Save ở đây là hợp lý
        Debug.Log("Saved: " + json);
        foreach (var kvp in Data.BoosterDic)
        {
            Debug.Log(kvp.Key + ": " + kvp.Value);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            // Code test gọn gàng
            Data.AddBooster(BoosterType.Hammer, 1);
            Data.AddBooster(BoosterType.Magnet, 1);
            Data.AddBooster(BoosterType.AddSlot, 1);
            Data.AddBooster(BoosterType.ChangeOrder, 1);
            SaveData();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("Deleted All Save");
            // Reset lại data để test
            LoadData();
        }
    }
}