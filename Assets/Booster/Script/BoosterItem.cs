using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using UnityEngine.UI;

public class BoosterItem : MonoBehaviour
{
    [SerializeField] public BoosterType boosterType;
    [SerializeField] private CanvasGroup boosterButton;
    [Header("Lock Cover")]
    [SerializeField] private GameObject lockCover;
    [SerializeField] private TextMeshProUGUI levelUnlock;
    [Header("Unlock")]
    [SerializeField] private GameObject unlockCover;
    [SerializeField] private Image avatar;
    [SerializeField] private Image amountIcon;
    [SerializeField] private TextMeshProUGUI amountTxt;

    private BoosterItemData data;
    private bool tutorialItem = false;

    private bool IsLock
    {
        get
        {
            //return false;
            if (BoosterController.Instance == null) return true;
            if (SceneManager.GetActiveScene().name == "LevelEditor")
            {
                return false;
            }
            //return BoosterController.Instance.boostersData[(int)boosterType].UnlockLevel > DataController.Instance.Data.LevelIndex;
            return data.UnlockLevel > 5;
        }
    }

    private int Amount
    {
        get
        {
            if (BoosterDataController.Instance == null)
            {
                return 0;
            }
            return BoosterDataController.Instance.Data.GetCount(boosterType) > 99 ? 99 : BoosterDataController.Instance.Data.GetCount(boosterType);
        }

    }

    public void SetUp(BoosterType type)
    {
        boosterType = type;
        
        for (int i = 0; i < BoosterController.Instance.boostersData.Length; i++)
        {
            var boosterData = BoosterController.Instance.boostersData[i];
            if (boosterData.Type == type)
            {
                data = boosterData;
                break;
            }
        }
        Refresh();
    }

    public void Refresh(bool tutorialItem = false)
    {
        if (tutorialItem)
        {
            this.tutorialItem = true;
            //avatar.sprite = data.AvatarSprite;
            unlockCover.gameObject.SetActive(true);
            lockCover.SetActive(false);
            //txtUnlock.gameObject.SetActive(false);
            amountIcon.gameObject.SetActive(true);
            if (BoosterDataController.Instance.Data.IsIntroduced(boosterType))
            {
                amountTxt.text = (BoosterDataController.Instance.Data.GetCount(boosterType) + 1).ToString();
            }
            else
            {
                amountTxt.text = (BoosterDataController.Instance.Data.GetCount(boosterType) + 2).ToString();
                BoosterDataController.Instance.Data.AddBooster(boosterType);
                BoosterDataController.Instance.Data.SetIntroduced(boosterType);
            }
            return;
        }
        avatar.sprite = data.AvatarSprite;
        avatar.SetNativeSize();
        unlockCover.gameObject.SetActive(!IsLock);
        lockCover.SetActive(IsLock);
        levelUnlock.gameObject.SetActive(IsLock);
        levelUnlock.text = "Level " + data.UnlockLevel;
        amountIcon.gameObject.SetActive(Amount > 0);
        amountTxt.text = Amount.ToString();
    }


    public void TapOnBoosterButton()
    {
        if (tutorialItem)
        {
            BoosterController.Instance.UseBooster(boosterType);
            //GameUIController.Instance.HideBoosterUnlockPopup();
            //TutorialController.Instance?.HideHand();
            gameObject.SetActive(false);
            return;
        }
        if (IsLock)
        {
            //GameUIController.Instance.ShowDialog(true, "Unlock at level " + data.UnlockLevel, Vector2.zero, BoosterController.Instance.dialogRoot);
            Debug.Log("Lock");
            return;
        }
        switch (boosterType)
        {
            case BoosterType.Clock:
                {
                    Debug.Log("clock");
                    // Kiểm tra điều kiện sử dụng booster
                    StageController.Instance.isBonusTime = true;
                    break;
                }
            case BoosterType.Hammer:
                {

                    break;
                }
            case BoosterType.IceClock:
                {
                    Debug.Log("IceClock");
                    GameUIController.Instance.UseButtonIceClock();
                    break;
                }
            case BoosterType.Scissors:
                {
                    // Kiểm tra điều kiện sử dụng booster
                    break;
                }
            case BoosterType.Magnet:
                {
                    // Kiểm tra điều kiện sử dụng booster
                    break;
                }
            case BoosterType.Glove:
                {
                    // Kiểm tra điều kiện sử dụng booster
                    break;
                }
        }
        if (BoosterDataController.Instance.Data.GetCount(boosterType) > 0)
        {
            BoosterDataController.Instance.Data.RemoveBooster(boosterType);
            BoosterController.Instance.UseBooster(boosterType);
            // GameUIController.Instance.SetInteractBoosterUI(false);
            Refresh();
        }
        else
        {
            Debug.Log("Show Popup");
            //Show popup
            //GameUIController.Instance.ShowBoosterPopup(data);
        }
    }

    public void SetInteractBooster(bool active)
    {
        if (active)
        {
            /*boosterButton.LeanAlpha(1, 0.3f).setOnComplete(() =>
            {
                boosterButton.blocksRaycasts = true;
            });*/
            boosterButton.blocksRaycasts = true;
        }
        else
        {
            boosterButton.blocksRaycasts = false;
            //boosterButton.LeanAlpha(0.5f, 0.3f);
        }
    }

}
