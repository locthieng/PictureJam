using System;
using UnityEngine;

public enum BoosterType
{
    Clock = 10,
    Hammer = 20,
    IceClock = 30,
    Scissors = 40,
    Magnet = 50,
    Glove = 60
}

[Serializable]
public class BoosterItemData
{
    public BoosterType Type;
    public string Name;
    public string Description;
    public int PriceInCoin;
    public int UnlockLevel;
    public int GiveAwayCount;
    public Sprite AvatarSprite;
    public string NameAnimIdle;
    public string NameAnimAction;
    public bool Loop;
    public bool IsIntroduced;
    public string tutorialText;
}

public class BoosterController : MonoBehaviour
{
    [SerializeField] public BoosterItem[] boosterItems;
    //public BoosterItemData[] boostersData = new BoosterItemData[Utility.GetEnumLength<BoosterType>()];
    public BoosterItemData[] boostersData;
    private bool usingHammer = false;
    public Transform dialogRoot;
    public int TotalBoostersUsedInLevel;

    public bool UsingHammer { get => usingHammer; set => usingHammer = value; }

    public static BoosterController instance;

    public static BoosterController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<BoosterController>();
            }
            return instance;
        }
    }

    private void Start()
    {
        int i = 0;
        foreach (BoosterType type in Enum.GetValues(typeof(BoosterType)))
        {
            boosterItems[i].SetUp(type);
            i++;
            if (i >= boosterItems.Length)
                break;
        }

        //CheckUnlockBooster();
    }

    public void UseBooster(BoosterType booster)
    {
        TotalBoostersUsedInLevel++;

        // Chặn tương tác với UI booster
        GameUIController.Instance.SetInteractBoosterUI(false);

        switch (booster)
        {
            case BoosterType.Clock:
                //Gọi hàm xử lý
                break;
            case BoosterType.Hammer:
                //Gọi hàm xử lý
                break;
            case BoosterType.IceClock:
                usingHammer = true;
                //GameUIController.Instance.SetTextForBoosterHammer(true);
                //GameUIController.Instance.SetAlphaBoosterUI(false);
                break;
            case BoosterType.Scissors:
                //Gọi hàm xử lý
                break;
            case BoosterType.Magnet:
                //Gọi hàm xử lý
                break;
            case BoosterType.Glove:
                //Gọi hàm xử lý
                break;
            default:
                break;
        }

        Debug.Log("Use booster");
    }

    public void SetInteractBooster(BoosterType booster, bool active)
    {
        foreach (var item in boosterItems)
        {
            if (item.boosterType == booster)
            {
                item.SetInteractBooster(active);
                break;
            }
        }
    }

    private void CheckUnlockBooster(int currentLevel)
    {
        for (int i = 0; i < boostersData.Length; i++)
        {
            if (currentLevel == boostersData[i].UnlockLevel)
            {
                // Hiển thị popup mở khóa booster
                //GameUIController.Instance.ShowBoosterUnlockPopup(boostersData[i]);
                break;
            }
        }
    }

}
